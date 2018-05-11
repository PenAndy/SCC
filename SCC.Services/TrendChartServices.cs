using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using SCC.Interface;
using SCC.Models;
using SCC.Common;

namespace SCC.Services
{
    public class TrendChartServices : BaseServices, ITrendChart
    {
        /// <summary>
        /// 获取彩种走势图配置信息
        /// </summary>
        /// <param name="LotteryId">彩种编号</param>
        /// <returns></returns>
        public List<DT_TrendChart> GetTrendChartConfig(int LotteryId)
        {
            List<DT_TrendChart> result = new List<DT_TrendChart>();
            var param = new SqlParameter[]{
                new SqlParameter("@Cid",LotteryId)
            };
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, TrendChartConfigSql, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = LoadDataList<DT_TrendChart>(ds.Tables[0]);
            }
            return result;
        }

        /// <summary>
        /// 获取彩种走势图显示项信息
        /// </summary>
        /// <param name="TrendChartIds">配置项编号列表</param>
        /// <returns></returns>
        public List<TrendChartItemInfo> GetTrendChartItem(List<int> TrendChartIds)
        {
            List<TrendChartItemInfo> result = new List<TrendChartItemInfo>();
            var querySql = string.Empty;
            if (TrendChartIds.Count == 0)
                return result;
            else if (TrendChartIds.Count == 1)
                querySql = string.Format("{0} = {1}", TrendChartItemSql, TrendChartIds[0]);
            else
                querySql = string.Format("{0} in ({1})", TrendChartItemSql, string.Join(",", TrendChartIds));
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, querySql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                List<DT_TrendChartItem> items = LoadDataList<DT_TrendChartItem>(ds.Tables[0]);
                TrendChartItemInfo chartItem = null;
                foreach (var dbItem in items)
                {
                    chartItem = new TrendChartItemInfo();
                    chartItem.Id = dbItem.Id;
                    chartItem.ChartId = dbItem.ChartId;
                    chartItem.ChartType = (TrendChartType)Enum.Parse(typeof(TrendChartType), dbItem.ChartType.ToString());
                    chartItem.ClassName = (ChartItemClassName)Enum.Parse(typeof(ChartItemClassName), dbItem.ClassName.ToString());
                    chartItem.ChartItemName = dbItem.ChartItemName;
                    chartItem.Cycle = dbItem.Cycle;
                    chartItem.ItemMinValue = dbItem.ItemMinValue.HasValue ? dbItem.ItemMinValue.Value : 0;
                    chartItem.ItemMaxValue = dbItem.ItemMaxValue.HasValue ? dbItem.ItemMaxValue.Value : 0;
                    chartItem.SplitNumberOfDX = dbItem.SplitNumberOfDX.HasValue ? dbItem.SplitNumberOfDX.Value : 0;
                    chartItem.ItemCount = dbItem.ItemCount;
                    chartItem.ItemString = dbItem.ItemString.Split(',');
                    chartItem.IndexStart = dbItem.IndexStart.HasValue ? dbItem.IndexStart.Value : 0;
                    chartItem.IndexEnd = dbItem.IndexEnd.HasValue ? dbItem.IndexEnd.Value : 0;
                    chartItem.DrawLine = dbItem.DrawLine;
                    chartItem.FuntionType = (ChartItemType)Enum.Parse(typeof(ChartItemType), dbItem.FuntionType.ToString());
                    chartItem.ChartCssId = dbItem.ChartCssId;
                    chartItem.OrderBy = dbItem.OrderBy.HasValue ? dbItem.OrderBy.Value : 0;
                    result.Add(chartItem);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取所有的走势图样式配置信息
        /// </summary>
        /// <returns></returns>
        public List<ChartCssConfigInfo> GetChartCssConfigs()
        {
            List<ChartCssConfigInfo> result = new List<ChartCssConfigInfo>();
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, TrendChartCssSql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = LoadDataList<ChartCssConfigInfo>(ds.Tables[0]);
            }
            return result;
        }

        /// <summary>
        /// 保存生成的走势图数据
        /// </summary>
        /// <param name="Lottery">彩种</param>
        /// <param name="ChartDatas">走势图数据</param>
        /// <returns></returns>
        public bool SaveTrendChartList(SCCLottery Lottery, List<TrendChartData> ChartDatas)
        {
            try
            {
                SqlParameter[] param = null;
                foreach (var data in ChartDatas)
                {
                    param = new SqlParameter[]{
                        new SqlParameter("@ChartId",data.ChartId),
                        new SqlParameter("@Term",data.Term),
                        new SqlParameter("@RecordCount",data.RecordCount),
                        new SqlParameter("@AllMaxMiss",data.AllMaxMiss.ArrayToString()),
                        new SqlParameter("@AllTimes",data.AllTimes.ArrayToString()),
                        new SqlParameter("@AllAvgMiss",data.AllAvgMiss.ArrayToString()),
                        new SqlParameter("@LastMiss",data.LastMiss.ArrayToString()),
                        new SqlParameter("@LocalMiss",data.LocalMiss.ArrayToString()),
                        new SqlParameter("@HtmlData",data.HtmlData),
                        new SqlParameter("@ChartType",data.ChartType)
                    };
                    SqlHelper.ExecuteNonQuery(CommandType.Text, string.Format(SaveTrendChartSql, EnumHelper.GetSCCLotteryTableName(Lottery)), param);
                }
                return true;
            }
            catch { }
            return false;
        }

        #region SQL语句
        /// <summary>
        /// 获取走势图配置信息的SQL语句
        /// </summary>
        private static string TrendChartConfigSql = @"SELECT * FROM DT_TrendChart WHERE Cid = @Cid";

        /// <summary>
        /// 获取走势图项配置信息的SQL语句
        /// </summary>
        private static string TrendChartItemSql = @"SELECT * FROM DT_TrendChartItem WHERE ChartId";

        /// <summary>
        /// 获取走势图样式配置信息的SQL语句
        /// </summary>
        private static string TrendChartCssSql = @"SELECT * FROM DT_ChartCssConfig";
        /// <summary>
        /// 保存走势图数据的SQL语句
        /// </summary>
        private static string SaveTrendChartSql = @"IF NOT EXISTS(SELECT TOP 1 1 FROM {0} WHERE ChartId = @ChartId AND Term = @Term)
                                                    BEGIN
	                                                    INSERT INTO {0}(ChartId,Term,RecordCount,AllMaxMiss,AllTimes,AllAvgMiss,LastMiss,LocalMiss,HtmlData,ChartType,Addtime)
	                                                    SELECT @ChartId,@Term,@RecordCount,@AllMaxMiss,@AllTimes,@AllAvgMiss,@LastMiss,@LocalMiss,@HtmlData,@ChartType,GETDATE()
                                                    END
                                                    ELSE
                                                    BEGIN
	                                                    UPDATE {0}
	                                                    SET RecordCount = @RecordCount,
		                                                    AllMaxMiss = @AllMaxMiss,
		                                                    AllTimes = @AllTimes,
		                                                    AllAvgMiss = @AllAvgMiss,
		                                                    LastMiss = @LastMiss,
		                                                    LocalMiss = @LocalMiss,
		                                                    HtmlData = @HtmlData,
		                                                    ChartType = @ChartType
	                                                    WHERE ChartId = @ChartId AND Term = @Term
                                                    END";
        #endregion
    }
}
