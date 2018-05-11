using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SCC.Models;

namespace SCC.Interface
{
    /// <summary>
    /// 走势图相关接口
    /// </summary>
    public interface ITrendChart
    {
        /// <summary>
        /// 获取彩种走势图配置信息
        /// </summary>
        /// <param name="LotteryId">彩种编号</param>
        /// <returns></returns>
        List<DT_TrendChart> GetTrendChartConfig(int LotteryId);

        /// <summary>
        /// 获取彩种走势图显示项信息
        /// </summary>
        /// <param name="TrendChartIds">配置项编号列表</param>
        /// <returns></returns>
        List<TrendChartItemInfo> GetTrendChartItem(List<int> TrendChartIds);

        /// <summary>
        /// 获取所有的走势图样式配置信息
        /// </summary>
        /// <returns></returns>
        List<ChartCssConfigInfo> GetChartCssConfigs();

        /// <summary>
        /// 保存生成的走势图数据
        /// </summary>
        /// <param name="Lottery">彩种</param>
        /// <param name="ChartDatas">走势图数据</param>
        /// <returns></returns>
        bool SaveTrendChartList(SCCLottery Lottery, List<TrendChartData> ChartDatas);
    }
}
