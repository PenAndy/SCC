using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Quartz;
using SCC.Common;
using SCC.Crawler.Tools;
using SCC.Interface;
using SCC.Models;
using SCC.Services;

namespace SCC.Crawler.GP
{
    /// <summary>
    ///     山西11选5
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class ShanXiTaiYuan11x5Job : IJob
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        public ShanXiTaiYuan11x5Job()
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
                log.Error(GetType(), string.Format("【{0}】抓取时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
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
        ///     (山西体彩网)
        /// </summary>
        private void DoTodayJobByMainUrl()
        {
            if (!string.IsNullOrEmpty(Config.MainUrl))
            {
                var OpenList = GetDocByMainUrl();
                if (OpenList.Count == 0) return; //无抓取数据
                var newestQiHao = OpenList.Max(w => w.Key);
                var startQiNum = Convert.ToInt32(LatestQiHao.Substring(6)) + 1;
                var newestQiNum = Convert.ToInt32(newestQiHao.Substring(6));
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
                var total = OpenList.Count;
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
        ///     (山西体彩网)
        /// </summary>
        private void DoYesterdayFailedListByMainUrl()
        {
            if (!string.IsNullOrEmpty(Config.MainUrl) && FailedQiHaoList.Count > 0)
            {
                var OpenList = GetDocByMainUrl(false);
                if (OpenList.Count == 0) return; //无抓取数据
                var total = OpenList.Count;
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
        private Dictionary<string, string> GetTodayOpenListFromBackUrl()
        {
            var result = new Dictionary<string, string>();
            try
            {
                var todayFirstQiHao = CommonHelper.GenerateTodayQiHaoYYMMDDQQ(1);
                var IsGetTodayAllOpenCode = false;

                var matchQiHao = string.Empty;
                var matchKJHaoMa = string.Empty;
                for (var k = 1; k <= 3; k++)
                {
                    if (IsGetTodayAllOpenCode) continue;
                    var requestUrl = string.Format(Config.BackUrl, k) + "&moreList=";
                    var HtmlResource = NetHelper.GetShanXiTaiYuan11x5UrlResponse(requestUrl);
                    if (HtmlResource == null) return result;

                    var doc = new HtmlDocument();
                    doc.LoadHtml(HtmlResource);
                    var table = doc.DocumentNode.SelectSingleNode("//table");
                    if (table == null) continue;
                    var trs = table.ChildNodes.Where(R => R.Name.ToLower() == "tr").ToList();
                    if (trs.Count < 3) continue;
                    for (var i = 2; i < trs.Count - 1; i++) //第一二行为表头,最后一行为页码
                    {
                        if (IsGetTodayAllOpenCode) continue;
                        var tds = trs[i].ChildNodes.Where(R => R.Name.ToLower() == "td").ToList();
                        if (tds.Count < 6) continue;
                        matchQiHao = tds[1].InnerText.Replace("期", string.Empty).Trim();
                        matchKJHaoMa = string.Format("{0},{1},{2},{3},{4}", tds[2].InnerText.Trim(),
                            tds[3].InnerText.Trim(), tds[4].InnerText.Trim(), tds[5].InnerText.Trim(),
                            tds[6].InnerText.Trim());
                        if (!result.ContainsKey(matchQiHao))
                            result.Add(matchQiHao, CommonHelper.TRHandleCode(matchKJHaoMa));
                        if (matchQiHao == todayFirstQiHao) IsGetTodayAllOpenCode = true;
                    }

                    var checkDataHelper = new CheckDataHelper();
                    var dbdata = services.GetListIn(currentLottery, true)
                        .ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr());
                    checkDataHelper.CheckData(dbdata, result, Config.Area, currentLottery);
                }
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过副站点抓取今日最新开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            return result;
        }

        /// <summary>
        ///     通过主站点抓取昨日开奖数据
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetYesterdayOpenListFromBackUrl()
        {
            var result = new Dictionary<string, string>();
            try
            {
                var matchQiHao = string.Empty;
                var matchKJHaoMa = string.Empty;
                for (var k = 1; k <= 6; k++)
                {
                    var requestUrl = string.Format(Config.BackUrl, k) + "&moreList=";
                    var HtmlResource = NetHelper.GetShanXiTaiYuan11x5UrlResponse(requestUrl);
                    if (!string.IsNullOrWhiteSpace(HtmlResource))
                    {
                        var doc = new HtmlDocument();
                        doc.LoadHtml(HtmlResource);
                        var table = doc.DocumentNode.SelectSingleNode("//table");
                        if (table == null) continue;
                        var trs = table.ChildNodes.Where(R => R.Name.ToLower() == "tr").ToList();
                        if (trs.Count < 3) continue;
                        for (var i = 2; i < trs.Count - 1; i++) //第一二行为表头,最后一行为页码
                        {
                            var tds = trs[i].ChildNodes.Where(R => R.Name.ToLower() == "td").ToList();
                            if (tds.Count < 6) continue;
                            matchQiHao = tds[1].InnerText.Replace("期", string.Empty).Trim();
                            matchKJHaoMa = string.Format("{0},{1},{2},{3},{4}", tds[2].InnerText.Trim(),
                                tds[3].InnerText.Trim(), tds[4].InnerText.Trim(), tds[5].InnerText.Trim(),
                                tds[6].InnerText.Trim());
                            if (!result.ContainsKey(matchQiHao))
                                result.Add(matchQiHao, CommonHelper.TRHandleCode(matchKJHaoMa));
                        }

                        var checkDataHelper = new CheckDataHelper();
                        var dbdata = services.GetListIn(currentLottery, false)
                            .ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr());
                        checkDataHelper.CheckData(dbdata, result, Config.Area, currentLottery);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过主站点抓取昨日开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            return result;
        }

        /// <summary>
        ///     从主站抓取开奖数据 昨天/今天
        /// </summary>
        /// <param name="IsToday"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetDocByMainUrl(bool IsToday = true)
        {
            var result = new Dictionary<string, string>();
            try
            {
                var time = DateTime.Now;
                var arg = new OpenCaiApiArg
                {
                    code = EnumHelper.GetLotteryCode(SCCLottery.ShanXiTaiYuan11x5),
                    // rows = int.Parse(GetPeriodsNumberToDay(time, IsToday)),
                    date = time.ToString("yyyy-MM-dd")
                };
                if (!IsToday) arg.date = time.AddDays(-1).ToString("yyyy-MM-dd");
                var data = OpenCaiApiServices.GetOpenCaiApiData(arg);
                if (data == null) return result;

                if (data.data != null)
                {
                    if (data.data.Count == 0) return result;

                    for (var i = 0; i < data.data.Count; i++)
                        result.Add(data.data[i].GetTermStr(), data.data[i].GetOpenCodeStr());
                    var checkDataHelper = new CheckDataHelper();
                    var dbdata = services.GetListIn(currentLottery, IsToday)
                        .ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr());
                    checkDataHelper.CheckData(dbdata, result, Config.Area, currentLottery);
                    // CheckData(dbdata, result);
                }
            }
            catch (Exception ee)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过站点抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ee.Message));
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
                var total = OpenList.Count;
                var newestQiHao = OpenList.Max(w => w.Key);
                var startQiNum = Convert.ToInt32(LatestQiHao.Substring(6)) + 1;
                var newestQiNum = Convert.ToInt32(newestQiHao.Substring(6));
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
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
                var total = OpenList.Count;
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
        private SCCLottery currentLottery => SCCLottery.ShanXiTaiYuan11x5;

        /// <summary>
        ///     邮件接口
        /// </summary>
        private IEmail email;

        #endregion
    }
}