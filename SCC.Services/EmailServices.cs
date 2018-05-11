using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using SCC.Interface;
using SCC.Common;
using SCC.Models;

namespace SCC.Services
{
    /// <summary>
    /// 邮件相关服务
    /// </summary>
    public class EmailServices : BaseServices, IEmail
    {
        /// <summary>
        /// 添加邮件提醒记录
        /// </summary>
        /// <param name="LotteryName">彩种名称</param>
        /// <param name="QiHao">期号</param>
        /// <param name="OpenTime">开奖时间</param>
        /// <returns></returns>
        public bool AddEmail(string LotteryName, string QiHao, DateTime OpenTime, string Spare = "")
        {
            var param = new SqlParameter[]{
                new SqlParameter("@LotteryName",LotteryName),
                new SqlParameter("@QiHao",QiHao),
                new SqlParameter("@OpenTime",OpenTime),
                new SqlParameter("@ID",Guid.NewGuid().ToString().Replace("-", "")),
                new SqlParameter("@Spare",Spare)
            };
            var result = SqlHelper.ExecuteNonQuery(CommandType.Text, AddEmailSql, param);
            return result > 0;
        }

        /// <summary>
        /// 获取所有待发送邮件列表
        /// </summary>
        /// <returns></returns>
        public List<EmailModel> GetAllNeedSendEmail()
        {
            List<EmailModel> result = new List<EmailModel>();
            var ds = SqlHelper.ExecuteDataset(CommandType.Text, GetAllNeedSendEmailSql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = LoadDataList<EmailModel>(ds.Tables[0]);
            }
            return result;
        }

        /// <summary>
        /// 更新邮件列表为已发送
        /// </summary>
        /// <param name="models">邮件列表</param>
        public void UpdateEmailToSend(List<EmailModel> models)
        {
            if (models.Count == 0) return;
            StringBuilder sb = new StringBuilder();
            foreach (var model in models)
            {
                sb.Append(string.Format("{0},", model.Id));
            }
            var exeSql = string.Format(UpdateEmailToSendSql, sb.ToString().Trim(','));
            SqlHelper.ExecuteNonQuery(CommandType.Text, exeSql, null);
        }

        #region Sql语句
        /// <summary>
        /// 添加邮件记录的Sql语句
        /// </summary>
        private static string AddEmailSql = @"IF NOT EXISTS(SELECT TOP 1 1 FROM SCCEmail WHERE LotteryName=@LotteryName AND QiHao=@QiHao)
                                                BEGIN
	                                                INSERT INTO SCCEmail
	                                                SELECT @ID,@LotteryName,@QiHao,@OpenTime,0,GETDATE(),@Spare
                                                END";
        /// <summary>
        /// 获取所有待发送邮件列表的Sql语句
        /// </summary>
        private static string GetAllNeedSendEmailSql = @"SELECT ID,LotteryName,QiHao,OpenTime From SCCEmail WHERE IsSend = 0";
        /// <summary>
        /// 更新邮件为已发送的Sql语句
        /// </summary>
        private static string UpdateEmailToSendSql = @"Update SCCEmail SET IsSend = 1 WHERE ID in ({0})";
        #endregion
    }
}
