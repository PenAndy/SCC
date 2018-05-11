using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Quartz;
using SCC.Interface;
using SCC.Common;
using SCC.Models;

namespace SCC.Crawler
{
    /// <summary>
    /// 发送邮件的作业
    /// </summary>
    public class SendEmailJob : IJob
    {
        private IEmail email = null;
        private LogHelper log = null;
        public SendEmailJob()
        {
            email = IOC.Resolve<IEmail>();
            log = new LogHelper();
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                log.Info(typeof(SendEmailJob), "正在执行发送邮件作业！");
                var mailConfig = ConfigHelper.GetConfigValue<bool>("MailTurnOn");
                if (mailConfig)
                {
                    List<EmailModel> sendList = email.GetAllNeedSendEmail();
                    if (sendList.Count == 0) return;

                    var emailSubject = string.Format("{0}第{1}期等一共{2}条抓取失败日志", sendList[0].LotteryName, sendList[0].QiHao, sendList.Count);
                    StringBuilder emailBody = new StringBuilder();
                    emailBody.Append("抓取失败的彩种及期号列表：\r\n");
                    emailBody.Append("<table style='border-collapse:collapse'><tr><th style='border:1px solid gray;'>彩种</th><th style='border:1px solid gray;'>期号</th><th style='border:1px solid gray;'>开奖时间</th></tr>");

                    foreach (var mail in sendList)
                    {
                        emailBody.Append(string.Format("<tr><td style='border:1px solid gray;'>{0}</td><td style='border:1px solid gray;'>{1}</td><td style='border:1px solid gray;'>{2}</td></tr>", mail.LotteryName, mail.QiHao, mail.OpenTime.ToString("yyyy-MM-dd HH:mm:ss")));
                    }
                    emailBody.Append("</table>");

                    if (CommonHelper.SendEmail(emailSubject, emailBody.ToString()))
                    {
                        email.UpdateEmailToSend(sendList);
                        log.Info(typeof(SendEmailJob), "今日发送邮件成功！");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(typeof(SendEmailJob), string.Format("发送邮件列表时发生错误，错误信息【{1}】", ex.Message));
            }
        }
    }
}
