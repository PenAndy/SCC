using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Configuration;

namespace SCC.Common
{
    /// <summary>
    /// 通过依赖注入方式实现控制反转
    /// </summary>
    public sealed class IOC
    {
        /// <summary>
        /// //通过接口名称和构造函数的参数得到实现
        /// </summary>
        /// <typeparam name="T">接口类型</typeparam>
        /// <param name="args">构造函数的参数</param>
        /// <returns>接口的实现</returns>
        public static T Resolve<T>(object[] args = null) where T : class
        {
            //获取类名
            string className = typeof(T).Name.Substring(1);
            //通过判断fullName中是否包含`符号来判断是否是泛型
            string fullName = typeof(T).FullName;
            int flag = fullName.IndexOf('`');
            //如果是泛型，需要特殊处理
            if (flag != -1)
            {
                int dot = fullName.LastIndexOf('.', flag);
                //这里去掉方法名前面的点和I
                className = fullName.Substring(dot + 2);
            }
            return DependencyInjector.GetClass<T>(className, args);
        }
    }
    /// <summary>
    /// 依赖注入
    /// </summary>
    public sealed class DependencyInjector
    {
        /// <summary>
        /// 根据名称和构造函数的参数加载相应的类
        /// </summary>
        /// <typeparam name="T">需要加载的类所实现的接口</typeparam>
        /// <param name="className">类的名称</param>
        /// <param name="args">构造函数的参数(默认为空)</param>
        /// <returns>类的接口</returns>
        public static T GetClass<T>(string className, object[] args = null) where T : class
        {
            //获取接口所在的命名空间
            string factoryName = typeof(T).Namespace;
            //通过依赖注入配置文件获取接口实现所在的命名空间
            string dllName = ConfigurationManager.AppSettings[factoryName];
            //获取类的全名
            string fullClassName = dllName + "." + className + "Services";
            //根据dll和类名，利用反射加载类
            object classObject = Assembly.Load(dllName).CreateInstance(fullClassName, true, BindingFlags.Default,null, args, null, null);
            return classObject as T;
        }
    }
}
