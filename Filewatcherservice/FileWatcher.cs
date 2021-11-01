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
        static string datetransaction = "";
        static string timetransaction = "";
        static string cassette = "";
        static string transNo = "";

        public FileWatcher()
        {
            //   string partOutputImage = ConfigurationManager.AppSettings["partOutputImage"];
            string partInputTexts = ConfigurationManager.AppSettings["partInputTexts"];
            string partInputImage = ConfigurationManager.AppSettings["partInputImage"];
            PathLocation(partInputImage);
            //  PathLocation(partOutputImage);
            _filewatcher = new FileSystemWatcher(PathLocation(partInputTexts));
            _filewatcher.Changed += new FileSystemEventHandler(_filewatcher_Changed);
            //  _filewatcher.Created += new FileSystemEventHandler(_filewatcher_Created);
            //  _filewatcher.Renamed += new FileSystemEventHandler(_filewatcher_Renamed);
            //  _filewatcher.Deleted += new FileSystemEventHandler(_filewatcher_Deleted);
            _filewatcher.EnableRaisingEvents = true;
        }
        List<string> listFileText(string path)

        {
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            List<string> imageFiles = new List<string>();
            foreach (string filename in files)
            {
                if (IsPhoto(filename))
                {
                    imageFiles.Add(filename);
                }

            }
            return imageFiles;
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
                _filewatcher.Changed -= _filewatcher_Changed;

                _filewatcher.EnableRaisingEvents = false;

                if (IsPhototext(e.Name))
                {
                    //    string partOutputImage = ConfigurationManager.AppSettings["partOutputImage"];
                    string partInputTexts = ConfigurationManager.AppSettings["partInputTexts"];
                    string partInputImage = ConfigurationManager.AppSettings["partInputImage"];

                    List<DetailText> listDetail = listDetailText(e.FullPath);
               




                    foreach (string filename in listFileText(partInputImage))
                    {

                        string getFilName = Path.GetFileName(filename);


                        AddTextUatermark(filename, partInputImage + getFilName.Remove(getFilName.Length - 4) + "_" + transNo + ".jpg", timetransaction, datetransaction, cassette, transNo);
                        FileInfo filetodelete = new FileInfo(filename);
                        filetodelete.Delete();

                    }

                }
                else
                {
                    Logger.Log(string.Format(_filewatcher.Path));
                }
                Logger.Log(string.Format("File:{0} Changed at time:{1}", e.Name, DateTime.Now.ToLocalTime()));
                Console.WriteLine($"File Changed. Name: {e.Name}");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            finally
            {
                _filewatcher.Changed += _filewatcher_Changed;
                _filewatcher.EnableRaisingEvents = true;
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
        static void AddTextUatermark(string partImage, string pasrtSave, string timetransaction, string datetransaction, string cassette, string transNo)
        {
            try
            {

                Image img;
                using (var bmpTemp = new Bitmap(partImage))
                {
                    img = new Bitmap(bmpTemp);

                    Graphics graphicImage = Graphics.FromImage(img);
                    StringFormat stringformat1 = new StringFormat();
                    stringformat1.Alignment = StringAlignment.Far;
                    Color stringColor1 = ColorTranslator.FromHtml("#fbfe47");
                    graphicImage.DrawString(cassette, new Font("arail", 12, FontStyle.Bold), new SolidBrush(stringColor1), new Point(5, 30));
                    graphicImage.DrawString(datetransaction, new Font("arail", 12, FontStyle.Bold), new SolidBrush(stringColor1), new Point(1150, 660));
                    graphicImage.DrawString(timetransaction, new Font("arail", 12, FontStyle.Bold), new SolidBrush(stringColor1), new Point(1150, 690));
                    graphicImage.DrawString("Trans No: " + transNo, new Font("arail", 12, FontStyle.Bold), new SolidBrush(stringColor1), new Point(600, 690));

                    img.Save(pasrtSave);
                    //img.dí

                }


                //  

            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("The process failed: {0}", ex.ToString()));

            }
        }
        public static List<DetailText> listDetailText(string fullPath )
        {
            List<DetailText> listDetail = new List<DetailText>();
            DetailText detail = new DetailText();
            foreach (var line in File.ReadAllLines(fullPath))
            {
                if (line.Contains("TRANSACTION START"))
                {
                    detail.setStartTime(line.Substring(0, 8));
                }
                if (line.Contains("TRANS NO"))
                {
                    //string aa = line.Substring(0, 8);
                    detail.setTransNo(line.Remove(0, 14));
                }
                if (line.Contains("DATE  TIME"))
                {
                    //string aa = line.Substring(0, 8);
                    detail.setTimeDay(line.Substring(13, 20));
                }

                if (line.Contains("CASH REQUEST:"))
                {
                    //string aa = line.Substring(0, 8);
                    detail.setCassette(line.Remove(0, 24));
                }


                if (line.Contains("TRANSACTION END"))
                {
                    detail.setEndTime(line.Substring(0, 8));
                    listDetail.Add(detail);


                }

            }

            return listDetail;
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

            return list;
        }

   

        public static bool IsPhototext(string fileName)
        {
            List<string> list = GetAllPhotosExtensiontext();
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
        public static List<string> GetAllPhotosExtensiontext()
        {
            List<string> list = new List<string>();
            list.Add(".jrn");
            return list;
        }


    }
}

