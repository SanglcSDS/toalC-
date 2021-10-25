using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Drawing;
namespace Filewatcherservice
{
    public class FileWatcher
    {

        private FileSystemWatcher _filewatcher;
        public FileWatcher()
        {
            string partInputImage = ConfigurationManager.AppSettings["partInputImage"];
            _filewatcher = new FileSystemWatcher(PathLocation(partInputImage));

            _filewatcher.Changed += new FileSystemEventHandler(_filewatcher_Changed);
            _filewatcher.Created += new FileSystemEventHandler(_filewatcher_Created);
            //  _filewatcher.Renamed += new FileSystemEventHandler(_filewatcher_Renamed);
            _filewatcher.Deleted += new FileSystemEventHandler(_filewatcher_Deleted);
            _filewatcher.EnableRaisingEvents = true;
            _filewatcher.IncludeSubdirectories = true;    
        }
        private string PathLocation(string value)
        {

            try
            {
                if (Directory.Exists(value))
                {

                    return value;
                }
                DirectoryInfo di = Directory.CreateDirectory(value);

            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("The process failed: {0}", ex.ToString()));

            }
            return value;



        }
        void _filewatcher_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (IsPhoto(e.Name))
                {
                    string partOutputImage = ConfigurationManager.AppSettings["partOutputImage"];
                    string partInputText = ConfigurationManager.AppSettings["partInputText"];
                    AddTextUatermark(_filewatcher.Path + e.Name, PathLocation(partInputText), PathLocation(partOutputImage) + e.Name);
                }
                else
                {
                    Logger.Log(string.Format(_filewatcher.Path));
                }
                Logger.Log(string.Format("File:{0} Changed at time:{1}", e.Name, DateTime.Now.ToLocalTime()));
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("The process failed: {0}", ex.ToString()));

            }

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
        static void AddTextUatermark(string partImage, string text, string pasrtSave)
        {
            try
            {
                Image bitmat = Bitmap.FromFile(partImage);
                Graphics graphicImage = Graphics.FromImage(bitmat);
                StringFormat stringformat1 = new StringFormat();
                stringformat1.Alignment = StringAlignment.Far;
                Color stringColor1 = ColorTranslator.FromHtml("#ff0000");

                string SO_TK = GetLine(text, 132);
                string SO_TIEN = GetLine(text, 133);
                string SO_DU = GetLine(text, 134);
                graphicImage.DrawString(SO_TK, new Font("arail", 10, FontStyle.Bold), new SolidBrush(stringColor1), new Point(1, 30));
                graphicImage.DrawString(SO_TIEN, new Font("arail", 10, FontStyle.Bold), new SolidBrush(stringColor1), new Point(1, 50));
                graphicImage.DrawString(SO_DU, new Font("arail", 10, FontStyle.Bold), new SolidBrush(stringColor1), new Point(1, 70));
                bitmat.Save(pasrtSave);
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("The process failed: {0}", ex.ToString()));

            }
        }
        static string GetLine(string fileName, int line)
        {
            using (var sr = new StreamReader(fileName))
            {
                for (int i = 1; i < line; i++)
                    sr.ReadLine();
                return sr.ReadLine();
            }
        }
        public static bool IsPhoto(string fileName)
        {
            List<string> list = GetAllPhotosExtensions();
            string filename = fileName.ToLower();
            bool isThere = false;
            foreach (string item in list)
            {
                if (filename.EndsWith(item))
                {
                    isThere = true;
                    break;
                }
            }
            return isThere;
        }
        public static List<string> GetAllPhotosExtensions()
        {
            List<string> list = new List<string>();
            list.Add(".jpg");
            list.Add(".png");
            list.Add(".bmp");
            list.Add(".jpeg");
            list.Add(".txt");
            return list;
        }


    }
}

