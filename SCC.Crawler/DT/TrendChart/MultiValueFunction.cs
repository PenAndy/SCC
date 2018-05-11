using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SCC.Models;

namespace SCC.Crawler.DT
{
    /// <summary>
    /// 获取多值项值类
    /// </summary>
    public class MultiValueFunction
    {
        /// <summary>
        /// [多值多列]开奖号码分布项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <param name="minNum">项最小值</param>
        /// <param name="maxNum">项最大值</param>
        /// <param name="index">项索引</param>
        /// <param name="missNumber">遗漏值</param>
        /// <param name="lastMiss">上期遗漏</param>
        /// <param name="maxMiss">最大遗漏</param>
        /// <param name="times">遗漏次数</param>
        /// <param name="avgMiss">平均遗漏</param>
        [ChartFunction("[多值多列]开奖号码分布项", ChartItemType.MultiValue_OpenCodeItem, ChartItemClassName.MultiValue)]
        public static void SetOpenCodeItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count, int minNum, int maxNum,
            ref int[] index, ref int[] missNumber, ref int[] lastMiss, ref int[] maxMiss, ref int[] times, ref int[] avgMiss, int RecordCount) where TEntity : LotteryOpenCode
        {
            GetMultiValue<TEntity>(entity, indexStart, indexEnd, minNum, maxNum, index, missNumber, lastMiss, maxMiss, times, avgMiss, RecordCount);
        }
        /// <summary>
        /// [多值多列]开奖号连号分布项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <param name="minNum">项最小值</param>
        /// <param name="maxNum">项最大值</param>
        /// <param name="index">项索引</param>
        /// <param name="missNumber">遗漏值</param>
        /// <param name="lastMiss">上期遗漏</param>
        /// <param name="maxMiss">最大遗漏</param>
        /// <param name="times">遗漏次数</param>
        /// <param name="avgMiss">平均遗漏</param>
        [ChartFunction("[多值多列]开奖号连号分布项", ChartItemType.MultiValue_LinkNumber, ChartItemClassName.MultiValue)]
        public static void SetLinkNumberValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count, int minNum, int maxNum,
            ref int[] index, ref int[] missNumber, ref int[] lastMiss, ref int[] maxMiss, ref int[] times, ref int[] avgMiss, int RecordCount) where TEntity : LotteryOpenCode
        {
            GetMultiValue<TEntity>(entity, indexStart, indexEnd, minNum, maxNum, index, missNumber, lastMiss, maxMiss, times, avgMiss, RecordCount);
        }

        /// <summary>
        /// [多值多列]开奖号重号分布项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <param name="minNum">项最小值</param>
        /// <param name="maxNum">项最大值</param>
        /// <param name="index">项索引</param>
        /// <param name="missNumber">遗漏值</param>
        /// <param name="lastMiss">上期遗漏</param>
        /// <param name="maxMiss">最大遗漏</param>
        /// <param name="times">遗漏次数</param>
        /// <param name="avgMiss">平均遗漏</param>
        [ChartFunction("[多值多列]开奖号重号分布项", ChartItemType.MultiValue_RepeatNumber, ChartItemClassName.MultiValue)]
        public static void SetRepeatNumberValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count, int minNum, int maxNum,
            ref int[] index, ref int[] missNumber, ref int[] lastMiss, ref int[] maxMiss, ref int[] times, ref int[] avgMiss, int RecordCount, ref IList<int> LastOpentCode, ref int[] lastItemIndex) where TEntity : LotteryOpenCode
        {
            GetMultiValue<TEntity>(entity, indexStart, indexEnd, minNum, maxNum, index, missNumber, lastMiss, maxMiss, times, avgMiss, RecordCount);
        }
        /// <summary>
        /// [多值多列]开奖号折号分布项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <param name="minNum">项最小值</param>
        /// <param name="maxNum">项最大值</param>
        /// <param name="index">项索引</param>
        /// <param name="missNumber">遗漏值</param>
        /// <param name="lastMiss">上期遗漏</param>
        /// <param name="maxMiss">最大遗漏</param>
        /// <param name="times">遗漏次数</param>
        /// <param name="avgMiss">平均遗漏</param>
        [ChartFunction("[多值多列]开奖号折号分布项", ChartItemType.MultiValue_ZheHaoNumber, ChartItemClassName.MultiValue)]
        public static void SetZheHaoHaoNumberValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count, int minNum, int maxNum,
            ref int[] index, ref int[] missNumber, ref int[] lastMiss, ref int[] maxMiss, ref int[] times, ref int[] avgMiss, int RecordCount, ref IList<int> LastOpentCode, ref int[] lastItemIndex) where TEntity : LotteryOpenCode
        {
            GetMultiValue<TEntity>(entity, indexStart, indexEnd, minNum, maxNum, index, missNumber, lastMiss, maxMiss, times, avgMiss, RecordCount);
        }
        /// <summary>
        /// [多值多列]开奖号斜连号分布项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <param name="minNum">项最小值</param>
        /// <param name="maxNum">项最大值</param>
        /// <param name="index">项索引</param>
        /// <param name="missNumber">遗漏值</param>
        /// <param name="lastMiss">上期遗漏</param>
        /// <param name="maxMiss">最大遗漏</param>
        /// <param name="times">遗漏次数</param>
        /// <param name="avgMiss">平均遗漏</param>
        [ChartFunction("[多值多列]开奖号斜连号分布项", ChartItemType.MultiValue_XieLianHaoNumber, ChartItemClassName.MultiValue)]
        public static void SetXieLianHaoNumberValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count, int minNum, int maxNum,
            ref int[] index, ref int[] missNumber, ref int[] lastMiss, ref int[] maxMiss, ref int[] times, ref int[] avgMiss, int RecordCount, ref IList<int> LastOpentCode, ref int[] lastItemIndex) where TEntity : LotteryOpenCode
        {
            GetMultiValue<TEntity>(entity, indexStart, indexEnd, minNum, maxNum, index, missNumber, lastMiss, maxMiss, times, avgMiss, RecordCount);
        }
        /// <summary>
        /// [多值多列]开奖号斜跳号分布项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <param name="minNum">项最小值</param>
        /// <param name="maxNum">项最大值</param>
        /// <param name="index">项索引</param>
        /// <param name="missNumber">遗漏值</param>
        /// <param name="lastMiss">上期遗漏</param>
        /// <param name="maxMiss">最大遗漏</param>
        /// <param name="times">遗漏次数</param>
        /// <param name="avgMiss">平均遗漏</param>
        [ChartFunction("[多值多列]开奖号斜跳号分布项", ChartItemType.MultiValue_XieTiaoHaoNumber, ChartItemClassName.MultiValue)]
        public static void SetTiaoHaoNumberValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count, int minNum, int maxNum,
            ref int[] index, ref int[] missNumber, ref int[] lastMiss, ref int[] maxMiss, ref int[] times, ref int[] avgMiss, int RecordCount, ref IList<int> LastOpentCode, ref int[] lastItemIndex) where TEntity : LotteryOpenCode
        {
            GetMultiValue<TEntity>(entity, indexStart, indexEnd, minNum, maxNum, index, missNumber, lastMiss, maxMiss, times, avgMiss, RecordCount);
        }
        /// <summary>
        /// [多值多列]开奖号竖三连号分布项
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <param name="minNum">项最小值</param>
        /// <param name="maxNum">项最大值</param>
        /// <param name="index">项索引</param>
        /// <param name="missNumber">遗漏值</param>
        /// <param name="lastMiss">上期遗漏</param>
        /// <param name="maxMiss">最大遗漏</param>
        /// <param name="times">遗漏次数</param>
        /// <param name="avgMiss">平均遗漏</param>
        [ChartFunction("[多值多列]开奖号竖三连号分布项", ChartItemType.MultiValue_ShuSanLianHaoNumber, ChartItemClassName.MultiValue)]
        public static void SetShuSanLianHaoNumberValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count, int minNum, int maxNum,
            ref int[] index, ref int[] missNumber, ref int[] lastMiss, ref int[] maxMiss, ref int[] times, ref int[] avgMiss, int RecordCount, ref IList<int> LastOpentCode, ref int[] lastItemIndex) where TEntity : LotteryOpenCode
        {
            GetMultiValue<TEntity>(entity, indexStart, indexEnd, minNum, maxNum, index, missNumber, lastMiss, maxMiss, times, avgMiss, RecordCount);
        }
        /// <summary>
        /// [多值多列]开奖号竖跳号分布
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <param name="minNum">项最小值</param>
        /// <param name="maxNum">项最大值</param>
        /// <param name="index">项索引</param>
        /// <param name="missNumber">遗漏值</param>
        /// <param name="lastMiss">上期遗漏</param>
        /// <param name="maxMiss">最大遗漏</param>
        /// <param name="times">遗漏次数</param>
        /// <param name="avgMiss">平均遗漏</param>
        [ChartFunction("[多值多列]开奖号竖跳号分布", ChartItemType.MultiValue_ShuTiaoHaoNumber, ChartItemClassName.MultiValue)]
        public static void SetShuTiaoHaoNumberValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count, int minNum, int maxNum,
            ref int[] index, ref int[] missNumber, ref int[] lastMiss, ref int[] maxMiss, ref int[] times, ref int[] avgMiss, int RecordCount, ref IList<int> LastOpentCode, ref int[] lastItemIndex) where TEntity : LotteryOpenCode
        {
            GetMultiValue<TEntity>(entity, indexStart, indexEnd, minNum, maxNum, index, missNumber, lastMiss, maxMiss, times, avgMiss, RecordCount);
        }

        /// <summary>
        /// [多值多列]快乐12号码分布
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <param name="minNum">项最小值</param>
        /// <param name="maxNum">项最大值</param>
        /// <param name="index">项索引</param>
        /// <param name="missNumber">遗漏值</param>
        /// <param name="lastMiss">上期遗漏</param>
        /// <param name="maxMiss">最大遗漏</param>
        /// <param name="times">遗漏次数</param>
        /// <param name="avgMiss">平均遗漏</param>
        [ChartFunction("[多值多列]快乐12号码分布", ChartItemType.MultiValue_KL12, ChartItemClassName.MultiValue)]
        public static void SetKL12NumberValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count, int minNum, int maxNum,
            ref int[] index, ref int[] missNumber, ref int[] lastMiss, ref int[] maxMiss, ref int[] times, ref int[] avgMiss, int RecordCount, ref IList<int> LastOpentCode, ref int[] lastItemIndex) where TEntity : LotteryOpenCode
        {
            GetMultiValue<TEntity>(entity, indexStart, indexEnd, minNum, maxNum, index, missNumber, lastMiss, maxMiss, times, avgMiss, RecordCount);
        }



        /// <summary>
        /// [多值多列]设置值通用方法
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <param name="minNum">项最小值</param>
        /// <param name="maxNum">项最大值</param>
        /// <param name="index">项索引</param>
        /// <param name="missNumber">遗漏值</param>
        /// <param name="lastMiss">上期遗漏</param>
        /// <param name="maxMiss">最大遗漏</param>
        /// <param name="times">遗漏次数</param>
        /// <param name="avgMiss">平均遗漏</param>
        private static void GetMultiValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int minNum, int maxNum, int[] index, int[] missNumber, int[] lastMiss, int[] maxMiss, int[] times, int[] avgMiss, int RecordCount) where TEntity : LotteryOpenCode
        {
            IList<int> list = LotteryUtils.GetOpenCodeList<TEntity>(entity, indexStart, indexEnd);

            for (int i = maxNum; i >= minNum; i--)
            {
                missNumber[i - minNum]++;
                if (list.Contains(i))
                {
                    index[i - minNum]++;
                    if (null != lastMiss)
                        lastMiss[i - minNum] = missNumber[i - minNum];//上期遗漏
                    missNumber[i - minNum] = 0;
                    if (null != times)
                        times[i - minNum]++;
                }
                //最大遗漏
                if (null != maxMiss)
                    if (missNumber[i - minNum] > maxMiss[i - minNum])
                    { maxMiss[i - minNum] = missNumber[i - minNum]; }
                // avgMiss[i - minNum] = maxMiss[i - minNum] / (times[i - minNum] + 1);//计算平均遗漏
                if (null != avgMiss && null != times)
                    avgMiss[i - minNum] = (RecordCount - times[i - minNum]) / (times[i - minNum] + 1);//计算平均遗漏
            }
        }
        /// <summary>
        /// [多值多列]获取快3二不同
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <param name="minNum">项最小值</param>
        /// <param name="maxNum">项最大值</param>
        /// <param name="index">项索引</param>
        /// <param name="missNumber">遗漏值</param>
        /// <param name="lastMiss">上期遗漏</param>
        /// <param name="maxMiss">最大遗漏</param>
        /// <param name="times">遗漏次数</param>
        /// <param name="avgMiss">平均遗漏</param>
        [ChartFunction("[多值多列]快3二不同", ChartItemType.MultiValue_K3ebt, ChartItemClassName.MultiValue)]
        public static void SetK3ebtItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count, int minNum, int maxNum,
            ref int[] index, ref int[] missNumber, ref int[] lastMiss, ref int[] maxMiss, ref int[] times, ref int[] avgMiss, int RecordCount, string[] ItemString) where TEntity : LotteryOpenCode
        {
            IList<int> list = LotteryUtils.GetOpenCodeList<TEntity>(entity, indexStart, indexEnd);
            int[] k3s = new int[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                k3s[i] = list[i];
            }
            Array.Sort(k3s);
            string[] results = new string[3];
            results[0] = k3s[0].ToString() + k3s[1].ToString();
            results[1] = k3s[0].ToString() + k3s[2].ToString();
            results[2] = k3s[1].ToString() + k3s[2].ToString();
            for (int i = maxNum; i >= minNum; i--)
            {
                missNumber[i - minNum]++;
                if (results.Contains(ItemString[i]))
                {
                    index[i - minNum]++;
                    if (null != lastMiss)
                        lastMiss[i - minNum] = missNumber[i - minNum];//上期遗漏
                    missNumber[i - minNum] = 0;
                    if (null != times)
                        times[i - minNum]++;
                }
                //最大遗漏
                if (null != maxMiss)
                    if (missNumber[i - minNum] > maxMiss[i - minNum])
                    { maxMiss[i - minNum] = missNumber[i - minNum]; }
                // avgMiss[i - minNum] = maxMiss[i - minNum] / (times[i - minNum] + 1);//计算平均遗漏
                if (null != avgMiss && null != times)
                    avgMiss[i - minNum] = (RecordCount - times[i - minNum]) / (times[i - minNum] + 1);//计算平均遗漏
            }
        }
        /// <summary>
        /// [多值多列]获取快3三不同形态
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <param name="minNum">项最小值</param>
        /// <param name="maxNum">项最大值</param>
        /// <param name="index">项索引</param>
        /// <param name="missNumber">遗漏值</param>
        /// <param name="lastMiss">上期遗漏</param>
        /// <param name="maxMiss">最大遗漏</param>
        /// <param name="times">遗漏次数</param>
        /// <param name="avgMiss">平均遗漏</param>
        [ChartFunction("[多值多列]快3三不同形态", ChartItemType.MultiValue_Sbtxt, ChartItemClassName.MultiValue)]
        public static void SetSbtxtItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count, int minNum, int maxNum,
            ref int[] index, ref int[] missNumber, ref int[] lastMiss, ref int[] maxMiss, ref int[] times, ref int[] avgMiss, int RecordCount, string[] ItemString) where TEntity : LotteryOpenCode
        {
            IList<int> list = LotteryUtils.GetOpenCodeList<TEntity>(entity, indexStart, indexEnd);
            int[] k3s = new int[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                k3s[i] = list[i];
            }
            Array.Sort(k3s);
            List<string> results = new List<string>();
            if (k3s[0] == k3s[1] && k3s[1] == k3s[2])
            {
                results.Add("三同号");
            }
            if (k3s[0] != k3s[1] && k3s[1] != k3s[2] && k3s[0] != k3s[2])
            {
                results.Add("三不同");
            }
            for (int i = maxNum; i >= minNum; i--)
            {
                missNumber[i - minNum]++;
                if (results.Contains(ItemString[i]))
                {
                    index[i - minNum]++;
                    if (null != lastMiss)
                        lastMiss[i - minNum] = missNumber[i - minNum];//上期遗漏
                    missNumber[i - minNum] = 0;
                    if (null != times)
                        times[i - minNum]++;
                }
                //最大遗漏
                if (null != maxMiss)
                    if (missNumber[i - minNum] > maxMiss[i - minNum])
                    { maxMiss[i - minNum] = missNumber[i - minNum]; }
                // avgMiss[i - minNum] = maxMiss[i - minNum] / (times[i - minNum] + 1);//计算平均遗漏
                if (null != avgMiss && null != times)
                    avgMiss[i - minNum] = (RecordCount - times[i - minNum]) / (times[i - minNum] + 1);//计算平均遗漏
            }
        }
        /// <summary>
        /// [多值多列]获取快3二不同形态
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <param name="minNum">项最小值</param>
        /// <param name="maxNum">项最大值</param>
        /// <param name="index">项索引</param>
        /// <param name="missNumber">遗漏值</param>
        /// <param name="lastMiss">上期遗漏</param>
        /// <param name="maxMiss">最大遗漏</param>
        /// <param name="times">遗漏次数</param>
        /// <param name="avgMiss">平均遗漏</param>
        [ChartFunction("[多值多列]快3二不同", ChartItemType.MultiValue_Ebtxt, ChartItemClassName.MultiValue)]
        public static void SetEbtxtItemValue<TEntity>(TEntity entity, int indexStart, int indexEnd, int count, int minNum, int maxNum,
            ref int[] index, ref int[] missNumber, ref int[] lastMiss, ref int[] maxMiss, ref int[] times, ref int[] avgMiss, int RecordCount, string[] ItemString) where TEntity : LotteryOpenCode
        {
            IList<int> list = LotteryUtils.GetOpenCodeList<TEntity>(entity, indexStart, indexEnd);
            int[] k3s = new int[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                k3s[i] = list[i];
            }
            Array.Sort(k3s);
            List<string> results = new List<string>();
            if (k3s[0] != k3s[1] || k3s[1] != k3s[2] || k3s[0] != k3s[2])
            {
                results.Add("二不同");
            }
            if (k3s[0] == k3s[1] || k3s[1] == k3s[2] || k3s[0] == k3s[2])
            {
                results.Add("二同");
            }
            for (int i = maxNum; i >= minNum; i--)
            {
                missNumber[i - minNum]++;
                if (results.Contains(ItemString[i]))
                {
                    index[i - minNum]++;
                    if (null != lastMiss)
                        lastMiss[i - minNum] = missNumber[i - minNum];//上期遗漏
                    missNumber[i - minNum] = 0;
                    if (null != times)
                        times[i - minNum]++;
                }
                //最大遗漏
                if (null != maxMiss)
                    if (missNumber[i - minNum] > maxMiss[i - minNum])
                    { maxMiss[i - minNum] = missNumber[i - minNum]; }
                // avgMiss[i - minNum] = maxMiss[i - minNum] / (times[i - minNum] + 1);//计算平均遗漏
                if (null != avgMiss && null != times)
                    avgMiss[i - minNum] = (RecordCount - times[i - minNum]) / (times[i - minNum] + 1);//计算平均遗漏
            }
        }
    }
}
