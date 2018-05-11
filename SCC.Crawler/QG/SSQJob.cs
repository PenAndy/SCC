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

namespace SCC.Crawler.QG
{
    /// <summary>
    ///     双色球
    /// </summary>
    public class SSQJob : IJob
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        public SSQJob()
        {
            log = new LogHelper();
            services = IOC.Resolve<IDTOpenCode>();
            email = IOC.Resolve<IEmail>();
        }


        #region SSQ作业执行入口
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
                if (CommonHelper.CheckDTIsNeedGetData(Config)) DoMainUrl();
                if (!LatestItem.Term.ToString().StartsWith(CommonHelper.SCCSysDateTime.ToString("yy")))
                    LatestItem = new OpenCode7DTModel
                    {
                        Term = CommonHelper.GenerateQiHaoYYYYQQQ(0),
                        OpenTime = new DateTime(CommonHelper.SCCSysDateTime.Year, 1, 1)
                    };
                //当今日开奖并且当前时间是晚上8点过后开始抓取
                if (CommonHelper.CheckTodayIsOpenDay(Config) && CommonHelper.SCCSysDateTime.Hour > 12) DoMainUrl();
                //

                //更新最后一期的开机号
                CheckGetKaiJiHao();


            }
            catch (Exception ex)
            {
                log.Error(GetType(), string.Format("【{0}】抓取时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            //保存最新期号
            context.JobDetail.JobDataMap["LatestItem"] = LatestItem;
        }
        #endregion


        #region SSQ爬取开机号数据
        /// <summary>
        /// 把获取的开机号数据更新到数据库
        /// </summary>
        private void CheckGetKaiJiHao()
        {
            try
            {
                List<KaiJiangHao> kai = GetKaiJiHao();
                foreach (var item in kai)
                {
                    var isSucc = services.UpdateSSQDetailByTerm(currentLottery, item.QiHao, item.Kaijianghao);
                    if (isSucc)
                    {

                        log.Info(GetType(), $"更新{Config.LotteryName}第{ item.QiHao}期开机号成功！");
                    }
                    else
                    {

                        log.Error(GetType(), $"更新{Config.LotteryName}第{ item.QiHao}期开机号失败！");
                    }
                }


            }
            catch (Exception e)
            {
                log.Error(GetType(), e);
            }
        }
        /// <summary>
        /// 根据网站获取开机号数据
        /// </summary>
        private List<KaiJiangHao> GetKaiJiHao()
        {
            List<KaiJiangHao> result = new List<KaiJiangHao>();

            var url = "https://www.8200.cn/kjh/ssq/kjih.htm?size=30";
            var htmlResource = NetHelper.GetUrlResponse(url, Encoding.GetEncoding("utf-8"));
            if (htmlResource == null) return null;

            if (!string.IsNullOrEmpty(htmlResource))
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);

                var table = doc.DocumentNode.SelectSingleNode("//table");
                if (table == null) return null;

                var trs = table.ChildNodes.Where(node => node.Name == "tr").ToList();
                KaiJiangHao model = null;
                for (var i = 0; i <= 15; i++)
                {
                    var tds = trs[i].ChildNodes.Where(S => S.Name.ToLower() == "td").ToList();
                    model = new KaiJiangHao();
                    model.QiHao = Convert.ToInt32(tds[0].InnerText.Trim());
                    if (tds[2].InnerText.Trim() == "--")
                    {
                        model.Kaijianghao = "";
                    }
                    else
                    {
                        string source = tds[2].InnerText.Replace(" + ", ",").Replace(" ", ",").Replace(",,", ",").Trim();
                        source = source.IndexOf(",") >= 0 ? source.Substring(1, source.Length - 1) : source;
                        model.Kaijianghao = source;
                    }
                    result.Add(model);
                }
            }
            return result;
        }
        #endregion


        #region SSQ爬取开奖号数据
        /// <summary>
        /// 把爬取的集合和数据库做比较并保存最新期数
        /// </summary>
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
        ///     通过主站点爬取开奖数据
        ///     (福建体彩网)
        ///     
        /// </summary>
        private List<OpenCode7DTModel> GetOpenListFromMainUrl(string mainUrl)
        {
            var result = new List<OpenCode7DTModel>();
            try
            {
                var url = new Uri(mainUrl);
                var htmlResource = NetHelper.GetUrlResponse(mainUrl, Encoding.GetEncoding("utf-8"));
                if (htmlResource == null) return result;

                var doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                var table = doc.DocumentNode.SelectSingleNode("//table");
                if (table == null) return result;
                var trs = table.ChildNodes.Where(node => node.Name == "tr").ToList();
                OpenCode7DTModel model = null;
                HtmlNode nodeA = null;
                var optimizeUrl = string.Empty;
                for (var i = 0; i < trs.Count; i++) //第一二行为表头
                {
                    var trstyle = trs[i].Attributes["style"];
                    if (trstyle != null && trstyle.Value == "display:none") continue;
                    var tds = trs[i].ChildNodes.Where(node => node.Name == "td").ToList();
                    if (tds.Count < 8) continue;
                    model = new OpenCode7DTModel();


                    model.Term = Convert.ToInt64(tds[0].InnerText.Trim());
                    optimizeUrl = model.Term.ToString();
                    model.OpenTime = Convert.ToDateTime(tds[1].InnerText.Substring(0, 5));
                    if (tds[2].ChildNodes.Count == 0) continue;
                    var b = tds[2].ChildNodes.Where(n => n.Name.ToLower() == "b").ToList();
                    var span = b[0].ChildNodes.Where(n => n.Name.ToLower() == "span").ToList();
                    model.OpenCode1 = Convert.ToInt32(span[0].InnerText.Replace(" ", "").Substring(0, 2).Trim());
                    model.OpenCode2 = Convert.ToInt32(span[0].InnerText.Replace(" ", "").Substring(2, 2).Trim());
                    model.OpenCode3 = Convert.ToInt32(span[0].InnerText.Replace(" ", "").Substring(4, 2).Trim());
                    model.OpenCode4 = Convert.ToInt32(span[0].InnerText.Replace(" ", "").Substring(6, 2).Trim());
                    model.OpenCode5 = Convert.ToInt32(span[0].InnerText.Replace(" ", "").Substring(8, 2).Trim());
                    model.OpenCode6 = Convert.ToInt32(span[0].InnerText.Replace(" ", "").Substring(10, 2).Trim());
                    model.OpenCode7 = Convert.ToInt32(span[1].InnerText.Replace(" ", "").Substring(0, 2).Trim());
                    var details = GetKaijiangDetails(optimizeUrl);
                    model.Spare = details;
                    result.Add(model);
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
                    string.Format("【{0}】通过主站抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            return result;
        }
        /// <summary>
        /// 双色球详情
        /// </summary>
        /// <param name="optimizeUrl"></param>
        /// <returns></returns>
        private string GetKaijiangDetails(string optimizeUrl)
        {
            var url = "https://www.8200.cn/kjh/ssq/" + optimizeUrl + ".htm";
            var htmlResource = NetHelper.GetUrlResponse(url, Encoding.GetEncoding("utf-8"));

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlResource);

            var div = doc.DocumentNode.SelectSingleNode("//div[@class='text-16']");
            if (div == null) return null;

            //爬去奖金
            var jiangjin = div.ChildNodes.Where(node => node.Name == "p").ToList();


            //爬去奖项
            //var tbody = div.ChildNodes.Where(node => node.Name == "tbody").ToList();
            var table = doc.DocumentNode.SelectSingleNode("//table");
            var trs = table.ChildNodes.Where(node => node.Name == "tr").ToList();
            var gdje = jiangjin[3].InnerText.Replace(" 亿元", "").Replace("奖池滚存：", "").Replace("--", "0").Replace(",", "")
                .Trim();
            var trje = jiangjin[2].InnerText.Replace(" 亿元", "").Replace("本期销量：", "").Replace("--", "0").Replace(",", "")
                .Trim();


            var entity = new KaijiangDetailsEntity
            {
                Gdje = gdje == "0" ? "0" : (double.Parse(gdje) * 100000000).ToString(),
                Trje = trje == "0" ? "0" : (double.Parse(trje) * 100000000).ToString()
            };
            //TODO 

            //组装详情  
            var list = new List<Kaijiangitem>();
            for (var i = 0; i < trs.Count; i++)
            {
                var tds = trs[i].ChildNodes.Where(node => node.Name == "td").ToList();


                var kaijiangitem = new Kaijiangitem();

                var TotalMoney = tds[1].InnerText.Replace("元", "").Replace("--", "0").Replace(",", "").Trim();
                kaijiangitem.Name = tds[0].InnerText.Trim();
                kaijiangitem.TotalMoney = TotalMoney == "0" ? "0" : double.Parse(TotalMoney).ToString();
                kaijiangitem.Total = tds[2].InnerText.Trim().Replace(" 注", "");
                list.Add(kaijiangitem);
            }

            entity.KaiJiangItems = list;

            return entity.TryToJson();
        }
        #endregion


        #region SSQ初始化

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
        private SCCLottery currentLottery => SCCLottery.SSQ;

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