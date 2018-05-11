using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace SCC.Crawler.DT
{
    public static class Extends
    {
        /// <summary>
        /// 将string类型转换成int类型
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToInt(this string str)
        {
            int ret_number = -1;
            int.TryParse(str, out ret_number);
            return ret_number;
        }
        /// <summary>
        /// 将以splitChar分割的字符串转换为整形IList数组
        ///   注：如果字符串内包含不能转换为整形数据的字符，则返回NULL
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="splitChar">分隔符char</param>
        /// <returns></returns>
        public static IList<int> ToIntArray(this string source, char splitChar)
        {
            if (string.IsNullOrEmpty(source))
                return null;
            string[] data = source.Split(splitChar);
            int n = 0;
            IList<int> list = new List<int>();
            foreach (var item in data)
            {
                if (Int32.TryParse(item, out n))
                { list.Add(n); }
                else { return null; }
            }
            return list;
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
        /// <summary>
        /// 将string[]类型转换成string（以 第二个参数 分隔）
        /// </summary>
        /// <param name="str">数组</param>
        /// <param name="con">分隔符号</param>
        /// <returns></returns>
        public static string ArrayToString(this string[] str, char con)
        {
            if (str == null)
                return "";
            StringBuilder sb = new StringBuilder(str.Length * 50);
            foreach (var item in str)
            {
                sb.Append(item + con.ToString());
            }
            return sb.ToString().TrimEnd(con);
        }

        /// <summary>
        /// 将对象转换成JSON
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string TO_Josin(this object obj)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            try
            {
                return js.Serialize(obj);
            }
            catch //(Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// 将JSON类型转换成对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T TO_Object<T>(this string str)
        {
            System.Web.Script.Serialization.JavaScriptSerializer js =
                new System.Web.Script.Serialization.JavaScriptSerializer();
            try
            {
                return js.Deserialize<T>(str);
            }
            catch //(Exception ex)
            {
                return default(T);
            }
        }
        /// <summary>
        /// 将Json类型转换成 List对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="JsonStr"></param>
        /// <returns></returns>
        public static List<T> JSONStringToList<T>(this string JsonStr)
        {
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            List<T> objs = Serializer.Deserialize<List<T>>(JsonStr);
            return objs;
        }
    }
}
