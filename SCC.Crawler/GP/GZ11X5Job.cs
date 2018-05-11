using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Quartz;
using SCC.Common;
using SCC.Interface;
using SCC.Models;

namespace SCC.Crawler.GP
{
    /// <summary>
    ///     贵州11选5
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class GZ11X5Job : IJob
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        public GZ11X5Job()
        {
            log = new LogHelper();
            services = IOC.Resolve<IOpen5Code>();
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
            LatestQiHao = context.JobDetail.JobDataMap.GetString("LatestQiHao");
            try
            {
                //服务启动时配置初始数据
                if (string.IsNullOrEmpty(LatestQiHao))
                {
                    var lastItem = services.GetLastItem(currentLottery);
                    if (lastItem != null) LatestQiHao = lastItem.Term.ToString();
                }

                //第一次启动服务或最新期号为昨天的开奖期号，则自检昨天开奖数据是否抓取完毕(否则插入邮件数据)，并重置当天期号和失败列表
                if (string.IsNullOrEmpty(LatestQiHao) ||
                    !LatestQiHao.StartsWith(CommonHelper.SCCSysDateTime.ToString("yyMMdd")))
                {
                    CheckingYesterdayTheLotteryData();
                    LatestQiHao = CommonHelper.GenerateTodayQiHaoYYMMDDQQ(0);
                }

                //当最新期号不符合当天总期数，执行当天作业
                if (Convert.ToInt32(LatestQiHao.Substring(6)) != Config.TimesPerDay)
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

            //保存最新期号和失败期号列表
            context.JobDetail.JobDataMap["LatestQiHao"] = LatestQiHao;
        }

        /// <summary>
        ///     自检昨天开奖数据
        /// </summary>
        private void CheckingYesterdayTheLotteryData()
        {
            if (Config.SkipDate.Contains(CommonHelper.SCCSysDateTime.AddDays(-1).ToString("yyyyMMdd")))
                return; //如果昨日设定不开奖则不自检昨日开奖数据
            //从数据库中获取昨天数据抓取失败列表
            FailedQiHaoList = services.GetYesterdayFailQQList(currentLottery, Config.TimesPerDay);
            if (FailedQiHaoList.Count > 0)
            {
                DoYesterdayFailedListByMainUrl();
                DoYesterdayFailedListByBackUrl();
            }
        }

        /// <summary>
        ///     通过主站点抓取开奖数据
        ///     (贵州体彩网)
        /// </summary>
        private void DoTodayJobByMainUrl()
        {
            if (!string.IsNullOrEmpty(Config.MainUrl))
            {
                var OpenList = GetTodayOpenListFromMainUrl();
                if (OpenList.Count == 0) return; //无抓取数据
                var newestQiHao = OpenList.First().Key;
                var startQiNum = Convert.ToInt32(LatestQiHao.Substring(6)) + 1;
                var newestQiNum = Convert.ToInt32(newestQiHao.Substring(6));
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
                var total = OpenList.Count; //处理最新开奖数据
                var getQiHao = string.Empty;
                for (var i = startQiNum; i <= newestQiNum; i++)
                {
                    getQiHao = CommonHelper.GenerateTodayQiHaoYYMMDDQQ(i);
                    var matchItem = OpenList.Where(R => R.Key == getQiHao).FirstOrDefault();
                    var step = 0;
                    var nowQiHao = Convert.ToInt32(getQiHao);
                    while (matchItem.Key == null)
                        if (step <= total)
                        {
                            nowQiHao++;
                            step++;
                            matchItem = OpenList.Where(R => R.Key.ToString() == nowQiHao.ToString()).FirstOrDefault();
                        }
                        else
                        {
                            break;
                        }

                    if (matchItem.Key != null && SaveRecord(getQiHao, matchItem.Value, false))
                    {
                        //处理成功写入日志
                        log.Info(GetType(), CommonHelper.GetJobMainLogInfo(Config, getQiHao));
                        LatestQiHao = getQiHao;
                    }
                }
            }
        }

        /// <summary>
        ///     通过主站抓取错误期号列表中每一个期号
        ///     (贵州体彩网)
        /// </summary>
        private void DoYesterdayFailedListByMainUrl()
        {
            if (!string.IsNullOrEmpty(Config.MainUrl) && FailedQiHaoList.Count > 0)
            {
                var OpenList = GetYesterdayOpenListFromMainUrl();
                if (OpenList.Count == 0) return; //无抓取数据
                var total = OpenList.Count; //处理最新开奖数据
                var SuccessList = new List<string>();
                foreach (var failedQiHao in FailedQiHaoList)
                {
                    var matchItem = OpenList.Where(R => R.Key == failedQiHao).FirstOrDefault();
                    var step = 0;
                    var nowQiHao = Convert.ToInt32(failedQiHao);
                    while (matchItem.Key == null)
                        if (step <= total)
                        {
                            nowQiHao++;
                            step++;
                            matchItem = OpenList.Where(R => R.Key.ToString() == nowQiHao.ToString()).FirstOrDefault();
                        }
                        else
                        {
                            break;
                        }

                    if (matchItem.Key != null && SaveRecord(failedQiHao, matchItem.Value, true))
                    {
                        //处理成功写入日志
                        log.Info(GetType(), CommonHelper.GetJobMainLogInfo(Config, failedQiHao));
                        SuccessList.Add(failedQiHao);
                    }
                }

                foreach (var successQiHao in SuccessList) FailedQiHaoList.Remove(successQiHao);
            }
        }

        /// <summary>
        ///     通过主站点抓取今日最新开奖数据
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetTodayOpenListFromMainUrl()
        {
            var requestUrl = Config.MainUrl +
                             string.Format("&type=3&pubdate={0}", CommonHelper.SCCSysDateTime.ToString("yyyy-MM-dd"));
            return GetOpenListFromMainUrl(requestUrl);
        }

        /// <summary>
        ///     通过主站点抓取昨日开奖数据
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetYesterdayOpenListFromMainUrl()
        {
            var requestUrl = Config.MainUrl + string.Format("&type=3&pubdate={0}",
                                 CommonHelper.SCCSysDateTime.AddDays(-1).ToString("yyyy-MM-dd"));
            return GetOpenListFromMainUrl(requestUrl);
        }

        /// <summary>
        ///     抓取主站点开奖数据
        /// </summary>
        /// <param name="url">主站点</param>
        /// <returns></returns>
        private Dictionary<string, string> GetOpenListFromMainUrl(string url)
        {
            var result = new Dictionary<string, string>();
            try
            {
                var HtmlResource = NetHelper.GetUrlResponse(url);
                if (HtmlResource == null) return result;

                var jsonString = HtmlResource.Replace("null([", string.Empty).Trim();
                var obj = jsonString.Substring(0, jsonString.Length - 2).JsonToEntity<JObject>();

                if (obj != null && obj["kjgplist"] != null)
                {
                    var matchQiHao = string.Empty;
                    var matchKJHaoMa = string.Empty;
                    foreach (var item in obj["kjgplist"])
                    {
                        matchQiHao = item["qihao"].ToString();
                        matchKJHaoMa = item["haoma"].ToString();
                        if (!result.ContainsKey(matchQiHao))
                            result.Add(matchQiHao, matchKJHaoMa);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过主站点抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + Config.LotteryName, ex.Message));
            }

            return result;
        }

        /// <summary>
        ///     通过备用站点抓取开奖数据
        ///     (爱彩乐)
        /// </summary>
        private void DoTodayJobByBackUrl()
        {
            if (!string.IsNullOrEmpty(Config.BackUrl))
            {
                var OpenList = GetTodayOpenListFromBackUrl();
                if (OpenList.Count == 0) return; //无抓取数据
                var newestQiHao = OpenList.First().Key;
                var startQiNum = Convert.ToInt32(LatestQiHao.Substring(6)) + 1;
                var newestQiNum = Convert.ToInt32(newestQiHao.Substring(6));
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
                var getQiHao = string.Empty;
                var total = OpenList.Count; //处理最新开奖数据

                for (var i = startQiNum; i <= newestQiNum; i++)
                {
                    getQiHao = CommonHelper.GenerateTodayQiHaoYYMMDDQQ(i);
                    var matchItem = OpenList.Where(R => R.Key == getQiHao).FirstOrDefault();
                    var step = 0;
                    var nowQiHao = Convert.ToInt32(getQiHao);
                    while (matchItem.Key == null)
                        if (step <= total)
                        {
                            nowQiHao++;
                            step++;
                            matchItem = OpenList.Where(R => R.Key.ToString() == nowQiHao.ToString()).FirstOrDefault();
                        }
                        else
                        {
                            break;
                        }

                    if (matchItem.Key != null && SaveRecord(getQiHao, matchItem.Value, false))
                    {
                        //处理成功写入日志
                        log.Info(GetType(), CommonHelper.GetJobBackLogInfo(Config, getQiHao));
                        LatestQiHao = getQiHao;
                    }
                }
            }
        }

        /// <summary>
        ///     通过备用地址抓取错误期号列表中每一个期号
        ///     (爱彩乐)
        /// </summary>
        private void DoYesterdayFailedListByBackUrl()
        {
            if (!string.IsNullOrEmpty(Config.BackUrl) && FailedQiHaoList.Count > 0)
            {
                var OpenList = GetYesterdayOpenListFromBackUrl();
                if (OpenList.Count == 0) return; //无抓取数据
                var total = OpenList.Count; //处理最新开奖数据
                var SuccessList = new List<string>();
                foreach (var failedQiHao in FailedQiHaoList)
                {
                    var matchItem = OpenList.Where(R => R.Key == failedQiHao).FirstOrDefault();
                    var step = 0;
                    var nowQiHao = Convert.ToInt32(failedQiHao);
                    while (matchItem.Key == null)
                        if (step <= total)
                        {
                            nowQiHao++;
                            step++;
                            matchItem = OpenList.Where(R => R.Key.ToString() == nowQiHao.ToString()).FirstOrDefault();
                        }
                        else
                        {
                            break;
                        }

                    if (matchItem.Key != null && SaveRecord(failedQiHao, matchItem.Value, true))
                    {
                        //处理成功写入日志
                        log.Info(GetType(), CommonHelper.GetJobBackLogInfo(Config, failedQiHao));
                        SuccessList.Add(failedQiHao);
                    }
                }

                foreach (var successQiHao in SuccessList) FailedQiHaoList.Remove(successQiHao);
            }
        }

        /// <summary>
        ///     通过备用站点抓取今日最新开奖数据
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetTodayOpenListFromBackUrl()
        {
            var result = new Dictionary<string, string>();
            try
            {
                var HtmlResource = NetHelper.GetUrlResponse(Config.BackUrl);
                if (HtmlResource == null) return result;

                if (!string.IsNullOrWhiteSpace(HtmlResource))
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(HtmlResource);
                    var table = doc.DocumentNode.SelectSingleNode("//table");
                    if (table == null) return result;
                    var trs = table.ChildNodes.Where(R => R.Name.ToLower() == "tr").ToList();
                    List<HtmlNode> tds = null, ems = null;
                    var matchQiHao = string.Empty;
                    var matchKJHaoMa = string.Empty;
                    for (var i = 1; i < trs.Count; i++) //第一行为表头
                    {
                        tds = trs[i].ChildNodes.Where(R => R.Name.ToLower() == "td").ToList();
                        if (tds.Count < 3) continue;
                        matchQiHao = tds[0].InnerText.Trim();
                        ems = tds[2].ChildNodes.Where(R => R.Name.ToLower() == "em").ToList();
                        if (ems.Count < 5) continue;
                        matchKJHaoMa = string.Format("{0},{1},{2},{3},{4}", ems[0].InnerText.Trim(),
                            ems[1].InnerText.Trim(), ems[2].InnerText.Trim(), ems[3].InnerText.Trim(),
                            ems[4].InnerText.Trim());
                        if (!result.ContainsKey(matchQiHao))
                            result.Add(matchQiHao, matchKJHaoMa);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过备用站点抓取今日最新开奖列表时发生错误，错误信息【{1}】", Config.Area + Config.LotteryName,
                        ex.Message));
            }

            return result;
        }

        /// <summary>
        ///     通过备用站点抓取昨日开奖列表
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetYesterdayOpenListFromBackUrl()
        {
            var result = new Dictionary<string, string>();
            try
            {
                var htmlResource =
                    NetHelper.GetUrlResponse(Config.BackUrl + "?action=chart&date=yesterday&id=524&async=true");
                if (!string.IsNullOrWhiteSpace(htmlResource))
                {
                    var obj = htmlResource.JsonToEntity<JObject>();
                    if (obj != null && obj["data"] != null)
                    {
                        var matchQiHao = string.Empty;
                        var matchKJHaoMa = string.Empty;
                        JArray openCodeList = null;
                        foreach (var item in obj["data"])
                        {
                            matchQiHao = item["dateNumber"].ToString();
                            openCodeList = (JArray) item["list"];
                            matchKJHaoMa = string.Format("{0},{1},{2},{3},{4}", openCodeList[0], openCodeList[1],
                                openCodeList[2], openCodeList[3], openCodeList[4]);
                            if (!result.ContainsKey(matchQiHao))
                                result.Add(matchQiHao, matchKJHaoMa);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过备用站点抓取昨日开奖列表时发生错误，错误信息【{1}】", Config.Area + Config.LotteryName, ex.Message));
            }

            return result;
        }

        /// <summary>
        ///     将此彩种指定期号和开奖号码保存至数据库
        /// </summary>
        /// <param name="QiHao">期号</param>
        /// <param name="OpenCode">开奖号码(形如01,02,03,04,05)</param>
        /// <param name="IsYesterdayRecord">是否是保存昨天的记录</param>
        /// <returns></returns>
        private bool SaveRecord(string QiHao, string OpenCode, bool IsYesterdayRecord)
        {
            if (!string.IsNullOrWhiteSpace(QiHao) && !string.IsNullOrWhiteSpace(OpenCode))
            {
                var model = new OpenCode5Model();
                model.Term = Convert.ToInt64(QiHao);
                var haoMaArray = OpenCode.Split(',');
                model.OpenCode1 = Convert.ToInt32(haoMaArray[0]);
                model.OpenCode2 = Convert.ToInt32(haoMaArray[1]);
                model.OpenCode3 = Convert.ToInt32(haoMaArray[2]);
                model.OpenCode4 = Convert.ToInt32(haoMaArray[3]);
                model.OpenCode5 = Convert.ToInt32(haoMaArray[4]);
                if (IsYesterdayRecord)
                    model.OpenTime = CommonHelper.GenerateYesterdayOpenTime(Config, QiHao);
                else
                    model.OpenTime = CommonHelper.GenerateTodayOpenTime(Config, QiHao);
                return services.AddOpen5Code(currentLottery, model);
            }

            return false;
        }

        #region Attribute

        /// <summary>
        ///     配置信息
        /// </summary>
        private SCCConfig Config;

        /// <summary>
        ///     当天抓取的最新一期期号
        /// </summary>
        private string LatestQiHao;

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
        private readonly IOpen5Code services;

        /// <summary>
        ///     当前彩种
        /// </summary>
        private SCCLottery currentLottery => SCCLottery.GuiZhou11x5;

        /// <summary>
        ///     邮件接口
        /// </summary>
        private IEmail email;

        #endregion
    }
}