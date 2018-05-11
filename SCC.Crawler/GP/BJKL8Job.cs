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
    ///     北京快乐8
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class BJKL8Job : IJob
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        public BJKL8Job()
        {
            log = new LogHelper();
            services = IOC.Resolve<IOpen21Code>();
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
            LatestItem = context.JobDetail.JobDataMap["LatestItem"] as OpenCode21Model;
            try
            {
                //服务启动时配置初始数据
                if (LatestItem == null) LatestItem = services.GetLastItem(currentLottery);
                //第一次启动服务或最新开奖记录为昨日滴，则自检昨天开奖数据是否抓取完毕(否则插入邮件数据)，并重置当天期号和失败列表
                if (LatestItem == null || LatestItem.OpenTime.ToString("yyyyMMdd") !=
                    CommonHelper.SCCSysDateTime.ToString("yyyyMMdd"))
                {
                    CheckingYesterdayTheLotteryData();
                    LatestItem = GenerateTodayFirstItem();
                }

                //当最新开奖记录不是今天最后一期，执行当天作业
                if (!CheckLatestItemIsTodayLastItem())
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
            context.JobDetail.JobDataMap["LatestItem"] = LatestItem;
        }

        /// <summary>
        ///     自检昨天开奖数据
        /// </summary>
        private void CheckingYesterdayTheLotteryData()
        {
            if (Config.SkipDate.Contains(CommonHelper.SCCSysDateTime.AddDays(-1).ToString("yyyyMMdd")))
                return; //如果昨日设定不开奖则不自检昨日开奖数据
            //从数据库中获取昨天数据抓取失败列表
            FailedQiHaoList = services.GetYesterdayFailQQQList(currentLottery, GenerateYesterdayQiHaoList());
            if (FailedQiHaoList.Count > 0)
            {
                DoYesterdayFailedListByMainUrl();
                DoYesterdayFailedListByBackUrl();
            }
        }

        /// <summary>
        ///     通过主站点抓取开奖数据
        ///     (百度彩票)
        /// </summary>
        private void DoTodayJobByMainUrl()
        {
            if (!string.IsNullOrEmpty(Config.MainUrl))
            {
                var OpenList = GetDocByMainUrl();
                if (OpenList.Count == 0) return; //无抓取数据
                var startQiNum = LatestItem.Term + 1;
                var newestQiNum = OpenList.Max(w => w.Term);
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
                for (var i = startQiNum; i <= newestQiNum; i++)
                {
                    var matchItem = OpenList.Where(R => R.Term == i).FirstOrDefault();
                    if (matchItem != null && services.AddOpen21Code(currentLottery, matchItem))
                    {
                        //处理成功写入日志
                        log.Info(GetType(), CommonHelper.GetJobMainLogInfo(Config, i.ToString()));
                        LatestItem = matchItem;
                    }
                }
            }
        }


        /// <summary>
        ///     通过主站抓取错误期号列表中每一个期号
        ///     (百度彩票)
        /// </summary>
        private void DoYesterdayFailedListByMainUrl()
        {
         
                var OpenList = GetDocByMainUrl();
                if (OpenList.Count == 0) return;//无抓取数据

                foreach (OpenCode21Model model in OpenList)
                {
                    if (services.AddOpen21Code(currentLottery, model))
                    {
                        //处理成功写入日志
                        log.Info(typeof(BJPK10Job), CommonHelper.GetJobBackLogInfo(Config, model.Term.ToString()));
                        LatestItem = model;
                    }
                
            }
        }


        /// <summary>
        ///     通过备用站点抓取开奖数据
        ///     (北京福彩官网)
        /// </summary>
        private void DoTodayJobByBackUrl()
        {
            if (!string.IsNullOrEmpty(Config.BackUrl))
            {
                var OpenList = GetOpenListFromBackUrl(Config.BackUrl);
                if (OpenList.Count == 0) return; //无抓取数据
                var startQiNum = LatestItem.Term + 1;
                var newestQiNum = OpenList.Max(w => w.Term);
                var total = OpenList.Count;
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
                for (var i = startQiNum; i <= newestQiNum; i++)
                {
                    var matchItem = OpenList.Where(R => R.Term == i).FirstOrDefault();
                    var step = 0;
                    var nowQiHao = Convert.ToInt32(i);

                    while (matchItem == null)
                        if (step <= total)
                        {
                            nowQiHao++;
                            step++;
                            matchItem = OpenList.Where(R => R.Term.ToString() == nowQiHao.ToString()).FirstOrDefault();
                        }
                        else
                        {
                            matchItem = null;
                            break;
                        }

                    if (matchItem != null && services.AddOpen21Code(currentLottery, matchItem))
                    {
                        //处理成功写入日志
                        log.Info(GetType(), CommonHelper.GetJobBackLogInfo(Config, i.ToString()));
                        LatestItem = matchItem;
                    }
                }
            }
        }

        /// <summary>
        ///     通过备用地址抓取错误期号列表中每一个期号
        ///     (北京福彩官网)
        /// </summary>
        private void DoYesterdayFailedListByBackUrl()
        {
            if (!string.IsNullOrEmpty(Config.BackUrl))
            {
                var OpenList = GetOpenListFromBackUrl(Config.BackUrl);
                if (OpenList.Count == 0) return;//无抓取数据

                foreach (OpenCode21Model model in OpenList)
                {
                    if (services.AddOpen21Code(currentLottery, model))
                    {
                        //处理成功写入日志
                        log.Info(typeof(BJPK10Job), CommonHelper.GetJobBackLogInfo(Config, model.Term.ToString()));
                        LatestItem = model;
                    }
                }
            }
        }

        /// <summary>
        ///     抓取备用站点开奖数据
        ///     由于北京福彩官网分页30条数据一页，而此彩种每天开179期，如果去跑6页，负荷太大，只有通过主站点来保证数据完整性
        /// </summary>
        /// <param name="url">备用站点</param>
        /// <returns></returns>
        private List<OpenCode21Model> GetOpenListFromBackUrl(string url, bool IsToday = true)
        {
            var result = new List<OpenCode21Model>();
            try
            {
                var HtmlResource = string.Empty;
                HtmlResource = NetHelper.GetUrlResponse(url);
                if (HtmlResource == null) return result;
                var doc = new HtmlDocument();
                doc.LoadHtml(HtmlResource);
                var table = doc.DocumentNode.SelectNodes("//table");
                if (table == null || table.Count < 2) return result;
                var trs = table[1].ChildNodes.Where(R => R.Name.ToLower() == "tr").ToList();
                if (trs.Count < 2) return result;
                List<HtmlNode> tds = null;
                OpenCode21Model model = null;
                string[] openCodeList = null;
                var openTime = string.Empty;
                var todayDateString = CommonHelper.SCCSysDateTime.ToString("yyyy-MM-dd");
                for (var i = 1; i < trs.Count; i++) //第一行是表头
                {
                    tds = trs[i].ChildNodes.Where(R => R.Name.ToLower() == "td").ToList();
                    if (tds.Count < 4) continue;
                    openTime = tds[3].InnerText.Trim();
        
                    model = new OpenCode21Model();
                    model.Term = Convert.ToInt64(tds[0].InnerText.Trim());
                    model.OpenCode21 = Convert.ToInt32(tds[2].InnerText.Trim()); //飞盘
                    openCodeList = tds[1].InnerText.Trim().Split(',');
                    if (openCodeList.Length < 20) continue;
                    model.OpenCode1 = Convert.ToInt32(openCodeList[0]);
                    model.OpenCode2 = Convert.ToInt32(openCodeList[1]);
                    model.OpenCode3 = Convert.ToInt32(openCodeList[2]);
                    model.OpenCode4 = Convert.ToInt32(openCodeList[3]);
                    model.OpenCode5 = Convert.ToInt32(openCodeList[4]);
                    model.OpenCode6 = Convert.ToInt32(openCodeList[5]);
                    model.OpenCode7 = Convert.ToInt32(openCodeList[6]);
                    model.OpenCode8 = Convert.ToInt32(openCodeList[7]);
                    model.OpenCode9 = Convert.ToInt32(openCodeList[8]);
                    model.OpenCode10 = Convert.ToInt32(openCodeList[9]);
                    model.OpenCode11 = Convert.ToInt32(openCodeList[10]);
                    model.OpenCode12 = Convert.ToInt32(openCodeList[11]);
                    model.OpenCode13 = Convert.ToInt32(openCodeList[12]);
                    model.OpenCode14 = Convert.ToInt32(openCodeList[13]);
                    model.OpenCode15 = Convert.ToInt32(openCodeList[14]);
                    model.OpenCode16 = Convert.ToInt32(openCodeList[15]);
                    model.OpenCode17 = Convert.ToInt32(openCodeList[16]);
                    model.OpenCode18 = Convert.ToInt32(openCodeList[17]);
                    model.OpenCode19 = Convert.ToInt32(openCodeList[18]);
                    model.OpenCode20 = Convert.ToInt32(openCodeList[19]);

                    model.OpenTime = Convert.ToDateTime(openTime);
                    if (!result.Contains(model))
                        result.Add(model);
                }

                var checkDataHelper = new CheckDataHelper();

                var data = services.GetListIn(currentLottery, IsToday);
                if (data != null)
                {
                    var dbdata = services.GetListIn(currentLottery, IsToday)
                        .ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr());
                    checkDataHelper.CheckData(dbdata, result.ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr()),
                        Config.Area, currentLottery);
                }
                else
                {
                    log.Warn(GetType(), $"{Config.LotteryName}没有最新数据！");
                }
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过备用站点抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            return result;
        }

        /// <summary>
        ///     从主要链接查询开奖号码 昨天/今天
        /// </summary>
        /// <param name="IsToday"></param>
        /// <returns></returns>
        private List<OpenCode21Model> GetDocByMainUrl(bool IsToday = true)
        {
            var list = new List<OpenCode21Model>();
            try
            {
                var time = DateTime.Now;
                var arg = new OpenCaiApiArg
                {
                    code = EnumHelper.GetLotteryCode(SCCLottery.BeiJingKL8),
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
                        var tmp = new OpenCode21Model();
                        var codeAry = data.data[i].GetOpenCodeStr().Split(',');
                        if (codeAry.Length == 21)
                        {
                            tmp.Term = long.Parse(data.data[i].expect);
                            tmp.OpenTime = DateTime.Parse(data.data[i].opentime);
                            tmp.OpenCode1 = int.Parse(codeAry[0]);
                            tmp.OpenCode2 = int.Parse(codeAry[1]);
                            tmp.OpenCode3 = int.Parse(codeAry[2]);
                            tmp.OpenCode4 = int.Parse(codeAry[3]);
                            tmp.OpenCode5 = int.Parse(codeAry[4]);
                            tmp.OpenCode6 = int.Parse(codeAry[5]);
                            tmp.OpenCode7 = int.Parse(codeAry[6]);
                            tmp.OpenCode8 = int.Parse(codeAry[7]);
                            tmp.OpenCode9 = int.Parse(codeAry[8]);
                            tmp.OpenCode10 = int.Parse(codeAry[9]);
                            tmp.OpenCode11 = int.Parse(codeAry[10]);
                            tmp.OpenCode12 = int.Parse(codeAry[11]);
                            tmp.OpenCode13 = int.Parse(codeAry[12]);
                            tmp.OpenCode14 = int.Parse(codeAry[13]);
                            tmp.OpenCode15 = int.Parse(codeAry[14]);
                            tmp.OpenCode16 = int.Parse(codeAry[15]);
                            tmp.OpenCode17 = int.Parse(codeAry[16]);
                            tmp.OpenCode18 = int.Parse(codeAry[17]);
                            tmp.OpenCode19 = int.Parse(codeAry[18]);
                            tmp.OpenCode20 = int.Parse(codeAry[19]);
                            tmp.OpenCode21 = int.Parse(codeAry[20]);
                        }

                        list.Add(tmp);
                    }

                    var checkDataHelper = new CheckDataHelper();
                    var dbdata = services.GetListIn(currentLottery, IsToday);
                    if (dbdata == null) return list;
                    var DATA = dbdata.ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr());
                    checkDataHelper.CheckData(DATA, list.ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr()),
                        Config.Area, currentLottery);
                }
            }
            catch (Exception ee)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过主要站点抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ee.Message));
            }

            return list;
        }

        /// <summary>
        ///     生成今日第一期记录
        /// </summary>
        /// <returns></returns>
        private OpenCode21Model GenerateTodayFirstItem()
        {
            var item = new OpenCode21Model();
            var currentDateTime = CommonHelper.SCCSysDateTime;
            var datepart = currentDateTime - new DateTime(2018, 1, 1);
            var t = Config.SkipDate.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            item.Term = (datepart.Days - t.Length) * Config.TimesPerDay + LastTermLastYear; //期号使用昨日最后一期期号，便于计算使用
            item.OpenTime = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day,
                Config.StartHour, Config.StartMinute, 0);
            return item;
        }

        /// <summary>
        ///     核实最新一期开奖记录是否为今日最后一期
        /// </summary>
        /// <param name="QiHao">期号</param>
        /// <returns></returns>
        private bool CheckLatestItemIsTodayLastItem()
        {
            if (LatestItem == null ||
                LatestItem.OpenTime.ToString("yyyyMMdd") != CommonHelper.SCCSysDateTime.ToString("yyyyMMdd"))
                return true;
            var firstItem = GenerateTodayFirstItem();
            if (LatestItem.Term == firstItem.Term + Config.TimesPerDay)
                return true;
            return false;
        }

        /// <summary>
        ///     获取今天第一期期号
        /// </summary>
        /// <returns></returns>
        private string GenerateTodayFirstQiHao()
        {
            var datepart = CommonHelper.SCCSysDateTime - new DateTime(2018, 1, 1);
            var t = Config.SkipDate.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            var terms = (datepart.Days - t.Length) * Config.TimesPerDay + LastTermLastYear + 1;
            return terms.ToString();
        }

        /// <summary>
        ///     生成昨天的期号列表
        /// </summary>
        /// <returns></returns>
        private List<string> GenerateYesterdayQiHaoList()
        {
            var result = new List<string>();
            var todayFirstQiHao = Convert.ToInt64(GenerateTodayFirstQiHao());
            for (var i = Config.TimesPerDay; i > 0; i--) result.Add((todayFirstQiHao - i).ToString());
            return result;
        }

        /// <summary>
        ///     生成快乐8的开奖时间
        /// </summary>
        /// <param name="QiHao"></param>
        /// <returns></returns>
        private DateTime GenerateYesterdayKL8OpenTime(string QiHao)
        {
            var openDay = CommonHelper.SCCSysDateTime.AddDays(-1);
            var StartTime = new DateTime(openDay.Year, openDay.Month, openDay.Day, Config.StartHour, Config.StartMinute,
                0);
            var t = (Convert.ToInt64(QiHao) - LastTermLastYear) % Config.TimesPerDay;
            if (t == 0)
                return StartTime.AddMinutes((Config.TimesPerDay - 1) * Config.Interval);
            return StartTime.AddMinutes((t - 1) * Config.Interval);
        }

        #region Attribute

        /// <summary>
        ///     配置信息
        /// </summary>
        private SCCConfig Config;

        /// <summary>
        ///     当天抓取的最新一期开奖记录
        /// </summary>
        private OpenCode21Model LatestItem;

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
        private readonly IOpen21Code services;

        /// <summary>
        ///     当前彩种
        /// </summary>
        private SCCLottery currentLottery => SCCLottery.BeiJingKL8;

        /// <summary>
        ///     邮件接口
        /// </summary>
        private IEmail email;

        /// <summary>
        ///     2016年最后一期期号
        /// </summary>
        private readonly long LastTermLastYear = 872705;

        #endregion
    }
}