using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SCC.Models
{
    /// <summary>
    /// 项目相关枚举类
    /// </summary>
    class SCCEnum
    {
    }

    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// 操作日志
        /// </summary>
        OperationLog = 1,
        /// <summary>
        /// 错误日志
        /// </summary>
        ErrorLog = 2
    }

    /// <summary>
    /// 彩票定义
    /// </summary>
    public enum LotteryDefine
    {
        /// <summary>
        /// 全国彩种
        /// </summary>
        National = 1,
        /// <summary>
        /// 地方彩种
        /// </summary>
        Locality = 2,
        /// <summary>
        /// 高频彩
        /// </summary>
        HighRate = 3
    }

    /// <summary>
    /// 彩种枚举
    /// </summary>
    public enum SCCLottery
    {
        #region 全国彩
        /// <summary>
        /// 福彩3D
        /// </summary>
        [TableName("QG_FC3D")]
        [Description("福彩3D")]
        FC3D,
        /// <summary>
        /// 福彩3D走势图
        /// </summary>
        [TableName("QG_FC3DTrendChartData")]
        FC3DTrendChart,

     




        /// <summary>
        /// 双色球
        /// </summary>
        [TableName("QG_FCSSQ")]
        [Description("双色球")]
        SSQ,
        /// <summary>
        /// 双色球走势图
        /// </summary>
        [TableName("QG_FCSSQTrendChartData")]
        SSQTrendChart,

        /// <summary>
        /// 七乐彩
        /// </summary>
        [TableName("QG_FCQLC")]
        [Description("七乐彩")]
        QLC,
        /// <summary>
        /// 七乐彩走势图
        /// </summary>
        [TableName("QG_FCQLCTrendChartData")]
        QLCTrendChart,

        /// <summary>
        /// 排列三、排列五
        /// </summary>
        [TableName("QG_TCP3")]
        [Description("排列三")]
        PL3,
        /// <summary>
        /// 排列三、排列五走势图
        /// </summary>
        [TableName("QG_TCP3TrendChartData")]
        PL3TrendChart,

        /// <summary>
        /// 排列五
        /// </summary>
        [TableName("QG_TCP5")]
        [Description("排列五")]
        PL5,
        /// <summary>
        /// 排列三、排列五走势图
        /// </summary>
        [TableName("QG_TCP5TrendChartData")]
        PL5TrendChart,
        /// <summary>
        /// 大乐透
        /// </summary>
        [TableName("QG_TCDLT")]
        [Description("大乐透")]
        DLT,
        /// <summary>
        /// 大乐透走势图
        /// </summary>
        [TableName("QG_TCDLTTrendChartData")]
        DLTTrendChart,

        /// <summary>
        /// 七星彩
        /// </summary>
        [TableName("QG_TCQXC")]
        [Description("七星彩")]
        QXC,
        /// <summary>
        /// 七星彩走势图
        /// </summary>
        [TableName("QG_TCQXCTrendChartData")]
        QXCTrendChart,
        #endregion

        #region 地方彩

        #region 华东六省

        /// <summary>
        /// 东方6+1
        /// </summary>
        [TableName("DF_DF6J1")]
        [LotteryCode("df6j1")]
        [Description("东方6+1")]
        DF6J1,
        /// <summary>
        /// 东方6+1走势图
        /// </summary>
        [TableName("DT_FCDF6J1TrendChartData")]
        DF6J1TrendChart,
        /// <summary>
        /// 华东15选5
        /// </summary>
        [TableName("DF_HD15x5")]
        [LotteryCode("hd15x5")]
        [Description("华东15选5")]
        HD15X5,
        /// <summary>
        /// 华东15选5走势图
        /// </summary>
        [TableName("DT_FCHD11X5TrendChartData")]
        HD15X5TrendChart,

        #endregion

        #region 北京



        #endregion

        #region 上海

        /// <summary>
        /// 上海天天彩选4
        /// </summary>
        [TableName("DF_TTCx4_ShangHai")]
        [LotteryCode("shttcx4")]
        [Description("上海天天彩选4")]
        ShangHaiTTCX4,

        #endregion

        #region 天津



        #endregion

        #region 江苏

        /// <summary>
        /// 江苏7位数
        /// </summary>
        [TableName("DF_TC7WS_JiangSu")]
        [LotteryCode("jstc7ws")]
        [Description("江苏体彩七位数")]
        JiangSuTC7WS,
        /// <summary>
        /// 江苏体彩七位数走势图
        /// </summary>
        [TableName("DT_TCJS7WSTrendChartData")]
        JSTC7WSTrendChart,


        #endregion

        #region 浙江

        /// <summary>
        /// 浙江20选5
        /// </summary>
        [TableName("DF_20x5_ZheJiang")]
        [LotteryCode("")]
        [Description("浙江20选5")]
        ZheJiang20x5,
        /// <summary>
        /// 浙江6+1
        /// </summary>
        [TableName("DF_6J1_ZheJiang")]
        [LotteryCode("")]
        [Description("浙江体彩6+1")]
        ZheJiang6J1,
        /// <summary>
        /// 浙江体彩6+1走势图
        /// </summary>
        [TableName("DT_TCZJ6J1TrendChartData")]
        ZJTC6J1TrendChart,


        #endregion

        #region 福建

        /// <summary>
        /// 福建体彩22选5
        /// </summary>
        [TableName("DF_TC22x5_FuJian")]
        [LotteryCode("fjtc22x5")]
        [Description("福建体彩22选5")]
        FuJian22x5,
        /// <summary>
        /// 福建31选7
        /// </summary>
        [TableName("DF_31x7_FuJian")]
        [LotteryCode("")]
        [Description("福建31选7")]
        FuJian31x7,
        /// <summary>
        /// 福建31选7走势图
        /// </summary>
        [TableName("DT_TCFJ31X7TrendChartData")]
        FJ31X7TrendChart,
        /// <summary>
        /// 福建体彩36选7
        /// </summary>
        [TableName("DF_TC36x7_FuJian")]
        [LotteryCode("fjtc36x7")]
        [Description("福建36选7")]
        FuJianTC36x7,
        /// <summary>
        /// 福建36选7走势图
        /// </summary>
        [TableName("DT_TCFJ36X7TrendChartData")]
        FJ36X7TrendChart,

        #endregion

        #region 广东

        /// <summary>
        /// 广东南粤风采26选5
        /// </summary>
        [TableName("DF_26x5_GuangDong")]
        [LotteryCode("gdfc26x5")]
        [Description("广东26选5")]
        GuangDong26X5,
        /// <summary>
        /// 广东36选7
        /// </summary>
        [TableName("DF_36x7_GuangDong")]
        [LotteryCode("gdfc36x5")]
        [Description("广东36选7")]
        GuangDong36x7,
        /// <summary>
        /// 广东36选7走势图
        /// </summary>
        [TableName("DT_FCNY36X7TrendChartData")]
        GD36X7TrendChart,
        /// <summary>
        /// 广东好彩1
        /// </summary>
        [TableName("DF_HC1_GuangDong")]
        [LotteryCode("gdfchc1")]
        [Description("好彩一")]
        GuangDongHC1,
        /// <summary>
        /// 广东好彩1走势图
        /// </summary>
        [TableName("DT_HC1TrendChartData")]
        GDHC1TrendChart,
        /// <summary>
        /// 广东深圳风采
        /// </summary>
        [TableName("DF_SZFC_GuangDong")]
        [LotteryCode("gdszfc")]
        [Description("广东深圳风采")]
        GuangDongSZFC,

        #endregion

        #region 深圳



        #endregion

        #region 黑龙江

        /// <summary>
        /// 黑龙江龙江风彩22选5
        /// </summary>
        [TableName("DF_LJFC22x5_HeiLongJiang")]
        [LotteryCode("hljfc22x5")]
        [Description("黑龙江龙江风彩22选5")]
        HeiLongJiangLJFC22x5,
        /// <summary>
        /// 黑龙江体彩6+1
        /// </summary>
        [TableName("DF_TC6J1_HeiLongJiang")]
        [LotteryCode("hljtc6j1")]
        [Description("黑龙江体彩6+1")]
        HeiLongJiangTC6J1,
        /// <summary>
        /// 黑龙江36选7
        /// </summary>
        [TableName("DF_36x7_HeiLongJiang")]
        [LotteryCode("")]
        [Description("黑龙江36选7")]
        HeiLongJiang36x7,
        /// <summary>
        /// 黑龙江P62
        /// </summary>
        [TableName("DF_P62_HeiLongJiang")]
        [LotteryCode("")]
        [Description("黑龙江P62")]
        HeiLongJiangP62,

        #endregion

        #region 四川



        #endregion

        #region 辽宁

        /// <summary>
        /// 辽宁风采35选7
        /// </summary>
        [TableName("DF_35x7_LiaoNing")]
        [LotteryCode("lnfc35x7")]
        [Description("辽宁风采35选7")]
        LiaoNingFC35X7,

        #endregion

        #region 河北

        /// <summary>
        /// 河北燕赵风采20选5
        /// </summary>
        [TableName("DF_20x5_HeBei")]
        [LotteryCode("yzfc20x5")]
        [Description("河北燕赵风采20选5")]
        HeBei20X5,
        /// <summary>
        /// 河北燕赵风采好运彩2
        /// </summary>
        [TableName("DF_HYC2_HeBei")]
        [LotteryCode("yzfchyc2")]
        [Description("河北燕赵风采好运彩2")]
        HeBeiHYC2,
        /// <summary>
        /// 河北燕赵风采好运彩3
        /// </summary>
        [TableName("DF_HYC3_HeBei")]
        [LotteryCode("yzfchyc3")]
        [Description("河北燕赵风采好运彩3")]
        HeBeiHYC3,
        /// <summary>
        /// 河北燕赵风采排列5
        /// </summary>
        [TableName("DF_PL5_HeBei")]
        [LotteryCode("yzfcpl5")]
        [Description("河北燕赵风采排列5")]
        HeBeiPL5,
        /// <summary>
        /// 河北燕赵风采排列7
        /// </summary>
        [TableName("DF_PL7_HeBei")]
        [LotteryCode("yzfcpl7")]
        [Description("河北燕赵风采排列7")]
        HeBeiPL7,

        #endregion

        #region 河南

        /// <summary>
        /// 河南中原风采22选5
        /// </summary>
        [TableName("DF_22x5_HeNan")]
        [LotteryCode("zyfc22x5")]
        [Description("河南22选5")]
        HeNan22X5,
        /// <summary>
        /// 河南22选5走势图
        /// </summary>
        [TableName("DT_FCHN22X5TrendChartData")]
        HeNan22X5TrendChart,

        #endregion

        #region 湖北

        /// <summary>
        /// 湖北30选5
        /// </summary>
        [TableName("DF_30x5_HuBei")]
        [LotteryCode("")]
        [Description("湖北30选5")]
        HuBei30X5,
        /// <summary>
        /// 湖北30选5走势图
        /// </summary>
        [TableName("DT_FCHB30X5TrendChartData")]
        HuBei30X5TrendChart,

        #endregion

        #region 山西



        #endregion

        #region 云南



        #endregion

        #region 新疆

        /// <summary>
        /// 新疆新疆风采18选7
        /// </summary>
        [TableName("DF_18x7_XinJiang")]
        [LotteryCode("xjfc18x7")]
        [Description("新疆新疆风采18选7")]
        XinJiangFC18X7,
        /// <summary>
        /// 新疆新疆风采25选7
        /// </summary>
        [TableName("DF_25x7_XinJiang")]
        [LotteryCode("xjfc25x7")]
        [Description("新疆新疆风采25选7")]
        XinJiangFC25X7,
        /// <summary>
        /// 新疆新疆风采35选7
        /// </summary>
        [TableName("DF_35x7_XinJiang")]
        [LotteryCode("xjfc35x7")]
        [Description("新疆35选7")]
        XinJiangFC35X7,
        /// <summary>
        /// 新疆35选7走势图
        /// </summary>
        [TableName("DT_FCXJ35X7TrendChartData")]
        XJ35X7TrendChart,

        #endregion

        #region 广西

        /// <summary>
        /// 广西快乐双彩
        /// </summary>
        [TableName("DF_KLSC_GuangXi")]
        [LotteryCode("gxklsc")]
        [Description("广西快乐双彩")]
        GuangXiKLSC,

        #endregion

        #region 江西



        #endregion

        #region 安徽

        /// <summary>
        /// 安徽福彩25选5
        /// </summary>
        [TableName("DF_FC25x5_AnHui")]
        [LotteryCode("ahfc25x5")]
        [Description("安徽福彩25选5")]
        AnHui22x5,

        #endregion

        #region 贵州



        #endregion

        #region 海南

        /// <summary>
        /// 海南4+1
        /// </summary>
        [TableName("DF_4J1_HaiNan")]
        [LotteryCode("")]
        [Description("海南4+1")]
        HaiNan4J1,

        #endregion

        #region 香港

        /// <summary>
        /// 香江彩
        /// </summary>
        [TableName("DF_SMHLHC_HongKong")]
        [LotteryCode("hk6")]
        [Description("香江彩")]
        HongKongSMHLHC,

        #endregion

        #endregion

        #region 高频彩

        #region 华东六省



        #endregion

        #region 北京

        /// <summary>
        /// 北京11选5
        /// </summary>
        [TableName("GP_11x5_BeiJing")]
        [LotteryCode("bj11x5")]
        BeiJing11x5,
        /// <summary>
        /// 北京快乐8
        /// </summary>
        [TableName("GP_KL8_BeiJing")]
        [LotteryCode("bjkl8")]
        BeiJingKL8,
        /// <summary>
        /// 北京快3
        /// </summary>
        [TableName("GP_K3_BeiJing")]
        [LotteryCode("bjk3")]
        BeiJingK3,
        /// <summary>
        /// 北京快中彩
        /// </summary>
        [TableName("GP_KZC_BeiJing")]
        [LotteryCode("bjkzc")]
        BeiJingKZC,
        /// <summary>
        /// 北京pk10(北京赛车)
        /// </summary>
        [TableName("GP_PK10_BeiJing")]
        [LotteryCode("bjpk10")]
        BeiJingPK10,
        /// <summary>
        /// 北京11选5
        /// </summary>
        [TableName("GP_11x5_BeiJing")]
        GP_11x5_BeiJing,

        #endregion

        #region 上海

        /// <summary>
        /// 上海时时乐
        /// </summary>
        [TableName("GP_SSL_ShangHai")]
        [LotteryCode("shssl")]
        ShangHaiSSL,
        /// <summary>
        /// 上海快3
        /// </summary>
        [TableName("GP_K3_ShangHai")]
        [LotteryCode("shk3")]
        ShangHaiK3,
        /// <summary>
        /// 上海11选5
        /// </summary>
        [TableName("GP_11x5_ShangHai")]
        [LotteryCode("sh11x5")]
        ShangHai11x5,

        #endregion

        #region 天津

        /// <summary>
        /// 天津快乐十分
        /// </summary>
        [TableName("GP_KL10F_TianJin")]
        [LotteryCode("tjklsf")]
        TianJinKL10F,
        /// <summary>
        /// 天津时时彩
        /// </summary>
        [TableName("GP_SSC_TianJin")]
        [LotteryCode("tjssc")]
        TianJinSSC,
        /// <summary>
        /// 天津11选5
        /// </summary>
        [TableName("GP_11x5_TianJin")]
        [LotteryCode("tj11x5")]
        TianJin11x5,

        #endregion

        #region 江苏

        /// <summary>
        /// 江苏11选5
        /// </summary>
        [TableName("GP_11x5_JiangSu")]
        [LotteryCode("js11x5")]
        JiangSu11X5,
        /// <summary>
        /// 江苏快3
        /// </summary>
        [TableName("GP_K3_JiangSu")]
        [LotteryCode("jsk3")]
        JiangSuK3,

        #endregion

        #region 浙江

        /// <summary>
        /// 浙江11选5
        /// </summary>
        [TableName("GP_11x5_ZheJiang")]
        [LotteryCode("zj11x5")]
        ZheJiang11X5,
        /// <summary>
        /// 浙江快乐12
        /// </summary>
        [TableName("GP_KL12_ZheJiang")]
        [LotteryCode("zjkl12")]
        ZheJiangKL12,

        #endregion

        #region 福建

        /// <summary>
        /// 福建11选5(即乐彩)
        /// </summary>
        [TableName("GP_11x5_FuJian")]
        [LotteryCode("fj11x5")]
        FuJian11x5,
        /// <summary>
        /// 福建快3
        /// </summary>
        [TableName("GP_K3_FuJian")]
        [LotteryCode("fjk3")]
        FuJianK3,

        #endregion

        #region 广东

        /// <summary>
        /// 广东11选5
        /// </summary>
        [TableName("GP_11x5_GuangDong")]
        [LotteryCode("gd11x5")]
        GuangDong11X5,
        /// <summary>
        /// 广东快乐十分
        /// </summary>
        [TableName("GP_KL10F_GuangDong")]
        [LotteryCode("gdklsf")]
        GuangDongKL10F,

        #endregion

        #region 深圳



        #endregion

        #region 黑龙江

        /// <summary>
        /// 黑龙江11选5
        /// </summary>
        [TableName("GP_11x5_HeiLongJiang")]
        [LotteryCode("hlj11x5")]
        HeiLongJiang11X5,
        /// <summary>
        /// 黑龙江快乐十分
        /// </summary>
        [TableName("GP_KL10F_HeiLongJiang")]
        [LotteryCode("hljklsf")]
        HeiLongJiangKL10F,
        /// <summary>
        /// 黑龙江时时彩
        /// </summary>
        [TableName("GP_SSC_HeiLongJiang")]
        [LotteryCode("hljssc")]
        HeiLongJiangSSC,

        #endregion

        #region 四川

        /// <summary>
        /// 四川快乐12
        /// </summary>
        [TableName("GP_KL12_SiChuan")]
        [LotteryCode("sckl12")]
        SiChuanKL12,

        #endregion

        #region 辽宁

        /// <summary>
        /// 辽宁11选5(全运彩)
        /// </summary>
        [TableName("GP_11x5_LiaoNing")]
        [LotteryCode("ln11x5")]
        LiaoNing11X5,
        /// <summary>
        /// 辽宁快乐12
        /// </summary>
        [TableName("GP_KL12_LiaoNing")]
        [LotteryCode("lnkl12")]
        LiaoNingKL12,

        #endregion

        #region 吉林

        /// <summary>
        /// 吉林11选5
        /// </summary>
        [TableName("GP_11x5_JiLin")]
        [LotteryCode("jl11x5")]
        JiLin11X5,
        /// <summary>
        /// 吉林快3
        /// </summary>
        [TableName("GP_K3_JiLin")]
        [LotteryCode("jlk3")]
        JiLinK3,

        #endregion

        #region 河北

        /// <summary>
        /// 河北11选5
        /// </summary>
        [TableName("GP_11x5_HeBei")]
        [LotteryCode("heb11x5")]
        HeBei11X5,
        /// <summary>
        /// 河北快3
        /// </summary>
        [TableName("GP_K3_HeBei")]
        [LotteryCode("hebk3")]
        HeBeiK3,

        #endregion

        #region 河南

        /// <summary>
        /// 河南快赢481(泳坛夺金)
        /// </summary>
        [TableName("GP_KY481_HeNan")]
        [LotteryCode("henytdj")]
        HeNanKY481,
        /// <summary>
        /// 河南快3
        /// </summary>
        [TableName("GP_K3_HeNan")]
        [LotteryCode("")]
        HeNanK3,

        #endregion

        #region 湖北

        /// <summary>
        /// 湖北快3
        /// </summary>
        [TableName("GP_K3_HuBei")]
        [LotteryCode("hubk3")]
        HuBeiK3,
        /// <summary>
        /// 湖北11选5
        /// </summary>
        [TableName("GP_11x5_HuBei")]
        [LotteryCode("hub11x5")]
        HuBei11x5,

        #endregion

        #region 山西

        /// <summary>
        /// 山西快乐十分
        /// </summary>
        [TableName("GP_KL10F_ShanXiTaiYuan")]
        [LotteryCode("sxrklsf")]
        ShanXiTaiYuanKL10F,
        /// <summary>
        /// 山西泳坛夺金(481)
        /// </summary>
        [TableName("GP_YTDJ_ShanXiTaiYuan")]
        [LotteryCode("sxrytdj")]
        ShanXiTaiYuan481,
        /// <summary>
        /// 山西11选5
        /// </summary>
        [TableName("GP_11x5_ShanXiTaiYuan")]
        [LotteryCode("sxr11x5")]
        ShanXiTaiYuan11x5,

        #endregion

        #region 云南

        /// <summary>
        /// 云南11选5
        /// </summary>
        [TableName("GP_11x5_YunNan")]
        [LotteryCode("yn11x5")]
        YunNan11X5,
        /// <summary>
        /// 云南快乐十分
        /// </summary>
        [TableName("GP_KL10F_YunNan")]
        [LotteryCode("ynklsf")]
        YunNanKL10F,
        /// <summary>
        /// 云南时时彩
        /// </summary>
        [TableName("GP_SSC_YunNan")]
        [LotteryCode("ynssc")]
        YunNanSSC,

        #endregion

        #region 新疆

        /// <summary>
        /// 新疆时时彩
        /// </summary>
        [TableName("GP_SSC_XinJiang")]
        [LotteryCode("xjssc")]
        XinJiangSSC,
        /// <summary>
        /// 新疆喜乐彩
        /// </summary>
        [TableName("GP_XLC_XinJiang")]
        [LotteryCode("xjxlc")]
        XinJiangXLC,
        /// <summary>
        /// 新疆11选5(新乐彩)
        /// </summary>
        [TableName("GP_11x5_XinJiang")]
        [LotteryCode("xj11x5")]
        XinJiang11x5,

        #endregion

        #region 重庆

        /// <summary>
        /// 重庆时时彩
        /// </summary>
        [TableName("GP_SSC_ChongQing")]
        [LotteryCode("cqssc")]
        ChongQingSSC,
        /// <summary>
        /// 重庆快乐十分(幸运农场)
        /// </summary>
        [TableName("GP_KL10F_ChongQing")]
        [LotteryCode("cqklsf")]
        ChongQingKL10F,

        #endregion

        #region 广西

        /// <summary>
        /// 广西快乐十分
        /// </summary>
        [TableName("GP_KL10F_GuangXi")]
        [LotteryCode("gxklsf")]
        GuangXiKL10F,
        /// <summary>
        /// 广西11选5
        /// </summary>
        [TableName("GP_11x5_GuangXi")]
        [LotteryCode("gx11x5")]
        GuangXi11x5,
        /// <summary>
        /// 广西快3
        /// </summary>
        [TableName("GP_K3_GuangXi")]
        [LotteryCode("gxk3")]
        GuangXiK3,

        #endregion

        #region 江西

        /// <summary>
        /// 江西11选5
        /// </summary>
        [TableName("GP_11x5_JiangXi")]
        [LotteryCode("jx11x5")]
        JiangXi11X5,
        /// <summary>
        /// 江西快3
        /// </summary>
        [TableName("GP_K3_JiangXi")]
        [LotteryCode("jxk3")]
        JiangXiK3,

        #endregion

        #region 安徽

        /// <summary>
        /// 安徽快3
        /// </summary>
        [TableName("GP_K3_AnHui")]
        [LotteryCode("ahk3")]
        AnHuiK3,
        /// <summary>
        /// 安徽11选5
        /// </summary>
        [TableName("GP_11x5_AnHui")]
        [LotteryCode("ah11x5")]
        AnHui11x5,

        #endregion

        #region 贵州

        /// <summary>
        /// 贵州11选5
        /// </summary>
        [TableName("GP_11x5_GuiZhou")]
        [LotteryCode("gz11x5")]
        GuiZhou11x5,
        /// <summary>
        /// 贵州快3
        /// </summary>
        [TableName("GP_K3_GuiZhou")]
        [LotteryCode("gzk3")]
        GuiZhouK3,

        #endregion

        #region 山东

        /// <summary>
        /// 山东11选5
        /// </summary>
        [TableName("GP_11x5_ShanDong")]
        [LotteryCode("sd11x5")]
        ShanDong11X5,
        /// <summary>
        /// 山东群英会
        /// </summary>
        [TableName("GP_QYH_ShanDong")]
        [LotteryCode("sdqyh")]
        ShanDongQYH,
        /// <summary>
        /// 山东快乐扑克3
        /// </summary>
        [TableName("GP_KLPK3_ShanDong")]
        [LotteryCode("sdklpk3")]
        ShanDongKLPK3,

        #endregion

        #region 海南



        #endregion

        #region 甘肃

        /// <summary>
        /// 甘肃11选5
        /// </summary>
        [TableName("GP_11x5_GanSu")]
        [LotteryCode("gs11x5")]
        GanSu11x5,
        /// <summary>
        /// 甘肃快3
        /// </summary>
        [TableName("GP_K3_GanSu")]
        [LotteryCode("gsk3")]
        GanSuK3,

        #endregion

        #region 湖南

        /// <summary>
        /// 湖南幸运赛车
        /// </summary>
        [TableName("GP_XYSC_HuNan")]
        [LotteryCode("hunxysc")]
        HuNanXYSC,
        /// <summary>
        /// 湖南快乐十分(动物总动员)
        /// </summary>
        [TableName("GP_KL10F_HuNan")]
        [LotteryCode("hunklsf")]
        HuNanKL10F,

        #endregion

        #region 陕西

        /// <summary>
        /// 陕西快乐十分
        /// </summary>
        [TableName("GP_KL10F_ShanXiXiAn")]
        [LotteryCode("sxlklsf")]
        ShanXiXiAnKL10F,
        /// <summary>
        /// 陕西11选5
        /// </summary>
        [TableName("GP_11x5_ShanXiXiAn")]
        [LotteryCode("sxl11x5")]
        ShanXiXiAn11x5,

        #endregion

        #region 内蒙古

        /// <summary>
        /// 内蒙古快3
        /// </summary>
        [TableName("GP_K3_NeiMengGu")]
        [LotteryCode("nmgk3")]
        NeiMengGuK3,
        /// <summary>
        /// 内蒙古11选5
        /// </summary>
        [TableName("GP_11x5_NeiMengGu")]
        [LotteryCode("nmg11x5")]
        NeiMengGu11x5,

        #endregion

        #region 青海

        /// <summary>
        /// 青海快3
        /// </summary>
        [TableName("GP_K3_QingHai")]
        [LotteryCode("")]
        QingHaiK3,

        #endregion

        #region 香港



        #endregion

        #endregion

        #region 彩票技巧
        /// <summary>
        ///
        /// </summary>
        [TableName("B_LotterySkill")]
        LotterySkill,
        #endregion

        #region 彩票术语
        /// <summary>
        ///
        /// </summary>
        [TableName("B_LotteryGlossary")]
        LotteryGlossary,
        #endregion


        #region 彩票术语
        /// <summary>
        ///
        /// </summary>
        [TableName("B_LotteryNews")]
        LotteryNews,
        #endregion

    }

    /// <summary>
    /// 彩种技巧枚举
    /// </summary>
    public enum LotterySkillType
    {
        [Description("福彩3D技巧")]
        FC3D = 1,
        
        [Description("双色球技巧")]
        SSQ = 2,

        [Description("排列三技巧")]
        PL3 = 3,

        [Description("大乐透技巧")]
        DLT = 4,

        [Description("七乐彩技巧")]
        QLC = 5,

        [Description("七星彩技巧")]
        QXC = 6,

        [Description("重庆时时彩")]
        CQSSC = 7,

        [Description("快乐123技巧")]
        KL123 = 8,

        [Description("上海时时乐")]
        SHSSC = 9,

        [Description("群英会")]
        QYH = 10,

        [Description("十一运夺金")]
        SYYDJ = 11,
    }

    /// <summary>
    /// 彩种术语枚举
    /// </summary>
    public enum LotteryGlossaryType
    {
        [Description("福彩3D术语")]
        FC3D = 1,
        [Description("双色球术语")]
        SSQ = 2,
        [Description("七乐彩常用术语")]
        QLC = 3,
        [Description("排列三常用术语")]
        P3 = 4,
        [Description("大乐透常用术语")]
        DLT = 5,
        [Description("七星彩常用术语")]
        QXC = 6,
    }

    public enum LotteryNewsType
    {
        [Description("双色球新闻")]
        SSQNews = 1,
        [Description("双色球中奖报道")]
        SSQZhongJiangNews = 2,
        [Description("福彩3D新闻")]
        FC3DNews = 3,
        [Description("七乐彩新闻")]
        QLCNews = 4,
        [Description("其他彩种新闻")]
        QTNews = 5,
  
    }
}
