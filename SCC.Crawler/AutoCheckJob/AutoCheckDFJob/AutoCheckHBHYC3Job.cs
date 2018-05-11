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

namespace SCC.Crawler.AutoCheckJob.AutoCheckDFJob
{
    public class AutoCheckHBHYC3Job : IJob
    {
        public AutoCheckHBHYC3Job()
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
            var url = "http://kaijiang.500.com/shtml/hbhy3/" + qishu + ".shtml";
            var htmlResource = NetHelper.GetUrlResponse(url, Encoding.GetEncoding("gb2312"));
            if (htmlResource == null) return null;
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlResource);
            var div = doc.DocumentNode.SelectSingleNode("//div[@style='margin:20px auto; width:500px;']");
            if (div == null) return null;
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
        private SCCLottery currentLottery => SCCLottery.HeBeiHYC3;

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