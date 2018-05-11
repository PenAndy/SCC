using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Quartz;
using SCC.Common;
using SCC.Crawler.Tools;
using SCC.Interface;
using SCC.Models;

namespace SCC.Crawler.DT
{
    /// <summary>
    ///     数据爬取类
    ///     华东15选5
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class HD15X5Job : IJob
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        public HD15X5Job()
        {
            log = new LogHelper();
            services = IOC.Resolve<IDTOpenCode>();
            email = IOC.Resolve<IEmail>();
        }

        /// <summary>
        ///     作业执行入口
        /// </summary>
        /// <param name="context">作业执行上下文</param>
        public void Execute(IJobExecutionContext context)
        {
            Config = CommonHelper.GetConfigFromDataMap(context.JobDetail.JobDataMap);

            //预设节假日不开奖
            if (Config.SkipDate.Contains(CommonHelper.SCCSysDateTime.ToString("yyyyMMdd"))) return;
            LatestItem = context.JobDetail.JobDataMap["LatestItem"] as OpenCode5DTModel;
            try
            {
                //服务启动时配置初始数据
                if (LatestItem == null)
                {
                    LatestItem = services.GetOpenCode5DTLastItem(currentLottery);
                    if (LatestItem == null)
                        LatestItem = new OpenCode5DTModel
                        {
                            Term = CommonHelper.GenerateQiHaoYYYYQQQ(0),
                            OpenTime = new DateTime(CommonHelper.SCCSysDateTime.Year, 1, 1)
                        };
                }

                //程序时间第二天，程序根据配置检查是否昨天有开奖
                isGetData = false;
                if (CommonHelper.CheckDTIsNeedGetData(Config))
                {
                    DoMainUrl();
                    DoBackUrl();
                }

                if (!LatestItem.Term.ToString().StartsWith(CommonHelper.SCCSysDateTime.ToString("yyyy")))
                    LatestItem = new OpenCode5DTModel
                    {
                        Term = CommonHelper.GenerateQiHaoYYYYQQQ(0),
                        OpenTime = new DateTime(CommonHelper.SCCSysDateTime.Year, 1, 1)
                    };
                //当今日开奖并且当前时间是晚上8点过后开始抓取
                if (CommonHelper.CheckTodayIsOpenDay(Config) && CommonHelper.SCCSysDateTime.Hour > 12)
                {
                    DoMainUrl();
                    DoBackUrl();
                }
            }
            catch (Exception ex)
            {
                log.Error(GetType(), string.Format("【{0}】抓取时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            //保存最新期号
            context.JobDetail.JobDataMap["LatestItem"] = LatestItem;
        }

        /// <summary>
        ///     通过主站点爬取开奖数据
        ///     (江苏福彩网)
        /// </summary>
        private void DoMainUrl()
        {
            if (!string.IsNullOrEmpty(Config.MainUrl))
            {
                var OpenList = GetOpenListFromMainUrl(Config.MainUrl);
                if (OpenList.Count == 0) return; //无抓取数据
                var newestQiHao = OpenList.Max(w => w.Term).ToString();
                var startQiNum = Convert.ToInt32(LatestItem.Term.ToString().Substring(4)) + 1;
                var newestQiNum = Convert.ToInt32(newestQiHao.Substring(4));
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
                var getQiHao = string.Empty;
                OpenCode5DTModel matchItem = null;
                for (var i = startQiNum; i <= newestQiNum; i++)
                {
                    getQiHao = LatestItem.Term.ToString().Substring(0, 4) + i.ToString().PadLeft(3, '0');
                    matchItem = OpenList.Where(R => R.Term.ToString() == getQiHao).FirstOrDefault();
                    if (matchItem != null && OptimizeMainModel(ref matchItem) &&
                        services.AddDTOpen5Code(currentLottery, matchItem))
                    {
                        //Do Success Log
                        log.Info(GetType(), CommonHelper.GetJobMainLogInfo(Config, getQiHao));
                        LatestItem = matchItem;
                        isGetData = true;
                    }
                }
            }
        }

        /// <summary>
        ///     获取主站开奖列表数据
        /// </summary>
        /// <param name="mainUrl">主站地址</param>
        /// <returns></returns>
        private List<OpenCode5DTModel> GetOpenListFromMainUrl(string mainUrl)
        {
            var result = new List<OpenCode5DTModel>();
            try
            {
                var postData = "PlayType=5&currentPage=1&pageSize=200";
                var htmlResource = NetHelper.GetUrlResponse(mainUrl, "POST", postData, Encoding.UTF8);
                if (htmlResource == null) return result;


                var obj = htmlResource.JsonToEntity<JObject>();
                var lotteryList = obj["LotteryNumberList"];
                OpenCode5DTModel model = null;
                string[] openCodeList = null;
                var issue = string.Empty;
                foreach (var item in lotteryList)
                {
                    issue = item["Issue"].ToString();
                    if (issue.StartsWith((CommonHelper.SCCSysDateTime.Year - 1).ToString())) break; //只抓取今年数据
                    model = new OpenCode5DTModel();
                    model.Term = Convert.ToInt64(issue);
                    model.OpenTime = Convert.ToDateTime(item["LotteryDate"].ToString());
                    openCodeList = item["BasicNumber"].ToString().Trim().Replace("[", string.Empty)
                        .Replace("]", string.Empty).Replace("\r\n", string.Empty).Replace("\"", string.Empty)
                        .Split(',');
                    if (openCodeList.Length != 5) continue;
                    model.OpenCode1 = Convert.ToInt32(openCodeList[0]);
                    model.OpenCode2 = Convert.ToInt32(openCodeList[1]);
                    model.OpenCode3 = Convert.ToInt32(openCodeList[2]);
                    model.OpenCode4 = Convert.ToInt32(openCodeList[3]);
                    model.OpenCode5 = Convert.ToInt32(openCodeList[4]);
                    model.Spare = string.IsNullOrEmpty(item["PrizePool"].ToString().Replace("万元", string.Empty))
                        ? "0"
                        : item["PrizePool"].ToString().Replace("万元", string.Empty);
                    result.Add(model);
                }

                var checkDataHelper = new CheckDataHelper();
                var dbdata = services.GetListS<OpenCode5DTModel>(currentLottery)
                    .ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr());
                checkDataHelper.CheckData(dbdata, result.ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr()),
                    Config.Area, currentLottery);
                result = result.OrderByDescending(S => S.Term).ToList();
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过主站点抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            return result;
        }

        /// <summary>
        ///     完善备用站点江苏体彩7位数开奖实体信息
        /// </summary>
        /// <param name="model"></param>
        private bool OptimizeBackModel(ref OpenCode5DTModel model, HtmlNode tr)
        {
            try
            {
                var entity = new KaijiangDetailsEntity();
                entity.KaiJiangItems = new List<Kaijiangitem>();
                var tds = tr.ChildNodes.Where(w => w.Name == "td").ToList();
                var xiaoshoue = tds[2].InnerText.Replace(",", "").Trim();
                var jiangchi = "0";
                var tedengjiang = new Kaijiangitem
                {
                    Name = "特等奖",
                    Total = tds[3].InnerText.Trim(),
                    TotalMoney = tds[4].InnerText.Trim()
                };
                entity.KaiJiangItems.Add(tedengjiang);
                var yidengjiang = new Kaijiangitem
                {
                    Name = "一等奖",
                    Total = tds[5].InnerText.Trim(),
                    TotalMoney = tds[6].InnerText.Trim()
                };
                entity.KaiJiangItems.Add(yidengjiang);
                var erdengjiang = new Kaijiangitem
                {
                    Name = "二等奖",
                    Total = tds[7].InnerText.Trim(),
                    TotalMoney = tds[8].InnerText.Trim()
                };
                entity.KaiJiangItems.Add(erdengjiang);
                entity.Trje = xiaoshoue;
                entity.Gdje = jiangchi;
                model.Spare = entity.TryToJson();
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过备用站点优化开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            return false;
        }

        /// <summary>
        ///     完善主站江苏体彩7位数开奖详情信息
        /// </summary>
        /// <param name="model"></param>
        private bool OptimizeMainModel(ref OpenCode5DTModel model)
        {
            var url = string.Format("http://www.jslottery.com/Lottery/LotteryDetails?PlayType=5&Issue={0}", model.Term);
            try
            {
                var entity = new KaijiangDetailsEntity();
                entity.KaiJiangItems = new List<Kaijiangitem>();

                var htmlResource = NetHelper.GetUrlResponse(url);


                if (!string.IsNullOrEmpty(htmlResource))
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(htmlResource);
                    var tables = doc.DocumentNode.SelectNodes("//table");
                    if (tables.Count < 5) return false;
                    var trs = tables[4].ChildNodes[0].ChildNodes.Where(S => S.Name.ToLower() == "tr").ToList();
                    foreach (var tr in trs)
                    {
                        var tds = tr.ChildNodes.Where(S => S.Name.ToLower() == "td").ToList();
                        if (tds.Count < 3) continue;
                        if (tds[0].InnerText == "特等奖")
                        {
                            var tmp = new Kaijiangitem
                            {
                                Name = "特等奖",
                                Total = tds[1].InnerText,
                                TotalMoney = tds[2].InnerText
                            };
                            entity.KaiJiangItems.Add(tmp);
                            // Level1Num = Convert.ToInt32(tds[1].InnerText);
                            // Level1Money = Convert.ToInt32(tds[2].InnerText);
                        }
                        else if (tds[0].InnerText == "一等奖")
                        {
                            var tmp = new Kaijiangitem
                            {
                                Name = "一等奖",
                                Total = tds[1].InnerText,
                                TotalMoney = tds[2].InnerText
                            };
                            entity.KaiJiangItems.Add(tmp);
                        }
                        else if (tds[0].InnerText == "二等奖")
                        {
                            var tmp = new Kaijiangitem
                            {
                                Name = "二等奖",
                                Total = tds[1].InnerText,
                                TotalMoney = tds[2].InnerText
                            };
                            entity.KaiJiangItems.Add(tmp);
                        }
                    }

                    var reg = new Regex(@"本期投注总额<span>([\d.,]*?)</span>");
                    var match = reg.Match(htmlResource);
                    if (match.Success)
                    {
                        entity.Trje = (Convert.ToDecimal(match.Result("$1").Replace(",", string.Empty)) * 10000)
                            .ToString();
                    }
                    else
                    {
                        reg = new Regex(@"本期投注总额<span lang=""EN-US"">([\d.,]*?)</span>");
                        match = reg.Match(htmlResource);
                        if (match.Success)
                            entity.Trje = (Convert.ToDecimal(match.Result("$1").Replace(",", string.Empty)) * 10000)
                                .ToString();
                    }

                    entity.Gdje = (Convert.ToDecimal(model.Spare.Replace(",", string.Empty)) * 10000).ToString();
                    model.Spare = entity.TryToJson();
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过主站点完善抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            return false;
        }

        /// <summary>
        ///     通过备用站点抓取开奖数据
        ///     (百度乐彩)
        /// </summary>
        private void DoBackUrl()
        {
            if (!string.IsNullOrEmpty(Config.BackUrl))
            {
                var OpenList = GetOpenListFromBackUrl();
                if (OpenList.Count == 0) return; //无抓取数据
                var newestQiHao = OpenList.First().Term.ToString();
                var startQiNum = Convert.ToInt32(LatestItem.Term.ToString().Substring(4)) + 1;
                var newestQiNum = Convert.ToInt32(newestQiHao.Substring(4));
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
                var getQiHao = string.Empty;
                OpenCode5DTModel matchItem = null;
                for (var i = startQiNum; i <= newestQiNum; i++)
                {
                    getQiHao = LatestItem.Term.ToString().Substring(0, 4) + i.ToString().PadLeft(3, '0');
                    matchItem = OpenList.Where(R => R.Term.ToString() == getQiHao).FirstOrDefault();
                    if (matchItem != null && services.AddDTOpen5Code(currentLottery, matchItem))
                    {
                        //Do Success Log
                        log.Info(GetType(), CommonHelper.GetJobBackLogInfo(Config, getQiHao));
                        LatestItem = matchItem;
                        isGetData = true;
                    }
                }
            }
        }


        /// <summary>
        ///     获取备用站点开奖列表数据
        /// </summary>
        /// <param name="backUrl">备用站点</param>
        /// <returns></returns>
        private List<OpenCode5DTModel> GetOpenListFromBackUrl()
        {
            var result = new List<OpenCode5DTModel>();
            try
            {
                var htmlResource = NetHelper.GetUrlResponse(Config.BackUrl);
                if (htmlResource == null) return result;

                var doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                var table = doc.DocumentNode.SelectSingleNode("//table");
                if (table == null) return result;
                var trs = table.ChildNodes.Where(node => node.Name == "tr").ToList();
                trs.RemoveAll(w => w.GetAttributeValue("class", "") != "");
                foreach (var item in trs)
                {
                    var tds = item.ChildNodes.Where(w => w.Name == "td").ToList();
                    ;
                    var qihao = tds[0].InnerText.Trim();
                    var kaijianghao = tds[1].InnerText.Trim();
                    var opentime = tds[9].InnerText.Trim();
                    var tmp = new OpenCode5DTModel
                    {
                        Term = long.Parse(qihao),
                        OpenTime = DateTime.Parse(opentime)
                    };
                    tmp.OpenCode1 = int.Parse(kaijianghao.Substring(0, 2));
                    tmp.OpenCode2 = int.Parse(kaijianghao.Substring(2, 2));
                    tmp.OpenCode3 = int.Parse(kaijianghao.Substring(4, 2));
                    tmp.OpenCode4 = int.Parse(kaijianghao.Substring(6, 2));
                    tmp.OpenCode5 = int.Parse(kaijianghao.Substring(8, 2));
                    OptimizeBackModel(ref tmp, item);
                    result.Add(tmp);
                }

                var checkDataHelper = new CheckDataHelper();
                var dbdata = services.GetListS<OpenCode5DTModel>(currentLottery)
                    .ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr());
                checkDataHelper.CheckData(dbdata, result.ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr()),
                    Config.Area, currentLottery);
                result = result.OrderByDescending(S => S.Term).ToList();
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过备用站点抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            return result;
        }

        #region Attribute

        /// <summary>
        ///     配置信息
        /// </summary>
        private SCCConfig Config;

        /// <summary>
        ///     当天抓取的最新一期开奖记录
        /// </summary>
        private OpenCode5DTModel LatestItem;

        /// <summary>
        ///     当天抓取失败列表
        /// </summary>
        private List<string> FailedQiHaoList = null;

        /// <summary>
        ///     日志对象
        /// </summary>
        private readonly LogHelper log;

        /// <summary>
        ///     数据服务
        /// </summary>
        private readonly IDTOpenCode services;

        /// <summary>
        ///     当前彩种
        /// </summary>
        private SCCLottery currentLottery => SCCLottery.HD15X5;

        /// <summary>
        ///     邮件接口
        /// </summary>
        private IEmail email;

        /// <summary>
        ///     是否本次运行抓取到开奖数据
        /// </summary>
        private bool isGetData;

        #endregion
    }
}