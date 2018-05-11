using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Quartz;
using SCC.Common;
using SCC.Crawler.Tools;
using SCC.Interface;
using SCC.Models;
using SCC.Services;

namespace SCC.Crawler.GP
{
    /// <summary>
    ///     黑龙江快乐十分
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class HeiLongJiangKL10FJob : IJob
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        public HeiLongJiangKL10FJob()
        {
            log = new LogHelper();
            services = IOC.Resolve<IOpen8Code>();
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
            LatestQiHao = context.JobDetail.JobDataMap.GetString("LatestQiHao");
            try
            {
                //服务启动时配置初始数据
                if (string.IsNullOrEmpty(LatestQiHao))
                {
                    var lastItem = services.GetLastItem(currentLottery);
                    if (lastItem != null) LatestQiHao = lastItem.Term.ToString();
                }

                DoYesterdayFailedListByMainUrl();
                DoYesterdayFailedListByBackUrl();
                
                
            }
            catch (Exception ex)
            {
                log.Error(GetType(), string.Format("【{0}】抓取时发生错误，错误信息【{1}】", Config.Area + currentLottery, ex.Message));
            }

            //保存最新期号和失败期号列表
            context.JobDetail.JobDataMap["LatestQiHao"] = LatestQiHao;
        }
        /// <summary>
        ///     从备用网址处理昨天失败的开彩
        /// </summary>
        public void DoYesterdayFailedListByBackUrl()
        {
            if (!string.IsNullOrEmpty(Config.BackUrl))
            {
                var OpenList = GetDocByBackUrl();
                if (OpenList.Count == 0) return; //无抓取数据
                var SuccessList = new List<string>();
                foreach (var item in OpenList)
                    if (item != null && SaveRecord(item))
                        log.Info(GetType(), CommonHelper.GetJobMainLogInfo(Config, item.Id.ToString()));
            }
        }

        /// <summary>
        ///     保存开奖信息到数据库中
        /// </summary>
        /// <param name="list"></param>
        /// <summary>
        ///     通过主站抓取错误期号列表中每一个期号
        /// </summary>
        private void DoYesterdayFailedListByMainUrl()
        {
            var OpenList = GetDocByMainUrl();
            if (OpenList.Count == 0) return; //无抓取数据
            var SuccessList = new List<string>();
            foreach (var item in OpenList)
                if (item != null && SaveRecord(item))
                    log.Info(GetType(), CommonHelper.GetJobMainLogInfo(Config, item.Id.ToString()));
        }

        /// <summary>
        ///     从备用站抓取开奖数据 昨天/今天
        /// </summary>
        /// <param name="IsToday"></param>
        /// <returns></returns>
        private List<HeiLongJiangKL10FEntity> GetDocByBackUrl(bool IsToday = true)

        {
            var list = new List<HeiLongJiangKL10FEntity>();
            try
            {
                var day = "1";
                if (!IsToday) day = "2";
                var url = string.Format(Config.BackUrl, day);
                var HtmlResource = NetHelper.GetUrlResponse(url);
                if (HtmlResource == null) return list;

                var doc = new HtmlDocument();
                doc.LoadHtml(HtmlResource);
                var tables = doc.DocumentNode.SelectNodes("//*[@id=\"ZstTable\"]");
                if (tables == null) return list;

                foreach (var item in tables[0].ChildNodes)
                {
                    var opencode = new List<string>();
                    var qihao = "";
                    if (item.GetAttributeValue("class", "").Contains("datainfo "))
                        foreach (var item2 in item.ChildNodes)
                        {
                            if (item2.GetAttributeValue("class", "") == "issue n_right_2") qihao = item2.InnerText;
                            if (!item2.HasAttributes || item2.GetAttributeValue("style", "") == "color:red;")
                                opencode.Add(item2.InnerText.Trim());
                        }

                    if (opencode.Count == 8)
                    {
                        var tmp = new HeiLongJiangKL10FEntity
                        {
                            Id = int.Parse(qihao),
                            OpenCode = CommonHelper.TRHandleCode(string.Join(",", opencode)),
                            OpenTime = HandleOpenTime(qihao)
                        };
                        list.Add(tmp);
                    }
                }

                var checkDataHelper = new CheckDataHelper();
                var dbdata = services.GetListIn(currentLottery, IsToday)
                    .ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr());
                checkDataHelper.CheckData(dbdata, list.ToDictionary(w => w.Id.ToString(), w => w.OpenCode), Config.Area,
                    currentLottery);
            }
            catch (Exception ee)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过站点抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ee.Message));
            }

            return list;
        }

        /// <summary>
        ///     从主站抓取开奖数据 昨天/今天
        /// </summary>
        /// <param name="IsToday"></param>
        /// <returns></returns>
        private List<HeiLongJiangKL10FEntity> GetDocByMainUrl(bool IsToday = true)
        {
            var list = new List<HeiLongJiangKL10FEntity>();
            try
            {
                var time = DateTime.Now;
                var arg = new OpenCaiApiArg
                {
                    code = EnumHelper.GetLotteryCode(SCCLottery.HeiLongJiangKL10F),
                    date = time.ToString("yyyy-MM-dd")
                };
                if (!IsToday) arg.date = time.AddDays(-1).ToString("yyyy-MM-dd");
                var data = OpenCaiApiServices.GetOpenCaiApiData(arg);
                if (data == null) return list;


                if (data.data != null)
                {
                    if (data.data.Count == 0) return list;

                    for (var i = 0; i < data.data.Count; i++)
                    {
                        var tmp = new HeiLongJiangKL10FEntity
                        {
                            Id = int.Parse(data.data[i].expect),
                            OpenCode = data.data[i].GetOpenCodeStr(),
                            OpenTime = DateTime.Parse(data.data[i].opentime)
                        };
                        list.Add(tmp);
                    }

                    var checkDataHelper = new CheckDataHelper();
                    var dbdata = services.GetListIn(currentLottery, IsToday)
                        .ToDictionary(w => w.Term.ToString(), w => w.GetCodeStr());
                    checkDataHelper.CheckData(dbdata, list.ToDictionary(w => w.Id.ToString(), w => w.OpenCode),
                        Config.Area, currentLottery);
                }
            }
            catch (Exception ee)
            {
                log.Error(GetType(),
                    string.Format("【{0}】通过站点抓取开奖列表时发生错误，错误信息【{1}】", Config.Area + currentLottery, ee.Message));
            }

            return list;
        }

        /// <summary>
        ///     插入单条
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SaveRecord(HeiLongJiangKL10FEntity data)
        {
            var model = new OpenCode8Model();
            model.Term = Convert.ToInt64(data.Id); //期号
            var haoMaArray = data.OpenCode.Split(',');
            model.OpenCode1 = Convert.ToInt32(haoMaArray[0]);
            model.OpenCode2 = Convert.ToInt32(haoMaArray[1]);
            model.OpenCode3 = Convert.ToInt32(haoMaArray[2]);
            model.OpenCode4 = Convert.ToInt32(haoMaArray[3]);
            model.OpenCode5 = Convert.ToInt32(haoMaArray[4]);
            model.OpenCode6 = Convert.ToInt32(haoMaArray[5]);
            model.OpenCode7 = Convert.ToInt32(haoMaArray[6]);
            model.OpenCode8 = Convert.ToInt32(haoMaArray[7]);
            model.OpenTime = data.OpenTime;
            if (services.AddOpen8Code(currentLottery, model))
            {
                GetMaxPeriodNum((int) model.Term); //添加成功存放期数
                return true;
            }

            return false;
        }


      

        private DateTime HandleOpenTime(string qihao)
        {
            try
            {
                var nnum = int.Parse(qihao);
                if (nnum < 193013) return DateTime.Now;
                var ny = (nnum - 193013) / Config.TimesPerDay;
                var nc = (nnum - 193013) % Config.TimesPerDay;
                var time = new DateTime(2017, 12, 11, Config.StartHour, Config.StartMinute, 0).AddDays(ny)
                    .AddMinutes(Config.Interval * (nc + 1));
                return time;
            }
            catch (Exception ee)
            {
                return DateTime.Now;
            }
        }


        /// <summary>
        ///     比较期号大的存入LatestQiHao
        /// </summary>
        /// <param name="PeriodNum"></param>
        public void GetMaxPeriodNum(int PeriodNum)
        {
            var lastqihao = 0;
            int.TryParse(LatestQiHao, out lastqihao);

            if (lastqihao < PeriodNum) LatestQiHao = PeriodNum.ToString();
        }

        #region Attribute

        /// <summary>
        ///     配置信息
        /// </summary>
        private SCCConfig Config;

        /// <summary>
        ///     当天抓取的最新一期期号
        /// </summary>
        private string LatestQiHao = "0";

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
        private readonly IOpen8Code services;

        /// <summary>
        ///     当前彩种
        /// </summary>
        private SCCLottery currentLottery => SCCLottery.HeiLongJiangKL10F;

        /// <summary>
        ///     邮件接口
        /// </summary>
        private IEmail email;

        #endregion
    }

    /// <summary>
    ///     开奖号实体
    /// </summary>
    public class HeiLongJiangKL10FEntity
    {
        public int Id { get; set; }
        public string OpenCode { get; set; }
        public DateTime OpenTime { get; set; }
    }
}