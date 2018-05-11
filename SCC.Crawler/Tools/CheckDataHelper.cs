using SCC.Common;
using SCC.Interface;
using SCC.Models;
using SCC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCC.Crawler.Tools
{
    public class CheckDataHelper
    {
        private static LogHelper log = null;
        private static IEmail email = null;
        private static CheckResultServices resultServices = null;
        static CheckDataHelper()
        {
            email = IOC.Resolve<IEmail>();
            log = new LogHelper();
            resultServices = new CheckResultServices();
        }

        /// <summary>
        /// 纠错参数
        /// </summary>
        /// <param name="dbData"></param>
        /// <param name="newData"></param>
        public void CheckData(Dictionary<string, string> dbData, Dictionary<string, string> newData, string Area, SCCLottery Lottery)
        {
            try
            {
                Dictionary<int, int> result = new Dictionary<int, int>();
                foreach (var item in dbData)
                {
                    var data = newData.SingleOrDefault(w => w.Key == item.Key);
                    if (data.Key != null)
                    {
                        var dbdata = Array.ConvertAll<string, int>(data.Value.Split(','), s => int.Parse(s)).ToList();
                        var newdata = Array.ConvertAll<string, int>(item.Value.Split(','), s => int.Parse(s)).ToList();
                        dbdata.Sort();
                        newdata.Sort();
                        var dbcode = string.Join(",", dbdata);
                        var newcode = string.Join(",", newdata);
                        if (dbcode != newcode)
                        {
                            result.Add(int.Parse(item.Key), 0);
                            email.AddEmail(Area + Lottery.ToString(), data.Key, DateTime.Now, "数据验证失败" + string.Format("数据库:{0},爬取:{1}", item.Value, data.Value));
                        }
                        else
                        {
                            result.Add(int.Parse(item.Key), 1);
                        }
                    }
                }
                var tableName = EnumHelper.GetSCCLotteryTableName(Lottery);
                int rt = resultServices.ExecuteResult(tableName, result);
                if (rt < 0)
                {
                    email.AddEmail(Area + Lottery.ToString(), "0", DateTime.Now, "执行验证结果结果失败");
                }

            }
            catch (Exception ex)
            {
                log.Error(this.GetType(), string.Format("【{0}】数据验证发生错误，错误信息【{1}】", Area + Lottery.ToString(), ex.Message));
            }
        }
    }
}
