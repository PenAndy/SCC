using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCC.Common
{
 public static   class CheckDataHelper
    {
        private IEmail email = null;
        static CheckDataHelper()
        {
            email = IOC.Resolve<IEmail>();

        }
      
        /// <summary>
        /// 纠错参数
        /// </summary>
        /// <param name="dbData"></param>
        /// <param name="newData"></param>
        private  static void CheckData(Dictionary<string, string> dbData, Dictionary<string, string> newData)
        {
            try
            {
                foreach (var item in dbData)
                {
                    var data = newData.SingleOrDefault(w => w.Key == item.Key);
                    if (data.Key != null)
                    {
                        if (data.Value != item.Value)
                        {
                            email.AddEmail(Config.Area + Config.LotteryName, data.Key, DateTime.Now, "数据验证失败");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //log.Error(this.GetType(), string.Format("【{0}】数据验证发生错误，错误信息【{1}】", Config.Area + Config.LotteryName, ex.Message));
                //报错处理
            }
        }

    }
}
