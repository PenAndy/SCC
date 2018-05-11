using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Diagnostics;

namespace SCC.Common
{
    /// <summary>
    /// 网络相关帮助类
    /// </summary>
    public class NetHelper
    {
        /// <summary>
        /// 获取IP
        /// </summary>
        /// <returns></returns>
        public static string GetIP()
        {
            string ip = string.Empty;
            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"]))
                ip = Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
            if (string.IsNullOrEmpty(ip))
                ip = Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
            if (string.IsNullOrEmpty(ip))
                ip = System.Web.HttpContext.Current.Request.UserHostAddress;
            return ip;
        }

        /// <summary>
        /// 通过IP地址获取所属国家，省份，城市
        /// </summary>
        /// <param name="IP"></param>
        /// <returns></returns>
        public static List<string> GetCityByIP(string IP)
        {
            var url = string.Format(@"http://int.dpool.sina.com.cn/iplookup/iplookup.php?ip={0}", IP);
            var htmlsource = GetUrlResponse(url, Encoding.GetEncoding("gbk"));
            if (!string.IsNullOrWhiteSpace(htmlsource))
            {
                string[] sarray = htmlsource.Trim().Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                if (sarray.Length == 6)
                {
                    List<string> result = new List<string>();
                    result.Add(sarray[3]);
                    result.Add(sarray[4]);
                    result.Add(sarray[5]);
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        /// 请求Url资源
        /// </summary>
        /// <param name="url">目标Url地址</param>
        /// <returns></returns>
        public static string GetUrlResponse(string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
                return GetUrlResponse(url, Encoding.UTF8);
            return string.Empty;
        }
        /// <summary>
        /// 请求Url资源
        /// </summary>
        /// <param name="url">目标Url地址</param>
        /// <param name="encode">编码规则</param>
        /// <returns></returns>
        public static string GetUrlResponse(string url, Encoding encode)
        {
            if (!string.IsNullOrWhiteSpace(url) && encode != null)
                return GetUrlResponse(url, "GET", string.Empty, encode);
            return string.Empty;
        }
        /// <summary>
        /// 请求Url资源
        /// </summary>
        /// <param name="url">目标Url地址</param>
        /// <param name="method">请求方式</param>
        /// <param name="postdata">请求附加数据</param>
        /// <returns></returns>
        public static string GetUrlResponse(string url, string method, string postdata)
        {
            if (!string.IsNullOrWhiteSpace(url) && !string.IsNullOrWhiteSpace(method) && !string.IsNullOrWhiteSpace(postdata))
                return GetUrlResponse(url, method, postdata, Encoding.UTF8);
            return string.Empty;
        }
        /// <summary>
        /// 请求Url资源
        /// </summary>
        /// <param name="url">目标Url地址</param>
        /// <param name="method">请求方式</param>
        /// <param name="postdata">请求附加数据</param>
        /// <param name="encode">编码规则</param>
        /// <returns></returns>
        public static string GetUrlResponse(string url, string method, string postdata, Encoding encode)
        {
            Stream responseStream = null;
            try
            {
                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Method = method;
                request.Timeout = 15 * 1000;

                if (!string.IsNullOrEmpty(postdata))
                {
                    byte[] bs = encode.GetBytes(postdata);
                    request.ContentLength = bs.Length;
                    request.ContentType = "application/x-www-form-urlencoded";
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(bs, 0, bs.Length);
                    }
                }
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var contentencode = response.ContentEncoding;
                    if (contentencode == "gzip")
                        responseStream = new System.IO.Compression.GZipStream(response.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
                    else
                        responseStream = response.GetResponseStream();

                    using (StreamReader sr = new StreamReader(responseStream, encode))
                    {
                        var content = sr.ReadToEnd();
                        return content.Replace("\r\n", string.Empty).Trim();
                    }
                }
                else
                {
                    Trace.WriteLine($"SCC.Common.NetHelper.GetUrlResponse执行错误，请求地址:{url}，状态码：{response.StatusCode}，{response.StatusDescription}.");
                    return null;
                }
            }
            catch( Exception e)
            {
                Trace.WriteLine($"SCC.Common.NetHelper.GetUrlResponse发生异常，请求地址:{url}，异常信息{e.Message}.");

                return null;
            }
            finally
            {
                if (responseStream != null)
                    responseStream.Close();
            }
        }

        #region 部分彩种自有请求网络数据方法
        /// <summary>
        /// 请求河南481(泳坛夺金)Url资源
        /// </summary>
        /// <param name="url">目标Url地址</param>
        /// <returns></returns>
        public static string GetHeNan481UrlResponse(string url)
        {
            Stream responseStream = null;
            try
            {
                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";
                request.Timeout = 10000;
                CookieContainer container = new CookieContainer();
                Cookie cookie = new Cookie("JSESSIONID", "76322438526518B500CEB13023461A22", "/", "www.hnlottery.com.cn");
                container.Add(cookie);
                request.CookieContainer = container;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                var contentencode = response.ContentEncoding;
                if (contentencode == "gzip")
                    responseStream = new System.IO.Compression.GZipStream(response.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
                else
                    responseStream = response.GetResponseStream();
                using (StreamReader sr = new StreamReader(responseStream, Encoding.UTF8))
                {
                    var content = sr.ReadToEnd();
                    return content.Replace("\r\n", string.Empty).Trim();
                }
            }
            catch
            {
            }
            finally
            {
                if (responseStream != null)
                    responseStream.Close();
            }
            return null;
        }
        /// <summary>
        /// 请求湖北11选5Url资源
        /// </summary>
        /// <param name="url">目标Url地址</param>
        /// <returns></returns>
        public static string GetHUB11X5UrlResponse(string url)
        {
            Stream responseStream = null;
            try
            {
                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";
                request.Timeout = 10000;
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                var contentencode = response.ContentEncoding;
                if (contentencode == "gzip")
                    responseStream = new System.IO.Compression.GZipStream(response.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
                else
                    responseStream = response.GetResponseStream();
                using (StreamReader sr = new StreamReader(responseStream, Encoding.UTF8))
                {
                    var content = sr.ReadToEnd();
                    return content.Replace("\r\n", string.Empty).Trim();
                }
            }
            catch
            {
            }
            finally
            {
                if (responseStream != null)
                    responseStream.Close();
            }
            return null;
        }
        /// <summary>
        /// 请求山西11选5Url资源
        /// </summary>
        /// <param name="url">目标Url地址</param>
        /// <returns></returns>
        public static string GetShanXiTaiYuan11x5UrlResponse(string url)
        {
            Stream responseStream = null;
            try
            {
                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";
                request.Timeout = 10000;
                CookieContainer container = new CookieContainer();
                Cookie cookie = new Cookie("JSESSIONID", "F76502A5D6582D7286C9CE7656FDAAEF", "/", "www.sxlottery.net");
                container.Add(cookie);
                request.CookieContainer = container;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                var contentencode = response.ContentEncoding;
                if (contentencode == "gzip")
                    responseStream = new System.IO.Compression.GZipStream(response.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
                else
                    responseStream = response.GetResponseStream();
                using (StreamReader sr = new StreamReader(responseStream, Encoding.UTF8))
                {
                    var content = sr.ReadToEnd();
                    return content.Replace("\r\n", string.Empty).Trim();
                }
            }
            catch
            {
            }
            finally
            {
                if (responseStream != null)
                    responseStream.Close();
            }
            return null;
        }
        /// <summary>
        /// 请求重庆快乐十分Url资源
        /// </summary>
        /// <param name="url">目标Url地址</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public static string GetCQKL10FUrlResponse(string url, int pageIndex)
        {
            Stream responseStream = null;
            try
            {
                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Method = "POST";
                request.Timeout = 10000;
                CookieContainer container = new CookieContainer();
                Cookie cookie = new Cookie("ASP.NET_SessionId", "4e0ri0cjp5pyjkigils0wzvy", "/", "buy.cqcp.net");
                container.Add(cookie);
                request.CookieContainer = container;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36";
                var postdata = "sPass=BEAB95B0BAA1242CF042D1659686F54B&idMode=9&iType=2&iCount=" + pageIndex;
                byte[] bs = Encoding.GetEncoding("gb2312").GetBytes(postdata);
                request.ContentLength = bs.Length;
                request.ContentType = "application/x-www-form-urlencoded";
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bs, 0, bs.Length);
                }

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                var contentencode = response.ContentEncoding;
                if (contentencode == "gzip")
                    responseStream = new System.IO.Compression.GZipStream(response.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
                else
                    responseStream = response.GetResponseStream();
                using (StreamReader sr = new StreamReader(responseStream, Encoding.GetEncoding("gb2312")))
                {
                    var content = sr.ReadToEnd();
                    return content.Replace("\r\n", string.Empty).Trim();
                }
            }
            catch
            {
            }
            finally
            {
                if (responseStream != null)
                    responseStream.Close();
            }
            return null;
        }
        /// <summary>
        /// 请求百度彩票Url资源
        /// </summary>
        /// <param name="url">目标Url地址</param>
        /// <returns></returns>
        public static string GetBaiDuLeCaiResponse(string url)
        {
            Stream responseStream = null;
            try
            {
                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";
                request.Timeout = 10000;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36";
                request.Headers.Add("Accept-Encoding", "gzip, deflate, sdch");
                request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                CookieContainer container = new CookieContainer();
                Cookie cookie = new Cookie("lehecai_request_control_stats", "2", "/", "baidu.lecai.com");
                container.Add(cookie);
                cookie = new Cookie("_lcas_uuid", "1886243347", "/", "baidu.lecai.com");
                container.Add(cookie);
                cookie = new Cookie("_lhc_uuid", "sp_587888388668b5.45001252", "/", "baidu.lecai.com");
                container.Add(cookie);
                request.CookieContainer = container;

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                var contentencode = response.ContentEncoding;
                if (contentencode == "gzip")
                    responseStream = new System.IO.Compression.GZipStream(response.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
                else
                    responseStream = response.GetResponseStream();
                using (StreamReader sr = new StreamReader(responseStream, Encoding.GetEncoding("gb2312")))
                {
                    var content = sr.ReadToEnd();
                    return content.Replace("\r\n", string.Empty).Trim();
                }
            }
            catch
            {
            }
            finally
            {
                if (responseStream != null)
                    responseStream.Close();
            }
            return null;
        }
        /// <summary>
        /// 请求甘肃11选5Url资源
        /// </summary>
        /// <param name="url">目标Url地址</param>
        /// <returns></returns>
        public static string GetGS11X5UrlResponse(string url)
        {
            Stream responseStream = null;
            try
            {
                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";
                request.Timeout = 10000;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36";
                request.Headers.Add("Accept-Encoding", "gzip, deflate, sdch");
                request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                CookieContainer container = new CookieContainer();
                Cookie cookie = new Cookie("sid", "d4cc22d9-9b4c-49d0-8965-61e3fdbe8167", "/", "www.gstc.org.cn");
                container.Add(cookie);
                request.CookieContainer = container;

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                var contentencode = response.ContentEncoding;
                if (contentencode == "gzip")
                    responseStream = new System.IO.Compression.GZipStream(response.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
                else
                    responseStream = response.GetResponseStream();
                using (StreamReader sr = new StreamReader(responseStream, Encoding.GetEncoding("gb2312")))
                {
                    var content = sr.ReadToEnd();
                    return content.Replace("\r\n", string.Empty).Trim();
                }
            }
            catch
            {
            }
            finally
            {
                if (responseStream != null)
                    responseStream.Close();
            }
            return null;
        }
        /// <summary>
        /// 请求吉林11选5Url资源
        /// </summary>
        /// <param name="url">目标Url地址</param>
        /// <param name="postdata">传送数据</param>
        /// <returns></returns>
        public static string GetJL11X5UrlResponse(string url, string postdata)
        {
            Stream responseStream = null;
            try
            {
                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Method = "POST";
                request.Timeout = 10000;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36";
                request.Headers.Add("Accept-Encoding", "gzip, deflate, sdch");
                request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                request.Referer = "http://jl.lottery.gov.cn/11x5.aspx";
                //CookieContainer container = new CookieContainer();
                //Cookie cookie = new Cookie("sid", "d4cc22d9-9b4c-49d0-8965-61e3fdbe8167", "/", "www.gstc.org.cn");
                //container.Add(cookie);
                //request.CookieContainer = container;
                byte[] bs = Encoding.UTF8.GetBytes(postdata);
                request.ContentLength = bs.Length;
                request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bs, 0, bs.Length);
                }

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                var contentencode = response.ContentEncoding;
                if (contentencode == "gzip")
                    responseStream = new System.IO.Compression.GZipStream(response.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
                else
                    responseStream = response.GetResponseStream();
                using (StreamReader sr = new StreamReader(responseStream, Encoding.UTF8))
                {
                    var content = sr.ReadToEnd();
                    return content.Replace("\r\n", string.Empty).Trim();
                }
            }
            catch
            {
            }
            finally
            {
                if (responseStream != null)
                    responseStream.Close();
            }
            return null;
        }
        /// <summary>
        /// 从彩票控爬取山西快乐十分开奖资源
        /// </summary>
        /// <param name="url">彩票控地址</param>
        /// <returns></returns>
        public static string GetShanXiTaiYuanKL10FResource(string url)
        {
            Stream responseStream = null;
            try
            {
                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";
                request.Timeout = 10000;
                request.Referer = "https://www.baidu.com/link?url=LlPuYDRBAgub8jzVvo8sugb4Js5-NQDPuKAGXZrc-UNS2AiizIzaKJC0C90Jh0rWEK7qargFxEp2lKxQwx1Vxa&wd=&eqid=abe817410018bc0a0000000557393f77";
                request.Host = "www.caipiaokong.com";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                request.Headers.Add("Accept-Encoding", "gzip, deflate, sdch");
                request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.87 Safari/537.36";
                request.Headers.Add("Cache-Control", "max-age=0");
                request.Headers.Add("Upgrade-Insecure-Requests", "1");
                request.Headers.Add("Cookie", "BAIDU_SSP_lcr=https://www.baidu.com/link?url=LlPuYDRBAgub8jzVvo8sugb4Js5-NQDPuKAGXZrc-UNS2AiizIzaKJC0C90Jh0rWEK7qargFxEp2lKxQwx1Vxa&wd=&eqid=abe817410018bc0a0000000557393f77; __cfduid=d506c4c6a0c7f88d91869fa379b2cb1421462343508; caipiaokong_4891_saltkey=MsvISvSd; caipiaokong_4891_lastvisit=1462339908; caipiaokong_4891_caipiaokong_eNr=1; caipiaokong_4891_lastact=1463370823%09index.php%09shxklsf; Hm_lvt_1fa650cb7d8eae53d0e6fbd8aec3eb67=1463369580,1463369608,1463369847,1463370211; Hm_lpvt_1fa650cb7d8eae53d0e6fbd8aec3eb67=1463370839");
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                var contentencode = response.ContentEncoding;
                if (contentencode == "gzip")
                    responseStream = new System.IO.Compression.GZipStream(response.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
                else
                    responseStream = response.GetResponseStream();
                using (StreamReader sr = new StreamReader(responseStream, Encoding.GetEncoding("gb2312")))//.UTF8))
                {
                    var content = sr.ReadToEnd();
                    return content;
                }
            }
            catch
            {
            }
            finally
            {
                if (responseStream != null)
                    responseStream.Close();
            }
            return string.Empty;
        }

        #endregion
        /// <summary>
        /// 保存网络图片
        /// </summary>
        /// <param name="ImageUrl">网络图片地址</param>
        /// <param name="SavePath">图片保存文件夹</param>
        /// <param name="SaveUrl">对应URL访问基础地址</param>
        /// <returns></returns>
        public static string SaveNetImage(string ImageUrl, string SavePath, string SaveUrl)
        {
            return SaveNetImage(ImageUrl, SavePath, SaveUrl, string.Empty);
        }

        /// <summary>
        /// 保存网络图片
        /// </summary>
        /// <param name="ImageUrl">网络图片地址</param>
        /// <param name="SavePath">图片保存文件夹</param>
        /// <param name="viewUrl">对应URL访问基础地址</param>
        /// <param name="FixFolderName">固定文件夹名(用于相同类型图片放在一个文件夹中，可空)</param>
        /// <returns></returns>
        public static string SaveNetImage(string ImageUrl, string SavePath, string viewUrl, string FixFolderName)
        {
            try
            {
                HttpWebRequest request = HttpWebRequest.Create(ImageUrl) as HttpWebRequest;
                request.Method = "GET";
                request.Timeout = 5000;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.143 Safari/537.36";
                request.KeepAlive = true;
                request.ProtocolVersion = HttpVersion.Version10;
                request.Headers.Add("Cookie", "__cfduid=d53136a7fdcb2c4c64f8ed24a66b32f251476954305");
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Image img = Image.FromStream(response.GetResponseStream());
                var today = DateTime.Now.ToString("yyyyMMdd", DateTimeFormatInfo.InvariantInfo);
                var saveFolder = Path.Combine(SavePath, today, FixFolderName);
                if (!Directory.Exists(saveFolder))
                {
                    Directory.CreateDirectory(saveFolder);
                }
                var fileExt = Path.GetExtension(ImageUrl).ToLower();
                string[] imgExt = new string[] { ".gif", ".jpg", ".jpeg", ".png", ".bmp" };
                if (string.IsNullOrWhiteSpace(fileExt) || Array.IndexOf(imgExt, fileExt) == -1)
                    fileExt = ".jpg";
                var savefile = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + fileExt;
                img.Save(Path.Combine(saveFolder, savefile));
                return UrlCombine(viewUrl, today, FixFolderName, savefile);
            }
            catch
            {
            }
            return string.Empty;
        }
        /// <summary>
        /// URL路径拼写
        /// </summary>
        /// <param name="paths">路径参数列表</param>
        /// <returns></returns>
        public static string UrlCombine(params string[] paths)
        {
            if (paths == null || paths.Length == 0)
            {
                throw new ArgumentNullException("paths");
            }
            StringBuilder sb = new StringBuilder();
            string item = string.Empty;
            for (int i = 0; i < paths.Length; i++)
            {
                item = paths[i];
                if (item == null)
                {
                    throw new ArgumentNullException("paths");
                }
                if (item.Length != 0)
                {
                    if (item.StartsWith("/"))
                        item = item.Substring(1);
                    if (item.IndexOf(".") != -1)
                    {
                        sb.Append(item);
                    }
                    else if (item.EndsWith("/"))
                        sb.Append(item);
                    else
                        sb.Append(item + "/");
                }
            }
            return sb.ToString();
        }
    }
}
