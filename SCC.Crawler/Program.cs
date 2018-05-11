using System.ServiceProcess;

namespace SCC.Crawler
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
               {
                   new Service1()
               };
            ServiceBase.Run(ServicesToRun);

            //JobManage job = new JobManage();
            //job.JobStart();

            //DF6J1Job job = new DF6J1Job();
            //job.ConvertShengXiaoToNumber("[\r\n  \"鸡\"\r\n]");


            //OpenCaiApiArg arg = new OpenCaiApiArg
            //{
            //    code = EnumHelper.GetLotteryCode(SCCLottery.AnHui22x5),
            //    rows = 20
            //};

            //OpenCaiApiServices.GetOpenCaiApiData(arg);
        }
    }
}
