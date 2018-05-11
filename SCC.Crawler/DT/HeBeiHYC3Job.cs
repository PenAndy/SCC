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
    ///     河南好运3
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class HeBeiHYC3Job : IJob
    {
        /// <summary>
        /// 初始化函数
        /// </summary>
        public HeBeiHYC3Job()
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
            LatestItem = context.JobDetail.JobDataMap["LatestItem"] as OpenCode3DTModel;
            try
            {
                //服务启动时配置初始数据
                if (LatestItem == null)
                {
                    LatestItem = services.GetOpenCode3DTLastItem(currentLottery);
                    if (LatestItem == null)
                        LatestItem = new OpenCode3DTModel
                        {
                            Term = CommonHelper.GenerateQiHaoYYYYQQQ(0),
                            OpenTime = new DateTime(CommonHelper.SCCSysDateTime.Year, 1, 1)
                        };
                }

                //程序时间第二天，程序根据配置检查是否昨天有开奖
                isGetData = false;
                if (CommonHelper.CheckDTIsNeedGetData(Config)) //
                    DoMainUrl();
                if (!LatestItem.Term.ToString().StartsWith(CommonHelper.SCCSysDateTime.ToString("yy")))
                    LatestItem = new OpenCode3DTModel
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


        #region 通过主站爬取数据
        /// <summary>
        /// 执行数据插入
        /// </summary>
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
                OpenCode3DTModel matchItem = null;
                for (var i = startQiNum; i <= newestQiHao; i++)
                {
                    getQiHao = i.ToString();
                    matchItem = openList.FirstOrDefault(r => r.Term.ToString() == getQiHao);

                    if (matchItem != null && services.AddDTOpen3Code(currentLottery, matchItem))
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
        ///     获取主站点开奖列表数据
        /// </summary>
        /// <returns></returns>
        private List<OpenCode3DTModel> GetOpenListFromMainUrl(string mainUrl)
        {
            var result = new List<OpenCode3DTModel>();
            try
            {
                var url = new Uri(mainUrl);
                var htmlResource = NetHelper.GetUrlResponse(mainUrl, Encoding.GetEncoding("UTF-8"));
                if (htmlResource == null) return result;

                var doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                var div = doc.DocumentNode.SelectSingleNode("//div[@id='jq_difang_index_body']");
                if (div == null) return result;
                var divs = div.ChildNodes.Where(node => node.Name == "div").ToList();
                if (divs == null) return result;
                var div2 = divs[2].ChildNodes.Where(node => node.Name == "div").ToList();
                if (div2 == null) return result;
                var table = div2[1].ChildNodes.Where(node => node.Name == "table").ToList();
                if (table == null) return result;
                var tbody = table[0].ChildNodes.Where(node => node.Name == "tbody").ToList();
                if (tbody == null) return result;
                var trs = tbody[0].ChildNodes.Where(node => node.Name == "tr").ToList();
                if (trs == null) return result;
                OpenCode3DTModel model = null;
                HtmlNode nodeA = null;
                var optimizeUrl = string.Empty;
                for (var i = 0; i < trs.Count; i++) //第一二行为表头
                {
                    var trstyle = trs[i].Attributes["style"];
                    if (trstyle != null && trstyle.Value == "display:none") continue;
                    var tds = trs[i].ChildNodes.Where(node => node.Name == "td").ToList();
                    if (tds.Count < 4) continue;
                    model = new OpenCode3DTModel();
                    nodeA = tds[2].ChildNodes.Where(n => n.Name == "a").FirstOrDefault();
                    if (nodeA == null) continue;
                    model.Term = Convert.ToInt64(tds[0].InnerText.Trim());

                    optimizeUrl = tds[0].InnerText.Trim();
                    model.OpenTime = Convert.ToDateTime(tds[3].InnerText.Substring(5, 10));
                    if (tds[1].ChildNodes.Count == 0) continue;
                    var opencode = tds[1].ChildNodes.Where(n => n.Name.ToLower() == "span").ToList();

                    var opencodeNode = opencode[0].ChildNodes.Where(n => n.Name.ToLower() == "strong").ToList();
                    var openCodeString = opencodeNode[0].InnerText.Trim();
                    model.OpenCode1 = Convert.ToInt32(openCodeString.Substring(0, 2));
                    model.OpenCode2 = Convert.ToInt32(openCodeString.Substring(3, 2));
                    model.OpenCode3 = Convert.ToInt32(openCodeString.Substring(6, 2));
                    var details = GetKaijiangDetails(optimizeUrl);
                    model.Spare = details;
                    result.Add(model);
                }

                var checkDataHelper = new CheckDataHelper();
                var dbdata = services.GetListS<OpenCode3DTModel>(currentLottery);
                if (dbdata == null) return result;
                var data = dbdata.ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr());
                checkDataHelper.CheckData(data, result.ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr()),
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
        ///     完善主站点江苏体彩7位数开奖实体信息
        /// </summary>
        /// <param name="model"></param>
        private string GetKaijiangDetails(string optimizeUrl)
        {
            var url = "http://kaijiang.500.com/shtml/hbhy3/" + optimizeUrl + ".shtml";
            var htmlResource = NetHelper.GetUrlResponse(url, Encoding.GetEncoding("gb2312"));
            if (string.IsNullOrWhiteSpace(htmlResource))
            {
                log.Warn(GetType(), string.Format("【{0}】通过主站点抓取开奖列表未获取到数据。", Config.Area + currentLottery));
                return null;
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlResource);
            var div = doc.DocumentNode.SelectSingleNode("//div[@style='margin:20px auto; width:500px;']");
            //爬去奖金
            var table = div.ChildNodes.Where(node => node.Name == "table").ToList();
            var tr = table[0].ChildNodes.Where(node => node.Name == "tr").ToList();
            var td = tr[2].ChildNodes.Where(node => node.Name == "td").ToList();
            var span = td[0].ChildNodes.Where(node => node.Name == "span").ToList();
            //爬去开奖详情
            var tr1 = table[1].ChildNodes.Where(node => node.Name == "tr").ToList();
            var td1 = tr1[2].ChildNodes.Where(node => node.Name == "td").ToList();

            //			<td>				本期销量：<span class="cfont1 ">2,154元</span>&nbsp;&nbsp;&nbsp;&nbsp;奖池滚存：<span class="cfont1 ">0元</span></td>	
            var entity = new KaijiangDetailsEntity
            {
                Gdje = span[1].InnerHtml.Trim().Replace("元", "").Replace(",", ""),
                Trje = span[0].InnerHtml.Trim().Replace("元", "").Replace(",", "")
            };
            //TODO 
            var list1 = new List<Kaijiangitem>();
            //组装详情
            var list = new List<Kaijiangitem>
            {
                new Kaijiangitem
                {
                    Name = "一等奖",
                    Total = td1[1].InnerHtml.Trim().Replace(",", ""),
                    TotalMoney = td1[2].InnerHtml.Trim().Replace(",", "")
                }
            };
            entity.KaiJiangItems = list;

            return entity.TryToJson();
        }

        #endregion

        #region Attribute

        /// <summary>
        ///     配置信息
        /// </summary>
        private SCCConfig Config;

        /// <summary>
        ///     当天抓取的最新一期开奖记录
        /// </summary>
        private OpenCode3DTModel LatestItem;

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
        private SCCLottery currentLottery => SCCLottery.HeBeiHYC3;

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