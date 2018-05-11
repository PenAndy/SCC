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
    /// <summary>
    /// 开奖21个球号的彩种数据服务
    /// </summary>
    public class Open21CodeServices : BaseServices, IOpen21Code
    {
        /// <summary>
        /// 获取最新一条记录
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <returns></returns>
        public OpenCode21Model GetLastItem(SCCLottery lottery)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(LastItemSql, TableName);
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, sqlString);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var result = LoadData<OpenCode21Model>(ds.Tables[0].Rows[0]);
                return result;
            }
            return null;
        }
        /// <summary>
        /// 获取开奖列表
        /// 
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <returns></returns>
        public List<OpenCode21Model> GetListIn(SCCLottery lottery, bool IsToday)
        {
            List<string> result = new List<string>();
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(QueryListSQL, TableName, IsToday ? CommonHelper.SCCSysDateTime.ToString("yyyyMMdd") : CommonHelper.SCCSysDateTime.AddDays(-1).ToString("yyyyMMdd"));
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, sqlString);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var data = LoadDataList<OpenCode21Model>(ds.Tables[0]);
                return data;
            }
            else
            {
                return null;
            }

        }
        /// <summary>
        /// 获取前一天开奖列表
        /// 
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <returns></returns>
        public List<OpenCode21Model> GetYesterdayList(SCCLottery lottery)
        {
            List<string> result = new List<string>();
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(YesterdayListSql, TableName, CommonHelper.SCCSysDateTime.AddDays(-1).ToString("yyyyMMdd"));
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, sqlString);
            long tempQiHao = 0;
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var data = LoadDataList<OpenCode21Model>(ds.Tables[0]);
                return data;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// 获取前一天失败列表
        /// 期号格式形如YYMMDDQQQ
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <param name="Terms">当前彩种当天期数列表</param>
        /// <returns></returns>
        public List<string> GetYesterdayFailQQQList(SCCLottery lottery, List<string> Terms)
        {
            List<string> result = new List<string>();
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(YesterdayListSql, TableName, CommonHelper.SCCSysDateTime.AddDays(-1).ToString("yyyyMMdd"));
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, sqlString);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var data = LoadDataList<OpenCode21Model>(ds.Tables[0]);
                OpenCode21Model item = null;
                foreach (var term in Terms)
                {
                    item = data.Where(R => R.Term == Convert.ToInt64(term)).FirstOrDefault();
                    if (item == null)
                        result.Add(term);
                }
                return result;
            }
            else
            {
                return Terms;
            }
        }

        /// <summary>
        /// 新增彩种开奖数据
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <param name="model">开奖数据模型</param>
        /// <returns></returns>
        public bool AddOpen21Code(SCCLottery lottery, OpenCode21Model model)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(AddItemSql, TableName);
            var param = new SqlParameter[]{
                new SqlParameter("@Term",model.Term),
                new SqlParameter("@OpenCode1",model.OpenCode1),
                new SqlParameter("@OpenCode2",model.OpenCode2),
                new SqlParameter("@OpenCode3",model.OpenCode3),
                new SqlParameter("@OpenCode4",model.OpenCode4),
                new SqlParameter("@OpenCode5",model.OpenCode5),
                new SqlParameter("@OpenCode6",model.OpenCode6),
                new SqlParameter("@OpenCode7",model.OpenCode7),
                new SqlParameter("@OpenCode8",model.OpenCode8),
                new SqlParameter("@OpenCode9",model.OpenCode9),
                new SqlParameter("@OpenCode10",model.OpenCode10),
                new SqlParameter("@OpenCode11",model.OpenCode11),
                new SqlParameter("@OpenCode12",model.OpenCode12),
                new SqlParameter("@OpenCode13",model.OpenCode13),
                new SqlParameter("@OpenCode14",model.OpenCode14),
                new SqlParameter("@OpenCode15",model.OpenCode15),
                new SqlParameter("@OpenCode16",model.OpenCode16),
                new SqlParameter("@OpenCode17",model.OpenCode17),
                new SqlParameter("@OpenCode18",model.OpenCode18),
                new SqlParameter("@OpenCode19",model.OpenCode19),
                new SqlParameter("@OpenCode20",model.OpenCode20),
                new SqlParameter("@OpenCode21",model.OpenCode21),
                new SqlParameter("@OpenTime",model.OpenTime),
                new SqlParameter("@ID",Guid.NewGuid().ToString().Replace("-", ""))
            };
            var result = SqlHelper.ExecuteNonQuery(CommandType.Text, sqlString, param);
            return result > 0;
        }


        #region Sql语句
        /// <summary>
        /// 获取最新一条记录的Sql语句
        /// </summary>
        private static string LastItemSql = @"SELECT TOP 1 * FROM {0} ORDER BY Term DESC";
        /// <summary>
        /// 获取前一天列表的Sql语句
        /// </summary>
        private static string YesterdayListSql = @"SELECT * FROM {0} 
                                                WHERE CONVERT(varchar(10),OpenTime,112)={1}
                                                ORDER BY OpenTime DESC";
        /// <summary>
        /// 新增开奖数据的Sql语句
        /// </summary>
        private static string AddItemSql = @"IF NOT EXISTS(SELECT TOP 1 1 FROM {0} WHERE Term = @Term)
                                            BEGIN
	                                            INSERT INTO {0}(Term,OpenCode1,OpenCode2,OpenCode3,OpenCode4,OpenCode5,OpenCode6,OpenCode7,OpenCode8,OpenCode9,OpenCode10,OpenCode11,OpenCode12,OpenCode13,OpenCode14,OpenCode15,OpenCode16,OpenCode17,OpenCode18,OpenCode19,OpenCode20,OpenCode21,OpenTime,Addtime,ID)
                                                SELECT @Term,@OpenCode1,@OpenCode2,@OpenCode3,@OpenCode4,@OpenCode5,@OpenCode6,@OpenCode7,@OpenCode8,@OpenCode9,@OpenCode10,@OpenCode11,@OpenCode12,@OpenCode13,@OpenCode14,@OpenCode15,@OpenCode16,@OpenCode17,@OpenCode18,@OpenCode19,@OpenCode20,@OpenCode21,@OpenTime,GETDATE(),@ID
                                            END";

        private static string QueryListSQL = @"SELECT * FROM  {0}
                                WHERE CONVERT(varchar(10),OpenTime,112)='{1}'
                                   ORDER BY OpenTime DESC";
        #endregion
    }
}
