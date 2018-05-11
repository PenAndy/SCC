using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Quartz;
using SCC.Common;
using SCC.Crawler.Tools;
using SCC.Interface;
using SCC.Models;

namespace SCC.Crawler.DT
{
    /// <summary>
    ///     福建31选7
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class FJ31X7Job : IJob
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        public FJ31X7Job()
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
            LatestItem = context.JobDetail.JobDataMap["LatestItem"] as OpenCode8DTModel;
            try
            {
                //服务启动时配置初始数据
                if (LatestItem == null)
                    LatestItem = services.GetOpenCode8DTLastItem(currentLottery) ?? new OpenCode8DTModel
                    {
                        Term = CommonHelper.GenerateQiHaoYYQQQ(0),
                        OpenTime = new DateTime(CommonHelper.SCCSysDateTime.Year, 1, 1)
                    };
                //程序时间第二天，程序根据配置检查是否昨天有开奖
                isGetData = false;
                if (CommonHelper.CheckDTIsNeedGetData(Config))
                {
                    DoMainUrl();
                    DoBackUrl();
                }

                if (!LatestItem.Term.ToString().StartsWith(CommonHelper.SCCSysDateTime.ToString("yy")))
                    LatestItem = new OpenCode8DTModel
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

        /// <summary>
        ///     通过主站点爬取开奖数据
        ///     (福建体彩网)
        /// </summary>
        private void DoMainUrl()
        {
            if (!string.IsNullOrEmpty(Config.MainUrl))
            {
                var OpenList = GetOpenListFromMainUrl();
                if (OpenList.Count == 0) return; //无抓取数据
                var newestQiHao = OpenList.Max(w => w.Term).ToString();
                var startQiNum = Convert.ToInt32(LatestItem.Term.ToString().Substring(2)) + 1;
                var newestQiNum = Convert.ToInt32(newestQiHao.Substring(2));
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
                var getQiHao = string.Empty;
                OpenCode8DTModel matchItem = null;
                for (var i = startQiNum; i <= newestQiNum; i++)
                {
                    getQiHao = LatestItem.Term.ToString().Substring(0, 2) + i.ToString().PadLeft(3, '0');
                    matchItem = OpenList.Where(R => R.Term.ToString() == getQiHao).FirstOrDefault();
                    if (matchItem != null && services.AddDTOpen8Code(currentLottery, matchItem))
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
        ///     获取主站开奖列表数据
        /// </summary>
        /// <returns></returns>
        private List<OpenCode8DTModel> GetOpenListFromMainUrl()
        {
            var result = new List<OpenCode8DTModel>();
            try
            {
                var htmlResource =
                    NetHelper.GetUrlResponse(Config.MainUrl +
                                             string.Format("&r={0}", new Random().Next(1000, 9999))); //只取第一页数据，最新5条记录
                if (htmlResource == null) return result;

                var doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                var divSections = doc.DocumentNode.SelectNodes("//div[@class='artCon KJDetail']").ToList();
                if (divSections == null) return result;

                OpenCode8DTModel model = null;
                var lastYear = (CommonHelper.SCCSysDateTime.Year - 1).ToString().Substring(2);
                HtmlNode divSummary, divDetail, table, tbody;
                List<HtmlNode> divContent, divInfos, spans, trs, tds, divDetails, divTexts;
                string term = string.Empty,
                    detailName = string.Empty,
                    detailNum = string.Empty,
                    detailMoney = string.Empty;
                string[] openCode = null;
                Regex reg = null;
                Match m = null;
                foreach (var divSection in divSections)
                {
                    var entity = new KaijiangDetailsEntity();
                    entity.KaiJiangItems = new List<Kaijiangitem>();
                    model = new OpenCode8DTModel();
                    divInfos = divSection.ChildNodes.Where(N => N.Name.ToLower() == "div").ToList();
                    if (divInfos.Count < 2) continue;
                    divContent = divInfos[1].ChildNodes.Where(N => N.Name.ToLower() == "div").ToList();
                    if (divContent.Count < 2) continue;
                    divSummary = divContent[0]; //概述
                    divDetail = divContent[1]; //详情
                    spans = divSummary.ChildNodes.First(N => N.Name.ToLower() == "strong").ChildNodes
                        .Where(N => N.Name.ToLower() == "span").ToList();
                    term = spans[0].InnerText;
                    if (term.StartsWith(lastYear)) break;
                    model.Term = Convert.ToInt32(term);
                    model.OpenTime = Convert.ToDateTime(spans[2].InnerText.Replace("年", "-").Replace("月", "-")
                        .Replace("日", string.Empty));
                    openCode = (spans[3].InnerText + " " + spans[4].InnerText).Trim().Split(' ');
                    model.OpenCode1 = Convert.ToInt32(openCode[0]);
                    model.OpenCode2 = Convert.ToInt32(openCode[1]);
                    model.OpenCode3 = Convert.ToInt32(openCode[2]);
                    model.OpenCode4 = Convert.ToInt32(openCode[3]);
                    model.OpenCode5 = Convert.ToInt32(openCode[4]);
                    model.OpenCode6 = Convert.ToInt32(openCode[5]);
                    model.OpenCode7 = Convert.ToInt32(openCode[6]);
                    model.OpenCode8 = Convert.ToInt32(openCode[7]);

                    table = divDetail.ChildNodes.FirstOrDefault(N => N.Name.ToLower() == "table");
                    if (table == null)
                    {
                        divDetails = divDetail.ChildNodes.Where(N => N.Name.ToLower() == "div").ToList();
                        if (divDetails.Count < 14) continue;
                        for (var i = 4; i < 12; i++)
                        {
                            divTexts = divDetails[i].ChildNodes.Where(N => N.Name.ToLower() == "#text").ToList();
                            if (divTexts.Count < 4) continue;
                            detailName = divTexts[0].InnerText.Replace("&nbsp;", string.Empty).Trim();
                            detailNum = divTexts[1].InnerText.Replace("&nbsp;", string.Empty).Replace("注", string.Empty)
                                .Trim();
                            detailMoney = divTexts[2].InnerText.Replace("&nbsp;", string.Empty)
                                .Replace(",", string.Empty).Replace("元", string.Empty).Trim();
                            if (detailName == "一等奖")
                            {
                                var tmp = new Kaijiangitem
                                {
                                    Name = "一等奖",
                                    Total = detailNum,
                                    TotalMoney = detailMoney
                                };
                                entity.KaiJiangItems.Add(tmp);
                            }
                            else if (detailName == "二等奖")
                            {
                                var tmp = new Kaijiangitem
                                {
                                    Name = "二等奖",
                                    Total = detailNum,
                                    TotalMoney = detailMoney
                                };
                                entity.KaiJiangItems.Add(tmp);
                                // Level2Num = Convert.ToInt32(detailNum);
                                // Level2Money = Convert.ToDecimal(detailMoney);
                            }
                            else if (detailName == "三等奖")
                            {
                                var tmp = new Kaijiangitem
                                {
                                    Name = "三等奖",
                                    Total = detailNum,
                                    TotalMoney = detailMoney
                                };
                                entity.KaiJiangItems.Add(tmp);
                            }
                            else if (detailName == "四等奖")
                            {
                                var tmp = new Kaijiangitem
                                {
                                    Name = "四等奖",
                                    Total = detailNum,
                                    TotalMoney = detailMoney
                                };
                                entity.KaiJiangItems.Add(tmp);
                            }
                            else if (detailName == "五等奖")
                            {
                                var tmp = new Kaijiangitem
                                {
                                    Name = "五等奖",
                                    Total = detailNum,
                                    TotalMoney = detailMoney
                                };
                                entity.KaiJiangItems.Add(tmp);
                            }
                            else if (detailName == "六等奖")
                            {
                                var tmp = new Kaijiangitem
                                {
                                    Name = "六等奖",
                                    Total = detailNum,
                                    TotalMoney = detailMoney
                                };
                                entity.KaiJiangItems.Add(tmp);
                            }
                        }
                    }
                    else
                    {
                        tbody = table.ChildNodes.FirstOrDefault(N => N.Name.ToLower() == "tbody");
                        if (tbody == null) continue;
                        trs = tbody.ChildNodes.Where(N => N.Name.ToLower() == "tr").ToList();
                        for (var i = 4; i < trs.Count; i++) //第一二三四行是表头
                        {
                            tds = trs[i].ChildNodes.Where(N => N.Name.ToLower() == "td").ToList();
                            if (tds.Count < 3) continue;
                            if (tds[0].InnerText.Trim() == "一等奖")
                            {
                                var tmp = new Kaijiangitem
                                {
                                    Name = "一等奖",
                                    Total = tds[1].InnerText.Replace("注", string.Empty).Replace("&nbsp;", string.Empty)
                                        .Replace(",", string.Empty).Trim(),
                                    TotalMoney =
                                        tds[2].InnerText.Replace("元", string.Empty).Replace("&nbsp;", string.Empty)
                                            .Trim()
                                };
                                entity.KaiJiangItems.Add(tmp);
                                //Level1Num = Convert.ToInt32(tds[1].InnerText.Replace("注", string.Empty).Replace("&nbsp;", string.Empty).Replace(",", string.Empty).Trim());
                                //Level1Money = Convert.ToDecimal(tds[2].InnerText.Replace("元", string.Empty).Replace("&nbsp;", string.Empty).Trim());
                            }
                            else if (tds[0].InnerText.Trim() == "二等奖")
                            {
                                var tmp = new Kaijiangitem
                                {
                                    Name = "二等奖",
                                    Total = tds[1].InnerText.Replace("注", string.Empty).Replace("&nbsp;", string.Empty)
                                        .Replace(",", string.Empty).Trim(),
                                    TotalMoney =
                                        tds[2].InnerText.Replace("元", string.Empty).Replace("&nbsp;", string.Empty)
                                            .Trim()
                                };
                                entity.KaiJiangItems.Add(tmp);
                                // Level2Num = Convert.ToInt32(tds[1].InnerText.Replace("注", string.Empty).Replace("&nbsp;", string.Empty).Replace(",", string.Empty).Trim());
                                //Level2Money = Convert.ToDecimal(tds[2].InnerText.Replace("元", string.Empty).Replace("&nbsp;", string.Empty).Trim());
                            }
                            else if (tds[0].InnerText.Trim() == "三等奖")
                            {
                                var tmp = new Kaijiangitem
                                {
                                    Name = "三等奖",
                                    Total = tds[1].InnerText.Replace("注", string.Empty).Replace("&nbsp;", string.Empty)
                                        .Replace(",", string.Empty).Trim(),
                                    TotalMoney =
                                        tds[2].InnerText.Replace("元", string.Empty).Replace("&nbsp;", string.Empty)
                                            .Trim()
                                };
                                entity.KaiJiangItems.Add(tmp);
                                // Level3Num = Convert.ToInt32(tds[1].InnerText.Replace("注", string.Empty).Replace("&nbsp;", string.Empty).Replace(",", string.Empty).Trim());
                                // Level3Money = Convert.ToDecimal(tds[2].InnerText.Replace("元", string.Empty).Replace("&nbsp;", string.Empty).Trim());
                            }
                            else if (tds[0].InnerText.Trim() == "四等奖")
                            {
                                var tmp = new Kaijiangitem
                                {
                                    Name = "四等奖",
                                    Total = tds[1].InnerText.Replace("注", string.Empty).Replace("&nbsp;", string.Empty)
                                        .Replace(",", string.Empty).Trim(),
                                    TotalMoney =
                                        tds[2].InnerText.Replace("元", string.Empty).Replace("&nbsp;", string.Empty)
                                            .Trim()
                                };
                                entity.KaiJiangItems.Add(tmp);
                            }
                            else if (tds[0].InnerText.Trim() == "五等奖")
                            {
                                var tmp = new Kaijiangitem
                                {
                                    Name = "五等奖",
                                    Total = tds[1].InnerText.Replace("注", string.Empty).Replace("&nbsp;", string.Empty)
                                        .Replace(",", string.Empty).Trim(),
                                    TotalMoney =
                                        tds[2].InnerText.Replace("元", string.Empty).Replace("&nbsp;", string.Empty)
                                            .Trim()
                                };
                                entity.KaiJiangItems.Add(tmp);
                            }
                            else if (tds[0].InnerText.Trim() == "六等奖")
                            {
                                var tmp = new Kaijiangitem
                                {
                                    Name = "六等奖",
                                    Total = tds[1].InnerText.Replace("注", string.Empty).Replace("&nbsp;", string.Empty)
                                        .Replace(",", string.Empty).Trim(),
                                    TotalMoney =
                                        tds[2].InnerText.Replace("元", string.Empty).Replace("&nbsp;", string.Empty)
                                            .Trim()
                                };
                                entity.KaiJiangItems.Add(tmp);
                            }
                        }
                    }

                    //抓取奖池累计数据
                    reg = new Regex(@"([\d,.]*?)元奖金滚入下期奖池");
                    // Sales = Convert.ToDecimal(spans[1].InnerText);
                    entity.Trje = spans[1].InnerText;
                    m = reg.Match(divSection.InnerHtml);
                    if (m.Success)
                        // Jackpot = Convert.ToDecimal(m.Result("$1"));
                        entity.Gdje = m.Result("$1");
                    model.Spare = entity.TryToJson();
                    //model.Spare = string.Format("{0},{1}^一等奖|{2}|{3},二等奖|{4}|{5},三等奖|{6}|{7},四等奖|{8}|{9},五等奖|{10}|{11},六等奖|{12}|{13}",
                    //    Sales, Jackpot, Level1Num, Level1Money, Level2Num, Level2Money, Level3Num, Level3Money, Level4Num,
                    //    Level4Money, Level5Num, Level5Money, Level6Num, Level6Money);
                    result.Add(model);
                }

                var checkDataHelper = new CheckDataHelper();
                var dbdata = services.GetListS<OpenCode8DTModel>(currentLottery)
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
        ///     通过备用站点抓取开奖数据
        ///     (彩票两元网)
        /// </summary>
        private void DoBackUrl()
        {
            if (!string.IsNullOrEmpty(Config.BackUrl))
            {
                var OpenList = GetOpenListFromBackUrl();
                if (OpenList.Count == 0) return; //无抓取数据
                var newestQiHao = OpenList.First().Term.ToString();
                var startQiNum = Convert.ToInt32(LatestItem.Term.ToString().Substring(2)) + 1;
                var newestQiNum = Convert.ToInt32(newestQiHao.Substring(2));
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
                var getQiHao = string.Empty;
                OpenCode8DTModel matchItem = null;
                for (var i = startQiNum; i <= newestQiNum; i++)
                {
                    getQiHao = LatestItem.Term.ToString().Substring(0, 2) + i.ToString().PadLeft(3, '0');
                    matchItem = OpenList.Where(R => R.Term.ToString() == getQiHao).FirstOrDefault();
                    if (matchItem != null && OptimizeBackModel(ref matchItem) &&
                        services.AddDTOpen8Code(currentLottery, matchItem))
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
        ///     获取备用站点开奖列表数据
        /// </summary>
        /// <returns></returns>
        private List<OpenCode8DTModel> GetOpenListFromBackUrl()
        {
            var result = new List<OpenCode8DTModel>();
            try
            {
                var url = new Uri(Config.BackUrl);
                var htmlResource = NetHelper.GetUrlResponse(Config.BackUrl, Encoding.GetEncoding("gb2312"));
                if (htmlResource == null) return result;

                var doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                var table = doc.DocumentNode.SelectSingleNode("//table");
                if (table == null) return result;
                var trs = table.ChildNodes.Where(node => node.Name == "tr").ToList();
                OpenCode8DTModel model = null;
                HtmlNode nodeA = null;
                string optimizeUrl = string.Empty, term = string.Empty;
                for (var i = 2; i < trs.Count; i++) //第一二行为表头
                {
                    var trstyle = trs[i].Attributes["style"];
                    if (trstyle != null && trstyle.Value == "display:none") continue;
                    var tds = trs[i].ChildNodes.Where(node => node.Name == "td").ToList();
                    if (tds.Count < 10) continue;
                    model = new OpenCode8DTModel();
                    nodeA = tds[0].ChildNodes.Where(n => n.Name == "a").FirstOrDefault();
                    if (nodeA == null) continue;
                    term = nodeA.InnerText.Trim().Substring(2);
                    if (!term.StartsWith(CommonHelper.SCCSysDateTime.ToString("yy"))) break;
                    model.Term = Convert.ToInt64(term);
                    optimizeUrl = nodeA.Attributes["href"].Value;
                    model.DetailUrl = new Uri(url, optimizeUrl).AbsoluteUri;
                    model.OpenTime = Convert.ToDateTime(tds[9].InnerText);
                    if (tds[1].ChildNodes.Count == 0) continue;
                    var opencodeNode = tds[1].ChildNodes.Where(n => n.Name.ToLower() == "i").ToList();
                    if (opencodeNode.Count < 8) continue;
                    model.OpenCode1 = Convert.ToInt32(opencodeNode[0].InnerText.Trim());
                    model.OpenCode2 = Convert.ToInt32(opencodeNode[1].InnerText.Trim());
                    model.OpenCode3 = Convert.ToInt32(opencodeNode[2].InnerText.Trim());
                    model.OpenCode4 = Convert.ToInt32(opencodeNode[3].InnerText.Trim());
                    model.OpenCode5 = Convert.ToInt32(opencodeNode[4].InnerText.Trim());
                    model.OpenCode6 = Convert.ToInt32(opencodeNode[5].InnerText.Trim());
                    model.OpenCode7 = Convert.ToInt32(opencodeNode[6].InnerText.Trim());
                    model.OpenCode8 = Convert.ToInt32(opencodeNode[7].InnerText.Trim());
                    result.Add(model);
                }

                var checkDataHelper = new CheckDataHelper();
                var dbdata = services.GetListS<OpenCode8DTModel>(currentLottery)
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

        /// <summary>
        ///     完善备用站点福建31选7开奖实体信息
        /// </summary>
        /// <param name="model"></param>
        private bool OptimizeBackModel(ref OpenCode8DTModel model)
        {
            try
            {
                var htmlResource = NetHelper.GetUrlResponse(model.DetailUrl, Encoding.GetEncoding("gb2312"));
                if (string.IsNullOrWhiteSpace(htmlResource)) return false;
                var doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                var table = doc.DocumentNode.SelectSingleNode("//table");
                if (table == null) return false;
                var trs = table.ChildNodes.Where(N => N.Name.ToLower() == "tr").ToList();
                List<HtmlNode> tds = null;
                var entity = new KaijiangDetailsEntity();
                entity.KaiJiangItems = new List<Kaijiangitem>();
                for (var i = 1; i < trs.Count; i++) //第一行为表头
                {
                    tds = trs[i].ChildNodes.Where(N => N.Name.ToLower() == "td").ToList();
                    if (tds.Count < 5) continue;
                    if (tds[1].InnerText == "一等奖")
                    {
                        var tmp = new Kaijiangitem
                        {
                            Name = "一等奖",
                            Total = tds[2].InnerText.Replace("注", string.Empty),
                            TotalMoney = tds[3].InnerText
                        };
                        entity.KaiJiangItems.Add(tmp);
                        // Level1Num = Convert.ToInt32(tds[2].InnerText.Replace("注", string.Empty));
                        // Level1Money = Convert.ToDecimal(tds[3].InnerText);
                    }

                    if (tds[1].InnerText == "二等奖")
                    {
                        var tmp = new Kaijiangitem
                        {
                            Name = "二等奖",
                            Total = tds[2].InnerText.Replace("注", string.Empty),
                            TotalMoney = tds[3].InnerText
                        };
                        entity.KaiJiangItems.Add(tmp);
                    }

                    if (tds[1].InnerText == "三等奖")
                    {
                        var tmp = new Kaijiangitem
                        {
                            Name = "三等奖",
                            Total = tds[2].InnerText.Replace("注", string.Empty),
                            TotalMoney = tds[3].InnerText
                        };
                        entity.KaiJiangItems.Add(tmp);
                    }

                    if (tds[1].InnerText == "四等奖")
                    {
                        var tmp = new Kaijiangitem
                        {
                            Name = "四等奖",
                            Total = tds[2].InnerText.Replace("注", string.Empty),
                            TotalMoney = tds[3].InnerText
                        };
                        entity.KaiJiangItems.Add(tmp);
                    }

                    if (tds[1].InnerText == "五等奖")
                    {
                        var tmp = new Kaijiangitem
                        {
                            Name = "五等奖",
                            Total = tds[2].InnerText.Replace("注", string.Empty),
                            TotalMoney = tds[3].InnerText
                        };
                        entity.KaiJiangItems.Add(tmp);
                    }

                    if (tds[1].InnerText == "六等奖")
                    {
                        var tmp = new Kaijiangitem
                        {
                            Name = "六等奖",
                            Total = tds[2].InnerText.Replace("注", string.Empty),
                            TotalMoney = tds[3].InnerText
                        };
                        entity.KaiJiangItems.Add(tmp);
                    }
                }

                var reg = new Regex(@"本期投注总额：([\d.,]*?)元");
                var match = reg.Match(htmlResource);
                if (match.Success) entity.Trje = match.Result("$1").Replace(",", string.Empty);
                reg = new Regex(@"奖池资金累计金额：([\d.,]*?)元");
                match = reg.Match(htmlResource);
                if (match.Success) entity.Gdje = match.Result("$1").Replace(",", string.Empty);
                model.Spare = model.TryToJson();
                //model.Spare = string.Format("{0},{1}^一等奖|{2}|{3},二等奖|{4}|{5},三等奖|{6}|{7},四等奖|{8}|{9},五等奖|{10}|{11},六等奖|{12}|{13}",
                //    Sales, Jackpot, Level1Num, Level1Money, Level2Num, Level2Money, Level3Num, Level3Money, Level4Num,
                //    Level4Money, Level5Num, Level5Money, Level6Num, Level6Money);
                return true;
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过备用站点优化开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            return false;
        }

        #region Attribute

        /// <summary>
        ///     配置信息
        /// </summary>
        private SCCConfig Config;

        /// <summary>
        ///     当天抓取的最新一期开奖记录
        /// </summary>
        private OpenCode8DTModel LatestItem;

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
        private SCCLottery currentLottery => SCCLottery.FuJian31x7;

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