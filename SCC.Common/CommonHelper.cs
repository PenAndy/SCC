using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using System.Net;
using System.Net.Mail;

using SCC.Models;
using Quartz;

namespace SCC.Common
{
    /// <summary>
    /// 公用帮助类
    /// </summary>
    public static class CommonHelper
    {
        /// <summary>
        /// 本系统当前时间
        /// (早上8点之前为昨天，8点之后为今天)
        /// </summary>
        public static DateTime SCCSysDateTime
        {
            get
            {
                return DateTime.Now.AddHours(-8);
            }
        }

        /// <summary>
        /// 将XML内容转换成目标对象实体集合
        /// </summary>
        /// <typeparam name="T">目标对象实体</typeparam>
        /// <param name="FileName">完整文件名(根目录下只需文件名称)</param>
        /// <param name="WrapperNodeName"></param>
        /// <returns></returns>
        public static List<T> ConvertXMLToObject<T>(string FileName, string WrapperNodeName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(FileName);
            List<T> result = new List<T>();
            var TType = typeof(T);
            XmlNodeList nodeList = doc.ChildNodes;
            if (!string.IsNullOrEmpty(WrapperNodeName))
            {
                foreach (XmlNode node in doc.ChildNodes)
                {
                    if (node.Name == WrapperNodeName)
                    {
                        nodeList = node.ChildNodes;
                        break;
                    }
                }
            }
            object oneT = null;
            foreach (XmlNode node in nodeList)
            {
                if (node.NodeType == XmlNodeType.Comment || node.NodeType == XmlNodeType.XmlDeclaration) continue;
                oneT = TType.Assembly.CreateInstance(TType.FullName);
                foreach (XmlNode item in node.ChildNodes)
                {
                    if (item.NodeType == XmlNodeType.Comment) continue;
                    var property = TType.GetProperty(item.Name);
                    if (property != null)
                        property.SetValue(oneT, Convert.ChangeType(item.InnerText, property.PropertyType), null);
                }
                result.Add((T)oneT);
            }
            return result;
        }

        /// <summary>
        /// 从作业数据地图中获取配置信息
        /// </summary>
        /// <param name="datamap">作业数据地图</param>
        /// <returns></returns>
        public static SCCConfig GetConfigFromDataMap(JobDataMap datamap)
        {
            SCCConfig config = new SCCConfig();
            var properties = typeof(SCCConfig).GetProperties();
            foreach (PropertyInfo info in properties)
            {
                if (info.PropertyType == typeof(string))
                    info.SetValue(config, datamap.GetString(info.Name), null);
                else if (info.PropertyType == typeof(Int32))
                    info.SetValue(config, datamap.GetInt(info.Name), null);
            }
            return config;
        }

        #region 生成期号
        /// <summary>
        /// 通过期号编号生成形如YYMMDDQQ的期号
        /// </summary>
        /// <param name="QNum">期号编号</param>
        /// <returns></returns>
        public static string GenerateTodayQiHaoYYMMDDQQ(int QNum)
        {
            return GenerateQiHaoYYMMDDQQ(SCCSysDateTime, QNum);
        }
        /// <summary>
        /// 通过期号编号生成形如YYMMDDQQQ的期号
        /// </summary>
        /// <param name="QNum">期号编号</param>
        /// <returns></returns>
        public static string GenerateTodayQiHaoYYMMDDQQQ(int QNum)
        {
            return GenerateQiHaoYYMMDDQQQ(SCCSysDateTime, QNum);
        }
        /// <summary>
        /// 通过期号编号生成昨天形如YYMMDDQQ的期号
        /// </summary>
        /// <param name="QNum">期号编号</param>
        /// <returns></returns>
        public static string GenerateYesterdayQiHaoYYMMDDQQ(int QNum)
        {
            return GenerateQiHaoYYMMDDQQ(SCCSysDateTime.AddDays(-1), QNum);
        }
        /// <summary>
        /// 通过期号编号生成昨天形如YYMMDDQQ的期号
        /// </summary>
        /// <param name="QNum">期号编号</param>
        /// <returns></returns>
        public static string GenerateYesterdayQiHaoYYMMDDQQQ(int QNum)
        {
            return GenerateQiHaoYYMMDDQQQ(SCCSysDateTime.AddDays(-1), QNum);
        }
        /// <summary>
        /// 通过时间和编号生成当天形如YYMMDDQQ的期号
        /// </summary>
        /// <param name="dt">时间</param>
        /// <param name="QNum">期号编号</param>
        /// <returns></returns>
        private static string GenerateQiHaoYYMMDDQQ(DateTime dt, int QNum)
        {
            return dt.ToString("yyMMdd") + (QNum).ToString().PadLeft(2, '0');
        }
        /// <summary>
        /// 通过时间和编号生成当天形如YYMMDDQQQ的期号
        /// </summary>
        /// <param name="dt">时间</param>
        /// <param name="QNum">期号编号</param>
        /// <returns></returns>
        private static string GenerateQiHaoYYMMDDQQQ(DateTime dt, int QNum)
        {
            return dt.ToString("yyMMdd") + (QNum).ToString().PadLeft(3, '0');
        }
        /// <summary>
        /// 生成广西快乐十分指定日期指定编号的期号
        /// </summary>
        /// <param name="dt">时间</param>
        /// <param name="SkipDate">配置的跳过时间</param>
        /// <param name="QNum">期号编号</param>
        /// <returns></returns>
        public static string GenerateGXKL10FQiHao(DateTime dt, string SkipDate, int QNum)
        {
            TimeSpan datepart = dt - new DateTime(dt.Year, 1, 1);
            var t = SkipDate.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var beforeDTSkipDates = t.Where(R => Convert.ToInt32(R) < Convert.ToInt32(dt.ToString("yyyyMMdd"))).ToList();
            return dt.Year.ToString() + (datepart.Days - beforeDTSkipDates.Count + 1).ToString().PadLeft(3, '0') + QNum.ToString().PadLeft(2, '0');
        }
        #endregion


        #region 生成开奖时间
        /// <summary>
        /// 获取昨天对应期号的开奖时间
        /// 期号需形如YYMMDDQQ或YYMMDDQQQ
        /// </summary>
        /// <param name="config">配置项</param>
        /// <param name="QiHao">开奖期号</param>
        /// <returns></returns>
        public static DateTime GenerateYesterdayOpenTime(SCCConfig config, string QiHao)
        {
            var openDay = SCCSysDateTime.AddDays(-1);
            var StartTime = new DateTime(openDay.Year, openDay.Month, openDay.Day, config.StartHour, config.StartMinute, 0);
            return StartTime.AddMinutes((Convert.ToInt32(QiHao.Substring(6)) - 1) * config.Interval);
        }
        /// <summary>
        /// 获取今日对应期号的开奖时间
        /// 期号需形如YYMMDDQQ或YYMMDDQQQ
        /// </summary>
        /// <param name="config">配置项</param>
        /// <param name="QiHao">开奖期号</param>
        /// <returns></returns>
        public static DateTime GenerateTodayOpenTime(SCCConfig config, string QiHao)
        {
            var StartTime = new DateTime(SCCSysDateTime.Year, SCCSysDateTime.Month, SCCSysDateTime.Day, config.StartHour, config.StartMinute, 0);
            return StartTime.AddMinutes((Convert.ToInt32(QiHao.Substring(6)) - 1) * config.Interval);
        }
        public static DateTime GenerateGXKL10FYesterdayOpenTime(SCCConfig config, string QiHao)
        {
            var openDay = SCCSysDateTime.AddDays(-1);
            var StartTime = new DateTime(openDay.Year, openDay.Month, openDay.Day, config.StartHour, config.StartMinute, 0);
            return StartTime.AddMinutes((Convert.ToInt32(QiHao.Substring(7)) - 1) * config.Interval);
        }
        public static DateTime GenerateGXKL10FTodayOpenTime(SCCConfig config, string QiHao)
        {
            var StartTime = new DateTime(SCCSysDateTime.Year, SCCSysDateTime.Month, SCCSysDateTime.Day, config.StartHour, config.StartMinute, 0);
            return StartTime.AddMinutes((Convert.ToInt32(QiHao.Substring(7)) - 1) * config.Interval);
        }
        #endregion

        #region 日志信息
        public static string GetJobMainLogInfo(SCCConfig config, string QiHao)
        {
            return string.Format("【{0}】通过主站地址抓取{1}期开奖数据成功", config.Area + config.LotteryName, QiHao);
        }
        public static string GetJobBackLogInfo(SCCConfig config, string QiHao)
        {
            return string.Format("【{0}】通过备用地址抓取{1}期开奖数据成功", config.Area + config.LotteryName, QiHao);
        }
        public static string GetJobLogError(SCCConfig config, string QiHao)
        {
            return string.Format("【{0}】抓取{1}期开奖数据失败", config.Area + config.LotteryName, QiHao);
        }
    
        #endregion

        #region 地方彩
        /// <summary>
        /// 通过编号生成当天形如YYQQQ的期号
        /// (目前只有地方彩使用)
        /// </summary>
        /// <param name="QNum">期号编号</param>
        /// <returns></returns>
        public static int GenerateQiHaoYYQQQ(int QNum)
        {
            return Convert.ToInt32(SCCSysDateTime.ToString("yy") + (QNum).ToString().PadLeft(3, '0'));
        }
        /// <summary>
        /// 通过编号生成当天形如YYYYQQQ的期号
        /// (目前只有地方彩使用)
        /// </summary>
        /// <param name="QNum">期号编号</param>
        /// <returns></returns>
        public static int GenerateQiHaoYYYYQQQ(int QNum)
        {
            return Convert.ToInt32(SCCSysDateTime.ToString("yyyy") + (QNum).ToString().PadLeft(3, '0'));
        }
        /// <summary>
        /// 核实该地方彩程序运行时间是否应该获取到数据
        /// 开奖第二天检查时应该抓取到开奖数据才正确
        /// </summary>
        /// <param name="config">配置信息</param>
        /// <returns></returns>
        public static bool CheckDTIsNeedGetData(SCCConfig config)
        {
            var week = SCCSysDateTime.AddDays(-1).DayOfWeek.ToString("d");
            if (config.KJTime.Contains(week))//第二天只检查1次//&& SCCSysDateTime.Hour < 1
                return true;
            return false;
        }
        /// <summary>
        /// 检查今天该地方彩是否应该开奖
        /// 是则进行爬取工作
        /// </summary>
        /// <param name="config">配置信息</param>
        /// <returns></returns>
        public static bool CheckTodayIsOpenDay(SCCConfig config)
        {
            var week = SCCSysDateTime.DayOfWeek.ToString("d");
            if (config.KJTime.Contains(week))
                return true;
            return false;
        }
        /// <summary>
        /// 生成地方彩昨天的开奖时间
        /// </summary>
        /// <param name="config">配置信息</param>
        /// <returns></returns>
        public static DateTime GenerateDTOpenTime(SCCConfig config)
        {
            var openday = SCCSysDateTime.AddDays(-1);
            return new DateTime(openday.Year, openday.Month, openday.Day, config.StartHour, config.StartMinute, 0);
        }
        #endregion

        /// <summary>
        /// 将值转换为T类型数据
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="value">数据值</param>
        /// <returns></returns>
        public static T ChangeType<T>(object value)
        {
            return ChangeType<T>(value, default(T));
        }

        /// <summary>
        /// 将值转换为T类型数据，失败则返回T类型默认值
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="value">数据值</param>
        /// <param name="defaultValue">T类型默认值</param>
        /// <returns></returns>
        public static T ChangeType<T>(object value, T defaultValue)
        {
            if (value != null)
            {
                Type nullableType = typeof(T);
                if (!nullableType.IsInterface && (!nullableType.IsClass || (nullableType == typeof(string))))
                {
                    if (nullableType.IsGenericType && (nullableType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    {
                        return (T)Convert.ChangeType(value, Nullable.GetUnderlyingType(nullableType));
                    }
                    if (nullableType.IsEnum)
                    {
                        return (T)Enum.Parse(nullableType, value.ToString());
                    }
                    return (T)Convert.ChangeType(value, nullableType);
                }
                if (value is T)
                {
                    return (T)value;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// 将值转换为type类型的值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="type">目标类型</param>
        /// <returns></returns>
        public static object ChangeType(object value, Type type)
        {
            if (value != null)
            {
                var nullableType = Nullable.GetUnderlyingType(type);
                if (nullableType != null)//可空
                {
                    return Convert.ChangeType(value, nullableType);
                }
                if (Convert.IsDBNull(value))//特殊处理，由于数据库类型与项目中的类型定义不匹配
                    return type.IsValueType ? Activator.CreateInstance(type) : null;
                return Convert.ChangeType(value, type);
            }
            return null;
        }

        #region 邮件功能
        /// <summary>
        /// 收件人
        /// </summary>
        private static readonly string MailTo = ConfigHelper.GetConfigValue<string>("MailTo");
        /// <summary>
        /// 发自
        /// </summary>
        private static readonly string MailFrom = ConfigHelper.GetConfigValue<string>("MailFrom");
        /// <summary>
        /// 抄送
        /// </summary>
        private static readonly string MailCC = ConfigHelper.GetConfigValue<string>("MailCC");
        /// <summary>
        /// 发件名称
        /// </summary>
        private static readonly string SenderUserName = ConfigHelper.GetConfigValue<string>("SenderUserName");
        /// <summary>
        /// 密码
        /// </summary>
        private static readonly string SenderPassWord = ConfigHelper.GetConfigValue<string>("SenderPassWord");
        /// <summary>
        /// 主机
        /// </summary>
        private static readonly string SMTPHost = ConfigHelper.GetConfigValue<string>("SMTPHost");
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="subject">邮件标题</param>
        /// <param name="body">邮件正文</param>
        /// <returns></returns>
        public static bool SendEmail(string subject, string body)
        {
            try
            {
                if (string.IsNullOrEmpty(MailTo))
                    return false;

                MailMessage message = new MailMessage
                {
                    Subject = subject,
                    SubjectEncoding = Encoding.UTF8,
                    Body = body,
                    BodyEncoding = Encoding.UTF8,
                    IsBodyHtml = true,
                    Priority = MailPriority.High,
                    From = new MailAddress(MailFrom, SenderUserName)
                };

                string[] toList = MailTo.Trim().Split(new char[] { ';' });
                //添加收件人
                foreach (string to in toList)
                {
                    if (!string.IsNullOrWhiteSpace(to))
                        message.To.Add(new MailAddress(to));
                }

                //抄送人
                string[] ccList = MailCC.Trim().Split(new char[] { ';' });
                foreach (string cc in ccList)
                {
                    if (!string.IsNullOrWhiteSpace(cc))
                        message.CC.Add(new MailAddress(cc));
                }

                SmtpClient client = new SmtpClient(SMTPHost, 25)
                {
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new System.Net.NetworkCredential(SenderUserName, SenderPassWord),
                    EnableSsl = true
                };

                client.Send(message);
                return true;
            }
            catch
            {
            }
            return false;
        }
        #endregion

        #region
        /// <summary>
        /// 从unix时间戳转时间
        /// </summary>
        /// <param name="TimeStamp"></param>
        /// <returns></returns>
        public static DateTime UnixTimeStamp(long TimeStamp)
        {

            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime dt = startTime.AddSeconds(TimeStamp);
            return dt;
        }
        /// <summary>
        /// 从时间转换成unix时间戳
        /// </summary>
        /// <param name="DateTime"></param>
        /// <returns></returns>
        public static long UnixTimeStamp(DateTime DateTime)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            long timeStamp = (long)(DateTime - startTime).TotalSeconds; // 相差秒数
            return timeStamp;
        }

        /// <summary>
        /// 如果是"01,02,03"就转为"1,2,3"
        /// </summary>
        /// <param name="opcoede"></param>
        /// <returns></returns>
        public  static string TRHandleCode(string opencode)
        {
            string[] ary= opencode.Split(',');
            List<int> list = new List<int>();
            foreach (var item in ary)
            {
                list.Add(int.Parse(item));
            }
            return string.Join(",", list);
        }

        /// <summary>
        /// 把广东好彩1和广东36选7的期号转换
        /// </summary>
        /// <param name="term">yyyyqqq格式</param>
        /// <returns></returns>
        public static DateTime GetGDHC1orGD367Opentime(string term)
        {
            try
            {
            if (term.Length!=7)
            {
                return DateTime.Now;
            }
            string year = term.Substring(0, 4);
            int qihao = int.Parse(term.Substring(4, 3));
            DateTime time = new DateTime(int.Parse(year),1,1);
            return time.AddDays(qihao+6);
            }
            catch (Exception ee)
            {

                return DateTime.Now;
            }


        }
        #endregion


    }
}
