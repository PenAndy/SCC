using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCC.Models
{
    /// <summary>
    /// 网络爬虫工具特性类
    /// </summary>
    class SCCSysAttribute
    {
    }

    /// <summary>
    /// 彩种表名称特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class TableNameAttribute : Attribute
    {
        private string _tablename;
        public string TableName { get { return _tablename; } }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="TableName">表名称</param>
        public TableNameAttribute(string TableName)
        {
            this._tablename = TableName;
        }
    }

    /// <summary>
    /// 彩种编码特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class LotteryCodeAttribute : Attribute
    {
        private string _code;
        public string Code { get { return _code; } }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="TableName">表名称</param>
        public LotteryCodeAttribute(string Code)
        {
            this._code = Code;
        }
    }
}
