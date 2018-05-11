using SCC.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SCC.Services
{
    public class CheckResultServices
    {

        /// <summary>
        /// 执行校验结果
        /// </summary>
        /// <param name="tablename">表名</param>
        /// <param name="result">校验结果key为期号 value为结果 1为成功0为失败</param>
        /// <returns></returns>
        public int ExecuteResult(string tablename, Dictionary<int, int> result)
        {
            try
            {
                if (result.Count == 0) return 1;
                var resultok = result.Where(w => w.Value == 1).Select(w => w.Key).ToArray();
                var resultall = result.Select(w => w.Key).ToArray(); ;
                string okstr = string.Join(",", resultok);
                string allstr = string.Join(",", resultall);
                string sql = string.Format(ResultSQL, tablename, okstr, allstr);
                int rt = SqlHelper.ExecuteNonQuery(CommandType.Text, sql, null);
                return rt;
            }
            catch (Exception ee)
            {
                return -2;
            }
        }

        private string ResultSQL = @"update {0} set IsChecked=1 ,IsPassed=( case when Term in({1}) then 1 else  0 end) where Term in ({2})";

    }
}
