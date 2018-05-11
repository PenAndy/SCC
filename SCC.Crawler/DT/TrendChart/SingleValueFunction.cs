using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SCC.Models;

namespace SCC.Crawler.DT
{
    /// <summary>
    /// 获取单值项值类
    /// </summary>
    public class SingleValueFunction
    {
        /// <summary>
        /// [单值单列]开奖号码展示项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>单列多个或全部开奖号码展示项</returns>
        [ChartFunction("[单值单列]开奖号码展示项", ChartItemType.SingleCell_OpenCodeItem, ChartItemClassName.SingleValue)]
        public static string GetOpenCodeItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            string num = "";
            foreach (int item in list)
            {
                num = num + item.ToString();
            }
            return num;
        }
        /// <summary>
        /// [单值单列]开奖号和值项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>单列和值项</returns>
        [ChartFunction("[单值单列]开奖号和值项", ChartItemType.SingleCell_SumItem, ChartItemClassName.SingleValue)]
        public static string GetSumItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            return LotteryUtils.GetSum(list).ToString();
        }
        /// <summary>
        /// [单值单列]开奖号和值尾数项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>多个号码和值尾数项</returns>
        [ChartFunction("[单值单列]开奖号和值尾数项", ChartItemType.SingleCell_HeWeiItem, ChartItemClassName.SingleValue)]
        public static string GetHeWeiItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            return (LotteryUtils.GetSum(list) % 10).ToString();
        }
        /// <summary>
        /// [单值单列]开奖号012比例项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>单列012比例项</returns>
        [ChartFunction("[单值单列]开奖号012比例项", ChartItemType.SingleCell_ProportionOf012Item, ChartItemClassName.SingleValue)]
        public static string GetProportionOf012ItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            return LotteryUtils.GetProportionOf012(list);
        }
        /// <summary>
        /// 质合项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>质合项</returns> 
        [ChartFunction("[单值单列]开奖号质合项", ChartItemType.SingleCell_ZhiHeStatusItem, ChartItemClassName.SingleValue)]
        public static string GetZhiHeItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            var tag = "";
            foreach (var item in list)
            {
                tag = tag + (LotteryUtils.IsPrimeNumbers(item) ? "质" : "合");
            }
            return tag;
        }
        /// <summary>
        /// [单值单列]开奖号大小比例项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <param name="splitNumber">大小分隔值</param>
        /// <returns>单列大小比例项</returns>
        [ChartFunction("[单值单列]开奖号大小比例项", ChartItemType.SingleCell_ProportionOfDxItem, ChartItemClassName.SingleValue)]
        public static string GetProportionOfDxItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count, int splitNumber) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            return LotteryUtils.GetProportionOfDX(list, splitNumber);
        }
        /// <summary>
        /// [单值单列]开奖号奇偶比列项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>单列奇偶比列项</returns>
        [ChartFunction("[单值单列]开奖号奇偶比列项", ChartItemType.SingleCell_ProportionOfJoItem, ChartItemClassName.SingleValue)]
        public static string GetProportionOfJoItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            return LotteryUtils.GetProportionOfJO(list);
        }
        /// <summary>
        /// [单值单列]开奖号质合比列项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>单列质合比列项</returns>
        [ChartFunction("[单值单列]开奖号质合比列项", ChartItemType.SingleCell_ProportionOfZhItem, ChartItemClassName.SingleValue)]
        public static string GetProportionOfZhItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            return LotteryUtils.GetProportionOfZh(list);
        }
        /// <summary>
        /// [单值单列]开奖号跨度项(大于两个号码)
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>全部号码跨度或两个号码以上(不包括两个号码)跨度</returns>
        [ChartFunction("[单值单列]开奖号跨度项(大于两个号码)", ChartItemType.SingleCell_SpanItem, ChartItemClassName.SingleValue)]
        public static string GetSpanItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            return LotteryUtils.GetSpan(list).ToString();
        }
        /// <summary>
        /// [单值单列]开奖号组三跨度项(大于两个号码)
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>全部号码跨度或两个号码以上(不包括两个号码)跨度</returns>
        [ChartFunction("[单值单列]开奖号组三跨度项(大于两个号码)", ChartItemType.SingleCell_ZSSpanItem, ChartItemClassName.SingleValue)]
        public static string GetZSSpanItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            return LotteryUtils.GetSpan(list).ToString();
        }
        /// <summary>
        /// [单值单列]开奖号AC值计算出012路值
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>根据AC值计算出012值</returns>
        [ChartFunction("[单值单列]开奖号AC值计算出012路值", ChartItemType.SingleCell_Ac012Lu, ChartItemClassName.SingleValue)]
        public static string GetAc012Lu<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {

            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            string[] kjh = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                kjh[i] = list[i].ToString();
            }
            string ac = LotteryUtils.GetAC(kjh).ToString();

            return (int.Parse(ac) % 3).ToString();
        }
        /// <summary>
        /// [单值单列]开奖号AC值计算出质合值
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>质、合</returns>
        [ChartFunction("[单值单列]开奖号AC值计算出质合值", ChartItemType.SingleCell_AcZhiHe, ChartItemClassName.SingleValue)]
        public static string GetAcZhiHe<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {

            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            string[] kjh = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                kjh[i] = list[i].ToString();
            }
            string ac = LotteryUtils.GetAC(kjh).ToString();

            if (LotteryUtils.IsPrimeNumbers(int.Parse(ac)))
            {
                return "质";
            }
            else
            {
                return "合";
            }
        }
        /// <summary>
        /// [单值单列]期号项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>单列期数项</returns>
        [ChartFunction("[单值单列]期号项", ChartItemType.Term_TermItem, ChartItemClassName.SingleValue)]
        public static string GetTermItemValue<TEntity>(TEntity entity) where TEntity : LotteryOpenCode
        {
            return entity.Term.ToString();
        }
        /// <summary>
        /// [单值单列]大乐透附加区值
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>开奖号码（区间）</returns>
        [ChartFunction("[单值单列]大乐透附加区值", ChartItemType.SingleCell_HqItem, ChartItemClassName.SingleValue)]
        public static string GetHqValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            var tag = "";
            for (int i = 0; i < list.Count; i++)
            {
                tag += list[i].ToString("00");
                if (i != list.Count - 1)
                {
                    tag += ",";
                }
            }
            return tag;
        }
        /// <summary>
        /// [单值单列]开奖号计算AC值(AC值是在不同数之间计算，因此号码个数必须大于1.注:indexStart于indexEnd必须同时有值,否则取全部号码)
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>AC值</returns>
        [ChartFunction("[单值单列]开奖号计算AC值", ChartItemType.SingleCell_Ac, ChartItemClassName.SingleValue)]
        public static string GetAcValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            string[] kjh = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                kjh[i] = list[i].ToString();
            }
            return LotteryUtils.GetAC(kjh).ToString();
        }
        /// <summary>
        /// [单值多列]单个开奖号码分布项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>获得单个开奖号码</returns>
        [ChartFunction("[单值多列]单个开奖号码分布项", ChartItemType.SingleValue_QuJianFenBu, ChartItemClassName.SingleValue)]
        public static string GetQuJianStatusItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            return (entity.OpenCode[indexStart]).ToString();
        }
        /// <summary>
        /// 计算出和尾奇偶状态
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>奇、偶</returns>
        [ChartFunction("[单值多列]开奖号和尾奇偶分布项", ChartItemType.SingleValue_HeWeiJiOu, ChartItemClassName.SingleValue)]
        public static string GetHeWeiJiOuFenBu<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            int a = LotteryUtils.GetSum(list) % 10;
            if (a % 2 == 0)
            {
                return "偶";
            }
            else
            {
                return "奇";
            }
        }
        /// <summary>
        /// 单个开奖号码数字项，即单个开奖号码作为项的值
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>开奖号其中一位</returns>
        [ChartFunction("[单值多列]开奖号码单个数字项", ChartItemType.SingleValue_NumberItem, ChartItemClassName.SingleValue)]
        public static string GetNumberItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            return entity.OpenCode[indexStart].ToString();
        }
        /// <summary>
        /// 两个号码跨度
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>两个号码跨度</returns>
        /// <summary>
        /// 计算重号项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="minValue">项最小值</param>
        /// <param name="maxValue">项最大值</param>
        /// <param name="lastItemIndex">上期重号数组</param>
        /// <returns>重号值</returns>
        [ChartFunction("[单值单列]开奖号重号项", ChartItemType.SingleCell_RepeatedNumber, ChartItemClassName.SingleValue)]
        public static string GetRepeatNumItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int minValue, int maxValue, ref int[] lastItemIndex, int itemcout) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            if (null == lastItemIndex)
            {
                lastItemIndex = new int[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    lastItemIndex[i] = list[i];
                }
                return "0";
            }
            int count = 0;
            foreach (var item in list)
            {
                if (lastItemIndex.Contains(item))
                {
                    count++;
                }
            }
            lastItemIndex = new int[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                lastItemIndex[i] = list[i];
            }
            return count.ToString();

            //if (null == lastItemIndex)
            //{
            //    lastItemIndex = new int[itemcout];
            //    foreach (var item in list)
            //    {

            //        lastItemIndex[item - minValue]++;
            //    }
            //    return "0";
            //}
            //int RepeatNum = 0;
            //foreach (var item in list)
            //{
            //    lastItemIndex[item - minValue]++;
            //}
            //foreach (var item in lastItemIndex)
            //{
            //    RepeatNum = RepeatNum + item / 2;
            //}

            //for (var j = maxValue - 1; j >= 0; j--)
            //{
            //    lastItemIndex[j] = 0;
            //}
            //foreach (var item in list)
            //{
            //    lastItemIndex[item - minValue]++;
            //}
            //return RepeatNum.ToString();
        }
        /// <summary>
        /// 计算连号项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>连号值</returns>
        [ChartFunction("[单值单列]开奖号连号项", ChartItemType.SingleCell_LinkNumber, ChartItemClassName.SingleValue)]
        public static string GetLinkNumItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            int LinkNum = 0;
            List<int> nums = entity.OpenCode.Take(indexEnd - indexStart).ToList();
            nums.Sort();
            for (int i = 1; i < nums.Count; i++)
            {
                if (i != 0 && nums[i - 1] + 1 == nums[i])
                {
                    LinkNum++;
                }
            }
            return LinkNum.ToString();
        }
        /// <summary>
        /// 单列试机号和值项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>试机号和值项</returns>
        [ChartFunction("[单值单列]试机号和值项", ChartItemType.SingleCell_ShiJiHaoHzItem, ChartItemClassName.SingleValue)]
        public static string GetShiJiHaoHzValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = GetShiJiHaoList<TEntity>(entity, indexStart, indexEnd);

            return LotteryUtils.GetSum(list).ToString();
        }
        /// <summary>
        /// 试机号跨度
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>试机号跨度</returns>
        [ChartFunction("[单值单列]试机号跨度项", ChartItemType.SingleCell_ShiJiHaoSpanItem, ChartItemClassName.SingleValue)]
        public static string GetShiJiHaoSpanValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = GetShiJiHaoList<TEntity>(entity, indexStart, indexEnd);

            return LotteryUtils.GetSpan(list).ToString();
        }
        /// <summary>
        /// 试机号奇偶比例
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>试机号奇偶比例</returns>
        [ChartFunction("[单值单列]试机号奇偶比例项", ChartItemType.SingleCell_ProportionOfShiJiHaoJoItem, ChartItemClassName.SingleValue)]
        public static string GetProportionOfShiJiHaoJoItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = GetShiJiHaoList<TEntity>(entity, indexStart, indexEnd);
            return LotteryUtils.GetProportionOfJO(list);
        }
        /// <summary>
        /// 试机号大小比例
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <param name="splitNumber">大小分隔值</param>
        /// <returns>试机号大小比例</returns>
        [ChartFunction("[单值单列]试机号大小比例项", ChartItemType.SingleCell_ProportionOfShiJiHaoDxItem, ChartItemClassName.SingleValue)]
        public static string GetProportionOfShiJiHaoDxItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count, int splitNumber) where TEntity : LotteryOpenCode
        {
            IList<int> list = GetShiJiHaoList<TEntity>(entity, indexStart, indexEnd);
            return LotteryUtils.GetProportionOfDX(list, splitNumber);
        }
        /// <summary>
        /// 单列组三遗漏项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="lastTerm">最近一期期号</param>
        /// <returns>组三遗漏</returns>
        [ChartFunction("[单值单列]开奖号组三遗漏项", ChartItemType.SingleCell_ZsMissItem, ChartItemClassName.SingleValue)]
        public static string GetZsMissItem<TEntity>(TEntity entity, ref long lastTerm) where TEntity : LotteryOpenCode
        {
            string miss = "";
            if (lastTerm == 0)
            {
                lastTerm = entity.Term;
                return "0";
            }
            if (entity.Term.ToString().Substring(0, 4) == lastTerm.ToString().Substring(0, 4))
            {
                miss = (entity.Term - lastTerm - 1).ToString();
                lastTerm = entity.Term;
                return miss;
            }
            //上一期的年数
            int year = Convert.ToInt32(lastTerm.ToString().Substring(0, 4));
            //上一年的最大期数
            long lastmaxqi = 0;
            if ((year % 4 == 0 && year % 100 != 0) || year % 400 == 0)
            {
                //瑞年
                lastmaxqi = year * 1000 + 359;
            }
            else
            {
                lastmaxqi = year * 1000 + 358;
            }
            long num = lastmaxqi - lastTerm - 1;
            long Tnum = num + (entity.Term - (Convert.ToInt32(entity.Term.ToString().Substring(0, 4)) * 1000 + 1));
            lastTerm = entity.Term;
            return Tnum.ToString();

        }
        /// <summary>
        /// 计算组三号码（必须为三个开奖号码）
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <returns>组三号码</returns>
        [ChartFunction("[单值单列]开奖号组三号码项", ChartItemType.SingleCell_ZsHaoMaItem, ChartItemClassName.SingleValue)]
        public static string GetZsHaoMaValue<TEntity>(TEntity entity) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (list[0] == list[1])
            {
                return list[0].ToString();
            }
            if (list[0] == list[2])
            {
                return list[0].ToString();
            }
            return list[1].ToString();
        }
        /// <summary>
        /// [单值单列]试机号展示项(限定福彩3D和排列三)
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>单列多个或全部试机号展示项</returns>
        [ChartFunction("[单值单列]试机号展示项(限福彩3D和排列三)", ChartItemType.SingleCell_ShiJiHao, ChartItemClassName.SingleValue)]
        public static string GetShiJiHaoItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = GetShiJiHaoList<TEntity>(entity, indexStart, indexEnd);

            string num = "";
            foreach (int item in list)
            {
                num = num + item.ToString();
            }
            return num;
        }
        /// <summary>
        /// 计算三区比(适用于双色球)
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>三区比(双色球)</returns>
        [ChartFunction("[单值单列]开奖号三区比项(限双色球)", ChartItemType.SingleCell_SanQu, ChartItemClassName.SingleValue)]
        public static string GetSsqsanqu<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            return LotteryUtils.SsqSanQu(list);
        }
        /// <summary>
        /// 开奖号码012值
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>开奖号码012值</returns>
        [ChartFunction("[单值单列]开奖号码012值", ChartItemType.SingleCell_012StatusItem, ChartItemClassName.SingleValue)]
        public static string Get012StatusItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            var tag = "";
            foreach (var item in list)
            {
                tag = tag + (item % 3).ToString();
            }
            return tag;
        }
        [ChartFunction("[单值多列]开奖号跨度项(两个号码)", ChartItemType.SingleValue_SpanNumberItem, ChartItemClassName.SingleValue)]
        public static string GetSpanNumberItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            return (Math.Abs(entity.OpenCode[indexStart] - entity.OpenCode[indexEnd])).ToString();
        }
        /// <summary>
        /// 全部号码跨度或两个号码以上(不包括两个号码)跨度分布
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>全部号码跨度或两个号码以上(不包括两个号码)跨度</returns>
        [ChartFunction("[单值多列]开奖号跨度(大于两个号码)分布项", ChartItemType.SingleValue_SpanItem, ChartItemClassName.SingleValue)]
        public static string GetSpanSingleValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            return LotteryUtils.GetSpan(list).ToString();
        }
        /// <summary>
        /// 单值_多个号码和值尾数分布项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>多个号码和值尾数项</returns>
        [ChartFunction("[单值多列]开奖号和尾分布项", ChartItemType.SingleValue_HeWeiItem, ChartItemClassName.SingleValue)]
        public static string GetHeWeiSingleValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            return (LotteryUtils.GetSum(list) % 10).ToString();
        }
        /// <summary>
        /// 单列和值分布项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>单列和值项</returns>
        [ChartFunction("[单值多列]开奖号和值分布项", ChartItemType.SingleValue_SumItem, ChartItemClassName.SingleValue)]
        public static string GetSumSingleValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            return LotteryUtils.GetSum(list).ToString();
        }
        /// <summary>
        /// 单列和值分布(区间)项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>单列和值项</returns>
        [ChartFunction("[单值多列]开奖号和值分布项(根据区间)", ChartItemType.SingleValue_SumItemGroup, ChartItemClassName.SingleValue)]
        public static string GetSumSingleValueGroup<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            return LotteryUtils.GetSum(list).ToString();
        }
        /// <summary>
        /// 通过AC值计算出奇偶状态
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>值计算出奇偶状态</returns>
        [ChartFunction("[单值多列]开奖号AC值计算出奇偶分布项", ChartItemType.SingleCell_AcJiOu, ChartItemClassName.SingleValue)]
        public static string GetAcJiOu<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {

            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            string[] kjh = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                kjh[i] = list[i].ToString();
            }
            string ac = LotteryUtils.GetAC(kjh).ToString();

            if (LotteryUtils.IsJoNumbers(int.Parse(ac)))
            {
                return "奇";
            }
            else
            {
                return "偶";
            }
        }
        /// <summary>
        /// 单个或多个开奖号码大小状态项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>单个或多个开奖号码大小状态项</returns>
        [ChartFunction("[单值多列]开奖号码大小分布项", ChartItemType.SingleValue_DaXiaoStatusItem, ChartItemClassName.SingleValue)]
        public static string GetDaXiaoStatusItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count, int splitNumber) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            var tag = "";
            foreach (var item in list)
            {
                tag = tag + (item >= splitNumber ? "大" : "小");
            }
            return tag;
        }
        /// <summary>
        /// 单个或多个开奖号码奇偶状态项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>单个或多个开奖号码奇偶状态项</returns>
        [ChartFunction("[单值多列]开奖号码奇偶分布项", ChartItemType.SingleValue_JiOuStatusItem, ChartItemClassName.SingleValue)]
        public static string GetJiOuStatusItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            var tag = "";
            foreach (var item in list)
            {
                tag = tag + (1 == item % 2 ? "奇" : "偶");
            }
            return tag;
        }
        /// <summary>
        /// 单个开奖号码012形态项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>单个开奖号码012形态项</returns> 
        [ChartFunction("[单值多列]开奖号码012路分布项", ChartItemType.SingleValue_Number012StatusItem, ChartItemClassName.SingleValue)]
        public static string GetNumber012StatusItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            var tag = "";
            foreach (var item in list)
            {
                tag = tag + (item % 3).ToString();
            }
            return tag;
            //return (entity.OpenCode[indexStart] % 3).ToString();
        }
        /// <summary>
        /// 质合状态项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>质合状态项</returns> 
        [ChartFunction("[单值多列]开奖号质合分布项", ChartItemType.SingleValue_ZhiHeStatusItem, ChartItemClassName.SingleValue)]
        public static string GetZhiHeStatusItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            var tag = "";
            foreach (var item in list)
            {
                tag = tag + (LotteryUtils.IsPrimeNumbers(item) ? "质" : "合");
            }
            return tag;
        }
        /// <summary>
        /// 号码组合项：组三、组六或组三、组六、豹子（特殊类项）
        /// 此项只有两至三列且限三个开奖号码
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>组三、组六、豹子</returns>
        [ChartFunction("[单值多列]开奖号组合项(组三、组六、豹子:限三个号码)", ChartItemType.SingleValue_ZuHeStatusItem, ChartItemClassName.SingleValue)]
        public static string GetZuHeStatusItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = LotteryUtils.GetOpenCodeList(entity, indexStart, indexEnd);
            IDictionary<int, int> d = new Dictionary<int, int>();
            foreach (var item in list)
            {
                if (!d.ContainsKey(item))
                    d.Add(item, 0);
                d[item]++;
            }
            var tag = "组六";
            foreach (var item in d.Keys)
            {
                if (3 == d[item])
                {
                    tag = 2 == count ? "组三" : "豹子";
                    break;
                }
                if (2 == d[item])
                {
                    tag = "组三";
                    break;
                }
            }
            return tag;
        }
        /// <summary>
        /// 单列多个或全部试机号展示项
        /// 此目前限定福彩3D和排列三
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>单列多个或全部试机号展示项</returns>
        [ChartFunction("[单值多列]试机号展示项(限定福彩3D和排列三)", ChartItemType.SingleValue_ShiJiHao, ChartItemClassName.SingleValue)]
        public static string GetShiJiHaoSingleValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = GetShiJiHaoList<TEntity>(entity, indexStart, indexEnd);

            string num = "";
            foreach (int item in list)
            {
                num = num + item.ToString();
            }
            return num;
        }
        /// <summary>
        /// [单值多列]和值的奇偶分布项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>奇、偶</returns>
        [ChartFunction("[单值多列]开奖号和值的奇偶分布项", ChartItemType.SingleValue_HzJoStatusItem, ChartItemClassName.SingleValue)]
        public static string GetHzJoStatusValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            int sum = LotteryUtils.GetSum(list);
            if (sum % 2 == 0)
            {
                return "偶";
            }
            else
            {
                return "奇";
            }
        }
        /// <summary>
        /// 计算和值大小状态
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>大、小</returns>
        [ChartFunction("[单值多列]开奖号和值大小分布项", ChartItemType.SingleValue_HzDxStatusItem, ChartItemClassName.SingleValue)]
        public static string GetHzDxStatusValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int SplitNumberOfDX, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            int sum = LotteryUtils.GetSum(list);
            if (sum >= SplitNumberOfDX)
            {
                return "大";
            }
            else
            {
                return "小";
            }
        }
        /// <summary>
        /// 计算开奖号各位大小状态
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>开奖号各位大小状态</returns>
        [ChartFunction("[单值多列]开奖号单个号码大小分布项", ChartItemType.SingleValue_DxStatusItem, ChartItemClassName.SingleValue)]
        public static string GetDxStatusValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            var tag = "";
            foreach (var item in list)
            {
                tag = tag + (LotteryUtils.IsDxNumbers(item) ? "大" : "小");
            }
            return tag;
        }
        /// <summary>
        /// 计算开奖号各位的奇偶状态
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>开奖号各位的奇偶状态</returns>
        [ChartFunction("[单值多列]开奖号单个号码奇偶分布项", ChartItemType.SingleValue_JoStatusItem, ChartItemClassName.SingleValue)]
        public static string GetJoStatusItem<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            var tag = "";
            foreach (var item in list)
            {
                tag = tag + (LotteryUtils.IsJoNumbers(item) ? "奇" : "偶");
            }
            return tag;
        }
        /// <summary>
        /// 计算组三形态
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>组三形态（AAB形态返回）</returns>
        [ChartFunction("[单值多列]开奖号组三分布项", ChartItemType.SingleValue_ZsStatusItem, ChartItemClassName.SingleValue)]
        public static string GetZsStatusItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (list[0] == list[1])
            {
                return "AAB";
            }
            if (list[0] == list[2])
            {
                return "ABA";
            }
            return "BAA";
        }
        /// <summary>
        /// 计算组三奇偶形态
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <returns>奇、偶</returns>
        [ChartFunction("[单值多列]开奖号组三奇偶分布项", ChartItemType.SingleValue_ZsJoStatusItem, ChartItemClassName.SingleValue)]
        public static string GetZsJoStatusValue<TEntity>(TEntity entity) where TEntity : LotteryOpenCode
        {
            int num = Convert.ToInt32(GetZsHaoMaValue(entity));
            if (num % 2 == 0)
            {
                return "偶";
            }
            return "奇";
        }
        /// <summary>
        /// 组三大小形态
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="splitNumber">大小分隔值</param>
        /// <returns>组三大小形态</returns>
        [ChartFunction("[单值多列]开奖号组三大小分布项", ChartItemType.SingleValue_ZsDxStatusItem, ChartItemClassName.SingleValue)]
        public static string GetZsDxStatusValue<TEntity>(TEntity entity, int splitNumber) where TEntity : LotteryOpenCode
        {
            int num = Convert.ToInt32(GetZsHaoMaValue(entity));
            if (num > splitNumber)
            {
                return "大";
            }
            return "小";
        }
        /// <summary>
        /// 组三012路形态
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <returns>路形态</returns>
        [ChartFunction("[单值多列]开奖号组三012路分布项", ChartItemType.SingleValue_Zs012StatusItem, ChartItemClassName.SingleValue)]
        public static string GetZs012StatusValue<TEntity>(TEntity entity) where TEntity : LotteryOpenCode
        {
            int num = Convert.ToInt32(GetZsHaoMaValue(entity));
            return (num % 3).ToString();
        }
        /// <summary>
        /// 试机号类型（组三、组六、豹子）
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>组三、组六、豹子</returns>
        [ChartFunction("[单值多列]试机号类型项(组三、组六、豹子:限三个开奖号码)", ChartItemType.SingleValue_ShiJiHaoTypeItem, ChartItemClassName.SingleValue)]
        public static string GetShiJiHaoTyepValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = GetShiJiHaoList<TEntity>(entity, indexStart, indexEnd);
            IDictionary<int, int> d = new Dictionary<int, int>();
            foreach (var item in list)
            {
                if (!d.ContainsKey(item))
                    d.Add(item, 0);
                d[item]++;
            }
            var tag = "组六";
            foreach (var item in d.Keys)
            {
                if (3 == d[item])
                {
                    tag = 2 == count ? "组三" : "豹子";
                    break;
                }
                if (2 == d[item])
                {
                    tag = "组三";
                    break;
                }
            }
            return tag;
        }
        /// <summary>
        /// 生肖状态项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>质合状态项</returns> 
        [ChartFunction("[单值多列]开奖号生肖分布项", ChartItemType.SingleValue_SX, ChartItemClassName.SingleValue)]
        public static string GetSXStatusItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            string[] SX = new string[12] { "鼠", "牛", "虎", "兔", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪" };
            var tag = "";
            foreach (var item in list)
            {
                tag = tag + (item % 12 == 0 ? "猪" : SX[item % 12 - 1]);
            }
            return tag;
        }
        /// <summary>
        /// 季节状态项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>质合状态项</returns> 
        [ChartFunction("[单值多列]开奖号季节分布项", ChartItemType.SingleValue_JJ, ChartItemClassName.SingleValue)]
        public static string GetJJStatusItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            var tag = "";
            foreach (var item in list)
            {
                if (item >= 1 && item <= 9)
                {
                    tag = tag + "春";
                    continue;
                }
                if (item >= 10 && item <= 18)
                {
                    tag = tag + "夏";
                    continue;
                }
                if (item >= 19 && item <= 27)
                {
                    tag = tag + "秋";
                    continue;
                }
                if (item >= 28 && item <= 36)
                {
                    tag = tag + "冬";
                    continue;
                }
            }
            return tag;
        }
        /// <summary>
        /// 方位状态项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>质合状态项</returns> 
        [ChartFunction("[单值多列]开奖号方位分布项", ChartItemType.SingleValue_FW, ChartItemClassName.SingleValue)]
        public static string GetFWStatusItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            var tag = "";
            foreach (var item in list)
            {
                if (item >= 1 && item <= 18)
                {
                    if (item % 2 == 1)
                    {
                        tag = tag + "东";
                    }
                    else
                    {
                        tag = tag + "南";
                    }
                    continue;
                }
                if (item >= 19 && item <= 36)
                {
                    if (item % 2 == 1)
                    {
                        tag = tag + "西";
                    }
                    else
                    {
                        tag = tag + "北";
                    }
                    continue;
                }
            }
            return tag;
        }

        /// <summary>
        /// 试机号转换成IList<int>类型
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <returns>试机号列表</returns>
        private static IList<int> GetShiJiHaoList<TEntity>(TEntity entity, int indexStart, int indexEnd) where TEntity : LotteryOpenCode
        {
            if (entity.ShiJiHao == "-1")
            {
                return new List<int> { -1, -1, -1 };
            }
            string[] arr_sjh = entity.ShiJiHao.Split(',');
            IList<int> list = new List<int>();
            foreach (var item in arr_sjh)
            {
                list.Add(Convert.ToInt32(item));
            }
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            return list;
        }
        /// <summary>
        /// 回摆项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>回摆状态</returns> 
        [ChartFunction("[单值多列]回摆", ChartItemType.SingleValue_HB, ChartItemClassName.SingleValue)]
        public static string HBSingleValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int minValue, int maxValue, ref int[] lastItemIndex, int itemcout) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            if (null == lastItemIndex)
            {
                lastItemIndex = new int[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    lastItemIndex[i] = list[i];
                }
                return "重号";
            }
            string result = "";
            foreach (var item in list)
            {
                if (lastItemIndex[0] > item)
                {
                    result = "反向";
                    break;
                }
            }
            foreach (var item in list)
            {
                if (lastItemIndex[0] < item)
                {
                    result = "正向";
                    break;
                }
            }
            foreach (var item in list)
            {
                if (lastItemIndex[0] == item)
                {
                    result = "重号";
                    break;
                }
            }
            lastItemIndex = new int[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                lastItemIndex[i] = list[i];
            }
            return result;
        }
        /// <summary>
        /// 振幅项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <returns>振幅值</returns> 
        [ChartFunction("[单值多列]振幅", ChartItemType.SingleCell_ZF, ChartItemClassName.SingleValue)]
        public static string ZFSingleCell<TEntity>(TEntity entity, int indexStart, int indexEnd, int minValue, int maxValue, ref int[] lastItemIndex, int itemcout) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            if (null == lastItemIndex)
            {
                lastItemIndex = new int[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    lastItemIndex[i] = list[i];
                }
                return list.Sum().ToString();
            }
            int result = list.Sum() - lastItemIndex.Sum();
            lastItemIndex = new int[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                lastItemIndex[i] = list[i];
            }
            return Math.Abs(result).ToString();
        }
        /// <summary>
        /// 福建31选7三区比
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="indexStart"></param>
        /// <param name="indexEnd"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [ChartFunction("[单值单列]福建31选7三区比", ChartItemType.SingleCell_FJ31X7SanQu, ChartItemClassName.SingleValue)]
        public static string GetFj31x7sanqu<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            return LotteryUtils.Fj31x7SanQu(list);
        }
        /// <summary>
        /// 福建36选7三区比
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="indexStart"></param>
        /// <param name="indexEnd"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [ChartFunction("[单值单列]福建36选7三区比", ChartItemType.SingleCell_FJ36X7SanQu, ChartItemClassName.SingleValue)]
        public static string GetFj36x7sanqu<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            return LotteryUtils.Fj36x7SanQu(list);
        }
        /// <summary>
        /// 和尾大小
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="indexStart"></param>
        /// <param name="indexEnd"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [ChartFunction("[单值单列]和尾大小", ChartItemType.SingleValue_HeWeiDx, ChartItemClassName.SingleValue)]
        public static string GetHeWeiDxFenBu<TEntity>(TEntity entity, int indexStart, int indexEnd, int SplitNumberOfDX, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            int sum = LotteryUtils.GetSum(list);
            int hw = sum % 10;
            if (hw >= SplitNumberOfDX)
            {
                return "大";
            }
            else
            {
                return "小";
            }
        }
        /// <summary>
        /// 生肖
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="indexStart"></param>
        /// <param name="indexEnd"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [ChartFunction("[单值单列]东方6+1生肖", ChartItemType.SingleValue_ShengXiao, ChartItemClassName.SingleValue)]
        public static string Getdf6j1sx<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            switch (list[0])
            {
                case 1:
                    return "鼠";
                case 2:
                    return "牛";
                case 3:
                    return "虎";
                case 4:
                    return "兔";
                case 5:
                    return "龙";
                case 6:
                    return "蛇";
                case 7:
                    return "马";
                case 8:
                    return "羊";
                case 9:
                    return "猴";
                case 10:
                    return "鸡";
                case 11:
                    return "狗";
                case 12:
                    return "猪";
                default:
                    return "";
            }
        }
        /// <summary>
        /// 华东15选5三区比
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="indexStart"></param>
        /// <param name="indexEnd"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [ChartFunction("[单值单列]华东15选5三区比", ChartItemType.SingleCell_Hd15x5SanQU, ChartItemClassName.SingleValue)]
        public static string Gethd15x5sanqu<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            return LotteryUtils.Hd15x5SanQu(list);
        }
        /// <summary>
        /// 华东15选5一区个数
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="indexStart"></param>
        /// <param name="indexEnd"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [ChartFunction("[单值单列]华东15选5一区的个数", ChartItemType.SingleValue_Hd11x5Yq, ChartItemClassName.SingleValue)]
        public static string Gethd15x5Yq<TEntity>(TEntity entity, int indexStart, int indexEnd, int count, int min, int max) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            int num = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] >= min && list[i] <= max)
                {
                    num++;
                }
            }
            return num.ToString();
        }
        /// <summary>
        /// 华东15选5二区个数
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="indexStart"></param>
        /// <param name="indexEnd"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [ChartFunction("[单值单列]华东15选5二区的个数", ChartItemType.SingleValue_Hd11x5Eq, ChartItemClassName.SingleValue)]
        public static string Gethd15x5Eq<TEntity>(TEntity entity, int indexStart, int indexEnd, int count, int min, int max) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            int num = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] >= min && list[i] <= max)
                {
                    num++;
                }
            }
            return num.ToString();
        }
        /// <summary>
        /// 华东15选5三区个数
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="indexStart"></param>
        /// <param name="indexEnd"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [ChartFunction("[单值单列]华东15选5三区的个数", ChartItemType.SingleValue_Hd11x5Sq, ChartItemClassName.SingleValue)]
        public static string Gethd15x5Sq<TEntity>(TEntity entity, int indexStart, int indexEnd, int count, int min, int max) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            int num = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] >= min && list[i] <= max)
                {
                    num++;
                }
            }
            return num.ToString();
        }
        /// <summary>
        /// 南粤36选7三区比
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="indexStart"></param>
        /// <param name="indexEnd"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [ChartFunction("[单值单列]南粤36选7三区比", ChartItemType.SingleCell_NY36x7Sanqu, ChartItemClassName.SingleValue)]
        public static string Getny36x7sanqu<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }

            return LotteryUtils.Ny36x7SanQu(list);
        }
        /// <summary>
        /// 和值012路
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="indexStart"></param>
        /// <param name="indexEnd"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [ChartFunction("[单值多列]和值012路", ChartItemType.SingleCell_Hz012, ChartItemClassName.SingleValue)]
        public static string GetHz012Value<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            string hz = (list.Sum() % 3).ToString();
            return hz;
        }
        /// <summary>
        /// 快3三不同走势
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="indexStart"></param>
        /// <param name="indexEnd"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [ChartFunction("[单值多列]快3三不同走势", ChartItemType.SingleValue_K3sbt, ChartItemClassName.SingleValue)]
        public static string GetK3sbtValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count, string[] ItemString) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            int[] k3s = new int[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                k3s[i] = list[i];
            }
            Array.Sort(k3s);
            string result = "";
            for (int i = 0; i < k3s.Count(); i++)
            {
                result += k3s[i];
            }
            if (ItemString.Contains(result))
            {
                return result;
            }
            return "";
        }
        /// <summary>
        /// 快3二不同(单值)
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="indexStart"></param>
        /// <param name="indexEnd"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [ChartFunction("[单值多列]快3二不同(单值)", ChartItemType.SingleCell_K3ebt, ChartItemClassName.SingleValue)]
        public static string GetEbtValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            string result = "";
            if (list.Count != 3)
            {
                return result;
            }
            if (list[0] == list[1])
            {
                result = list[0].ToString() + list[1].ToString();
                return result;
            }
            if (list[0] == list[2])
            {
                result = list[0].ToString() + list[2].ToString();
                return result;
            }
            if (list[1] == list[2])
            {
                result = list[1].ToString() + list[2].ToString();
                return result;
            }
            return result;
        }

        /// <summary>
        /// 奇偶个数
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="indexStart"></param>
        /// <param name="indexEnd"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [ChartFunction("[单值多列]奇偶个数", ChartItemType.SingleValue_JoValue, ChartItemClassName.SingleValue)]
        public static string GetJoValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            int result = 0;
            foreach (var item in list)
            {
                if (item % 2 != 0)
                {
                    result++;
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// 大小个数
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="indexStart"></param>
        /// <param name="indexEnd"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [ChartFunction("[单值多列]大小个数", ChartItemType.SingleValue_DxValue, ChartItemClassName.SingleValue)]
        public static string GetDxValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count, int splitNumber) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            int result = 0;
            foreach (var item in list)
            {
                if (item >= splitNumber)
                {
                    result++;
                }
            }
            return result.ToString();
        }
        /// <summary>
        /// 质合个数
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="indexStart"></param>
        /// <param name="indexEnd"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [ChartFunction("[单值多列]质合个数", ChartItemType.SingleValue_ZhValue, ChartItemClassName.SingleValue)]
        public static string GetZhValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            int[] zs = { 1, 2, 3, 5 };
            int result = 0;
            foreach (var item in list)
            {
                if (zs.Contains(item))
                {
                    result++;
                }
            }
            return result.ToString();
        }
        /// <summary>
        /// 大小奇偶
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="indexStart"></param>
        /// <param name="indexEnd"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [ChartFunction("[单值多列]大小奇偶", ChartItemType.SingleValue_DxjoValue, ChartItemClassName.SingleValue)]
        public static string GetDxjoValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int splitNumber, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            string result = "";
            if (list[0] > splitNumber)
            {
                result = "大";
            }
            else
            {
                result = "小";
            }
            if (list[0] % 2 == 0)
            {
                result += "双";
            }
            else
            {
                result += "单";
            }
            return result;
        }
        /// <summary>
        /// 小数个数
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="indexStart"></param>
        /// <param name="indexEnd"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [ChartFunction("[单值多列]小数个数", ChartItemType.SingleValue_XsValue, ChartItemClassName.SingleValue)]
        public static string GetXsValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count, int splitNumber) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            int result = 0;
            foreach (var item in list)
            {
                if (item < splitNumber)
                {
                    result++;
                }
            }
            return result.ToString();
        }
        /// <summary>
        /// 合数个数
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="indexStart"></param>
        /// <param name="indexEnd"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [ChartFunction("[单值多列]合数个数", ChartItemType.SingleValue_HsValue, ChartItemClassName.SingleValue)]
        public static string GetHsValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            int[] zs = { 4, 6 };
            int result = 0;
            foreach (var item in list)
            {
                if (zs.Contains(item))
                {
                    result++;
                }
            }
            return result.ToString();
        }
        /// <summary>
        /// 偶数个数
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="indexStart"></param>
        /// <param name="indexEnd"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [ChartFunction("[单值多列]偶数个数", ChartItemType.SingleValue_OsValue, ChartItemClassName.SingleValue)]
        public static string GetOsValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count) where TEntity : LotteryOpenCode
        {
            IList<int> list = new List<int>(entity.OpenCode);
            if (-1 != indexEnd)
            {
                for (int i = list.Count - 1; i >= indexEnd; i--)
                { list.RemoveAt(indexEnd); }
            }
            for (int i = 0; i < indexStart; i++)
            { list.RemoveAt(0); }
            int result = 0;
            foreach (var item in list)
            {
                if (item % 2 == 0)
                {
                    result++;
                }
            }
            return result.ToString();
        }
    }
}
