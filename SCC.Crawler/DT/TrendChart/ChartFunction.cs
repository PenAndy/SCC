using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SCC.Models;

namespace SCC.Crawler.DT
{
    /// <summary>
    /// 走势图项值计算函数描述类
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ChartFunction : Attribute
    {
        /// <summary>
        /// 项值计算方法描述
        /// </summary>
        public string Name;
        /// <summary>
        /// 项值计算方法对应枚举类型
        /// </summary>
        public ChartItemType ItemType;
        /// <summary>
        /// 项处理类类型(处理项的类类型)
        /// </summary>
        public ChartItemClassName ClassName;

        public ChartFunction(string name, ChartItemType itemType, ChartItemClassName className)
        {
            this.Name = name;
            this.ItemType = itemType;
            this.ClassName = className;
        }
    }
}
