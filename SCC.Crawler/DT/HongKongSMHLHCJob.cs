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

namespace SCC.Crawler.DT
{
    /// <summary>
    ///     香港六合彩
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class HongKongSMHLHCJob : IJob
    {
        /// <summary>
        /// 初始化函数
        /// </summary>
        public HongKongSMHLHCJob()
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
                {
                    LatestItem = services.GetOpenCode7DTLastItem(currentLottery);
                    if (LatestItem == null)
                        LatestItem = new OpenCode7DTModel
                        {
                            Term = CommonHelper.GenerateQiHaoYYYYQQQ(0),
                            OpenTime = new DateTime(CommonHelper.SCCSysDateTime.Year, 1, 1)
                        };
                }

                //程序时间第二天，程序根据配置检查是否昨天有开奖
                isGetData = false;
                if (CommonHelper.CheckDTIsNeedGetData(Config)) //
                    DoMainUrl();
                DoBackUrl();
                if (!LatestItem.Term.ToString().StartsWith(CommonHelper.SCCSysDateTime.ToString("yy")))
                    LatestItem = new OpenCode7DTModel
                    {
                        Term = CommonHelper.GenerateQiHaoYYYYQQQ(0),
                        OpenTime = new DateTime(CommonHelper.SCCSysDateTime.Year, 1, 1)
                    };
                //当今日开奖并且当前时间是晚上8点过后开始抓取
                if (CommonHelper.CheckTodayIsOpenDay(Config) && CommonHelper.SCCSysDateTime.Hour > 12) DoMainUrl();
            }
            catch (Exception ex)
            {
                log.Error(GetType(), string.Format("【{0}】抓取时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            //保存最新期号
            context.JobDetail.JobDataMap["LatestItem"] = LatestItem;
        }



        #region 通过备用站爬取数据
        private void DoBackUrl()
        {
            if (!string.IsNullOrEmpty(Config.BackUrl))
            {
                var openList = GetOpenListFromBackUrl(Config.BackUrl);
                if (openList.Count == 0) return; //无抓取数据
                //抓取到的最新期数
                var newestQiHao = Convert.ToInt32(openList.First().Term.ToString());
                //数据库里面最新期数
                var startQiNum = Convert.ToInt32(LatestItem.Term.ToString());

                if (startQiNum > newestQiHao) return; //无最新数据

                //处理最新开奖数据
                var getQiHao = string.Empty;
                OpenCode7DTModel matchItem = null;
                for (var i = startQiNum; i <= newestQiHao; i++)
                {
                    getQiHao = i.ToString();
                    matchItem = openList.FirstOrDefault(r => r.Term.ToString() == getQiHao);

                    if (matchItem != null && services.AddDTOpen7Code(currentLottery, matchItem))
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
        ///     获取备用站点开奖列表数据
        /// </summary>
        /// <returns></returns>
        private List<OpenCode7DTModel> GetOpenListFromBackUrl(string BackUrl)
        {
            var result = new List<OpenCode7DTModel>();
            try
            {
                var url = new Uri(BackUrl);
                var htmlResource = NetHelper.GetUrlResponse(BackUrl, Encoding.GetEncoding("gb2312"));
                if (htmlResource == null) return result;

                var doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                var table = doc.DocumentNode.SelectNodes("//table[@border='1']");

                if (table == null) return result;
                var trs = table[1].ChildNodes.Where(s => s.Name.ToLower() == "tr").ToList();

                OpenCode7DTModel model = null;
                var optimizeUrl = string.Empty;
                for (var i = 1; i < trs.Count; i++) //第一二行为表头
                {
                    var tds = trs[i].ChildNodes.Where(node => node.Name == "td").ToList();
                    if (tds.Count < 16) continue;
                    model = new OpenCode7DTModel();
                 
           

                    model.OpenTime = Convert.ToDateTime(tds[1].InnerText);
                    model.Term = Convert.ToInt64(TermSet(tds[2].InnerText));
                    var openCodeString = tds[3].InnerText.Replace("shownumber(\"", "").Replace("\")", "").Trim().Split(','); ;
              
                 
                    model.OpenCode1 = Convert.ToInt32(openCodeString[0]);
                    model.OpenCode2 = Convert.ToInt32(openCodeString[1]);
                    model.OpenCode3 = Convert.ToInt32(openCodeString[2]);
                    model.OpenCode4 = Convert.ToInt32(openCodeString[3]);
                    model.OpenCode5 = Convert.ToInt32(openCodeString[4]);
                    model.OpenCode6 = Convert.ToInt32(openCodeString[5]);
                    model.OpenCode7 = Convert.ToInt32(tds[4].InnerText.Replace("shownumber(\"", "").Replace("\")", "").Trim());
                    model.Spare = string.Empty;
                    if (result.SingleOrDefault(w => w.Term == model.Term) == null) result.Add(model);
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

        private  String TermSet(string Term)
        {

            string date = CommonHelper.SCCSysDateTime.ToString("yyyy");
            string Qishu;
            if (Term.Length == 1) {
                Qishu = date + "00" + Term;
            }
           else if(Term.Length == 2)
            {
                Qishu = date + "0" + Term;
            }
            else
            {
                Qishu = date  + Term;
            }
            return Qishu;

        }
        #endregion
        #region 通过主站爬取数据

        private void DoMainUrl()
        {
            if (!string.IsNullOrEmpty(Config.MainUrl))
            {
                var openList = GetOpenListFromMainUrl(Config.MainUrl);
                if (openList.Count == 0) return; //无抓取数据
                //抓取到的最新期数
                var newestQiHao = Convert.ToInt32(openList.First().Term.ToString());
                //数据库里面最新期数
                var startQiNum = Convert.ToInt32(LatestItem.Term.ToString());

                if (startQiNum > newestQiHao) return; //无最新数据

                //处理最新开奖数据
                var getQiHao = string.Empty;
                OpenCode7DTModel matchItem = null;
                for (var i = startQiNum; i <= newestQiHao; i++)
                {
                    getQiHao = i.ToString();
                    matchItem = openList.FirstOrDefault(r => r.Term.ToString() == getQiHao);

                    if (matchItem != null && services.AddDTOpen7Code(currentLottery, matchItem))
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
        ///     获取备用站点开奖列表数据
        /// </summary>
        /// <returns></returns>
        private List<OpenCode7DTModel> GetOpenListFromMainUrl(string mainUrl)
        {
            var result = new List<OpenCode7DTModel>();
            try
            {
                var url = new Uri(mainUrl);
                var htmlResource = NetHelper.GetUrlResponse(mainUrl, Encoding.GetEncoding("UTF-8"));
                if (htmlResource == null) return result;

                var doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                var div = doc.DocumentNode.SelectSingleNode("//div[@class='bd']");
                if (div == null) return result;

                var table = div.ChildNodes.Where(node => node.Name == "table").ToList();
                if (table == null) return result;
                var trs = table[0].ChildNodes.Where(node => node.Name == "tr").ToList();
                OpenCode7DTModel model = null;
                var optimizeUrl = string.Empty;
                for (var i = 1; i < trs.Count; i++) //第一二行为表头
                {
                    var tds = trs[i].ChildNodes.Where(node => node.Name == "td").ToList();
                    if (tds.Count < 23) continue;
                    model = new OpenCode7DTModel();
                    var QiHao = tds[0].InnerText.Substring(0, 4) + tds[1].InnerText.Trim().Substring(0, 3);
                    model.Term = Convert.ToInt64(QiHao);

                    model.OpenTime = Convert.ToDateTime(tds[0].InnerText);
                    var openCodeString = tds[2].InnerText.Trim();

                    model.OpenCode1 = Convert.ToInt32(openCodeString.Substring(0, 2));
                    model.OpenCode2 = Convert.ToInt32(openCodeString.Substring(3, 2));
                    model.OpenCode3 = Convert.ToInt32(openCodeString.Substring(6, 2));
                    model.OpenCode4 = Convert.ToInt32(openCodeString.Substring(9, 2));
                    model.OpenCode5 = Convert.ToInt32(openCodeString.Substring(12, 2));
                    model.OpenCode6 = Convert.ToInt32(openCodeString.Substring(15, 2));
                    model.OpenCode7 = Convert.ToInt32(tds[3].InnerText.Trim().Substring(0, 2));
                    model.Spare = string.Empty;
                    if (result.SingleOrDefault(w => w.Term == model.Term) == null) result.Add(model);
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
        ///     完善备用站点江苏体彩7位数开奖实体信息
        /// </summary>
        /// <param name="model"></param>
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
        private List<string> FailedQiHaoList = null;

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
        private SCCLottery currentLottery => SCCLottery.HongKongSMHLHC;

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