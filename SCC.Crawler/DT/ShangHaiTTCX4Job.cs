using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Quartz;
using SCC.Common;
using SCC.Crawler.Tools;
using SCC.Interface;
using SCC.Models;

namespace SCC.Crawler.DT
{
    /// <summary>
    ///     上海天天彩
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class ShangHaiTTCX4Job : IJob
    {
        /// <summary>
        /// 初始化函数
        /// </summary>
        public ShangHaiTTCX4Job()
        {
            log = new LogHelper();
            services = IOC.Resolve<IDTOpenCode>();
            email = IOC.Resolve<IEmail>();
        }

        public void Execute(IJobExecutionContext context)
        {
            Config = CommonHelper.GetConfigFromDataMap(context.JobDetail.JobDataMap);
            //预设节假日不开奖
            if (Config.SkipDate.Contains(CommonHelper.SCCSysDateTime.ToString("yyyyMMdd"))) return;
            LatestItem = context.JobDetail.JobDataMap["LatestItem"] as OpenCode4DTModel;
            try
            {
                //服务启动时配置初始数据
                if (LatestItem == null)
                {
                    LatestItem = services.GetOpenCode4DTLastItem(currentLottery);
                    if (LatestItem == null)
                        LatestItem = new OpenCode4DTModel
                        {
                            Term = CommonHelper.GenerateQiHaoYYYYQQQ(0),
                            OpenTime = new DateTime(CommonHelper.SCCSysDateTime.Year, 1, 1)
                        };
                }

                //程序时间第二天，程序根据配置检查是否昨天有开奖
                isGetData = false;
                if (CommonHelper.CheckDTIsNeedGetData(Config)) //
                {
                    DoMainUrl();
                    DoBackUrl();
                }

                if (!LatestItem.Term.ToString().StartsWith(CommonHelper.SCCSysDateTime.ToString("yy")))
                    LatestItem = new OpenCode4DTModel
                    {
                        Term = CommonHelper.GenerateQiHaoYYYYQQQ(0),
                        OpenTime = new DateTime(CommonHelper.SCCSysDateTime.Year, 1, 1)
                    };
                //当今日开奖并且当前时间是晚上8点过后开始抓取

                DoMainUrl();
                DoBackUrl();
            }
            catch (Exception ex)
            {
                log.Error(GetType(), string.Format("【{0}】抓取时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            //保存最新期号
            context.JobDetail.JobDataMap["LatestItem"] = LatestItem;
        }

        private void DoMainUrl()
        {
            if (!string.IsNullOrEmpty(Config.MainUrl))
            {
                var openList = GetOpenListFromMainUrl(Config.MainUrl);
                if (openList.Count == 0) return; //无抓取数据
                //抓取到的最新期数
                var newestQiHao = Convert.ToInt32(openList.Max(w => w.Term).ToString());
                //数据库里面最新期数
                var startQiNum = Convert.ToInt32(LatestItem.Term.ToString());

                if (startQiNum > newestQiHao) return; //无最新数据

                //处理最新开奖数据
                var getQiHao = string.Empty;
                OpenCode4DTModel matchItem = null;
                for (var i = startQiNum; i <= newestQiHao; i++)
                {
                    getQiHao = i.ToString();
                    matchItem = openList.FirstOrDefault(r => r.Term.ToString() == getQiHao);

                    if (matchItem != null && services.AddDTOpen4Code(currentLottery, matchItem))
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
        ///     爬取主网站信息
        /// </summary>
        /// <returns></returns>
        private List<OpenCode4DTModel> GetOpenListFromMainUrl(string mainUrl)
        {
            var result = new List<OpenCode4DTModel>();
            try
            {
                var mainurl = mainUrl + "?type=4d&number=50";
                var htmlResource = NetHelper.GetUrlResponse(mainurl);
                if (htmlResource == null) return result;

                string term = string.Empty, openCodeString = string.Empty, optimizeUrl = string.Empty;
                OpenCode4DTModel model = null;
                var data = htmlResource.JsonToEntity<JObject>();
                for (var i = 0; i < data["Models"].Count(); i++)
                {
                    model = new OpenCode4DTModel();

                    term = data["Models"][i]["no"].ToString();
                    model.Term = long.Parse(term);
                    model.OpenTime = DateTime.Now;
                    openCodeString = data["Models"][i]["Number"].ToString();

                    var openCode = openCodeString.Split(',');
                    model.OpenCode1 = Convert.ToInt32(openCode[0]);
                    model.OpenCode2 = Convert.ToInt32(openCode[1]);
                    model.OpenCode3 = Convert.ToInt32(openCode[2]);
                    model.OpenCode4 = Convert.ToInt32(openCode[3]);
                    model.Spare = string.Empty;
                    model.DetailUrl = mainUrl;
                    result.Add(model);
                }


                var checkDataHelper = new CheckDataHelper();
                var dbdata = services.GetListS<OpenCode4DTModel>(currentLottery)
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
        ///     获取备用站点开奖列表数据
        /// </summary>
        /// <returns></returns>
        private List<OpenCode4DTModel> GetOpenListFromBackUrl()
        {
            var result = new List<OpenCode4DTModel>();
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
                OpenCode4DTModel model = null;
                HtmlNode nodeA = null;
                var optimizeUrl = string.Empty;
                for (var i = 1; i < trs.Count; i++) //第一行为表头
                {
                    var trstyle = trs[i].Attributes["style"];
                    if (trstyle != null && trstyle.Value == "display:none") continue;
                    var tds = trs[i].ChildNodes.Where(node => node.Name == "td").ToList();
                    if (tds.Count < 4) continue;
                    model = new OpenCode4DTModel();
                    nodeA = tds[0].ChildNodes.Where(n => n.Name == "a").FirstOrDefault();
                    if (nodeA == null) continue;
                    model.Term = Convert.ToInt64(nodeA.InnerText.Trim());
                    optimizeUrl = nodeA.Attributes["href"].Value;
                    model.DetailUrl = new Uri(url, optimizeUrl).AbsoluteUri;
                    model.OpenTime = Convert.ToDateTime(tds[3].InnerText);
                    if (tds[0].ChildNodes.Count == 0) continue;
                    model.OpenTime = DateTime.Now;
                    var opencodeNode = tds[1].ChildNodes.Where(n => n.Name.ToLower() == "i").ToList();
                    if (opencodeNode.Count < 4) continue;
                    model.OpenCode1 = Convert.ToInt32(opencodeNode[0].InnerText.Trim());
                    model.OpenCode2 = Convert.ToInt32(opencodeNode[1].InnerText.Trim());
                    model.OpenCode3 = Convert.ToInt32(opencodeNode[2].InnerText.Trim());
                    model.OpenCode4 = Convert.ToInt32(opencodeNode[3].InnerText.Trim());
                    result.Add(model);
                }

                var checkDataHelper = new CheckDataHelper();
                var dbdata = services.GetListS<OpenCode4DTModel>(currentLottery)
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
        ///     验证以前的七号数据
        /// </summary>
        private void DoBackUrl()
        {
            if (!string.IsNullOrEmpty(Config.BackUrl))
            {
                var OpenList = GetOpenListFromBackUrl();
                if (OpenList.Count == 0) return; //无抓取数据
                var newestQiHao = OpenList.Max(w => w.Term).ToString();
                var startQiNum = Convert.ToInt32(LatestItem.Term.ToString().Substring(4)) + 1;
                var newestQiNum = Convert.ToInt32(newestQiHao.Substring(4));
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
                var getQiHao = string.Empty;
                OpenCode4DTModel matchItem = null;
                for (var i = startQiNum; i <= newestQiNum; i++)
                {
                    getQiHao = i.ToString();
                    matchItem = OpenList.Where(R => R.Term.ToString() == getQiHao).FirstOrDefault();
                    if (matchItem != null && services.AddDTOpen4Code(currentLottery, matchItem))
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
        ///     获取主站下开奖详情
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        /// <summary>
        ///     配置信息
        /// </summary>
        private SCCConfig Config;

        /// <summary>
        ///     当天抓取的最新一期开奖记录
        /// </summary>
        private OpenCode4DTModel LatestItem;

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
        private SCCLottery currentLottery => SCCLottery.ShangHaiTTCX4;

        /// <summary>
        ///     邮件接口
        /// </summary>
        private IEmail email;

        /// <summary>
        ///     是否本次运行抓取到开奖数据
        /// </summary>
        private bool isGetData;

       
    }
}