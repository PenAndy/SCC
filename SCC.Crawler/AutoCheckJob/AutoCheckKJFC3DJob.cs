using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using Quartz;
using SCC.Common;
using SCC.Interface;
using SCC.Models;

namespace SCC.Crawler.AutoCheckJob
{
    public class AutoCheckKJFC3DJob : IJob
    {
        public AutoCheckKJFC3DJob()
        {
            log = new LogHelper();
            services = IOC.Resolve<IDTOpenCode>();
            email = IOC.Resolve<IEmail>();
        }

        public void Execute(IJobExecutionContext context)
        {
            Config = CommonHelper.GetConfigFromDataMap(context.JobDetail.JobDataMap);

            //每天检测
            Check();
        }

        private void Check()
        {
            try
            {
                var dict = services.GetLast1NTerm(currentLottery, 10);
                if (dict.Count > 0)
                    foreach (var dic in dict)
                    {
                        var key = dic.Key;
                        var Spare = dic.Value;
                        var openList = GetOpenListFromMainUrl(Config.MainUrl);
                        OpenCodeFC3DTModel matchItem = null;
                        matchItem = openList.FirstOrDefault(r => r.Term.ToString() == key.ToString());
                        var res = GetKaijiangDetails(key.ToString());

                        //TODO 更新数据库
                        bool isSucc = services.UpdateKJDetail3DByTerm(currentLottery, key, res, matchItem);
                        if (isSucc)
                        {
                            Trace.WriteLine($"更新{Config.LotteryName}第{key}期开奖详情成功！");

                            log.Info(GetType(), $"更新{Config.LotteryName}第{key}期开奖详情成功！");
                        }
                        else
                        {
                            Trace.WriteLine($"更新{Config.LotteryName}第{key}期开奖详情失败！");

                            log.Error(GetType(), $"更新{Config.LotteryName}第{key}期开奖详情失败！");
                        }
                    }
            }
            catch (Exception e)
            {
                log.Error(GetType(), e);
            }
        }

        /// <summary>
        ///     获取开奖详情
        /// </summary>
        /// <param name="qishu"></param>
        /// <returns></returns>
        private string GetKaijiangDetails(string qishu)
        {
            var url = "https://www.8200.cn/kjh/3d/" + qishu + ".htm";
            var htmlResource = NetHelper.GetUrlResponse(url, Encoding.GetEncoding("utf-8"));
            if (htmlResource == null) return null;

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

            var trje = jiangjin[3].InnerText.Replace(" 万元", "").Replace("本期销量：", "").Replace("--", "0").Replace(",", "")
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

                var TotalMoney = tds[1].InnerText.Replace("元", "").Replace("--", "0").Replace(",", "").Trim();

                kaijiangitem.Name = tds[0].InnerText.Trim();
                kaijiangitem.TotalMoney = TotalMoney == "0" ? "0" : double.Parse(TotalMoney).ToString();
                kaijiangitem.Total = tds[2].InnerText.Trim().Replace(" 注", "").Replace("--", "0").Trim();
                list.Add(kaijiangitem);
            }

            entity.KaiJiangItems = list;

            return entity.TryToJson();
        }

        private List<OpenCodeFC3DTModel> GetOpenListFromMainUrl(string mainUrl)
        {
            var result = new List<OpenCodeFC3DTModel>();
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
                OpenCodeFC3DTModel model = null;
                HtmlNode nodeA = null;
                var optimizeUrl = string.Empty;
                for (var i = 0; i < trs.Count; i++) //第一二行为表头
                {
                    var trstyle = trs[i].Attributes["style"];
                    if (trstyle != null && trstyle.Value == "display:none") continue;
                    var tds = trs[i].ChildNodes.Where(node => node.Name == "td").ToList();
                    if (tds.Count < 9) continue;
                    model = new OpenCodeFC3DTModel();


                    model.Term = Convert.ToInt64(tds[0].InnerText.Trim());
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

                    model.KaiJiHao = tds[3].InnerText.Trim().Replace(" ", ",").Trim();
                    model.ShiJiHao = tds[4].InnerText.Trim().Replace(" ", ",").Trim();
                    var details = GetKaijiangDetails(optimizeUrl);
                    model.Spare = details;
                    result.Add(model);
                }

                result = result.OrderByDescending(S => S.Term).ToList();
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过主抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            return result;
        }

        #region Attribute

        /// <summary>
        ///     配置信息
        /// </summary>
        private SCCConfig Config;

        /// <summary>
        ///     当天抓取的最新一期开奖记录
        /// </summary>
        private OpenCode7DTModel LatestItem = null;

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
        private SCCLottery currentLottery => SCCLottery.FC3D;

        /// <summary>
        ///     邮件接口
        /// </summary>
        private IEmail email;

        /// <summary>
        ///     是否本次运行抓取到开奖数据
        /// </summary>
        private bool isGetData = false;

        #endregion
    }
}