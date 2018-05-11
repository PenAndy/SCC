using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SCC.Models;

namespace SCC.Crawler.DT
{
    public class CssValueFunction
    {
        #region 单值
        /// <summary>
        /// 单值单列期号
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_TermItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css,
            string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值单列开奖号012路值
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_012StatusItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css,
            string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值单列和尾
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_HeWeiItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css,
            string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值单列跨度值
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_SpanItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css,
            string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值单列组三跨度值
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_ZSSpanItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css,
            string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值单列和值
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_SumItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css,
            string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值单列试机号
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_ShiJiHaoItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css,
            string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值多列试机号分布
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_ShiJiHao(bool isValue, string fomart, string attr, ChartCssConfigInfo css,
            string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }

        /// <summary>
        /// 单值多列组三形态
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_ZsStatusItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css,
            string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值多列组三奇偶形态
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_ZsJoStatusItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css,
            string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值多列和尾分布形态
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_HeWeiItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css,
            string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值多列组三大小形态
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_ZsDxStatusItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css,
            string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值多列组三012形态
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_Zs012StatusItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css,
            string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值单列后区号码
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_HqItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css,
            string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值单列重号项
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_RepeatedNumber(bool isValue, string fomart, string attr, ChartCssConfigInfo css,
            string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值单列连号项
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_LinkNumber(bool isValue, string fomart, string attr, ChartCssConfigInfo css,
            string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 和值(区间)分布
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_SumItemGroup(bool isValue, string fomart, string attr, ChartCssConfigInfo css,
            string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }

        /// <summary>
        /// 单值单列开奖号码
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_OpenCodeItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css,
            string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值单列012比
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_ProportionOf012Item(bool isValue, string fomart, string attr, ChartCssConfigInfo css,
            string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值单列大小比例
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_ProportionOfDxItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css,
            string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值单列质合形态比例
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_ProportionOfZhItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css,
            string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值单列奇偶比例
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_ProportionOfJoItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css,
            string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }

        /// <summary>
        /// 单值多列号码项
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_NumberItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值多列大小状态项
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_DaXiaoStatusItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值多列012分布项
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_Number012StatusItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值多列奇偶状态项
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_JiOuStatusItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值多列多于两个号码跨度项
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_SpanItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值多列两个号码跨度项
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_SpanNumberItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值多列开奖号码和值奇偶分布项
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_HzJoStatusItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值多列开奖号码和值大小分布项
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_HzDxStatusItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值多列开奖号码和值分布
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_SumItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值单列开奖号码质合项
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_ZhiHeStatusItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值多列开奖号码质合分布项
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_ZhiHeStatusItem(bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值多列组三类型
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_ZuHeStatusItem<TEntity>(TEntity LocalEntity, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            if (itemValue == "组三")
            {
                return GetCssValue(isValue, fomart, attr, css, "3", index);
            }
            if (itemValue == "组六")
            {
                return GetCssValueExtend1(isValue, fomart, attr, css, "6", index);
            }
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值单列双色球AC值
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_Ac<TEntity>(TEntity LocalEntity, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值单列双色球三区比
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_SanQu<TEntity>(TEntity LocalEntity, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值单列AC值奇偶值
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_AcJiOu<TEntity>(TEntity LocalEntity, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值单列AC值质合值
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_AcZhiHe<TEntity>(TEntity LocalEntity, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值单列AC值012路
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_Ac012Lu<TEntity>(TEntity LocalEntity, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 和尾奇偶状态
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_HeWeiJiOu<TEntity>(TEntity LocalEntity, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值单列试机号和值项
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_ShiJiHaoHzItem<TEntity>(TEntity LocalEntity, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值单列试机号跨度项
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_ShiJiHaoSpanItem<TEntity>(TEntity LocalEntity, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值单列试机号大小比项
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_ProportionOfShiJiHaoDxItem<TEntity>(TEntity LocalEntity, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值多列试机号组三组六类型比项
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_ShiJiHaoTypeItem<TEntity>(TEntity LocalEntity, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            if (itemValue == "组三")
            {
                return GetCssValue(isValue, fomart, attr, css, "3", index);
            }
            if (itemValue == "组六")
            {
                return GetCssValueExtend1(isValue, fomart, attr, css, "6", index);
            }
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值单列组三号码
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_ZsHaoMaItem<TEntity>(TEntity LocalEntity, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值单列组三遗漏
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_ZsMissItem<TEntity>(TEntity LocalEntity, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 单值单列试机号奇偶比比项
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_ProportionOfShiJiHaoJoItem<TEntity>(TEntity LocalEntity, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        #endregion

        #region 多值
        /// <summary>
        /// 多值多列重号分布
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string MultiValue_RepeatNumber<TEntity>(TEntity LocalEntity, TrendChartItemInfo ItemConfig, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            //IList<int> list = LotteryUtils.GetOpenCodeList<TEntity>(LocalEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            ////上一期数据 ,下一期数据
            //IList<int> lastOpenCode = null, nextOpenCode = null;
            ////根据Cid判断_彩种
            //switch (ItemConfig.Cid)
            //{
            //    case 2:
            //    case 3: //P5
            //        IList<TCP3Info> TCP3lastList = TCP3Service.ToListForTrend(LocalEntity.Term, null);//获取前一期开奖数据
            //        if (null != TCP3lastList && TCP3lastList[0].Term == LocalEntity.Term && TCP3lastList.Count == 2)
            //        {
            //            lastOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(TCP3lastList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //        }
            //        IList<TCP3Info> TCP3nextList = TCP3Service.ToListForNextTrend(LocalEntity.Term, null);
            //        if (null != TCP3nextList && TCP3nextList[0].Term == LocalEntity.Term && TCP3nextList.Count == 2)//获取后一期开奖数据
            //        {
            //            nextOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(TCP3nextList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //        }
            //        if (null != lastOpenCode && lastOpenCode.Contains(itemValue.ToInt())) //如果上期开奖号码中有本期项值（即重号）
            //        {
            //            return GetCssValueExtend1(isValue, fomart, attr, css, itemValue, index);
            //        }
            //        if (null != nextOpenCode && nextOpenCode.Contains(itemValue.ToInt())) //如果下期开奖号码中有本期项值（即重号）
            //        {
            //            return GetCssValueExtend1(isValue, fomart, attr, css, itemValue, index);
            //        }
            //        return GetCssValue(isValue, fomart, attr, css, itemValue, index);
            //    case 4: //双色球
            //        IList<FCSSQInfo> FCSSQlastList = FCSSQService.ToListForTrend(LocalEntity.Term, null);//获取前一期开奖数据
            //        if (null != FCSSQlastList && FCSSQlastList[0].Term == LocalEntity.Term && FCSSQlastList.Count == 2)
            //        {
            //            lastOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(FCSSQlastList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //        }
            //        IList<FCSSQInfo> FCSSQnextList = FCSSQService.ToListForNextTrend(LocalEntity.Term, null);
            //        if (null != FCSSQnextList && FCSSQnextList[0].Term == LocalEntity.Term && FCSSQnextList.Count == 2)//获取后一期开奖数据
            //        {
            //            nextOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(FCSSQnextList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //        }
            //        if (css != null) //有样式
            //        {
            //            if (css.ChildList == null) //没有子样式 
            //            {
            //                if (isValue) //项值
            //                {
            //                    return string.Format(fomart, css.NumberCssName, attr, itemValue);
            //                }
            //                else //遗漏
            //                {
            //                    return string.Format(fomart, css.MissCssName, attr, itemValue);
            //                }
            //            }
            //            else //有子样式
            //            {
            //                foreach (var item in css.ChildList)
            //                {
            //                    for (int i = item.endNum; i >= item.startNum; i--)
            //                    {

            //                        if (isValue)
            //                        {
            //                            if (index == i - css.ChildList[0].startNum)
            //                            {
            //                                if (null != lastOpenCode && lastOpenCode.Contains(itemValue.ToInt())) //如果上期开奖号码中有本期项值（即重号）
            //                                {
            //                                    return string.Format(fomart, item.Extend1, attr, itemValue);
            //                                }
            //                                if (null != nextOpenCode && nextOpenCode.Contains(itemValue.ToInt())) //如果下期开奖号码中有本期项值（即重号）
            //                                {
            //                                    return string.Format(fomart, item.Extend1, attr, itemValue);
            //                                }
            //                                return string.Format(fomart, item.NumberCssName, attr, itemValue);
            //                            }
            //                        }
            //                        else
            //                        {
            //                            if (index == i - css.ChildList[0].startNum)
            //                            {
            //                                return string.Format(fomart, item.MissCssName, attr, itemValue);
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        break;
            //    case 5: //七乐彩
            //        IList<FCQLCInfo> FCQLClastList = FCQLCService.ToListForTrend(LocalEntity.Term, null);//获取前一期开奖数据
            //        if (null != FCQLClastList && FCQLClastList[0].Term == LocalEntity.Term && FCQLClastList.Count == 2)
            //        {
            //            lastOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(FCQLClastList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //        }
            //        IList<FCQLCInfo> FCQLCnextList = FCQLCService.ToListForNextTrend(LocalEntity.Term, null);
            //        if (null != FCQLCnextList && FCQLCnextList[0].Term == LocalEntity.Term && FCQLCnextList.Count == 2)//获取后一期开奖数据
            //        {
            //            nextOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(FCQLCnextList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //        }
            //        if (css != null) //有样式
            //        {
            //            if (css.ChildList == null) //没有子样式 
            //            {
            //                if (isValue) //项值
            //                {
            //                    return string.Format(fomart, css.NumberCssName, attr, itemValue);
            //                }
            //                else //遗漏
            //                {
            //                    return string.Format(fomart, css.MissCssName, attr, itemValue);
            //                }
            //            }
            //            else //有子样式
            //            {
            //                foreach (var item in css.ChildList)
            //                {
            //                    for (int i = item.endNum; i >= item.startNum; i--)
            //                    {

            //                        if (isValue)
            //                        {
            //                            if (index == i - css.ChildList[0].startNum)
            //                            {
            //                                if (null != lastOpenCode && lastOpenCode.Contains(itemValue.ToInt())) //如果上期开奖号码中有本期项值（即重号）
            //                                {
            //                                    if (list[list.Count - 1] == itemValue.ToInt()) //是特别号码
            //                                    {
            //                                        return string.Format(fomart, item.Extend3, attr, itemValue);
            //                                    }
            //                                    return string.Format(fomart, item.Extend2, attr, itemValue);
            //                                }
            //                                if (null != nextOpenCode && nextOpenCode.Contains(itemValue.ToInt())) //如果下期开奖号码中有本期项值（即重号）
            //                                {
            //                                    if (list[list.Count - 1] == itemValue.ToInt()) //是特别号码
            //                                    {
            //                                        return string.Format(fomart, item.Extend3, attr, itemValue);
            //                                    }
            //                                    return string.Format(fomart, item.Extend2, attr, itemValue);
            //                                }
            //                                if (list[list.Count - 1] == itemValue.ToInt()) //是特别号码
            //                                {
            //                                    return string.Format(fomart, item.Extend1, attr, itemValue);
            //                                }
            //                                return string.Format(fomart, item.NumberCssName, attr, itemValue);
            //                            }
            //                        }
            //                        else
            //                        {
            //                            if (index == i - css.ChildList[0].startNum)
            //                            {
            //                                return string.Format(fomart, item.MissCssName, attr, itemValue);
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        break;
            //    case 12: //大乐透
            //        IList<TCDLTInfo> TCDLTlastList = TCDLTService.ToListForTrend(LocalEntity.Term, null);//获取前一期开奖数据
            //        if (null != TCDLTlastList && TCDLTlastList[0].Term == LocalEntity.Term && TCDLTlastList.Count == 2)
            //        {
            //            lastOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(TCDLTlastList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //        }
            //        IList<TCDLTInfo> TCDLTnextList = TCDLTService.ToListForNextTrend(LocalEntity.Term, null);
            //        if (null != TCDLTnextList && TCDLTnextList[0].Term == LocalEntity.Term && TCDLTnextList.Count == 2)//获取后一期开奖数据
            //        {
            //            nextOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(TCDLTnextList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //        }
            //        if (css != null) //有样式
            //        {
            //            if (css.ChildList == null) //没有子样式 
            //            {
            //                if (isValue) //项值
            //                {
            //                    return string.Format(fomart, css.NumberCssName, attr, itemValue);
            //                }
            //                else //遗漏
            //                {
            //                    return string.Format(fomart, css.MissCssName, attr, itemValue);
            //                }
            //            }
            //            else //有子样式
            //            {
            //                foreach (var item in css.ChildList)
            //                {
            //                    for (int i = item.endNum; i >= item.startNum; i--)
            //                    {

            //                        if (isValue)
            //                        {
            //                            if (index == i - css.ChildList[0].startNum)
            //                            {
            //                                if (null != lastOpenCode && lastOpenCode.Contains(itemValue.ToInt())) //如果上期开奖号码中有本期项值（即重号）
            //                                {
            //                                    return string.Format(fomart, item.Extend1, attr, itemValue);
            //                                }
            //                                if (null != nextOpenCode && nextOpenCode.Contains(itemValue.ToInt())) //如果下期开奖号码中有本期项值（即重号）
            //                                {
            //                                    return string.Format(fomart, item.Extend1, attr, itemValue);
            //                                }
            //                                return string.Format(fomart, item.NumberCssName, attr, itemValue);
            //                            }
            //                        }
            //                        else
            //                        {
            //                            if (index == i - css.ChildList[0].startNum)
            //                            {
            //                                return string.Format(fomart, item.MissCssName, attr, itemValue);
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        break;
            //}
            return string.Format(fomart, "", attr, itemValue); //没有样式(填充默认颜色)
        }

        /// <summary>
        /// 多值多列连号
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string MultiValue_LinkNumber<TEntity>(TEntity LocalEntity, TrendChartItemInfo ItemConfig, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            IList<int> list = LotteryUtils.GetOpenCodeList<TEntity>(LocalEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //根据Cid判断_彩种
            switch (ItemConfig.Cid)
            {
                case 4:        //双色球
                    if (css != null) //有样式
                    {
                        if (css.ChildList == null) //没有子样式 
                        {
                            if (isValue) //项值
                            {
                                return string.Format(fomart, css.NumberCssName, attr, itemValue);
                            }
                            else //遗漏
                            {
                                return string.Format(fomart, css.MissCssName, attr, itemValue);
                            }
                        }
                        else //有子样式
                        {
                            foreach (var item in css.ChildList)
                            {
                                for (int i = item.endNum; i >= item.startNum; i--)
                                {

                                    if (isValue)
                                    {
                                        if (index == i - css.ChildList[0].startNum)
                                        {
                                            if (list.Contains(itemValue.ToInt() - 1) || list.Contains(itemValue.ToInt() + 1)) //是连号用扩展样式1
                                            {
                                                return string.Format(fomart, item.Extend1, attr, itemValue);
                                            }
                                            return string.Format(fomart, item.NumberCssName, attr, itemValue);
                                        }
                                    }
                                    else
                                    {
                                        if (index == i - css.ChildList[0].startNum)
                                        {
                                            return string.Format(fomart, item.MissCssName, attr, itemValue);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case 5:        //七乐彩
                    if (css != null) //有样式
                    {
                        if (css.ChildList == null) //没有子样式 
                        {
                            if (isValue) //项值
                            {
                                return string.Format(fomart, css.NumberCssName, attr, itemValue);
                            }
                            else //遗漏
                            {
                                return string.Format(fomart, css.MissCssName, attr, itemValue);
                            }
                        }
                        else //有子样式
                        {
                            foreach (var item in css.ChildList)
                            {
                                for (int i = item.endNum; i >= item.startNum; i--)
                                {

                                    if (isValue)
                                    {
                                        if (index == i - css.ChildList[0].startNum)
                                        {
                                            if (list.Contains(itemValue.ToInt() - 1) || list.Contains(itemValue.ToInt() + 1)) //是连号用扩展样式（红球用扩展2，蓝球用扩展3）
                                            {
                                                if (itemValue.ToInt() == list[list.Count - 1])//蓝球连号
                                                {
                                                    return string.Format(fomart, item.Extend3, attr, itemValue);
                                                }
                                                return string.Format(fomart, item.Extend2, attr, itemValue);
                                            }
                                            if (itemValue.ToInt() == list[list.Count - 1])//不是连号，是项值（蓝球）
                                            {
                                                return string.Format(fomart, item.Extend1, attr, itemValue);
                                            }
                                            return string.Format(fomart, item.NumberCssName, attr, itemValue);//不是连号，是项值（红球）
                                        }
                                    }
                                    else
                                    {
                                        if (index == i - css.ChildList[0].startNum)
                                        {
                                            return string.Format(fomart, item.MissCssName, attr, itemValue);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case 12:        //大乐透
                    if (css != null) //有样式
                    {
                        if (css.ChildList == null) //没有子样式 
                        {
                            if (isValue) //项值
                            {
                                return string.Format(fomart, css.NumberCssName, attr, itemValue);
                            }
                            else //遗漏
                            {
                                return string.Format(fomart, css.MissCssName, attr, itemValue);
                            }
                        }
                        else //有子样式
                        {
                            foreach (var item in css.ChildList)
                            {
                                for (int i = item.endNum; i >= item.startNum; i--)
                                {

                                    if (isValue)
                                    {
                                        if (index == i - css.ChildList[0].startNum)
                                        {
                                            if (list.Contains(itemValue.ToInt() - 1) || list.Contains(itemValue.ToInt() + 1)) //是连号用扩展样式1
                                            {
                                                return string.Format(fomart, item.Extend1, attr, itemValue);
                                            }
                                            return string.Format(fomart, item.NumberCssName, attr, itemValue);
                                        }
                                    }
                                    else
                                    {
                                        if (index == i - css.ChildList[0].startNum)
                                        {
                                            return string.Format(fomart, item.MissCssName, attr, itemValue);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }

        /// <summary>
        /// 多值多列开奖号码
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string MultiValue_OpenCodeItem<TEntity>(TEntity LocalEntity, TrendChartItemInfo ItemConfig, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            IList<int> list = LotteryUtils.GetOpenCodeList<TEntity>(LocalEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            IDictionary<int, int> d = new Dictionary<int, int>();
            //根据Cid判断_彩种
            switch (ItemConfig.Cid)
            {
                case 1:   //福彩3D
                case 2:   //体彩P3
                case 88:   //江苏快3  djp 2016-06-16 新增组三样式形态
                case 86:   //安徽快3
                case 87:   //湖北快3  暂未开奖2016-06-16
                case 89:   //吉林快3
                case 96:   //河北快3
                case 97:   //内蒙古快3
                    foreach (var item in list)
                    {
                        if (!d.ContainsKey(item))
                            d.Add(item, 0);
                        d[item]++;
                    }
                    foreach (var item in d.Keys)
                    {
                        if (2 == d[item])
                        {
                            if (item.ToString() == itemValue)
                            {
                                return GetCssValueExtend1(isValue, fomart, attr, css, itemValue, index);
                            }
                        }
                        if (3 == d[item])
                        {
                            return GetCssValueExtend2(isValue, fomart, attr, css, itemValue, index);
                        }
                    }
                    break;
                case 5:  //七乐彩
                case 60: //福建31选7
                case 61: //福建36选7
                case 65: //华东15选5
                case 68: //新疆35选7
                case 69: //南粤36选7
                    if (list[list.Count - 1].ToString() == itemValue)
                    {
                        return GetCssValueExtend1(isValue, fomart, attr, css, itemValue, index);
                    }
                    break;
                case 19:  //七星彩
                    foreach (var item in list)
                    {
                        if (!d.ContainsKey(item))
                            d.Add(item, 0);
                        d[item]++;
                    }
                    foreach (var item in d.Keys)
                    {
                        if (2 <= d[item])
                        {
                            if (item.ToString() == itemValue)
                            {
                                return GetCssValueExtend1(isValue, fomart, attr, css, itemValue, index);
                            }
                        }
                    }
                    break;
            }
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }

        /// <summary>
        /// 多值多列折号
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string MultiValue_ZheHaoNumber<TEntity>(TEntity LocalEntity, TrendChartItemInfo ItemConfig, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            //IList<int> list = LotteryUtils.GetOpenCodeList<TEntity>(LocalEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            ////上一期数据 ,下一期数据
            //IList<int> lastOpenCode = null, nextOpenCode = null;
            ////根据Cid判断_彩种
            //switch (ItemConfig.Cid)
            //{
            //    case 4:   //双色球
            //        IList<FCSSQInfo> FCSSQlastList = FCSSQService.ToListForTrend(LocalEntity.Term, null);//获取前一期开奖数据
            //        if (null != FCSSQlastList && FCSSQlastList[0].Term == LocalEntity.Term && FCSSQlastList.Count == 2)
            //        {
            //            lastOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(FCSSQlastList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //        }
            //        IList<FCSSQInfo> FCSSQnextList = FCSSQService.ToListForNextTrend(LocalEntity.Term, null);
            //        if (null != FCSSQnextList && FCSSQnextList[0].Term == LocalEntity.Term && FCSSQnextList.Count == 2)//获取后一期开奖数据
            //        {
            //            nextOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(FCSSQnextList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //        }
            //        if (css != null) //有样式
            //        {
            //            if (css.ChildList == null) //没有子样式 
            //            {
            //                if (isValue) //项值
            //                {
            //                    return string.Format(fomart, css.NumberCssName, attr, itemValue);
            //                }
            //                else //遗漏
            //                {
            //                    return string.Format(fomart, css.MissCssName, attr, itemValue);
            //                }
            //            }
            //            else //有子样式
            //            {
            //                foreach (var item in css.ChildList)
            //                {
            //                    for (int i = item.endNum; i >= item.startNum; i--)
            //                    {

            //                        if (isValue)
            //                        {
            //                            if (index == i - css.ChildList[0].startNum)
            //                            {
            //                                if (null != lastOpenCode && ((lastOpenCode.Contains(itemValue.ToInt() - 1) && list.Contains(itemValue.ToInt() - 1)) || (lastOpenCode.Contains(itemValue.ToInt() - 1) && lastOpenCode.Contains(itemValue.ToInt())) || (lastOpenCode.Contains(itemValue.ToInt()) && lastOpenCode.Contains(itemValue.ToInt() + 1)) || (lastOpenCode.Contains(itemValue.ToInt() + 1) && list.Contains(itemValue.ToInt() + 1)) || (lastOpenCode.Contains(itemValue.ToInt()) && list.Contains(itemValue.ToInt() - 1)) || (lastOpenCode.Contains(itemValue.ToInt()) && list.Contains(itemValue.ToInt() + 1)))) //根据上期开奖号码判断当前是否折号
            //                                {
            //                                    return string.Format(fomart, item.Extend1, attr, itemValue);
            //                                }
            //                                if (null != nextOpenCode && ((nextOpenCode.Contains(itemValue.ToInt() - 1) && list.Contains(itemValue.ToInt() - 1)) || (nextOpenCode.Contains(itemValue.ToInt() - 1) && nextOpenCode.Contains(itemValue.ToInt())) || (nextOpenCode.Contains(itemValue.ToInt()) && nextOpenCode.Contains(itemValue.ToInt() + 1)) || (nextOpenCode.Contains(itemValue.ToInt() + 1) && list.Contains(itemValue.ToInt() + 1)) || (nextOpenCode.Contains(itemValue.ToInt()) && list.Contains(itemValue.ToInt() - 1)) || (nextOpenCode.Contains(itemValue.ToInt()) && list.Contains(itemValue.ToInt() + 1)))) //根据下期开奖号码判断当前是否折号
            //                                {
            //                                    return string.Format(fomart, item.Extend1, attr, itemValue);
            //                                }
            //                                return string.Format(fomart, item.NumberCssName, attr, itemValue);
            //                            }
            //                        }
            //                        else
            //                        {
            //                            if (index == i - css.ChildList[0].startNum)
            //                            {
            //                                return string.Format(fomart, item.MissCssName, attr, itemValue);
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        break;
            //    case 12:   //大乐透
            //        IList<TCDLTInfo> TCDLTlastList = TCDLTService.ToListForTrend(LocalEntity.Term, null);//获取前一期开奖数据
            //        if (null != TCDLTlastList && TCDLTlastList[0].Term == LocalEntity.Term && TCDLTlastList.Count == 2)
            //        {
            //            lastOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(TCDLTlastList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //        }
            //        IList<TCDLTInfo> TCDLTnextList = TCDLTService.ToListForNextTrend(LocalEntity.Term, null);
            //        if (null != TCDLTnextList && TCDLTnextList[0].Term == LocalEntity.Term && TCDLTnextList.Count == 2)//获取后一期开奖数据
            //        {
            //            nextOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(TCDLTnextList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //        }
            //        if (css != null) //有样式
            //        {
            //            if (css.ChildList == null) //没有子样式 
            //            {
            //                if (isValue) //项值
            //                {
            //                    return string.Format(fomart, css.NumberCssName, attr, itemValue);
            //                }
            //                else //遗漏
            //                {
            //                    return string.Format(fomart, css.MissCssName, attr, itemValue);
            //                }
            //            }
            //            else //有子样式
            //            {
            //                foreach (var item in css.ChildList)
            //                {
            //                    for (int i = item.endNum; i >= item.startNum; i--)
            //                    {

            //                        if (isValue)
            //                        {
            //                            if (index == i - css.ChildList[0].startNum)
            //                            {
            //                                if (null != lastOpenCode && ((lastOpenCode.Contains(itemValue.ToInt() - 1) && list.Contains(itemValue.ToInt() - 1)) || (lastOpenCode.Contains(itemValue.ToInt() - 1) && lastOpenCode.Contains(itemValue.ToInt())) || (lastOpenCode.Contains(itemValue.ToInt()) && lastOpenCode.Contains(itemValue.ToInt() + 1)) || (lastOpenCode.Contains(itemValue.ToInt() + 1) && list.Contains(itemValue.ToInt() + 1)) || (lastOpenCode.Contains(itemValue.ToInt()) && list.Contains(itemValue.ToInt() - 1)) || (lastOpenCode.Contains(itemValue.ToInt()) && list.Contains(itemValue.ToInt() + 1)))) //根据上期开奖号码判断当前是否折号
            //                                {
            //                                    return string.Format(fomart, item.Extend1, attr, itemValue);
            //                                }
            //                                if (null != nextOpenCode && ((nextOpenCode.Contains(itemValue.ToInt() - 1) && list.Contains(itemValue.ToInt() - 1)) || (nextOpenCode.Contains(itemValue.ToInt() - 1) && nextOpenCode.Contains(itemValue.ToInt())) || (nextOpenCode.Contains(itemValue.ToInt()) && nextOpenCode.Contains(itemValue.ToInt() + 1)) || (nextOpenCode.Contains(itemValue.ToInt() + 1) && list.Contains(itemValue.ToInt() + 1)) || (nextOpenCode.Contains(itemValue.ToInt()) && list.Contains(itemValue.ToInt() - 1)) || (nextOpenCode.Contains(itemValue.ToInt()) && list.Contains(itemValue.ToInt() + 1)))) //根据下期开奖号码判断当前是否折号
            //                                {
            //                                    return string.Format(fomart, item.Extend1, attr, itemValue);
            //                                }
            //                                return string.Format(fomart, item.NumberCssName, attr, itemValue);
            //                            }
            //                        }
            //                        else
            //                        {
            //                            if (index == i - css.ChildList[0].startNum)
            //                            {
            //                                return string.Format(fomart, item.MissCssName, attr, itemValue);
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        break;
            //}
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 多值多列斜连号分布
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string MultiValue_XieLianHaoNumber<TEntity>(TEntity LocalEntity, TrendChartItemInfo ItemConfig, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            //IList<int> list = LotteryUtils.GetOpenCodeList<TEntity>(LocalEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            ////上一期数据 ,下一期数据
            //IList<int> lastOpenCode = null, nextOpenCode = null;
            ////根据Cid判断_彩种
            //switch (ItemConfig.Cid)
            //{
            //    case 4:   //双色球
            //        IList<FCSSQInfo> FCSSQlastList = FCSSQService.ToListForTrend(LocalEntity.Term, null);//获取前一期开奖数据
            //        if (null != FCSSQlastList && FCSSQlastList[0].Term == LocalEntity.Term && FCSSQlastList.Count == 2)
            //        {
            //            lastOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(FCSSQlastList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //        }
            //        IList<FCSSQInfo> FCSSQnextList = FCSSQService.ToListForNextTrend(LocalEntity.Term, null);
            //        if (null != FCSSQnextList && FCSSQnextList[0].Term == LocalEntity.Term && FCSSQnextList.Count == 2)//获取后一期开奖数据
            //        {
            //            nextOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(FCSSQnextList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //        }
            //        if (css != null) //有样式
            //        {
            //            if (css.ChildList == null) //没有子样式 
            //            {
            //                if (isValue) //项值
            //                {
            //                    return string.Format(fomart, css.NumberCssName, attr, itemValue);
            //                }
            //                else //遗漏
            //                {
            //                    return string.Format(fomart, css.MissCssName, attr, itemValue);
            //                }
            //            }
            //            else //有子样式
            //            {
            //                foreach (var item in css.ChildList)
            //                {
            //                    for (int i = item.endNum; i >= item.startNum; i--)
            //                    {

            //                        if (isValue)
            //                        {
            //                            if (index == i - css.ChildList[0].startNum)
            //                            {
            //                                if (null != lastOpenCode && (lastOpenCode.Contains(itemValue.ToInt() - 1) || lastOpenCode.Contains(itemValue.ToInt() + 1))) //根据上期开奖号码判断当前是否斜连号
            //                                {
            //                                    return string.Format(fomart, item.Extend1, attr, itemValue);
            //                                }
            //                                if (null != nextOpenCode && (nextOpenCode.Contains(itemValue.ToInt() - 1) || nextOpenCode.Contains(itemValue.ToInt() + 1))) //根据下期开奖号码判断当前是否斜连号
            //                                {
            //                                    return string.Format(fomart, item.Extend1, attr, itemValue);
            //                                }
            //                                return string.Format(fomart, item.NumberCssName, attr, itemValue);
            //                            }
            //                        }
            //                        else
            //                        {
            //                            if (index == i - css.ChildList[0].startNum)
            //                            {
            //                                return string.Format(fomart, item.MissCssName, attr, itemValue);
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        break;
            //    case 12:   //大乐透
            //        IList<TCDLTInfo> TCDLTlastList = TCDLTService.ToListForTrend(LocalEntity.Term, null);//获取前一期开奖数据
            //        if (null != TCDLTlastList && TCDLTlastList[0].Term == LocalEntity.Term && TCDLTlastList.Count == 2)
            //        {
            //            lastOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(TCDLTlastList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //        }
            //        IList<TCDLTInfo> TCDLTnextList = TCDLTService.ToListForNextTrend(LocalEntity.Term, null);
            //        if (null != TCDLTnextList && TCDLTnextList[0].Term == LocalEntity.Term && TCDLTnextList.Count == 2)//获取后一期开奖数据
            //        {
            //            nextOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(TCDLTnextList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //        }
            //        if (css != null) //有样式
            //        {
            //            if (css.ChildList == null) //没有子样式 
            //            {
            //                if (isValue) //项值
            //                {
            //                    return string.Format(fomart, css.NumberCssName, attr, itemValue);
            //                }
            //                else //遗漏
            //                {
            //                    return string.Format(fomart, css.MissCssName, attr, itemValue);
            //                }
            //            }
            //            else //有子样式
            //            {
            //                foreach (var item in css.ChildList)
            //                {
            //                    for (int i = item.endNum; i >= item.startNum; i--)
            //                    {

            //                        if (isValue)
            //                        {
            //                            if (index == i - css.ChildList[0].startNum)
            //                            {
            //                                if (null != lastOpenCode && (lastOpenCode.Contains(itemValue.ToInt() - 1) || lastOpenCode.Contains(itemValue.ToInt() + 1))) //根据上期开奖号码判断当前是否斜连号
            //                                {
            //                                    return string.Format(fomart, item.Extend1, attr, itemValue);
            //                                }
            //                                if (null != nextOpenCode && (nextOpenCode.Contains(itemValue.ToInt() - 1) || nextOpenCode.Contains(itemValue.ToInt() + 1))) //根据下期开奖号码判断当前是否斜连号
            //                                {
            //                                    return string.Format(fomart, item.Extend1, attr, itemValue);
            //                                }
            //                                return string.Format(fomart, item.NumberCssName, attr, itemValue);
            //                            }
            //                        }
            //                        else
            //                        {
            //                            if (index == i - css.ChildList[0].startNum)
            //                            {
            //                                return string.Format(fomart, item.MissCssName, attr, itemValue);
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        break;
            //}
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 多值多列跳号
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string MultiValue_XieTiaoHaoNumber<TEntity>(TEntity LocalEntity, TrendChartItemInfo ItemConfig, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            //IList<int> list = LotteryUtils.GetOpenCodeList<TEntity>(LocalEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            ////上一期数据 ,下一期数据
            //IList<int> lastOpenCode = null, nextOpenCode = null, lastlastOpenCode = null, nextnextOpenCode = null; ;
            ////根据Cid判断_彩种
            //switch (ItemConfig.Cid)
            //{
            //    case 4: //双色球
            //        IList<FCSSQInfo> FCSSQlastList = FCSSQService.ToListForTrend(LocalEntity.Term, null);//获取前一期开奖数据
            //        if (null != FCSSQlastList && FCSSQlastList[0].Term == LocalEntity.Term && FCSSQlastList.Count == 2)
            //        {
            //            lastOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(FCSSQlastList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //            IList<FCSSQInfo> FCSSQlastlastList = FCSSQService.ToListForTrend(FCSSQlastList[1].Term, null);//获取前一期开奖数据
            //            if (null != FCSSQlastlastList && FCSSQlastlastList[0].Term == FCSSQlastList[1].Term && FCSSQlastlastList.Count == 2)
            //            {
            //                lastlastOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(FCSSQlastlastList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //            }
            //        }
            //        IList<FCSSQInfo> FCSSQnextList = FCSSQService.ToListForNextTrend(LocalEntity.Term, null);
            //        if (null != FCSSQnextList && FCSSQnextList[0].Term == LocalEntity.Term && FCSSQnextList.Count == 2)//获取后一期开奖数据
            //        {
            //            nextOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(FCSSQnextList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //            IList<FCSSQInfo> FCSSQnextnextList = FCSSQService.ToListForNextTrend(FCSSQnextList[1].Term, null);//获取前一期开奖数据
            //            if (null != FCSSQnextnextList && FCSSQnextnextList[0].Term == FCSSQnextList[1].Term && FCSSQnextnextList.Count == 2)
            //            {
            //                nextnextOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(FCSSQnextnextList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //            }
            //        }
            //        if (css != null) //有样式
            //        {
            //            if (css.ChildList == null) //没有子样式 
            //            {
            //                if (isValue) //项值
            //                {
            //                    return string.Format(fomart, css.NumberCssName, attr, itemValue);
            //                }
            //                else //遗漏
            //                {
            //                    return string.Format(fomart, css.MissCssName, attr, itemValue);
            //                }
            //            }
            //            else //有子样式
            //            {
            //                foreach (var item in css.ChildList)
            //                {
            //                    for (int i = item.endNum; i >= item.startNum; i--)
            //                    {

            //                        if (isValue)
            //                        {
            //                            if (index == i - css.ChildList[0].startNum)
            //                            {

            //                                if (null != lastlastOpenCode && (lastlastOpenCode.Contains(itemValue.ToInt() + 2) || lastlastOpenCode.Contains(itemValue.ToInt() - 2)))
            //                                //根据上上期开奖号码判断当前是否为斜跳号
            //                                {
            //                                    return string.Format(fomart, item.Extend1, attr, itemValue);
            //                                }
            //                                if (null != nextnextOpenCode && (nextnextOpenCode.Contains(itemValue.ToInt() + 2) || nextnextOpenCode.Contains(itemValue.ToInt() - 2)))
            //                                //根据下下期开奖号码判断当前是否为斜跳号
            //                                {
            //                                    return string.Format(fomart, item.Extend1, attr, itemValue);
            //                                }
            //                                return string.Format(fomart, item.NumberCssName, attr, itemValue);
            //                            }
            //                        }
            //                        else
            //                        {
            //                            if (index == i - css.ChildList[0].startNum)
            //                            {
            //                                return string.Format(fomart, item.MissCssName, attr, itemValue);
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        break;
            //    case 12: //大乐透
            //        IList<TCDLTInfo> TCDLTlastList = TCDLTService.ToListForTrend(LocalEntity.Term, null); //获取前一期开奖数据
            //        if (null != TCDLTlastList && TCDLTlastList[0].Term == LocalEntity.Term && TCDLTlastList.Count == 2)
            //        {
            //            lastOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(TCDLTlastList[1] as TEntity,
            //                ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //            IList<TCDLTInfo> TCDLTlastlastList = TCDLTService.ToListForTrend(TCDLTlastList[1].Term, null);
            //            //获取前一期开奖数据
            //            if (null != TCDLTlastlastList && TCDLTlastlastList[0].Term == TCDLTlastList[1].Term &&
            //                TCDLTlastlastList.Count == 2)
            //            {
            //                lastlastOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(TCDLTlastlastList[1] as TEntity,
            //                    ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //            }
            //        }
            //        IList<TCDLTInfo> TCDLTnextList = TCDLTService.ToListForNextTrend(LocalEntity.Term, null);
            //        if (null != TCDLTnextList && TCDLTnextList[0].Term == LocalEntity.Term && TCDLTnextList.Count == 2)
            //        //获取后一期开奖数据
            //        {
            //            nextOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(TCDLTnextList[1] as TEntity,
            //                ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //            IList<TCDLTInfo> TCDLTnextnextList = TCDLTService.ToListForNextTrend(TCDLTnextList[1].Term, null);
            //            //获取前一期开奖数据
            //            if (null != TCDLTnextnextList && TCDLTnextnextList[0].Term == TCDLTnextList[1].Term &&
            //                TCDLTnextnextList.Count == 2)
            //            {
            //                nextnextOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(TCDLTnextnextList[1] as TEntity,
            //                    ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //            }
            //        }
            //        if (css != null) //有样式
            //        {
            //            if (css.ChildList == null) //没有子样式 
            //            {
            //                if (isValue) //项值
            //                {
            //                    return string.Format(fomart, css.NumberCssName, attr, itemValue);
            //                }
            //                else //遗漏
            //                {
            //                    return string.Format(fomart, css.MissCssName, attr, itemValue);
            //                }
            //            }
            //            else //有子样式
            //            {
            //                foreach (var item in css.ChildList)
            //                {
            //                    for (int i = item.endNum; i >= item.startNum; i--)
            //                    {

            //                        if (isValue)
            //                        {
            //                            if (index == i - css.ChildList[0].startNum)
            //                            {

            //                                if (null != lastlastOpenCode && (lastlastOpenCode.Contains(itemValue.ToInt() + 2) || lastlastOpenCode.Contains(itemValue.ToInt() - 2)))
            //                                //根据上上期开奖号码判断当前是否为斜跳号
            //                                {
            //                                    return string.Format(fomart, item.Extend1, attr, itemValue);
            //                                }
            //                                if (null != nextnextOpenCode && (nextnextOpenCode.Contains(itemValue.ToInt() + 2) || nextnextOpenCode.Contains(itemValue.ToInt() - 2)))
            //                                //根据下下期开奖号码判断当前是否为斜跳号
            //                                {
            //                                    return string.Format(fomart, item.Extend1, attr, itemValue);
            //                                }
            //                                return string.Format(fomart, item.NumberCssName, attr, itemValue);
            //                            }
            //                        }
            //                        else
            //                        {
            //                            if (index == i - css.ChildList[0].startNum)
            //                            {
            //                                return string.Format(fomart, item.MissCssName, attr, itemValue);
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        break;
            //}
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 多值多列竖三连号
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string MultiValue_ShuSanLianHaoNumber<TEntity>(TEntity LocalEntity, TrendChartItemInfo ItemConfig, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            //IList<int> list = LotteryUtils.GetOpenCodeList<TEntity>(LocalEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            ////上一期数据 ,下一期数据
            //IList<int> lastOpenCode = null, nextOpenCode = null, lastlastOpenCode = null, nextnextOpenCode = null; ;
            ////根据Cid判断_彩种
            //switch (ItemConfig.Cid)
            //{
            //    case 4:   //双色球
            //        IList<FCSSQInfo> FCSSQlastList = FCSSQService.ToListForTrend(LocalEntity.Term, null);//获取前一期开奖数据
            //        if (null != FCSSQlastList && FCSSQlastList[0].Term == LocalEntity.Term && FCSSQlastList.Count == 2)
            //        {
            //            lastOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(FCSSQlastList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //            IList<FCSSQInfo> FCSSQlastlastList = FCSSQService.ToListForTrend(FCSSQlastList[1].Term, null);//获取前一期开奖数据
            //            if (null != FCSSQlastlastList && FCSSQlastlastList[0].Term == FCSSQlastList[1].Term && FCSSQlastlastList.Count == 2)
            //            {
            //                lastlastOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(FCSSQlastlastList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //            }
            //        }
            //        IList<FCSSQInfo> FCSSQnextList = FCSSQService.ToListForNextTrend(LocalEntity.Term, null);
            //        if (null != FCSSQnextList && FCSSQnextList[0].Term == LocalEntity.Term && FCSSQnextList.Count == 2)//获取后一期开奖数据
            //        {
            //            nextOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(FCSSQnextList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //            IList<FCSSQInfo> FCSSQnextnextList = FCSSQService.ToListForNextTrend(FCSSQnextList[1].Term, null);//获取前一期开奖数据
            //            if (null != FCSSQnextnextList && FCSSQnextnextList[0].Term == FCSSQnextList[1].Term && FCSSQnextnextList.Count == 2)
            //            {
            //                nextnextOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(FCSSQnextnextList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //            }
            //        }
            //        if (css != null) //有样式
            //        {
            //            if (css.ChildList == null) //没有子样式 
            //            {
            //                if (isValue) //项值
            //                {
            //                    return string.Format(fomart, css.NumberCssName, attr, itemValue);
            //                }
            //                else //遗漏
            //                {
            //                    return string.Format(fomart, css.MissCssName, attr, itemValue);
            //                }
            //            }
            //            else //有子样式
            //            {
            //                foreach (var item in css.ChildList)
            //                {
            //                    for (int i = item.endNum; i >= item.startNum; i--)
            //                    {

            //                        if (isValue)
            //                        {
            //                            if (index == i - css.ChildList[0].startNum)
            //                            {
            //                                if (null != lastOpenCode && null != nextOpenCode &&
            //                                    lastOpenCode.Contains(itemValue.ToInt()) &&
            //                                     nextOpenCode.Contains(itemValue.ToInt())) //根据上下期开奖号码判断当前是否为竖三连号
            //                                {
            //                                    return string.Format(fomart, item.Extend1, attr, itemValue);
            //                                }
            //                                if (null != lastlastOpenCode && null != lastOpenCode && lastOpenCode.Contains(itemValue.ToInt()) && lastlastOpenCode.Contains(itemValue.ToInt())) //根据上期和上上期开奖号码判断当前是否为竖三连号
            //                                {
            //                                    return string.Format(fomart, item.Extend1, attr, itemValue);
            //                                }
            //                                if (null != nextnextOpenCode && null != nextOpenCode && nextOpenCode.Contains(itemValue.ToInt()) && nextnextOpenCode.Contains(itemValue.ToInt())) //根据下期和下下期开奖号码判断当前是否为竖三连号
            //                                {
            //                                    return string.Format(fomart, item.Extend1, attr, itemValue);
            //                                }
            //                                return string.Format(fomart, item.NumberCssName, attr, itemValue);
            //                            }
            //                        }
            //                        else
            //                        {
            //                            if (index == i - css.ChildList[0].startNum)
            //                            {
            //                                return string.Format(fomart, item.MissCssName, attr, itemValue);
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        break;
            //}
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }

        /// <summary>
        /// 多值多列竖跳号
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string MultiValue_ShuTiaoHaoNumber<TEntity>(TEntity LocalEntity, TrendChartItemInfo ItemConfig, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            //IList<int> list = LotteryUtils.GetOpenCodeList<TEntity>(LocalEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            ////上一期数据 ,下一期数据
            //IList<int> lastOpenCode = null, nextOpenCode = null, lastlastOpenCode = null, nextnextOpenCode = null; ;
            ////根据Cid判断_彩种
            //switch (ItemConfig.Cid)
            //{
            //    case 12:   //大乐透
            //        IList<TCDLTInfo> TCDLTlastList = TCDLTService.ToListForTrend(LocalEntity.Term, null);//获取前一期开奖数据
            //        if (null != TCDLTlastList && TCDLTlastList[0].Term == LocalEntity.Term && TCDLTlastList.Count == 2)
            //        {
            //            lastOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(TCDLTlastList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //            IList<TCDLTInfo> TCDLTlastlastList = TCDLTService.ToListForTrend(TCDLTlastList[1].Term, null);//获取前一期开奖数据
            //            if (null != TCDLTlastlastList && TCDLTlastlastList[0].Term == TCDLTlastList[1].Term && TCDLTlastlastList.Count == 2)
            //            {
            //                lastlastOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(TCDLTlastlastList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //            }
            //        }
            //        IList<TCDLTInfo> TCDLTnextList = TCDLTService.ToListForNextTrend(LocalEntity.Term, null);
            //        if (null != TCDLTnextList && TCDLTnextList[0].Term == LocalEntity.Term && TCDLTnextList.Count == 2)//获取后一期开奖数据
            //        {
            //            nextOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(TCDLTnextList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //            IList<TCDLTInfo> TCDLTnextnextList = TCDLTService.ToListForNextTrend(TCDLTnextList[1].Term, null);//获取前一期开奖数据
            //            if (null != TCDLTnextnextList && TCDLTnextnextList[0].Term == TCDLTnextList[1].Term && TCDLTnextnextList.Count == 2)
            //            {
            //                nextnextOpenCode = LotteryUtils.GetOpenCodeList<TEntity>(TCDLTnextnextList[1] as TEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //            }
            //        }
            //        if (css != null) //有样式
            //        {
            //            if (css.ChildList == null) //没有子样式 
            //            {
            //                if (isValue) //项值
            //                {
            //                    return string.Format(fomart, css.NumberCssName, attr, itemValue);
            //                }
            //                else //遗漏
            //                {
            //                    return string.Format(fomart, css.MissCssName, attr, itemValue);
            //                }
            //            }
            //            else //有子样式
            //            {
            //                foreach (var item in css.ChildList)
            //                {
            //                    for (int i = item.endNum; i >= item.startNum; i--)
            //                    {

            //                        if (isValue)
            //                        {
            //                            if (index == i - css.ChildList[0].startNum)
            //                            {

            //                                if (null != lastlastOpenCode && lastlastOpenCode.Contains(itemValue.ToInt())) //根据上上期开奖号码判断当前是否为竖跳号
            //                                {
            //                                    return string.Format(fomart, item.Extend1, attr, itemValue);
            //                                }
            //                                if (null != nextnextOpenCode && nextnextOpenCode.Contains(itemValue.ToInt())) //根据下下期开奖号码判断当前是否为竖跳号
            //                                {
            //                                    return string.Format(fomart, item.Extend1, attr, itemValue);
            //                                }
            //                                return string.Format(fomart, item.NumberCssName, attr, itemValue);
            //                            }
            //                        }
            //                        else
            //                        {
            //                            if (index == i - css.ChildList[0].startNum)
            //                            {
            //                                return string.Format(fomart, item.MissCssName, attr, itemValue);
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        break;
            //}
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        #endregion

        #region 特殊

        /// <summary>
        /// 福彩3D 012路走势图4
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="LocalEntity"></param>
        /// <param name="ItemConfig"></param>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SpecialValue_FC3D012_4<TEntity>(TEntity LocalEntity, TrendChartItemInfo ItemConfig, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            StringBuilder table = new StringBuilder(1000);
            IList<int> list = LotteryUtils.GetOpenCodeList<TEntity>(LocalEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            table.Append("<table  id=\"zstable\" class=\"zstable\">");
            table.Append("<tbody>");
            table.Append("<tr>");
            for (int i = 0; i < 10; i = i + 3)
            {
                if (list.Count(s => s == i) == 3)
                {
                    table.Append(string.Format("<td width=\"48px\"  class=\"{0}\">{1}", css.Extend2, i));
                    table.Append("</td>");
                }
                if (list.Count(s => s == i) == 2)
                {
                    table.Append(string.Format("<td width=\"48px\"  class=\"{0}\">{1}", css.Extend1, i));
                    table.Append("</td>");
                }
                if (list.Count(s => s == i) == 1)
                {
                    table.Append(string.Format("<td width=\"48px\"  class=\"{0}\">{1}", css.NumberCssName, i));
                    table.Append("</td>");
                }
                if (list.Count(s => s == i) == 0)
                {
                    table.Append(string.Format("<td width=\"48px\"  class=\"{0}\">{1}", css.MissCssName, i));
                    table.Append("</td>");
                }
            }
            table.Append("</tr>");
            table.Append("<tr>");
            for (int i = 1; i < 10; i = i + 3)
            {
                if (list.Count(s => s == i) == 3)
                {
                    table.Append(string.Format("<td width=\"48px\"  class=\"{0}\">{1}", css.Extend2, i));
                    table.Append("</td>");
                }
                if (list.Count(s => s == i) == 2)
                {
                    table.Append(string.Format("<td width=\"48px\"  class=\"{0}\">{1}", css.Extend1, i));
                    table.Append("</td>");
                }
                if (list.Count(s => s == i) == 1)
                {
                    table.Append(string.Format("<td width=\"48px\"  class=\"{0}\">{1}", css.NumberCssName, i));
                    table.Append("</td>");
                }
                if (list.Count(s => s == i) == 0)
                {
                    table.Append(string.Format("<td width=\"48px\"  class=\"{0}\">{1}", css.MissCssName, i));
                    table.Append("</td>");
                }
            }
            table.Append(string.Format("<td width=\"48px\"  class=\"{0}\">{1}", css.MissCssName, ""));
            table.Append("</td>");
            table.Append("</tr>");
            table.Append("<tr>");
            for (int i = 2; i < 10; i = i + 3)
            {
                if (list.Count(s => s == i) == 3)
                {
                    table.Append(string.Format("<td width=\"48px\"  class=\"{0}\">{1}", css.Extend2, i));
                    table.Append("</td>");
                }
                if (list.Count(s => s == i) == 2)
                {
                    table.Append(string.Format("<td width=\"48px\"  class=\"{0}\">{1}", css.Extend1, i));
                    table.Append("</td>");
                }
                if (list.Count(s => s == i) == 1)
                {
                    table.Append(string.Format("<td width=\"48px\"  class=\"{0}\">{1}", css.NumberCssName, i));
                    table.Append("</td>");
                }
                if (list.Count(s => s == i) == 0)
                {
                    table.Append(string.Format("<td width=\"48px\"  class=\"{0}\">{1}", css.MissCssName, i));
                    table.Append("</td>");
                }
            }
            table.Append(string.Format("<td width=\"48px\"  class=\"{0}\">{1}", css.MissCssName, ""));
            table.Append("</td>");
            table.Append("</tr>");
            table.Append("<tr>");
            table.Append(string.Format("<td   colspan=\"4\" class=\"{0}\">{1}", css.MissCssName, LocalEntity.Term.ToString() + " 期"));
            table.Append("</td>");
            table.Append("</tr>");
            table.Append("</tbody>");
            table.Append("</table>");

            return string.Format(fomart, "", attr, table.ToString());
        }
        /// <summary>
        /// 体彩P3 012路走势图4
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="LocalEntity"></param>
        /// <param name="ItemConfig"></param>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SpecialValue_TCP3012_4<TEntity>(TEntity LocalEntity, TrendChartItemInfo ItemConfig, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            StringBuilder table = new StringBuilder(1000);
            IList<int> list = LotteryUtils.GetOpenCodeList<TEntity>(LocalEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            table.Append("<table  id=\"zstable\" class=\"zstable\">");
            table.Append("<tbody>");
            table.Append("<tr>");
            for (int i = 0; i < 10; i = i + 3)
            {
                if (list.Count(s => s == i) == 3)
                {
                    table.Append(string.Format("<td width=\"48px\" class=\"{0}\">{1}", css.Extend2, i));
                    table.Append("</td>");
                }
                if (list.Count(s => s == i) == 2)
                {
                    table.Append(string.Format("<td width=\"48px\" class=\"{0}\">{1}", css.Extend1, i));
                    table.Append("</td>");
                }
                if (list.Count(s => s == i) == 1)
                {
                    table.Append(string.Format("<td width=\"48px\" class=\"{0}\">{1}", css.NumberCssName, i));
                    table.Append("</td>");
                }
                if (list.Count(s => s == i) == 0)
                {
                    table.Append(string.Format("<td width=\"48px\" class=\"{0}\">{1}", css.MissCssName, i));
                    table.Append("</td>");
                }
            }
            table.Append("</tr>");
            table.Append("<tr>");
            for (int i = 1; i < 10; i = i + 3)
            {
                if (list.Count(s => s == i) == 3)
                {
                    table.Append(string.Format("<td width=\"48px\" class=\"{0}\">{1}", css.Extend2, i));
                    table.Append("</td>");
                }
                if (list.Count(s => s == i) == 2)
                {
                    table.Append(string.Format("<td width=\"48px\" class=\"{0}\">{1}", css.Extend1, i));
                    table.Append("</td>");
                }
                if (list.Count(s => s == i) == 1)
                {
                    table.Append(string.Format("<td width=\"48px\" class=\"{0}\">{1}", css.NumberCssName, i));
                    table.Append("</td>");
                }
                if (list.Count(s => s == i) == 0)
                {
                    table.Append(string.Format("<td width=\"48px\" class=\"{0}\">{1}", css.MissCssName, i));
                    table.Append("</td>");
                }
            }
            table.Append(string.Format("<td width=\"48px\" class=\"{0}\">{1}", css.MissCssName, ""));
            table.Append("</td>");
            table.Append("</tr>");
            table.Append("<tr>");
            for (int i = 2; i < 10; i = i + 3)
            {
                if (list.Count(s => s == i) == 3)
                {
                    table.Append(string.Format("<td width=\"48px\" class=\"{0}\">{1}", css.Extend2, i));
                    table.Append("</td>");
                }
                if (list.Count(s => s == i) == 2)
                {
                    table.Append(string.Format("<td width=\"48px\" class=\"{0}\">{1}", css.Extend1, i));
                    table.Append("</td>");
                }
                if (list.Count(s => s == i) == 1)
                {
                    table.Append(string.Format("<td width=\"48px\" class=\"{0}\">{1}", css.NumberCssName, i));
                    table.Append("</td>");
                }
                if (list.Count(s => s == i) == 0)
                {
                    table.Append(string.Format("<td width=\"48px\" class=\"{0}\">{1}", css.MissCssName, i));
                    table.Append("</td>");
                }
            }
            table.Append(string.Format("<td width=\"48px\" class=\"{0}\">{1}", css.MissCssName, ""));
            table.Append("</td>");
            table.Append("</tr>");
            table.Append("<tr>");
            table.Append(string.Format("<td  colspan=\"4\" class=\"{0}\">{1}", css.MissCssName, LocalEntity.Term.ToString() + " 期"));
            table.Append("</td>");
            table.Append("</tr>");
            table.Append("</tbody>");
            table.Append("</table>");

            return string.Format(fomart, "", attr, table.ToString());
        }
        /// <summary>
        /// 福彩 双色球出号频率
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="LocalEntity"></param>
        /// <param name="ItemConfig"></param>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SpecialValue_FCSSQ_ChuHaoPL<TEntity>(TEntity LocalEntity, TrendChartItemInfo ItemConfig, ChartCssConfigInfo css, int[] ItemIndex, string fomart) where TEntity : LotteryOpenCode
        {
            StringBuilder sp = new StringBuilder();

            if (ItemConfig.ItemCount == 33)     //红球
            {
                sp.Append("<tr>");
                sp.Append("<td class=\"white pl\">150</td>");
                for (int i = 0; i < ItemConfig.ItemCount; i++)
                {
                    sp.Append(string.Format("<td rowspan=\"16\" style=\"vertical-align: bottom;\" class=\"{0} \">", GetCssName(true, css, i)));
                    sp.Append(string.Format("<span>{0}</span><br>", ItemConfig.ItemString[i]));
                    sp.Append(string.Format("<img src=\"http://www.55128.cn/images/zst/redline.gif\"  style=\"vertical-align: bottom;\" width=\"8\" height=\"{0}\">", (3 * ItemIndex[i]).ToString()));
                    sp.Append("</td>");
                }
                sp.Append("@Row1");
                sp.Append("<tr><td class=\"pl\">140</td></tr><tr><td class=\"pl\">130</td></tr><tr><td class=\"pl\">120</td></tr><tr><td class=\"pl\">110</td></tr><tr><td class=\"pl\">100</td></tr><tr><td class=\"pl\">90</td></tr><tr><td class=\"pl\">80</td></tr><tr><td class=\"pl\">70</td></tr><tr><td class=\"pl\">60</td></tr><tr><td class=\"pl\">50</td></tr><tr><td class=\"pl\">40</td></tr><tr><td class=\"pl\">30</td></tr><tr><td class=\"pl\">20</td></tr><tr><td class=\"pl\">10</td></tr><tr><td class=\"pl\">0</td></tr>");

                sp.Append("<tr>");
                sp.Append("<td class=\"white\">出现次数</td>");
                for (int i = 0; i < ItemConfig.ItemCount; i++)
                {
                    sp.Append(GetCssValue(true, fomart, "", css, ItemIndex[i].ToString(), i));
                }
                sp.Append("@Row2");
                sp.Append("<tr>");
                sp.Append("<td class=\"white\">出现频率(%)</td>");
                for (int i = 0; i < ItemConfig.ItemCount; i++)
                {
                    sp.Append(GetCssValue(true, fomart, "", css, string.Format("{0:F1}", (double)ItemIndex[i] * 100 / ItemIndex.Sum()), i));
                }
                sp.Append("@Row3");
            }
            if (ItemConfig.ItemCount == 16)     //蓝球
            {
                for (int i = 0; i < ItemConfig.ItemCount; i++)
                {
                    sp.Append(string.Format("<td rowspan=\"16\"  style=\"vertical-align: bottom;\" class=\"{0}\">", GetCssName(true, css, i)));
                    sp.Append(string.Format("<span>{0}</span><br>", ItemConfig.ItemString[i]));
                    sp.Append(string.Format("<img src=\"http://www.55128.cn/images/zst/blueline.gif\"  style=\"vertical-align: bottom;\" width=\"8\" height=\"{0}\">", (3 * ItemIndex[i]).ToString()));
                    sp.Append("</td>");
                }
                sp.Append("</tr>");
                sp.Append("~");
                for (int i = 0; i < ItemConfig.ItemCount; i++)
                {
                    sp.Append(GetCssValue(true, fomart, "", css, ItemIndex[i].ToString(), i));
                }
                sp.Append("</tr>");
                sp.Append("~");
                for (int i = 0; i < ItemConfig.ItemCount; i++)
                {
                    sp.Append(GetCssValue(true, fomart, "", css, string.Format("{0:F1}", (double)ItemIndex[i] * 100 / ItemIndex.Sum()), i));
                }
                sp.Append("</tr>");
            }
            return sp.ToString();
        }

        /// <summary>
        /// 体彩 大乐透出号频率
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="LocalEntity"></param>
        /// <param name="ItemConfig"></param>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SpecialValue_TCDLT_ChuHaoPL<TEntity>(TEntity LocalEntity, TrendChartItemInfo ItemConfig, ChartCssConfigInfo css, int[] ItemIndex, string fomart) where TEntity : LotteryOpenCode
        {
            StringBuilder sp = new StringBuilder();

            if (ItemConfig.ItemCount == 35)     //红球
            {
                sp.Append("<tr>");
                sp.Append("<td class=\"white pl\">150</td>");
                for (int i = 0; i < ItemConfig.ItemCount; i++)
                {
                    sp.Append(string.Format("<td rowspan=\"16\" style=\"vertical-align: bottom;\" class=\"{0} \">", GetCssName(true, css, i)));
                    sp.Append(string.Format("<span>{0}</span><br>", ItemConfig.ItemString[i]));
                    sp.Append(string.Format("<img src=\"http://www.55128.cn/images/zst/redline.gif\"  style=\"vertical-align: bottom;\" width=\"8\" height=\"{0}\">", (3 * ItemIndex[i]).ToString()));
                    sp.Append("</td>");
                }
                sp.Append("@Row1");
                sp.Append("<tr><td class=\"pl\">140</td></tr><tr><td class=\"pl\">130</td></tr><tr><td class=\"pl\">120</td></tr><tr><td class=\"pl\">110</td></tr><tr><td class=\"pl\">100</td></tr><tr><td class=\"pl\">90</td></tr><tr><td class=\"pl\">80</td></tr><tr><td class=\"pl\">70</td></tr><tr><td class=\"pl\">60</td></tr><tr><td class=\"pl\">50</td></tr><tr><td class=\"pl\">40</td></tr><tr><td class=\"pl\">30</td></tr><tr><td class=\"pl\">20</td></tr><tr><td class=\"pl\">10</td></tr><tr><td class=\"pl\">0</td></tr>");

                sp.Append("<tr>");
                sp.Append("<td class=\"white\">出现次数</td>");
                for (int i = 0; i < ItemConfig.ItemCount; i++)
                {
                    sp.Append(GetCssValue(true, fomart, "", css, ItemIndex[i].ToString(), i));
                }
                sp.Append("@Row2");
                sp.Append("<tr>");
                sp.Append("<td class=\"white\">出现频率(%)</td>");
                for (int i = 0; i < ItemConfig.ItemCount; i++)
                {
                    sp.Append(GetCssValue(true, fomart, "", css, string.Format("{0:F1}", (double)ItemIndex[i] * 100 / ItemIndex.Sum()), i));
                }
                sp.Append("@Row3");
            }
            if (ItemConfig.ItemCount == 12)     //蓝球
            {
                for (int i = 0; i < ItemConfig.ItemCount; i++)
                {
                    sp.Append(string.Format("<td rowspan=\"16\"  style=\"vertical-align: bottom;\" class=\"{0}\">", GetCssName(true, css, i)));
                    sp.Append(string.Format("<span>{0}</span><br>", ItemConfig.ItemString[i]));
                    sp.Append(string.Format("<img src=\"http://www.55128.cn/images/zst/blueline.gif\"  style=\"vertical-align: bottom;\" width=\"8\" height=\"{0}\">", (3 * ItemIndex[i]).ToString()));
                    sp.Append("</td>");
                }
                sp.Append("</tr>");
                sp.Append("~");
                for (int i = 0; i < ItemConfig.ItemCount; i++)
                {
                    sp.Append(GetCssValue(true, fomart, "", css, ItemIndex[i].ToString(), i));
                }
                sp.Append("</tr>");
                sp.Append("~");
                for (int i = 0; i < ItemConfig.ItemCount; i++)
                {
                    sp.Append(GetCssValue(true, fomart, "", css, string.Format("{0:F1}", (double)ItemIndex[i] * 100 / ItemIndex.Sum()), i));
                }
                sp.Append("</tr>");
            }
            return sp.ToString();
        }


        #endregion

        #region 高频
        /// <summary>
        /// 多值多列快乐12开奖号码分布
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string MultiValue_KL12<TEntity>(TEntity LocalEntity, TrendChartItemInfo ItemConfig, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            IList<int> list = LotteryUtils.GetOpenCodeList<TEntity>(LocalEntity, ItemConfig.IndexStart, ItemConfig.IndexEnd);
            //根据Cid判断_彩种
            switch (ItemConfig.Cid)
            {
                case 5000:   //快乐12

                    if (list[0].ToString() == itemValue)
                    {
                        return GetCssValueExtend1(isValue, fomart, attr, css, itemValue, index);
                    }
                    else
                    {
                        return GetCssValue(isValue, fomart, attr, css, itemValue, index);
                    }

            }
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        #endregion


        /// <summary>
        /// 获取应该调用哪个CSS名称
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetCssName(bool isValue, ChartCssConfigInfo css, int index)
        {
            if (css != null) //有样式
            {
                if (css.ChildList == null)//没有子样式 
                {
                    if (isValue) //项值
                    {
                        return css.NumberCssName;
                    }
                    else //遗漏
                    {
                        return css.MissCssName;
                    }
                }
                else //有子样式
                {
                    foreach (var item in css.ChildList)
                    {
                        for (int i = item.endNum; i >= item.startNum; i--)
                        {

                            if (isValue)
                            {
                                if (index == i - css.ChildList[0].startNum)
                                {
                                    return item.NumberCssName;
                                }
                            }
                            else
                            {
                                if (index == i - css.ChildList[0].startNum)
                                {
                                    return item.MissCssName;
                                }
                            }
                        }
                    }
                }
            }
            return ""; //没有样式(填充默认颜色)
        }
        /// <summary>
        /// 设置样式通用方法
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetCssValue(bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index)
        {
            if (css != null) //有样式
            {
                if (css.ChildList == null)//没有子样式 
                {
                    if (isValue) //项值
                    {
                        return string.Format(fomart, css.NumberCssName, attr, itemValue);
                    }
                    else //遗漏
                    {
                        return string.Format(fomart, css.MissCssName, attr, itemValue);
                    }
                }
                else //有子样式
                {
                    foreach (var item in css.ChildList)
                    {
                        for (int i = item.endNum; i >= item.startNum; i--)
                        {

                            if (isValue)
                            {
                                if (index == i - css.ChildList[0].startNum)
                                {
                                    return string.Format(fomart, item.NumberCssName, attr, itemValue);
                                }
                            }
                            else
                            {
                                if (index == i - css.ChildList[0].startNum)
                                {
                                    return string.Format(fomart, item.MissCssName, attr, itemValue);
                                }
                            }
                        }
                    }
                }
            }
            return string.Format(fomart, "", attr, itemValue); //没有样式(填充默认颜色)
        }
        /// <summary>
        /// 设置样式通用方法(使用扩展样式1)
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static string GetCssValueExtend1(bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index)
        {
            if (css != null) //有样式
            {
                if (css.ChildList == null)//没有子样式 
                {
                    if (isValue) //项值
                    {
                        return string.Format(fomart, css.Extend1, attr, itemValue);
                    }
                    else //遗漏
                    {
                        return string.Format(fomart, css.MissCssName, attr, itemValue);
                    }
                }
                else //有子样式
                {
                    foreach (var item in css.ChildList)
                    {
                        for (int i = item.endNum; i >= item.startNum; i--)
                        {

                            if (isValue)
                            {
                                if (index == i - css.ChildList[0].startNum)
                                {
                                    return string.Format(fomart, item.Extend1, attr, itemValue);
                                }
                            }
                            else //遗漏
                            {
                                if (index == i - css.ChildList[0].startNum)
                                {
                                    return string.Format(fomart, item.MissCssName, attr, itemValue);
                                }
                            }
                        }
                    }
                }
            }
            return string.Format(fomart, "", attr, itemValue); //没有样式(填充默认颜色)
        }
        /// <summary>
        /// 设置样式通用方法(使用扩展样式2)
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static string GetCssValueExtend2(bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index)
        {
            if (css != null) //有样式
            {
                if (css.ChildList == null)//没有子样式 
                {
                    if (isValue) //项值
                    {
                        return string.Format(fomart, css.Extend2, attr, itemValue);
                    }
                    else //遗漏
                    {
                        return string.Format(fomart, css.MissCssName, attr, itemValue);
                    }
                }
                else //有子样式
                {
                    foreach (var item in css.ChildList)
                    {
                        for (int i = item.endNum; i >= item.startNum; i--)
                        {

                            if (isValue)
                            {
                                if (index == i - css.ChildList[0].startNum)
                                {
                                    return string.Format(fomart, item.Extend2, attr, itemValue);
                                }
                            }
                            else //遗漏
                            {
                                if (index == i - css.ChildList[0].startNum)
                                {
                                    return string.Format(fomart, item.MissCssName, attr, itemValue);
                                }
                            }
                        }
                    }
                }
            }
            return string.Format(fomart, "", attr, itemValue); //没有样式(填充默认颜色)
        }
        /// <summary>
        /// 设置样式通用方法(使用扩展样式3)
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static string GetCssValueExtend3(bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index)
        {
            if (css != null) //有样式
            {
                if (css.ChildList == null)//没有子样式 
                {
                    if (isValue) //项值
                    {
                        return string.Format(fomart, css.Extend3, attr, itemValue);
                    }
                    else //遗漏
                    {
                        return string.Format(fomart, css.MissCssName, attr, itemValue);
                    }
                }
                else //有子样式
                {
                    foreach (var item in css.ChildList)
                    {
                        for (int i = item.endNum; i >= item.startNum; i--)
                        {

                            if (isValue)
                            {
                                if (index == i - css.ChildList[0].startNum)
                                {
                                    return string.Format(fomart, item.Extend3, attr, itemValue);
                                }
                            }
                            else //遗漏
                            {
                                if (index == i - css.ChildList[0].startNum)
                                {
                                    return string.Format(fomart, item.MissCssName, attr, itemValue);
                                }
                            }
                        }
                    }
                }
            }
            return string.Format(fomart, "", attr, itemValue); //没有样式(填充默认颜色)
        }
        /// <summary>
        /// 设置样式通用方法(使用扩展样式2)
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static string GetCssValueExtend4(bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index)
        {
            if (css != null) //有样式
            {
                if (css.ChildList == null)//没有子样式 
                {
                    if (isValue) //项值
                    {
                        return string.Format(fomart, css.Extend4, attr, itemValue);
                    }
                    else //遗漏
                    {
                        return string.Format(fomart, css.MissCssName, attr, itemValue);
                    }
                }
                else //有子样式
                {
                    foreach (var item in css.ChildList)
                    {
                        for (int i = item.endNum; i >= item.startNum; i--)
                        {

                            if (isValue)
                            {
                                if (index == i - css.ChildList[0].startNum)
                                {
                                    return string.Format(fomart, item.Extend4, attr, itemValue);
                                }
                            }
                            else //遗漏
                            {
                                if (index == i - css.ChildList[0].startNum)
                                {
                                    return string.Format(fomart, item.MissCssName, attr, itemValue);
                                }
                            }
                        }
                    }
                }
            }
            return string.Format(fomart, "", attr, itemValue); //没有样式(填充默认颜色)
        }
        /// <summary>
        /// 设置样式通用方法(使用扩展样式2)
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static string GetCssValueExtend5(bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index)
        {
            if (css != null) //有样式
            {
                if (css.ChildList == null)//没有子样式 
                {
                    if (isValue) //项值
                    {
                        return string.Format(fomart, css.Extend5, attr, itemValue);
                    }
                    else //遗漏
                    {
                        return string.Format(fomart, css.MissCssName, attr, itemValue);
                    }
                }
                else //有子样式
                {
                    foreach (var item in css.ChildList)
                    {
                        for (int i = item.endNum; i >= item.startNum; i--)
                        {

                            if (isValue)
                            {
                                if (index == i - css.ChildList[0].startNum)
                                {
                                    return string.Format(fomart, item.Extend5, attr, itemValue);
                                }
                            }
                            else //遗漏
                            {
                                if (index == i - css.ChildList[0].startNum)
                                {
                                    return string.Format(fomart, item.MissCssName, attr, itemValue);
                                }
                            }
                        }
                    }
                }
            }
            return string.Format(fomart, "", attr, itemValue); //没有样式(填充默认颜色)
        }
        /// <summary>
        /// 回摆
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="LocalEntity"></param>
        /// <param name="ItemConfig"></param>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_HB<TEntity>(TEntity LocalEntity, TrendChartItemInfo ItemConfig, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 振幅
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="chartCssConfigInfo"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_ZF(bool isValue, string fomart, string attr, ChartCssConfigInfo chartCssConfigInfo, string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, chartCssConfigInfo, itemValue, index);
        }
        /// <summary>
        /// 福建31选7三区比
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="LocalEntity"></param>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_FJ31X7SanQu<TEntity>(TEntity LocalEntity, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 福建31选7三区比
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="LocalEntity"></param>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_FJ36X7SanQu<TEntity>(TEntity LocalEntity, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 和尾大小
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="LocalEntity"></param>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_HeWeiDx<TEntity>(TEntity LocalEntity, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 生肖
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="LocalEntity"></param>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_ShengXiao(bool isValue, string fomart, string attr, ChartCssConfigInfo chartCssConfigInfo, string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, chartCssConfigInfo, itemValue, index);
        }
        /// <summary>
        /// 华东15选5三区比
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="LocalEntity"></param>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_HD15X5SanQu<TEntity>(TEntity LocalEntity, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 华东15选5一区个数
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="chartCssConfigInfo"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_Hd11x5Yq(bool isValue, string fomart, string attr, ChartCssConfigInfo chartCssConfigInfo, string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, chartCssConfigInfo, itemValue, index);
        }
        /// <summary>
        /// 华东15选5二区个数
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="chartCssConfigInfo"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_Hd11x5Eq(bool isValue, string fomart, string attr, ChartCssConfigInfo chartCssConfigInfo, string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, chartCssConfigInfo, itemValue, index);
        }
        /// <summary>
        /// 华东15选5三区个数
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="chartCssConfigInfo"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleValue_Hd11x5Sq(bool isValue, string fomart, string attr, ChartCssConfigInfo chartCssConfigInfo, string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, chartCssConfigInfo, itemValue, index);
        }
        /// <summary>
        /// 南粤36选7三区比
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="LocalEntity"></param>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_NY36x7SanQu<TEntity>(TEntity LocalEntity, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 和值012路
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="chartCssConfigInfo"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_Hz012(bool isValue, string fomart, string attr, ChartCssConfigInfo chartCssConfigInfo, string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, chartCssConfigInfo, itemValue, index);
        }
        /// <summary>
        /// 快3三不同走势
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="chartCssConfigInfo"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_K3sbt(bool isValue, string fomart, string attr, ChartCssConfigInfo chartCssConfigInfo, string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, chartCssConfigInfo, itemValue, index);
        }
        /// <summary>
        /// 快三二不同走势
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="LocalEntity"></param>
        /// <param name="ItemConfig"></param>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="css"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SetK3ebtItemValue<TEntity>(TEntity LocalEntity, TrendChartItemInfo ItemConfig, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            return GetCssValueExtend1(isValue, fomart, attr, css, itemValue, index);
        }
        /// <summary>
        /// 快3二不同走势
        /// </summary>
        /// <param name="isValue"></param>
        /// <param name="fomart"></param>
        /// <param name="attr"></param>
        /// <param name="chartCssConfigInfo"></param>
        /// <param name="itemValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string SingleCell_K3ebt(bool isValue, string fomart, string attr, ChartCssConfigInfo chartCssConfigInfo, string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, chartCssConfigInfo, itemValue, index);
        }

        public static string SingleValue_JoValue(bool isValue, string fomart, string attr, ChartCssConfigInfo chartCssConfigInfo, string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, chartCssConfigInfo, itemValue, index);
        }

        public static string SingleValue_DxValue(bool isValue, string fomart, string attr, ChartCssConfigInfo chartCssConfigInfo, string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, chartCssConfigInfo, itemValue, index);
        }

        public static string SingleValue_ZhValue(bool isValue, string fomart, string attr, ChartCssConfigInfo chartCssConfigInfo, string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, chartCssConfigInfo, itemValue, index);
        }

        public static string SetSbtxtItemValue<TEntity>(TEntity LocalEntity, TrendChartItemInfo ItemConfig, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }

        public static string SetEbtxtItemValue<TEntity>(TEntity LocalEntity, TrendChartItemInfo ItemConfig, bool isValue, string fomart, string attr, ChartCssConfigInfo css, string itemValue, int index) where TEntity : LotteryOpenCode
        {
            return GetCssValue(isValue, fomart, attr, css, itemValue, index);
        }

        public static string SingleValue_DxJoValue(bool isValue, string fomart, string attr, ChartCssConfigInfo chartCssConfigInfo, string itemValue, int index)
        {
            return GetCssValue(isValue, fomart, attr, chartCssConfigInfo, itemValue, index);
        }
    }
}
