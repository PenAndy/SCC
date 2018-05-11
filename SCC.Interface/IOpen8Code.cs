using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SCC.Models;

namespace SCC.Interface
{
    /// <summary>
    /// 开奖8个球号的彩种相关数据接口
    /// </summary>
    public interface IOpen8Code
    {
        /// <summary>
        /// 获取最新一条记录
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <returns></returns>
        OpenCode8Model GetLastItem(SCCLottery lottery);
        /// <summary>
        /// 根据日期获取开奖信息
        /// </summary>
        /// <param name="lottery"></param>
        /// <param name="IsToday"> 是否今天 否：昨天</param>
        /// <returns></returns>
        List<OpenCode8Model> GetListIn(SCCLottery lottery, bool IsToday);

        /// <summary>
        /// 获取前一天失败列表
        /// 期号格式形如YYMMDDQQ
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <param name="TotalQNum">当前彩种每天总期数</param>
        /// <returns></returns>
        List<string> GetYesterdayFailQQList(SCCLottery lottery, int TotalQNum);

        /// <summary>
        /// 获取前一天失败列表
        /// 期号格式形如YYMMDDQQQ
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <param name="TotalQNum">当前彩种每天总期数</param>
        /// <returns></returns>
        List<string> GetYesterdayFailQQQList(SCCLottery lottery, int TotalQNum);

        /// <summary>
        /// 新增彩种开奖数据
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <param name="model">开奖数据模型</param>
        /// <returns></returns>
        bool AddOpen8Code(SCCLottery lottery, OpenCode8Model model);
    }
}
