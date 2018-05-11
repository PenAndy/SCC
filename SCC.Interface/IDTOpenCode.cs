using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SCC.Models;

namespace SCC.Interface
{
    /// <summary>
    /// 地方彩相关数据服务接口
    /// </summary>
    public interface IDTOpenCode
    {
        /// <summary>
        /// 获取最近指定n条记录的期号
        /// </summary>
        /// <param name="lottery"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        Dictionary<int, string> GetLast1NTerm(SCCLottery lottery, int n);
        /// <summary>
        /// 根据期数更新当前彩种的开奖详情
        /// </summary>
        /// <param name="lottery"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        bool UpdateKJDetailByTerm(SCCLottery lottery, int term, string source);
        bool UpdateSSQDetailByTerm(SCCLottery lottery, int term, string source);
        /// <summary>
        /// 获取最新一条记录
        /// 开奖1个球号的地方彩
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <returns></returns>
        OpenCode1DTModel GetOpenCode1DTLastItem(SCCLottery lottery);
        /// <summary>
        /// 获取最新一条记录
        /// 开奖5个球号的地方彩
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <returns></returns>
        OpenCode5DTModel GetOpenCode5DTLastItem(SCCLottery lottery);
        OpenCode3DTModel GetOpenCode3DTLastItem(SCCLottery lottery);
        OpenCodeFC3DTModel GetOpenCodeFC3DTLastItem(SCCLottery lottery);
        OpenCodePL5TModel GetOpenCodePL5TLastItem(SCCLottery lottery);
  
        OpenCodePL3TModel GetOpenCodePL3TLastItem(SCCLottery lottery);
        bool UpdateKJDetail3DByTerm(SCCLottery currentLottery, int key, string res, OpenCodeFC3DTModel matchItem);
        bool LotterySkillModel(SCCLottery currentLottery, LotterySkillModel matchItem);
        bool LotteryGlossaryModel(SCCLottery currentLottery, LotteryGlossaryModel matchItem);

        bool LotteryNewsModel(SCCLottery currentLottery, LotteryNewsModel matchItem);
        
        bool UpdateKJDetailP5ByTerm(SCCLottery currentLottery, int key, string res, OpenCodePL5TModel matchItem);
        bool UpdateKJDetailP3ByTerm(SCCLottery currentLottery, int key, string res, OpenCodePL3TModel matchItem);
        OpenCode2DTModel GetOpenCode2DTLastItem(SCCLottery lottery);
        OpenCode4DTModel GetOpenCode4DTLastItem(SCCLottery lottery);
        /// <summary>
        /// 获取最新一条记录
        /// 开奖7个球号的地方彩
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <returns></returns>
        OpenCode7DTModel GetOpenCode7DTLastItem(SCCLottery lottery);
        /// <summary>
        /// 获取最新一条记录
        /// 开奖8个球号的地方彩
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <returns></returns>
        OpenCode8DTModel GetOpenCode8DTLastItem(SCCLottery lottery);

        /// <summary>
        /// 获取今年的失败期号列表
        /// 第1期与数据库最新一期之间的失败期号列表
        /// 格式YYQQQ
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <returns></returns>
        List<string> GetFailedYYQQQList(SCCLottery lottery);

        /// <summary>
        /// 获取今年的失败期号列表
        /// 第1期与数据库最新一期之间的失败期号列表
        /// 格式YYQQQ
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <returns></returns>
        List<string> GetFailedYYYYQQQList(SCCLottery lottery);
        /// <summary>
        /// 新增彩种开奖数据
        /// 开奖1个球号的地方彩
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <param name="model">开奖数据模型</param>
        /// <returns></returns>
        bool AddDTOpen1Code(SCCLottery lottery, OpenCode1DTModel model);
        /// <summary>
        /// 新增彩种开奖数据
        /// 开奖5个球号的地方彩
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <param name="model">开奖数据模型</param>
        /// <returns></returns>
        bool AddDTOpen5Code(SCCLottery lottery, OpenCode5DTModel model);
        bool AddDTOpen3Code(SCCLottery lottery, OpenCode3DTModel model);

        bool AddDTOpenFC3DCode(SCCLottery lottery, OpenCodeFC3DTModel model);
        bool AddDTOpenPL5Code(SCCLottery lottery, OpenCodePL5TModel model);
        bool AddDTOpenPL3Code(SCCLottery lottery, OpenCodePL3TModel model);
        bool AddDTOpen2Code(SCCLottery lottery, OpenCode2DTModel model);
        /// <summary>
        /// 新增彩种开奖数据
        /// 开奖7个球号的地方彩
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <param name="model">开奖数据模型</param>
        /// <returns></returns>
        bool AddDTOpen7Code(SCCLottery lottery, OpenCode7DTModel model);
        bool AddDTOpen4Code(SCCLottery lottery, OpenCode4DTModel model);
        /// <summary>
        /// 新增彩种开奖数据
        /// 开奖8个球号的地方彩
        /// </summary>
        /// <param name="lottery">彩种名称</param>
        /// <param name="model">开奖数据模型</param>
        /// <returns></returns>
        bool AddDTOpen8Code(SCCLottery lottery, OpenCode8DTModel model);
        /// <summary>
        /// 获取江苏七位数所有开奖记录
        /// 生成走势图所需数据
        /// </summary>
        /// <returns></returns>
        List<TCJS7WSInfo> GetJS7WSListOpenCode();
        /// <summary>
        /// 获取浙江体彩6+1所有开奖记录
        /// 生成走势图所需数据
        /// </summary>
        /// <returns></returns>
        List<TCZJ6J1Info> GetZJ6J1ListOpenCode();
        /// <summary>
        /// 获取新疆35选7所有开奖记录
        /// 生成走势图所需数据
        /// </summary>
        /// <returns></returns>
        List<FCXJ35X7Info> GetXJ35X7ListOpenCode();
        /// <summary>
        /// 获取东方6+1所有开奖记录
        /// 生成走势图所需数据
        /// </summary>
        /// <returns></returns>
        List<FCDF6J1Info> GetDF6J1ListOpenCode();
        /// <summary>
        /// 获取东方6+1最新指定条数所有开奖记录
        /// </summary>
        /// <param name="period">指定条数</param>
        /// <returns></returns>
        List<FCDF6J1Info> GetDF6J1ListOpenCode(int period);
        /// <summary>
        /// 获取指定期数的开奖详情
        /// </summary>
        /// <param name="Term">指定期数</param>
        /// <returns></returns>
        OpenCode7DTModel GetDF6J1Detail(int Term);
        /// <summary>
        /// 获取华东15选5所有开奖记录
        /// 生成走势图所需数据
        /// </summary>
        /// <returns></returns>
        List<FCHD15X5Info> GetHD15X5ListOpenCode();
        /// <summary>
        /// 获取华东15选5最新指定条数所有开奖记录
        /// </summary>
        /// <param name="period">指定条数</param>
        /// <returns></returns>
        List<FCHD15X5Info> GetHD15X5ListOpenCode(int period);
        /// <summary>
        /// 获取指定期数的开奖详情
        /// </summary>
        /// <param name="Term">指定期数</param>
        /// <returns></returns>
        OpenCode5DTModel GetHD15X5Detail(int Term);
        /// <summary>
        /// 获取河南22选5所有开奖记录
        /// 生成走势图所需数据
        /// </summary>
        /// <returns></returns>
        List<FCHN22X5Info> GetHN22X5ListOpenCode();
        /// <summary>
        /// 获取广东36选7所有开奖记录
        /// 生成走势图所需数据
        /// </summary>
        /// <returns></returns>
        List<FCNY36X7Info> GetGD36X7ListOpenCode();
        /// <summary>
        /// 获取湖北30选5所有开奖记录
        /// 生成走势图所需数据
        /// </summary>
        /// <returns></returns>
        List<FCHB30X5Info> GetHuBei30X5ListOpenCode();
        /// <summary>
        /// 获取福建36选7所有开奖记录
        /// 生成走势图所需数据
        /// </summary>
        /// <returns></returns>
        List<TCFJ36X7Info> GetFJ36X7ListOpenCode();
        /// <summary>
        /// 获取福建31选7所有开奖记录
        /// 生成走势图所需数据
        /// </summary>
        /// <returns></returns>
        List<TCFJ31X7Info> GetFJ31X7ListOpenCode();
        /// <summary>
        /// 获取广东好彩1所有开奖记录
        /// 生成走势图所需数据
        /// </summary>
        /// <returns></returns>
        List<FCGDHC1Info> GetGDHC1ListOpenCode();

        /// <summary>
        /// 校验使用查询数据库数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lottery"></param>
        /// <param name="IsToday"></param>
        /// <returns></returns>
        List<T> GetListS<T>(SCCLottery lottery);
    }
}
