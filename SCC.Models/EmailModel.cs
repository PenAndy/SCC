using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCC.Models
{
    /// <summary>
    /// 邮件模型
    /// </summary>
    public class EmailModel
    {
        /// <summary>
        /// 数据编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 彩种名称
        /// </summary>
        public string LotteryName { get; set; }
        /// <summary>
        /// 失败期号
        /// </summary>
        public string QiHao { get; set; }
        /// <summary>
        /// 开奖时间
        /// </summary>
        public DateTime OpenTime { get; set; }
    }
}
