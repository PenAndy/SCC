using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using Quartz;
using SCC.Common;
using SCC.Interface;
using SCC.Models;

namespace SCC.Crawler.LotteryNews
{
  public  class QLCNewsJob : IJob
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public QLCNewsJob()
        {
            log = new LogHelper();
            services = IOC.Resolve<IDTOpenCode>();
            email = IOC.Resolve<IEmail>();
        }
        /// <summary>
        /// 执行入口
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            Config = CommonHelper.GetConfigFromDataMap(context.JobDetail.JobDataMap);


            DoMainUrl();
        }
        /// <summary>
        /// 执行主站技巧
        /// </summary>
        private void DoMainUrl()
        {
            List<string> urls = GetMainUrl(Config);
            LotteryNewsModel lotterySkill = null;
            foreach (string url in urls)
            {
                List<LotteryNewsModel> res = GetOpenListFromMainUrl(url);

                foreach (var LotteryNewsModel in res)
                {
                    if (LotteryNewsModel.Content != null)
                    {
                        if (services.LotteryNewsModel(currentLottery, LotteryNewsModel))
                        {
                            //Do Success Log
                            log.Info(GetType(), CommonHelper.GetJobMainLogInfo(Config, LotteryNewsModel.Title));

                            isGetData = true;
                        }
                    }
                }

            }
        }

        /// <summary>
        /// 组装主站爬取地址
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private List<string> GetMainUrl(SCCConfig config)
        {
            List<string> urlList = new List<string>();
            string url = config.MainUrl;
            int pages = config.MainUrlPages > 0 ? config.MainUrlPages : 1;
            for (int i = 1; i <= pages; i++)
            {
                string res;
                if (i == 1)
                {
                    res = "http://www.zhcw.com/xinwen/caizhongxinwenqlc/";
                }
                else
                {
                    res = string.Format(url, i);
                }

                if (!urlList.Contains(res))
                {
                    urlList.Add(res);
                }
            }
            return urlList;
        }

        /// <summary>
        /// 爬取主站技巧列表
        /// </summary>
        /// <param name="mainUrl"></param>
        /// <returns></returns>
        private List<LotteryNewsModel> GetOpenListFromMainUrl(string mainUrl)
        {
            var result = new List<LotteryNewsModel>();
            try
            {
                var url = new Uri(mainUrl);
                var htmlResource = NetHelper.GetUrlResponse(mainUrl, Encoding.GetEncoding("utf-8"));
                if (htmlResource == null) return result;

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                //获取li下面所有a标签
                HtmlNodeCollection nodeList = doc.DocumentNode.SelectNodes("//*[@class='Nleftbox']/ul/li/span/a");
                if (nodeList == null) return result;

                List<string> urls = new List<string>();
                //遍历a标签
                foreach (HtmlNode node in nodeList)
                {
                    HtmlAttribute attr = node.Attributes.SingleOrDefault(a => a.Name.Equals("href"));
                    if (attr != null)
                    {
                        string href = Host + attr.Value;
                        //去重
                        if (!urls.Contains(href))
                        {
                            urls.Add(href);
                        }

                    }
                }

                foreach (var url1 in urls)
                {
                    var LotterySkill = GetSkillModel(url1);
                    result.Add(LotterySkill);
                }
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过主抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }
            return result;
        }
        /// <summary>
        /// 根据主站url获取技巧详情
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private LotteryNewsModel GetSkillModel(string url)
        {
            LotteryNewsModel lotterySkill = new LotteryNewsModel();
            try
            {
                var htmlResource = NetHelper.GetUrlResponse(url, Encoding.GetEncoding("utf-8"));
                if (htmlResource == null) return lotterySkill;

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                //获取li下面所有a标签
                var div = doc.DocumentNode.SelectSingleNode("//*[@class='news_content']");
                var Title = div.ChildNodes.Where(node => node.Name == "h2").ToList();
                var div1 = div.ChildNodes.Where(node => node.Name == "div").ToList();
                string txt = div1[2].InnerHtml.Trim();
                var Content = txt.Replace("<img src=\"", " <img src=\"http://www.zhcw.com").Replace("中彩网讯", "").Replace("中彩网综合报道", "综合报道").Replace("中彩网", "");
                if (Content == "")
                {
                    Content = null;
                }



                lotterySkill.Title = Title[0].InnerText.Trim();
                lotterySkill.Author = "cn55128";
                lotterySkill.Content = Content;
                lotterySkill.IsDelete = false;
                lotterySkill.SourceUrl = url.ToString();
                lotterySkill.TypeID = lotterySkillType;
                lotterySkill.TypeName = lotterySkillType.GetEnumDescription();
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过主抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }
            return lotterySkill;
        }

        #region Attribute

        /// <summary>
        /// 主机地址
        /// </summary>
        public string Host = "http://www.zhcw.com";
        /// <summary>
        ///     配置信息
        /// </summary>
        private SCCConfig Config;

        /// <summary>
        ///     当天抓取的最新一期开奖记录
        /// </summary>
        private LotteryNewsModel LatestItem = null;

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
        private SCCLottery currentLottery => SCCLottery.LotteryNews;

        /// <summary>
        /// 福彩3D技巧
        /// </summary>
        private LotteryNewsType lotterySkillType = LotteryNewsType.QLCNews;

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
