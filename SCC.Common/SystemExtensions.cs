using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SCC.Common
{
    /// <summary>
    /// 系统扩展方法
    /// </summary>
    public static class SystemExtensions
    {
        #region 字符串类扩展方法
        /// <summary>
        /// 生成手机号码的友好显示
        /// 形如133****1234
        /// </summary>
        /// <param name="s">电话号码</param>
        /// <returns></returns>
        public static string ToMobileNumReplaceShow(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return string.Empty;
            s = s.Trim();
            Regex reg = new Regex(@"^1[34578]\d{9}$");
            if (reg.IsMatch(s))
                s = s.Remove(3, 4).Insert(3, "****");
            return s;
        }

        /// <summary>
        /// 该字符串是否为电话号码
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns></returns>
        public static bool IsMobileString(this string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            s = s.Trim();
            Regex reg = new Regex(@"^1[34578]\d{9}$");
            return reg.IsMatch(s);
        }

        /// <summary>
        /// 将string[]类型转换成string（用|分隔）
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ArrayToString(this string[] str)
        {
            if (str == null)
                return "";
            StringBuilder sb = new StringBuilder(str.Length * 50);
            foreach (var item in str)
            {
                sb.Append(item + "|");
            }
            return sb.ToString().TrimEnd('|');
        }

        #endregion

        #region 时间类扩展方法

        /// <summary>
        /// 生成时间的友好提示
        /// 如：刚刚，多少秒前，多少小时前，多少天前
        /// </summary>
        /// <param name="dateTime">生成友好提示的时间对象</param>
        /// <param name="dateFormat">日期格式(可空，默认yyyy-MM-dd)</param>
        /// <param name="showTime">是否显示时间(可控，默认显示，格式HH:mm)</param>
        /// <returns></returns>
        public static string ToFriendlyString(this DateTime dateTime, string dateFormat = "", bool showTime = true)
        {
            if (dateTime == DateTime.MinValue)
                return "-";

            if (string.IsNullOrEmpty(dateFormat))
                dateFormat = "yyyy-MM-dd";

            string timeFormat = "HH:mm";

            DateTime userDate = dateTime;
            DateTime userNow = DateTime.Now;

            if (userDate > userNow)
            {
                return userDate.ToString(dateFormat + (showTime ? " " + timeFormat : ""));
            }

            TimeSpan intervalTime = userNow - userDate;

            int intervalDays;

            if (userNow.Year == userDate.Year)
                intervalDays = userNow.DayOfYear - userDate.DayOfYear;
            else
                intervalDays = intervalTime.Days + 1;

            string result = "{0}";
            if (showTime)
                result = "{0}" + " " + userDate.ToString(timeFormat);

            if (intervalDays > 7)
            {

                if (userDate.Year == userNow.Year)
                {
                    return string.Format("{0}月{1}日{2}",
                                         userDate.Month,
                                         userDate.Day,
                                         showTime ? " " + userDate.ToString(timeFormat) : "");
                }

                return userDate.ToString(dateFormat + (showTime ? " " + timeFormat : ""));
            }

            if (intervalDays >= 3)
            {
                string timeScope = string.Format("{0}天之前", intervalDays);
                return string.Format(result, timeScope);
            }

            if (intervalDays == 2)
            {
                return string.Format(result, "前天");
            }

            if (intervalDays == 1)
            {
                return string.Format(result, "昨天");
            }

            if (intervalTime.Hours >= 1)
                return string.Format("{0}小时之前", intervalTime.Hours);

            if (intervalTime.Minutes >= 1)
                return string.Format("{0}分钟之前", intervalTime.Minutes);

            if (intervalTime.Seconds >= 1)
                return string.Format("{0}秒之前", intervalTime.Seconds);

            return "现在";
        }

        #endregion

        #region 字典类扩展方法
        /// <summary>
        /// 从字典中获取key的值，若不存在或返回指定类型失败，则返回指定类型的默认值
        /// </summary>
        /// <typeparam name="T">指定返回类型</typeparam>
        /// <param name="dictionary">字典</param>
        /// <param name="key">获取的key</param>
        /// <param name="defaultValue">指定返回类型的默认值</param>
        /// <returns></returns>
        public static T Get<T>(this IDictionary<string, object> dictionary, string key, T defaultValue)
        {
            if (dictionary.ContainsKey(key))
            {
                object obj2;
                dictionary.TryGetValue(key, out obj2);
                return ChangeType<T>(obj2, defaultValue);
            }
            return defaultValue;
        }
        /// <summary>
        /// 向字典中尝试添加键值
        /// </summary>
        /// <typeparam name="TKey">该字典键类型</typeparam>
        /// <typeparam name="TValue">该字典值类型</typeparam>
        /// <param name="dictionary">字典</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (value != null && !dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            return dictionary;
        }
        #endregion

        #region 强制类型转换

        public static T ChangeType<T>(object value)
        {
            return ChangeType<T>(value, default(T));
        }

        public static T ChangeType<T>(object value, T defalutValue)
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
            return defalutValue;
        }
        #endregion

        #region 本系统时间扩展

        /// <summary>
        /// 生成指定的时间字符串
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string ToSCCDateTimeString1(this DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 生成指定的时间字符串
        /// yyyyMMddHHmmss
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string ToSCCDateTimeString2(this DateTime datetime)
        {
            return datetime.ToString("yyyyMMddHHmmss");
        }
        /// <summary>
        /// 生成指定的时间字符串
        /// yyyy-MM-dd
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string ToSCCDateTimeString3(this DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd");
        }
        #endregion
    }
}
