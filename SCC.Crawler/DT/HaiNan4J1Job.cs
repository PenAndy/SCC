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
    ///     海南4+1
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class HaiNan4J1Job : IJob
    {
        /// <summary>
        ///     河南四加一
        /// </summary>
        public HaiNan4J1Job()
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
            LatestItem = context.JobDetail.JobDataMap["LatestItem"] as OpenCode5DTModel;
            try
            {
                //服务启动时配置初始数据
                if (LatestItem == null)
                {
                    LatestItem = services.GetOpenCode5DTLastItem(currentLottery);
                    if (LatestItem == null)
                        LatestItem = new OpenCode5DTModel
                        {
                            Term = CommonHelper.GenerateQiHaoYYQQQ(0),
                            OpenTime = new DateTime(CommonHelper.SCCSysDateTime.Year, 1, 1)
                        };
                }

                //程序时间第二天，程序根据配置检查是否昨天有开奖
                isGetData = false;
                if (CommonHelper.CheckDTIsNeedGetData(Config)) //
                {
                    DoMainUrl();
                    DoBackUrl();
                }

                if (!LatestItem.Term.ToString().StartsWith(CommonHelper.SCCSysDateTime.ToString("yy")))
                    LatestItem = new OpenCode5DTModel
                    {
                        Term = CommonHelper.GenerateQiHaoYYQQQ(0),
                        OpenTime = new DateTime(CommonHelper.SCCSysDateTime.Year, 1, 1)
                    };
                //当今日开奖并且当前时间是晚上8点过后开始抓取
                if (CommonHelper.CheckTodayIsOpenDay(Config) && CommonHelper.SCCSysDateTime.Hour > 12)
                {
                    DoMainUrl();
                    DoBackUrl();
                }
            }
            catch (Exception ex)
            {
                log.Error(GetType(), string.Format("【{0}】抓取时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            //保存最新期号
            context.JobDetail.JobDataMap["LatestItem"] = LatestItem;
        }

        #region 通过主站爬取数据
        /// <summary>
        /// 根据主站的数据列表插入数据库
        /// </summary>
        private void DoMainUrl()
        {
            if (!string.IsNullOrEmpty(Config.MainUrl))
            {
                var openList = GetOpenListFromMainUrl(Config.MainUrl);
                if (openList.Count == 0) return; //无抓取数据
                //抓取到的最新期数
                var newestQiHao = Convert.ToInt32(openList.Max(w => w.Term).ToString());
                //数据库里面最新期数
                var startQiNum = Convert.ToInt32(LatestItem.Term.ToString());

                if (startQiNum > newestQiHao) return; //无最新数据

                //处理最新开奖数据
                var getQiHao = string.Empty;
                OpenCode5DTModel matchItem = null;
                for (var i = startQiNum; i <= newestQiHao; i++)
                {
                    getQiHao = i.ToString();
                    matchItem = openList.FirstOrDefault(r => r.Term.ToString() == getQiHao);

                    if (matchItem != null && services.AddDTOpen5Code(currentLottery, matchItem))
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
        /// 根据主站的url获取到数据列表
        /// </summary>
        /// <param name="mainUrl"></param>
        /// <returns></returns>
        private List<OpenCode5DTModel> GetOpenListFromMainUrl(string mainUrl)
        {
            var result = new List<OpenCode5DTModel>();
            try
            {
                var htmlResource = NetHelper.GetUrlResponse(mainUrl, Encoding.GetEncoding("utf-8"));
                if (htmlResource == null) return result;

                var doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                var table = doc.DocumentNode.SelectSingleNode("//tbody");
                if (table == null) return result;
                var trs = table.ChildNodes.Where(node => node.Name == "tr").ToList();
                List<HtmlNode> tds = null;
                string term = string.Empty, openCodeString = string.Empty, optimizeUrl = string.Empty;
                OpenCode5DTModel model = null;
                for (var i = 0; i < trs.Count; i++)
                {
                    tds = trs[i].ChildNodes.Where(S => S.Name.ToLower() == "td").ToList();
                    if (tds.Count < 8) continue;

                    model = new OpenCode5DTModel();
                    term = tds[0].InnerText.Trim();
                    if (term.StartsWith((CommonHelper.SCCSysDateTime.Year - 1).ToString())) break;

                    model.Term = Convert.ToInt64(term);
                    openCodeString = tds[1].InnerText.Trim();
                    //3+9+7+4+2
                    model.OpenCode1 = Convert.ToInt32(openCodeString.Substring(0, 1));
                    model.OpenCode2 = Convert.ToInt32(openCodeString.Substring(2, 1));
                    model.OpenCode3 = Convert.ToInt32(openCodeString.Substring(4, 1));
                    model.OpenCode4 = Convert.ToInt32(openCodeString.Substring(6, 1));
                    model.OpenCode5 = Convert.ToInt32(openCodeString.Substring(8, 1));
                    model.OpenTime = Convert.ToDateTime(tds[12].InnerText.Trim());
                    //组装开奖详情
                    var details = GetKaijiangDetails(tds);
                    model.Spare = details;

                    model.DetailUrl = mainUrl;

                    result.Add(model);
                }

                var checkDataHelper = new CheckDataHelper();
                var dbdata = services.GetListS<OpenCode5DTModel>(currentLottery)
                    .ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr());
                checkDataHelper.CheckData(dbdata, result.ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr()),
                    Config.Area, currentLottery);
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
                Gdje = nodes[11].InnerText.Replace(",", ""),
                Trje = nodes[10].InnerText.Replace(",", "")
            };
            //TODO 
            var list1 = new List<Kaijiangitem>();
            //组装详情
            var list = new List<Kaijiangitem>
            {
                new Kaijiangitem
                {
                    Name = "4+1",
                    Total = nodes[2].InnerText.Replace("<br>", "").Replace("\t", "").Replace("元", "")
                        .Split("注".ToCharArray())[0],
                    TotalMoney =
                        nodes[2].InnerText.Replace("<br>", "").Replace("\t", "").Replace("元", "").Replace(",", "")
                            .Split("注".ToCharArray())[1]
                },
                new Kaijiangitem
                {
                    Name = "定位4",
                    Total = nodes[3].InnerText.Replace("<br>", "").Replace("\t", "").Replace("元", "")
                        .Split("注".ToCharArray())[0],
                    TotalMoney =
                        nodes[3].InnerText.Replace("<br>", "").Replace("\t", "").Replace("元", "").Replace(",", "")
                            .Split("注".ToCharArray())[1]
                },
                new Kaijiangitem
                {
                    Name = "定位3",
                    Total = nodes[4].InnerText.Replace("<br>", "").Replace("\t", "").Replace("元", "")
                        .Split("注".ToCharArray())[0],
                    TotalMoney =
                        nodes[4].InnerText.Replace("<br>", "").Replace("\t", "").Replace("元", "").Replace(",", "")
                            .Split("注".ToCharArray())[1]
                },
                new Kaijiangitem
                {
                    Name = "定位2",
                    Total = nodes[5].InnerText.Replace("<br>", "").Replace("\t", "").Replace("元", "")
                        .Split("注".ToCharArray())[0],
                    TotalMoney =
                        nodes[5].InnerText.Replace("<br>", "").Replace("\t", "").Replace("元", "").Replace(",", "")
                            .Split("注".ToCharArray())[1]
                },
                new Kaijiangitem
                {
                    Name = "定位1",
                    Total = nodes[6].InnerText.Replace("<br>", "").Replace("\t", "").Replace("元", "")
                        .Split("注".ToCharArray())[0],
                    TotalMoney =
                        nodes[6].InnerText.Replace("<br>", "").Replace("\t", "").Replace("元", "")
                            .Split("注".ToCharArray())[1]
                },
                new Kaijiangitem
                {
                    Name = "任选4(3重)",
                    Total = nodes[6].InnerText.Replace("<br>", "").Replace("\t", "").Replace("元", "")
                        .Split("注".ToCharArray())[0],
                    TotalMoney =
                        nodes[6].InnerText.Replace("<br>", "").Replace("\t", "").Replace("元", "").Replace(",", "")
                            .Split("注".ToCharArray())[1]
                },
                new Kaijiangitem
                {
                    Name = "任选4(2双重)",
                    Total = nodes[6].InnerText.Replace("<br>", "").Replace("\t", "").Replace("元", "")
                        .Split("注".ToCharArray())[0],
                    TotalMoney =
                        nodes[6].InnerText.Replace("<br>", "").Replace("\t", "").Replace("元", "").Replace(",", "")
                            .Split("注".ToCharArray())[1]
                },
                new Kaijiangitem
                {
                    Name = "任选4(1双重)",
                    Total = nodes[6].InnerText.Replace("<br>", "").Replace("\t", "").Replace("元", "")
                        .Split("注".ToCharArray())[0],
                    TotalMoney =
                        nodes[6].InnerText.Replace("<br>", "").Replace("\t", "").Replace("元", "").Replace(",", "")
                            .Split("注".ToCharArray())[1]
                }
            };
            entity.KaiJiangItems = list;

            return entity.TryToJson();
        }

        #endregion

        #region 通过副站爬取数据

      
        private List<OpenCode5DTModel> GetOpenListFromBackUrl()
        {
            var result = new List<OpenCode5DTModel>();
            try
            {
                var url = new Uri(Config.BackUrl);
                var htmlResource = NetHelper.GetUrlResponse(Config.BackUrl, Encoding.GetEncoding("utf-8"));
                if (htmlResource == null) return result;

                var doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                var table = doc.DocumentNode.SelectNodes("//table[@id ='tabTrend']");
                if (table == null) return result;
                var trs = table[0].ChildNodes.Where(node => node.Name == "tr").ToList();
                OpenCode5DTModel model = null;
                var optimizeUrl = string.Empty;
                for (var i = 1; i < trs.Count; i++) //第一二行为表头
                {
                    var tds = trs[i].ChildNodes.Where(node => node.Name == "td").ToList();
                    if (tds.Count < 10) continue;

                    if (tds[1].ChildNodes.Count == 0) continue;
                    var openCodeString = tds[1].InnerText.Trim();
                    if (openCodeString.Length != 6) continue;
                    model = new OpenCode5DTModel();
                    model.Term = Convert.ToInt64(tds[0].InnerText.Trim());
                    model.OpenCode1 = Convert.ToInt32(openCodeString.Substring(0, 1));
                    model.OpenCode2 = Convert.ToInt32(openCodeString.Substring(1, 1));
                    model.OpenCode3 = Convert.ToInt32(openCodeString.Substring(2, 1));
                    model.OpenCode4 = Convert.ToInt32(openCodeString.Substring(3, 1));
                    model.OpenCode5 = Convert.ToInt32(openCodeString.Substring(5));
                    result.Add(model);
                }

                var checkDataHelper = new CheckDataHelper();
                var dbdata = services.GetListS<OpenCode5DTModel>(currentLottery)
                    .ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr());
                checkDataHelper.CheckData(dbdata, result.ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr()),
                    Config.Area, currentLottery);
                result = result.OrderByDescending(S => S.Term).ToList();
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过备用站点抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            return result;
        }

        private void DoBackUrl()
        {
            if (!string.IsNullOrEmpty(Config.BackUrl))
            {
                var OpenList = GetOpenListFromBackUrl();
                if (OpenList.Count == 0) return; //无抓取数据
                var newestQiHao = OpenList.First().Term.ToString();
                var startQiNum = Convert.ToInt32(LatestItem.Term.ToString().Substring(4)) + 1;
                var newestQiNum = Convert.ToInt32(newestQiHao.Substring(4));
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
                var getQiHao = string.Empty;
                OpenCode5DTModel matchItem = null;
                for (var i = startQiNum; i <= newestQiNum; i++)
                {
                    getQiHao = i.ToString();
                    matchItem = OpenList.Where(R => R.Term.ToString() == getQiHao).FirstOrDefault();
                    if (matchItem != null && services.AddDTOpen5Code(currentLottery, matchItem))
                    {
                        //Do Success Log
                        log.Info(GetType(), CommonHelper.GetJobBackLogInfo(Config, getQiHao));
                        LatestItem = matchItem;
                        isGetData = true;
                    }
                }
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
        private OpenCode5DTModel LatestItem;

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
        private SCCLottery currentLottery => SCCLottery.HaiNan4J1;

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