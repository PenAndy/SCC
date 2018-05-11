using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using HtmlAgilityPack;
using Quartz;
using SCC.Common;
using SCC.Interface;
using SCC.Models;

namespace SCC.Crawler.LotterySkill
{
    /// <summary>
    /// 3d抓取技巧
    /// </summary>
    public class FC3DSkillJob : IJob
    {

        public FC3DSkillJob()
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

            DoBackUrl();

            DoMainUrl();
        }
        /// <summary>
        /// 执行主站技巧
        /// </summary>
        private void DoMainUrl()
        {
            List<string> urls = GetMainUrl(Config);
            LotterySkillModel lotterySkill = null;
            foreach (string url in urls)
            {
                List<LotterySkillModel> res = GetOpenListFromMainUrl(url);

                foreach (var lotterySkillModel in res)
                {
                    if (services.LotterySkillModel(currentLottery, lotterySkillModel))
                    {
                        //Do Success Log
                        log.Info(GetType(), CommonHelper.GetJobMainLogInfo(Config, lotterySkillModel.Title));

                        isGetData = true;
                    }
                }

            }
        }
        /// <summary>
        /// 执行副站技巧
        /// </summary>
        private void DoBackUrl()
        {
            List<string> urls = GetBackUrl(Config);
            LotterySkillModel lotterySkill = null;
            foreach (string url in urls)
            {
                List<LotterySkillModel> res = GetOpenListFromBackUrl(url);

                foreach (var lotterySkillModel in res)
                {
                    if (services.LotterySkillModel(currentLottery, lotterySkillModel))
                    {
                        //Do Success Log
                        log.Info(GetType(), CommonHelper.GetJobMainLogInfo(Config, lotterySkillModel.Title));

                        isGetData = true;
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
                string res = string.Format(url, i);
                if (!urlList.Contains(res))
                {
                    urlList.Add(res);
                }
            }
            return urlList;
        }
        /// <summary>
        /// 组装副站爬取地址
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private List<string> GetBackUrl(SCCConfig config)
        {
            List<string> urlList = new List<string>();
            string url = config.BackUrl;
            int pages = config.MainUrlPages > 0 ? config.BackUrlPages : 1;
            for (int i = 1; i <= pages; i++)
            {
                string res = string.Format(url, i);
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
        private List<LotterySkillModel> GetOpenListFromMainUrl(string mainUrl)
        {
            var result = new List<LotterySkillModel>();
            try
            {
                var url = new Uri(mainUrl);
                var htmlResource = NetHelper.GetUrlResponse(mainUrl, Encoding.GetEncoding("utf-8"));
                if (htmlResource == null) return result;

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                //获取li下面所有a标签
                HtmlNodeCollection nodeList = doc.DocumentNode.SelectNodes("//*[@class='art-list']/ul/li/a");
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
        /// 爬取副站技巧列表
        /// </summary>
        /// <param name="backUrl"></param>
        /// <returns></returns>
        private List<LotterySkillModel> GetOpenListFromBackUrl(string backUrl)
        {
            var result = new List<LotterySkillModel>();
            try
            {
                var url = new Uri(backUrl);
                var htmlResource = NetHelper.GetUrlResponse(backUrl, Encoding.GetEncoding("utf-8"));
                if (htmlResource == null) return result;

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                //获取li下面所有a标签
                HtmlNodeCollection nodeList = doc.DocumentNode.SelectNodes("//*[@class='listContainer']/ul/li/a");
                if (nodeList == null) return result;

                List<string> urls = new List<string>();
                //遍历a标签
                foreach (HtmlNode node in nodeList)
                {
                    HtmlAttribute attr = node.Attributes.SingleOrDefault(a => a.Name.Equals("href"));
                    if (attr != null)
                    {
                        string href = HostBackUrl + attr.Value;
                        //去重
                        if (!urls.Contains(href))
                        {
                            urls.Add(href);
                        }

                    }
                }

                foreach (var url1 in urls)
                {
                    var LotterySkill = GetSkillModelBackUrl(url1);
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
        /// 根据副站url获取技巧详情
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private LotterySkillModel GetSkillModelBackUrl(string url)
        {
            LotterySkillModel lotterySkill = new LotterySkillModel();
            try
            {
                var htmlResource = NetHelper.GetUrlResponse(url, Encoding.GetEncoding("utf-8"));
                if (htmlResource == null) return lotterySkill;

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                //获取li下面所有a标签
                var div = doc.DocumentNode.SelectSingleNode("//*[@class='article mb-10']");
                var Title = div.ChildNodes.Where(node => node.Name == "h5").ToList();
                var div1 = div.ChildNodes.Where(node => node.Name == "div").ToList();

                lotterySkill.Title = Title[0].InnerText.Trim();
                lotterySkill.Author = "cn55128";
                lotterySkill.Content = div1[2].InnerHtml.Trim();
                lotterySkill.IsDelete = false;
                lotterySkill.SourceUrl = url.ToString();
                lotterySkill.TypeId = lotterySkillType;
                lotterySkill.TypeName = lotterySkillType.GetEnumDescription();
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过主抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }
            return lotterySkill;
        }
        /// <summary>
        /// 根据主站url获取技巧详情
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private LotterySkillModel GetSkillModel(string url)
        {
            LotterySkillModel lotterySkill = new LotterySkillModel();
            try
            {
                var htmlResource = NetHelper.GetUrlResponse(url, Encoding.GetEncoding("utf-8"));
                if (htmlResource == null) return lotterySkill;

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                //获取li下面所有a标签
                var div = doc.DocumentNode.SelectSingleNode("//*[@class='artile']");
                var Title = div.ChildNodes.Where(node => node.Name == "h1").ToList();
                var div1 = div.ChildNodes.Where(node => node.Name == "div").ToList();

                lotterySkill.Title = Title[0].InnerText.Trim();
                lotterySkill.Author = "cn55128";
                lotterySkill.Content = div1[1].InnerHtml.Trim();
                lotterySkill.IsDelete = false;
                lotterySkill.SourceUrl = url.ToString();
                lotterySkill.TypeId = lotterySkillType;
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
        public string Host = "http://www.55125.cn/";
        /// <summary>
        /// 副站地址
        /// </summary>
        public string HostBackUrl = "https://www.cz89.com/";
        /// <summary>
        ///     配置信息
        /// </summary>
        private SCCConfig Config;
        /// <summary>
        ///     当天抓取的最新一期开奖记录
        /// </summary>
        private LotterySkillModel LatestItem = null;
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
        private SCCLottery currentLottery => SCCLottery.LotterySkill;
        /// <summary>
        /// 福彩3D技巧
        /// </summary>
        private LotterySkillType lotterySkillType = LotterySkillType.FC3D;
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
