﻿using System;
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
    ///     山东快乐扑克3
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class SDKLPK3Job : IJob
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        public SDKLPK3Job()
        {
            log = new LogHelper();
            services = IOC.Resolve<IOpen3Code>();
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
        ///     (网易163)
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
        ///     (网易163)
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
                    code = EnumHelper.GetLotteryCode(SCCLottery.ShanDongKLPK3),
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
                    {
                        var code = data.data[i].GetOpenCodeStr().Split(',');
                        for (var j = 0; j < code.Length; j++) code[j] = CetapiTOdb(int.Parse(code[j])).ToString();
                        result.Add(data.data[i].GetTermStr(), string.Join(",", code));
                    }

                    var checkDataHelper = new CheckDataHelper();
                    var dbdata = services.GetListIn(currentLottery, IsToday)
                        .ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr());
                    checkDataHelper.CheckData(dbdata, result, Config.Area, currentLottery);
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
        ///     将花色和号码转化为开奖号码
        ///     1为方片，2为红心，3为梅花，4为黑桃
        ///     A为1，J为11，Q为12，K为13
        /// </summary>
        /// <param name="KJHaoMa">号码</param>
        /// <param name="ClassName">花色类名称</param>
        /// <returns></returns>
        private string GetMainPuKeString(string KJHaoMa, string ClassName)
        {
            var result = string.Empty;
            switch (KJHaoMa)
            {
                case "A":
                    result = "01";
                    break;
                case "J":
                    result = "11";
                    break;
                case "Q":
                    result = "12";
                    break;
                case "K":
                    result = "13";
                    break;
                default:
                    result = KJHaoMa.PadLeft(2, '0');
                    break;
            }

            if (ClassName.Contains("fangpian"))
                return "1" + result;
            if (ClassName.Contains("hongtao"))
                return "2" + result;
            if (ClassName.Contains("meihua"))
                return "3" + result;
            return "4" + result; //heitao
        }

        /// <summary>
        ///     通过备用站点抓取开奖数据
        ///     (新浪爱彩)
        /// </summary>
        private void DoTodayJobByBackUrl()
        {
            if (!string.IsNullOrEmpty(Config.BackUrl))
            {
                var OpenList = GetTodayOpenListFromBackUrl();
                if (OpenList.Count == 0) return; //无抓取数据
                var newestQiHao = OpenList.Max(w => w.Key);
                var startQiNum = Convert.ToInt32(LatestQiHao.Substring(6)) + 1;
                var total = OpenList.Count;
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
        ///     (新浪爱彩)
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
            return GetOpenListFromBackUrl(Config.BackUrl, "sDate=&eDate=&q=today&sortTag=up");
        }

        /// <summary>
        ///     通过备用站点抓取昨日开奖数据
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetYesterdayOpenListFromBackUrl()
        {
            return GetOpenListFromBackUrl(Config.BackUrl, "sDate=&eDate=&q=yesterday&sortTag=up", false);
        }

        /// <summary>
        ///     抓取备用站点开奖数据
        /// </summary>
        /// <param name="url">备用站点</param>
        /// <param name="postdata">发送数据</param>
        /// <returns></returns>
        private Dictionary<string, string> GetOpenListFromBackUrl(string url, string postdata, bool IsToday = true)
        {
            var result = new Dictionary<string, string>();
            try
            {
                var HtmlResource = NetHelper.GetUrlResponse(url, "POST", postdata);
                if (HtmlResource == null) return result;

                var doc = new HtmlDocument();
                doc.LoadHtml(HtmlResource);
                var table = doc.GetElementbyId("chartsTable");
                if (table == null) return result;
                var trs = table.SelectSingleNode("tbody").ChildNodes.Where(node => node.Name == "tr").ToList();
                if (trs.Count < 1) return result;
                var matchQiHao = string.Empty;
                var matchKJHaoMa = string.Empty;
                for (var i = 0; i < trs.Count; i++)
                {
                    var trstyle = trs[i].Attributes["style"];
                    if (trstyle != null && trstyle.Value == "display:none") continue;
                    var tds = trs[i].ChildNodes.Where(node => node.Name == "td").ToList();
                    if (tds.Count < 5) continue;
                    matchQiHao = CommonHelper.SCCSysDateTime.ToString("yy") + tds[0].InnerText.Trim();
                    matchKJHaoMa = string.Format("{0},{1},{2}", GetBackPuKeString(tds[2].InnerText.Trim()),
                        GetBackPuKeString(tds[3].InnerText.Trim()), GetBackPuKeString(tds[4].InnerText.Trim()));
                    if (!result.ContainsKey(matchQiHao))
                        result.Add(matchQiHao, CommonHelper.TRHandleCode(matchKJHaoMa));
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
        ///     将备用站点的快乐扑克3号码转化成所需开奖号码
        /// </summary>
        /// <param name="KJHaoMa">开奖号码</param>
        /// <returns></returns>
        private string GetBackPuKeString(string KJHaoMa)
        {
            if (string.IsNullOrWhiteSpace(KJHaoMa)) return string.Empty;
            if (!"♦♥♣♠".Contains(KJHaoMa.Substring(0, 1))) return string.Empty;
            var haoma = KJHaoMa.Substring(1);
            switch (haoma)
            {
                case "A":
                    haoma = "01";
                    break;
                case "J":
                    haoma = "11";
                    break;
                case "Q":
                    haoma = "12";
                    break;
                case "K":
                    haoma = "13";
                    break;
                default:
                    haoma = haoma.PadLeft(2, '0');
                    break;
            }

            if (KJHaoMa.Substring(0, 1) == "♦")
                return "1" + haoma;
            if (KJHaoMa.Substring(0, 1) == "♥")
                return "2" + haoma;
            if (KJHaoMa.Substring(0, 1) == "♣")
                return "3" + haoma;
            return "4" + haoma;
        }

        /// <summary>
        ///     吧opencai的数据转换成数据原有格式
        /// </summary>
        /// <param name="KJHaoMa"></param>
        /// <returns></returns>
        private int CetapiTOdb(int KJHaoMa)
        {
            // key 数据库原有格式 value opencai格式
            var MDict = new Dictionary<int, int>
            {
                {100, 400},
                {200, 200},
                {300, 300},
                {400, 100}
            };
            if (KJHaoMa / 100 > 0)
            {
                var bstr = MDict[KJHaoMa / 100 * 100];
                var gstr = KJHaoMa % 100;
                return bstr + gstr;
            }

            return 0;
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
                var model = new OpenCode3Model();
                model.Term = Convert.ToInt64(QiHao);
                var haoMaArray = OpenCode.Split(',');
                model.OpenCode1 = Convert.ToInt32(haoMaArray[0]);
                model.OpenCode2 = Convert.ToInt32(haoMaArray[1]);
                model.OpenCode3 = Convert.ToInt32(haoMaArray[2]);
                if (IsYesterdayRecord)
                    model.OpenTime = CommonHelper.GenerateYesterdayOpenTime(Config, QiHao);
                else
                    model.OpenTime = CommonHelper.GenerateTodayOpenTime(Config, QiHao);
                return services.AddOpen3Code(currentLottery, model);
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
        private readonly IOpen3Code services;

        /// <summary>
        ///     当前彩种
        /// </summary>
        private SCCLottery currentLottery => SCCLottery.ShanDongKLPK3;

        /// <summary>
        ///     邮件接口
        /// </summary>
        private IEmail email;

        #endregion
    }
}