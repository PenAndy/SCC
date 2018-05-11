using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Quartz;
using SCC.Common;
using SCC.Interface;
using SCC.Models;

namespace SCC.Crawler.DT
{
    /// <summary>
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class XJ35X7Job : IJob
    {
        /// <summary>
        /// 初始化函数
        /// </summary>
        public XJ35X7Job()
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
            LatestItem = context.JobDetail.JobDataMap["LatestItem"] as OpenCode8DTModel;
            try
            {
                //服务启动时配置初始数据
                if (LatestItem == null)
                {
                    LatestItem = services.GetOpenCode8DTLastItem(currentLottery);
                    if (LatestItem == null)
                        LatestItem = new OpenCode8DTModel
                        {
                            Term = CommonHelper.GenerateQiHaoYYYYQQQ(0),
                            OpenTime = new DateTime(CommonHelper.SCCSysDateTime.Year, 1, 1)
                        };
                }

                //程序时间第二天，程序根据配置检查是否昨天有开奖
                isGetData = false;
                if (CommonHelper.CheckDTIsNeedGetData(Config)) CheckingOpenDayTheLotteryData();
                if (!LatestItem.Term.ToString().StartsWith(CommonHelper.SCCSysDateTime.ToString("yyyy")))
                    LatestItem = new OpenCode8DTModel
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
                log.Error(GetType(),
                    string.Format("【{0}】抓取时发生错误，错误信息【{1}】", Config.Area + Config.LotteryName, ex.Message));
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
                if (LatestItem.OpenTime.ToString("yyyyMMdd") !=
                    CommonHelper.SCCSysDateTime.AddDays(-1).ToString("yyyyMMdd"))
                {
                    var openQiHao = (LatestItem.Term + 1).ToString();
                    if (email.AddEmail(Config.Area + Config.LotteryName, openQiHao,
                        CommonHelper.GenerateDTOpenTime(Config)))
                        log.Error(GetType(), CommonHelper.GetJobLogError(Config, openQiHao));
                }
            }
        }

        /// <summary>
        ///     通过主站点爬取开奖数据
        ///     (百度乐彩)
        /// </summary>
        private void DoTodayJobByMainUrl()
        {
            if (!string.IsNullOrEmpty(Config.MainUrl))
            {
                var OpenList = GetOpenListFromMainUrl();
                if (OpenList.Count == 0) return; //无抓取数据
                var newestQiHao = OpenList.First().Term.ToString();
                var startQiNum = Convert.ToInt32(LatestItem.Term.ToString().Substring(4)) + 1;
                var newestQiNum = Convert.ToInt32(newestQiHao.Substring(4));
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
                var getQiHao = string.Empty;
                OpenCode8DTModel matchItem = null;
                for (var i = startQiNum; i <= newestQiNum; i++)
                {
                    getQiHao = LatestItem.Term.ToString().Substring(0, 4) + i.ToString().PadLeft(3, '0');
                    matchItem = OpenList.Where(R => R.Term.ToString() == getQiHao).FirstOrDefault();
                    if (matchItem != null && OptimizeMainModel(ref matchItem) &&
                        services.AddDTOpen8Code(currentLottery, matchItem))
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
        ///     (百度乐彩)
        /// </summary>
        private void DoYesterdayFailedListByMainUrl()
        {
            if (!string.IsNullOrEmpty(Config.MainUrl) && FailedQiHaoList.Count > 0)
            {
                var OpenList = GetOpenListFromMainUrl();
                if (OpenList.Count == 0) return; //无抓取数据
                OpenCode8DTModel matchItem = null;
                var SuccessList = new List<string>();
                foreach (var failedQiHao in FailedQiHaoList)
                {
                    matchItem = OpenList.Where(R => R.Term.ToString() == failedQiHao).FirstOrDefault();
                    if (matchItem != null && OptimizeMainModel(ref matchItem) &&
                        services.AddDTOpen8Code(currentLottery, matchItem))
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
        ///     通过主站点爬取开奖列表数据
        /// </summary>
        /// <returns></returns>
        private List<OpenCode8DTModel> GetOpenListFromMainUrl()
        {
            var result = new List<OpenCode8DTModel>();
            try
            {
                var requestUrl = string.Format("{0}?r={1}", Config.MainUrl, new Random().Next(1000, 9999));
                var htmlResource = NetHelper.GetBaiDuLeCaiResponse(requestUrl);
                if (htmlResource == null) return result;

                var doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                var table = doc.DocumentNode.SelectSingleNode("//tbody");
                if (table == null) return result;
                var trs = table.ChildNodes.Where(node => node.Name == "tr").ToList();
                OpenCode8DTModel model = null;
                List<HtmlNode> tds, alist = null;
                for (var i = 0; i < trs.Count; i++) //第一行
                {
                    var trstyle = trs[i].Attributes["style"];
                    if (trstyle != null && trstyle.Value == "display:none") continue;
                    tds = trs[i].ChildNodes.Where(node => node.Name == "td").ToList();
                    if (tds.Count < 4) continue;
                    model = new OpenCode8DTModel();
                    model.OpenTime = Convert.ToDateTime(tds[0].InnerText);
                    alist = tds[1].ChildNodes.Where(n => n.Name == "a").ToList();
                    if (alist.Count < 0) continue;
                    model.Term = Convert.ToInt64(alist[0].InnerText.Trim());
                    if (tds[2].ChildNodes.Count == 0) continue;
                    var opencodeNode = tds[2].ChildNodes[0].ChildNodes.Where(n => n.Name == "span").ToList();
                    if (opencodeNode.Count < 8) continue;
                    model.OpenCode1 = Convert.ToInt32(opencodeNode[0].InnerText.Trim());
                    model.OpenCode2 = Convert.ToInt32(opencodeNode[1].InnerText.Trim());
                    model.OpenCode3 = Convert.ToInt32(opencodeNode[2].InnerText.Trim());
                    model.OpenCode4 = Convert.ToInt32(opencodeNode[3].InnerText.Trim());
                    model.OpenCode5 = Convert.ToInt32(opencodeNode[4].InnerText.Trim());
                    model.OpenCode6 = Convert.ToInt32(opencodeNode[5].InnerText.Trim());
                    model.OpenCode7 = Convert.ToInt32(opencodeNode[6].InnerText.Trim());
                    model.OpenCode8 = Convert.ToInt32(opencodeNode[7].InnerText.Trim());
                    result.Add(model);
                }

                result = result.OrderByDescending(S => S.Term).ToList();
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过主站点抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + Config.LotteryName, ex.Message));
            }

            return result;
        }

        /// <summary>
        ///     完善主站点开奖实体信息
        /// </summary>
        /// <param name="model"></param>
        private bool OptimizeMainModel(ref OpenCode8DTModel model)
        {
            var url = string.Format("http://baidu.lecai.com/lottery/draw/view/520/{0}?r={1}", model.Term,
                new Random().Next(1000, 9999));
            try
            {
                var htmlResource = NetHelper.GetBaiDuLeCaiResponse(url);
                var reg = new Regex(@"var phaseData = ([\s\S]*?);");
                var m = reg.Match(htmlResource);
                if (m.Success)
                {
                    var dataJson = m.Result("$1");
                    var obj = dataJson.JsonToEntity<dynamic>();
                    var data = obj[model.Term.ToString()];
                    if (data != null)
                    {
                        int Level1Num = 0,
                            Level2Num = 0,
                            Level3Num = 0,
                            Level4Num = 0,
                            Level5Num = 0,
                            Level6Num = 0,
                            Level7Num = 0;
                        decimal Level1Money = 0,
                            Level2Money = 0,
                            Level3Money = 0,
                            Level4Money = 0,
                            Level5Money = 0,
                            Level6Money = 0,
                            Level7Money = 0,
                            Sales = 0,
                            Jackpot = 0;
                        Jackpot = Convert.ToDecimal(data["formatPoolAmount"]);
                        Sales = Convert.ToDecimal(data["formatSaleAmount"]);
                        Level1Num = Convert.ToInt32(data["list"]["prize1"]["bet"].Value.Replace(",", string.Empty)
                            .Replace("注", string.Empty));
                        Level1Money = Convert.ToDecimal(data["list"]["prize1"]["prize"].Value.Replace(",", string.Empty)
                            .Replace("元", string.Empty));
                        Level2Num = Convert.ToInt32(data["list"]["prize2"]["bet"].Value.Replace(",", string.Empty)
                            .Replace("注", string.Empty));
                        Level2Money = Convert.ToDecimal(data["list"]["prize2"]["prize"].Value.Replace(",", string.Empty)
                            .Replace("元", string.Empty));
                        Level3Num = Convert.ToInt32(data["list"]["prize3"]["bet"].Value.Replace(",", string.Empty)
                            .Replace("注", string.Empty));
                        Level3Money = Convert.ToDecimal(data["list"]["prize3"]["prize"].Value.Replace(",", string.Empty)
                            .Replace("元", string.Empty));
                        Level4Num = Convert.ToInt32(data["list"]["prize4"]["bet"].Value.Replace(",", string.Empty)
                            .Replace("注", string.Empty));
                        Level4Money = Convert.ToDecimal(data["list"]["prize4"]["prize"].Value.Replace(",", string.Empty)
                            .Replace("元", string.Empty));
                        Level5Num = Convert.ToInt32(data["list"]["prize5"]["bet"].Value.Replace(",", string.Empty)
                            .Replace("注", string.Empty));
                        Level5Money = Convert.ToDecimal(data["list"]["prize5"]["prize"].Value.Replace(",", string.Empty)
                            .Replace("元", string.Empty));
                        Level6Num = Convert.ToInt32(data["list"]["prize6"]["bet"].Value.Replace(",", string.Empty)
                            .Replace("注", string.Empty));
                        Level6Money = Convert.ToDecimal(data["list"]["prize6"]["prize"].Value.Replace(",", string.Empty)
                            .Replace("元", string.Empty));
                        Level7Num = Convert.ToInt32(data["list"]["prize7"]["bet"].Value.Replace(",", string.Empty)
                            .Replace("注", string.Empty));
                        Level7Money = Convert.ToDecimal(data["list"]["prize7"]["prize"].Value.Replace(",", string.Empty)
                            .Replace("元", string.Empty));
                        model.Spare = string.Format(
                            "{0},{1}^一等奖|{2}|{3},二等奖|{4}|{5},三等奖|{6}|{7},四等奖|{8}|{9},五等奖|{10}|{11},六等奖|{12}|{13},七等奖|{14}|{15}",
                            Sales, Jackpot, Level1Num, Level1Money, Level2Num, Level2Money, Level3Num, Level3Money,
                            Level4Num, Level4Money, Level5Num, Level5Money, Level6Num, Level6Money, Level7Num,
                            Level7Money);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过主站点完善开奖列表时发生错误，错误信息【{1}】", Config.Area + Config.LotteryName, ex.Message));
            }

            return false;
        }


        /// <summary>
        ///     通过备用站点抓取开奖数据
        ///     (新疆体彩网)
        ///     仅仅抓取首页上的最新5条数据
        /// </summary>
        private void DoTodayJobByBackUrl()
        {
            if (!string.IsNullOrEmpty(Config.BackUrl))
            {
                var OpenList = GetOpenListFromBackUrl();
                if (OpenList.Count == 0) return; //无抓取数据
                var newestQiHao = OpenList.First().Key;
                var startQiNum = Convert.ToInt32(LatestItem.Term.ToString().Substring(4)) + 1;
                var newestQiNum = Convert.ToInt32(newestQiHao.Substring(4));
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
                var getQiHao = string.Empty;
                OpenCode8DTModel matchItem = null;
                KeyValuePair<string, string> termItem;
                for (var i = startQiNum; i <= newestQiNum; i++)
                {
                    getQiHao = LatestItem.Term.ToString().Substring(0, 4) + i.ToString().PadLeft(3, '0');
                    termItem = OpenList.Where(R => R.Key == getQiHao).FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(termItem.Key)) continue;
                    matchItem = GetOpenModelFromBackUrl(termItem);
                    if (matchItem != null && services.AddDTOpen8Code(currentLottery, matchItem))
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
        ///     (新疆体彩网)
        /// </summary>
        private void DoYesterdayFailedListByBackUrl()
        {
            if (!string.IsNullOrEmpty(Config.BackUrl) && FailedQiHaoList.Count > 0)
            {
                var OpenList = GetOpenListFromBackUrl();
                if (OpenList.Count == 0) return; //无抓取数据
                OpenCode8DTModel matchItem = null;
                KeyValuePair<string, string> termItem;
                var SuccessList = new List<string>();
                foreach (var failedQiHao in FailedQiHaoList)
                {
                    termItem = OpenList.Where(R => R.Key == failedQiHao).FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(termItem.Key)) continue;
                    matchItem = GetOpenModelFromBackUrl(termItem);
                    if (matchItem != null && services.AddDTOpen8Code(currentLottery, matchItem))
                    {
                        //Do Success Log
                        log.Info(GetType(), CommonHelper.GetJobBackLogInfo(Config, failedQiHao));
                        if (matchItem.Term > LatestItem.Term) LatestItem = matchItem;
                        SuccessList.Add(failedQiHao);
                        isGetData = true;
                    }
                }

                foreach (var successQiHao in SuccessList) FailedQiHaoList.Remove(successQiHao);
            }
        }

        /// <summary>
        ///     获取主站有开奖详情的期号列表
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetOpenListFromBackUrl()
        {
            var result = new Dictionary<string, string>();
            try
            {
                var htmlResource = NetHelper.GetUrlResponse(Config.BackUrl);
                if (!string.IsNullOrEmpty(htmlResource))
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(htmlResource);
                    var select = doc.GetElementbyId("the7Select");
                    if (select == null) return result;
                    var selectList = select.ChildNodes.Where(N => N.Name.ToLower() == "option").ToList();
                    foreach (var node in selectList)
                    {
                        var QiHao = node.NextSibling.InnerText;
                        var OptionValue = node.Attributes["value"].Value;
                        result.Add(QiHao, OptionValue);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过备用站点抓取开奖期号列表时发生错误，错误信息【{1}】", Config.Area + Config.LotteryName, ex.Message));
            }

            return result;
        }

        /// <summary>
        ///     通过期号编号从备用站点爬取开奖信息
        /// </summary>
        /// <param name="termItem">备用站点的期号信息</param>
        /// <returns></returns>
        private OpenCode8DTModel GetOpenModelFromBackUrl(KeyValuePair<string, string> termItem)
        {
            var result = new OpenCode8DTModel();
            try
            {
                if (string.IsNullOrWhiteSpace(termItem.Key) || string.IsNullOrWhiteSpace(termItem.Value)) return result;
                result.Term = Convert.ToInt64(termItem.Key);
                var lotteryNumberUrl = "http://www.xjflcp.com/getLotteryNumber";
                var numberJson = NetHelper.GetUrlResponse(lotteryNumberUrl, "POST",
                    string.Format("lotteryId={0}", termItem.Value));
                if (numberJson == null) return result;

                int Level1Num = 0,
                    Level2Num = 0,
                    Level3Num = 0,
                    Level4Num = 0,
                    Level5Num = 0,
                    Level6Num = 0,
                    Level7Num = 0;
                decimal Level1Money = 0,
                    Level2Money = 0,
                    Level3Money = 0,
                    Level4Money = 0,
                    Level5Money = 0,
                    Level6Money = 0,
                    Level7Money = 0,
                    Sales = 0,
                    Jackpot = 0;
                var numberObj = numberJson.JsonToEntity<dynamic>();
                for (var i = 0; i < numberObj.Count; i++)
                    if (numberObj[i].key == "announceTime")
                        result.OpenTime = Convert.ToDateTime(numberObj[i].value);
                    else if (numberObj[i].key == "totalSale")
                        Sales = Convert.ToDecimal(numberObj[i].value);
                    else if (numberObj[i].key == "totalProgressive")
                        Jackpot = Convert.ToDecimal(numberObj[i].value);
                    else if (numberObj[i].key == "0")
                        result.OpenCode1 = Convert.ToInt32(numberObj[i].value);
                    else if (numberObj[i].key == "1")
                        result.OpenCode2 = Convert.ToInt32(numberObj[i].value);
                    else if (numberObj[i].key == "2")
                        result.OpenCode3 = Convert.ToInt32(numberObj[i].value);
                    else if (numberObj[i].key == "3")
                        result.OpenCode4 = Convert.ToInt32(numberObj[i].value);
                    else if (numberObj[i].key == "4")
                        result.OpenCode5 = Convert.ToInt32(numberObj[i].value);
                    else if (numberObj[i].key == "5")
                        result.OpenCode6 = Convert.ToInt32(numberObj[i].value);
                    else if (numberObj[i].key == "6")
                        result.OpenCode7 = Convert.ToInt32(numberObj[i].value);
                    else if (numberObj[i].key == "7")
                        result.OpenCode8 = Convert.ToInt32(numberObj[i].value);
                var lotteryDetailUrl = "http://www.xjflcp.com/getLotteryDetailInfo";
                var detailJson = NetHelper.GetUrlResponse(lotteryDetailUrl, "POST",
                    string.Format("lotteryId={0}&gameId=4", termItem.Value));
                if (!string.IsNullOrEmpty(numberJson))
                {
                    var detailObj = detailJson.JsonToEntity<dynamic>();
                    for (var i = 0; i < detailObj.Count; i++)
                        if (detailObj[i].prizeName == "一等奖")
                        {
                            Level1Num = Convert.ToInt32(detailObj[i].number.ToString().Replace(",", ""));
                            Level1Money = Convert.ToDecimal(detailObj[i].prize);
                        }
                        else if (detailObj[i].prizeName == "二等奖")
                        {
                            Level2Num = Convert.ToInt32(detailObj[i].number.ToString().Replace(",", ""));
                            Level2Money = Convert.ToDecimal(detailObj[i].prize);
                        }
                        else if (detailObj[i].prizeName == "三等奖")
                        {
                            Level3Num = Convert.ToInt32(detailObj[i].number.ToString().Replace(",", ""));
                            Level3Money = Convert.ToDecimal(detailObj[i].prize);
                        }
                        else if (detailObj[i].prizeName == "四等奖")
                        {
                            Level4Num = Convert.ToInt32(detailObj[i].number.ToString().Replace(",", ""));
                            Level4Money = Convert.ToDecimal(detailObj[i].prize);
                        }
                        else if (detailObj[i].prizeName == "五等奖")
                        {
                            Level5Num = Convert.ToInt32(detailObj[i].number.ToString().Replace(",", ""));
                            Level5Money = Convert.ToDecimal(detailObj[i].prize);
                        }
                        else if (detailObj[i].prizeName == "六等奖")
                        {
                            Level6Num = Convert.ToInt32(detailObj[i].number.ToString().Replace(",", ""));
                            Level6Money = Convert.ToDecimal(detailObj[i].prize);
                        }
                        else if (detailObj[i].prizeName == "七等奖")
                        {
                            Level7Num = Convert.ToInt32(detailObj[i].number.ToString().Replace(",", ""));
                            Level7Money = Convert.ToDecimal(detailObj[i].prize);
                        }
                }

                result.Spare = string.Format(
                    "{0},{1}^一等奖|{2}|{3},二等奖|{4}|{5},三等奖|{6}|{7},四等奖|{8}|{9},五等奖|{10}|{11},六等奖|{12}|{13},七等奖|{14}|{15}",
                    Sales, Jackpot, Level1Num, Level1Money, Level2Num, Level2Money, Level3Num, Level3Money,
                    Level4Num, Level4Money, Level5Num, Level5Money, Level6Num, Level6Money, Level7Num, Level7Money);
            }

            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过备用站点抓取开奖详情数据时发生错误，错误信息【{1}】", Config.Area + Config.LotteryName, ex.Message));
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
        private OpenCode8DTModel LatestItem;

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
        private SCCLottery currentLottery => SCCLottery.XinJiangFC35X7;

        /// <summary>
        ///     邮件接口
        /// </summary>
        private readonly IEmail email;

        /// <summary>
        ///     是否本次运行抓取到开奖数据
        /// </summary>
        private bool isGetData;

        #endregion
    }
}