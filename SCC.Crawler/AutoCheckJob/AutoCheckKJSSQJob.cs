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
    public class AutoCheckKJSSQJob : IJob
    {
        public AutoCheckKJSSQJob()
        {
            log = new LogHelper();
            services = IOC.Resolve<IDTOpenCode>();
            email = IOC.Resolve<IEmail>();
        }

        public void Execute(IJobExecutionContext context)
        {
            Config = CommonHelper.GetConfigFromDataMap(context.JobDetail.JobDataMap);
            //CheckGetKaiJiHao();
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
                        var res = GetKaijiangDetails(key.ToString());
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
            var url = "https://www.8200.cn/kjh/ssq/" + qishu + ".htm";
            var htmlResource = NetHelper.GetUrlResponse(url, Encoding.GetEncoding("utf-8"));
            if (htmlResource == null) return null;
            if (!string.IsNullOrEmpty(htmlResource))
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);

                var div = doc.DocumentNode.SelectSingleNode("//div[@class='text-16']");
                if (div == null) return null;
                //爬去奖金
                var jiangjin = div.ChildNodes.Where(node => node.Name == "p").ToList();
                var kaijianghao = jiangjin[0].InnerText.Remove(0, 8).Remove(22);
                //爬去奖项
                //var tbody = div.ChildNodes.Where(node => node.Name == "tbody").ToList();
                var table = doc.DocumentNode.SelectSingleNode("//table");
                var trs = table.ChildNodes.Where(node => node.Name == "tr").ToList();
                var gdje = jiangjin[3].InnerText.Replace(" 亿元", "").Replace("奖池滚存：", "").Replace("--", "0")
                    .Replace(",", "").Trim();
                var trje = jiangjin[2].InnerText.Replace(" 亿元", "").Replace("本期销量：", "").Replace("--", "0")
                    .Replace(",", "").Trim();

                var entity = new KaijiangDetailsEntity
                {
                    Gdje = gdje == "0" ? "0" : (double.Parse(gdje) * 100000000).ToString(),
                    Trje = trje == "0" ? "0" : (double.Parse(trje) * 100000000).ToString()
                };

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

            return "";
        }

        //#region  更新双色球开机号数据
        //private List<KaiJiangHao> GetKaiJiHao()
        //{
        //    var result = new List<KaiJiangHao>();
        //    var url = "https://www.8200.cn/kjh/ssq/kjih.htm?size=300";
        //    var htmlResource = NetHelper.GetUrlResponse(url, Encoding.GetEncoding("utf-8"));
        //    if (htmlResource == null) return null;
        //    if (!string.IsNullOrEmpty(htmlResource))
        //    {
        //        var doc = new HtmlDocument();
        //        doc.LoadHtml(htmlResource);

        //        var table = doc.DocumentNode.SelectSingleNode("//table");
        //        if (table == null) return result;
        //        var trs = table.ChildNodes.Where(node => node.Name == "tr").ToList();
        //        KaiJiangHao model = null;
        //        for (var i = 0; i < trs.Count; i++)
        //        {
        //            var tds = trs[i].ChildNodes.Where(S => S.Name.ToLower() == "td").ToList();
        //            model = new KaiJiangHao();
        //            model.QiHao = Convert.ToInt32(tds[0].InnerText.Trim());
        //            if (tds[2].InnerText.Trim() == "--")
        //            {
        //                model.Kaijianghao = null;
        //            }
        //            else
        //            {
        //                string source = tds[2].InnerText.Replace(" + ", ",").Replace(" ", ",").Replace(",,", ",").Trim();
        //                source = source.IndexOf(",") >= 0 ? source.Substring(1, source.Length - 1) : source;
        //                model.Kaijianghao = source;
        //            }
        //            result.Add(model);
        //        }
        //    }
        //    return result;
        //}
        //private void CheckGetKaiJiHao()
        //{
        //    try
        //    {
        //        var kaiJiHao = GetKaiJiHao();
        //        var newestQiHao = kaiJiHao.Count();

        //        List<KaiJiangHao> kai = GetKaiJiHao();
        //        foreach (var item in kai)
        //        {
        //            var isSucc = services.UpdateSSQDetailByTerm(currentLottery, item.QiHao, item.Kaijianghao);
        //            if (isSucc)
        //            {
        //                Trace.WriteLine($"更新{Config.LotteryName}第{ item.QiHao}期开奖详情成功！");

        //                log.Info(GetType(), $"更新{Config.LotteryName}第{ item.QiHao}期开奖详情成功！");
        //            }
        //            else
        //            {
        //                Trace.WriteLine($"更新{Config.LotteryName}第{ item.QiHao}期开奖详情失败！");

        //                log.Error(GetType(), $"更新{Config.LotteryName}第{ item.QiHao}期开奖详情失败！");
        //            }
        //        }


        //    }
        //    catch (Exception e)
        //    {
        //        log.Error(GetType(), e);
        //    }
        //}

        ////<span class="text-red">11 15 20 21 26 33</span> + <span class="text-blue">15</span>
        ////<b class="text-16"> 08  14  17  22  23  29  + 02</b>
        ////<b class="text-16"><i class="bg-nude font-style-normal">01</i>  05  14 <i class="bg-nude font-style-normal">22</i>  26  29  + 13</b>
        //#endregion

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
        private SCCLottery currentLottery => SCCLottery.SSQ;

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