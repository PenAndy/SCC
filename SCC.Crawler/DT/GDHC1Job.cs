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
    ///     广东好彩1
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class GDHC1Job : IJob
    {
        private static readonly Dictionary<string, int> AnimalDict = new Dictionary<string, int>
        {
            {"鼠", 1},
            {"牛", 2},
            {"虎", 3},
            {"兔", 4},
            {"龙", 5},
            {"蛇", 6},
            {"马", 7},
            {"羊", 8},
            {"猴", 9},
            {"鸡", 10},
            {"狗", 11},
            {"猪", 12}
        };

        private static readonly Dictionary<string, int> SeasonDict = new Dictionary<string, int>
        {
            {"春", 1},
            {"夏", 2},
            {"秋", 3},
            {"冬", 4}
        };

        private static readonly Dictionary<string, int> DirectionDict = new Dictionary<string, int>
        {
            {"东", 1},
            {"西", 3},
            {"南", 2},
            {"北", 4}
        };

        /// <summary>
        ///     构造函数
        /// </summary>
        public GDHC1Job()
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
            LatestItem = context.JobDetail.JobDataMap["LatestItem"] as OpenCode4DTModel;
            try
            {
                //服务启动时配置初始数据
                if (LatestItem == null)
                {
                    LatestItem = services.GetOpenCode4DTLastItem(currentLottery);
                    if (LatestItem == null)
                        LatestItem = new OpenCode4DTModel
                        {
                            Term = CommonHelper.GenerateQiHaoYYYYQQQ(0),
                            OpenTime = new DateTime(CommonHelper.SCCSysDateTime.Year, 1, 1)
                        };
                }

                //程序时间第二天，程序根据配置检查是否昨天有开奖
                isGetData = false;
                if (CommonHelper.CheckDTIsNeedGetData(Config))
                {
                    DoMainUrl();
                    DoBackUrl();
                }

                if (!LatestItem.Term.ToString().StartsWith(CommonHelper.SCCSysDateTime.ToString("yyyy")))
                    LatestItem = new OpenCode4DTModel
                    {
                        Term = CommonHelper.GenerateQiHaoYYYYQQQ(0),
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

        private void DoMainUrl()
        {
            if (!string.IsNullOrEmpty(Config.MainUrl))
            {
                var OpenList = GetOpenListFromMainUrl(Config.MainUrl);
                if (OpenList.Count == 0) return; //无抓取数据
                var newestQiHao = OpenList.Max(w => w.Term).ToString();
                var startQiNum = Convert.ToInt32(LatestItem.Term.ToString().Substring(4)) + 1;
                var newestQiNum = Convert.ToInt32(newestQiHao.Substring(4));
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
                var getQiHao = string.Empty;
                OpenCode4DTModel matchItem = null;
                for (var i = startQiNum; i <= newestQiNum; i++)
                {
                    getQiHao = LatestItem.Term.ToString().Substring(0, 4) + i.ToString().PadLeft(3, '0');
                    matchItem = OpenList.Where(R => R.Term.ToString() == getQiHao).FirstOrDefault();
                    if (matchItem != null && services.AddDTOpen4Code(currentLottery, matchItem))
                    {
                        //Do Success Log
                        log.Info(GetType(), CommonHelper.GetJobMainLogInfo(Config, getQiHao));
                        LatestItem = matchItem;
                        isGetData = true;
                    }
                }
            }
        }

        private List<OpenCode4DTModel> GetOpenListFromMainUrl(string mainUrl)
        {
            var result = new List<OpenCode4DTModel>();
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
                OpenCode4DTModel model = null;
                HtmlNode nodeA = null;
                var optimizeUrl = string.Empty;
                for (var i = 2; i < trs.Count; i++) //第一二行为表头
                {
                    var trstyle = trs[i].Attributes["style"];
                    if (trstyle != null && trstyle.Value == "display:none") continue;
                    var tds = trs[i].ChildNodes.Where(node => node.Name == "td").ToList();
                    if (tds.Count < 9) continue;
                    model = new OpenCode4DTModel();
                    nodeA = tds[0].ChildNodes.Where(n => n.Name == "a").FirstOrDefault();
                    if (nodeA == null) continue;
                    model.Term = Convert.ToInt64(nodeA.InnerText.Trim());
                    optimizeUrl = nodeA.Attributes["href"].Value;
                    model.DetailUrl = new Uri(url, optimizeUrl).AbsoluteUri;
                    model.OpenTime = Convert.ToDateTime(tds[9].InnerText);
                    if (tds[1].ChildNodes.Count == 0) continue;
                    var opencodeNode = tds[1].ChildNodes.Where(n => n.Name.ToLower() == "i").ToList();
                    if (opencodeNode.Count < 4) continue;
                    model.OpenCode1 = Convert.ToInt32(opencodeNode[0].InnerText.Trim());
                    model.OpenCode2 = Convert.ToInt32(opencodeNode[1].InnerText.Trim());
                    model.OpenCode3 = Convert.ToInt32(opencodeNode[2].InnerText.Trim());
                    model.OpenCode4 = Convert.ToInt32(opencodeNode[3].InnerText.Trim());
                    var details = GetKaijiangDetails(tds);
                    model.Spare = details;
                    result.Add(model);
                }

                var checkDataHelper = new CheckDataHelper();
                var dbdata = services.GetListS<OpenCode4DTModel>(currentLottery)
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
                    Name = "数字",
                    Total = string.IsNullOrEmpty(nodes[3].InnerText) ? "0" : nodes[3].InnerText,
                    TotalMoney = string.IsNullOrEmpty(nodes[4].InnerText) ? "0" : nodes[4].InnerText
                },
                new Kaijiangitem
                {
                    Name = "生肖",
                    Total = string.IsNullOrEmpty(nodes[5].InnerText) ? "0" : nodes[5].InnerText,
                    TotalMoney = string.IsNullOrEmpty(nodes[6].InnerText) ? "0" : nodes[6].InnerText
                },
                new Kaijiangitem
                {
                    Name = "季节",
                    Total = string.IsNullOrEmpty(nodes[7].InnerText) ? "0" : nodes[7].InnerText,
                    TotalMoney = string.IsNullOrEmpty(nodes[8].InnerText) ? "0" : nodes[8].InnerText
                }
            };
            entity.KaiJiangItems = list;

            return entity.TryToJson();
        }

        /// <summary>
        ///     获取主站开奖列表数据
        /// </summary>
        /// <param name="mainUrl">主站地址</param>
        /// <returns></returns>
        /// <summary>
        ///     获取备用站点开奖列表数据
        /// </summary>
        /// <param name="backUrl">备用站点</param>
        /// <returns></returns>
        private List<OpenCode4DTModel> GetOpenListFromBackUrl()
        {
            var result = new List<OpenCode4DTModel>();
            try
            {
                var resourceUrl = new Uri(Config.BackUrl);
                var htmlResource = NetHelper.GetUrlResponse(resourceUrl.AbsoluteUri, Encoding.GetEncoding("gb2312"));
                if (htmlResource == null) return result;

                var doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                var table = doc.DocumentNode.SelectSingleNode("//table");
                if (table == null) return result;
                var trs = table.ChildNodes.Where(C => C.Name == "tr").ToList();
                var lastYear = (DateTime.Now.Year - 1).ToString();
                List<HtmlNode> tds = null;
                HtmlNode tagA = null;
                OpenCode4DTModel model = null;
                string openCodes = null;
                string[] openList = null;
                for (var i = 1; i < trs.Count; i++) //第一行为表头
                {
                    tds = trs[i].ChildNodes.Where(S => S.Name.ToLower() == "td").ToList();
                    if (tds.Count < 3) continue;
                    model = new OpenCode4DTModel();
                    if (tds[0].InnerText.Trim().StartsWith(lastYear)) break;
                    model.Term = Convert.ToInt32(tds[0].InnerText);
                    openCodes = tds[1].Attributes["luckyNo"].Value;
                    if (string.IsNullOrWhiteSpace(openCodes)) continue;
                    openList = openCodes.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                    if (openList.Length < 4) continue;
                    model.OpenCode1 = Convert.ToInt32(openList[0]);
                    model.OpenCode2 = GetDictValue(openList[1], DictType.Animal);
                    model.OpenCode3 = GetDictValue(openList[2], DictType.Season);
                    model.OpenCode4 = GetDictValue(openList[3], DictType.Direction);
                    tagA = tds[2].ChildNodes.Where(N => N.Name.ToLower() == "a").FirstOrDefault();
                    model.OpenTime = CommonHelper.GetGDHC1orGD367Opentime(tds[0].InnerText);
                    if (tagA == null) continue;
                    model.DetailUrl = tagA.InnerText.Trim();
                    result.Add(model);
                }

                var checkDataHelper = new CheckDataHelper();
                var dbdata = services.GetListS<OpenCode4DTModel>(currentLottery)
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
        ///     完善主站江苏体彩7位数开奖详情信息
        /// </summary>
        /// <param name="model"></param>
        private bool OptimizeMainModel(ref OpenCode4DTModel model, HtmlNode tr)
        {
            var entity = new KaijiangDetailsEntity();
            entity.KaiJiangItems = new List<Kaijiangitem>();
            try
            {
                var tds = tr.ChildNodes.Where(w => w.Name == "td").ToList();
                var xiaoshoue = tds[4].InnerText.Replace(",", "");
                var jiangchi = tds[13].InnerText.Replace(",", "");
                var shuzijiang = new Kaijiangitem
                {
                    Name = "数字",
                    Total = tds[5].InnerText,
                    TotalMoney = tds[6].InnerText
                };
                entity.KaiJiangItems.Add(shuzijiang);
                var shengxiaojiang = new Kaijiangitem
                {
                    Name = "生肖",
                    Total = tds[7].InnerText,
                    TotalMoney = tds[8].InnerText
                };
                entity.KaiJiangItems.Add(shengxiaojiang);
                var jijiejiang = new Kaijiangitem
                {
                    Name = "季节",
                    Total = tds[9].InnerText,
                    TotalMoney = tds[10].InnerText
                };
                entity.KaiJiangItems.Add(jijiejiang);
                var fangweijiang = new Kaijiangitem
                {
                    Name = "方位",
                    Total = tds[11].InnerText,
                    TotalMoney = tds[12].InnerText
                };
                entity.KaiJiangItems.Add(fangweijiang);
                entity.Gdje = jiangchi;
                entity.Trje = xiaoshoue;
                model.Spare = entity.TryToJson();
                return true;
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过主站点优化开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            return false;
        }

        /// <summary>
        ///     完善主站江苏体彩7位数开奖详情信息
        /// </summary>
        /// <param name="model"></param>
        private bool OptimizeBackModel(ref OpenCode4DTModel model)
        {
            var entity = new KaijiangDetailsEntity();
            entity.KaiJiangItems = new List<Kaijiangitem>();
            try
            {
                var url = string.Format(@"http://www.gdfc.org.cn/{0}", model.DetailUrl);
                var htmlResource = NetHelper.GetUrlResponse(url, Encoding.GetEncoding("gb2312"));
                if (string.IsNullOrWhiteSpace(htmlResource)) return false;
                var doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                var opendiv = doc.DocumentNode.SelectSingleNode("//div[@class='play_R_tbox']");
                var moneydiv = doc.DocumentNode.SelectSingleNode("//div[@class='play_R_jbox']");
                var kaijiangitemtable = doc.DocumentNode.SelectNodes("//table"); //第二个
                if (opendiv != null)
                {
                    var opentime = opendiv.InnerText.Trim().Substring(0, 10);
                    model.OpenTime = DateTime.Parse(opentime);
                }

                if (moneydiv != null)
                {
                    var divs = moneydiv.ChildNodes.Where(w => w.Name == "div").ToList();
                    var xiaoshoudiv = divs[0].ChildNodes.Where(w => w.Name == "span").ToList();
                    var jiangchidiv = divs[1].ChildNodes.Where(w => w.Name == "span").ToList();
                    var xiaoshoue = xiaoshoudiv[0].InnerText.Replace("￥", "").Replace(",", "");
                    var jiangchi = jiangchidiv[0].InnerText.Replace("￥", "").Replace(",", "");
                    entity.Trje = xiaoshoue;
                    entity.Gdje = jiangchi;
                }

                if (kaijiangitemtable != null)
                {
                    var trs = kaijiangitemtable[1].ChildNodes.Where(w => w.Name == "tr").ToList();
                    for (var i = 2; i < trs.Count; i++)
                    {
                        var tds = trs[i].ChildNodes.Where(w => w.Name == "td").ToList();
                        var name = tds[0].InnerText.Trim();
                        var count = tds[1].InnerText.Trim();
                        var money = tds[2].InnerText.Replace("￥", "").Trim();
                        var tmp = new Kaijiangitem
                        {
                            Name = name,
                            Total = count,
                            TotalMoney = money
                        };
                        entity.KaiJiangItems.Add(tmp);
                    }
                }

                model.Spare = entity.TryToJson();
            }
            catch (Exception ex)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过主站点优化开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            return false;
        }

        private void DoBackUrl()
        {
            if (!string.IsNullOrEmpty(Config.BackUrl))
            {
                var OpenList = GetOpenListFromBackUrl();
                if (OpenList.Count == 0) return; //无抓取数据
                var newestQiHao = OpenList.Max(w => w.Term).ToString();
                var startQiNum = Convert.ToInt32(LatestItem.Term.ToString().Substring(4)) + 1;
                var newestQiNum = Convert.ToInt32(newestQiHao.Substring(4));
                if (startQiNum > newestQiNum) return; //无最新数据
                //处理最新开奖数据
                var getQiHao = string.Empty;
                OpenCode4DTModel matchItem = null;
                for (var i = startQiNum; i <= newestQiNum; i++)
                {
                    getQiHao = LatestItem.Term.ToString().Substring(0, 4) + i.ToString().PadLeft(3, '0');
                    matchItem = OpenList.Where(R => R.Term.ToString() == getQiHao).FirstOrDefault();
                    OptimizeBackModel(ref matchItem);
                    if (matchItem != null && services.AddDTOpen4Code(currentLottery, matchItem))
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
        ///     把生肖，方向，季节转换成数字
        /// </summary>
        /// <param name="key"></param>
        /// <param name="enumType"></param>
        /// <returns></returns>
        private int GetDictValue(string key, DictType enumType)
        {
            try
            {
                key = key.Trim().Replace(" ", "");

                if (enumType == DictType.Animal) return AnimalDict[key];
                if (enumType == DictType.Direction) return DirectionDict[key];
                if (enumType == DictType.Season) return SeasonDict[key];
                return 0;
            }
            catch (Exception ee)
            {
                return -1;
            }
        }

        private enum DictType
        {
            /// <summary>
            ///     生效
            /// </summary>
            Animal = 1,

            /// <summary>
            ///     季节
            /// </summary>
            Season = 2,

            /// <summary>
            ///     方向
            /// </summary>
            Direction
        }


        #region Attribute

        /// <summary>
        ///     配置信息
        /// </summary>
        private SCCConfig Config;

        /// <summary>
        ///     当天抓取的最新一期开奖记录
        /// </summary>
        private OpenCode4DTModel LatestItem;

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
        private SCCLottery currentLottery => SCCLottery.GuangDongHC1;

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