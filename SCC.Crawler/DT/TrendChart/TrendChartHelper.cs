using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SCC.Models;
using SCC.Common;
using SCC.Interface;
using SCC.Services;

namespace SCC.Crawler.DT
{
    /// <summary>
    /// 地方彩生成走势图的帮助类
    /// </summary>
    public static class TrendChartHelper
    {
        /// <summary>
        /// 生成东方6+1走势图
        /// </summary>
        /// <param name="Log">日志实例</param>
        public static void GenerateFCDF6J1TrendChart(LogHelper Log)
        {
            ITrendChart helper = new TrendChartServices();
            IDTOpenCode service = new DTOpenCodeServices();
            var DF6J1 = ConfigHelper.GetConfigValue<int>("DF6J1");
            var configList = helper.GetTrendChartConfig(DF6J1);
            if (configList.Count == 0)
            {
                Console.WriteLine("未找到有效走势图信息");
                return;
            }
            var trendChartItemList = helper.GetTrendChartItem(configList.Select(c => c.Id).ToList());
            foreach (var itemInfo in trendChartItemList)
            {
                itemInfo.Cid = DF6J1;
            }
            var dbItemList = service.GetDF6J1ListOpenCode();
            if (dbItemList.Count == 0)
            {
                Console.WriteLine("未找到有效开奖数据");
                return;
            }
            var chartCssConfigs = helper.GetChartCssConfigs();
            List<TrendChartData> trendChartDataList = new List<TrendChartData>();
            foreach (var config in configList)//基本走势图，和值走势图，手机版
            {
                var chartId = config.Id;
                var trendChartItems = trendChartItemList.Where(S => S.ChartId == chartId).OrderBy(S => S.OrderBy).ToList();
                if (trendChartItems.Count == 0)
                {
                    Console.WriteLine("未找到该走势图的显示项信息");
                    return;
                }
                Console.WriteLine(string.Format("------生成{0}------", config.Name));
                var chartItems = new TrendChartItemHelper<FCDF6J1Info>().InitTrendChartItem(chartCssConfigs, trendChartItems);
                var count = trendChartItems.Count;
                //var j = 0;
                TrendChartData entity = null;
                var ResultEntity = new TrendChartData
                {
                    ChartId = chartId,
                    Term = 0,
                    ChartType = TrendChartType.PC,
                    LocalMiss = new string[count],
                    LastMiss = new string[count],
                    AllMaxMiss = new string[count],
                    AllAvgMiss = new string[count],
                    AllTimes = new string[count]
                };
                foreach (var dbItem in dbItemList)
                {
                    var twoInfoList = dbItemList.Where(R => R.Term <= dbItem.Term).OrderByDescending(O => O.Term).Take(2).ToList();

                    FCDF6J1Info info = null;
                    info = twoInfoList[0];

                    if (twoInfoList.Count == 2)
                    {
                        entity = ResultEntity;//变量存储上一个走势图数据
                        //if(j==0)
                        //    entity
                    }

                    bool yes = true;
                    var sb = new StringBuilder(20000);
                    sb.Append("<tr>");
                    for (var i = 0; i < count; i++)
                    {
                        chartItems[i].InitMissData(entity, i);
                        //计算项值及遗漏计算
                        yes = yes && chartItems[i].SetItemValue(info);
                        if (yes)
                        {
                            //结果集赋值
                            ResultEntity.LocalMiss[i] = chartItems[i].GetMissData(MissDataType.LocalMiss);
                            ResultEntity.LastMiss[i] = chartItems[i].GetMissData(MissDataType.LastMiss);
                            ResultEntity.AllMaxMiss[i] = chartItems[i].GetMissData(MissDataType.AllMaxMiss);
                            ResultEntity.AllAvgMiss[i] = chartItems[i].GetMissData(MissDataType.AllAvgMiss);
                            ResultEntity.AllTimes[i] = chartItems[i].GetMissData(MissDataType.AllTimes);

                            sb.Append(chartItems[i].GetFomartString("<td {0}>{1}</td>"));
                        }
                    }
                    sb.Append("</tr>");
                    if (null != entity)
                        ResultEntity.RecordCount = entity.RecordCount + 1;
                    else
                        ResultEntity.RecordCount = 1;
                    ResultEntity.Term = dbItem.Term;
                    ResultEntity.HtmlData = sb.ToString();

                    Console.WriteLine(string.Format("为{0}期开奖数据生成{1}成功", dbItem.Term, config.Name));
                    trendChartDataList.Add(new TrendChartData()
                    {
                        Id = ResultEntity.Id,
                        ChartId = ResultEntity.ChartId,
                        Term = ResultEntity.Term,
                        AllMaxMiss = ResultEntity.AllMaxMiss.Clone() as string[],
                        AllTimes = ResultEntity.AllTimes.Clone() as string[],
                        RecordCount = ResultEntity.RecordCount,
                        AllAvgMiss = ResultEntity.AllAvgMiss.Clone() as string[],
                        LastMiss = ResultEntity.LastMiss.Clone() as string[],
                        LocalMiss = ResultEntity.LocalMiss.Clone() as string[],
                        HtmlData = ResultEntity.HtmlData,
                        ChartType = ResultEntity.ChartType,
                        Addtime = ResultEntity.Addtime
                    });
                }
                Console.WriteLine();
            }
            if (helper.SaveTrendChartList(SCCLottery.DF6J1TrendChart, trendChartDataList))
                Log.Info(typeof(TrendChartHelper), "------<<生成东方6+1走势图数据成功！>>------");
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("------<<生成东方6+1走势图数据失败！>>------");
                Console.ResetColor();
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// 生成华东15选5走势图
        /// </summary>
        /// <param name="Log">日志实例</param>
        public static void GenerateHD15X5TrendChart(LogHelper Log)
        {
            ITrendChart helper = new TrendChartServices();
            IDTOpenCode service = new DTOpenCodeServices();
            var HD15X5 = ConfigHelper.GetConfigValue<int>("HD15X5");
            var configList = helper.GetTrendChartConfig(HD15X5);
            if (configList.Count == 0)
            {
                Console.WriteLine("未找到有效走势图信息");
                return;
            }
            var trendChartItemList = helper.GetTrendChartItem(configList.Select(c => c.Id).ToList());
            foreach (var itemInfo in trendChartItemList)
            {
                itemInfo.Cid = HD15X5;
            }
            var dbItemList = service.GetHD15X5ListOpenCode();
            if (dbItemList.Count == 0)
            {
                Console.WriteLine("未找到有效开奖数据");
                return;
            }
            var chartCssConfigs = helper.GetChartCssConfigs();
            List<TrendChartData> trendChartDataList = new List<TrendChartData>();
            foreach (var config in configList)//基本走势图，和值走势图，手机版
            {
                var chartId = config.Id;
                var trendChartItems = trendChartItemList.Where(S => S.ChartId == chartId).OrderBy(S => S.OrderBy).ToList();
                if (trendChartItems.Count == 0)
                {
                    Console.WriteLine("未找到该走势图的显示项信息");
                    return;
                }
                Console.WriteLine(string.Format("------生成{0}------", config.Name));
                var chartItems = new TrendChartItemHelper<FCHD15X5Info>().InitTrendChartItem(chartCssConfigs, trendChartItems);
                var count = trendChartItems.Count;
                //var j = 0;
                TrendChartData entity = null;
                var ResultEntity = new TrendChartData
                {
                    ChartId = chartId,
                    Term = 0,
                    ChartType = TrendChartType.PC,
                    LocalMiss = new string[count],
                    LastMiss = new string[count],
                    AllMaxMiss = new string[count],
                    AllAvgMiss = new string[count],
                    AllTimes = new string[count]
                };
                foreach (var dbItem in dbItemList)
                {
                    var twoInfoList = dbItemList.Where(R => R.Term <= dbItem.Term).OrderByDescending(O => O.Term).Take(2).ToList();

                    FCHD15X5Info info = null;
                    info = twoInfoList[0];

                    if (twoInfoList.Count == 2)
                    {
                        entity = ResultEntity;//变量存储上一个走势图数据
                        //if(j==0)
                        //    entity
                    }

                    bool yes = true;
                    var sb = new StringBuilder(20000);
                    sb.Append("<tr>");
                    for (var i = 0; i < count; i++)
                    {
                        chartItems[i].InitMissData(entity, i);
                        //计算项值及遗漏计算
                        yes = yes && chartItems[i].SetItemValue(info);
                        if (yes)
                        {
                            //结果集赋值
                            ResultEntity.LocalMiss[i] = chartItems[i].GetMissData(MissDataType.LocalMiss);
                            ResultEntity.LastMiss[i] = chartItems[i].GetMissData(MissDataType.LastMiss);
                            ResultEntity.AllMaxMiss[i] = chartItems[i].GetMissData(MissDataType.AllMaxMiss);
                            ResultEntity.AllAvgMiss[i] = chartItems[i].GetMissData(MissDataType.AllAvgMiss);
                            ResultEntity.AllTimes[i] = chartItems[i].GetMissData(MissDataType.AllTimes);

                            sb.Append(chartItems[i].GetFomartString("<td {0}>{1}</td>"));
                        }
                    }
                    sb.Append("</tr>");
                    if (null != entity)
                        ResultEntity.RecordCount = entity.RecordCount + 1;
                    else
                        ResultEntity.RecordCount = 1;
                    ResultEntity.Term = dbItem.Term;
                    ResultEntity.HtmlData = sb.ToString();

                    Console.WriteLine(string.Format("为{0}期开奖数据生成{1}成功", dbItem.Term, config.Name));
                    trendChartDataList.Add(new TrendChartData()
                    {
                        Id = ResultEntity.Id,
                        ChartId = ResultEntity.ChartId,
                        Term = ResultEntity.Term,
                        AllMaxMiss = ResultEntity.AllMaxMiss.Clone() as string[],
                        AllTimes = ResultEntity.AllTimes.Clone() as string[],
                        RecordCount = ResultEntity.RecordCount,
                        AllAvgMiss = ResultEntity.AllAvgMiss.Clone() as string[],
                        LastMiss = ResultEntity.LastMiss.Clone() as string[],
                        LocalMiss = ResultEntity.LocalMiss.Clone() as string[],
                        HtmlData = ResultEntity.HtmlData,
                        ChartType = ResultEntity.ChartType,
                        Addtime = ResultEntity.Addtime
                    });
                }
                Console.WriteLine();
            }
            if (helper.SaveTrendChartList(SCCLottery.HD15X5TrendChart, trendChartDataList))
                Log.Info(typeof(TrendChartHelper), "------<<生成华东15选5走势图数据成功！>>------");
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("------<<生成华东15选5走势图数据失败！>>------");
                Console.ResetColor();
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// 生成河南22选5走势图
        /// </summary>
        /// <param name="Log">日志实例</param>
        public static void GenerateHN22X5TrendChart(LogHelper Log)
        {
            ITrendChart helper = new TrendChartServices();
            IDTOpenCode service = new DTOpenCodeServices();
            var HeNan22X5 = ConfigHelper.GetConfigValue<int>("HeNan22X5");
            var configList = helper.GetTrendChartConfig(HeNan22X5);
            if (configList.Count == 0)
            {
                Console.WriteLine("未找到有效走势图信息");
                return;
            }
            var trendChartItemList = helper.GetTrendChartItem(configList.Select(c => c.Id).ToList());
            foreach (var itemInfo in trendChartItemList)
            {
                itemInfo.Cid = HeNan22X5;
            }
            var dbItemList = service.GetHN22X5ListOpenCode();
            if (dbItemList.Count == 0)
            {
                Console.WriteLine("未找到有效开奖数据");
                return;
            }
            var chartCssConfigs = helper.GetChartCssConfigs();
            List<TrendChartData> trendChartDataList = new List<TrendChartData>();
            foreach (var config in configList)//基本走势图，和值走势图，手机版
            {
                var chartId = config.Id;
                var trendChartItems = trendChartItemList.Where(S => S.ChartId == chartId).OrderBy(S => S.OrderBy).ToList();
                if (trendChartItems.Count == 0)
                {
                    Console.WriteLine("未找到该走势图的显示项信息");
                    return;
                }
                Console.WriteLine(string.Format("------生成{0}------", config.Name));
                var chartItems = new TrendChartItemHelper<FCHN22X5Info>().InitTrendChartItem(chartCssConfigs, trendChartItems);
                var count = trendChartItems.Count;
                //var j = 0;
                TrendChartData entity = null;
                var ResultEntity = new TrendChartData
                {
                    ChartId = chartId,
                    Term = 0,
                    ChartType = TrendChartType.PC,
                    LocalMiss = new string[count],
                    LastMiss = new string[count],
                    AllMaxMiss = new string[count],
                    AllAvgMiss = new string[count],
                    AllTimes = new string[count]
                };
                foreach (var dbItem in dbItemList)
                {
                    var twoInfoList = dbItemList.Where(R => R.Term <= dbItem.Term).OrderByDescending(O => O.Term).Take(2).ToList();

                    FCHN22X5Info info = null;
                    info = twoInfoList[0];

                    if (twoInfoList.Count == 2)
                    {
                        entity = ResultEntity;//变量存储上一个走势图数据
                        //if(j==0)
                        //    entity
                    }

                    bool yes = true;
                    var sb = new StringBuilder(20000);
                    sb.Append("<tr>");
                    for (var i = 0; i < count; i++)
                    {
                        chartItems[i].InitMissData(entity, i);
                        //计算项值及遗漏计算
                        yes = yes && chartItems[i].SetItemValue(info);
                        if (yes)
                        {
                            //结果集赋值
                            ResultEntity.LocalMiss[i] = chartItems[i].GetMissData(MissDataType.LocalMiss);
                            ResultEntity.LastMiss[i] = chartItems[i].GetMissData(MissDataType.LastMiss);
                            ResultEntity.AllMaxMiss[i] = chartItems[i].GetMissData(MissDataType.AllMaxMiss);
                            ResultEntity.AllAvgMiss[i] = chartItems[i].GetMissData(MissDataType.AllAvgMiss);
                            ResultEntity.AllTimes[i] = chartItems[i].GetMissData(MissDataType.AllTimes);

                            sb.Append(chartItems[i].GetFomartString("<td {0}>{1}</td>"));
                        }
                    }
                    sb.Append("</tr>");
                    if (null != entity)
                        ResultEntity.RecordCount = entity.RecordCount + 1;
                    else
                        ResultEntity.RecordCount = 1;
                    ResultEntity.Term = dbItem.Term;
                    ResultEntity.HtmlData = sb.ToString();

                    Console.WriteLine(string.Format("为{0}期开奖数据生成{1}成功", dbItem.Term, config.Name));
                    trendChartDataList.Add(new TrendChartData()
                    {
                        Id = ResultEntity.Id,
                        ChartId = ResultEntity.ChartId,
                        Term = ResultEntity.Term,
                        AllMaxMiss = ResultEntity.AllMaxMiss.Clone() as string[],
                        AllTimes = ResultEntity.AllTimes.Clone() as string[],
                        RecordCount = ResultEntity.RecordCount,
                        AllAvgMiss = ResultEntity.AllAvgMiss.Clone() as string[],
                        LastMiss = ResultEntity.LastMiss.Clone() as string[],
                        LocalMiss = ResultEntity.LocalMiss.Clone() as string[],
                        HtmlData = ResultEntity.HtmlData,
                        ChartType = ResultEntity.ChartType,
                        Addtime = ResultEntity.Addtime
                    });
                }
                Console.WriteLine();
            }
            if (helper.SaveTrendChartList(SCCLottery.HeNan22X5TrendChart, trendChartDataList))
                Log.Info(typeof(TrendChartHelper), "------<<生成河南22选5走势图数据成功！>>------");
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("------<<生成河南22选5走势图数据失败！>>------");
                Console.ResetColor();
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// 生成广东36选7走势图
        /// </summary>
        /// <param name="Log">日志实例</param>
        public static void GenerateGD36X7TrendChart(LogHelper Log)
        {
            ITrendChart helper = new TrendChartServices();
            IDTOpenCode service = new DTOpenCodeServices();
            var NY36X7 = ConfigHelper.GetConfigValue<int>("NY36X7");
            var configList = helper.GetTrendChartConfig(NY36X7);
            if (configList.Count == 0)
            {
                Console.WriteLine("未找到有效走势图信息");
                return;
            }
            var trendChartItemList = helper.GetTrendChartItem(configList.Select(c => c.Id).ToList());
            foreach (var itemInfo in trendChartItemList)
            {
                itemInfo.Cid = NY36X7;
            }
            var dbItemList = service.GetGD36X7ListOpenCode();
            if (dbItemList.Count == 0)
            {
                Console.WriteLine("未找到有效开奖数据");
                return;
            }
            var chartCssConfigs = helper.GetChartCssConfigs();
            List<TrendChartData> trendChartDataList = new List<TrendChartData>();
            foreach (var config in configList)//基本走势图，和值走势图，手机版
            {
                var chartId = config.Id;
                var trendChartItems = trendChartItemList.Where(S => S.ChartId == chartId).OrderBy(S => S.OrderBy).ToList();
                if (trendChartItems.Count == 0)
                {
                    Console.WriteLine("未找到该走势图的显示项信息");
                    return;
                }
                Console.WriteLine(string.Format("------生成{0}------", config.Name));
                var chartItems = new TrendChartItemHelper<FCNY36X7Info>().InitTrendChartItem(chartCssConfigs, trendChartItems);
                var count = trendChartItems.Count;
                //var j = 0;
                TrendChartData entity = null;
                var ResultEntity = new TrendChartData
                {
                    ChartId = chartId,
                    Term = 0,
                    ChartType = TrendChartType.PC,
                    LocalMiss = new string[count],
                    LastMiss = new string[count],
                    AllMaxMiss = new string[count],
                    AllAvgMiss = new string[count],
                    AllTimes = new string[count]
                };
                foreach (var dbItem in dbItemList)
                {
                    var twoInfoList = dbItemList.Where(R => R.Term <= dbItem.Term).OrderByDescending(O => O.Term).Take(2).ToList();

                    FCNY36X7Info info = null;
                    info = twoInfoList[0];

                    if (twoInfoList.Count == 2)
                    {
                        entity = ResultEntity;//变量存储上一个走势图数据
                        //if(j==0)
                        //    entity
                    }

                    bool yes = true;
                    var sb = new StringBuilder(20000);
                    sb.Append("<tr>");
                    for (var i = 0; i < count; i++)
                    {
                        chartItems[i].InitMissData(entity, i);
                        //计算项值及遗漏计算
                        yes = yes && chartItems[i].SetItemValue(info);
                        if (yes)
                        {
                            //结果集赋值
                            ResultEntity.LocalMiss[i] = chartItems[i].GetMissData(MissDataType.LocalMiss);
                            ResultEntity.LastMiss[i] = chartItems[i].GetMissData(MissDataType.LastMiss);
                            ResultEntity.AllMaxMiss[i] = chartItems[i].GetMissData(MissDataType.AllMaxMiss);
                            ResultEntity.AllAvgMiss[i] = chartItems[i].GetMissData(MissDataType.AllAvgMiss);
                            ResultEntity.AllTimes[i] = chartItems[i].GetMissData(MissDataType.AllTimes);

                            sb.Append(chartItems[i].GetFomartString("<td {0}>{1}</td>"));
                        }
                    }
                    sb.Append("</tr>");
                    if (null != entity)
                        ResultEntity.RecordCount = entity.RecordCount + 1;
                    else
                        ResultEntity.RecordCount = 1;
                    ResultEntity.Term = dbItem.Term;
                    ResultEntity.HtmlData = sb.ToString();

                    Console.WriteLine(string.Format("为{0}期开奖数据生成{1}成功", dbItem.Term, config.Name));
                    trendChartDataList.Add(new TrendChartData()
                    {
                        Id = ResultEntity.Id,
                        ChartId = ResultEntity.ChartId,
                        Term = ResultEntity.Term,
                        AllMaxMiss = ResultEntity.AllMaxMiss.Clone() as string[],
                        AllTimes = ResultEntity.AllTimes.Clone() as string[],
                        RecordCount = ResultEntity.RecordCount,
                        AllAvgMiss = ResultEntity.AllAvgMiss.Clone() as string[],
                        LastMiss = ResultEntity.LastMiss.Clone() as string[],
                        LocalMiss = ResultEntity.LocalMiss.Clone() as string[],
                        HtmlData = ResultEntity.HtmlData,
                        ChartType = ResultEntity.ChartType,
                        Addtime = ResultEntity.Addtime
                    });
                }
                Console.WriteLine();
            }
            if (helper.SaveTrendChartList(SCCLottery.GD36X7TrendChart, trendChartDataList))
                Log.Info(typeof(TrendChartHelper), "------<<生成广东36选7走势图数据成功！>>------");
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("------<<生成广东36选7走势图数据失败！>>------");
                Console.ResetColor();
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// 生成湖北30选5走势图
        /// </summary>
        public static void GenerateHuBei30X5TrendChart(LogHelper Log)
        {
            ITrendChart helper = new TrendChartServices();
            IDTOpenCode service = new DTOpenCodeServices();
            var HUB30X5 = ConfigHelper.GetConfigValue<int>("HUB30X5");
            var configList = helper.GetTrendChartConfig(HUB30X5);
            if (configList.Count == 0)
            {
                Console.WriteLine("未找到有效走势图信息");
                return;
            }
            var trendChartItemList = helper.GetTrendChartItem(configList.Select(c => c.Id).ToList());
            foreach (var itemInfo in trendChartItemList)
            {
                itemInfo.Cid = HUB30X5;
            }
            var dbItemList = service.GetHuBei30X5ListOpenCode();
            if (dbItemList.Count == 0)
            {
                Console.WriteLine("未找到有效开奖数据");
                return;
            }
            var chartCssConfigs = helper.GetChartCssConfigs();
            List<TrendChartData> trendChartDataList = new List<TrendChartData>();
            foreach (var config in configList)//基本走势图，和值走势图，手机版
            {
                var chartId = config.Id;
                var trendChartItems = trendChartItemList.Where(S => S.ChartId == chartId).OrderBy(S => S.OrderBy).ToList();
                if (trendChartItems.Count == 0)
                {
                    Console.WriteLine("未找到该走势图的显示项信息");
                    return;
                }
                Console.WriteLine(string.Format("------生成{0}------", config.Name));
                var chartItems = new TrendChartItemHelper<FCHB30X5Info>().InitTrendChartItem(chartCssConfigs, trendChartItems);
                var count = trendChartItems.Count;
                //var j = 0;
                TrendChartData entity = null;
                var ResultEntity = new TrendChartData
                {
                    ChartId = chartId,
                    Term = 0,
                    ChartType = TrendChartType.PC,
                    LocalMiss = new string[count],
                    LastMiss = new string[count],
                    AllMaxMiss = new string[count],
                    AllAvgMiss = new string[count],
                    AllTimes = new string[count]
                };
                foreach (var dbItem in dbItemList)
                {
                    var twoInfoList = dbItemList.Where(R => R.Term <= dbItem.Term).OrderByDescending(O => O.Term).Take(2).ToList();

                    FCHB30X5Info info = null;
                    info = twoInfoList[0];

                    if (twoInfoList.Count == 2)
                    {
                        entity = ResultEntity;//变量存储上一个走势图数据
                        //if(j==0)
                        //    entity
                    }

                    bool yes = true;
                    var sb = new StringBuilder(20000);
                    sb.Append("<tr>");
                    for (var i = 0; i < count; i++)
                    {
                        chartItems[i].InitMissData(entity, i);
                        //计算项值及遗漏计算
                        yes = yes && chartItems[i].SetItemValue(info);
                        if (yes)
                        {
                            //结果集赋值
                            ResultEntity.LocalMiss[i] = chartItems[i].GetMissData(MissDataType.LocalMiss);
                            ResultEntity.LastMiss[i] = chartItems[i].GetMissData(MissDataType.LastMiss);
                            ResultEntity.AllMaxMiss[i] = chartItems[i].GetMissData(MissDataType.AllMaxMiss);
                            ResultEntity.AllAvgMiss[i] = chartItems[i].GetMissData(MissDataType.AllAvgMiss);
                            ResultEntity.AllTimes[i] = chartItems[i].GetMissData(MissDataType.AllTimes);

                            sb.Append(chartItems[i].GetFomartString("<td {0}>{1}</td>"));
                        }
                    }
                    sb.Append("</tr>");
                    if (null != entity)
                        ResultEntity.RecordCount = entity.RecordCount + 1;
                    else
                        ResultEntity.RecordCount = 1;
                    ResultEntity.Term = dbItem.Term;
                    ResultEntity.HtmlData = sb.ToString();

                    Console.WriteLine(string.Format("为{0}期开奖数据生成{1}成功", dbItem.Term, config.Name));
                    trendChartDataList.Add(new TrendChartData()
                    {
                        Id = ResultEntity.Id,
                        ChartId = ResultEntity.ChartId,
                        Term = ResultEntity.Term,
                        AllMaxMiss = ResultEntity.AllMaxMiss.Clone() as string[],
                        AllTimes = ResultEntity.AllTimes.Clone() as string[],
                        RecordCount = ResultEntity.RecordCount,
                        AllAvgMiss = ResultEntity.AllAvgMiss.Clone() as string[],
                        LastMiss = ResultEntity.LastMiss.Clone() as string[],
                        LocalMiss = ResultEntity.LocalMiss.Clone() as string[],
                        HtmlData = ResultEntity.HtmlData,
                        ChartType = ResultEntity.ChartType,
                        Addtime = ResultEntity.Addtime
                    });
                }
                Console.WriteLine();
            }
            if (helper.SaveTrendChartList(SCCLottery.HuBei30X5TrendChart, trendChartDataList))
                Log.Info(typeof(TrendChartHelper), "------<<生成湖北30选5走势图数据成功！>>------");
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("------<<生成湖北30选5走势图数据失败！>>------");
                Console.ResetColor();
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// 生成新疆35选7走势图
        /// </summary>
        /// <param name="Log">日志实例</param>
        public static void GenerateXJ35X7TrendChart(LogHelper Log)
        {
            ITrendChart helper = new TrendChartServices();
            IDTOpenCode service = new DTOpenCodeServices();
            var XJ35X7 = ConfigHelper.GetConfigValue<int>("XJ35X7");
            var configList = helper.GetTrendChartConfig(XJ35X7);
            if (configList.Count == 0)
            {
                Console.WriteLine("未找到有效走势图信息");
                return;
            }
            var trendChartItemList = helper.GetTrendChartItem(configList.Select(c => c.Id).ToList());
            foreach (var itemInfo in trendChartItemList)
            {
                itemInfo.Cid = XJ35X7;
            }
            var dbItemList = service.GetXJ35X7ListOpenCode();
            if (dbItemList.Count == 0)
            {
                Console.WriteLine("未找到有效开奖数据");
                return;
            }
            var chartCssConfigs = helper.GetChartCssConfigs();
            List<TrendChartData> trendChartDataList = new List<TrendChartData>();
            foreach (var config in configList)//基本走势图，和值走势图，手机版
            {
                var chartId = config.Id;
                var trendChartItems = trendChartItemList.Where(S => S.ChartId == chartId).OrderBy(S => S.OrderBy).ToList();
                if (trendChartItems.Count == 0)
                {
                    Console.WriteLine("未找到该走势图的显示项信息");
                    return;
                }
                Console.WriteLine(string.Format("------生成{0}------", config.Name));
                var chartItems = new TrendChartItemHelper<FCXJ35X7Info>().InitTrendChartItem(chartCssConfigs, trendChartItems);
                var count = trendChartItems.Count;
                //var j = 0;
                TrendChartData entity = null;
                var ResultEntity = new TrendChartData
                {
                    ChartId = chartId,
                    Term = 0,
                    ChartType = TrendChartType.PC,
                    LocalMiss = new string[count],
                    LastMiss = new string[count],
                    AllMaxMiss = new string[count],
                    AllAvgMiss = new string[count],
                    AllTimes = new string[count]
                };
                foreach (var dbItem in dbItemList)
                {
                    var twoInfoList = dbItemList.Where(R => R.Term <= dbItem.Term).OrderByDescending(O => O.Term).Take(2).ToList();

                    FCXJ35X7Info info = null;
                    info = twoInfoList[0];

                    if (twoInfoList.Count == 2)
                    {
                        entity = ResultEntity;//变量存储上一个走势图数据
                        //if(j==0)
                        //    entity
                    }

                    bool yes = true;
                    var sb = new StringBuilder(20000);
                    sb.Append("<tr>");
                    for (var i = 0; i < count; i++)
                    {
                        chartItems[i].InitMissData(entity, i);
                        //计算项值及遗漏计算
                        yes = yes && chartItems[i].SetItemValue(info);
                        if (yes)
                        {
                            //结果集赋值
                            ResultEntity.LocalMiss[i] = chartItems[i].GetMissData(MissDataType.LocalMiss);
                            ResultEntity.LastMiss[i] = chartItems[i].GetMissData(MissDataType.LastMiss);
                            ResultEntity.AllMaxMiss[i] = chartItems[i].GetMissData(MissDataType.AllMaxMiss);
                            ResultEntity.AllAvgMiss[i] = chartItems[i].GetMissData(MissDataType.AllAvgMiss);
                            ResultEntity.AllTimes[i] = chartItems[i].GetMissData(MissDataType.AllTimes);

                            sb.Append(chartItems[i].GetFomartString("<td {0}>{1}</td>"));
                        }
                    }
                    sb.Append("</tr>");
                    if (null != entity)
                        ResultEntity.RecordCount = entity.RecordCount + 1;
                    else
                        ResultEntity.RecordCount = 1;
                    ResultEntity.Term = dbItem.Term;
                    ResultEntity.HtmlData = sb.ToString();

                    Console.WriteLine(string.Format("为{0}期开奖数据生成{1}成功", dbItem.Term, config.Name));
                    trendChartDataList.Add(new TrendChartData()
                    {
                        Id = ResultEntity.Id,
                        ChartId = ResultEntity.ChartId,
                        Term = ResultEntity.Term,
                        AllMaxMiss = ResultEntity.AllMaxMiss.Clone() as string[],
                        AllTimes = ResultEntity.AllTimes.Clone() as string[],
                        RecordCount = ResultEntity.RecordCount,
                        AllAvgMiss = ResultEntity.AllAvgMiss.Clone() as string[],
                        LastMiss = ResultEntity.LastMiss.Clone() as string[],
                        LocalMiss = ResultEntity.LocalMiss.Clone() as string[],
                        HtmlData = ResultEntity.HtmlData,
                        ChartType = ResultEntity.ChartType,
                        Addtime = ResultEntity.Addtime
                    });
                }
                Console.WriteLine();
            }
            if (helper.SaveTrendChartList(SCCLottery.XJ35X7TrendChart, trendChartDataList))
                Log.Info(typeof(TrendChartHelper), "------<<生成新疆35选7走势图数据成功！>>------");
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("------<<生成新疆35选7走势图数据失败！>>------");
                Console.ResetColor();
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// 生成江苏体彩七位数走势图
        /// </summary>
        /// <param name="Log">日志实例</param>
        public static void GenerateJSTC7WSTrendChart(LogHelper Log)
        {
            ITrendChart helper = new TrendChartServices();
            IDTOpenCode service = new DTOpenCodeServices();
            var JS7WS = ConfigHelper.GetConfigValue<int>("JS7WS");
            var configList = helper.GetTrendChartConfig(JS7WS);
            if (configList.Count == 0)
            {
                Console.WriteLine("未找到有效走势图信息");
                return;
            }
            var trendChartItemList = helper.GetTrendChartItem(configList.Select(c => c.Id).ToList());
            foreach (var itemInfo in trendChartItemList)
            {
                itemInfo.Cid = JS7WS;
            }
            var dbItemList = service.GetJS7WSListOpenCode();
            if (dbItemList.Count == 0)
            {
                Console.WriteLine("未找到有效开奖数据");
                return;
            }
            var chartCssConfigs = helper.GetChartCssConfigs();
            List<TrendChartData> trendChartDataList = new List<TrendChartData>();
            foreach (var config in configList)//基本走势图，和值走势图，手机版
            {
                var chartId = config.Id;
                var trendChartItems = trendChartItemList.Where(S => S.ChartId == chartId).OrderBy(S => S.OrderBy).ToList();
                if (trendChartItems.Count == 0)
                {
                    Console.WriteLine("未找到该走势图的显示项信息");
                    return;
                }
                Console.WriteLine(string.Format("------生成{0}------", config.Name));
                var chartItems = new TrendChartItemHelper<TCJS7WSInfo>().InitTrendChartItem(chartCssConfigs, trendChartItems);
                var count = trendChartItems.Count;
                //var j = 0;
                TrendChartData entity = null;
                var ResultEntity = new TrendChartData
                {
                    ChartId = chartId,
                    Term = 0,
                    ChartType = TrendChartType.PC,
                    LocalMiss = new string[count],
                    LastMiss = new string[count],
                    AllMaxMiss = new string[count],
                    AllAvgMiss = new string[count],
                    AllTimes = new string[count]
                };
                foreach (var dbItem in dbItemList)
                {
                    var twoInfoList = dbItemList.Where(R => R.Term <= dbItem.Term).OrderByDescending(O => O.Term).Take(2).ToList();

                    TCJS7WSInfo info = null;
                    info = twoInfoList[0];

                    if (twoInfoList.Count == 2)
                    {
                        entity = ResultEntity;//变量存储上一个走势图数据
                        //if(j==0)
                        //    entity
                    }

                    bool yes = true;
                    var sb = new StringBuilder(20000);
                    sb.Append("<tr>");
                    for (var i = 0; i < count; i++)
                    {
                        chartItems[i].InitMissData(entity, i);
                        //计算项值及遗漏计算
                        yes = yes && chartItems[i].SetItemValue(info);
                        if (yes)
                        {
                            //结果集赋值
                            ResultEntity.LocalMiss[i] = chartItems[i].GetMissData(MissDataType.LocalMiss);
                            ResultEntity.LastMiss[i] = chartItems[i].GetMissData(MissDataType.LastMiss);
                            ResultEntity.AllMaxMiss[i] = chartItems[i].GetMissData(MissDataType.AllMaxMiss);
                            ResultEntity.AllAvgMiss[i] = chartItems[i].GetMissData(MissDataType.AllAvgMiss);
                            ResultEntity.AllTimes[i] = chartItems[i].GetMissData(MissDataType.AllTimes);

                            sb.Append(chartItems[i].GetFomartString("<td {0}>{1}</td>"));
                        }
                    }
                    sb.Append("</tr>");
                    if (null != entity)
                        ResultEntity.RecordCount = entity.RecordCount + 1;
                    else
                        ResultEntity.RecordCount = 1;
                    ResultEntity.Term = dbItem.Term;
                    ResultEntity.HtmlData = sb.ToString();

                    Console.WriteLine(string.Format("为{0}期开奖数据生成{1}成功", dbItem.Term, config.Name));
                    trendChartDataList.Add(new TrendChartData()
                    {
                        Id = ResultEntity.Id,
                        ChartId = ResultEntity.ChartId,
                        Term = ResultEntity.Term,
                        AllMaxMiss = ResultEntity.AllMaxMiss.Clone() as string[],
                        AllTimes = ResultEntity.AllTimes.Clone() as string[],
                        RecordCount = ResultEntity.RecordCount,
                        AllAvgMiss = ResultEntity.AllAvgMiss.Clone() as string[],
                        LastMiss = ResultEntity.LastMiss.Clone() as string[],
                        LocalMiss = ResultEntity.LocalMiss.Clone() as string[],
                        HtmlData = ResultEntity.HtmlData,
                        ChartType = ResultEntity.ChartType,
                        Addtime = ResultEntity.Addtime
                    });
                }
                Console.WriteLine();
            }
            if (helper.SaveTrendChartList(SCCLottery.JSTC7WSTrendChart, trendChartDataList))
                Log.Info(typeof(TrendChartHelper), "------<<生成江苏体彩七位数走势图数据成功！>>------");
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("------<<生成江苏体彩七位数走势图数据失败！>>------");
                Console.ResetColor();
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// 生成浙江体彩6+1走势图
        /// </summary>
        /// <param name="Log">日志实例</param>
        public static void GenerateZJTC6J1TrendChart(LogHelper Log)
        {
            ITrendChart helper = new TrendChartServices();
            IDTOpenCode service = new DTOpenCodeServices();
            var ZJ6J1 = ConfigHelper.GetConfigValue<int>("ZJ6J1");
            var configList = helper.GetTrendChartConfig(ZJ6J1);
            if (configList.Count == 0)
            {
                Console.WriteLine("未找到有效走势图信息");
                return;
            }
            var trendChartItemList = helper.GetTrendChartItem(configList.Select(c => c.Id).ToList());
            foreach (var itemInfo in trendChartItemList)
            {
                itemInfo.Cid = ZJ6J1;
            }
            var dbItemList = service.GetZJ6J1ListOpenCode();
            if (dbItemList.Count == 0)
            {
                Console.WriteLine("未找到有效开奖数据");
                return;
            }
            var chartCssConfigs = helper.GetChartCssConfigs();
            List<TrendChartData> trendChartDataList = new List<TrendChartData>();
            foreach (var config in configList)//基本走势图，和值走势图，手机版
            {
                var chartId = config.Id;
                var trendChartItems = trendChartItemList.Where(S => S.ChartId == chartId).OrderBy(S => S.OrderBy).ToList();
                if (trendChartItems.Count == 0)
                {
                    Console.WriteLine("未找到该走势图的显示项信息");
                    return;
                }
                Console.WriteLine(string.Format("------生成{0}------", config.Name));
                var chartItems = new TrendChartItemHelper<TCZJ6J1Info>().InitTrendChartItem(chartCssConfigs, trendChartItems);
                var count = trendChartItems.Count;
                //var j = 0;
                TrendChartData entity = null;
                var ResultEntity = new TrendChartData
                {
                    ChartId = chartId,
                    Term = 0,
                    ChartType = TrendChartType.PC,
                    LocalMiss = new string[count],
                    LastMiss = new string[count],
                    AllMaxMiss = new string[count],
                    AllAvgMiss = new string[count],
                    AllTimes = new string[count]
                };
                foreach (var dbItem in dbItemList)
                {
                    var twoInfoList = dbItemList.Where(R => R.Term <= dbItem.Term).OrderByDescending(O => O.Term).Take(2).ToList();

                    TCZJ6J1Info info = null;
                    info = twoInfoList[0];

                    if (twoInfoList.Count == 2)
                    {
                        entity = ResultEntity;//变量存储上一个走势图数据
                        //if(j==0)
                        //    entity
                    }

                    bool yes = true;
                    var sb = new StringBuilder(20000);
                    sb.Append("<tr>");
                    for (var i = 0; i < count; i++)
                    {
                        chartItems[i].InitMissData(entity, i);
                        //计算项值及遗漏计算
                        yes = yes && chartItems[i].SetItemValue(info);
                        if (yes)
                        {
                            //结果集赋值
                            ResultEntity.LocalMiss[i] = chartItems[i].GetMissData(MissDataType.LocalMiss);
                            ResultEntity.LastMiss[i] = chartItems[i].GetMissData(MissDataType.LastMiss);
                            ResultEntity.AllMaxMiss[i] = chartItems[i].GetMissData(MissDataType.AllMaxMiss);
                            ResultEntity.AllAvgMiss[i] = chartItems[i].GetMissData(MissDataType.AllAvgMiss);
                            ResultEntity.AllTimes[i] = chartItems[i].GetMissData(MissDataType.AllTimes);

                            sb.Append(chartItems[i].GetFomartString("<td {0}>{1}</td>"));
                        }
                    }
                    sb.Append("</tr>");
                    if (null != entity)
                        ResultEntity.RecordCount = entity.RecordCount + 1;
                    else
                        ResultEntity.RecordCount = 1;
                    ResultEntity.Term = dbItem.Term;
                    ResultEntity.HtmlData = sb.ToString();

                    Console.WriteLine(string.Format("为{0}期开奖数据生成{1}成功", dbItem.Term, config.Name));
                    trendChartDataList.Add(new TrendChartData()
                    {
                        Id = ResultEntity.Id,
                        ChartId = ResultEntity.ChartId,
                        Term = ResultEntity.Term,
                        AllMaxMiss = ResultEntity.AllMaxMiss.Clone() as string[],
                        AllTimes = ResultEntity.AllTimes.Clone() as string[],
                        RecordCount = ResultEntity.RecordCount,
                        AllAvgMiss = ResultEntity.AllAvgMiss.Clone() as string[],
                        LastMiss = ResultEntity.LastMiss.Clone() as string[],
                        LocalMiss = ResultEntity.LocalMiss.Clone() as string[],
                        HtmlData = ResultEntity.HtmlData,
                        ChartType = ResultEntity.ChartType,
                        Addtime = ResultEntity.Addtime
                    });
                }
                Console.WriteLine();
            }
            if (helper.SaveTrendChartList(SCCLottery.ZJTC6J1TrendChart, trendChartDataList))
                Log.Info(typeof(TrendChartHelper), "------<<生成浙江体彩6+1走势图数据成功！>>------");
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("------<<生成浙江体彩6+1走势图数据失败！>>------");
                Console.ResetColor();
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// 生成福建36选7走势图
        /// </summary>
        /// <param name="Log">日志实例</param>
        public static void GenerateFJ36X7TrendChart(LogHelper Log)
        {
            ITrendChart helper = new TrendChartServices();
            IDTOpenCode service = new DTOpenCodeServices();
            var FJ36X7 = ConfigHelper.GetConfigValue<int>("FJ36X7");
            var configList = helper.GetTrendChartConfig(FJ36X7);
            if (configList.Count == 0)
            {
                Console.WriteLine("未找到有效走势图信息");
                return;
            }
            var trendChartItemList = helper.GetTrendChartItem(configList.Select(c => c.Id).ToList());
            foreach (var itemInfo in trendChartItemList)
            {
                itemInfo.Cid = FJ36X7;
            }
            var dbItemList = service.GetFJ36X7ListOpenCode();
            if (dbItemList.Count == 0)
            {
                Console.WriteLine("未找到有效开奖数据");
                return;
            }
            var chartCssConfigs = helper.GetChartCssConfigs();
            List<TrendChartData> trendChartDataList = new List<TrendChartData>();
            foreach (var config in configList)//基本走势图，和值走势图，手机版
            {
                var chartId = config.Id;
                var trendChartItems = trendChartItemList.Where(S => S.ChartId == chartId).OrderBy(S => S.OrderBy).ToList();
                if (trendChartItems.Count == 0)
                {
                    Console.WriteLine("未找到该走势图的显示项信息");
                    return;
                }
                Console.WriteLine(string.Format("------生成{0}------", config.Name));
                var chartItems = new TrendChartItemHelper<TCFJ36X7Info>().InitTrendChartItem(chartCssConfigs, trendChartItems);
                var count = trendChartItems.Count;
                //var j = 0;
                TrendChartData entity = null;
                var ResultEntity = new TrendChartData
                {
                    ChartId = chartId,
                    Term = 0,
                    ChartType = TrendChartType.PC,
                    LocalMiss = new string[count],
                    LastMiss = new string[count],
                    AllMaxMiss = new string[count],
                    AllAvgMiss = new string[count],
                    AllTimes = new string[count]
                };
                foreach (var dbItem in dbItemList)
                {
                    var twoInfoList = dbItemList.Where(R => R.Term <= dbItem.Term).OrderByDescending(O => O.Term).Take(2).ToList();

                    TCFJ36X7Info info = null;
                    info = twoInfoList[0];

                    if (twoInfoList.Count == 2)
                    {
                        entity = ResultEntity;//变量存储上一个走势图数据
                        //if(j==0)
                        //    entity
                    }

                    bool yes = true;
                    var sb = new StringBuilder(20000);
                    sb.Append("<tr>");
                    for (var i = 0; i < count; i++)
                    {
                        chartItems[i].InitMissData(entity, i);
                        //计算项值及遗漏计算
                        yes = yes && chartItems[i].SetItemValue(info);
                        if (yes)
                        {
                            //结果集赋值
                            ResultEntity.LocalMiss[i] = chartItems[i].GetMissData(MissDataType.LocalMiss);
                            ResultEntity.LastMiss[i] = chartItems[i].GetMissData(MissDataType.LastMiss);
                            ResultEntity.AllMaxMiss[i] = chartItems[i].GetMissData(MissDataType.AllMaxMiss);
                            ResultEntity.AllAvgMiss[i] = chartItems[i].GetMissData(MissDataType.AllAvgMiss);
                            ResultEntity.AllTimes[i] = chartItems[i].GetMissData(MissDataType.AllTimes);

                            sb.Append(chartItems[i].GetFomartString("<td {0}>{1}</td>"));
                        }
                    }
                    sb.Append("</tr>");
                    if (null != entity)
                        ResultEntity.RecordCount = entity.RecordCount + 1;
                    else
                        ResultEntity.RecordCount = 1;
                    ResultEntity.Term = dbItem.Term;
                    ResultEntity.HtmlData = sb.ToString();

                    Console.WriteLine(string.Format("为{0}期开奖数据生成{1}成功", dbItem.Term, config.Name));
                    trendChartDataList.Add(new TrendChartData()
                    {
                        Id = ResultEntity.Id,
                        ChartId = ResultEntity.ChartId,
                        Term = ResultEntity.Term,
                        AllMaxMiss = ResultEntity.AllMaxMiss.Clone() as string[],
                        AllTimes = ResultEntity.AllTimes.Clone() as string[],
                        RecordCount = ResultEntity.RecordCount,
                        AllAvgMiss = ResultEntity.AllAvgMiss.Clone() as string[],
                        LastMiss = ResultEntity.LastMiss.Clone() as string[],
                        LocalMiss = ResultEntity.LocalMiss.Clone() as string[],
                        HtmlData = ResultEntity.HtmlData,
                        ChartType = ResultEntity.ChartType,
                        Addtime = ResultEntity.Addtime
                    });
                }
                Console.WriteLine();
            }
            if (helper.SaveTrendChartList(SCCLottery.FJ36X7TrendChart, trendChartDataList))
                Log.Info(typeof(TrendChartHelper), "------<<生成福建36选7走势图数据成功！>>------");
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("------<<生成福建36选7走势图数据失败！>>------");
                Console.ResetColor();
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// 生成福建31选7走势图
        /// </summary>
        /// <param name="Log">日志实例</param>
        public static void GenerateFJ31X7TrendChart(LogHelper Log)
        {
            ITrendChart helper = new TrendChartServices();
            IDTOpenCode service = new DTOpenCodeServices();
            var FJ31X7 = ConfigHelper.GetConfigValue<int>("FJ31X7");
            var configList = helper.GetTrendChartConfig(FJ31X7);
            if (configList.Count == 0)
            {
                Console.WriteLine("未找到有效走势图信息");
                return;
            }
            var trendChartItemList = helper.GetTrendChartItem(configList.Select(c => c.Id).ToList());
            foreach (var itemInfo in trendChartItemList)
            {
                itemInfo.Cid = FJ31X7;
            }
            var dbItemList = service.GetFJ31X7ListOpenCode();
            if (dbItemList.Count == 0)
            {
                Console.WriteLine("未找到有效开奖数据");
                return;
            }
            var chartCssConfigs = helper.GetChartCssConfigs();
            List<TrendChartData> trendChartDataList = new List<TrendChartData>();
            foreach (var config in configList)//基本走势图，和值走势图，手机版
            {
                var chartId = config.Id;
                var trendChartItems = trendChartItemList.Where(S => S.ChartId == chartId).OrderBy(S => S.OrderBy).ToList();
                if (trendChartItems.Count == 0)
                {
                    Console.WriteLine("未找到该走势图的显示项信息");
                    return;
                }
                Console.WriteLine(string.Format("------生成{0}------", config.Name));
                var chartItems = new TrendChartItemHelper<TCFJ31X7Info>().InitTrendChartItem(chartCssConfigs, trendChartItems);
                var count = trendChartItems.Count;
                //var j = 0;
                TrendChartData entity = null;
                var ResultEntity = new TrendChartData
                {
                    ChartId = chartId,
                    Term = 0,
                    ChartType = TrendChartType.PC,
                    LocalMiss = new string[count],
                    LastMiss = new string[count],
                    AllMaxMiss = new string[count],
                    AllAvgMiss = new string[count],
                    AllTimes = new string[count]
                };
                foreach (var dbItem in dbItemList)
                {
                    var twoInfoList = dbItemList.Where(R => R.Term <= dbItem.Term).OrderByDescending(O => O.Term).Take(2).ToList();

                    TCFJ31X7Info info = null;
                    info = twoInfoList[0];

                    if (twoInfoList.Count == 2)
                    {
                        entity = ResultEntity;//变量存储上一个走势图数据
                        //if(j==0)
                        //    entity
                    }

                    bool yes = true;
                    var sb = new StringBuilder(20000);
                    sb.Append("<tr>");
                    for (var i = 0; i < count; i++)
                    {
                        chartItems[i].InitMissData(entity, i);
                        //计算项值及遗漏计算
                        yes = yes && chartItems[i].SetItemValue(info);
                        if (yes)
                        {
                            //结果集赋值
                            ResultEntity.LocalMiss[i] = chartItems[i].GetMissData(MissDataType.LocalMiss);
                            ResultEntity.LastMiss[i] = chartItems[i].GetMissData(MissDataType.LastMiss);
                            ResultEntity.AllMaxMiss[i] = chartItems[i].GetMissData(MissDataType.AllMaxMiss);
                            ResultEntity.AllAvgMiss[i] = chartItems[i].GetMissData(MissDataType.AllAvgMiss);
                            ResultEntity.AllTimes[i] = chartItems[i].GetMissData(MissDataType.AllTimes);

                            sb.Append(chartItems[i].GetFomartString("<td {0}>{1}</td>"));
                        }
                    }
                    sb.Append("</tr>");
                    if (null != entity)
                        ResultEntity.RecordCount = entity.RecordCount + 1;
                    else
                        ResultEntity.RecordCount = 1;
                    ResultEntity.Term = dbItem.Term;
                    ResultEntity.HtmlData = sb.ToString();

                    Console.WriteLine(string.Format("为{0}期开奖数据生成{1}成功", dbItem.Term, config.Name));
                    trendChartDataList.Add(new TrendChartData()
                    {
                        Id = ResultEntity.Id,
                        ChartId = ResultEntity.ChartId,
                        Term = ResultEntity.Term,
                        AllMaxMiss = ResultEntity.AllMaxMiss.Clone() as string[],
                        AllTimes = ResultEntity.AllTimes.Clone() as string[],
                        RecordCount = ResultEntity.RecordCount,
                        AllAvgMiss = ResultEntity.AllAvgMiss.Clone() as string[],
                        LastMiss = ResultEntity.LastMiss.Clone() as string[],
                        LocalMiss = ResultEntity.LocalMiss.Clone() as string[],
                        HtmlData = ResultEntity.HtmlData,
                        ChartType = ResultEntity.ChartType,
                        Addtime = ResultEntity.Addtime
                    });
                }
                Console.WriteLine();
            }
            if (helper.SaveTrendChartList(SCCLottery.FJ31X7TrendChart, trendChartDataList))
                Log.Info(typeof(TrendChartHelper), "------<<生成福建31选7走势图数据成功！>>------");
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("------<<生成福建31选7走势图数据失败！>>------");
                Console.ResetColor();
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// 生成广东好彩1走势图
        /// </summary>
        /// <param name="Log">日志实例</param>
        public static void GenerateGDHC1TrendChart(LogHelper Log)
        {
            ITrendChart helper = new TrendChartServices();
            IDTOpenCode service = new DTOpenCodeServices();
            var GDHC1 = ConfigHelper.GetConfigValue<int>("GDHC1");
            var configList = helper.GetTrendChartConfig(GDHC1);
            if (configList.Count == 0)
            {
                Console.WriteLine("未找到有效走势图信息");
                return;
            }
            var trendChartItemList = helper.GetTrendChartItem(configList.Select(c => c.Id).ToList());
            foreach (var itemInfo in trendChartItemList)
            {
                itemInfo.Cid = GDHC1;
            }
            var dbItemList = service.GetGDHC1ListOpenCode();
            if (dbItemList.Count == 0)
            {
                Console.WriteLine("未找到有效开奖数据");
                return;
            }
            var chartCssConfigs = helper.GetChartCssConfigs();
            List<TrendChartData> trendChartDataList = new List<TrendChartData>();
            foreach (var config in configList)//基本走势图，和值走势图，手机版
            {
                var chartId = config.Id;
                var trendChartItems = trendChartItemList.Where(S => S.ChartId == chartId).OrderBy(S => S.OrderBy).ToList();
                if (trendChartItems.Count == 0)
                {
                    Console.WriteLine("未找到该走势图的显示项信息");
                    return;
                }
                Console.WriteLine(string.Format("------生成{0}------", config.Name));
                var chartItems = new TrendChartItemHelper<FCGDHC1Info>().InitTrendChartItem(chartCssConfigs, trendChartItems);
                var count = trendChartItems.Count;
                //var j = 0;
                TrendChartData entity = null;
                var ResultEntity = new TrendChartData
                {
                    ChartId = chartId,
                    Term = 0,
                    ChartType = TrendChartType.PC,
                    LocalMiss = new string[count],
                    LastMiss = new string[count],
                    AllMaxMiss = new string[count],
                    AllAvgMiss = new string[count],
                    AllTimes = new string[count]
                };
                foreach (var dbItem in dbItemList)
                {
                    var twoInfoList = dbItemList.Where(R => R.Term <= dbItem.Term).OrderByDescending(O => O.Term).Take(2).ToList();

                    FCGDHC1Info info = null;
                    info = twoInfoList[0];

                    if (twoInfoList.Count == 2)
                    {
                        entity = ResultEntity;//变量存储上一个走势图数据
                        //if(j==0)
                        //    entity
                    }

                    bool yes = true;
                    var sb = new StringBuilder(20000);
                    sb.Append("<tr>");
                    for (var i = 0; i < count; i++)
                    {
                        chartItems[i].InitMissData(entity, i);
                        //计算项值及遗漏计算
                        yes = yes && chartItems[i].SetItemValue(info);
                        if (yes)
                        {
                            //结果集赋值
                            ResultEntity.LocalMiss[i] = chartItems[i].GetMissData(MissDataType.LocalMiss);
                            ResultEntity.LastMiss[i] = chartItems[i].GetMissData(MissDataType.LastMiss);
                            ResultEntity.AllMaxMiss[i] = chartItems[i].GetMissData(MissDataType.AllMaxMiss);
                            ResultEntity.AllAvgMiss[i] = chartItems[i].GetMissData(MissDataType.AllAvgMiss);
                            ResultEntity.AllTimes[i] = chartItems[i].GetMissData(MissDataType.AllTimes);

                            sb.Append(chartItems[i].GetFomartString("<td {0}>{1}</td>"));
                        }
                    }
                    sb.Append("</tr>");
                    if (null != entity)
                        ResultEntity.RecordCount = entity.RecordCount + 1;
                    else
                        ResultEntity.RecordCount = 1;
                    ResultEntity.Term = dbItem.Term;
                    ResultEntity.HtmlData = sb.ToString();

                    Console.WriteLine(string.Format("为{0}期开奖数据生成{1}成功", dbItem.Term, config.Name));
                    trendChartDataList.Add(new TrendChartData()
                    {
                        Id = ResultEntity.Id,
                        ChartId = ResultEntity.ChartId,
                        Term = ResultEntity.Term,
                        AllMaxMiss = ResultEntity.AllMaxMiss.Clone() as string[],
                        AllTimes = ResultEntity.AllTimes.Clone() as string[],
                        RecordCount = ResultEntity.RecordCount,
                        AllAvgMiss = ResultEntity.AllAvgMiss.Clone() as string[],
                        LastMiss = ResultEntity.LastMiss.Clone() as string[],
                        LocalMiss = ResultEntity.LocalMiss.Clone() as string[],
                        HtmlData = ResultEntity.HtmlData,
                        ChartType = ResultEntity.ChartType,
                        Addtime = ResultEntity.Addtime
                    });
                }
                Console.WriteLine();
            }
            if (helper.SaveTrendChartList(SCCLottery.GDHC1TrendChart, trendChartDataList))
                Log.Info(typeof(TrendChartHelper), "------<<生成广东好彩1走势图数据成功！>>------");
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("------<<生成广东好彩1走势图数据失败！>>------");
                Console.ResetColor();
            }
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
