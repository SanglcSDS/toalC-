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
        Queue<string> queueFullPath = new Queue<string>();
        private static string INPUT_TEXT = ConfigurationManager.AppSettings["text"];
        private static string DELAY_MINUTE = ConfigurationManager.AppSettings["delayMinutes"];
        private static string TEST = ConfigurationManager.AppSettings["test"];
        private static string nameText = null;
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
            // timer.Interval = (int)TimeSpan.FromMinutes(Int32.Parse(DELAY_MINUTE)).TotalMilliseconds;
            timer.Interval = 200;
            timer.Elapsed += timer_Elapsed;
            timer.Start();

        }
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            Console.WriteLine(e.SignalTime);
            string date = e.SignalTime.ToString("yyyyMMdd");
            if (nameText == null || date.Equals(nameText))
            {


                if (string.IsNullOrEmpty(TEST))
                {
                    nameText = date;
                    queueFullPath.Enqueue(date);
                   
                }
                else
                {
                    nameText = TEST;
                    queueFullPath.Enqueue(TEST);
                }

            }

            else
            {

                if (string.IsNullOrEmpty(TEST))
                {
                   
                    queueFullPath.Enqueue(nameText);
                    queueFullPath.Enqueue(date);
                     nameText = date;
                }
                else
                {
                   
                    queueFullPath.Enqueue(nameText);
                    queueFullPath.Enqueue(TEST);
                    nameText = TEST;

                }



            }


            while (queueFullPath.Count > 0)
            {
                string fileName = (string)queueFullPath.Peek();
                string fileNamejrn = "//" + fileName + ".jrn";
                if (File.Exists(INPUT_TEXT + fileNamejrn))
                {

                    FileWatcher f = new FileWatcher();
                    f.fileWatcher(fileName);

                    if (queueFullPath.Count > 0)
                    {
                        queueFullPath.Dequeue();

                    }

                }


            }
        }


        protected override void OnStop()
        {
        }
    }
}
