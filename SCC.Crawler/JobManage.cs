using Quartz;
using Quartz.Impl;
using SCC.Common;
using SCC.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace SCC.Crawler
{
    /// <summary>
    /// 作业管理类
    /// </summary>
    public class JobManage
    {
        private IScheduler _sched = null;
        /// <summary>
        /// 开始所有作业调度
        /// </summary>
        public void JobStart()
        {
            string configFile = AppDomain.CurrentDomain.BaseDirectory + "/SCCConfig.xml";
            List<SCCConfig> configs = CommonHelper.ConvertXMLToObject<SCCConfig>(configFile, "SCCSettings");

            var properties = new NameValueCollection
            {
                ["author"] = "同鑫网络"
            };
            ISchedulerFactory sf = new StdSchedulerFactory(properties);
            _sched = sf.GetScheduler();

            List<Type> allInheirtFromIJob = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IJob))).ToList();
            foreach (SCCConfig config in configs)
            {
                Type jobType = allInheirtFromIJob.FirstOrDefault(s => s.Name == config.JobName);
                if (jobType == null) continue;

                IJobDetail job = JobBuilder.Create(jobType)
                                    .WithIdentity(config.JobIdentityName, config.JobGroup)
                                    .Build();
                ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                                    .WithIdentity(config.TriggerIdentityName, config.JobGroup)
                                    .WithCronSchedule(config.CronExpression)
                                    .Build();

                foreach (PropertyInfo property in typeof(SCCConfig).GetProperties())
                {
                    job.JobDataMap.Put(property.Name, property.GetValue(config, null));
                }

                DateTimeOffset ft = _sched.ScheduleJob(job, trigger);
                Trace.WriteLine(ft.DateTime);
            }
            _sched.Start();
        }

        /// <summary>
        /// 停止所有作业调度
        /// </summary>
        public void JobStop()
        {
            if (_sched != null && _sched.IsStarted)
                _sched.Shutdown(true);
        }
    }
}
