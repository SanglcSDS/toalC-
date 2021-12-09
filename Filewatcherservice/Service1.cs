using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Filewatcherservice
{
    public partial class Service1 : ServiceBase
    {
        ///  Queue<string> queueFullPath = new Queue<string>();
        private static string INPUT_TEXT = ConfigurationManager.AppSettings["text"];
        private static string DELAY_MINUTE = ConfigurationManager.AppSettings["delayMinutes"];
        private static string TEST = ConfigurationManager.AppSettings["test"];

        FileWatcher f = new FileWatcher();
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
            timer.Interval = (int)TimeSpan.FromMinutes(Int32.Parse(DELAY_MINUTE)).TotalMilliseconds;
           //  timer.Interval = 200;
            timer.Elapsed += timer_Elapsed;
            timer.Start();

        }
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            string date = e.SignalTime.ToString("yyyyMMdd");
            Logger.Log($"File Changed. Name: {date + ".jrn"}" + " Timer:" + e.SignalTime);

            if (string.IsNullOrEmpty(TEST))
            {
                string fileNamejrn = "//" + date + ".jrn";
                if (File.Exists(INPUT_TEXT + fileNamejrn))
                {
                    f.fileWatcher(date);

                }


            }
            else
            {
                string fileNamejrn = "//" + TEST + ".jrn";
                if (File.Exists(INPUT_TEXT + fileNamejrn))
                {
                    f.fileWatcher(TEST);

                }

            }









        }


        protected override void OnStop()
        {
        }
    }
}
