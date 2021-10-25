using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace Filewatcherservice
{
    public class FileWatcher {

        private FileSystemWatcher _filewatcher;
        public FileWatcher() {
            _filewatcher = new FileSystemWatcher(PathLocation());
            _filewatcher.Changed += new FileSystemEventHandler(_filewatcher_Changed);
            _filewatcher.Created += new FileSystemEventHandler(_filewatcher_Created);
          //  _filewatcher.Renamed += new FileSystemEventHandler(_filewatcher_Renamed);
            _filewatcher.Deleted += new FileSystemEventHandler(_filewatcher_Deleted);
            _filewatcher.EnableRaisingEvents = true;
            _filewatcher.IncludeSubdirectories = true;
        }
        private string PathLocation()
        {
            string value = string.Empty;
            try {
                value = ConfigurationSettings.AppSettings["location"];

            } catch(Exception ex)
            {

            }
            return value;



        }
        void _filewatcher_Changed(object sender, FileSystemEventArgs e)
        {
            Logger.Log(string.Format("File:{0} Changed at time:{1}", e.Name, DateTime.Now.ToLocalTime()));

        }
        void _filewatcher_Created(object sender, FileSystemEventArgs e)
        {
            Logger.Log(string.Format("File:{0} Created at time:{1}", e.Name, DateTime.Now.ToLocalTime()));

        }
        void _filewatcher_Renamed(object sender, FileSystemEventArgs e)
        {
            Logger.Log(string.Format("File:{0} Renamed at time:{1}", e.Name, DateTime.Now.ToLocalTime()));

        }
        void _filewatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Logger.Log(string.Format("File:{0} Deleted at time:{1}", e.Name, DateTime.Now.ToLocalTime()));

        }

    }
}
