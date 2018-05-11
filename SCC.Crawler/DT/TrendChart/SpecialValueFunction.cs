using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SCC.Models;

namespace SCC.Crawler.DT
{
    public class SpecialValueFunction
    {
        /// <summary>
        /// [特殊项]福彩3D 012路走势图4
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <param name="minNum">项最小值</param>
        /// <param name="maxNum">项最大值</param>
        [ChartFunction("[特殊项]福彩3D 012路走势图4", ChartItemType.SpecialValue_FC3D012_4, ChartItemClassName.SpecialValue)]
        public static void SpecialValue_FC3D012_4<TEntity>(TEntity entity, int indexStart, int indexEnd, int count,
            int minNum, int maxNum,
            ref int[] index) where TEntity : LotteryOpenCode
        {
            IList<int> list = LotteryUtils.GetOpenCodeList<TEntity>(entity, indexStart, indexEnd);

            for (int i = maxNum; i >= minNum; i--)
            {
                if (list.Contains(i))
                {
                    index[i - minNum]++;
                }
            }
        }
        /// <summary>
        /// [特殊项]体彩P3 012路走势图4
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <param name="minNum">项最小值</param>
        /// <param name="maxNum">项最大值</param>
        [ChartFunction("[特殊项]体彩P3 012路走势图4", ChartItemType.SpecialValue_TCP3012_4, ChartItemClassName.SpecialValue)]
        public static void SpecialValue_TCP3012_4<TEntity>(TEntity entity, int indexStart, int indexEnd, int count,
            int minNum, int maxNum,
            ref int[] index) where TEntity : LotteryOpenCode
        {
            IList<int> list = LotteryUtils.GetOpenCodeList<TEntity>(entity, indexStart, indexEnd);

            for (int i = maxNum; i >= minNum; i--)
            {
                if (list.Contains(i))
                {
                    index[i - minNum]++;
                }
            }
        }
        /// <summary>
        /// [特殊项]福彩 双色球出号频率
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <param name="minNum">项最小值</param>
        /// <param name="maxNum">项最大值</param>
        [ChartFunction("[特殊项]福彩双色球 出号频率", ChartItemType.SpecialValue_FCSSQ_ChuHaoPL, ChartItemClassName.SpecialValue)]
        public static void SpecialValue_FCSSQ_ChuHaoPL<TEntity>(TEntity entity, int indexStart, int indexEnd, int count,
            int minNum, int maxNum, ref int[] index, ref int[] itemIndex) where TEntity : LotteryOpenCode
        {
            IList<int> list = LotteryUtils.GetOpenCodeList<TEntity>(entity, indexStart, indexEnd);

            for (int i = maxNum; i >= minNum; i--)
            {
                if (list.Contains(i))
                {
                    itemIndex[i - minNum]++;
                }
            }
        }
        /// <summary>
        /// [特殊项]福彩 双色球出号频率
        /// </summary>
        /// <typeparam name="TEntity">泛型实体</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="indexStart">开奖号开始</param>
        /// <param name="indexEnd">开奖号结束</param>
        /// <param name="count">项中列的个数</param>
        /// <param name="minNum">项最小值</param>
        /// <param name="maxNum">项最大值</param>
        [ChartFunction("[特殊项]体彩大乐透 出号频率", ChartItemType.SpecialValue_TCDLT_ChuHaoPL, ChartItemClassName.SpecialValue)]
        public static void SpecialValue_TCDLT_ChuHaoPL<TEntity>(TEntity entity, int indexStart, int indexEnd, int count,
            int minNum, int maxNum, ref int[] index, ref int[] itemIndex) where TEntity : LotteryOpenCode
        {
            IList<int> list = LotteryUtils.GetOpenCodeList<TEntity>(entity, indexStart, indexEnd);

            for (int i = maxNum; i >= minNum; i--)
            {
                if (list.Contains(i))
                {
                    itemIndex[i - minNum]++;
                }
            }
        }


    }
}
