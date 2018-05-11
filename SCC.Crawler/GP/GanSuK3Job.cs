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
    ///     甘肃快3
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class GanSuK3Job : IJob
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        public GanSuK3Job()
        {
            log = new LogHelper();
            services = IOC.Resolve<IOpen3Code>();
            email = IOC.Resolve<IEmail>();
        }

        /// 作业执行入口
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
        ///     从备用地址执行今天任务
        /// </summary>
        private void DoTodayJobByBackUrl()
        {
            if (!string.IsNullOrEmpty(Config.BackUrl))
            {
                var OpenList = GetDocByBackUrl();
                if (OpenList.Count == 0) return; //无抓取数据
                var newestQiHao = OpenList.Max(w => w.QiHao);
                var startQiNum = Convert.ToInt32(LatestQiHao.Substring(6)) + 1;
                var newestQiNum = Convert.ToInt32(newestQiHao.Substring(6));
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
                var total = OpenList.Count;

                var getQiHao = string.Empty;
                for (var i = startQiNum; i <= newestQiNum; i++)
                {
                    getQiHao = CommonHelper.GenerateTodayQiHaoYYMMDDQQQ(i);
                    var matchItem = OpenList.Where(R => R.QiHao.ToString() == getQiHao).FirstOrDefault();
                    var step = 0;
                    var nowQiHao = Convert.ToInt32(getQiHao);

                    while (matchItem == null)
                        if (step <= total)
                        {
                            nowQiHao++;
                            step++;
                            matchItem = OpenList.Where(R => R.QiHao.ToString() == nowQiHao.ToString()).FirstOrDefault();
                        }
                        else
                        {
                            matchItem = null;
                            break;
                        }

                    if (matchItem != null && SaveRecord(matchItem))
                    {
                        //处理成功写入日志
                        log.Info(GetType(), CommonHelper.GetJobBackLogInfo(Config, getQiHao));
                        LatestQiHao = getQiHao;
                    }
                }
            }
        }

        /// <summary>
        ///     通过主站点抓取开奖数据
        /// </summary>
        private void DoTodayJobByMainUrl()
        {
            var OpenList = GetDocByMainUrl();
            if (OpenList.Count == 0) return; //无抓取数据
            var newestQiHao = OpenList.Max(W => W.QiHao);
            var startQiNum = Convert.ToInt32(LatestQiHao.Substring(6)) + 1;
            var newestQiNum = Convert.ToInt32(newestQiHao.Substring(6));
            if (startQiNum > newestQiNum) return; //无最新数据
            var total = OpenList.Count;
            //处理最新开奖数据
            var getQiHao = string.Empty;
            for (var i = startQiNum; i <= newestQiNum; i++)
            {
                getQiHao = CommonHelper.GenerateTodayQiHaoYYMMDDQQQ(i);
                var matchItem = OpenList.Where(R => R.QiHao.ToString() == getQiHao).FirstOrDefault();
                var step = 0;
                var nowQiHao = Convert.ToInt32(getQiHao);

                while (matchItem == null)
                    if (step <= total)
                    {
                        nowQiHao++;
                        step++;
                        matchItem = OpenList.Where(R => R.QiHao.ToString() == nowQiHao.ToString()).FirstOrDefault();
                    }
                    else
                    {
                        matchItem = null;
                        break;
                    }

                if (matchItem != null && SaveRecord(matchItem))
                {
                    //处理成功写入日志
                    log.Info(GetType(), CommonHelper.GetJobMainLogInfo(Config, getQiHao));
                    LatestQiHao = getQiHao;
                }
            }
        }

        /// <summary>
        ///     从备用网址处理昨天失败的开彩
        /// </summary>
        public void DoYesterdayFailedListByBackUrl()
        {
            if (!string.IsNullOrEmpty(Config.BackUrl) && FailedQiHaoList.Count > 0)
            {
                var OpenList = GetDocByBackUrl(false);
                if (OpenList.Count == 0) return; //无抓取数据
                var total = OpenList.Count;
                var SuccessList = new List<string>();
                foreach (var failedQiHao in FailedQiHaoList)
                {
                    var matchItem = OpenList.Where(R => R.QiHao.ToString() == failedQiHao).FirstOrDefault();
                    var step = 0;
                    var nowQiHao = Convert.ToInt32(failedQiHao);

                    while (matchItem == null)
                        if (step <= total)
                        {
                            nowQiHao++;
                            step++;
                            matchItem = OpenList.Where(R => R.QiHao.ToString() == nowQiHao.ToString()).FirstOrDefault();
                        }
                        else
                        {
                            matchItem = null;
                            break;
                        }

                    if (matchItem != null && SaveRecord(matchItem))
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
        ///     保存开奖信息到数据库中
        /// </summary>
        /// <param name="list"></param>
        /// <summary>
        ///     通过主站抓取错误期号列表中每一个期号
        /// </summary>
        private void DoYesterdayFailedListByMainUrl()
        {
            var OpenList = GetDocByMainUrl(false);
            if (OpenList.Count == 0) return; //无抓取数据
            var total = OpenList.Count;
            var SuccessList = new List<string>();
            foreach (var failedQiHao in FailedQiHaoList)
            {
                var matchItem = OpenList.Where(R => R.QiHao.ToString() == failedQiHao).FirstOrDefault();
                var step = 0;
                var nowQiHao = Convert.ToInt32(failedQiHao);

                while (matchItem == null)
                    if (step <= total)
                    {
                        nowQiHao++;
                        step++;
                        matchItem = OpenList.Where(R => R.QiHao.ToString() == nowQiHao.ToString()).FirstOrDefault();
                    }
                    else
                    {
                        matchItem = null;
                        break;
                    }

                if (matchItem != null && SaveRecord(matchItem))
                {
                    //处理成功写入日志
                    log.Info(GetType(), CommonHelper.GetJobMainLogInfo(Config, failedQiHao));
                    SuccessList.Add(failedQiHao);
                }
            }

            foreach (var successQiHao in SuccessList) FailedQiHaoList.Remove(successQiHao);
        }


        /// <summary>
        ///     从备用站抓取开奖数据 昨天/今天
        /// </summary>
        /// <param name="IsToday"></param>
        /// <returns></returns>
        /// <summary>
        ///     从主站抓取开奖数据 昨天/今天
        /// </summary>
        /// <param name="IsToday"></param>
        /// <returns></returns>
        private List<GanSuK3Entity> GetDocByMainUrl(bool IsToday = true)
        {
            var list = new List<GanSuK3Entity>();
            try
            {
                var time = DateTime.Now;
                var arg = new OpenCaiApiArg
                {
                    code = EnumHelper.GetLotteryCode(SCCLottery.GanSuK3),
                    // rows = int.Parse(GetPeriodsNumberToDay(time, IsToday)),
                    date = time.ToString("yyyy-MM-dd")
                };
                if (!IsToday) arg.date = time.AddDays(-1).ToString("yyyy-MM-dd");
                var data = OpenCaiApiServices.GetOpenCaiApiData(arg);
                if (data == null) return list;
                if (data.data != null)
                {
                    if (data.data.Count == 0) return list;

                    for (var i = 0; i < data.data.Count; i++)
                    {
                        var tmp = new GanSuK3Entity
                        {
                            QiHao = data.data[i].GetTermStr(),
                            KaiJiangHao = data.data[i].GetOpenCodeStr(),
                            OpenTime = DateTime.Parse(data.data[i].opentime)
                        };
                        list.Add(tmp);
                    }

                    var checkDataHelper = new CheckDataHelper();
                    var dbdata = services.GetListIn(currentLottery, IsToday)
                        .ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr());
                    checkDataHelper.CheckData(dbdata, list.ToDictionary(w => w.QiHao.ToString(), w => w.KaiJiangHao),
                        Config.Area, currentLottery);
                }
            }
            catch (Exception ee)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过站点抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ee.Message));
            }

            return list;
        }

        /// <summary>
        ///     将此彩种指定期号和开奖号码保存至数据库
        /// </summary>
        /// <param name="QiHao">期号</param>
        /// <param name="OpenCode">开奖号码(形如01,02,03)</param>
        /// <param name="IsYesterdayRecord">是否是保存昨天的记录</param>
        /// <returns></returns>
        private bool SaveRecord(GanSuK3Entity data)
        {
            var model = new OpenCode3Model();
            model.Term = Convert.ToInt64(data.QiHao); //期号
            var haoMaArray = data.KaiJiangHao.Split(',');
            model.OpenCode1 = Convert.ToInt32(haoMaArray[0]);
            model.OpenCode2 = Convert.ToInt32(haoMaArray[1]);
            model.OpenCode3 = Convert.ToInt32(haoMaArray[2]);

            model.OpenTime = data.OpenTime;
            if (services.AddOpen3Code(currentLottery, model))
            {
                GetMaxPeriodNum((int) model.Term); //添加成功存放期数
                return true;
            }

            return false;
        }

        /// <summary>
        ///     获取今天期数
        /// </summary>
        /// <param name="time"></param>
        /// <param name="IsToday"></param>
        /// <returns></returns>
        public string GetPeriodsNumberToDay(DateTime time, bool IsToday = false)
        {
            if (!IsToday) return Config.TimesPerDay.ToString();
            int hc = 0, mc = 0;
            if (time.Hour >= Config.StartHour) //
            {
                hc = (time.Hour - Config.StartHour) * 6; //已开期数小时部分
                mc = time.Minute / 10; //已开期分钟部分
                if (Config.StartMinute == 0) mc += 1;
            }

            return hc + mc >= 10 ? (hc + mc).ToString() : "0" + (hc + mc); //;
        }

        /// <summary>
        ///     比较期号大的存入LatestQiHao
        /// </summary>
        /// <param name="PeriodNum"></param>
        public void GetMaxPeriodNum(int PeriodNum)
        {
            if (int.Parse(LatestQiHao) < PeriodNum) LatestQiHao = PeriodNum.ToString();
        }

        /// <summary>
        ///     从备用站抓取开奖数据 昨天/今天
        /// </summary>
        /// <param name="IsToday"></param>
        /// <returns></returns>

        #region   爬取爱彩乐网站数据
        private List<GanSuK3Entity> GetDocByBackUrl(bool IsToday = true)
        {
            var list = new List<GanSuK3Entity>();
            try
            {
                var day = "?op=zhzs&num=jt";
                if (!IsToday) day = "?op=zhzs&num=zt";
                var url = string.Format("{0}{1}", Config.BackUrl, day);
                var HtmlResource = NetHelper.GetUrlResponse(url);
                if (!string.IsNullOrWhiteSpace(HtmlResource))
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(HtmlResource);
                    var tables = doc.DocumentNode.SelectNodes("//*[@id=\"fixedtable\"]");
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

                            if (opencode.Count == 3)
                            {
                                var qihao = num;
                                var tmp = new GanSuK3Entity
                                {
                                    QiHao = num.Remove(6, 1),
                                    KaiJiangHao = CommonHelper.TRHandleCode(string.Join(",", opencode)),
                                    OpenTime = GetOpenTimeByPeriodNum(int.Parse(num))
                                };
                                list.Add(tmp);
                            }
                        }
                }

                var checkDataHelper = new CheckDataHelper();
                var dbdata = services.GetListIn(currentLottery, IsToday)
                    .ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr());
                checkDataHelper.CheckData(dbdata, list.ToDictionary(w => w.QiHao, w => w.KaiJiangHao), Config.Area,
                    currentLottery);
            }
            catch (Exception ee)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过站点抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ee.Message));
            }

            return list;
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
            var qi = int.Parse(PeriodNum.ToString().Substring(6, 3)) - 1;
            var time = new DateTime(2000 + year, month, day, Config.StartHour, Config.StartMinute, 0);
            time = time.AddMinutes(qi * Config.Interval);
            return time;
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

        #endregion

        #region Attribute

        /// <summary>
        ///     配置信息
        /// </summary>
        private SCCConfig Config;

        /// <summary>
        ///     当天抓取的最新一期期号
        /// </summary>
        private string LatestQiHao = "0";

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
        private readonly IOpen3Code services;

        /// <summary>
        ///     当前彩种
        /// </summary>
        private SCCLottery currentLottery => SCCLottery.GanSuK3;

        /// <summary>
        ///     邮件接口
        /// </summary>
        private IEmail email;

        #endregion
    }

    public class GanSuK3Entity
    {
        public string QiHao { get; set; }
        public string KaiJiangHao { get; set; }
        public DateTime OpenTime { get; set; }
    }
}