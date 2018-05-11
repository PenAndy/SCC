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
    ///     广东36选7
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class GD36X7Job : IJob
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        public GD36X7Job()
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
                //isGetData = false;
                //if (!LatestItem.Term.ToString().StartsWith(CommonHelper.SCCSysDateTime.ToString("yyyy")))
                //    LatestItem = new OpenCode7DTModel
                //    {
                //        Term = CommonHelper.GenerateQiHaoYYYYQQQ(0),
                //        OpenTime = new DateTime(CommonHelper.SCCSysDateTime.Year, 1, 1)
                //    };
                ////当今日开奖并且当前时间是晚上8点过后开始抓取
                //if (CommonHelper.CheckTodayIsOpenDay(Config) && CommonHelper.SCCSysDateTime.Hour > 12)
                //{
                    DoMainUrl();
                    DoBackUrl();
                //}
            }
            catch (Exception ex)
            {
                log.Error(GetType(), string.Format("【{0}】抓取时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            //保存最新期号
            context.JobDetail.JobDataMap["LatestItem"] = LatestItem;
        }
        /// <summary>
        /// 执行主站的数据插入
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
        /// 执行副站的数据插入
        /// </summary>
        private void DoBackUrl()
        {
            if (!string.IsNullOrEmpty(Config.BackUrl))
            {
                var OpenList = GetOpenListFromBackUrl(Config.BackUrl);
                if (OpenList.Count == 0) return; //无抓取数据
                //抓取到的最新期数
                var newestQiHao = Convert.ToInt32(OpenList.First().Term.ToString());
                //数据库里面最新期数
                var startQiNum = Convert.ToInt32(LatestItem.Term.ToString());
                if (startQiNum > newestQiHao) return; //无最新数据
                //处理最新开奖数据
                var getQiHao = string.Empty;
                OpenCode7DTModel matchItem = null;
                for (var i = startQiNum; i <= newestQiHao; i++)
                {
                    getQiHao = i.ToString();
                    matchItem = OpenList.FirstOrDefault(r => r.Term.ToString() == getQiHao);
                    if (matchItem != null && services.AddDTOpen7Code(currentLottery, matchItem))
                    {
                        //Do Success Log
                        log.Info(GetType(), CommonHelper.GetJobBackLogInfo(Config, getQiHao));
                        LatestItem = matchItem;
                        isGetData = true;
                    }
                }
            }
        }
        /// <summary>
        /// 根据主站获取数据列表
        /// </summary>
        /// <param name="mainUrl"></param>
        /// <returns></returns>
        private List<OpenCode7DTModel> GetOpenListFromMainUrl(string mainUrl)
        {
            var result = new List<OpenCode7DTModel>();
            try
            {
                var url = new Uri(mainUrl);
                var htmlResource = NetHelper.GetUrlResponse(mainUrl, Encoding.GetEncoding("gb2312"));
                if (htmlResource == null) return result;

                var doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                var table = doc.DocumentNode.SelectSingleNode("//table");
                if (table == null) return result;
                var trs = table.ChildNodes.Where(node => node.Name == "tr").ToList();
                OpenCode7DTModel model = null;
                HtmlNode nodeA = null;
                var optimizeUrl = string.Empty;
                for (var i = 2; i < trs.Count; i++) //第一二行为表头
                {
                    var trstyle = trs[i].Attributes["style"];
                    if (trstyle != null && trstyle.Value == "display:none") continue;
                    var tds = trs[i].ChildNodes.Where(node => node.Name == "td").ToList();
                    if (tds.Count < 9) continue;
                    model = new OpenCode7DTModel();
                    nodeA = tds[0].ChildNodes.Where(n => n.Name == "a").FirstOrDefault();
                    if (nodeA == null) continue;
                    model.Term = Convert.ToInt64(nodeA.InnerText.Trim());
                    optimizeUrl = nodeA.Attributes["href"].Value;
                    model.DetailUrl = new Uri(url, optimizeUrl).AbsoluteUri;
                    model.OpenTime = Convert.ToDateTime(tds[9].InnerText);
                    if (tds[1].ChildNodes.Count == 0) continue;
                    var opencodeNode = tds[1].ChildNodes.Where(n => n.Name.ToLower() == "i").ToList();
                    if (opencodeNode.Count < 7) continue;
                    model.OpenCode1 = Convert.ToInt32(opencodeNode[0].InnerText.Trim());
                    model.OpenCode2 = Convert.ToInt32(opencodeNode[1].InnerText.Trim());
                    model.OpenCode3 = Convert.ToInt32(opencodeNode[2].InnerText.Trim());
                    model.OpenCode4 = Convert.ToInt32(opencodeNode[3].InnerText.Trim());
                    model.OpenCode5 = Convert.ToInt32(opencodeNode[4].InnerText.Trim());
                    model.OpenCode6 = Convert.ToInt32(opencodeNode[5].InnerText.Trim());
                    model.OpenCode7 = Convert.ToInt32(opencodeNode[6].InnerText.Trim());
                    var details = GetKaijiangDetails(tds);
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
                    string.Format("【{0}】通过主站点抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            return result;
        }

        /// <summary>
        ///     获取主站下开奖详情
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        
        private string GetKaijiangDetails(List<HtmlNode> nodes)
        {
            var entity = new KaijiangDetailsEntity
            {
                Gdje = "0",
                Trje = string.IsNullOrEmpty(nodes[2].InnerText.Replace(",", "").Replace("元", ""))
                    ? "0"
                    : nodes[2].InnerText.Replace(",", "").Replace("元", "")
            };
            //TODO 
            var list1 = new List<Kaijiangitem>();
            //组装详情
            var list = new List<Kaijiangitem>
            {
                new Kaijiangitem
                {
                    Name = "一等奖",
                    Total = string.IsNullOrEmpty(nodes[3].InnerText) ? "0" : nodes[3].InnerText,
                    TotalMoney = string.IsNullOrEmpty(nodes[4].InnerText) ? "0" : nodes[4].InnerText
                },
                new Kaijiangitem
                {
                    Name = "二等奖",
                    Total = string.IsNullOrEmpty(nodes[5].InnerText) ? "0" : nodes[5].InnerText,
                    TotalMoney = string.IsNullOrEmpty(nodes[6].InnerText) ? "0" : nodes[6].InnerText
                },
                new Kaijiangitem
                {
                    Name = "三等奖",
                    Total = string.IsNullOrEmpty(nodes[7].InnerText) ? "0" : nodes[7].InnerText,
                    TotalMoney = string.IsNullOrEmpty(nodes[8].InnerText) ? "0" : nodes[8].InnerText
                }
            };
            entity.KaiJiangItems = list;

            return entity.TryToJson();
        }

        #region 通过副站爬取数据
        /// <summary>
        /// 根据副站的网址爬取数据列表
        /// </summary>
        /// <param name="mainUrl"></param>
        /// <returns></returns>
        private List<OpenCode7DTModel> GetOpenListFromBackUrl(string mainUrl)
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
                for (var i = 2; i < trs.Count; i++) //第一二行为表头
                {
                    var trstyle = trs[i].Attributes["style"];
                    if (trstyle != null && trstyle.Value == "display:none") continue;
                    var tds = trs[i].ChildNodes.Where(node => node.Name == "td").ToList();
                    if (tds.Count < 14) continue;
                    model = new OpenCode7DTModel();
                    nodeA = tds[13].ChildNodes.Where(n => n.Name == "a").FirstOrDefault();
                    if (nodeA == null) continue;
                    model.Term = Convert.ToInt64(tds[0].InnerText.Trim());

                    model.DetailUrl = new Uri(url, optimizeUrl).AbsoluteUri;
                    model.OpenTime = Convert.ToDateTime(tds[1].InnerText.Substring(0, 10));
                    if (tds[2].ChildNodes.Count == 0) continue;
                    var opencodeNode = tds[2].ChildNodes.Where(n => n.Name.ToLower() == "em").ToList();
                    if (opencodeNode.Count < 7) continue;
                    model.OpenCode1 = Convert.ToInt32(opencodeNode[0].InnerText.Trim());
                    model.OpenCode2 = Convert.ToInt32(opencodeNode[1].InnerText.Trim());
                    model.OpenCode3 = Convert.ToInt32(opencodeNode[2].InnerText.Trim());
                    model.OpenCode4 = Convert.ToInt32(opencodeNode[3].InnerText.Trim());
                    model.OpenCode5 = Convert.ToInt32(opencodeNode[4].InnerText.Trim());
                    model.OpenCode6 = Convert.ToInt32(opencodeNode[5].InnerText.Trim());
                    model.OpenCode7 = Convert.ToInt32(opencodeNode[6].InnerText.Trim());
                    var details =KaijiangDetails(tds);
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
                    string.Format("【{0}】通过主站点抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            return result;
        }

        /// <summary>
        ///     获取主站下开奖详情
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private string KaijiangDetails(List<HtmlNode> nodes)
        {
            var entity = new KaijiangDetailsEntity
            {
                Gdje = "0",
                Trje = "0"
            };
            //TODO 
            var list1 = new List<Kaijiangitem>();
            //组装详情
            var TotalMoney = nodes[4].InnerText.Replace("万", "").Replace("元", "").Replace(" ", "0").Trim();


            var list = new List<Kaijiangitem>
            {
                new Kaijiangitem
                {
                    Name = "一等奖",
                    Total = nodes[3].InnerText,
                    TotalMoney = TotalMoney = TotalMoney == "" ? "0" : (double.Parse(TotalMoney) * 1).ToString()
                },
                new Kaijiangitem
                {
                    Name = "二等奖",
                    Total = nodes[5].InnerText,
                    TotalMoney = nodes[6].InnerText.Replace("元", "")
                },
                new Kaijiangitem
                {
                    Name = "三等奖",
                    Total = nodes[7].InnerText,
                    TotalMoney = nodes[8].InnerText.Replace("元", "")
                },
                new Kaijiangitem
                {
                    Name = "四等奖",
                    Total = nodes[9].InnerText,
                    TotalMoney = "500"
                },
                new Kaijiangitem
                {
                    Name = "五等奖",
                    Total = nodes[10].InnerText,
                    TotalMoney = "100"
                },
                new Kaijiangitem
                {
                    Name = "六等奖",
                    Total = nodes[11].InnerText,
                    TotalMoney = "10"
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
        private SCCLottery currentLottery => SCCLottery.GuangDong36x7;

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