using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SCC.Interface;

namespace SCC.Services
{
    /// <summary>
    /// 公共方法服务
    /// </summary>
    public class CommonServices : ICommon
    {
        /// <summary>
        /// 获取等级对应分数
        /// </summary>
        /// <param name="level">等级</param>
        /// <returns></returns>
        public int GetScore(int level)
        {
            if (level > 10) return 100;
            if (level < 0) return 0;
            return level * 10;
        }
    }
}
