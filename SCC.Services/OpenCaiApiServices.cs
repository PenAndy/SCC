using System;
using System.Net;
using System.Text;
using SCC.Common;
using SCC.Interface;
using SCC.Models;

namespace SCC.Services
{
    /// <summary>
    /// 调用开彩网接口服务
    /// </summary>
    public static class OpenCaiApiServices
    {
        private static readonly object _lock = new object();

        /// <summary>
        /// 调用开彩网接口服务
        /// </summary>
        static OpenCaiApiServices()
        {
            ApiHost = ConfigHelper.GetConfigValue<string>("OpenCaiApiHostMain") ?? ConfigHelper.GetConfigValue<string>("OpenCaiApiHostBack");
            //TODO 检测是否同主机能够通信

            helper = new HttpHelper();
            log = new LogHelper();
            email = IOC.Resolve<IEmail>();
        }

        /// <summary>
        /// 获取接口数据
        /// <para>备注：接口支持3种参数模式，分别如下：</para>
        /// <para>1、按最新查询，只需要传入code、rows（如有需要）参数即可</para>
        /// <para>2、按最新查询（批量），只需要传入code（多个用逗号隔开）、rows（如有需要）参数即可</para>
        /// <para>3、按开奖日期查询，只需要传入code、date参数即可</para>
        ///  </summary>
        /// <param name="openCaiApiArg"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static OpenCaiBaseJson GetOpenCaiApiData(OpenCaiApiArg openCaiApiArg, Type type = null)
        {
            OpenCaiBaseJson baseJson = new OpenCaiBaseJson();
            lock (_lock)
            {
                try
                {
                    //按开奖日期查询地址修正
                    string action = !string.IsNullOrEmpty(openCaiApiArg.date) ? "daily" : "newly";
                    //最终接口地址
                    string url = string.Format(ApiHost, GetRequsetArg(openCaiApiArg), action);

                    //组装参数
                    HttpItem item = new HttpItem
                    {
                        Url = url,
                        Method = "GET",
                        ContentType = "application/json",
                        Timeout = 90 * 1000,
                        ReadWriteTimeout = 90 * 1000,
                        Encoding = Encoding.UTF8
                    };
                    //开始请求
                    HttpResult result = helper.GetHtml(item);
                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        string jsonStr = result.Html;
                        if (!string.IsNullOrEmpty(jsonStr))
                        {
                            if (jsonStr.IndexOf("请求频率过快", StringComparison.Ordinal) < 0)
                            {
                                baseJson = jsonStr.JsonToEntity<OpenCaiBaseJson>();
                            }
                            else
                            {
                                log.Debug(type ?? typeof(OpenCaiApiServices), "调用了接口，请求参数：" + openCaiApiArg.TryToJson() + "，请求地址："+ url + "\r\n");
                            }
                        }
                    }
                    else
                    {
                        log.Error(typeof(OpenCaiApiServices), "请求接口[" + url + "]失败，状态码：" + result.StatusCode);
                    }
                }
                catch (Exception e)
                {
                    log.Error(typeof(OpenCaiApiServices), e);
                }
                //finally
                //{
                //    log.Debug(type, "调用了接口，请求参数：" + openCaiApiArg.TryToJson() + "\r\n");
                //}
            }
            return baseJson;
        }

        /// <summary>
        /// 组装请求参数
        /// </summary>
        /// <param name="openCaiApiArg"></param>
        /// <returns></returns>
        private static string GetRequsetArg(OpenCaiApiArg openCaiApiArg)
        {
            StringBuilder builder = new StringBuilder();
            if (!string.IsNullOrEmpty(openCaiApiArg.token))
            {
                builder.Append(string.Format("token={0}&", openCaiApiArg.token));
            }
            if (!string.IsNullOrEmpty(openCaiApiArg.code))
            {
                builder.Append(string.Format("code={0}&", openCaiApiArg.code));
            }
            if (openCaiApiArg.rows > 0)
            {
                builder.Append(string.Format("rows={0}&", openCaiApiArg.rows));
            }
            if (!string.IsNullOrEmpty(openCaiApiArg.format))
            {
                builder.Append(string.Format("format={0}&", openCaiApiArg.format));
            }
            if (!string.IsNullOrEmpty(openCaiApiArg.date))
            {
                builder.Append(string.Format("date={0}&", openCaiApiArg.date));
            }
            if (!string.IsNullOrEmpty(openCaiApiArg.callback))
            {
                builder.Append(string.Format("callback={0}", openCaiApiArg.callback));
            }
            return StringHelper.DelLastChar(builder.ToString(), "&");
        }

        #region Attribute
        /// <summary>
        /// 日志对象
        /// </summary>
        private static LogHelper log = null;
        /// <summary>
        /// 邮件接口
        /// </summary>
        private static IEmail email = null;
        /// <summary>
        /// HttpHelper
        /// </summary>
        private static HttpHelper helper = null;
        /// <summary>
        /// 开彩网接口地址
        /// </summary>
        private static string ApiHost = String.Empty;
        #endregion
    }
}