using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    ///     辽宁快乐12
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class LNKL12Job : IJob
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        public LNKL12Job()
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
                    LatestQiHao = CommonHelper.GenerateTodayQiHaoYYMMDDQQQ(0);
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
            FailedQiHaoList = services.GetYesterdayFailQQQList(currentLottery, Config.TimesPerDay);
            if (FailedQiHaoList.Count > 0)
            {
                DoYesterdayFailedListByMainUrl();
                DoYesterdayFailedListByBackUrl();
            }
        }

        /// <summary>
        ///     通过主站点抓取开奖数据
        ///     (爱彩乐)
        /// </summary>
        private void DoTodayJobByMainUrl()
        {
            if (!string.IsNullOrEmpty(Config.MainUrl))
            {
                var OpenList = GetDocByMainUrl();
                if (OpenList.Count == 0) return; //无抓取数据
                var newestQiHao = OpenList.First().Key;
                var startQiNum = Convert.ToInt32(LatestQiHao.Substring(6)) + 1;
                var newestQiNum = Convert.ToInt32(newestQiHao.Substring(6));
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
                var total = OpenList.Count;
                var getQiHao = string.Empty;
                for (var i = startQiNum; i <= newestQiNum; i++)
                {
                    getQiHao = CommonHelper.GenerateTodayQiHaoYYMMDDQQQ(i);
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
        ///     (爱彩乐)
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
                    code = EnumHelper.GetLotteryCode(SCCLottery.LiaoNingKL12),
                    date = time.ToString("yyyy-MM-dd")
                };
                if (!IsToday) arg.date = time.AddDays(-1).ToString("yyyy-MM-dd");
                var data = OpenCaiApiServices.GetOpenCaiApiData(arg);
                if (data == null) return result;

                if (data.data != null)
                {
                    if (data.data.Count == 0) return result;

                    for (var i = 0; i < data.data.Count; i++)
                    {
                        var qi = data.data[i].GetTermStr();
                        var qihao = qi;
                        result.Add(qihao, data.data[i].GetOpenCodeStr());
                    }

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
        ///     (一定牛)
        /// </summary>
        private void DoTodayJobByBackUrl()
        {
            if (!string.IsNullOrEmpty(Config.BackUrl))
            {
                var OpenList = GetTodayOpenListFromBackUrl();
                if (OpenList.Count == 0) return; //无抓取数据
                var newestQiHao = OpenList.Last().Key;
                var startQiNum = Convert.ToInt32(LatestQiHao.Substring(6)) + 1;
                var newestQiNum = Convert.ToInt32(newestQiHao.Substring(6));
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
                var total = OpenList.Count;
                var getQiHao = string.Empty;
                for (var i = startQiNum; i <= newestQiNum; i++)
                {
                    getQiHao = CommonHelper.GenerateTodayQiHaoYYMMDDQQQ(i);
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
        ///     (一定牛)
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
        ///     通过备用站点抓取今日最新开奖数据
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetTodayOpenListFromBackUrl()
        {
            var requestUrl = "?op=dcjb&num=jt";

            return GetOpenListFromBackUrl(Config.BackUrl + requestUrl);
        }

        /// <summary>
        ///     通过备用站点抓取昨日开奖数据
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetYesterdayOpenListFromBackUrl()
        {
            var requestUrl = "?op=dcjb&num=zt";
            // var requestUrl = Config.BackUrl + "&type=rxfbzs&index=5";
            return GetOpenListFromBackUrl(Config.BackUrl + requestUrl, false);
        }

        /// <summary>
        ///     通过备用站点抓取开奖数据
        /// </summary>
        /// <param name="requestUrl">备用站点</param>
        /// <returns></returns>
        private Dictionary<string, string> GetOpenListFromBackUrl(string requestUrl, bool IsToday = true)
        {
            var result = new Dictionary<string, string>();
            try
            {
                //string url = "http://lnkl12.icaile.com/?op=dcjb&num=zt";

                var HtmlResource = NetHelper.GetUrlResponse(requestUrl);
                if (HtmlResource == null) return result;

                var doc = new HtmlDocument();
                doc.LoadHtml(HtmlResource);
                var tables = doc.DocumentNode.SelectNodes("//*[@id=\"fixedtable\"]");
                if (tables == null) return result;

                var matchQiHao = string.Empty;
                var matchKJHaoMa = string.Empty;
                foreach (var item in tables[0].ChildNodes)
                    if (item.HasChildNodes && item.Name == "tr")
                    {
                        var opencode = new List<string>();
                        var num = string.Empty;
                        foreach (var item2 in item.ChildNodes)
                        {
                            if (item2.GetAttributeValue("class", "") == "chart-bg-qh") num = item2.InnerText.Trim();
                            if (item2.GetAttributeValue("style", "") == "display:none;")
                            {
                                //数据加密保存在此，去掉两头然后base64转码
                                var base64str = item2.InnerText.Trim().Substring(1);
                                base64str = base64str.Substring(0, base64str.Length - 1);
                                var codestr = DecodeBase64(Encoding.Default, base64str);
                                opencode.Add(codestr);
                            }
                        }

                        if (opencode.Count == 5)
                        {
                            var qi = num;
                            var qihao = qi.Length == 8 ? qi : qi.Remove(qi.Length - 3, 1);
                            result.Add(qihao, CommonHelper.TRHandleCode(string.Join(",", opencode)));
                            matchQiHao = num.Substring(2);
                        }
                    }

                var checkDataHelper = new CheckDataHelper();
                var dbdata = services.GetListIn(currentLottery, IsToday)
                    .ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr());
                checkDataHelper.CheckData(dbdata, result, Config.Area, currentLottery);
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过备用站点抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            return result;
        }

        /// <summary>
        ///     根据期号得到开奖日期
        /// </summary>
        /// <param name="PeriodNum"></param>
        /// <returns></returns>
        public DateTime GetOpenTimeByPeriodNum(int PeriodNum)
        {
            if (PeriodNum < 10000000) return DateTime.Now;
            var year = int.Parse(PeriodNum.ToString().Substring(0, 2));
            var month = int.Parse(PeriodNum.ToString().Substring(2, 2));
            var day = int.Parse(PeriodNum.ToString().Substring(4, 2));
            var qi = int.Parse(PeriodNum.ToString().Substring(6, 2)) - 1;
            var time = new DateTime(2000 + year, month, day, Config.StartHour, Config.StartMinute, 0);
            time = time.AddMinutes(qi * Config.Interval);
            return time;
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

        /// <summary>
        ///     base64解码
        /// </summary>
        /// <param name="encode"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string DecodeBase64(Encoding encode, string result)
        {
            var decode = "";
            var bytes = Convert.FromBase64String(result);
            try
            {
                decode = encode.GetString(bytes);
            }
            catch
            {
                decode = result;
            }

            return decode;
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
        private SCCLottery currentLottery => SCCLottery.LiaoNingKL12;

        /// <summary>
        ///     邮件接口
        /// </summary>
        private IEmail email;

        #endregion
    }
}