using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using SCC.Models;

namespace SCC.Common
{
    /// <summary>
    /// 枚举帮助类
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// 获取彩种对应数据库表名称
        /// </summary>
        /// <param name="value">彩种枚举类型</param>
        /// <returns></returns>
        public static string GetSCCLotteryTableName(Enum value)
        {
            if (value == null)
            {
                throw new ArgumentException("value");
            }
            string tableName = value.ToString();
            var fieldInfo = value.GetType().GetField(tableName);
            var attributes = (TableNameAttribute[])fieldInfo.GetCustomAttributes(typeof(TableNameAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                tableName = attributes[0].TableName;
            }
            return tableName;
        }

        /// <summary>
        /// 获取彩种对应编码
        /// </summary>
        /// <param name="value">彩种枚举类型</param>
        /// <returns></returns>
        public static string GetLotteryCode(Enum value)
        {
            if (value == null)
            {
                throw new ArgumentException("value");
            }
            string code = value.ToString();
            var fieldInfo = value.GetType().GetField(code);

            var attributes = (LotteryCodeAttribute[])fieldInfo.GetCustomAttributes(typeof(LotteryCodeAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                code = attributes[0].Code;
            }
            return code;
        }

        /// <summary>
        /// 获取枚举值描述信息
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription(this Enum value)
        {
            if (value == null)
            {
                throw new ArgumentException("value");
            }
            string code = value.ToString();
            var fieldInfo = value.GetType().GetField(code);

            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                code = attributes[0].Description;
            }
            return code;
        }
    }
}
