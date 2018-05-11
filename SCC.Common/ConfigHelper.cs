using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SCC.Common
{
    /// <summary>
    /// 配置管理类
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// 获取配置项的值
        /// </summary>
        /// <param name="key">配置项key</param>
        /// <returns></returns>
        public static object GetConfigValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// 获取指定返回类型的配置项值
        /// 如果类型不匹配则返回该返回类型默认值
        /// </summary>
        /// <typeparam name="T">指定返回类型</typeparam>
        /// <param name="key">配置项key</param>
        /// <returns></returns>
        public static T GetConfigValue<T>(string key)
        {
            try
            {
                object o = ConfigurationManager.AppSettings[key];
                if (o != null)
                {
                    return CommonHelper.ChangeType<T>(o);
                }
            }
            catch { }
            return default(T);
        }
    }
}
