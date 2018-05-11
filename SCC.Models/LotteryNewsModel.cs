using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCC.Models
{
    public class LotteryNewsModel
    {


        /// <summary>
        /// 编号
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        ///彩种分类
        /// </summary>
        public LotteryNewsType TypeID { get; set; }
        /// <summary>
        /// 彩种描述
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 增加时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 文章的URL
        /// </summary>
        public string SourceUrl { get; set; }
        /// <summary>
        /// 是否删除  truu 
        /// </summary>
        public bool IsDelete { get; set; }
    }
}
