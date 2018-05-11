using System;
using System.Collections.Generic;

namespace SCC.Models
{
    /// <summary>
    /// 开彩网API基类
    /// </summary>
    public class OpenCaiBaseJson
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public int rows { get; set; }
        /// <summary>
        /// 彩票编码，形如ah11x5
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string info { get; set; }
        /// <summary>
        /// 开奖号实体
        /// </summary>
        public List<OpenCodeItem> data { get; set; }
    }

    /// <summary>
    /// 开奖号实体
    /// </summary>
    public class OpenCodeItem
    {
        /// <summary>
        /// 期数，2017111705
        /// </summary>
        public string expect { get; set; }
        /// <summary>
        /// 开奖号码，形如09,01,08,05
        /// </summary>
        public string opencode { get; set; }
        /// <summary>
        /// 开奖时间
        /// </summary>
        public string opentime { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public long opentimestamp { get; set; }

        /// <summary>
        /// code去0
        /// </summary>
        /// <returns></returns>
        public string GetOpenCodeStr()
        {
            string[] dataary = opencode.Split(',','+');
            List<int> list = new List<int>();
            foreach (var item in dataary)
            {
                list.Add(int.Parse(item));
            }
            return string.Join(",", list);
        }
        /// <summary>
        /// 期数去20标头
        /// </summary>
        /// <returns></returns>
        public string GetTermStr()
        {
            if (expect.Length >9)
            {
                return expect.Substring(2);
            }
            else
            {
                return expect;
            }
        }
    }
}