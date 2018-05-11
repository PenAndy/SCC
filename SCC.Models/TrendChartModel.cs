using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCC.Models
{
    /// <summary>
    /// 走势图相关模型
    /// </summary>
    class TrendChartModel
    {
    }
    /// <summary>
    /// 基础实体类
    /// </summary>
    public class BaseEntity
    {
        public int Id { get; set; }
    }
    /// <summary>
    /// 走势图数据基类
    /// </summary>
    public class TrendChartData : BaseEntity
    {
        /// <summary>
        /// 走势图ChartId
        /// </summary>
        public int ChartId { get; set; }
        /// <summary>
        /// 期数
        /// </summary>
        public int Term { get; set; }
        /// <summary>
        /// 历史最大遗漏
        /// </summary>
        public string[] AllMaxMiss { get; set; }
        /// <summary>
        /// 历史出现次数
        /// </summary>
        public string[] AllTimes { get; set; }
        /// <summary>
        /// 开奖记录
        /// </summary>
        public int RecordCount { get; set; }
        /// <summary>
        /// 历史平均遗漏
        /// </summary>
        public string[] AllAvgMiss { get; set; }
        /// <summary>
        /// 上期遗漏
        /// </summary>
        public string[] LastMiss { get; set; }
        /// <summary>
        /// 当前遗漏
        /// </summary>
        public string[] LocalMiss { get; set; }
        /// <summary>
        /// HTML代码
        /// </summary>
        public string HtmlData { get; set; }
        /// <summary>
        /// 走势图类型
        /// </summary>
        public TrendChartType ChartType { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime Addtime { get; set; }
    }


    /// <summary>
    /// 走势图每项具体配置信息
    /// </summary>
    public class TrendChartItemInfo : BaseEntity
    {
        /// <summary>
        /// 彩种
        /// </summary>
        public int Cid { get; set; }
        /// <summary>
        /// 走势图ID
        /// </summary>
        public int ChartId { get; set; }
        /// <summary>
        /// 走势图类型
        /// </summary>
        public TrendChartType ChartType { get; set; }
        /// <summary>
        /// 项类类型
        /// </summary>
        public ChartItemClassName ClassName { get; set; }
        /// <summary>
        /// 自定义项名称
        /// </summary>
        public string ChartItemName { get; set; }
        /// <summary>
        /// 列最小周期
        /// 即该项所有列周期内出现次数最小的列的出现次数
        /// </summary>
        public int Cycle { get; set; }
        /// <summary>
        /// 项最小值
        /// </summary>
        public int ItemMinValue { get; set; }
        /// <summary>
        /// 项最大值
        /// </summary>
        public int ItemMaxValue { get; set; }
        /// <summary>
        /// 中间值以区别大小
        /// 大于等于splitNumber算大数
        /// </summary>
        public int SplitNumberOfDX { get; set; }
        /// <summary>
        /// 项中列的个数
        /// </summary>
        public int ItemCount { get; set; }
        /// <summary>
        /// 项字符串数据
        /// </summary>
        public string[] ItemString { get; set; }
        /// <summary>
        /// 起始索引
        /// 计算项值时的起始索引号
        /// 特殊值-1表示IndexStart无效
        /// 注：单值项时仅IndexStart有效
        /// </summary>
        public int IndexStart { get; set; }
        /// <summary>
        /// 结束索引
        /// 计算项值时的结束索引号
        /// 特殊值-1表示IndexEnd无效
        /// </summary>
        public int IndexEnd { get; set; }
        /// <summary>
        /// 是否画连接线
        /// </summary>
        public bool DrawLine { get; set; }
        /// <summary>
        /// 项值函数类型(决定项值的计算方式)
        /// </summary>
        public ChartItemType FuntionType { get; set; }
        /// <summary>
        /// CSS配置ID
        /// </summary>
        public int ChartCssId { get; set; }
        /// <summary>
        /// 排序序号
        /// </summary>
        public int OrderBy { get; set; }
    }


    /// <summary>
    /// 走势图样式
    /// </summary>
    public class ChartCssConfigInfo : BaseEntity
    {
        /// <summary>
        /// 样式名称
        /// </summary>		
        public string Name { get; set; }

        /// <summary>
        /// 项对应的样式组
        /// </summary>		
        public int FuntionTypeCss { get; set; }

        /// <summary>
        /// 是否有子样式
        /// 0表示取子样式
        /// -1表示没有子样式（取自身）
        /// </summary>		
        public int ParentId { get; set; }

        /// <summary>
        /// 样式开始下标
        /// </summary>		
        public int startNum { get; set; }

        /// <summary>
        /// 样式结束下标
        /// </summary>		
        public int endNum { get; set; }

        /// <summary>
        /// 遗漏样式名称
        /// </summary>		
        public string MissCssName { get; set; }

        /// <summary>
        /// 选中样式名称
        /// </summary>		
        public string NumberCssName { get; set; }

        /// <summary>
        /// 画线样式名称
        /// </summary>		
        public string LineColor { get; set; }

        /// <summary>
        /// 数据分析样式
        /// </summary>		
        public string DataAnalysisCssName { get; set; }

        /// <summary>
        /// 数据分析(出现次数)图片
        /// </summary>		
        public string DataAnalysisImgName { get; set; }

        /// <summary>
        /// 扩展1
        /// </summary>		
        public string Extend1 { get; set; }

        /// <summary>
        /// 扩展2
        /// </summary>		
        public string Extend2 { get; set; }

        /// <summary>
        /// 扩展3
        /// </summary>		
        public string Extend3 { get; set; }

        /// <summary>
        /// 扩展4
        /// </summary>		
        public string Extend4 { get; set; }

        /// <summary>
        /// 扩展5
        /// </summary>		
        public string Extend5 { get; set; }

        /// <summary>
        /// 描述
        /// </summary>		
        public string Descript { get; set; }
        /// <summary>
        /// 子样式列表(ParentId=0有数据)
        /// </summary>
        public List<ChartCssConfigInfo> ChildList { get; set; }

    }

    /// <summary>
    /// 项值函数类型(决定项值的计算方式)
    /// </summary>
    public enum ChartItemType
    {
        /// <summary>
        /// 单列期数项
        /// </summary>
        Term_TermItem = 1,
        /// <summary>
        /// 012值(单值)
        /// </summary>
        SingleCell_012StatusItem = 2,
        /// <summary>
        /// 大小状态项(单值)
        /// </summary>
        SingleValue_DaXiaoStatusItem = 3,
        /// <summary>
        /// 和值尾数项(单值)
        /// </summary>
        SingleValue_HeWeiItem = 4,
        /// <summary>
        /// 奇偶状态项(单值)
        /// </summary>
        SingleValue_JiOuStatusItem = 5,
        /// <summary>
        /// 单个号码012形态项(单值)
        /// </summary>
        SingleValue_Number012StatusItem = 6,
        /// <summary>
        /// 单个号码数字项
        /// </summary>
        SingleValue_NumberItem = 7,
        /// <summary>
        /// 多于两个号码跨度项
        /// </summary>
        SingleValue_SpanItem = 8,
        /// <summary>
        /// 两个号码跨度项
        /// </summary>
        SingleValue_SpanNumberItem = 9,
        /// <summary>
        /// 和值项
        /// </summary>
        SingleValue_SumItem = 10,
        /// <summary>
        /// 质合状态项
        /// </summary>
        SingleValue_ZhiHeStatusItem = 11,
        /// <summary>
        /// 组三组六项
        /// </summary>
        SingleValue_ZuHeStatusItem = 12,
        /// <summary>
        /// 单列和尾项
        /// </summary>
        SingleCell_HeWeiItem = 13,
        /// <summary>
        /// 单列开奖号码展示项
        /// </summary>
        SingleCell_OpenCodeItem = 14,
        /// <summary>
        /// 单列012比例项
        /// </summary>
        SingleCell_ProportionOf012Item = 15,
        /// <summary>
        /// 单列大小比例项
        /// </summary>
        SingleCell_ProportionOfDxItem = 16,
        /// <summary>
        /// 单列奇偶比例项
        /// </summary>
        SingleCell_ProportionOfJoItem = 17,
        /// <summary>
        /// 单列质合比例项
        /// </summary>
        SingleCell_ProportionOfZhItem = 18,
        /// <summary>
        /// 单列跨度值项
        /// </summary>
        SingleCell_SpanItem = 19,
        /// <summary>
        /// 单列和值项
        /// </summary>
        SingleCell_SumItem = 20,
        /// <summary>
        /// 多值开奖号码展示项
        /// </summary>
        MultiValue_OpenCodeItem = 21,
        /// <summary>
        /// 单列试机号项
        /// </summary>
        SingleCell_ShiJiHao = 22,
        /// <summary>
        /// 和值奇偶状态
        /// </summary>
        SingleValue_HzJoStatusItem = 23,
        /// <summary>
        /// 和值大小状态
        /// </summary>
        SingleValue_HzDxStatusItem = 24,
        /// <summary>
        /// （多值）大小形态
        /// </summary>
        SingleValue_DxStatusItem = 25,
        /// <summary>
        /// (多值)奇偶形态
        /// </summary>
        SingleValue_JoStatusItem = 26,
        /// <summary>
        /// 单值试机号
        /// </summary>
        SingleValue_ShiJiHao = 27,
        /// <summary>
        /// 单列试机号和值项
        /// </summary>
        SingleCell_ShiJiHaoHzItem = 28,
        /// <summary>
        /// 单列试机号跨度项
        /// </summary>
        SingleCell_ShiJiHaoSpanItem = 29,
        /// <summary>
        /// 单列试机号奇偶比例
        /// </summary>
        SingleCell_ProportionOfShiJiHaoJoItem = 30,
        /// <summary>
        /// 单列试机号大小比例
        /// </summary>
        SingleCell_ProportionOfShiJiHaoDxItem = 31,
        /// <summary>
        /// 单列试机号类型项
        /// </summary>
        SingleValue_ShiJiHaoTypeItem = 32,
        /// <summary>
        /// 组三形态
        /// </summary>
        SingleValue_ZsStatusItem = 33,
        /// <summary>
        /// 单列组三遗漏项
        /// </summary>
        SingleCell_ZsMissItem = 34,
        /// <summary>
        /// 组三号码
        /// </summary>
        SingleCell_ZsHaoMaItem = 35,
        /// <summary>
        /// 单值AC值
        /// </summary>
        SingleCell_Ac = 36,
        /// <summary>
        /// 三区比(只适用于双色球)
        /// </summary>
        SingleCell_SanQu = 38,
        /// <summary>
        /// 单列ac值奇偶状态
        /// </summary>
        SingleCell_AcJiOu = 39,
        /// <summary>
        /// 单列ac值质合状态
        /// </summary>
        SingleCell_AcZhiHe = 40,
        /// <summary>
        /// 单列ac值012路
        /// </summary>
        SingleCell_Ac012Lu = 41,
        /// <summary>
        /// 单个号码的区间分布
        /// </summary>
        SingleValue_QuJianFenBu = 42,
        /// <summary>
        /// 和尾奇偶状态
        /// </summary>
        SingleValue_HeWeiJiOu = 43,
        /// <summary>
        /// 单列重号
        /// </summary>
        SingleCell_RepeatedNumber = 50,
        /// <summary>
        /// 单列连号
        /// </summary>
        SingleCell_LinkNumber = 51,
        /// <summary>
        /// 和值(区间)分布
        /// </summary>
        SingleValue_SumItemGroup = 52,
        /// <summary>
        /// 组三奇偶形态
        /// </summary>
        SingleValue_ZsJoStatusItem = 60,
        /// <summary>
        /// 组三大小形态
        /// </summary> 
        SingleValue_ZsDxStatusItem = 61,
        /// <summary>
        /// 组三012形态
        /// </summary>
        SingleValue_Zs012StatusItem = 62,
        /// <summary>
        /// 后区号码
        /// </summary>
        SingleCell_HqItem = 63,
        /// <summary>
        /// 多值多列连号分布
        /// </summary>
        MultiValue_LinkNumber = 65,
        /// <summary>
        /// 单列组三跨度值项
        /// </summary>
        SingleCell_ZSSpanItem = 66,
        /// <summary>
        /// 质合状态项
        /// </summary>
        SingleCell_ZhiHeStatusItem = 67,
        /// <summary>
        /// 多值多列重号分布
        /// </summary>
        MultiValue_RepeatNumber = 68,
        /// <summary>
        /// 多值多列折号分布
        /// </summary>
        MultiValue_ZheHaoNumber = 69,
        /// <summary>
        /// 多值多列斜连号分布
        /// </summary>
        MultiValue_XieLianHaoNumber = 70,
        /// <summary>
        /// 多值多列斜跳号分布
        /// </summary>
        MultiValue_XieTiaoHaoNumber = 71,
        /// <summary>
        /// 多值多列竖三连分布
        /// </summary>
        MultiValue_ShuSanLianHaoNumber = 72,
        /// <summary>
        /// 多值多列竖跳号分布
        /// </summary>
        MultiValue_ShuTiaoHaoNumber = 73,

        /// <summary>
        /// 福彩3D 012路走势图4
        /// </summary>
        SpecialValue_FC3D012_4 = 74,
        /// <summary>
        /// 福彩 双色球出号频率
        /// </summary>
        SpecialValue_FCSSQ_ChuHaoPL = 75,
        /// <summary>
        /// 体彩PD 012路走势图4
        /// </summary>
        SpecialValue_TCP3012_4 = 76,
        /// <summary>
        /// 体彩 大乐透出号频率
        /// </summary>
        SpecialValue_TCDLT_ChuHaoPL = 77,

        /// <summary>
        /// 多值多列快乐12号码分布
        /// </summary>
        MultiValue_KL12 = 78,
        /// <summary>
        /// 生肖分布
        /// </summary>
        SingleValue_SX = 79,
        /// <summary>
        /// 季节分布
        /// </summary>
        SingleValue_JJ = 80,
        /// <summary>
        /// 方位分布
        /// </summary>
        SingleValue_FW = 81,
        /// <summary>
        /// 回摆
        /// </summary>
        SingleValue_HB = 82,
        /// <summary>
        /// 振幅
        /// </summary>
        SingleCell_ZF = 83,
        /// <summary>
        /// 福建31选7三区比
        /// </summary>
        SingleCell_FJ31X7SanQu = 84,
        /// <summary>
        /// 福建36选7三区比
        /// </summary>
        SingleCell_FJ36X7SanQu = 85,
        /// <summary>
        /// 和尾大小形态
        /// </summary>
        SingleValue_HeWeiDx = 86,
        /// <summary>
        /// 生肖
        /// </summary>
        SingleValue_ShengXiao = 87,
        /// <summary>
        /// 华东15选5三区比
        /// </summary>
        SingleCell_Hd15x5SanQU = 88,
        /// <summary>
        /// 华东1区个数
        /// </summary>
        SingleValue_Hd11x5Yq = 89,
        /// <summary>
        /// 华东2区个数
        /// </summary>
        SingleValue_Hd11x5Eq = 90,
        /// <summary>
        /// 华东3区个数
        /// </summary>
        SingleValue_Hd11x5Sq = 91,
        /// <summary>
        /// 南粤三区比
        /// </summary>
        SingleCell_NY36x7Sanqu = 92,
        /// <summary>
        /// 和值012比
        /// </summary>
        SingleCell_Hz012 = 93,
        /// <summary>
        /// 快3三连号走势
        /// </summary>
        SingleValue_K3sbt = 94,
        /// <summary>
        /// 快3二不同单选走势
        /// </summary>
        MultiValue_K3ebt = 95,
        /// <summary>
        /// 快3二同号（单值）
        /// </summary>
        SingleCell_K3ebt = 96,
        /// <summary>
        /// 奇偶个数
        /// </summary>
        SingleValue_JoValue = 97,
        /// <summary>
        /// 大小个数
        /// </summary>
        SingleValue_DxValue = 98,
        /// <summary>
        /// 质合个数
        /// </summary>
        SingleValue_ZhValue = 99,
        /// <summary>
        /// 三不同形态
        /// </summary>
        MultiValue_Sbtxt = 100,
        /// <summary>
        /// 二不同形态
        /// </summary>
        MultiValue_Ebtxt = 101,
        /// <summary>
        /// 大小奇偶
        /// </summary>
        SingleValue_DxjoValue = 102,

        /// <summary>
        /// 小数个数
        /// </summary>
        SingleValue_XsValue = 103,
        /// <summary>
        /// 合数个数
        /// </summary>
        SingleValue_HsValue = 104,
        /// <summary>
        /// 偶数个数
        /// </summary>
        SingleValue_OsValue = 105

    }

    /// <summary>
    /// 项处理类类型(处理项的类类型) 
    /// </summary>
    public enum ChartItemClassName
    {
        /// <summary>
        /// 单值项类型
        /// </summary>
        SingleValue = 1,
        /// <summary>
        /// 多值项类型
        /// </summary>
        MultiValue = 2,
        /// <summary>
        /// 特殊项
        /// </summary>
        SpecialValue = 3
    }

    /// <summary>
    /// 遗漏数据类型
    /// </summary>
    public enum MissDataType
    {
        /// <summary>
        /// 本期遗漏
        /// </summary>
        LocalMiss = 1,
        /// <summary>
        /// 上期遗漏
        /// </summary>
        LastMiss = 2,
        /// <summary>
        /// 最大遗漏
        /// </summary>
        AllMaxMiss = 3,
        /// <summary>
        /// 平均遗漏
        /// </summary>
        AllAvgMiss = 4,
        /// <summary>
        /// 出现次数
        /// </summary>
        AllTimes = 5
    }

    /// <summary>
    /// 走势图类型
    /// </summary>
    public enum TrendChartType
    {
        /// <summary>
        /// 电脑走势图
        /// </summary>
        PC = 1,
        /// <summary>
        /// 手机走势图
        /// </summary>
        WAP = 2,
        /// <summary>
        /// 电视走势图
        /// </summary>
        TV = 3
    }


    #region 地方彩种实体类
    /// <summary>
    /// 彩票开奖彩种基类
    /// </summary>
    public class LotteryOpenCode : BaseEntity
    {
        /// <summary>
        /// 期数
        /// </summary>
        public int Term { get; set; }
        /// <summary>
        /// 开奖号码
        /// </summary>
        public IList<int> OpenCode { get; set; }
        /// <summary>
        /// 开奖时间
        /// </summary>
        public DateTime OpenTime { get; set; }
        /// <summary>
        /// 试机号
        /// </summary>
        public string ShiJiHao { get; set; }
        /// <summary>
        /// 开机号
        /// </summary>
        public string KaiJiHao { get; set; }
        /// <summary>
        /// 开奖详细
        /// </summary>
        public string Detail { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime Addtime { get; set; }
    }
    /// <summary>
    /// 东方6+1
    /// </summary>
    public class FCDF6J1Info : LotteryOpenCode
    {
        public int OpenCode1 { get; set; }

        public int OpenCode2 { get; set; }

        public int OpenCode3 { get; set; }

        public int OpenCode4 { get; set; }

        public int OpenCode5 { get; set; }

        public int OpenCode6 { get; set; }

        public int OpenCode7 { get; set; }
    }
    /// <summary>
    /// 华东15选5
    /// </summary>
    public class FCHD15X5Info : LotteryOpenCode
    {
        public int OpenCode1 { get; set; }

        public int OpenCode2 { get; set; }

        public int OpenCode3 { get; set; }

        public int OpenCode4 { get; set; }

        public int OpenCode5 { get; set; }
    }
    /// <summary>
    /// 河南22选5
    /// </summary>
    public class FCHN22X5Info : LotteryOpenCode
    {
        public int OpenCode1 { get; set; }

        public int OpenCode2 { get; set; }

        public int OpenCode3 { get; set; }

        public int OpenCode4 { get; set; }

        public int OpenCode5 { get; set; }
    }
    /// <summary>
    /// 广东(南粤)36选7
    /// </summary>
    public class FCNY36X7Info : LotteryOpenCode
    {
        public int OpenCode1 { get; set; }

        public int OpenCode2 { get; set; }

        public int OpenCode3 { get; set; }

        public int OpenCode4 { get; set; }

        public int OpenCode5 { get; set; }

        public int OpenCode6 { get; set; }

        public int OpenCode7 { get; set; }
    }
    /// <summary>
    /// 湖北30选5
    /// </summary>
    public class FCHB30X5Info : LotteryOpenCode
    {
        public int OpenCode1 { get; set; }

        public int OpenCode2 { get; set; }

        public int OpenCode3 { get; set; }

        public int OpenCode4 { get; set; }

        public int OpenCode5 { get; set; }
    }
    /// <summary>
    /// 新疆35选7
    /// </summary>
    public class FCXJ35X7Info : LotteryOpenCode
    {
        public int OpenCode1 { get; set; }

        public int OpenCode2 { get; set; }

        public int OpenCode3 { get; set; }

        public int OpenCode4 { get; set; }

        public int OpenCode5 { get; set; }

        public int OpenCode6 { get; set; }

        public int OpenCode7 { get; set; }

        public int OpenCode8 { get; set; }
    }
    /// <summary>
    /// 江苏体彩七位数
    /// </summary>
    public class TCJS7WSInfo : LotteryOpenCode
    {
        public int OpenCode1 { get; set; }

        public int OpenCode2 { get; set; }

        public int OpenCode3 { get; set; }

        public int OpenCode4 { get; set; }

        public int OpenCode5 { get; set; }

        public int OpenCode6 { get; set; }

        public int OpenCode7 { get; set; }
    }
    /// <summary>
    /// 浙江体彩6+1
    /// </summary>
    public class TCZJ6J1Info : LotteryOpenCode
    {
        public int OpenCode1 { get; set; }

        public int OpenCode2 { get; set; }

        public int OpenCode3 { get; set; }

        public int OpenCode4 { get; set; }

        public int OpenCode5 { get; set; }

        public int OpenCode6 { get; set; }

        public int OpenCode7 { get; set; }
    }
    /// <summary>
    /// 福建36选7
    /// </summary>
    public class TCFJ36X7Info : LotteryOpenCode
    {
        public int OpenCode1 { get; set; }

        public int OpenCode2 { get; set; }

        public int OpenCode3 { get; set; }

        public int OpenCode4 { get; set; }

        public int OpenCode5 { get; set; }

        public int OpenCode6 { get; set; }

        public int OpenCode7 { get; set; }

        public int OpenCode8 { get; set; }
    }
    /// <summary>
    /// 福建31选7
    /// </summary>
    public class TCFJ31X7Info : LotteryOpenCode
    {
        public int OpenCode1 { get; set; }

        public int OpenCode2 { get; set; }

        public int OpenCode3 { get; set; }

        public int OpenCode4 { get; set; }

        public int OpenCode5 { get; set; }

        public int OpenCode6 { get; set; }

        public int OpenCode7 { get; set; }

        public int OpenCode8 { get; set; }
    }
    /// <summary>
    /// 福彩广东好彩1
    /// </summary>
    public class FCGDHC1Info : LotteryOpenCode
    {
        public int OpenCode1 { get; set; }
    }

    #endregion

    /// <summary>
    /// 走势图配置模型
    /// </summary>
    public class DT_TrendChart : BaseEntity
    {
        public string Name { get; set; }
        public int Cid { get; set; }
        public int Tid { get; set; }
        public int Status { get; set; }
        public int OrderBy { get; set; }
        public string Url { get; set; }
        public int type { get; set; }
        public string hTitle { get; set; }
        public string hKeywords { get; set; }
        public string hDescription { get; set; }
        /// <summary>
        /// 屏幕方向(横屏或竖屏)
        /// 默认都为0横屏
        /// </summary>
        public int Direction { get; set; }
        public int TemplateId { get; set; }
    }
    /// <summary>
    /// 走势图配置显示项模型
    /// </summary>
    public class DT_TrendChartItem : BaseEntity
    {
        public int ChartId { get; set; }
        public int ChartType { get; set; }
        public int ClassName { get; set; }
        public string ChartItemName { get; set; }
        public int Cycle { get; set; }
        public int? ItemMinValue { get; set; }
        public int? ItemMaxValue { get; set; }
        public int? SplitNumberOfDX { get; set; }
        public int ItemCount { get; set; }
        public string ItemString { get; set; }
        public int? IndexStart { get; set; }
        public int? IndexEnd { get; set; }
        public bool DrawLine { get; set; }
        public int FuntionType { get; set; }
        public int ChartCssId { get; set; }
        public int? OrderBy { get; set; }
    }
}
