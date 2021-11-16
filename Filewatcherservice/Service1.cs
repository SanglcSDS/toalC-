using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Filewatcherservice
{
    public partial class Service1 : ServiceBase
    {
        static System.Timers.Timer t;
        public Service1()
        {
            InitializeComponent();
        }
        public void OnDebug()
        {
            OnStart(null);

        }

        protected override void OnStart(string[] args)
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 500;
            timer.Elapsed += timer_Elapsed;
            timer.Start();
           
        }
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            DetailImage itemDetail = new DetailImage();

            string date = e.SignalTime.ToString("yyyyMMdd");
            FileWatcher f = new FileWatcher(date);
        }

    
        protected override void OnStop()
        {
        }
    }
}
