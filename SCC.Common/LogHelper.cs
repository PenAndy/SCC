using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using log4net;
using log4net.Config;

namespace SCC.Common
{
    /// <summary>
    /// 日志帮助类
    /// </summary>
    public class LogHelper
    {
        private static readonly object _lock = new object();

        static LogHelper()
        {
            var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
            XmlConfigurator.ConfigureAndWatch(logCfg);
        }
        
        /// <summary>
        /// 输出日志到Log4Net
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ex"></param>
        public void Error(Type t, Exception ex)
        {
            lock (_lock)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(t);
                if (log.IsErrorEnabled)
                    log.Error("Error", ex);
            }
        }

        /// <summary>
        /// 输出日志到Log4Net
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ex"></param>
        public void Error(Type t, string msg)
        {
            lock (_lock)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(t);
                if (log.IsErrorEnabled)
                    log.Error(msg);
            }
        }

        /// <summary>
        /// 输出日志到Log4Net
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ex"></param>
        public void Info(Type t, Exception ex)
        {
            lock (_lock)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(t);
                if (log.IsInfoEnabled)
                    log.Info("Info", ex);
            }
        }

        /// <summary>
        /// 输出日志到Log4Net
        /// </summary>
        /// <param name="t"></param>
        /// <param name="msg"></param>
        public void Info(Type t, string msg)
        {
            lock (_lock)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(t);
                if (log.IsInfoEnabled)
                    log.Info(msg);
            }
        }

        /// <summary>
        /// 输出日志到Log4Net
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ex"></param>
        public void Warn(Type t, Exception ex)
        {
            lock (_lock)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(t);
                if (log.IsWarnEnabled)
                    log.Warn("Warn", ex);
            }
        }

        /// <summary>
        /// 输出日志到Log4Net
        /// </summary>
        /// <param name="t"></param>
        /// <param name="msg"></param>
        public void Warn(Type t, string msg)
        {
            lock (_lock)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(t);
                if (log.IsWarnEnabled)
                    log.Warn(msg);
            }
        }

        /// <summary>
        /// 输出日志到Log4Net
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ex"></param>
        public void Debug(Type t, Exception ex)
        {
            lock (_lock)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(t);
                if (log.IsDebugEnabled)
                    log.Debug("Debug", ex);
            }
        }

        /// <summary>
        /// 输出日志到Log4Net
        /// </summary>
        /// <param name="t"></param>
        /// <param name="msg"></param>
        public void Debug(Type t, string msg)
        {
            lock (_lock)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(t);
                if (log.IsDebugEnabled)
                    log.Debug(msg);
            }
        }
    }
}
