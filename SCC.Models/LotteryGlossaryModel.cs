using System;

namespace SCC.Models
{
    public class LotteryGlossaryModel
    {
        /// <summary>
        /// id
        /// </summary>
        public string  ID { get; set; }
        /// <summary>
        /// 类型Id
        /// </summary>
        public LotteryGlossaryType TypeID { get; set; }
        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 标题名称
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
        /// 内容来源地址
        /// </summary>
        public string SourceUrl { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }

    }
}