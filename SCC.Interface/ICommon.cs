using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCC.Interface
{
    /// <summary>
    /// 公共方法接口
    /// </summary>
    public interface ICommon
    {
        /// <summary>
        /// 获取等级对应分数
        /// </summary>
        /// <param name="level">等级</param>
        /// <returns></returns>
        int GetScore(int level);
    }
}
