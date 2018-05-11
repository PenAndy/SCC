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
    /// 地方彩相关数据服务
    /// </summary>
    public class DTOpenCodeServices : BaseServices, IDTOpenCode
    {
        /// <summary>
        /// 获取最近指定n条记录的期号
        /// </summary>
        /// <param name="lottery"></param>
        /// <param name="n">记录数，默认10条</param>
        /// <returns></returns>
        public Dictionary<int,string> GetLast1NTerm(SCCLottery lottery, int n = 10)
        {
            Dictionary<int, string> res = new Dictionary<int, string>();
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            string sql = string.Format(GetLast1NTermSql, TableName, n);

            var ds = SqlHelper.ExecuteDataset(CommandType.Text, sql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int.TryParse(row["Term"].ToString(), out int t);
                    string str = row["Spare"].ToString() ?? "";

                    if (!res.ContainsKey(t))
                    {
                        res.Add(t, str);
                    }
                    else
                    {
                        res[t] = str;
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// 根据期数更新当前彩种的开奖详情
        /// </summary>
        /// <param name="lottery"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        public bool UpdateKJDetailByTerm(SCCLottery lottery,int term,string source)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            string sql = string.Format(UpdateKJDetailByTermSql, TableName);
            
            var param = new SqlParameter[]{
                new SqlParameter("@Term",term),
                new SqlParameter("@Spare",source),
            };
            var result = SqlHelper.ExecuteNonQuery(CommandType.Text, sql, param);
            return result > 0;
        }
        public bool UpdateSSQDetailByTerm(SCCLottery lottery, int term, string source)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            string sql = string.Format(UpdateSSQDetailByTermSql, TableName);

            var param = new SqlParameter[]{
                new SqlParameter("@Term",term),
                new SqlParameter("@KaiJiHao",source),
            };
            var result = SqlHelper.ExecuteNonQuery(CommandType.Text, sql, param);
            return result > 0;
        }

        /// <summary>
        /// 获取最新一条记录
        /// 开奖1个球号的地方彩
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <returns></returns>
        public OpenCode1DTModel GetOpenCode1DTLastItem(SCCLottery lottery)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(LastItemSql, TableName,1);
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, sqlString);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var result = LoadData<OpenCode1DTModel>(ds.Tables[0].Rows[0]);
                return result;
            }
            return null;
        }
        public OpenCode2DTModel GetOpenCode2DTLastItem(SCCLottery lottery)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(LastItemSql, TableName,1);
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, sqlString);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var result = LoadData<OpenCode2DTModel>(ds.Tables[0].Rows[0]);
                return result;
            }
            return null;
        }
        public OpenCode3DTModel GetOpenCode3DTLastItem(SCCLottery lottery)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(LastItemSql, TableName,1);
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, sqlString);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var result = LoadData<OpenCode3DTModel>(ds.Tables[0].Rows[0]);
                return result;
            }
            return null;
        }
        public OpenCode4DTModel GetOpenCode4DTLastItem(SCCLottery lottery)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(LastItemSql, TableName,1);
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, sqlString);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var result = LoadData<OpenCode4DTModel>(ds.Tables[0].Rows[0]);
                return result;
            }
            return null;
        }
        /// <summary>
        /// 获取最新一条记录
        /// 开奖5个球号的地方彩
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <returns></returns>
        public OpenCode5DTModel GetOpenCode5DTLastItem(SCCLottery lottery)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(LastItemSql, TableName,1);
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, sqlString);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var result = LoadData<OpenCode5DTModel>(ds.Tables[0].Rows[0]);
                return result;
            }
            return null;
        }



        /// <summary>
        /// 获取开奖列表
        /// 当前年 去年的不算
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <returns></returns>
        /// 
        /// 
        public List<T> GetListS<T>(SCCLottery lottery)
        {
            List<string> result = new List<string>();
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(QueryListSQL, TableName, CommonHelper.SCCSysDateTime.ToString("yyyy"));
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, sqlString);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var data = LoadDataList<T>(ds.Tables[0]);
                return data;
            }
            else
            {
                return new List<T>();
            }

        }
        /// <summary>
        /// 获取最新一条记录
        /// 开奖7个球号的地方彩
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <returns></returns>
        public OpenCode7DTModel GetOpenCode7DTLastItem(SCCLottery lottery)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(LastItemSql, TableName,1);
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, sqlString);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var result = LoadData<OpenCode7DTModel>(ds.Tables[0].Rows[0]);
                return result;
            }
            return null;
        }
        
        /// <summary>
        /// 获取最新一条记录
        /// 开奖8个球号的地方彩
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <returns></returns>
        public OpenCode8DTModel GetOpenCode8DTLastItem(SCCLottery lottery)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(LastItemSql, TableName,1);
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, sqlString);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var result = LoadData<OpenCode8DTModel>(ds.Tables[0].Rows[0]);
                return result;
            }
            return null;
        }

        /// <summary>
        /// 获取今年的失败期号列表
        /// 第1期与数据库最新一期之间的失败期号列表
        /// 期号格式形如YYQQQ
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <returns></returns>
        public List<string> GetFailedYYQQQList(SCCLottery lottery)
        {
            List<string> result = new List<string>();
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(FailedQiHaoListSql, TableName);
            var param = new SqlParameter[]{
                new SqlParameter("@Year",CommonHelper.SCCSysDateTime.Year)
            };
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, sqlString, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                List<string> termList = new List<string>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    termList.Add(dr["Term"].ToString());
                }
                var topQiHao = Convert.ToInt32(termList[0].Substring(2));
                var qiHao = string.Empty;
                for (var i = 1; i <= topQiHao; i++)
                {
                    qiHao = CommonHelper.GenerateQiHaoYYQQQ(i).ToString();
                    if (!termList.Contains(qiHao))
                        result.Add(qiHao);
                }
            }
            return result;
        }
        /// <summary>
        /// 获取今年的失败期号列表
        /// 第1期与数据库最新一期之间的失败期号列表
        /// 期号格式形如YYYYQQQ
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <returns></returns>
        public List<string> GetFailedYYYYQQQList(SCCLottery lottery)
        {
            List<string> result = new List<string>();
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(FailedQiHaoListSql, TableName);
            var param = new SqlParameter[]{
                new SqlParameter("@Year",CommonHelper.SCCSysDateTime.Year)
            };
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, sqlString, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                List<string> termList = new List<string>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    termList.Add(dr["Term"].ToString());
                }
                var topQiHao = Convert.ToInt32(termList[0].Substring(4));
                var qiHao = string.Empty;
                for (var i = 1; i <= topQiHao; i++)
                {
                    qiHao = CommonHelper.GenerateQiHaoYYYYQQQ(i).ToString();
                    if (!termList.Contains(qiHao))
                        result.Add(qiHao);
                }
            }
            return result;
        }

        /// <summary>
        /// 新增彩种开奖数据
        /// 开奖1个球号的地方彩
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <param name="model">开奖数据模型</param>
        /// <returns></returns>
        public bool AddDTOpen1Code(SCCLottery lottery, OpenCode1DTModel model)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(AddOpenCode1DTItemSql, TableName);
            var param = new SqlParameter[]{
                new SqlParameter("@Term",model.Term),
                new SqlParameter("@OpenCode1",model.OpenCode1),
                new SqlParameter("@OpenTime",model.OpenTime),
                new SqlParameter("@Spare",model.Spare)
            };
            var result = SqlHelper.ExecuteNonQuery(CommandType.Text, sqlString, param);
            return result > 0;
        }
        /// <summary>
        /// 新增彩种开奖数据
        /// 开奖5个球号的地方彩
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <param name="model">开奖数据模型</param>
        /// <returns></returns>
        public bool AddDTOpen5Code(SCCLottery lottery, OpenCode5DTModel model)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(AddOpenCode5DTItemSql, TableName);
            var param = new SqlParameter[]{
                new SqlParameter("@Term",model.Term),
                new SqlParameter("@OpenCode1",model.OpenCode1),
                new SqlParameter("@OpenCode2",model.OpenCode2),
                new SqlParameter("@OpenCode3",model.OpenCode3),
                new SqlParameter("@OpenCode4",model.OpenCode4),
                new SqlParameter("@OpenCode5",model.OpenCode5),
                new SqlParameter("@OpenTime",model.OpenTime),
                new SqlParameter("@ID",Guid.NewGuid().ToString().Replace("-","").ToLower()),
                new SqlParameter("@Spare",model.Spare)
            };
            var result = SqlHelper.ExecuteNonQuery(CommandType.Text, sqlString, param);
            return result > 0;
        }





        public bool AddDTOpen4Code(SCCLottery lottery, OpenCode4DTModel model)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(AddOpenCode4DTItemSql, TableName);
            var param = new SqlParameter[]{
                new SqlParameter("@Term",model.Term),
                new SqlParameter("@OpenCode1",model.OpenCode1),
                new SqlParameter("@OpenCode2",model.OpenCode2),
                new SqlParameter("@OpenCode3",model.OpenCode3),
                new SqlParameter("@OpenCode4",model.OpenCode4),
                new SqlParameter("@OpenTime",model.OpenTime),
                new SqlParameter("@ID",Guid.NewGuid().ToString().Replace("-","").ToLower()),
                new SqlParameter("@Spare",model.Spare)
            };
            var result = SqlHelper.ExecuteNonQuery(CommandType.Text, sqlString, param);
            return result > 0;
        }
        /// <summary>
        /// 新增彩种开奖数据
        /// 开奖7个球号的地方彩
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <param name="model">开奖数据模型</param>
        /// <returns></returns>
        public bool AddDTOpen7Code(SCCLottery lottery, OpenCode7DTModel model)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(AddOpenCode7DTItemSql, TableName);
            var param = new SqlParameter[]{
                new SqlParameter("@Term",model.Term),
                new SqlParameter("@OpenCode1",model.OpenCode1),
                new SqlParameter("@OpenCode2",model.OpenCode2),
                new SqlParameter("@OpenCode3",model.OpenCode3),
                new SqlParameter("@OpenCode4",model.OpenCode4),
                new SqlParameter("@OpenCode5",model.OpenCode5),
                new SqlParameter("@OpenCode6",model.OpenCode6),
                new SqlParameter("@OpenCode7",model.OpenCode7),
                new SqlParameter("@OpenTime",model.OpenTime),
                 new SqlParameter("@ID",Guid.NewGuid().ToString().Replace("-","").ToLower()),
                new SqlParameter("@Spare",model.Spare)
            };
            var result = SqlHelper.ExecuteNonQuery(CommandType.Text, sqlString, param);
            return result > 0;
        }
        /// <summary>
        /// 新增彩种开奖数据
        /// 开奖8个球号的地方彩
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <param name="model">开奖数据模型</param>
        /// <returns></returns>
        public bool AddDTOpen8Code(SCCLottery lottery, OpenCode8DTModel model)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(AddOpenCode8DTItemSql, TableName);
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
                new SqlParameter("@OpenTime",model.OpenTime),
                new SqlParameter("@Spare",model.Spare),
                new SqlParameter("@ID",Guid.NewGuid().ToString().Replace("-", ""))
            };
            var result = SqlHelper.ExecuteNonQuery(CommandType.Text, sqlString, param);
            return result > 0;
        }

        /// <summary>
        /// 获取江苏七位数所有开奖记录
        /// 生成走势图所需数据
        /// </summary>
        /// <returns></returns>
        public List<TCJS7WSInfo> GetJS7WSListOpenCode()
        {
            List<TCJS7WSInfo> result = new List<TCJS7WSInfo>();
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, GetJS7WSListSql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = LoadDataList<TCJS7WSInfo>(ds.Tables[0]);
                foreach (var info in result)
                {
                    info.OpenCode = new List<int>(){
                        info.OpenCode1,
                        info.OpenCode2,
                        info.OpenCode3,
                        info.OpenCode4,
                        info.OpenCode5,
                        info.OpenCode6,
                        info.OpenCode7
                    };
                }
            }
            return result;
        }

        /// <summary>
        /// 获取浙江体彩6+1所有开奖记录
        /// 生成走势图所需数据
        /// </summary>
        /// <returns></returns>
        public List<TCZJ6J1Info> GetZJ6J1ListOpenCode()
        {
            List<TCZJ6J1Info> result = new List<TCZJ6J1Info>();
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, GetZJ6J1ListSql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = LoadDataList<TCZJ6J1Info>(ds.Tables[0]);
                foreach (var info in result)
                {
                    info.OpenCode = new List<int>(){
                        info.OpenCode1,
                        info.OpenCode2,
                        info.OpenCode3,
                        info.OpenCode4,
                        info.OpenCode5,
                        info.OpenCode6,
                        info.OpenCode7
                    };
                }
            }
            return result;
        }

        /// <summary>
        /// 获取新疆35选7所有开奖记录
        /// 生成走势图所需数据
        /// </summary>
        /// <returns></returns>
        public List<FCXJ35X7Info> GetXJ35X7ListOpenCode()
        {
            List<FCXJ35X7Info> result = new List<FCXJ35X7Info>();
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, GetXJ35X7ListSql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = LoadDataList<FCXJ35X7Info>(ds.Tables[0]);
                foreach (var info in result)
                {
                    info.OpenCode = new List<int>(){
                        info.OpenCode1,
                        info.OpenCode2,
                        info.OpenCode3,
                        info.OpenCode4,
                        info.OpenCode5,
                        info.OpenCode6,
                        info.OpenCode7,
                        info.OpenCode8
                    };
                }
            }
            return result;
        }

        /// <summary>
        /// 获取东方6+1所有开奖记录
        /// 生成走势图所需数据
        /// </summary>
        /// <returns></returns>
        public List<FCDF6J1Info> GetDF6J1ListOpenCode()
        {
            List<FCDF6J1Info> result = new List<FCDF6J1Info>();
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, GetDF6J1ListSql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = LoadDataList<FCDF6J1Info>(ds.Tables[0]);
                foreach (var info in result)
                {
                    info.OpenCode = new List<int>(){
                        info.OpenCode1,
                        info.OpenCode2,
                        info.OpenCode3,
                        info.OpenCode4,
                        info.OpenCode5,
                        info.OpenCode6,
                        info.OpenCode7
                    };
                }
            }
            return result;
        }

        /// <summary>
        /// 获取东方6+1最新指定条数所有开奖记录
        /// </summary>
        /// <param name="period">指定条数</param>
        /// <returns></returns>
        public List<FCDF6J1Info> GetDF6J1ListOpenCode(int period)
        {
            List<FCDF6J1Info> result = new List<FCDF6J1Info>();
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, string.Format(GetDF6J1TopCountListSql, period));
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = LoadDataList<FCDF6J1Info>(ds.Tables[0]);
            }
            return result;
        }

        /// <summary>
        /// 获取指定期数的开奖详情
        /// </summary>
        /// <param name="Term">指定期数</param>
        /// <returns></returns>
        public OpenCode7DTModel GetDF6J1Detail(int Term)
        {
            var param = new SqlParameter[]{
                new SqlParameter("@Term",Term)
            };
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, GetDF6J1DetailSql, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return LoadData<OpenCode7DTModel>(ds.Tables[0].Rows[0]);
            }
            return null;
        }

        /// <summary>
        /// 获取华东15选5所有开奖记录
        /// 生成走势图所需数据
        /// </summary>
        /// <returns></returns>
        public List<FCHD15X5Info> GetHD15X5ListOpenCode()
        {
            List<FCHD15X5Info> result = new List<FCHD15X5Info>();
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, GetHD15X5ListSql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = LoadDataList<FCHD15X5Info>(ds.Tables[0]);
                foreach (var info in result)
                {
                    info.OpenCode = new List<int>(){
                        info.OpenCode1,
                        info.OpenCode2,
                        info.OpenCode3,
                        info.OpenCode4,
                        info.OpenCode5
                    };
                }
            }
            return result;
        }
        /// <summary>
        /// 获取华东15选5最新指定条数所有开奖记录
        /// </summary>
        /// <param name="period">指定条数</param>
        /// <returns></returns>
        public List<FCHD15X5Info> GetHD15X5ListOpenCode(int period)
        {
            List<FCHD15X5Info> result = new List<FCHD15X5Info>();
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, string.Format(GetHD15X5TopCountListSql, period));
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = LoadDataList<FCHD15X5Info>(ds.Tables[0]);
            }
            return result;
        }
        /// <summary>
        /// 获取指定期数的开奖详情
        /// </summary>
        /// <param name="Term">指定期数</param>
        /// <returns></returns>
        public OpenCode5DTModel GetHD15X5Detail(int Term)
        {
            var param = new SqlParameter[]{
                new SqlParameter("@Term",Term)
            };
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, GetHD15X5DetailSql, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return LoadData<OpenCode5DTModel>(ds.Tables[0].Rows[0]);
            }
            return null;
        }

        /// <summary>
        /// 获取河南22选5所有开奖记录
        /// 生成走势图所需数据
        /// </summary>
        /// <returns></returns>
        public List<FCHN22X5Info> GetHN22X5ListOpenCode()
        {
            List<FCHN22X5Info> result = new List<FCHN22X5Info>();
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, GetHN22X5ListSql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = LoadDataList<FCHN22X5Info>(ds.Tables[0]);
                foreach (var info in result)
                {
                    info.OpenCode = new List<int>(){
                        info.OpenCode1,
                        info.OpenCode2,
                        info.OpenCode3,
                        info.OpenCode4,
                        info.OpenCode5
                    };
                }
            }
            return result;
        }

        /// <summary>
        /// 获取广东36选7所有开奖记录
        /// 生成走势图所需数据
        /// </summary>
        /// <returns></returns>
        public List<FCNY36X7Info> GetGD36X7ListOpenCode()
        {
            List<FCNY36X7Info> result = new List<FCNY36X7Info>();
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, GetGD36X7ListSql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = LoadDataList<FCNY36X7Info>(ds.Tables[0]);
                foreach (var info in result)
                {
                    info.OpenCode = new List<int>(){
                        info.OpenCode1,
                        info.OpenCode2,
                        info.OpenCode3,
                        info.OpenCode4,
                        info.OpenCode5,
                        info.OpenCode6,
                        info.OpenCode7
                    };
                }
            }
            return result;
        }

        /// <summary>
        /// 获取湖北30选5所有开奖记录
        /// 生成走势图所需数据
        /// </summary>
        /// <returns></returns>
        public List<FCHB30X5Info> GetHuBei30X5ListOpenCode()
        {
            List<FCHB30X5Info> result = new List<FCHB30X5Info>();
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, GetHuBei30X5ListSql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = LoadDataList<FCHB30X5Info>(ds.Tables[0]);
                foreach (var info in result)
                {
                    info.OpenCode = new List<int>(){
                        info.OpenCode1,
                        info.OpenCode2,
                        info.OpenCode3,
                        info.OpenCode4,
                        info.OpenCode5
                    };
                }
            }
            return result;
        }

        /// <summary>
        /// 获取福建36选7所有开奖记录
        /// 生成走势图所需数据
        /// </summary>
        /// <returns></returns>
        public List<TCFJ36X7Info> GetFJ36X7ListOpenCode()
        {
            List<TCFJ36X7Info> result = new List<TCFJ36X7Info>();
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, GetFJ36X7ListSql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = LoadDataList<TCFJ36X7Info>(ds.Tables[0]);
                foreach (var info in result)
                {
                    info.OpenCode = new List<int>(){
                        info.OpenCode1,
                        info.OpenCode2,
                        info.OpenCode3,
                        info.OpenCode4,
                        info.OpenCode5,
                        info.OpenCode6,
                        info.OpenCode7,
                        info.OpenCode8
                    };
                }
            }
            return result;
        }

        /// <summary>
        /// 获取福建31选7所有开奖记录
        /// 生成走势图所需数据
        /// </summary>
        /// <returns></returns>
        public List<TCFJ31X7Info> GetFJ31X7ListOpenCode()
        {
            List<TCFJ31X7Info> result = new List<TCFJ31X7Info>();
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, GetFJ31X7ListSql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = LoadDataList<TCFJ31X7Info>(ds.Tables[0]);
                foreach (var info in result)
                {
                    info.OpenCode = new List<int>(){
                        info.OpenCode1,
                        info.OpenCode2,
                        info.OpenCode3,
                        info.OpenCode4,
                        info.OpenCode5,
                        info.OpenCode6,
                        info.OpenCode7,
                        info.OpenCode8
                    };
                }
            }
            return result;
        }

        /// <summary>
        /// 获取广东好彩1所有开奖记录
        /// 生成走势图所需数据
        /// </summary>
        /// <returns></returns>
        public List<FCGDHC1Info> GetGDHC1ListOpenCode()
        {
            List<FCGDHC1Info> result = new List<FCGDHC1Info>();
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, GetGDHC1ListSql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = LoadDataList<FCGDHC1Info>(ds.Tables[0]);
                foreach (var info in result)
                {
                    info.OpenCode = new List<int>(){
                        info.OpenCode1
                    };
                }
            }
            return result;
        }

        public bool AddDTOpen3Code(SCCLottery lottery, OpenCode3DTModel model)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(AddOpenCode3DTItemSql, TableName);
            var param = new SqlParameter[]{
                new SqlParameter("@Term",model.Term),
                new SqlParameter("@OpenCode1",model.OpenCode1),
                new SqlParameter("@OpenCode2",model.OpenCode2),
                new SqlParameter("@OpenCode3",model.OpenCode3),
                new SqlParameter("@OpenTime",model.OpenTime),
                new SqlParameter("@ID",Guid.NewGuid().ToString().Replace("-","").ToLower()),
                new SqlParameter("@Spare",model.Spare)
            };
            var result = SqlHelper.ExecuteNonQuery(CommandType.Text, sqlString, param);
            return result > 0;
        }

        public bool AddDTOpen2Code(SCCLottery lottery, OpenCode2DTModel model)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(AddOpenCode2DTItemSql, TableName);
            var param = new SqlParameter[]{
                new SqlParameter("@Term",model.Term),
                new SqlParameter("@OpenCode1",model.OpenCode1),
                new SqlParameter("@OpenCode2",model.OpenCode2),
                new SqlParameter("@OpenTime",model.OpenTime),
                new SqlParameter("@ID",Guid.NewGuid().ToString().Replace("-","").ToLower()),
                new SqlParameter("@Spare",model.Spare)
            };
            var result = SqlHelper.ExecuteNonQuery(CommandType.Text, sqlString, param);
            return result > 0;
        }

        public OpenCodeFC3DTModel GetOpenCodeFC3DTLastItem(SCCLottery lottery)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(LastItemSql, TableName,1);
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, sqlString);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var result = LoadData<OpenCodeFC3DTModel>(ds.Tables[0].Rows[0]);
                return result;
            }
            return null;
        }

        public bool AddDTOpenFC3DCode(SCCLottery lottery, OpenCodeFC3DTModel model)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(AddOpenCodeFC3DTItemSql, TableName);
            var param = new SqlParameter[]{
                new SqlParameter("@Term",model.Term),
                new SqlParameter("@OpenCode1",model.OpenCode1),
                new SqlParameter("@OpenCode2",model.OpenCode2),
                new SqlParameter("@OpenCode3",model.OpenCode3),
                new SqlParameter("@OpenTime",model.OpenTime),
                new SqlParameter("@ID",Guid.NewGuid().ToString().Replace("-","").ToLower()),
                new SqlParameter ("@KaiJiHao",model.KaiJiHao),
                new SqlParameter("@ShiJiHao",model.ShiJiHao),
                new SqlParameter("@Spare",model.Spare)
            };
            var result = SqlHelper.ExecuteNonQuery(CommandType.Text, sqlString, param);
            return result > 0;
        }

        public OpenCodePL5TModel GetOpenCodePL5TLastItem(SCCLottery lottery)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(LastItemSql, TableName,1);
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, sqlString);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var result = LoadData<OpenCodePL5TModel>(ds.Tables[0].Rows[0]);
                return result;
            }
            return null;
        }
        public OpenCodePL3TModel GetOpenCodePL3TLastItem(SCCLottery lottery)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(LastItemSql, TableName, 1);
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, sqlString);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var result = LoadData<OpenCodePL3TModel>(ds.Tables[0].Rows[0]);
                return result;
            }
            return null;
        }
        public bool AddDTOpenPL5Code(SCCLottery lottery, OpenCodePL5TModel model)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(AddOpenCodeFC5DTItemSql, TableName);
            var param = new SqlParameter[]{
                new SqlParameter("@Term",model.Term),
                new SqlParameter("@OpenCode1",model.OpenCode1),
                new SqlParameter("@OpenCode2",model.OpenCode2),
                new SqlParameter("@OpenCode3",model.OpenCode3),
                new SqlParameter("@OpenCode4",model.OpenCode4),
                new SqlParameter("@OpenCode5",model.OpenCode5),
                new SqlParameter("@OpenTime",model.OpenTime),
                new SqlParameter("@ID",Guid.NewGuid().ToString().Replace("-","").ToLower()),
                new SqlParameter ("@KaiJiHao",model.KaiJiHao),
                new SqlParameter("@ShiJiHao",model.ShiJiHao),
                new SqlParameter("@Spare",model.Spare)
            };
            var result = SqlHelper.ExecuteNonQuery(CommandType.Text, sqlString, param);
            return result > 0;
        }
        public bool AddDTOpenPL3Code(SCCLottery lottery, OpenCodePL3TModel model)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(lottery);
            var sqlString = string.Format(AddOpenCodePL3DTItemSql, TableName);
            var param = new SqlParameter[]{
                new SqlParameter("@Term",model.Term),
                new SqlParameter("@OpenCode1",model.OpenCode1),
                new SqlParameter("@OpenCode2",model.OpenCode2),
                new SqlParameter("@OpenCode3",model.OpenCode3),
                new SqlParameter("@OpenTime",model.OpenTime),
                new SqlParameter("@ID",Guid.NewGuid().ToString().Replace("-","").ToLower()),
                new SqlParameter ("@KaiJiHao",model.KaiJiHao),
                new SqlParameter("@ShiJiHao",model.ShiJiHao),
                new SqlParameter("@Spare",model.Spare)
            };
            var result = SqlHelper.ExecuteNonQuery(CommandType.Text, sqlString, param);
            return result > 0;
        }
        public bool UpdateKJDetail3DByTerm(SCCLottery currentLottery, int key, string res, OpenCodeFC3DTModel matchItem)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(currentLottery);
            string sql = string.Format(UpdateKJDetail3DByTermSql, TableName);

            var param = new SqlParameter[]{
                new SqlParameter ("@OpenCode1",matchItem.OpenCode1),
                new SqlParameter ("@OpenCode2",matchItem.OpenCode2),
                new SqlParameter ("@OpenCode3",matchItem.OpenCode3),
                new SqlParameter("@Term",key),
                new SqlParameter("@Spare",res),
            };
            var result = SqlHelper.ExecuteNonQuery(CommandType.Text, sql, param);
            return result > 0;
        }

        public bool UpdateKJDetailP5ByTerm(SCCLottery currentLottery, int key, string res, OpenCodePL5TModel matchItem)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(currentLottery);
            string sql = string.Format(UpdateKJDetail5DByTermSql, TableName);

            var param = new SqlParameter[]{
                new SqlParameter ("@OpenCode1",matchItem.OpenCode1),
                new SqlParameter ("@OpenCode2",matchItem.OpenCode2),
                new SqlParameter ("@OpenCode3",matchItem.OpenCode3),
                new SqlParameter ("@OpenCode4",matchItem.OpenCode4),
                new SqlParameter ("@OpenCode5",matchItem.OpenCode5),
                new SqlParameter("@Term",key),
                new SqlParameter("@Spare",res),
            };
            var result = SqlHelper.ExecuteNonQuery(CommandType.Text, sql, param);
            return result > 0;
        }


        public bool UpdateKJDetailP3ByTerm(SCCLottery currentLottery, int key, string res, OpenCodePL3TModel matchItem)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(currentLottery);
            string sql = string.Format(UpdateKJDetailPL3DByTermSql, TableName);

            var param = new SqlParameter[]{
                new SqlParameter ("@OpenCode1",matchItem.OpenCode1),
                new SqlParameter ("@OpenCode2",matchItem.OpenCode2),
                new SqlParameter ("@OpenCode3",matchItem.OpenCode3),
                new SqlParameter("@Term",key),
                new SqlParameter("@Spare",res),
            };
            var result = SqlHelper.ExecuteNonQuery(CommandType.Text, sql, param);
            return result > 0;
        }

        public bool LotterySkillModel(SCCLottery currentLottery, LotterySkillModel matchItem)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(currentLottery);
            string sql = string.Format(LotterySkillModelSql, TableName);

            var param = new SqlParameter[]{
                new SqlParameter ("@Title",matchItem.Title),
                new SqlParameter ("@Author",matchItem.Author),
                new SqlParameter ("@Content",matchItem.Content),
                new SqlParameter("@ID",Guid.NewGuid().ToString().Replace("-","").ToLower()),
                new SqlParameter ("@IsDelete",matchItem.IsDelete),
                new SqlParameter ("@SourceUrl",matchItem.SourceUrl),
                new SqlParameter ("@TypeName",matchItem.TypeName),
                new SqlParameter ("@TypeId",matchItem.TypeId),
            };
            var result = SqlHelper.ExecuteNonQuery(CommandType.Text, sql, param);
            return result > 0;
        }
        public bool LotteryGlossaryModel(SCCLottery currentLottery, LotteryGlossaryModel matchItem)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(currentLottery);
            string sql = string.Format(LotterySkillModelSql, TableName);

            var param = new SqlParameter[]{
                new SqlParameter ("@Title",matchItem.Title),
                new SqlParameter ("@Author",matchItem.Author),
                new SqlParameter ("@Content",matchItem.Content),
                new SqlParameter("@ID",Guid.NewGuid().ToString().Replace("-","").ToLower()),
                new SqlParameter ("@IsDelete",matchItem.IsDelete),
                new SqlParameter ("@SourceUrl",matchItem.SourceUrl),
                new SqlParameter ("@TypeName",matchItem.TypeName),
                new SqlParameter ("@TypeId",matchItem.TypeID),
            };
            var result = SqlHelper.ExecuteNonQuery(CommandType.Text, sql, param);
            return result > 0;
        }
        public bool LotteryNewsModel(SCCLottery currentLottery, LotteryNewsModel matchItem)
        {
            var TableName = EnumHelper.GetSCCLotteryTableName(currentLottery);
            string sql = string.Format(LotterySkillModelSql, TableName);

            var param = new SqlParameter[]{
                new SqlParameter ("@Title",matchItem.Title),
                new SqlParameter ("@Author",matchItem.Author),
                new SqlParameter ("@Content",matchItem.Content),
                new SqlParameter("@ID",Guid.NewGuid().ToString().Replace("-","").ToLower()),
                new SqlParameter ("@IsDelete",matchItem.IsDelete),
                new SqlParameter ("@SourceUrl",matchItem.SourceUrl),
                new SqlParameter ("@TypeName",matchItem.TypeName),
                new SqlParameter ("@TypeId",matchItem.TypeID),
            };
            var result = SqlHelper.ExecuteNonQuery(CommandType.Text, sql, param);
            return result > 0;
        }

        

        #region Sql语句
        /// <summary>
        /// 获取最近指定n条记录的期号
        /// </summary>
        private static string GetLast1NTermSql = "SELECT TOP {1} [Term],[Spare] FROM {0} ORDER BY Term DESC";
        /// <summary>
        /// 根据期数更新当前彩种的开奖详情
        /// </summary>
        private static string UpdateKJDetailByTermSql = @"IF EXISTS(SELECT TOP 1 1 FROM {0} WHERE Term = @Term)
                                                        BEGIN
	                                                        update {0} set [Spare] = @Spare where Term = @Term
                                                        END";
        private static string UpdateSSQDetailByTermSql = @"IF EXISTS(SELECT TOP 1 1 FROM {0} WHERE Term = @Term)
                                                        BEGIN
	                                                        update {0} set [KaiJiHao] = @KaiJiHao where Term = @Term
                                                        END";

        private static string UpdateKJDetail3DByTermSql = @"IF EXISTS(SELECT TOP 1 1 FROM {0} WHERE Term = @Term)
                                                        BEGIN
	                                                        update {0} set [OpenCode1]=@OpenCode1,[OpenCode2]=@OpenCode2,[OpenCode3]=@OpenCode3, [Spare] = @Spare where Term = @Term
                                                        END";

        private static string UpdateKJDetail5DByTermSql = @"IF EXISTS(SELECT TOP 1 1 FROM {0} WHERE Term = @Term)
                                                        BEGIN
	                                                        update {0} set [OpenCode1]=@OpenCode1,[OpenCode2]=@OpenCode2,[OpenCode3]=@OpenCode3,[OpenCode4]=@OpenCode4,[OpenCode5]=@OpenCode5, [Spare] = @Spare where Term = @Term
                                                        END";
        private static string UpdateKJDetailPL3DByTermSql = @"IF EXISTS(SELECT TOP 1 1 FROM {0} WHERE Term = @Term)
                                                        BEGIN
	                                                        update {0} set [OpenCode1]=@OpenCode1,[OpenCode2]=@OpenCode2,[OpenCode3]=@OpenCode3, [Spare] = @Spare where Term = @Term
                                                        END";



        /// <summary>
        /// 获取最新一条记录的Sql语句
        /// </summary>
        private static string LastItemSql = @"SELECT TOP 1 * FROM {0} ORDER BY Term DESC";
        /// <summary>
        /// 获取今年的失败期号列表的Sql语句
        /// </summary>
        private static string FailedQiHaoListSql = @"SELECT Term FROM {0} 
                                                    WHERE YEAR(OpenTime) = @year
                                                    ORDER BY Term DESC";
        /// <summary>
        /// 新增开奖1个球号的彩种数据的Sql语句
        /// </summary>
        private static string AddOpenCode1DTItemSql = @"IF NOT EXISTS(SELECT TOP 1 1 FROM {0} WHERE Term = @Term)
                                                        BEGIN
	                                                        INSERT INTO {0}(Term,OpenCode1,OpenTime,Addtime,Spare)
                                                            SELECT @Term,@OpenCode1,@OpenTime,GETDATE(),@Spare
                                                        END";

        private static string AddOpenCode2DTItemSql = @"IF NOT EXISTS(SELECT TOP 1 1 FROM {0} WHERE Term = @Term)
                                                        BEGIN
	                                                        INSERT INTO {0}(Term,OpenCode1,OpenCode2,OpenTime,Addtime,Spare,ID)
                                                            SELECT @Term,@OpenCode1,@OpenCode2,@OpenTime,GETDATE(),@Spare,@ID
                                                        END";
        private static string AddOpenCode3DTItemSql = @"IF NOT EXISTS(SELECT TOP 1 1 FROM {0} WHERE Term = @Term)
                                                        BEGIN
	                                                        INSERT INTO {0}(Term,OpenCode1,OpenCode2,OpenCode3,OpenTime,Addtime,Spare,ID)
                                                            SELECT @Term,@OpenCode1,@OpenCode2,@OpenCode3,@OpenTime,GETDATE(),@Spare,@ID
                                                        END";
        private static string AddOpenCodeFC3DTItemSql = @"IF NOT EXISTS(SELECT TOP 1 1 FROM {0} WHERE Term = @Term)
                                                        BEGIN
	                                                        INSERT INTO {0}(Term,OpenCode1,OpenCode2,OpenCode3,OpenTime,Addtime,Spare,ID,ShiJiHao,KaiJiHao)
                                                            SELECT @Term,@OpenCode1,@OpenCode2,@OpenCode3,@OpenTime,GETDATE(),@Spare,@ID,@ShiJiHao,@KaiJiHao
                                                        END";
        private static string AddOpenCodeFC5DTItemSql = @"IF NOT EXISTS(SELECT TOP 1 1 FROM {0} WHERE Term = @Term)
                                                        BEGIN
	                                                        INSERT INTO {0}(Term,OpenCode1,OpenCode2,OpenCode3,OpenCode4,OpenCode5,OpenTime,Addtime,Spare,ID,ShiJiHao,KaiJiHao)
                                                            SELECT @Term,@OpenCode1,@OpenCode2,@OpenCode3,@OpenCode4,@OpenCode5,@OpenTime,GETDATE(),@Spare,@ID,@ShiJiHao,@KaiJiHao
                                                        END";
        private static string AddOpenCodePL3DTItemSql = @"IF NOT EXISTS(SELECT TOP 1 1 FROM {0} WHERE Term = @Term)
                                                        BEGIN
	                                                        INSERT INTO {0}(Term,OpenCode1,OpenCode2,OpenCode3,OpenTime,Addtime,Spare,ID,ShiJiHao,KaiJiHao)
                                                            SELECT @Term,@OpenCode1,@OpenCode2,@OpenCode3,@OpenTime,GETDATE(),@Spare,@ID,@ShiJiHao,@KaiJiHao
                                                        END";
        #endregion

        private static string LotterySkillModelSql = @"IF NOT EXISTS(SELECT TOP 1 1 FROM {0} WHERE SourceUrl = @SourceUrl)
                                                        BEGIN
	                                                        INSERT INTO {0}(Title,Author,Content,IsDelete,SourceUrl,TypeName,TypeId,Addtime,ID)
                                                            SELECT @Title,@Author,@Content,@IsDelete,@SourceUrl,@TypeName,@TypeId,GETDATE(),@ID
                                                        END";
        /// <summary>
        /// 新增开奖5个球号的彩种数据的Sql语句
        /// </summary>
        /// 
        private static string AddOpenCode5DTItemSql = @"IF NOT EXISTS(SELECT TOP 1 1 FROM {0} WHERE Term = @Term)
                                                        BEGIN
	                                                        INSERT INTO {0}(Term,OpenCode1,OpenCode2,OpenCode3,OpenCode4,OpenCode5,OpenTime,Addtime,Spare,ID)
                                                            SELECT @Term,@OpenCode1,@OpenCode2,@OpenCode3,@OpenCode4,@OpenCode5,@OpenTime,GETDATE(),@Spare,@ID
                                                        END";

        private static string AddOpenCode4DTItemSql = @"IF NOT EXISTS(SELECT TOP 1 1 FROM {0} WHERE Term = @Term)
                                                        BEGIN
	                                                        INSERT INTO {0}(Term,OpenCode1,OpenCode2,OpenCode3,OpenCode4,OpenTime,Addtime,Spare,ID)
                                                            SELECT @Term,@OpenCode1,@OpenCode2,@OpenCode3,@OpenCode4,@OpenTime,GETDATE(),@Spare,@ID
                                                        END";


        /// <summary>
        /// 新增开奖7个球号的彩种数据的Sql语句
        /// </summary>
        private static string AddOpenCode7DTItemSql = @"IF NOT EXISTS(SELECT TOP 1 1 FROM {0} WHERE Term = @Term)
                                                        BEGIN
	                                                        INSERT INTO {0}(Term,OpenCode1,OpenCode2,OpenCode3,OpenCode4,OpenCode5,OpenCode6,OpenCode7,OpenTime,Addtime,Spare,ID)
                                                            SELECT @Term,@OpenCode1,@OpenCode2,@OpenCode3,@OpenCode4,@OpenCode5,@OpenCode6,@OpenCode7,@OpenTime,GETDATE(),@Spare  ,@ID
                                                        END";
        /// <summary>
        /// 新增开奖8个秋毫的彩种数据的Sql语句
        /// </summary>
        private static string AddOpenCode8DTItemSql = @"IF NOT EXISTS(SELECT TOP 1 1 FROM {0} WHERE Term = @Term)
                                                        BEGIN
	                                                        INSERT INTO {0}(Term,OpenCode1,OpenCode2,OpenCode3,OpenCode4,OpenCode5,OpenCode6,OpenCode7,OpenCode8,OpenTime,Addtime,Spare,ID)
                                                            SELECT @Term,@OpenCode1,@OpenCode2,@OpenCode3,@OpenCode4,@OpenCode5,@OpenCode6,@OpenCode7,@OpenCode8,@OpenTime,GETDATE(),@Spare,@ID
                                                        END";
        /// <summary>
        /// 获取江苏七位数所有开奖记录的Sql语句
        /// </summary>
        private static string GetJS7WSListSql = @"SELECT * FROM DT_TCJS7WS ORDER BY Term ASC";
        /// <summary>
        /// 获取浙江体彩6+1所有开奖记录的Sql语句
        /// </summary>
        private static string GetZJ6J1ListSql = @"SELECT * FROM DT_TCZJ6J1 ORDER BY Term ASC";
        /// <summary>
        /// 获取新疆35选7所有开奖记录的Sql语句
        /// </summary>
        private static string GetXJ35X7ListSql = @"SELECT * FROM DT_FCXJ35X7 ORDER BY Term ASC";
        /// <summary>
        /// 获取东方6+1所有开奖记录的Sql语句
        /// </summary>
        private static string GetDF6J1ListSql = @"SELECT * FROM DT_FCDF6J1 ORDER BY Term ASC";
        /// <summary>
        /// 获取东方6+1最新指定条数开奖记录的Sql语句
        /// </summary>
        private static string GetDF6J1TopCountListSql = @"SELECT TOP {0} * FROM DT_FCDF6J1 ORDER BY Term DESC";
        /// <summary>
        /// 获取东方6+1指定期数的开奖详情的Sql语句
        /// </summary>
        private static string GetDF6J1DetailSql = @"SELECT * FROM DT_FCDF6J1 WHERE Term = @Term";
        /// <summary>
        /// 获取华东15选5所有开奖记录的Sql语句
        /// </summary>
        private static string GetHD15X5ListSql = @"SELECT * FROM DT_FCHD11X5 ORDER BY Term ASC";
        /// <summary>
        /// 获取华东15选5最新指定条数开奖记录的Sql语句
        /// </summary>
        private static string GetHD15X5TopCountListSql = @"SELECT TOP {0} * FROM DT_FCHD11X5 ORDER BY Term DESC";
        /// <summary>
        /// 获取华东15选5指定期数的开奖详情的Sql语句
        /// </summary>
        private static string GetHD15X5DetailSql = @"SELECT * FROM DT_FCHD11X5 WHERE Term = @Term";
        /// <summary>
        /// 获取河南22选5所有开奖记录的Sql语句
        /// </summary>
        private static string GetHN22X5ListSql = @"SELECT * FROM DT_FCHN22X5 ORDER BY Term ASC";
        /// <summary>
        /// 获取广东36选7所有开奖记录的Sql语句
        /// </summary>
        private static string GetGD36X7ListSql = @"SELECT * FROM DT_FCNY36X7 ORDER BY Term ASC";
        /// <summary>
        /// 获取湖北30选5所有开奖记录的Sql语句
        /// </summary>
        private static string GetHuBei30X5ListSql = @"SELECT * FROM DT_FCHB30X5 ORDER BY Term ASC";
        /// <summary>
        /// 获取福建36选7所有开奖记录的Sql语句
        /// </summary>
        private static string GetFJ36X7ListSql = @"SELECT * FROM DT_TCFJ36X7 ORDER BY Term ASC";
        /// <summary>
        /// 获取福建31选7所有开奖记录的Sql语句
        /// </summary>
        private static string GetFJ31X7ListSql = @"SELECT * FROM DT_TCFJ31X7 ORDER BY Term ASC";
        /// <summary>
        /// 获取广东好彩1所有开奖记录的Sql语句
        /// </summary>
        private static string GetGDHC1ListSql = @"SELECT * FROM DT_HC1 ORDER BY Term ASC";
   
        private static string QueryListSQL = @"SELECT * FROM  {0}
                                WHERE CONVERT(varchar(4),OpenTime,112)='{1}'
                                   ORDER BY OpenTime DESC";
    }
}
