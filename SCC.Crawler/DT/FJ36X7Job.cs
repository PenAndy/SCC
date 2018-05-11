using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Quartz;
using SCC.Common;
using SCC.Crawler.Tools;
using SCC.Interface;
using SCC.Models;

namespace SCC.Crawler.DT
{
    /// <summary>
    ///     福建36选7
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class FJ36X7Job : IJob
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        public FJ36X7Job()
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
                {
                    LatestItem = services.GetOpenCode8DTLastItem(currentLottery);
                    if (LatestItem == null)
                        LatestItem = new OpenCode8DTModel
                        {
                            Term = CommonHelper.GenerateQiHaoYYQQQ(0),
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
        ///     (百度乐彩)
        /// </summary>
        private void DoMainUrl()
        {
            if (!string.IsNullOrEmpty(Config.MainUrl))
            {
                var OpenList = GetOpenListFromMainUrl();
                OpenList.RemoveAll(w => w.Term < 17000); //只取今年
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
        ///     通过备用站点抓取开奖数据
        ///     (福建体彩网)
        /// </summary>
        private void DoBackUrl()
        {
            if (!string.IsNullOrEmpty(Config.BackUrl))
            {
                var OpenList = GetOpenListFromBackUrl();
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
                        log.Info(GetType(), CommonHelper.GetJobBackLogInfo(Config, getQiHao));
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
        private List<OpenCode8DTModel> GetOpenListFromBackUrl()
        {
            var result = new List<OpenCode8DTModel>();
            try
            {
                var htmlResource = NetHelper.GetBaiDuLeCaiResponse(Config.BackUrl);
                if (htmlResource == null) return result;

                var doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                var tables = doc.DocumentNode.SelectNodes("//table[@class]");
                if (tables == null) return result;

                var trs = tables[0].ChildNodes.Select(w => w).ToList();
                trs.RemoveAll(w => w.Name != "tr");
                trs.RemoveAll(w => w.GetAttributeValue("class", "") == "th kj-bd");
                foreach (var item in trs)
                {
                    var tds = item.ChildNodes.Select(w => w).ToList();
                    tds.RemoveAll(w => w.Name != "td");
                    if (tds.Count != 10) continue;
                    var qihao = tds[0].InnerText.Substring(2);
                    var kaijianghao = tds[1].InnerText.Trim(); //010203040506 格式

                    var opentime = tds[9].InnerText.Trim();
                    var tmp = new OpenCode8DTModel
                    {
                        Term = long.Parse(qihao),
                        OpenTime = DateTime.Parse(opentime)
                    };
                    tmp.OpenCode1 = int.Parse(kaijianghao.Substring(0, 2));
                    tmp.OpenCode2 = int.Parse(kaijianghao.Substring(2, 2));
                    tmp.OpenCode3 = int.Parse(kaijianghao.Substring(4, 2));
                    tmp.OpenCode4 = int.Parse(kaijianghao.Substring(6, 2));
                    tmp.OpenCode5 = int.Parse(kaijianghao.Substring(8, 2));
                    tmp.OpenCode6 = int.Parse(kaijianghao.Substring(10, 2));
                    tmp.OpenCode7 = int.Parse(kaijianghao.Substring(12, 2));
                    tmp.OpenCode8 = int.Parse(kaijianghao.Substring(14, 2));
                    OptimizeBackModel(ref tmp, item);
                    result.Add(tmp);
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
        /// 完善备站数据详情
        /// </summary>
        /// <param name="model"></param>
        /// <param name="tr"></param>
        /// <returns></returns>
        private bool OptimizeBackModel(ref OpenCode8DTModel model, HtmlNode tr)
        {
            try
            {
                var entity = new KaijiangDetailsEntity();
                entity.KaiJiangItems = new List<Kaijiangitem>();
                var tds = tr.ChildNodes.Where(w => w.Name == "td").ToList();
                var xiaoshoue = tds[2].InnerText.Trim().Replace(",", "").Replace("元", "");
                var jiangchi = "";
                var tedengjiang = new Kaijiangitem
                {
                    Name = "特等奖",
                    Total = tds[3].InnerText.Trim(),
                    TotalMoney = tds[4].InnerText
                };
                entity.KaiJiangItems.Add(tedengjiang);
                var yidengjiang = new Kaijiangitem
                {
                    Name = "一等奖",
                    Total = tds[5].InnerText.Trim(),
                    TotalMoney = tds[6].InnerText
                };
                entity.KaiJiangItems.Add(yidengjiang);
                var erdengjiang = new Kaijiangitem
                {
                    Name = "二等奖",
                    Total = tds[7].InnerText.Trim(),
                    TotalMoney = tds[8].InnerText
                };
                entity.KaiJiangItems.Add(erdengjiang);
                entity.Gdje = jiangchi;
                entity.Trje = xiaoshoue;
                model.Spare = entity.TryToJson();
            }
            catch (Exception ex)
            {
                log.Error(typeof(JSTC7WSJob),
                    string.Format("【{0}】通过主站点优化开奖详情时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            return false;
        }

        /// <summary>
        ///     给开奖信息加上开奖详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool OptimizeMainModel(ref OpenCode8DTModel modelr, HtmlNode tr)
        {
            try
            {
                var entity = new KaijiangDetailsEntity();
                entity.KaiJiangItems = new List<Kaijiangitem>();
                var tds = tr.ChildNodes.Select(w => w).ToList();
                tds.RemoveAll(w => w.Name != "td");
                if (tds.Count != 13) return false;
                var tdjstr = tds[2].InnerText.Trim().Split('注', '元');
                var tedengjiang = new Kaijiangitem
                {
                    Name = "特等奖",
                    Total = tdjstr[0].Replace(",", "").Trim(),
                    TotalMoney = tdjstr[1].Replace(",", "").Trim()
                };
                entity.KaiJiangItems.Add(tedengjiang);
                var ydjstr = tds[3].InnerText.Trim().Split('注', '元');
                var yidengjiang = new Kaijiangitem
                {
                    Name = "一等奖",
                    Total = ydjstr[0].Replace(",", "").Trim(),
                    TotalMoney = ydjstr[1].Replace(",", "").Trim()
                };
                entity.KaiJiangItems.Add(yidengjiang);
                var edjstr = tds[4].InnerText.Trim().Split('注', '元');
                var erdengjiang = new Kaijiangitem
                {
                    Name = "二等奖",
                    Total = edjstr[0].Replace(",", "").Trim(),
                    TotalMoney = edjstr[1].Replace(",", "").Trim()
                };
                entity.KaiJiangItems.Add(erdengjiang);
                var sdjstr = tds[5].InnerText.Trim().Split('注', '元');
                var sandengjiang = new Kaijiangitem
                {
                    Name = "三等奖",
                    Total = sdjstr[0].Replace(",", "").Trim(),
                    TotalMoney = sdjstr[1].Replace(",", "").Trim()
                };
                entity.KaiJiangItems.Add(sandengjiang);
                var sidjstr = tds[6].InnerText.Trim().Split('注', '元');
                var sidengjiang = new Kaijiangitem
                {
                    Name = "四等奖",
                    Total = sidjstr[0].Replace(",", "").Trim(),
                    TotalMoney = sidjstr[1].Replace(",", "").Trim()
                };
                entity.KaiJiangItems.Add(sidengjiang);

                var wdjstr = tds[7].InnerText.Trim().Split('注', '元');
                var wudengjiang = new Kaijiangitem
                {
                    Name = "五等奖",
                    Total = wdjstr[0].Replace(",", "").Trim(),
                    TotalMoney = wdjstr[1].Replace(",", "").Trim()
                };
                entity.KaiJiangItems.Add(wudengjiang);

                var ldjstr = tds[8].InnerText.Trim().Split('注', '元');
                var liudengjiang = new Kaijiangitem
                {
                    Name = "六等奖",
                    Total = ldjstr[0].Replace(",", "").Trim(),
                    TotalMoney = ldjstr[1].Replace(",", "").Trim()
                };
                entity.KaiJiangItems.Add(liudengjiang);

                var xyjstr = tds[9].InnerText.Trim().Split('注', '元');
                var xingyunjiang = new Kaijiangitem
                {
                    Name = "幸运奖",
                    Total = xyjstr[0].Replace(",", "").Trim(),
                    TotalMoney = xyjstr[1].Replace(",", "").Trim()
                };
                entity.KaiJiangItems.Add(xingyunjiang);
                entity.Gdje = tds[11].InnerText.Replace(",", "").Trim();
                entity.Trje = tds[10].InnerText.Replace(",", "").Trim();
                modelr.Spare = entity.TryToJson();
                return true;
            }
            catch (Exception ee)
            {
                return false;
            }
        }

        /// <summary>
        ///     获取备用站点开奖列表数据
        /// </summary>
        /// <returns></returns>
        private List<OpenCode8DTModel> GetOpenListFromMainUrl()
        {
            var result = new List<OpenCode8DTModel>();
            try
            {
                var htmlResource = NetHelper.GetUrlResponse(Config.MainUrl);
                if (htmlResource == null) return result;

                var doc = new HtmlDocument();
                doc.LoadHtml(htmlResource);
                var tables = doc.DocumentNode.SelectNodes("//table[@cellpadding]");
                if (tables == null) return result;

                var tbody = tables[0].ChildNodes.Where(w => w.Name == "tbody").ToList();
                var trs = tbody[0].ChildNodes.Where(w => w.Name == "tr").ToList();
                foreach (var item in trs)
                {
                    var tds = item.ChildNodes.Select(w => w).ToList();
                    tds.RemoveAll(w => w.Name != "td");
                    var qihao = tds[0].InnerText.Trim();
                    var kaijianghaoma = tds[1].InnerText.Trim().Split('+');
                    var opentime = tds[12].InnerText.Trim();
                    var tmp = new OpenCode8DTModel
                    {
                        Term = long.Parse(qihao),
                        OpenTime = DateTime.Parse(opentime)
                    };
                    tmp.OpenCode1 = int.Parse(kaijianghaoma[0]);
                    tmp.OpenCode2 = int.Parse(kaijianghaoma[1]);
                    tmp.OpenCode3 = int.Parse(kaijianghaoma[2]);
                    tmp.OpenCode4 = int.Parse(kaijianghaoma[3]);
                    tmp.OpenCode5 = int.Parse(kaijianghaoma[4]);
                    tmp.OpenCode6 = int.Parse(kaijianghaoma[5]);
                    tmp.OpenCode7 = int.Parse(kaijianghaoma[6]);
                    tmp.OpenCode8 = int.Parse(kaijianghaoma[7]);

                    OptimizeMainModel(ref tmp, item);
                    result.Add(tmp);
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
        private SCCLottery currentLottery => SCCLottery.FuJianTC36x7;

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