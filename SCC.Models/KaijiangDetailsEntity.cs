using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCC.Models
{
    /// <summary>
    /// 开奖详情
    /// </summary>
   public class KaijiangDetailsEntity
    {

        /// <summary>
        /// 投入金额
        /// </summary>
        public string Trje { get; set; }
        /// <summary>
        /// 滚动金额
        /// </summary>
        public string Gdje { get; set; }
        /// <summary>
        /// 奖项详情
        /// </summary>
        public List<Kaijiangitem> KaiJiangItems { get; set; }


    }

    public class Kaijiangitem
    {
        /// <summary>
        /// 奖项
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 中奖数
        /// </summary>
        public string Total { get; set; }
        /// <summary>
        /// 奖金
        /// </summary>
        public string TotalMoney { get; set; }
    }

}
