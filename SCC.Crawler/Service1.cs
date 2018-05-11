using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace SCC.Crawler
{
    public partial class Service1 : ServiceBase
    {
        private JobManage manage = null;
        public Service1()
        {
            InitializeComponent();
            manage = new JobManage();
        }

        protected override void OnStart(string[] args)
        {
            manage.JobStart();
        }

        protected override void OnStop()
        {
            manage.JobStop();
        }
    }
}
