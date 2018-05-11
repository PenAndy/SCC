using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Quartz;
using SCC.Common;
using SCC.Interface;
using SCC.Models;

namespace SCC.Crawler.AutoCheckJob.AutoCheckDFJob
{
    public class AutoCheckFJ31X7Job : IJob
    {
        public AutoCheckFJ31X7Job()
        {
            log = new LogHelper();
            services = IOC.Resolve<IDTOpenCode>();
        }

        public void Execute(IJobExecutionContext context)
        {
            Config = CommonHelper.GetConfigFromDataMap(context.JobDetail.JobDataMap);

            Check();
        }

        private void Check()
        {
            try
            {
                var dict = services.GetLast1NTerm(currentLottery, 10);
                //数据库期号与网站上期号对应关系字典
                var dictIid = GetTermAndIid();

                if (dict.Count > 0)
                    foreach (var dic in dict)
                    {
                        //期号
                        var key = dic.Key;
                        //开奖详情
                        var Spare = dic.Value;
                        //存在才去获取详情
                        if (dictIid.ContainsKey(key.ToString()))
                        {
                            var res = GetKaijiangDetails(dictIid[key.ToString()]);

                            if (!string.IsNullOrEmpty(res) && !res.Equals(Spare))
                            {
                                //TODO 更新数据库
                                var isSucc = services.UpdateKJDetailByTerm(currentLottery, key, res);
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
                            else
                            {
                                Trace.WriteLine($"未更新{Config.LotteryName}第{key}期开奖详情！原因：内容相同。");
                            }
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
            var url = $"https://fx.cp2y.com/draw/draw.jsp?lid=10066&iid={qishu}";
            var htmlResource = NetHelper.GetUrlResponse(url, Encoding.GetEncoding("gb2312"));
            if (htmlResource == null) return null;
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlResource);

            var div = doc.DocumentNode.SelectSingleNode("//div[@class='fl lh24 f14']");
            if (div == null) return null;
            var div2 = div.ChildNodes.Where(node => node.Name == "div").ToList();
            //爬去奖金
            var trjeg = div2[1].ChildNodes.Where(node => node.Name == "span").ToList();
            var gdjeg = div2[2].ChildNodes.Where(node => node.Name == "span").ToList();

            //爬去奖项
            //var tbody = div.ChildNodes.Where(node => node.Name == "tbody").ToList();
            var table = doc.DocumentNode.SelectSingleNode("//table");
            var trs = table.ChildNodes.Where(node => node.Name == "tr").ToList();

            var gdje = gdjeg[1].InnerText.Replace("奖池资金累计金额：", "").Replace("元", "").Replace("--", "0").Replace(",", "")
                .Trim();
            var trje = trjeg[1].InnerText.Replace("本期投注总额：", "").Replace("元", "").Replace("--", "0").Replace(",", "")
                .Trim();

            var entity = new KaijiangDetailsEntity
            {
                Gdje = gdje == "0" ? "0" : (double.Parse(gdje) * 1).ToString(),
                Trje = trje == "0" ? "0" : (double.Parse(trje) * 1).ToString()
            };
            //TODO 

            //组装详情  
            var list = new List<Kaijiangitem>();
            for (var i = 1; i < trs.Count; i++)
            {
                var tds = trs[i].ChildNodes.Where(node => node.Name == "td").ToList();


                var kaijiangitem = new Kaijiangitem();

                var TotalMoney = tds[3].InnerText.Replace("元", "").Replace("--", "0").Replace(",", "").Trim();
                kaijiangitem.Name = tds[1].InnerText.Trim();
                kaijiangitem.TotalMoney = TotalMoney == "0" ? "0" : double.Parse(TotalMoney).ToString();
                kaijiangitem.Total = tds[2].InnerText.Replace(" 注", "").Replace("--", "0").Trim();
                list.Add(kaijiangitem);
            }

            entity.KaiJiangItems = list;

            return entity.TryToJson();
        }

        /// <summary>
        ///     期数-转换后期数
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetTermAndIid()
        {
            var dict = new Dictionary<string, string>();
            var url = "https://fx.cp2y.com/draw/draw.jsp?lid=10066";

            var htmlResource = NetHelper.GetUrlResponse(url, Encoding.GetEncoding("gb2312"));
            if (!string.IsNullOrEmpty(htmlResource))
            {
                var document = new HtmlDocument();
                document.LoadHtml(htmlResource);

                var mainNode = document.DocumentNode.SelectSingleNode("//*[@id=\"history-select\"]/div");

                var divs = mainNode.ChildNodes.Where(node => node.Name == "div").ToList();
                // <a class="option parent" href="?lid=10062&iid=17045697" value="17045697">第2017358期</a>

                foreach (var div in divs)
                {
                    var qi = div.InnerText.Replace("第", "").Replace("期", "");
                    if (qi.StartsWith("20")) qi = qi.Replace("20", "");
                    var a = div.InnerHtml.Replace("\"", "'");
                    // value=".+"
                    var regex = new Regex("value='.+'");
                    if (regex.IsMatch(a))
                    {
                        //value='17045697'
                        var val = regex.Matches(a)[0].Groups[0].Value;
                        if (!string.IsNullOrEmpty(val)) val = val.Replace("value=", "").Replace("\'", "");

                        dict.Add(qi, val);
                    }
                }
            }

            return dict;
        }


        #region Attribute

        /// <summary>
        ///     配置信息
        /// </summary>
        private SCCConfig Config;

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
        private SCCLottery currentLottery => SCCLottery.FuJian31x7;

        #endregion
    }
}