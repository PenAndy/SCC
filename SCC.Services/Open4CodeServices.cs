﻿using System;
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
    /// 开奖4个球号的彩种数据服务
    /// </summary>
    public class Open4CodeServices : BaseServices, IOpen4Code
    {
        /// <summary>
        /// 获取最新一条记录
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <returns></returns>
        public OpenCode4Model GetLastItem(SCCLottery lottery)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(LastItemSql, TableName);
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, sqlString);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var result = LoadData<OpenCode4Model>(ds.Tables[0].Rows[0]);
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
        public List<OpenCode4Model> GetListIn(SCCLottery lottery, bool IsToday)
        {
            List<string> result = new List<string>();
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(QueryListSQL, TableName, IsToday ? CommonHelper.SCCSysDateTime.ToString("yyyyMMdd") : CommonHelper.SCCSysDateTime.AddDays(-1).ToString("yyyyMMdd"));
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, sqlString);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var data = LoadDataList<OpenCode4Model>(ds.Tables[0]);
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
        public List<OpenCode4Model> GetYesterdayList(SCCLottery lottery)
        {
            List<string> result = new List<string>();
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(YesterdayListSql, TableName, CommonHelper.SCCSysDateTime.AddDays(-1).ToString("yyyyMMdd"));
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, sqlString);
            long tempQiHao = 0;
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var data = LoadDataList<OpenCode4Model>(ds.Tables[0]);
                return data;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// 获取前一天失败列表
        /// 期号格式形如YYMMDDQQ
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <param name="TotalQNum">当前彩种每天总期数</param>
        /// <returns></returns>
        public List<string> GetYesterdayFailQQList(SCCLottery lottery, int TotalQNum)
        {
            List<string> result = new List<string>();
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(YesterdayListSql, TableName, CommonHelper.SCCSysDateTime.AddDays(-1).ToString("yyyyMMdd"));
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, sqlString);
            long tempQiHao = 0;
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var data = LoadDataList<OpenCode4Model>(ds.Tables[0]);
                OpenCode4Model item = null;
                for (var i = 1; i <= TotalQNum; i++)
                {
                    tempQiHao = Convert.ToInt64(CommonHelper.GenerateYesterdayQiHaoYYMMDDQQ(i));
                    item = data.Where(R => R.Term == tempQiHao).FirstOrDefault();
                    if (item == null)
                        result.Add(tempQiHao.ToString());
                }
            }
            else
            {
                for (var i = 1; i <= TotalQNum; i++)
                {
                    tempQiHao = Convert.ToInt64(CommonHelper.GenerateYesterdayQiHaoYYMMDDQQ(i));
                    result.Add(tempQiHao.ToString());
                }
            }
            return result;
        }

        /// <summary>
        /// 新增彩种开奖数据
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <param name="model">开奖数据模型</param>
        /// <returns></returns>
        public bool AddOpen4Code(SCCLottery lottery, OpenCode4Model model)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(AddItemSql, TableName);
            var param = new SqlParameter[]{
                new SqlParameter("@Term",model.Term),
                new SqlParameter("@OpenCode1",model.OpenCode1),
                new SqlParameter("@OpenCode2",model.OpenCode2),
                new SqlParameter("@OpenCode3",model.OpenCode3),
                new SqlParameter("@OpenCode4",model.OpenCode4),
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
	                                            INSERT INTO {0}(Term,OpenCode1,OpenCode2,OpenCode3,OpenCode4,OpenTime,Addtime,ID)
                                                SELECT @Term,@OpenCode1,@OpenCode2,@OpenCode3,@OpenCode4,@OpenTime,GETDATE(),@ID
                                            END";
        private static string QueryListSQL = @"SELECT * FROM  {0}
                                WHERE CONVERT(varchar(10),OpenTime,112)='{1}'
                                   ORDER BY OpenTime DESC";
        #endregion
    }
}
