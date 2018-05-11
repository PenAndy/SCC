using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SCC.Models;

namespace SCC.Interface
{
    /// <summary>
    /// 邮件相关接口
    /// </summary>
    public interface IEmail
    {
        /// <summary>
        /// 添加邮件提醒记录
        /// </summary>
        /// <param name="LotteryName">彩种名称</param>
        /// <param name="QiHao">期号</param>
        /// <param name="OpenTime">开奖时间</param>
        /// <returns></returns>
        bool AddEmail(string LotteryName, string QiHao, DateTime OpenTime,string Spare="");

        /// <summary>
        /// 获取所有待发送邮件列表
        /// </summary>
        /// <returns></returns>
        List<EmailModel> GetAllNeedSendEmail();

        /// <summary>
        /// 更新邮件列表为已发送
        /// </summary>
        /// <param name="models">邮件列表</param>
        void UpdateEmailToSend(List<EmailModel> models);
    }
}
