using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Quartz;
using SCC.Common;
using SCC.Crawler.Tools;
using SCC.Interface;
using SCC.Models;

namespace SCC.Crawler.DT
{
    /// <summary>
    ///     江苏体彩7位数
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class JSTC7WSJob : IJob
    {
        /// <summary>
        /// 初始化函数
        /// </summary>
        public JSTC7WSJob()
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
            LatestItem = context.JobDetail.JobDataMap["LatestItem"] as OpenCode7DTModel;
            try
            {
                //服务启动时配置初始数据
                if (LatestItem == null)
                {
                    LatestItem = services.GetOpenCode7DTLastItem(currentLottery);
                    if (LatestItem == null)
                        LatestItem = new OpenCode7DTModel
                        {
                            Term = CommonHelper.GenerateQiHaoYYQQQ(0),
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

                if (!LatestItem.Term.ToString().StartsWith(CommonHelper.SCCSysDateTime.ToString("yy")))
                    LatestItem = new OpenCode7DTModel
                    {
                        Term = CommonHelper.GenerateQiHaoYYQQQ(0),
                        OpenTime = new DateTime(CommonHelper.SCCSysDateTime.Year, 1, 1)
                    };
                //当今日开奖并且当前时间是晚上8点过后开始抓取
                if (CommonHelper.CheckTodayIsOpenDay(Config) && CommonHelper.SCCSysDateTime.Hour > 12)
                {
                    DoMainUrl();
                    DoBackUrl();
                }

                // 
                // TrendChartHelper.GenerateJSTC7WSTrendChart(log);
            }
            catch (Exception ex)
            {
                log.Error(typeof(JSTC7WSJob),
                    string.Format("【{0}】抓取时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            //保存最新期号
            context.JobDetail.JobDataMap["LatestItem"] = LatestItem;
        }

        /// <summary>
        ///     通过主站点爬取开奖数据
        ///     (江苏体彩网)
        /// </summary>
        private void DoMainUrl()
        {
            if (!string.IsNullOrEmpty(Config.MainUrl))
            {
                var OpenList = GetOpenListFromMainUrl(Config.MainUrl);
                if (OpenList.Count == 0) return; //无抓取数据
                var newestQiHao = OpenList.First().Term.ToString();
                var startQiNum = Convert.ToInt32(LatestItem.Term.ToString().Substring(2)) + 1;
                var newestQiNum = Convert.ToInt32(newestQiHao.Substring(2));
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
                var getQiHao = string.Empty;
                OpenCode7DTModel matchItem = null;
                for (var i = startQiNum; i <= newestQiNum; i++)
                {
                    getQiHao = LatestItem.Term.ToString().Substring(0, 2) + i.ToString().PadLeft(3, '0');
                    matchItem = OpenList.Where(R => R.Term.ToString() == getQiHao).FirstOrDefault();
                    if (matchItem != null && OptimizeMainModel(ref matchItem) &&
                        services.AddDTOpen7Code(currentLottery, matchItem))
                    {
                        //Do Success Log
                        log.Info(typeof(JSTC7WSJob), CommonHelper.GetJobMainLogInfo(Config, getQiHao));
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
        private List<OpenCode7DTModel> GetOpenListFromMainUrl(string mainUrl)
        {
            var result = new List<OpenCode7DTModel>();
            try
            {
                var pageIndex = 1;
                var htmlResource = string.Empty;
                var resourceUrl = new Uri(mainUrl);
                var isLoop = true;
                var lastYear = (DateTime.Now.Year - 1).ToString().Substring(2);
                var postData = "current_page={0}&all_count=0&num=";
                var OpenTime = string.Empty;
                while (isLoop)
                {
                    htmlResource = NetHelper.GetUrlResponse(resourceUrl.AbsoluteUri, "POST",
                        string.Format(postData, pageIndex), Encoding.UTF8);
                    var jsonData = htmlResource.JsonToEntity<dynamic>();
                    var dataList = jsonData["items"];
                    foreach (var data in dataList)
                    {
                        if (data["num"].Value.StartsWith(lastYear))
                        {
                            isLoop = false;
                            break;
                        }

                        OpenTime = data["date_publish"].Value.Insert(6, "-").Insert(4, "-");
                        result.Add(new OpenCode7DTModel
                        {
                            Term = Convert.ToInt32(data["num"].Value),
                            OpenCode1 = Convert.ToInt32(data["one"].Value),
                            OpenCode2 = Convert.ToInt32(data["two"].Value),
                            OpenCode3 = Convert.ToInt32(data["three"].Value),
                            OpenCode4 = Convert.ToInt32(data["four"].Value),
                            OpenCode5 = Convert.ToInt32(data["five"].Value),
                            OpenCode6 = Convert.ToInt32(data["six"].Value),
                            OpenCode7 = Convert.ToInt32(data["seven"].Value),
                            OpenTime = Convert.ToDateTime(OpenTime),
                            DetailUrl = string.Format(
                                "http://www.js-lottery.com/Article/news/group_id/3/article_id/{0}.html",
                                data["article_id"].Value)
                        });
                    }

                    pageIndex++;
                }

                var checkDataHelper = new CheckDataHelper();
                var dbdata = services.GetListS<OpenCode7DTModel>(currentLottery)
                    .ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr());
                checkDataHelper.CheckData(dbdata, result.ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr()),
                    Config.Area, currentLottery);
                result = result.OrderByDescending(S => S.Term).ToList();
            }
            catch (Exception ex)
            {
                log.Error(typeof(JSTC7WSJob),
                    string.Format("【{0}】通过主站点抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            return result;
        }

        /// <summary>
        ///     完善主站江苏体彩7位数开奖详情信息
        /// </summary>
        /// <param name="model"></param>
        private bool OptimizeBackModel(ref OpenCode7DTModel model, HtmlNode tr)
        {
            try
            {
                var entity = new KaijiangDetailsEntity();
                entity.KaiJiangItems = new List<Kaijiangitem>();
                var tds = tr.ChildNodes.Where(w => w.Name == "td").ToList();
                var xiaoshoue = tds[2].InnerText.Trim().Replace(",", "").Replace("元", "");
                var jiangchi = "";
                var tedengjiang = new Kaijiangitem
                {
                    Name = "特等奖",
                    Total = tds[3].InnerText.Trim(),
                    TotalMoney = tds[4].InnerText
                };
                entity.KaiJiangItems.Add(tedengjiang);
                var yidengjiang = new Kaijiangitem
                {
                    Name = "一等奖",
                    Total = tds[5].InnerText.Trim(),
                    TotalMoney = tds[6].InnerText
                };
                entity.KaiJiangItems.Add(yidengjiang);
                var erdengjiang = new Kaijiangitem
                {
                    Name = "二等奖",
                    Total = tds[7].InnerText.Trim(),
                    TotalMoney = tds[8].InnerText
                };
                entity.KaiJiangItems.Add(erdengjiang);
                entity.Gdje = jiangchi;
                entity.Trje = xiaoshoue;
                model.Spare = entity.TryToJson();
            }
            catch (Exception ex)
            {
                log.Error(typeof(JSTC7WSJob),
                    string.Format("【{0}】通过主站点优化开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            return false;
        }

        /// <summary>
        ///     完善主站江苏体彩7位数开奖详情信息
        /// </summary>
        /// <param name="model"></param>
        private bool OptimizeMainModel(ref OpenCode7DTModel model)
        {
            try
            {
                var entity = new KaijiangDetailsEntity();
                entity.KaiJiangItems = new List<Kaijiangitem>();
                var htmlResource = NetHelper.GetUrlResponse(model.DetailUrl);
                if (htmlResource == null) return false;
                if (!string.IsNullOrEmpty(htmlResource))
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(htmlResource);
                    var table = doc.DocumentNode.SelectNodes("//table");
                    if (table != null && table.Count > 1)
                    {
                        var trs = table[1].ChildNodes.Where(N => N.Name.ToLower() == "tbody").First().ChildNodes
                            .Where(N => N.Name.ToLower() == "tr").ToList();
                        for (var i = 0; i < trs.Count; i++)
                        {
                            var tds = trs[i].ChildNodes.Where(N => N.Name.ToLower() == "td").ToList();
                            if (tds[0].InnerText == "特等奖")
                            {
                                var tmp = new Kaijiangitem
                                {
                                    Name = "特等奖",
                                    Total = tds[1].InnerText.Replace(",", string.Empty).Replace("注", string.Empty),
                                    TotalMoney =
                                        tds[2].InnerText.Replace("元", string.Empty).Replace("--", "0").Replace(",", "")
                                            .Trim()
                                };
                                entity.KaiJiangItems.Add(tmp);
                                //Level1Num = Convert.ToInt32(tds[1].InnerText.Replace(",", string.Empty).Replace("注", string.Empty));
                                //Level1Money = Convert.ToDecimal(tds[2].InnerText.Replace("元", string.Empty));
                            }
                            else if (tds[0].InnerText == "一等奖")
                            {
                                var tmp = new Kaijiangitem
                                {
                                    Name = "一等奖",
                                    Total = tds[1].InnerText.Replace(",", string.Empty).Replace("注", string.Empty),
                                    TotalMoney =
                                        tds[2].InnerText.Replace("元", string.Empty).Replace("--", "0").Replace(",", "")
                                            .Trim()
                                };
                                entity.KaiJiangItems.Add(tmp);
                            }
                            else if (tds[0].InnerText == "二等奖")
                            {
                                var tmp = new Kaijiangitem
                                {
                                    Name = "二等奖",
                                    Total = tds[1].InnerText.Replace(",", string.Empty).Replace("注", string.Empty),
                                    TotalMoney =
                                        tds[2].InnerText.Replace("元", string.Empty).Replace("--", "0").Replace(",", "")
                                            .Trim()
                                };
                                entity.KaiJiangItems.Add(tmp);
                            }
                            else if (tds[0].InnerText == "三等奖")
                            {
                                var tmp = new Kaijiangitem
                                {
                                    Name = "三等奖",
                                    Total = tds[1].InnerText.Replace(",", string.Empty).Replace("注", string.Empty),
                                    TotalMoney =
                                        tds[2].InnerText.Replace("元", string.Empty).Replace("--", "0").Replace(",", "")
                                            .Trim()
                                };
                                entity.KaiJiangItems.Add(tmp);
                            }
                            else if (tds[0].InnerText == "四等奖")
                            {
                                var tmp = new Kaijiangitem
                                {
                                    Name = "四等奖",
                                    Total = tds[1].InnerText.Replace(",", string.Empty).Replace("注", string.Empty),
                                    TotalMoney =
                                        tds[2].InnerText.Replace("元", string.Empty).Replace("--", "0").Replace(",", "")
                                            .Trim()
                                };
                                entity.KaiJiangItems.Add(tmp);
                            }
                            else if (tds[0].InnerText == "五等奖")
                            {
                                var tmp = new Kaijiangitem
                                {
                                    Name = "五等奖",
                                    Total = tds[1].InnerText.Replace(",", string.Empty).Replace("注", string.Empty),
                                    TotalMoney =
                                        tds[2].InnerText.Replace("元", string.Empty).Replace("--", "0").Replace(",", "")
                                            .Trim()
                                };
                                entity.KaiJiangItems.Add(tmp);
                            }
                        }

                        var reg1 = new Regex(@"本省（区、市）销售额：([\s\S]*?)元");
                        var match1 = reg1.Match(htmlResource);
                        if (match1.Success)
                        {
                            //2016年182期及以前期数
                            //Sales = Convert.ToDecimal(match1.Result("$1"));
                            entity.Trje = match1.Result("$1");
                        }
                        else
                        {
                            //2016年183期及以后期数
                            reg1 = new Regex(@"本期销售金额：([\s\S]*?)元");
                            match1 = reg1.Match(htmlResource);
                            if (match1.Success) entity.Trje = match1.Result("$1");
                        }

                        var ps = table[1].ParentNode.ChildNodes.Where(N => N.Name.ToLower() == "p").ToList();
                        var potString = ps.Last().InnerHtml;
                        reg1 = new Regex(@"<br>([\s\S]*?)元");
                        match1 = reg1.Match(potString);
                        if (match1.Success)
                        {
                            var potValue = match1.Result("$1").Replace("&nbsp;", string.Empty);
                            if (potValue.Contains("<br>"))
                                // Jackpot = Convert.ToDecimal(potValue.Substring(potValue.IndexOf("<br>") + 4));
                                entity.Gdje = potValue.Substring(potValue.IndexOf("<br>") + 4);
                            else
                                // Jackpot = Convert.ToDecimal(potValue);
                                entity.Gdje = potValue;
                        }

                        model.Spare = entity.TryToJson();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(typeof(JSTC7WSJob),
                    string.Format("【{0}】通过主站点优化开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
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
                var startQiNum = Convert.ToInt32(LatestItem.Term.ToString().Substring(2)) + 1;
                var newestQiNum = Convert.ToInt32(newestQiHao.Substring(2));
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
                var getQiHao = string.Empty;
                OpenCode7DTModel matchItem = null;
                for (var i = startQiNum; i <= newestQiNum; i++)
                {
                    getQiHao = LatestItem.Term.ToString().Substring(0, 2) + i.ToString().PadLeft(3, '0');
                    matchItem = OpenList.Where(R => R.Term.ToString() == getQiHao).FirstOrDefault();
                    if (matchItem != null && services.AddDTOpen7Code(currentLottery, matchItem))
                    {
                        //Do Success Log
                        log.Info(typeof(JSTC7WSJob), CommonHelper.GetJobBackLogInfo(Config, getQiHao));
                        LatestItem = matchItem;
                        isGetData = true;
                    }
                }
            }
        }

        private List<OpenCode7DTModel> GetOpenListFromBackUrl()
        {
            var result = new List<OpenCode7DTModel>();
            try
            {
                var url = Config.BackUrl + "?lid=10048&page=1";

                var htmlResource = NetHelper.GetBaiDuLeCaiResponse(url);
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
                    var qihao = tds[0].InnerText.Trim().Substring(2);
                    var kaijianghao = tds[1].InnerText.Trim();
                    var opentime = tds[9].InnerText.Trim();
                    var tmp = new OpenCode7DTModel
                    {
                        Term = long.Parse(qihao),
                        OpenTime = DateTime.Parse(opentime)
                    };
                    tmp.OpenCode1 = int.Parse(kaijianghao.Substring(0, 1));
                    tmp.OpenCode2 = int.Parse(kaijianghao.Substring(1, 1));
                    tmp.OpenCode3 = int.Parse(kaijianghao.Substring(2, 1));
                    tmp.OpenCode4 = int.Parse(kaijianghao.Substring(3, 1));
                    tmp.OpenCode5 = int.Parse(kaijianghao.Substring(4, 1));
                    tmp.OpenCode6 = int.Parse(kaijianghao.Substring(5, 1));
                    tmp.OpenCode7 = int.Parse(kaijianghao.Substring(6, 1));
                    OptimizeBackModel(ref tmp, item);
                    result.Add(tmp);
                }

                var checkDataHelper = new CheckDataHelper();
                var dbdata = services.GetListS<OpenCode7DTModel>(currentLottery)
                    .ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr());
                checkDataHelper.CheckData(dbdata, result.ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr()),
                    Config.Area, currentLottery);
                result = result.OrderByDescending(S => S.Term).ToList();
            }
            catch (Exception ex)
            {
                log.Error(typeof(JSTC7WSJob),
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
        private OpenCode7DTModel LatestItem;

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
        private SCCLottery currentLottery => SCCLottery.JiangSuTC7WS;

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