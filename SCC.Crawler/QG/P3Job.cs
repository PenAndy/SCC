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
    ///     排列3
    /// </summary>
    public class P3Job : IJob
    {
        public P3Job()
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
            LatestItem = context.JobDetail.JobDataMap["LatestItem"] as OpenCodePL3TModel;
            try
            {
                //服务启动时配置初始数据
                if (LatestItem == null)
                {
                    LatestItem = services.GetOpenCodePL3TLastItem(currentLottery);
                    if (LatestItem == null)
                        LatestItem = new OpenCodePL3TModel
                        {
                            Term = CommonHelper.GenerateQiHaoYYYYQQQ(0),
                            OpenTime = new DateTime(CommonHelper.SCCSysDateTime.Year, 1, 1)
                        };
                }

                //程序时间第二天，程序根据配置检查是否昨天有开奖
                isGetData = false;
                if (CommonHelper.CheckDTIsNeedGetData(Config)) DoMainUrl();
                if (!LatestItem.Term.ToString().StartsWith(CommonHelper.SCCSysDateTime.ToString("yy")))
                    LatestItem = new OpenCodePL3TModel
                    {
                        Term = CommonHelper.GenerateQiHaoYYYYQQQ(0),
                        OpenTime = new DateTime(CommonHelper.SCCSysDateTime.Year, 1, 1)
                    };
                //当今日开奖并且当前时间是晚上8点过后开始抓取
                if (CommonHelper.CheckTodayIsOpenDay(Config) && CommonHelper.SCCSysDateTime.Hour > 12) DoMainUrl();
                CheckGetKaiJiHao();
            }
            catch (Exception ex)
            {
                log.Error(GetType(), string.Format("【{0}】抓取时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            //保存最新期号
            context.JobDetail.JobDataMap["LatestItem"] = LatestItem;
        }

        /// <summary>
        ///     通过主站点爬取开奖数据
        ///     (福建体彩网)
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
                OpenCodePL3TModel matchItem = null;
                for (var i = startQiNum; i <= newestQiHao; i++)
                {
                    getQiHao = i.ToString();
                    matchItem = openList.FirstOrDefault(r => r.Term.ToString() == getQiHao);

                    if (matchItem != null && services.AddDTOpenPL3Code(currentLottery, matchItem))
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
        /// 获取开奖号列表
        /// </summary>
        /// <param name="mainUrl"></param>
        /// <returns></returns>
        private List<OpenCodePL3TModel> GetOpenListFromMainUrl(string mainUrl)
        {
            var result = new List<OpenCodePL3TModel>();
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
                OpenCodePL3TModel model = null;
                HtmlNode nodeA = null;
                var optimizeUrl = string.Empty;
                for (var i = 0; i < trs.Count; i++) //第一二行为表头
                {
                    var trstyle = trs[i].Attributes["style"];
                    if (trstyle != null && trstyle.Value == "display:none") continue;
                    var tds = trs[i].ChildNodes.Where(node => node.Name == "td").ToList();
                    if (tds.Count < 6) continue;
                    model = new OpenCodePL3TModel();

                    var Term = tds[0].InnerText.Trim();
                    if (Term.Length <= 7)
                    {
                        model.Term = Convert.ToInt64("20" + Term);
                    }
                    else
                    {
                        model.Term = Convert.ToInt64(Term);
                    }
                    optimizeUrl = model.Term.ToString();
                    model.OpenTime = Convert.ToDateTime(tds[1].InnerText.Substring(0, 5));
                    var openCodeString = tds[2].InnerText.Replace("    ", "").Trim();
                    if (openCodeString == "--")
                    {
                        model.OpenCode1 = Convert.ToInt32("-1");
                        model.OpenCode2 = Convert.ToInt32("-1");
                        model.OpenCode3 = Convert.ToInt32("-1");
                    }
                    else
                    {
                        model.OpenCode1 = Convert.ToInt32(openCodeString.Substring(0, 1));
                        model.OpenCode2 = Convert.ToInt32(openCodeString.Substring(2, 1));
                        model.OpenCode3 = Convert.ToInt32(openCodeString.Substring(4, 1));
                    }


                    model.KaiJiHao = null;
                    model.ShiJiHao = tds[3].InnerText.Trim().Replace(" ", ",").Trim();
                    var details = GetKaijiangDetails(optimizeUrl);
                    model.Spare = details;
                    result.Add(model);
                }

                var checkDataHelper = new CheckDataHelper();
                var dbdata = services.GetListS<OpenCodePL5TModel>(currentLottery)
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
        /// 获取开奖详情
        /// </summary>
        /// <param name="optimizeUrl"></param>
        /// <returns></returns>
        private string GetKaijiangDetails(string optimizeUrl)
        {
            var url = "https://www.8200.cn/kjh/p3/20" + optimizeUrl + ".htm";
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
            var trje = jiangjin[2].InnerText.Replace(" 万元", "").Replace("本期销量：", "").Replace("--", "0").Replace(",", "")
                .Trim();


            var entity = new KaijiangDetailsEntity
            {
                Gdje = null,
                Trje = trje == "0" ? "0" : (double.Parse(trje) * 10000).ToString()
            };
            //TODO 

            //组装详情  
            var list = new List<Kaijiangitem>();
            for (var i = 0; i < trs.Count; i++)
            {
                var tds = trs[i].ChildNodes.Where(node => node.Name == "td").ToList();


                var kaijiangitem = new Kaijiangitem();
                var TotalMoney = tds[1].InnerText.Replace(" 万元", "").Replace(" 元", "").Replace("--", "0").Replace(",", "").Trim();


                kaijiangitem.Name = tds[0].InnerText.Trim();
                kaijiangitem.TotalMoney = TotalMoney == "0" ? "0" : (double.Parse(TotalMoney) * 1).ToString();
                kaijiangitem.Total = tds[2].InnerText.Trim().Replace(" 注", "").Replace("--", "0").Trim();
                list.Add(kaijiangitem);
            }

            entity.KaiJiangItems = list;

            return entity.TryToJson();
        }

        #region 爬去开机号的数据
        /// <summary>
        /// 获取开机号列表
        /// </summary>
        /// <returns></returns>
        private List<KaiJiangHao> GetKaiJiHao()
        {
            var result = new List<KaiJiangHao>();
            var url = "http://kjh.55128.cn/p3-kaijihao-100.htm";
            var htmlResource = NetHelper.GetUrlResponse(url, Encoding.GetEncoding("utf-8"));
            if (htmlResource == null) return null;
            if (!string.IsNullOrEmpty(htmlResource))
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);

                var table = doc.DocumentNode.SelectSingleNode("//tbody");
                if (table == null) return result;
                var trs = table.ChildNodes.Where(node => node.Name == "tr").ToList();
                KaiJiangHao model = null;
                for (var i = 0; i < 10; i++)
                {
                    var tds = trs[i].ChildNodes.Where(S => S.Name.ToLower() == "td").ToList();
                    model = new KaiJiangHao();
                    model.QiHao = Convert.ToInt32(tds[0].InnerText.Trim());
                    if (tds[2].InnerText.Trim() == "--")
                    {
                        model.Kaijianghao = null;
                    }
                    else
                    {
                        string source = tds[2].InnerText.Replace("，", ",").Trim();

                        model.Kaijianghao = source;
                    }
                    result.Add(model);
                }
            }
            return result;
        }
        /// <summary>
        ///更新开机号
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
        #endregion


        #region Attribute

        /// <summary>
        ///     配置信息
        /// </summary>
        private SCCConfig Config;

        /// <summary>
        ///     当天抓取的最新一期开奖记录
        /// </summary>
        private OpenCodePL3TModel LatestItem;

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
        private SCCLottery currentLottery => SCCLottery.PL3;

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