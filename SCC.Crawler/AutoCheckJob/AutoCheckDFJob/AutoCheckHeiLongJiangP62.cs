﻿using System;
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
    public class AutoCheckHeiLongJiangP62 : IJob
    {
        public AutoCheckHeiLongJiangP62()
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
            var url = "https://www.8200.cn/kjh/hljp62/" + qishu + ".htm";
            var htmlResource = NetHelper.GetUrlResponse(url, Encoding.GetEncoding("utf-8"));
            if (htmlResource == null) return null;

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlResource);

            var div = doc.DocumentNode.SelectSingleNode("//div[@class='maininfo']");
            if (div == null) return null;

            var div1 = div.ChildNodes.Where(node => node.Name == "div").ToList();
            var div2 = div1[0].ChildNodes.Where(node => node.Name == "div").ToList();
            var gdjediv = div2[5].ChildNodes.Where(node => node.Name == "span").ToList();
            var trjediv = div2[4].ChildNodes.Where(node => node.Name == "span").ToList();
            //爬去奖金
            var jiangjin = div.ChildNodes.Where(node => node.Name == "p").ToList();


            //爬去奖项
            //var tbody = div.ChildNodes.Where(node => node.Name == "tbody").ToList();
            var table = doc.DocumentNode.SelectSingleNode("//table");
            var trs = table.ChildNodes.Where(node => node.Name == "tr").ToList();

            var gdje = gdjediv[1].InnerText.Replace("元", "").Replace("奖池奖金：", "").Replace("--", "0").Replace(",", "")
                .Trim();
            var trje = trjediv[1].InnerText.Replace("元", "").Replace("全国销量：", "").Replace("--", "0").Replace(",", "")
                .Trim();


            var entity = new KaijiangDetailsEntity
            {
                Gdje = gdje == "0" ? "0" : (double.Parse(gdje) * 1).ToString(),
                Trje = trje == "0" ? "0" : (double.Parse(trje) * 1).ToString()
            };
            //TODO 

            //组装详情  
            var list = new List<Kaijiangitem>();
            for (var i = 0; i < trs.Count; i++)
            {
                var tds = trs[i].ChildNodes.Where(node => node.Name == "td").ToList();


                var kaijiangitem = new Kaijiangitem();

                var TotalMoney = tds[2].InnerText.Replace("元", "").Replace("--", "0").Replace(",", "").Trim();
                kaijiangitem.Name = tds[0].InnerText.Trim();
                kaijiangitem.TotalMoney = TotalMoney == "0" ? "0" : double.Parse(TotalMoney).ToString();
                kaijiangitem.Total = tds[1].InnerText.Replace(" 注", "").Replace("--", "0").Trim();
                list.Add(kaijiangitem);
            }

            entity.KaiJiangItems = list;

            return entity.TryToJson();
        }

        #region Attribute

        /// <summary>
        ///     配置信息
        /// </summary>
        private SCCConfig Config;

        private readonly LogHelper log;

        /// <summary>
        ///     数据服务
        /// </summary>
        private readonly IDTOpenCode services;

        /// <summary>
        ///     当前彩种
        /// </summary>
        private SCCLottery currentLottery => SCCLottery.HeiLongJiangP62;

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