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
    ///     浙江体彩6+1
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class ZJTC6J1Job : IJob
    {
        /// <summary>
        /// 初始化函数
        /// </summary>
        public ZJTC6J1Job()
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
                            Term = CommonHelper.GenerateQiHaoYYQQQ(0),
                            OpenTime = new DateTime(CommonHelper.SCCSysDateTime.Year, 1, 1)
                        };
                }

                //程序时间第二天，程序根据配置检查是否昨天有开奖
                isGetData = false;
                if (CommonHelper.CheckDTIsNeedGetData(Config)) CheckingOpenDayTheLotteryData();
                if (!LatestItem.Term.ToString().StartsWith(CommonHelper.SCCSysDateTime.ToString("yy")))
                    LatestItem = new OpenCode7DTModel
                    {
                        Term = CommonHelper.GenerateQiHaoYYQQQ(0),
                        OpenTime = new DateTime(CommonHelper.SCCSysDateTime.Year, 1, 1)
                    };
                //当今日开奖并且当前时间是晚上8点过后开始抓取
                if (CommonHelper.CheckTodayIsOpenDay(Config) && CommonHelper.SCCSysDateTime.Hour > 12)
                {
                    DoTodayJobByMainUrl();
                    DoTodayJobByBackUrl();
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
        ///     自检爬取未爬取到的开奖数据，并对昨日开奖但未爬取到开奖数据的彩种添加邮件提醒
        /// </summary>
        private void CheckingOpenDayTheLotteryData()
        {
            //从数据库中获取昨天数据抓取失败列表
            FailedQiHaoList = services.GetFailedYYQQQList(currentLottery);
            if (FailedQiHaoList.Count > 0)
            {
                DoYesterdayFailedListByMainUrl();
                DoYesterdayFailedListByBackUrl();
            }

            if (LatestItem.OpenTime.ToString("yyyyMMdd") !=
                CommonHelper.SCCSysDateTime.AddDays(-1).ToString("yyyyMMdd"))
            {
                //开奖时间(昨天)未抓取到最新开奖数据,则再抓取一次，若还不成功则写入待发送邮件列表
                DoTodayJobByMainUrl();
                DoTodayJobByBackUrl();
                if (LatestItem.OpenTime.ToString("yyyyMMdd") !=
                    CommonHelper.SCCSysDateTime.AddDays(-1).ToString("yyyyMMdd"))
                {
                    var openQiHao = (LatestItem.Term + 1).ToString();
                    if (email.AddEmail(Config.Area + currentLottery, openQiHao,
                        CommonHelper.GenerateDTOpenTime(Config)))
                        log.Error(GetType(), CommonHelper.GetJobLogError(Config, openQiHao));
                }
            }
        }

        /*
         * 2017358 17045697
         * 2017357 17039516
         * 2017356 17033161
         * 2017355 17026630
         * 2017354 17020136
         * 
         * 2017260 16419781
         * 2017259 16415163
         */

        /// <summary>
        ///     通过主站点爬取开奖数据
        ///     (浙江体彩网)
        /// </summary>
        private void DoTodayJobByMainUrl()
        {
            if (!string.IsNullOrEmpty(Config.MainUrl))
            {
                var OpenList = GetOpenListFromMainUrl(Config.MainUrl);
                if (OpenList.Count == 0) return; //无抓取数据
                var newestQiHao = OpenList.Max(w => w.Term).ToString();
                var startQiNum = Convert.ToInt32(LatestItem.Term.ToString().Substring(2)) + 1;
                var newestQiNum = Convert.ToInt32(newestQiHao.Substring(2));
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
                var getQiHao = string.Empty;
                OpenCode7DTModel matchItem = null;
                for (var i = startQiNum; i <= newestQiNum; i++)
                {
                    getQiHao = LatestItem.Term.ToString().Substring(0, 2) + i.ToString().PadLeft(3, '0');
                    matchItem = OpenList.Where(R => R.Term.ToString() == getQiHao).FirstOrDefault();
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
        ///     通过主站爬取错误期号列表中每一个期号
        ///     (浙江体彩网)
        /// </summary>
        private void DoYesterdayFailedListByMainUrl()
        {
            if (!string.IsNullOrEmpty(Config.MainUrl) && FailedQiHaoList.Count > 0)
            {
                var OpenList = GetOpenListFromMainUrl(Config.MainUrl);
                if (OpenList.Count == 0) return; //无抓取数据
                OpenCode7DTModel matchItem = null;
                var SuccessList = new List<string>();
                foreach (var failedQiHao in FailedQiHaoList)
                {
                    matchItem = OpenList.Where(R => R.Term.ToString() == failedQiHao).FirstOrDefault();
                    if (matchItem != null && services.AddDTOpen7Code(currentLottery, matchItem))
                    {
                        //Do Success Log
                        log.Info(GetType(), CommonHelper.GetJobMainLogInfo(Config, failedQiHao));
                        if (matchItem.Term > LatestItem.Term) LatestItem = matchItem;
                        isGetData = true;
                        SuccessList.Add(failedQiHao);
                    }
                }

                foreach (var successQiHao in SuccessList) FailedQiHaoList.Remove(successQiHao);
            }
        }

        /// <summary>
        ///     获取主站开奖列表数据
        /// </summary>
        /// <param name="mainUrl">主站地址</param>
        /// <returns></returns>
        private List<OpenCode7DTModel> GetOpenListFromMainUrl(string mainUrl)
        {
            var result = new List<OpenCode7DTModel>();
            try
            {
                var htmlResource = NetHelper.GetUrlResponse(Config.MainUrl, Encoding.UTF8);
                if (htmlResource == null) return result;

                var doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                var tbody = doc.DocumentNode.SelectSingleNode("//tbody");
                if (tbody == null) return result;
                var trs = tbody.ChildNodes.Where(N => N.Name.ToLower() == "tr").ToList();
                foreach (var item in trs)
                {
                    var tds = item.ChildNodes.Where(w => w.Name == "td").ToList();
                    var qihao = tds[0].InnerText.Trim();
                    var kaijianghao = tds[1].InnerText.Trim().Replace(" ", "");
                    var opentime = tds[10].InnerText.Trim();
                    var tmp = new OpenCode7DTModel
                    {
                        Term = long.Parse(qihao),
                        OpenTime = DateTime.Parse(opentime)
                    };
                    tmp.OpenCode1 = int.Parse(kaijianghao.Substring(0, 1));
                    tmp.OpenCode2 = int.Parse(kaijianghao.Substring(1, 1));
                    tmp.OpenCode3 = int.Parse(kaijianghao.Substring(2, 1));
                    tmp.OpenCode4 = int.Parse(kaijianghao.Substring(3, 1));
                    tmp.OpenCode5 = int.Parse(kaijianghao.Substring(4, 1));
                    tmp.OpenCode6 = int.Parse(kaijianghao.Substring(5, 1));
                    tmp.OpenCode7 = int.Parse(kaijianghao.Substring(6, 1));
                    OptimizeMainModel(ref tmp, item);
                    result.Add(tmp);
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
        ///     完善主站浙江体彩6+1开奖详情信息
        /// </summary>
        /// <param name="model"></param>
        private bool OptimizeMainModel(ref OpenCode7DTModel model, HtmlNode tr)
        {
            try
            {
                var tds = tr.ChildNodes.Where(w => w.Name == "td").ToList();
                var entity = new KaijiangDetailsEntity();
                entity.KaiJiangItems = new List<Kaijiangitem>();
                var xiaoshoue = tds[8].InnerText.Trim();
                var jiangchi = (double.Parse(tds[9].InnerText.Trim()) * 10000).ToString();
                var tedengjiangary = tds[2].InnerText.Trim().Split('注', '元');

                var tedengjiang = new Kaijiangitem
                {
                    Name = "特等奖",
                    Total = tedengjiangary[0].Replace(",", string.Empty),
                    TotalMoney = tedengjiangary[1].Replace(",", string.Empty).Replace("\t", "")
                };
                entity.KaiJiangItems.Add(tedengjiang);
                var yidengjiangary = tds[3].InnerText.Trim().Split('注', '元');
                var yidengjiang = new Kaijiangitem
                {
                    Name = "一等奖",
                    Total = yidengjiangary[0].Replace(",", string.Empty),
                    TotalMoney = yidengjiangary[1].Replace(",", string.Empty).Replace("\t", "")
                };
                entity.KaiJiangItems.Add(yidengjiang);
                var erdengjiangary = tds[4].InnerText.Trim().Split('注', '元');
                var erdengjiang = new Kaijiangitem
                {
                    Name = "二等奖",
                    Total = erdengjiangary[0].Replace(",", string.Empty),
                    TotalMoney = erdengjiangary[1].Replace(",", string.Empty).Replace("t\\", "")
                };
                entity.KaiJiangItems.Add(erdengjiang);
                var sandengjiangary = tds[5].InnerText.Trim().Split('注', '元');
                var sandengjiang = new Kaijiangitem
                {
                    Name = "三等奖",
                    Total = sandengjiangary[0].Replace(",", string.Empty),
                    TotalMoney = sandengjiangary[1].Replace(",", string.Empty).Replace("\t", "")
                };
                entity.KaiJiangItems.Add(sandengjiang);
                var sidengjiangary = tds[6].InnerText.Trim().Split('注', '元');
                var sidengjiang = new Kaijiangitem
                {
                    Name = "四等奖",
                    Total = sidengjiangary[0].Replace(",", string.Empty),
                    TotalMoney = sidengjiangary[1].Replace(",", string.Empty).Replace("\t", "")
                };
                entity.KaiJiangItems.Add(sidengjiang);
                var wudengjiangary = tds[7].InnerText.Trim().Split('注', '元');
                var wudengjiang = new Kaijiangitem
                {
                    Name = "五等奖",
                    Total = wudengjiangary[0].Replace(",", string.Empty),
                    TotalMoney = wudengjiangary[1].Replace(",", string.Empty).Replace("\t", "")
                };
                entity.KaiJiangItems.Add(wudengjiang);
                model.Spare = entity.TryToJson();
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过主站点优化开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            return false;
        }

        /// <summary>
        ///     通过备用站点抓取开奖数据
        ///     (彩票两元网)
        /// </summary>
        private void DoTodayJobByBackUrl()
        {
            if (!string.IsNullOrEmpty(Config.BackUrl))
            {
                var OpenList = GetOpenListFromBackUrl(Config.BackUrl);
                if (OpenList.Count == 0) return; //无抓取数据
                var newestQiHao = OpenList.Max(w => w.Term).ToString();
                var startQiNum = Convert.ToInt32(LatestItem.Term.ToString().Substring(2)) + 1;
                var newestQiNum = Convert.ToInt32(newestQiHao.Substring(2));
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
                var getQiHao = string.Empty;
                OpenCode7DTModel matchItem = null;
                for (var i = startQiNum; i <= newestQiNum; i++)
                {
                    getQiHao = LatestItem.Term.ToString().Substring(0, 2) + i.ToString().PadLeft(3, '0');
                    matchItem = OpenList.Where(R => R.Term.ToString() == getQiHao).FirstOrDefault();
                    if (matchItem != null && OptimizeBackModel(ref matchItem) &&
                        services.AddDTOpen7Code(currentLottery, matchItem))
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
        ///     通过备用地址抓取错误期号列表中每一个期号
        ///     (彩票两元网)
        /// </summary>
        private void DoYesterdayFailedListByBackUrl()
        {
            if (!string.IsNullOrEmpty(Config.BackUrl) && FailedQiHaoList.Count > 0)
            {
                var OpenList = GetOpenListFromBackUrl(Config.BackUrl);
                if (OpenList.Count == 0) return; //无抓取数据
                OpenCode7DTModel matchItem = null;
                var SuccessList = new List<string>();
                foreach (var failedQiHao in FailedQiHaoList)
                {
                    matchItem = OpenList.Where(R => R.Term.ToString() == failedQiHao).FirstOrDefault();
                    if (matchItem != null && OptimizeBackModel(ref matchItem) &&
                        services.AddDTOpen7Code(currentLottery, matchItem))
                    {
                        //Do Success Log
                        log.Info(GetType(), CommonHelper.GetJobBackLogInfo(Config, failedQiHao));
                        if (matchItem.Term > LatestItem.Term) LatestItem = matchItem;
                        SuccessList.Add(failedQiHao);
                        isGetData = true;
                    }
                }

                foreach (var successQiHao in SuccessList) FailedQiHaoList.Remove(successQiHao);
            }
        }

        /// <summary>
        ///     获取备用站点开奖列表数据
        /// </summary>
        /// <param name="backUrl">备用站点</param>
        /// <returns></returns>
        private List<OpenCode7DTModel> GetOpenListFromBackUrl(string backUrl)
        {
            var result = new List<OpenCode7DTModel>();
            try
            {
                var htmlResource = NetHelper.GetUrlResponse(backUrl);
                if (htmlResource == null) return result;

                var doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                var table = doc.DocumentNode.SelectSingleNode("//table");
                if (table == null) return result;
                var trs = table.ChildNodes.Where(node => node.Name.ToLower() == "tr").ToList();
                OpenCode7DTModel model = null;
                var lastYear = (CommonHelper.SCCSysDateTime.Year - 1).ToString().Substring(2);
                for (var i = 0; i < trs.Count; i++) //第一行
                {
                    var trstyle = trs[i].Attributes["style"];
                    if (trstyle != null && trstyle.Value == "display:none") continue;
                    var tds = trs[i].ChildNodes.Where(node => node.Name.ToLower() == "td").ToList();
                    if (tds.Count < 11) continue;
                    if (tds[0].InnerText.Trim().StartsWith(lastYear)) break;
                    model = new OpenCode7DTModel();
                    model.OpenTime = Convert.ToDateTime(tds[9].InnerText.Trim());
                    model.Term = Convert.ToInt64(tds[0].InnerText.Trim());
                    var opencodeNode = tds[1].ChildNodes.Where(n => n.Name.ToLower() == "i").ToList();
                    if (opencodeNode.Count < 7) continue;
                    model.OpenCode1 = Convert.ToInt32(opencodeNode[0].InnerText.Trim());
                    model.OpenCode2 = Convert.ToInt32(opencodeNode[1].InnerText.Trim());
                    model.OpenCode3 = Convert.ToInt32(opencodeNode[2].InnerText.Trim());
                    model.OpenCode4 = Convert.ToInt32(opencodeNode[3].InnerText.Trim());
                    model.OpenCode5 = Convert.ToInt32(opencodeNode[4].InnerText.Trim());
                    model.OpenCode6 = Convert.ToInt32(opencodeNode[5].InnerText.Trim());
                    model.OpenCode7 = Convert.ToInt32(opencodeNode[6].InnerText.Trim());
                    model.DetailUrl = backUrl + model.Term + "/";
                    OptimizeBackModel(ref model);
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
                    string.Format("【{0}】通过备用站点抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            return result;
        }

        /// <summary>
        ///     完善备用站点浙江体彩6+1开奖实体信息
        /// </summary>
        /// <param name="model"></param>
        private bool OptimizeBackModel(ref OpenCode7DTModel model)
        {
            try
            {
                var entity = new KaijiangDetailsEntity();
                entity.KaiJiangItems = new List<Kaijiangitem>();
                var htmlResource = NetHelper.GetUrlResponse(model.DetailUrl);
                if (!string.IsNullOrEmpty(htmlResource))
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(htmlResource);
                    var tables = doc.DocumentNode.SelectNodes("//table");
                    if (tables.Count < 1) return false;
                    var trs = tables[0].ChildNodes.Where(N => N.Name.ToLower() == "tr").ToList();
                    for (var i = 1; i < trs.Count; i++) //第一行为表头
                    {
                        var tds = trs[i].ChildNodes.Where(N => N.Name.ToLower() == "td").ToList();
                        if (tds.Count < 4) continue;
                        if (tds[1].InnerText == "特等奖")
                        {
                            var tmp = new Kaijiangitem
                            {
                                Name = "特等奖",
                                Total = tds[2].InnerText.Replace("注", string.Empty).Replace(",", string.Empty).Trim(),
                                TotalMoney =
                                    tds[3].InnerText.Replace("元", string.Empty).Trim() == "--"
                                        ? "0"
                                        : tds[3].InnerText.Replace("元", string.Empty).Trim()
                            };
                            entity.KaiJiangItems.Add(tmp);
                            //Level1Num = Convert.ToInt32(tds[2].InnerText.Replace("注", string.Empty).Replace(",", string.Empty).Trim());
                            // Level1Money = Convert.ToDecimal(tds[3].InnerText.Replace("元", string.Empty).Trim() == "--" ? "0" : tds[3].InnerText.Replace("元", string.Empty).Trim());
                        }
                        else if (tds[1].InnerText == "一等奖")
                        {
                            var tmp = new Kaijiangitem
                            {
                                Name = "一等奖",
                                Total = tds[2].InnerText.Replace("注", string.Empty).Replace(",", string.Empty).Trim(),
                                TotalMoney =
                                    tds[3].InnerText.Replace("元", string.Empty).Trim() == "--"
                                        ? "0"
                                        : tds[3].InnerText.Replace("元", string.Empty).Trim()
                            };
                            entity.KaiJiangItems.Add(tmp);
                        }
                        else if (tds[1].InnerText == "二等奖")
                        {
                            var tmp = new Kaijiangitem
                            {
                                Name = "二等奖",
                                Total = tds[2].InnerText.Replace("注", string.Empty).Replace(",", string.Empty).Trim(),
                                TotalMoney =
                                    tds[3].InnerText.Replace("元", string.Empty).Trim() == "--"
                                        ? "0"
                                        : tds[3].InnerText.Replace("元", string.Empty).Trim()
                            };
                            entity.KaiJiangItems.Add(tmp);
                        }
                        else if (tds[1].InnerText == "三等奖")
                        {
                            var tmp = new Kaijiangitem
                            {
                                Name = "三等奖",
                                Total = tds[2].InnerText.Replace("注", string.Empty).Replace(",", string.Empty).Trim(),
                                TotalMoney =
                                    tds[3].InnerText.Replace("元", string.Empty).Trim() == "--"
                                        ? "0"
                                        : tds[3].InnerText.Replace("元", string.Empty).Trim()
                            };
                            entity.KaiJiangItems.Add(tmp);
                        }
                        else if (tds[1].InnerText == "四等奖")
                        {
                            var tmp = new Kaijiangitem
                            {
                                Name = "四等奖",
                                Total = tds[2].InnerText.Replace("注", string.Empty).Replace(",", string.Empty).Trim(),
                                TotalMoney =
                                    tds[3].InnerText.Replace("元", string.Empty).Trim() == "--"
                                        ? "0"
                                        : tds[3].InnerText.Replace("元", string.Empty).Trim()
                            };
                            entity.KaiJiangItems.Add(tmp);
                        }
                        else if (tds[1].InnerText == "五等奖")
                        {
                            var tmp = new Kaijiangitem
                            {
                                Name = "五等奖",
                                Total = tds[2].InnerText.Replace("注", string.Empty).Replace(",", string.Empty).Trim(),
                                TotalMoney =
                                    tds[3].InnerText.Replace("元", string.Empty).Trim() == "--"
                                        ? "0"
                                        : tds[3].InnerText.Replace("元", string.Empty).Trim()
                            };
                            entity.KaiJiangItems.Add(tmp);
                        }

                        var reg1 = new Regex(@"投注总额：([\d,.]*?) 元");
                        var match1 = reg1.Match(htmlResource);
                        if (match1.Success)
                        {
                            var sales = match1.Result("$1");
                            // Sales = Convert.ToDecimal(sales);
                            entity.Trje = sales;
                        }

                        var reg2 = new Regex(@"奖池资金累计金额：([\d,.]*?) 元");
                        var match2 = reg2.Match(htmlResource);
                        if (match2.Success)
                        {
                            var jackpot = match2.Result("$1");
                            //Jackpot = Convert.ToDecimal(jackpot);
                            entity.Gdje = jackpot;
                        }

                        // string.Format("{0},{1}^特等奖|{2}|{3},一等奖|{4}|{5},二等奖|{6}|{7},三等奖|{8}|{9},四等奖|{10}|{11},五等奖|{12}|{13}",
                        // Sales, Jackpot, Level1Num, Level1Money, Level2Num, Level2Money, Level3Num, Level3Money,
                        // Level4Num, Level4Money, Level5Num, Level5Money, Level6Num, Level6Money);
                    }

                    model.Spare = entity.TryToJson();
                }

                return true;
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过主站点优化开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
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
        private OpenCode7DTModel LatestItem;

        /// <summary>
        ///     当天抓取失败列表
        /// </summary>
        private List<string> FailedQiHaoList;

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
        private SCCLottery currentLottery => SCCLottery.ZheJiang6J1;

        /// <summary>
        ///     邮件接口
        /// </summary>
        private readonly IEmail email;

        /// <summary>
        ///     是否本次运行抓取到开奖数据
        /// </summary>
        private bool isGetData;

        #endregion
    }
}