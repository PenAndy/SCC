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
    ///     河南22选5
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class HeNan22X5Job : IJob
    {
        /// <summary>
        /// 初始化函数
        /// </summary>
        public HeNan22X5Job()
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
                if (CommonHelper.CheckDTIsNeedGetData(Config)) CheckingOpenDayTheLotteryData();
                if (!LatestItem.Term.ToString().StartsWith(CommonHelper.SCCSysDateTime.ToString("yyyy")))
                    LatestItem = new OpenCode5DTModel
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
                if (LatestItem.OpenTime.ToString("yyyyMMdd") !=
                    CommonHelper.SCCSysDateTime.AddDays(-1).ToString("yyyyMMdd"))
                {
                    var openQiHao = (LatestItem.Term + 1).ToString();
                    if (email.AddEmail(Config.Area + currentLottery, openQiHao,
                        CommonHelper.GenerateDTOpenTime(Config)))
                        log.Error(GetType(), CommonHelper.GetJobLogError(Config, openQiHao));
                }
            }
        }

        /// <summary>
        ///     通过主站点爬取开奖数据
        ///     (河南福彩网)
        /// </summary>
        private void DoTodayJobByMainUrl()
        {
            if (!string.IsNullOrEmpty(Config.MainUrl))
            {
                var OpenList = GetOpenListFromMainUrl(Config.MainUrl);
                if (OpenList.Count == 0) return; //无抓取数据
                var newestQiHao = OpenList.First().Term.ToString();
                var startQiNum = Convert.ToInt32(LatestItem.Term.ToString().Substring(4)) + 1;
                var newestQiNum = Convert.ToInt32(newestQiHao);
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
        ///     通过主站爬取错误期号列表中每一个期号
        ///     (河南福彩网)
        /// </summary>
        private void DoYesterdayFailedListByMainUrl()
        {
            if (!string.IsNullOrEmpty(Config.MainUrl) && FailedQiHaoList.Count > 0)
            {
                var OpenList = GetOpenListFromMainUrl(Config.MainUrl);
                if (OpenList.Count == 0) return; //无抓取数据
                OpenCode5DTModel matchItem = null;
                var SuccessList = new List<string>();
                foreach (var failedQiHao in FailedQiHaoList)
                {
                    matchItem = OpenList.Where(R => R.Term.ToString() == failedQiHao).FirstOrDefault();
                    if (matchItem != null && OptimizeMainModel(ref matchItem) &&
                        services.AddDTOpen5Code(currentLottery, matchItem))
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
        private List<OpenCode5DTModel> GetOpenListFromMainUrl(string mainUrl)
        {
            var result = new List<OpenCode5DTModel>();
            try
            {
                var url = new Uri(mainUrl);
                var htmlResource = NetHelper.GetUrlResponse(url.AbsoluteUri);
                if (htmlResource == null) return result;

                var doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                var table = doc.DocumentNode.SelectSingleNode("//table");
                if (table == null) return result;
                var trs = table.ChildNodes.Where(S => S.Name.ToLower() == "tr").ToList();
                List<HtmlNode> tds = null;
                string term = string.Empty, openCodeString = string.Empty, optimizeUrl = string.Empty;
                decimal sales = 0, pool = 0;
                OpenCode5DTModel model = null;
                HtmlNode nodeA = null;
                for (var i = 1; i == trs.Count; i++)
                {
                    tds = trs[i].ChildNodes.Where(S => S.Name.ToLower() == "td").ToList();
                    if (tds.Count < 8) continue;
                    model = new OpenCode5DTModel();
                    term = tds[0].InnerText.Trim();
                    if (term.StartsWith((CommonHelper.SCCSysDateTime.Year - 1).ToString())) break;
                    model.Term = Convert.ToInt64(tds[0].InnerText.Trim());
                    openCodeString = tds[1].InnerText.Trim();
                    model.OpenCode1 = Convert.ToInt32(openCodeString.Substring(0, 2));
                    model.OpenCode2 = Convert.ToInt32(openCodeString.Substring(2, 2));
                    model.OpenCode3 = Convert.ToInt32(openCodeString.Substring(4, 2));
                    model.OpenCode4 = Convert.ToInt32(openCodeString.Substring(6, 2));
                    model.OpenCode5 = Convert.ToInt32(openCodeString.Substring(8, 2));
                    model.OpenTime = Convert.ToDateTime(tds[6].InnerText.Trim());
                    sales = Convert.ToDecimal(tds[2].InnerText.Trim());
                    pool = Convert.ToDecimal(tds[5].InnerText.Trim());
                    model.Spare = sales + "|" + pool;
                    nodeA = tds[7].ChildNodes.Where(N => N.Name.ToLower() == "a").FirstOrDefault();
                    if (nodeA == null) continue;
                    optimizeUrl = nodeA.Attributes["href"].Value;
                    model.DetailUrl = new Uri(url, optimizeUrl).AbsoluteUri;
                    result.Add(model);
                }

                var checkDataHelper = new CheckDataHelper();
                var dbdata = services.GetListS<OpenCode5DTModel>(currentLottery)
                    .ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr());
                checkDataHelper.CheckData(dbdata, result.ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr()),
                    Config.Area, currentLottery);
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
        private bool OptimizeMainModel(ref OpenCode5DTModel model)
        {
            try
            {
                var entity = new KaijiangDetailsEntity();
                entity.KaiJiangItems = new List<Kaijiangitem>();
                var htmlResource = NetHelper.GetUrlResponse(model.DetailUrl);
                if (string.IsNullOrWhiteSpace(htmlResource)) return false;
                var doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                var table = doc.DocumentNode.SelectSingleNode("//table");
                if (table == null) return false;
                var trs = table.ChildNodes.Where(C => C.Name == "tr").ToList();
                List<HtmlNode> tds = null;
                foreach (var tr in trs)
                {
                    tds = tr.ChildNodes.Where(S => S.Name.ToLower() == "td").ToList();
                    if (tds.Count < 3) continue;
                    if (tds[0].InnerText == "一等奖")
                    {
                        var tmp = new Kaijiangitem
                        {
                            Name = "一等奖",
                            Total = tds[1].InnerText.Replace("注", ""),
                            TotalMoney = tds[2].InnerText.Replace("元", "")
                        };
                        entity.KaiJiangItems.Add(tmp);
                        // Level1Num = Convert.ToInt32(tds[1].InnerText.Replace("注", ""));
                        // Level1Money = Convert.ToInt32(tds[2].InnerText.Replace("元", ""));
                    }
                    else if (tds[0].InnerText == "二等奖")
                    {
                        var tmp = new Kaijiangitem
                        {
                            Name = "二等奖",
                            Total = tds[1].InnerText.Replace("注", ""),
                            TotalMoney = tds[2].InnerText.Replace("元", "")
                        };
                        entity.KaiJiangItems.Add(tmp);
                    }
                    else if (tds[0].InnerText == "三等奖")
                    {
                        var tmp = new Kaijiangitem
                        {
                            Name = "三等奖",
                            Total = tds[1].InnerText.Replace("注", ""),
                            TotalMoney = tds[2].InnerText.Replace("元", "")
                        };
                        entity.KaiJiangItems.Add(tmp);
                    }
                    else if (tds[0].InnerText == "好运二")
                    {
                        var tmp = new Kaijiangitem
                        {
                            Name = "好运二",
                            Total = tds[1].InnerText.Replace("注", ""),
                            TotalMoney = tds[2].InnerText.Replace("元", "")
                        };
                        entity.KaiJiangItems.Add(tmp);
                    }
                    else if (tds[0].InnerText == "好运三")
                    {
                        var tmp = new Kaijiangitem
                        {
                            Name = "好运三",
                            Total = tds[1].InnerText.Replace("注", ""),
                            TotalMoney = tds[2].InnerText.Replace("元", "")
                        };
                        entity.KaiJiangItems.Add(tmp);
                    }
                    else if (tds[0].InnerText == "好运四")
                    {
                        var tmp = new Kaijiangitem
                        {
                            Name = "好运四",
                            Total = tds[1].InnerText.Replace("注", ""),
                            TotalMoney = tds[2].InnerText.Replace("元", "")
                        };
                        entity.KaiJiangItems.Add(tmp);
                    }
                }

                var salesjackpot = model.Spare.Split('|');
                entity.Trje = salesjackpot[0];
                entity.Gdje = salesjackpot[1];
                model.Spare = entity.TryToJson();
                //string.Format("{0},{1}^一等奖|{2}|{3},二等奖|{4}|{5},三等奖|{6}|{7},好运二|{8}|{9},好运三|{10}|{11},好运四|{12}|{13}",
                // salesjackpot[0], salesjackpot[1], Level1Num, Level1Money, Level2Num, Level2Money, Level3Num, Level3Money,
                //  Level4Num, Level4Money, Level5Num, Level5Money, Level6Num, Level6Money);
                return true;
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
        ///     (彩票两元网)
        /// </summary>
        private void DoTodayJobByBackUrl()
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
                    if (matchItem != null && OptimizeBackModel(ref matchItem) &&
                        services.AddDTOpen5Code(currentLottery, matchItem))
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
        ///     (彩票两元网)
        /// </summary>
        private void DoYesterdayFailedListByBackUrl()
        {
            if (!string.IsNullOrEmpty(Config.BackUrl) && FailedQiHaoList.Count > 0)
            {
                var OpenList = GetOpenListFromBackUrl();
                if (OpenList.Count == 0) return; //无抓取数据
                OpenCode5DTModel matchItem = null;
                var SuccessList = new List<string>();
                foreach (var failedQiHao in FailedQiHaoList)
                {
                    matchItem = OpenList.Where(R => R.Term.ToString() == failedQiHao).FirstOrDefault();
                    if (matchItem != null && OptimizeBackModel(ref matchItem) &&
                        services.AddDTOpen5Code(currentLottery, matchItem))
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
        ///     获取备用站点开奖列表数据
        /// </summary>
        /// <returns></returns>
        private List<OpenCode5DTModel> GetOpenListFromBackUrl()
        {
            var result = new List<OpenCode5DTModel>();
            try
            {
                var url = new Uri(Config.BackUrl);
                var htmlResource = NetHelper.GetUrlResponse(Config.BackUrl, Encoding.GetEncoding("gb2312"));
                if (htmlResource == null) return result;

                var doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                var table = doc.DocumentNode.SelectSingleNode("//table");
                if (table == null) return result;
                var trs = table.ChildNodes.Where(node => node.Name == "tr").ToList();
                OpenCode5DTModel model = null;
                HtmlNode nodeA = null;
                var optimizeUrl = string.Empty;
                for (var i = 2; i < trs.Count; i++) //第一二行为表头
                {
                    var trstyle = trs[i].Attributes["style"];
                    if (trstyle != null && trstyle.Value == "display:none") continue;
                    var tds = trs[i].ChildNodes.Where(node => node.Name == "td").ToList();
                    if (tds.Count < 10) continue;
                    model = new OpenCode5DTModel();
                    nodeA = tds[0].ChildNodes.Where(n => n.Name == "a").FirstOrDefault();
                    if (nodeA == null) continue;
                    model.Term = Convert.ToInt64(nodeA.InnerText.Trim());
                    optimizeUrl = nodeA.Attributes["href"].Value;
                    model.DetailUrl = new Uri(url, optimizeUrl).AbsoluteUri;
                    model.OpenTime = Convert.ToDateTime(tds[9].InnerText);
                    if (tds[1].ChildNodes.Count == 0) continue;
                    var opencodeNode = tds[1].ChildNodes.Where(n => n.Name.ToLower() == "i").ToList();
                    if (opencodeNode.Count < 5) continue;
                    model.OpenCode1 = Convert.ToInt32(opencodeNode[0].InnerText.Trim());
                    model.OpenCode2 = Convert.ToInt32(opencodeNode[1].InnerText.Trim());
                    model.OpenCode3 = Convert.ToInt32(opencodeNode[2].InnerText.Trim());
                    model.OpenCode4 = Convert.ToInt32(opencodeNode[3].InnerText.Trim());
                    model.OpenCode5 = Convert.ToInt32(opencodeNode[4].InnerText.Trim());
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
                    string.Format("【{0}】通过备用站点抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            return result;
        }

        /// <summary>
        ///     完善备用站点江苏体彩7位数开奖实体信息
        /// </summary>
        /// <param name="model"></param>
        private bool OptimizeBackModel(ref OpenCode5DTModel model)
        {
            try
            {
                var entity = new KaijiangDetailsEntity();
                entity.KaiJiangItems = new List<Kaijiangitem>();
                var htmlResource = NetHelper.GetUrlResponse(model.DetailUrl, Encoding.GetEncoding("gb2312"));
                if (string.IsNullOrWhiteSpace(htmlResource)) return false;
                var doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                var table = doc.DocumentNode.SelectSingleNode("//table");
                if (table == null) return false;
                var trs = table.ChildNodes.Where(N => N.Name.ToLower() == "tr").ToList();
                List<HtmlNode> tds = null;
                for (var i = 1; i < trs.Count; i++) //第一行为表头
                {
                    tds = trs[i].ChildNodes.Where(N => N.Name.ToLower() == "td").ToList();
                    if (tds.Count < 5) continue;
                    if (tds[1].InnerText == "一等奖")
                    {
                        var tmp = new Kaijiangitem
                        {
                            Name = "一等奖",
                            Total = tds[2].InnerText,
                            TotalMoney = tds[3].InnerText
                        };
                        entity.KaiJiangItems.Add(tmp);
                        //Level1Num = Convert.ToInt32(tds[2].InnerText);
                        // Level1Money = Convert.ToDecimal(tds[3].InnerText);
                    }

                    if (tds[1].InnerText == "二等奖")
                    {
                        var tmp = new Kaijiangitem
                        {
                            Name = "二等奖",
                            Total = tds[2].InnerText,
                            TotalMoney = tds[3].InnerText
                        };
                        entity.KaiJiangItems.Add(tmp);
                    }

                    if (tds[1].InnerText == "三等奖")
                    {
                        var tmp = new Kaijiangitem
                        {
                            Name = "三等奖",
                            Total = tds[2].InnerText,
                            TotalMoney = tds[3].InnerText
                        };
                        entity.KaiJiangItems.Add(tmp);
                    }

                    if (tds[1].InnerText == "好运二")
                    {
                        var tmp = new Kaijiangitem
                        {
                            Name = "好运二",
                            Total = tds[2].InnerText,
                            TotalMoney = tds[3].InnerText
                        };
                        entity.KaiJiangItems.Add(tmp);
                    }

                    if (tds[1].InnerText == "好运三")
                    {
                        var tmp = new Kaijiangitem
                        {
                            Name = "好运三",
                            Total = tds[2].InnerText,
                            TotalMoney = tds[3].InnerText
                        };
                        entity.KaiJiangItems.Add(tmp);
                    }

                    if (tds[1].InnerText == "好运四")
                    {
                        var tmp = new Kaijiangitem
                        {
                            Name = "好运四",
                            Total = tds[2].InnerText,
                            TotalMoney = tds[3].InnerText
                        };
                        entity.KaiJiangItems.Add(tmp);
                    }
                }

                var reg = new Regex(@"本期投注总额：([\d.,]*?)元");
                var match = reg.Match(htmlResource);
                if (match.Success) entity.Trje = match.Result("$1").Replace(",", string.Empty);
                reg = new Regex(@"奖池资金累计金额：([\d.,]*?)元");
                match = reg.Match(htmlResource);
                if (match.Success) entity.Gdje = match.Result("$1").Replace(",", string.Empty);
                model.Spare = entity.TryToJson();
                //string.Format("{0},{1}^一等奖|{2}|{3},二等奖|{4}|{5},三等奖|{6}|{7},好运二|{8}|{9},好运三|{10}|{11},好运四|{12}|{13}",
                // Sales, Jackpot, Level1Num, Level1Money, Level2Num, Level2Money, Level3Num, Level3Money,
                //  Level4Num, Level4Money, Level5Num, Level5Money, Level6Num, Level6Money);
                return true;
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过备用站点优化开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            return false;
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
        private SCCLottery currentLottery => SCCLottery.HeNan22X5;

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