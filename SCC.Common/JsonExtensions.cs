using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace SCC.Common
{
    /// <summary>
    /// 转换Json格式帮助类
    /// </summary>
    public static class JsonExtensions
    {
        /// <summary>
        /// 对象序列化成Json字符串
        /// </summary>
        /// <param name="obj">需要序列化的对象</param>
        /// <param name="isIgnoreNullValue">是否忽略值为NULL的属性，默认false</param>
        /// <returns></returns>
        public static string TryToJson(this object obj, bool isIgnoreNullValue = false)
        {
            string res;
            try
            {
                if (isIgnoreNullValue)
                {
                    JsonSerializerSettings jsetting = new JsonSerializerSettings();

                    JsonConvert.DefaultSettings = () =>
                    {
                        //日期类型默认格式化处理
                        //jsetting.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
                        //jsetting.DateFormatString = "yyyy-MM-dd HH:mm:ss";

                        //空值处理,忽略值为NULL的属性
                        jsetting.NullValueHandling = NullValueHandling.Ignore;

                        return jsetting;
                    };
                    res = JsonConvert.SerializeObject(obj, Formatting.Indented, jsetting);
                }
                else
                {
                    res = JsonConvert.SerializeObject(obj);
                }
            }
            catch (Exception e)
            {
                LogHelper log = new LogHelper();
                log.Warn(typeof(JsonExtensions), "对象序列化成Json字符串失败，方法：TryToJson()，异常信息：" + e.Message);

                res = "";
            }
            return res;
        }

        /// <summary>
        /// Json字符串反序列化成对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static List<T> JsonToList<T>(this string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<List<T>>(json);
            }
            catch (Exception e)
            {
                LogHelper log = new LogHelper();
                log.Warn(typeof(JsonExtensions), "Json字符串序列化成对象失败，方法：JsonToList<T>()，异常信息：" + e.Message);
            }

            return default(List<T>);
        }

        /// <summary>
        /// Json字符串反序列化成实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T JsonToEntity<T>(this string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                LogHelper log = new LogHelper();
                log.Warn(typeof(JsonExtensions), "\r\n===============================================================================\r\n");
                log.Warn(typeof(JsonExtensions), "Json字符串序列化成对象失败，方法：JsonToEntity<T>()，异常信息：" + e.Message + "\r\n");

                log.Warn(typeof(JsonExtensions), json + "\r\n");
                log.Warn(typeof(JsonExtensions), "\r\n===============================================================================\r\n");
            }
            return default(T);
        }
    }
}
