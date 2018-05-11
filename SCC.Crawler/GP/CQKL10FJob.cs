using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
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
    ///     重庆快乐十分
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class CQKL10FJob : IJob
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        public CQKL10FJob()
        {
            log = new LogHelper();
            services = IOC.Resolve<IOpen8Code>();
            email = IOC.Resolve<IEmail>();
        }

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
            FailedQiHaoList = services.GetYesterdayFailQQList(currentLottery, Config.TimesPerDay);
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
                var total = OpenList.Count;
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
                var getQiHao = string.Empty;
                for (var i = startQiNum; i <= newestQiNum; i++)
                {
                    getQiHao = CommonHelper.GenerateTodayQiHaoYYMMDDQQ(i);
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

                    if (matchItem.QiHao != null && SaveRecord(matchItem))
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
            if (!string.IsNullOrEmpty(Config.MainUrl))
            {
                var OpenList =GetDocByMainUrl();
                if (OpenList.Count == 0) return; //无抓取数据
                var newestQiHao = OpenList.Max(w => w.QiHao);
                var startQiNum = Convert.ToInt32(LatestQiHao.Substring(6)) + 1;
                var newestQiNum = Convert.ToInt32(newestQiHao.Substring(6));
                var total = OpenList.Count;
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
                var getQiHao = string.Empty;
                for (var i = startQiNum; i <= newestQiNum; i++)
                {
                    getQiHao = CommonHelper.GenerateTodayQiHaoYYMMDDQQ(i);
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

                    if (matchItem.QiHao != null && SaveRecord(matchItem))
                    {
                        //处理成功写入日志
                        log.Info(GetType(), CommonHelper.GetJobBackLogInfo(Config, getQiHao));
                        LatestQiHao = getQiHao;
                    }
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
            var SuccessList = new List<string>();
            var total = OpenList.Count;

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
        ///     抓取站点开奖数据
        /// </summary>
        /// <param name="url">备用站点</param>
        /// <returns></returns>
        private List<CQKL10FEnitity> GetDocByBackUrl(bool IsToday = true)
        {
            var list = new List<CQKL10FEnitity>();
            try
            {
                var day = DateTime.Now.ToString("yyyyMMdd");
                if (!IsToday) day = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
                var url = string.Format(Config.BackUrl, day);
                //" http://kaijiang.500.com/static/info/kaijiang/xml/bjk3/20171124.xml?_A=YLQMBVJT1511764350898"; 
                //string url = @"http://www.bwlc.net/bulletin/prevqck3.html?page=3";
                var HtmlResource = NetHelper.GetUrlResponse(url);
                if (HtmlResource == null) return list;

                var doc = new XmlDocument();
                doc.LoadXml(HtmlResource);
                var records = doc.SelectNodes("//row");
                if (records == null) return list;

                foreach (XmlNode xmlnode in records)
                {
                    if (xmlnode.Attributes["expect"] == null ||
                        string.IsNullOrWhiteSpace(xmlnode.Attributes["expect"].Value) ||
                        xmlnode.Attributes["opencode"] == null ||
                        string.IsNullOrWhiteSpace(xmlnode.Attributes["opencode"].Value)) continue;
                    var num = xmlnode.Attributes["expect"].Value;
                    var opencode = xmlnode.Attributes["opencode"].Value;
                    var opentime = xmlnode.Attributes["opentime"].Value;
                    var tmp = new CQKL10FEnitity
                    {
                        QiHao = num,
                        KaiJiangHao = opencode,
                        OpenTime = DateTime.Parse(opentime)
                    };
                    list.Add(tmp);
                }

                var checkDataHelper = new CheckDataHelper();
                var dbdata = services.GetListIn(currentLottery, IsToday)
                    .ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr());
                checkDataHelper.CheckData(dbdata, list.ToDictionary(w => w.QiHao.ToString(), w => w.KaiJiangHao),
                    Config.Area, currentLottery);
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过主站点抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            return list;
        }

        /// 从主站抓取开奖数据 昨天/今天
        /// </summary>
        /// <param name="IsToday"></param>
        /// <returns></returns>
        private List<CQKL10FEnitity> GetDocByMainUrl(bool IsToday = true)
        {
            var list = new List<CQKL10FEnitity>();
            try
            {
                var time = DateTime.Now;
                var arg = new OpenCaiApiArg
                {
                    code = EnumHelper.GetLotteryCode(SCCLottery.ChongQingKL10F),
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
                        var tmp = new CQKL10FEnitity
                        {
                            QiHao = data.data[i].expect.Substring(2).Remove(6,1),
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
        private bool SaveRecord(CQKL10FEnitity data)
        {
            var model = new OpenCode8Model();
            model.Term = Convert.ToInt64(data.QiHao); //期号
            var haoMaArray = data.KaiJiangHao.Split(',');
            model.OpenCode1 = Convert.ToInt32(haoMaArray[0]);
            model.OpenCode2 = Convert.ToInt32(haoMaArray[1]);
            model.OpenCode3 = Convert.ToInt32(haoMaArray[2]);
            model.OpenCode4 = Convert.ToInt32(haoMaArray[3]);
            model.OpenCode5 = Convert.ToInt32(haoMaArray[4]);
            model.OpenCode6 = Convert.ToInt32(haoMaArray[5]);
            model.OpenCode7 = Convert.ToInt32(haoMaArray[6]);
            model.OpenCode8 = Convert.ToInt32(haoMaArray[7]);
            model.OpenTime = data.OpenTime;
            if (services.AddOpen8Code(currentLottery, model))
            {
                GetMaxPeriodNum((int)model.Term); //添加成功存放期数
                return true;
            }

            return false;
        }

        /// <summary>
        ///     比较期号大的存入LatestQiHao
        /// </summary>
        /// <param name="PeriodNum"></param>
        public void GetMaxPeriodNum(int PeriodNum)
        {
            var lastqihao = 0;
            int.TryParse(LatestQiHao, out lastqihao);

            if (lastqihao < PeriodNum) LatestQiHao = PeriodNum.ToString();
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
        private readonly IOpen8Code services;

        /// <summary>
        ///     当前彩种
        /// </summary>
        private SCCLottery currentLottery => SCCLottery.ChongQingKL10F;

        /// <summary>
        ///     邮件接口
        /// </summary>
        private IEmail email;

        #endregion
    }

    public class CQKL10FEnitity
    {
        public string QiHao { get; set; }
        public string KaiJiangHao { get; set; }
        public DateTime OpenTime { get; set; }
    }

}