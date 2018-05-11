using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SCC.Models;

namespace SCC.Interface
{
    /// <summary>
    /// 开奖7个球号的彩种数据接口
    /// </summary>
    public interface IOpen7Code
    {
        /// <summary>
        /// 获取最新一条记录
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <returns></returns>
        OpenCode7Model GetLastItem(SCCLottery lottery);

        /// <summary>
        /// 根据日期获取开奖信息
        /// </summary>
        /// <param name="lottery"></param>
        /// <param name="IsToday"> 是否今天 否：昨天</param>
        /// <returns></returns>
        List<OpenCode7Model> GetListIn(SCCLottery lottery, bool IsToday);

        /// <summary>
        /// 获取前一天失败列表
        /// 期号格式形如YYMMDDQQ
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <param name="TotalQNum">当前彩种每天总期数</param>
        /// <returns></returns>
        List<string> GetYesterdayFailQQList(SCCLottery lottery, int TotalQNum);

        /// <summary>
        /// 新增彩种开奖数据
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <param name="model">开奖数据模型</param>
        /// <returns></returns>
        bool AddOpen7Code(SCCLottery lottery, OpenCode7Model model);

        #region 地方彩
        #endregion
    }
}
