using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using SCC.Models;

namespace SCC.Crawler.DT
{
    /// <summary>
    /// 彩票相关术语计算
    /// </summary>
    public class LotteryUtils
    {
        /// <summary>
        /// 计算跨度;
        /// count:从左至右依次取号码个数;
        /// </summary>
        /// <param name="OpenCode">号码数组</param>
        /// <param name="count">从左至右依次取号码个数</param>
        /// <returns></returns>
        public static int GetSpan(IList<int> OpenCode, int count = 0)
        {
            List<int> list = new List<int>(OpenCode);
            int n = 0 >= count ? list.Count : count;
            n = n > list.Count ? list.Count - 1 : n - 1;
            for (int i = list.Count - 1; i > n; i--)
            { list.RemoveAt(i); }
            list.Sort();
            return Math.Abs(list[0] - list[list.Count - 1]);
        }
        /// <summary>
        /// 计算和值;
        /// count:从左至右依次取号码个数;
        /// </summary>
        /// <param name="OpenCode">号码数组</param>
        /// <param name="count">从左至右依次取号码个数</param>
        /// <returns></returns>
        public static int GetSum(IList<int> OpenCode, int count = 0)
        {
            List<int> list = new List<int>(OpenCode);
            int n = 0 >= count ? list.Count : count;
            n = n > list.Count ? list.Count - 1 : n - 1;
            for (int i = list.Count - 1; i > n; i--)
            { list.RemoveAt(i); }
            int sum = 0;
            foreach (int item in list)
            { sum = sum + item; }
            return sum;
        }
        /// <summary>
        /// 计算大小比例;
        /// 大于等于splitNumber算大数;
        /// count:从左至右依次取号码个数;
        /// </summary>
        /// <param name="OpenCode">号码数组</param>
        /// <param name="splitNumber">分隔数字</param>
        /// <param name="count">从左至右依次取号码个数</param>
        /// <returns></returns>
        public static string GetProportionOfDX(IList<int> OpenCode, int splitNumber, int count = 0)
        {
            List<int> list = new List<int>(OpenCode);
            int n = 0 >= count ? list.Count : count;
            n = n > list.Count ? list.Count - 1 : n - 1;
            for (int i = list.Count - 1; i > n; i--)
            { list.RemoveAt(i); }
            int a = 0, b = 0;
            foreach (int item in list)
            {
                if (item >= splitNumber)
                { a++; }
                else { b++; }
            }
            return a.ToString() + ":" + b.ToString();
        }
        /// <summary>
        /// 计算奇偶比例;
        /// count:从左至右依次取号码个数;
        /// </summary>
        /// <param name="OpenCode">号码数组</param>
        /// <param name="count">从左至右依次取号码个数</param>
        /// <returns></returns>
        public static string GetProportionOfJO(IList<int> OpenCode, int count = 0)
        {
            List<int> list = new List<int>(OpenCode);
            int n = 0 >= count ? list.Count : count;
            n = n > list.Count ? list.Count - 1 : n - 1;
            for (int i = list.Count - 1; i > n; i--)
            { list.RemoveAt(i); }
            int a = 0, b = 0;
            foreach (int item in list)
            {
                if (1 == item % 2)
                { a++; }
                else { b++; }
            }
            return a.ToString() + ":" + b.ToString();
        }
        /// <summary>
        /// 计算012比例
        /// </summary>
        /// <param name="OpenCode">号码数组</param>
        /// <returns></returns>
        public static string GetProportionOf012(IList<int> OpenCode)
        {
            int[] t = { 0, 0, 0 };
            foreach (var item in OpenCode)
            {
                t[item % 3]++;
            }
            return t[0].ToString() + ":" + t[1].ToString() + ":" + t[2].ToString();
        }
        /// <summary>
        /// 计算质合比例
        /// </summary>
        /// <param name="OpenCode"></param>
        /// <returns></returns>
        public static string GetProportionOfZh(IList<int> OpenCode)
        {
            int[] t = { 0, 0 };
            foreach (var item in OpenCode)
            {
                if (IsPrimeNumbers(item))
                    t[0]++;
                else
                    t[1]++;
            }
            return t[0].ToString() + ":" + t[1].ToString();
        }
        /// <summary>
        /// 判断是否为质数
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsPrimeNumbers(int number)
        {
            if (0 == number)
                return false;

            int iii = number / 2;
            for (int ii = 2; ii <= iii; ii++)
            {
                if (0 == number % ii)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 福彩3D判断大小
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool IsDxNumbers(int item)
        {
            if (item > 4)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 计算三区比(适用于双色球)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string SsqSanQu(IList<int> kjh)
        {
            int sanqu = 0, sanqu1 = 0, sanqu2 = 0;
            foreach (int item in kjh)
            {
                if (item >= 1 && item <= 11)
                {
                    sanqu++;
                }
                if (item >= 12 && item <= 22)
                {
                    sanqu1++;
                }
                if (item >= 23 && item <= 33)
                {
                    sanqu2++;
                }
            }



            return string.Format("{0}:{1}:{2}", sanqu, sanqu1, sanqu2);
        }
        /// <summary>
        /// 计算ac值
        /// </summary>
        /// <param name="kjh">需计算的开奖号码</param>
        /// <returns></returns>
        public static int GetAC(string[] kjh)
        {
            List<string> result = GetCombination(kjh, 2);
            ArrayList acarray = new ArrayList();
            int tpac = 0;
            for (int i = 0; i < result.Count; i++)
            {
                string[] tp = result[i].Split(',');
                int tmp = Math.Abs(Convert.ToInt32(tp[0]) - Convert.ToInt32(tp[1]));
                if (!acarray.Contains(tmp))
                {
                    tpac++;
                    acarray.Add(tmp);
                }
            }
            return tpac - (kjh.Length - 1);
        }
        public static List<string> GetCombination(string[] data, int count)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            List<string> output = new List<string>();
            for (int i = 0; i < data.Length; i++)
            {
                dic.Add(data[i], i);
            }
            SelectN(dic, data, count, 1, ref output);
            return output;
        }
        private static void SelectN(Dictionary<string, int> dd, string[] data, int count, int times, ref List<string> output)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();

            foreach (KeyValuePair<string, int> kv in dd)
            {
                for (int i = kv.Value + 1; i < data.Length; i++)
                {
                    if (times < count - 1)
                    {
                        dic.Add(kv.Key + "," + data[i], i);
                    }
                    else
                    {
                        output.Add(kv.Key + "," + data[i]);
                    }
                }
            }
            times++;
            if (dic.Count > 0)
            {
                SelectN(dic, data, count, times, ref output);
            }
        }
        /// <summary>
        /// 判断奇偶
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool IsJoNumbers(int item)
        {
            if (item % 2 == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 根据开奖号获取奇偶形态
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string GetJOString(IList<int> list)
        {
            StringBuilder sb = new StringBuilder(list.Count * 2);
            foreach (var item in list)
            {
                if (item % 2 == 0)
                {
                    sb.Append("偶");
                }
                else
                {
                    sb.Append("奇");
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 根据开奖号获取大小形态
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string GetDXString(IList<int> list, int splitNumber)
        {
            StringBuilder sb = new StringBuilder(list.Count * 2);
            foreach (var item in list)
            {
                if (item >= splitNumber)
                {
                    sb.Append("大");
                }
                else
                {
                    sb.Append("小");
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 根据开奖号获取质合形态
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string GetZHString(IList<int> list)
        {
            StringBuilder sb = new StringBuilder(list.Count * 2);
            bool zh;
            foreach (var item in list)
            {
                zh = true;
                if (item == 0)
                {
                    sb.Append("合");
                    continue;
                }
                for (int i = 2; i < item; i++)
                {
                    if (item % i == 0)
                    {
                        sb.Append("合");
                        zh = false;
                        break;
                    }
                }
                if (zh)
                {
                    sb.Append("质");
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 根据开奖号获取012形态
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string Get012String(IList<int> list)
        {
            StringBuilder sb = new StringBuilder(list.Count * 2);
            foreach (var item in list)
            {
                sb.Append((item % 3).ToString());
            }
            return sb.ToString();
        }
        /// <summary>
        /// 根据开奖号获取和尾值形态
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string GetHWString(IList<int> list)
        {
            string sum = GetSum(list).ToString();
            return sum[sum.Length - 1].ToString();
        }
        /// <summary>
        /// 模拟开奖号期数下拉列表
        /// </summary>
        /// <param name="topSize"></param>
        /// <param name="latestQi"></param>
        /// <param name="lottery"></param>
        /// <param name="formartUrl">格式化期数</param>
        /// <returns></returns>
        public static string GetLotteryDDLQi(int topSize, long latestQi, string lottery, string formartUrl)
        {
            StringBuilder sp = new StringBuilder(512);
            int minQi = 0, y = 0, j = 1, endQi = 0, line = 1, _qi = 0;
            int year = (int)latestQi / 1000;
            int startQi = year * 1000 + 1;
            switch (lottery)
            {
                case "qlc":
                    minQi = 2002001;
                    break;
                case "3d":
                    minQi = 2002001;
                    break;
                case "ssq":
                    minQi = 2003001;
                    break;
                case "dlt":
                    minQi = 2007001;
                    break;
                case "p3":
                    minQi = 2004001;
                    break;
                case "p5":
                    minQi = 2004001;
                    break;
                case "qxc":
                    minQi = 2006001;
                    break;
                case "3dshijihao":
                    minQi = 2002001;
                    break;
                case "p3shijihao":
                    minQi = 2004001;
                    break;
                case "hc1":
                    minQi = 2002001;
                    break;
            }
            while (line <= topSize)
            {
                _qi = (int)latestQi - j;
                if (_qi < minQi)
                    break;
                if (_qi < startQi)
                {
                    y = y + 1;
                    startQi = (year - y) * 1000 + 1;
                    endQi = (year - y) * 1000 + 153;
                    if (lottery == "3d" || lottery == "p3" || lottery == "p5" || lottery == "3dshijihao" || lottery == "p3shijihao" || lottery == "hc1")
                    {
                        endQi = (year - y) * 1000 + 358;
                        if (2014 == (year - y))
                            endQi = 2014357;
                    }
                    if (0 == ((year - y) % 4))
                    {
                        endQi = endQi + 1;
                    }
                    latestQi = endQi;
                    _qi = endQi;
                    j = 0;
                }
                sp.AppendFormat("<li><a href=\"{0}.htm\">{1}</a></li>", string.Format(formartUrl + _qi), _qi);
                j++;
                line++;
            }
            return sp.ToString();
        }
        /// <summary>
        /// 手机端开奖结果下拉
        /// </summary>
        /// <param name="topSize"></param>
        /// <param name="latestQi"></param>
        /// <returns></returns>
        public static string GetLotteryDDLQi(int topSize, long latestQi, string lottery)
        {
            StringBuilder sp = new StringBuilder(512);
            int minQi = 0, y = 0, j = 1, endQi = 0, line = 1, _qi = 0;
            int year = (int)latestQi / 1000;
            int startQi = year * 1000 + 1;
            switch (lottery)
            {
                case "qlc":
                    minQi = 2002001;
                    break;
                case "3d":
                    minQi = 2002001;
                    break;
                case "ssq":
                    minQi = 2003001;
                    break;
                case "dlt":
                    minQi = 2007001;
                    break;
                case "p3":
                    minQi = 2004001;
                    break;
                case "p5":
                    minQi = 2004001;
                    break;
                case "qxc":
                    minQi = 2006001;
                    break;
                case "3dshijihao":
                    minQi = 2002001;
                    break;
                case "p3shijihao":
                    minQi = 2004001;
                    break;
                case "hc1":
                    minQi = 2002001;
                    break;
                case "df6j1":
                    minQi = 2016059;
                    break;
            }
            while (line <= topSize)
            {
                _qi = (int)latestQi - j;
                if (_qi < minQi)
                    break;
                if (_qi < startQi)
                {
                    y = y + 1;
                    startQi = (year - y) * 1000 + 1;
                    endQi = (year - y) * 1000 + 153;
                    if (lottery == "3d" || lottery == "p3" || lottery == "p5" || lottery == "3dshijihao" || lottery == "p3shijihao" || lottery == "hc1" || lottery == "df6j1")
                    {
                        endQi = (year - y) * 1000 + 358;
                        if (2014 == (year - y))
                            endQi = 2014357;
                    }
                    if (0 == ((year - y) % 4))
                    {
                        endQi = endQi + 1;
                    }
                    latestQi = endQi;
                    _qi = endQi;
                    j = 0;
                }
                sp.AppendFormat("<option>{0}</option>", _qi);
                j++;
                line++;
            }
            return sp.ToString();
        }
        /// <summary>
        /// 模拟开奖号、试机号、开机号期数下拉列表
        /// </summary>
        /// <param name="topSize"></param>
        /// <param name="latestQi"></param>
        /// <param name="lottery"></param>
        /// <param name="formatHTML"></param>
        /// <param name="formatUrl"></param>
        /// <returns></returns>
        public static string GetLotteryDDLQi(int topSize, long latestQi, string lottery, string formatHTML, string formatUrl)
        {
            var sp = new StringBuilder(topSize * (formatHTML.Length + formatUrl.Length + 10));
            int minQi = 0, y = 0, j = 0, endQi = 0, line = 1, _qi = 0;
            int year = (int)latestQi / 1000;
            int startQi = year * 1000 + 1;
            switch (lottery)
            {
                case "qlc":
                    minQi = 2002001;
                    break;
                case "3d":
                    minQi = 2002001;
                    break;
                case "ssq":
                    minQi = 2003001;
                    break;
                case "dlt":
                    minQi = 2007001;
                    break;
                case "p3":
                    minQi = 2004001;
                    break;
                case "p5":
                    minQi = 2004001;
                    break;
                case "qxc":
                    minQi = 2006001;
                    break;
                case "3dshijihao":
                    minQi = 2002001;
                    break;
                case "p3shijihao":
                    minQi = 2004001;
                    break;
                case "hc1":
                    minQi = 2002001;
                    break;
            }
            while (line <= topSize)
            {
                _qi = (int)latestQi - j;
                if (_qi < minQi)
                    break;
                if (_qi < startQi)
                {
                    y = y + 1;
                    startQi = (year - y) * 1000 + 1;
                    endQi = (year - y) * 1000 + 153;
                    if (lottery == "3d" || lottery == "p3" || lottery == "p5" || lottery == "3dshijihao" || lottery == "p3shijihao" || lottery == "hc1")
                    {
                        endQi = (year - y) * 1000 + 358;
                        if (2014 == (year - y))
                            endQi = 2014357;
                    }
                    if (0 == ((year - y) % 4))
                    {
                        endQi = endQi + 1;
                    }
                    latestQi = endQi;
                    _qi = endQi;
                    j = 0;
                }
                sp.AppendFormat(formatHTML, string.Format(formatUrl, _qi), _qi);
                j++;
                line++;
            }
            return sp.ToString();
        }
        /// <summary>
        /// 获取开奖号码
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="indexStart"></param>
        /// <param name="indexEnd"></param>
        /// <returns></returns>
        public static IList<int> GetOpenCodeList<TEntity>(TEntity entity, int indexStart, int indexEnd) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(i); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            return list;
        }
        /// <summary>
        /// 福建31选7三区比
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string Fj31x7SanQu(IList<int> list)
        {
            int sanqu = 0, sanqu1 = 0, sanqu2 = 0;
            foreach (int item in list)
            {
                if (item >= 1 && item <= 10)
                {
                    sanqu++;
                }
                if (item >= 11 && item <= 20)
                {
                    sanqu1++;
                }
                if (item >= 21 && item <= 31)
                {
                    sanqu2++;
                }
            }



            return string.Format("{0}:{1}:{2}", sanqu, sanqu1, sanqu2);
        }
        /// <summary>
        /// 福建31选7三区比
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string Fj36x7SanQu(IList<int> list)
        {
            int sanqu = 0, sanqu1 = 0, sanqu2 = 0;
            foreach (int item in list)
            {
                if (item >= 1 && item <= 12)
                {
                    sanqu++;
                }
                if (item >= 13 && item <= 24)
                {
                    sanqu1++;
                }
                if (item >= 25 && item <= 36)
                {
                    sanqu2++;
                }
            }



            return string.Format("{0}:{1}:{2}", sanqu, sanqu1, sanqu2);
        }
        /// <summary>
        /// 华东15选5三区比
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string Hd15x5SanQu(IList<int> list)
        {
            int sanqu = 0, sanqu1 = 0, sanqu2 = 0;
            foreach (int item in list)
            {
                if (item >= 1 && item <= 5)
                {
                    sanqu++;
                }
                if (item >= 6 && item <= 10)
                {
                    sanqu1++;
                }
                if (item >= 11 && item <= 15)
                {
                    sanqu2++;
                }
            }



            return string.Format("{0}:{1}:{2}", sanqu, sanqu1, sanqu2);
        }
        /// <summary>
        /// 南粤36选7三区比
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string Ny36x7SanQu(IList<int> list)
        {
            int sanqu = 0, sanqu1 = 0, sanqu2 = 0;
            foreach (int item in list)
            {
                if (item >= 1 && item <= 12)
                {
                    sanqu++;
                }
                if (item >= 13 && item <= 24)
                {
                    sanqu1++;
                }
                if (item >= 25 && item <= 36)
                {
                    sanqu2++;
                }
            }



            return string.Format("{0}:{1}:{2}", sanqu, sanqu1, sanqu2);
        }
    }
}
