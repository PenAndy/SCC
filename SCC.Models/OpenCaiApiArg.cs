namespace SCC.Models
{
    /// <summary>
    /// 开彩网接口参数
    /// </summary>
    public class OpenCaiApiArg
    {
        /// <summary>
        /// 用户账号。付费用户必填，唯一标识。例：token=a1b2c3d4f5
        /// </summary>
        public string token => "t8cbbf71a97e104e2k";

        /// <summary>
        /// 彩票代码。部份接口格式支持逗号分割的多种彩票。例：code=cqssc,cqklsf
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 返回行数。仅按最新查询时有效，默认5行。例：rows=3
        /// </summary>
        public int rows { get; set; }

        /// <summary>
        /// 数据格式。支持xml与json格式，默认json。例：format=json
        /// </summary>
        public string format => "json";

        /// <summary>
        /// 日期参数。仅按日期查询(daily.do)时有效，格式yyyyMMdd或yyyy-MM-dd，低频按年返回。例：date=2015-02-18
        /// </summary>
        public string date { get; set; }

        /// <summary>
        /// Jsonp回调函数。可选，为jsonp提供支持。例：callback=reader
        /// </summary>
        public string callback { get; set; }
    }
}