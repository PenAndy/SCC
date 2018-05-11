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
    ///     东方6+1
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class DF6J1Job : IJob
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        public DF6J1Job()
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
                    LatestItem = services.GetOpenCode7DTLastItem(currentLottery) ?? new OpenCode7DTModel
                    {
                        Term = CommonHelper.GenerateQiHaoYYYYQQQ(0),
                        OpenTime = new DateTime(CommonHelper.SCCSysDateTime.Year, 1, 1)
                    };
                //程序时间第二天，程序根据配置检查是否昨天有开奖
                isGetData = false;
                if (CommonHelper.CheckDTIsNeedGetData(Config)) CheckingOpenDayTheLotteryData();
                if (!LatestItem.Term.ToString().StartsWith(CommonHelper.SCCSysDateTime.ToString("yyyy")))
                    LatestItem = new OpenCode7DTModel
                    {
                        Term = CommonHelper.GenerateQiHaoYYYYQQQ(0),
                        OpenTime = new DateTime(CommonHelper.SCCSysDateTime.Year, 1, 1)
                    };
                //当今日开奖并且当前时间是晚上8点过后开始抓取
                if (CommonHelper.CheckTodayIsOpenDay(Config) && CommonHelper.SCCSysDateTime.Hour > 12)
                {
                    DoTodayJobByMainUrl();
                    DoTodayJobByBackUrl();
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
        ///     自检爬取未爬取到的开奖数据，并对昨日开奖但未爬取到开奖数据的彩种添加邮件提醒
        /// </summary>
        private void CheckingOpenDayTheLotteryData()
        {
            //从数据库中获取昨天数据抓取失败列表
            FailedQiHaoList = services.GetFailedYYYYQQQList(currentLottery);
            if (FailedQiHaoList.Count > 0)
            {
                DoYesterdayFailedListByMainUrl();
                DoYesterdayFailedListByBackUrl();
            }

            if (LatestItem.OpenTime.ToString("yyyyMMdd") !=
                CommonHelper.SCCSysDateTime.AddDays(-1).ToString("yyyyMMdd"))
            {
                //开奖时间(昨天)未抓取到最新开奖数据,则再抓取一次，若还不成功则写入待发送邮件列表
                DoTodayJobByMainUrl();
                DoTodayJobByBackUrl();
            }
        }

        /// <summary>
        ///     通过主站点爬取开奖数据
        ///     (江苏福彩网)
        /// </summary>
        private void DoTodayJobByMainUrl()
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
                OpenCode7DTModel matchItem = null;
                for (var i = startQiNum; i <= newestQiNum; i++)
                {
                    getQiHao = LatestItem.Term.ToString().Substring(0, 4) + i.ToString().PadLeft(3, '0');
                    matchItem = OpenList.FirstOrDefault(R => R.Term.ToString() == getQiHao);
                    if (matchItem != null && OptimizeMainModel(ref matchItem) &&
                        services.AddDTOpen7Code(currentLottery, matchItem))
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
        ///     通过主站爬取错误期号列表中每一个期号
        ///     (江苏福彩网)
        /// </summary>
        private void DoYesterdayFailedListByMainUrl()
        {
            if (!string.IsNullOrEmpty(Config.MainUrl) && FailedQiHaoList.Count > 0)
            {
                var OpenList = GetOpenListFromMainUrl(Config.MainUrl);
                if (OpenList.Count == 0) return; //无抓取数据
                OpenCode7DTModel matchItem = null;
                var SuccessList = new List<string>();
                foreach (var failedQiHao in FailedQiHaoList)
                {
                    matchItem = OpenList.FirstOrDefault(R => R.Term.ToString() == failedQiHao);
                    if (matchItem != null && OptimizeMainModel(ref matchItem) &&
                        services.AddDTOpen7Code(currentLottery, matchItem))
                    {
                        //Do Success Log
                        log.Info(GetType(), CommonHelper.GetJobMainLogInfo(Config, failedQiHao));

                        if (matchItem.Term > LatestItem.Term) LatestItem = matchItem;
                        isGetData = true;
                        SuccessList.Add(failedQiHao);
                    }
                }

                foreach (var successQiHao in SuccessList) FailedQiHaoList.Remove(successQiHao);
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
                var postData = "PlayType=6&currentPage=1&pageSize=200";
                var htmlResource = NetHelper.GetUrlResponse(mainUrl, "POST", postData, Encoding.UTF8);
                if (htmlResource == null) return result;

                var obj = htmlResource.JsonToEntity<JObject>();
                var lotteryList = obj["LotteryNumberList"];
                OpenCode7DTModel model = null;
                string[] openCodeList = null;
                var issue = string.Empty;
                foreach (var item in lotteryList)
                {
                    issue = item["Issue"].ToString();
                    if (issue.StartsWith((CommonHelper.SCCSysDateTime.Year - 1).ToString())) break; //只抓取今年数据
                    model = new OpenCode7DTModel();
                    model.Term = Convert.ToInt64(issue);
                    model.OpenTime = Convert.ToDateTime(item["LotteryDate"].ToString());
                    openCodeList = item["BasicNumber"].ToString().Trim().Replace("[", string.Empty)
                        .Replace("]", string.Empty).Replace("\r\n", string.Empty).Replace("\"", string.Empty)
                        .Split(',');
                    if (openCodeList.Length != 6) continue;
                    model.OpenCode1 = Convert.ToInt32(openCodeList[0]);
                    model.OpenCode2 = Convert.ToInt32(openCodeList[1]);
                    model.OpenCode3 = Convert.ToInt32(openCodeList[2]);
                    model.OpenCode4 = Convert.ToInt32(openCodeList[3]);
                    model.OpenCode5 = Convert.ToInt32(openCodeList[4]);
                    model.OpenCode6 = Convert.ToInt32(openCodeList[5]);
                    model.OpenCode7 = ConvertShengXiaoToNumber(item["SpecialNumber"].ToString());
                    model.Spare = string.IsNullOrEmpty(item["PrizePool"].ToString().Replace("万元", string.Empty))
                        ? "0"
                        : item["PrizePool"].ToString().Replace("万元", string.Empty);
                    result.Add(model);
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
                log.Error(GetType(),
                    string.Format("【{0}】通过主站点抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            return result;
        }

        /// <summary>
        ///     完善主站江苏体彩7位数开奖详情信息
        /// </summary>
        /// <param name="model"></param>
        private bool OptimizeMainModel(ref OpenCode7DTModel model)
        {
            var url = string.Format("http://www.jslottery.com/Lottery/LotteryDetails?PlayType=6&Issue={0}", model.Term);
            try
            {
                var htmlResource = NetHelper.GetUrlResponse(url);


                if (!string.IsNullOrEmpty(htmlResource))
                {
                    var entity = new KaijiangDetailsEntity
                    {
                        KaiJiangItems = new List<Kaijiangitem>()
                    };
                    var doc = new HtmlDocument();
                    doc.LoadHtml(htmlResource);
                    var tables = doc.DocumentNode.SelectNodes("//table");
                    if (tables.Count < 5) return false;
                    var trs = tables[4].ChildNodes[0].ChildNodes.Where(S => S.Name.ToLower() == "tr").ToList();
                    decimal Sales = 0, Jackpot = 0;
                    foreach (var tr in trs)
                    {
                        var tds = tr.ChildNodes.Where(S => S.Name.ToLower() == "td").ToList();
                        if (tds.Count < 3) continue;
                        if (tds[0].InnerText == "一等奖")
                        {
                            var tmp = new Kaijiangitem
                            {
                                Name = "一等奖",
                                Total = tds[1].InnerText.Trim(),
                                TotalMoney = tds[2].InnerText.Trim()
                            };
                            entity.KaiJiangItems.Add(tmp);
                            // Level1Num = Convert.ToInt32(tds[1].InnerText);
                            //Level1Money = Convert.ToInt32(tds[2].InnerText);
                        }
                        else if (tds[0].InnerText == "二等奖")
                        {
                            var tmp = new Kaijiangitem
                            {
                                Name = "二等奖",
                                Total = tds[1].InnerText.Trim(),
                                TotalMoney = tds[2].InnerText.Trim()
                            };
                            entity.KaiJiangItems.Add(tmp);
                        }
                        else if (tds[0].InnerText == "三等奖")
                        {
                            var tmp = new Kaijiangitem
                            {
                                Name = "三等奖",
                                Total = tds[1].InnerText.Trim(),
                                TotalMoney = tds[2].InnerText.Trim()
                            };
                            entity.KaiJiangItems.Add(tmp);
                        }
                        else if (tds[0].InnerText == "四等奖")
                        {
                            var tmp = new Kaijiangitem
                            {
                                Name = "四等奖",
                                Total = tds[1].InnerText.Trim(),
                                TotalMoney = tds[2].InnerText.Trim()
                            };
                            entity.KaiJiangItems.Add(tmp);
                        }
                        else if (tds[0].InnerText == "五等奖")
                        {
                            var tmp = new Kaijiangitem
                            {
                                Name = "五等奖",
                                Total = tds[1].InnerText.Trim(),
                                TotalMoney = tds[2].InnerText.Trim()
                            };
                            entity.KaiJiangItems.Add(tmp);
                        }
                        else if (tds[0].InnerText == "六等奖")
                        {
                            var tmp = new Kaijiangitem
                            {
                                Name = "六等奖",
                                Total = tds[1].InnerText.Trim(),
                                TotalMoney = tds[2].InnerText.Trim()
                            };
                            entity.KaiJiangItems.Add(tmp);
                        }
                    }

                    var reg = new Regex(@"本期投注总额<span>([\d.,]*?)</span>");
                    var match = reg.Match(htmlResource);
                    if (match.Success)
                    {
                        Sales = Convert.ToDecimal(match.Result("$1").Replace(",", string.Empty)) * 10000;
                    }
                    else
                    {
                        reg = new Regex(@"本期投注总额<span lang=""EN-US"">([\d.,]*?)</span>");
                        match = reg.Match(htmlResource);
                        if (match.Success)
                            Sales = Convert.ToDecimal(match.Result("$1").Replace(",", string.Empty)) * 10000;
                    }

                    Jackpot = Convert.ToDecimal(model.Spare.Replace(",", string.Empty)) * 10000;
                    entity.Gdje = Jackpot.ToString();
                    entity.Trje = Sales.ToString();
                    model.Spare = entity.TryToJson();
                    //model.Spare = string.Format("{0},{1}^一等奖|{2}|{3},二等奖|{4}|{5},三等奖|{6}|{7},四等奖|{8}|{9},五等奖|{10}|{11},六等奖|{12}|{13}",
                    //            Sales, Jackpot, Level1Num, Level1Money, Level2Num, Level2Money, Level3Num, Level3Money,
                    //            Level4Num, Level4Money, Level5Num, Level5Money, Level6Num, Level6Money);
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
        ///     将12生肖文字转换为对应的1-12的数字
        /// </summary>
        /// <param name="shengXiao">生肖文字</param>
        /// <returns></returns>
        public int ConvertShengXiaoToNumber(string shengXiao)
        {
            var result = 0;
            if (!string.IsNullOrEmpty(shengXiao))
            {
                var regex = new Regex("[\u4e00-\u9fa5]");
                var match = regex.Match(shengXiao);
                var res = match.Value;

                switch (res)
                {
                    case "鼠":
                        result = 1;
                        break;
                    case "牛":
                        result = 2;
                        break;
                    case "虎":
                        result = 3;
                        break;
                    case "兔":
                        result = 4;
                        break;
                    case "龙":
                        result = 5;
                        break;
                    case "蛇":
                        result = 6;
                        break;
                    case "马":
                        result = 7;
                        break;
                    case "羊":
                        result = 8;
                        break;
                    case "猴":
                        result = 9;
                        break;
                    case "鸡":
                        result = 10;
                        break;
                    case "狗":
                        result = 11;
                        break;
                    case "猪":
                        result = 12;
                        break;
                }
            }

            return result;
        }

        /// <summary>
        ///     通过备用站点抓取开奖数据
        /// </summary>
        private void DoTodayJobByBackUrl()
        {
            if (!string.IsNullOrEmpty(Config.BackUrl))
            {
                var OpenList = GetOpenListFromBackUrl();
                if (OpenList.Count == 0) return; //无抓取数据
                var newestQiHao = OpenList.Max(w => w.Term).ToString();
                var startQiNum = Convert.ToInt32(LatestItem.Term.ToString().Substring(4)) + 1;
                var newestQiNum = Convert.ToInt32(newestQiHao.Substring(4));
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
                var getQiHao = string.Empty;
                OpenCode7DTModel matchItem = null;
                for (var i = startQiNum; i <= newestQiNum; i++)
                {
                    getQiHao = LatestItem.Term.ToString().Substring(0, 4) + i.ToString().PadLeft(3, '0');
                    matchItem = OpenList.FirstOrDefault(R => R.Term.ToString() == getQiHao);
                    if (matchItem != null && services.AddDTOpen7Code(currentLottery, matchItem))
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
        ///     通过备用地址抓取错误期号列表中每一个期号
        ///     (百度乐彩)
        /// </summary>
        private void DoYesterdayFailedListByBackUrl()
        {
            if (!string.IsNullOrEmpty(Config.BackUrl) && FailedQiHaoList.Count > 0)
            {
                var OpenList = GetOpenListFromBackUrl();
                if (OpenList.Count == 0) return; //无抓取数据
                OpenCode7DTModel matchItem = null;
                var SuccessList = new List<string>();
                foreach (var failedQiHao in FailedQiHaoList)
                {
                    matchItem = OpenList.FirstOrDefault(R => R.Term.ToString() == failedQiHao);
                    if (matchItem != null && services.AddDTOpen7Code(currentLottery, matchItem))
                    {
                        //Do Success Log
                        log.Info(GetType(), CommonHelper.GetJobMainLogInfo(Config, failedQiHao));
                        if (matchItem.Term > LatestItem.Term) LatestItem = matchItem;
                        SuccessList.Add(failedQiHao);
                        isGetData = true;
                    }
                }

                foreach (var successQiHao in SuccessList) FailedQiHaoList.Remove(successQiHao);
            }
        }

        /// <summary>
        ///     获取备用站点开奖列表数据
        /// </summary>
        /// <returns></returns>
        private List<OpenCode7DTModel> GetOpenListFromBackUrl()
        {
            var result = new List<OpenCode7DTModel>();
            try
            {
                var url = string.Format("{0}&page = 1", Config.BackUrl);
                var htmlResource = NetHelper.GetUrlResponse(url, Encoding.Default);
                if (!string.IsNullOrEmpty(htmlResource))
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(htmlResource);
                    var tables = doc.DocumentNode.SelectNodes("//table[@class]");
                    foreach (var item in tables[0].ChildNodes)
                        if (item.Name == "tr")
                            if (item.GetAttributeValue("class", "") == "")
                            {
                                var tds = item.ChildNodes.Select(w => w).ToList();
                                tds.RemoveAll(w => w.Name != "td");
                                if (tds.Count != 10) continue;
                                var qihao = tds[0].InnerText;
                                var code = tds[1].InnerText;
                                var optime = tds[9].InnerText;
                                if (code.Length > 6)
                                {
                                    var tmp = new OpenCode7DTModel
                                    {
                                        Term = long.Parse(qihao),
                                        OpenTime = DateTime.Parse(optime)
                                    };
                                    tmp.OpenCode1 = int.Parse(code.Substring(0, 1));
                                    tmp.OpenCode2 = int.Parse(code.Substring(1, 1));
                                    tmp.OpenCode3 = int.Parse(code.Substring(2, 1));
                                    tmp.OpenCode4 = int.Parse(code.Substring(3, 1));
                                    tmp.OpenCode5 = int.Parse(code.Substring(4, 1));
                                    tmp.OpenCode6 = int.Parse(code.Substring(5, 1));
                                    tmp.OpenCode7 = int.Parse(code.Substring(6));
                                    var entity = new KaijiangDetailsEntity();
                                    entity.KaiJiangItems = new List<Kaijiangitem>();
                                    var xiaoshoue = tds[2].InnerText.Replace(',', ' ').Replace('元', ' ').Trim();
                                    var yidengjiang = new Kaijiangitem
                                    {
                                        Name = "一等奖",
                                        Total = tds[3].InnerText.Trim(),
                                        TotalMoney = tds[4].InnerText.Trim()
                                    };
                                    entity.KaiJiangItems.Add(yidengjiang);
                                    var erdengjiang = new Kaijiangitem
                                    {
                                        Name = "二等奖",
                                        Total = tds[5].InnerText.Trim(),
                                        TotalMoney = tds[6].InnerText.Trim()
                                    };
                                    entity.KaiJiangItems.Add(erdengjiang);
                                    var sandengjiang = new Kaijiangitem
                                    {
                                        Name = "三等奖",
                                        Total = tds[7].InnerText.Trim(),
                                        TotalMoney = tds[8].InnerText.Trim()
                                    };
                                    entity.KaiJiangItems.Add(sandengjiang);
                                    entity.Trje = xiaoshoue;
                                    entity.Gdje = "0";
                                    tmp.Spare = entity.TryToJson();
                                    result.Add(tmp);
                                }
                            }

                    var checkDataHelper = new CheckDataHelper();
                    var dbdata = services.GetListS<OpenCode7DTModel>(currentLottery)
                        .ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr());
                    checkDataHelper.CheckData(dbdata, result.ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr()),
                        Config.Area, currentLottery);
                }
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
        private OpenCode7DTModel LatestItem;

        /// <summary>
        ///     当天抓取失败列表
        /// </summary>
        private List<string> FailedQiHaoList;

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
        private SCCLottery currentLottery => SCCLottery.DF6J1;

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