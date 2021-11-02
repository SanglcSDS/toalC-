using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Drawing;
using System.Globalization;

namespace Filewatcherservice
{
    public class FileWatcher
    {

        private FileSystemWatcher _filewatcher;
        private static string partInputTexts = ConfigurationManager.AppSettings["partInputTexts"];
        private static string partInputImage = ConfigurationManager.AppSettings["partInputImage"];
        public FileWatcher()
        {
            PathLocation(partInputImage);
            _filewatcher = new FileSystemWatcher(PathLocation(partInputTexts));
            _filewatcher.Changed += new FileSystemEventHandler(_filewatcher_Changed);
            //  _filewatcher.Created += new FileSystemEventHandler(_filewatcher_Created);
            //  _filewatcher.Renamed += new FileSystemEventHandler(_filewatcher_Renamed);
            //  _filewatcher.Deleted += new FileSystemEventHandler(_filewatcher_Deleted);
            _filewatcher.EnableRaisingEvents = true;
        }


 
        void _filewatcher_Changed(object sender, FileSystemEventArgs e)
        {


            try
            {
                _filewatcher.Changed -= _filewatcher_Changed;

                _filewatcher.EnableRaisingEvents = false;

                if (IsPhototext(e.Name))
                {
                    foreach (DetailText itemText in listDetailText(e.FullPath))
                    {


                        foreach (DetailImage itemimage in listImageTransaction(partInputImage, itemText.getStartTime(), itemText.getEndTime()))
                        {
                            string getFilName = Path.GetFileName(itemimage.getPathImage());
                            string pasrtSave = partInputImage + getFilName.Remove(getFilName.Length - 4) + "_" + itemText.getTransNo() + ".jpg";
                            AddTextUatermark(itemimage.getPathImage(), pasrtSave, itemText.getCurrentTime(), itemText.getCurrentDate(), itemText.getCassetteo(), itemText.getTransNo());

                        }
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

        public static List<DetailImage> listImageTransaction(string partInputImage, DateTime startDateTransaction, DateTime endDateDateTransaction)
        {
            List<DetailImage> listDetailImage = new List<DetailImage>();
            foreach (string filename in listFileText(partInputImage))
            {
                if (filename != null)
                {
                    DetailImage itemDetail = new DetailImage();
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    string getFilName = Path.GetFileName(filename);
                    string[] arrListStr = getFilName.Split(new char[] { '_' });
                    itemDetail.setTerminalIdy(arrListStr[0]);
                    DateTime currentDate = DateTime.ParseExact(arrListStr[1], "yyyyMMddHHmmss", provider);
                    if (startDateTransaction <= currentDate && endDateDateTransaction >= currentDate)
                    {
                        itemDetail.setCurrentDate(DateTime.ParseExact(arrListStr[1], "yyyyMMddHHmmss", provider));
                        itemDetail.setPathImage(filename);
                        itemDetail.setCamera(arrListStr[2]);
                        string[] description = arrListStr[3].Split(new char[] { '-' });
                        itemDetail.setDescriptioItem(description[0]);
                        if (IsPhoto(arrListStr[4]))
                        {
                            itemDetail.setDescription(description[1] + "_" + arrListStr[4].Remove(arrListStr[4].Length - 4));
                        }
                        else
                        {
                            itemDetail.setDescription(description[1] + "_" + arrListStr[4]);
                            itemDetail.setCardNo(arrListStr[5].Remove(arrListStr[5].Length - 4));

                        }
                        listDetailImage.Add(itemDetail);
                    }

                }


            }
            return listDetailImage;

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

        public static List<TransactionItem> getListTransactionItem(string path)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            List<TransactionItem> list = new List<TransactionItem>();
            string getFilName = Path.GetFileName(path);
            DetailText itemDetailText = new DetailText();
            TransactionItem transactionItem = new TransactionItem();
            foreach (var line in File.ReadAllLines(path))
            {
              
                if ( line.Contains("TRANSACTION START"))
                {
                    transactionItem.setStartTime(DateTime.ParseExact(getFilName.Remove(getFilName.Length - 4) + line.Substring(0, 8), "yyyyMMddHH:mm:ss", provider));
                }
                if (itemDetailText.get)
                {
                    itemDetailText.setStartTime(DateTime.ParseExact(getFilName.Remove(getFilName.Length - 4) + line.Substring(0, 8), "yyyyMMddHH:mm:ss", provider));
                }



                if( line.Contains("TRANSACTION END")){
                    transactionItem.setEndTime(DateTime.ParseExact(getFilName.Remove(getFilName.Length - 4) + line.Substring(0, 8), "yyyyMMddHH:mm:ss", provider));
                    list.Add(transactionItem);
                }
               
                
            }
            return list;
        }
        public static List<DetailText> listDetailText(string fullPath)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            string getFilName = Path.GetFileName(fullPath);
            List<DetailText> listDetail = new List<DetailText>();
            DetailText detail = new DetailText();
            foreach (var line in File.ReadAllLines(fullPath))
            {
                if (line.Contains("TRANSACTION START"))
                {
                    detail.setStartTime(DateTime.ParseExact(getFilName.Remove(getFilName.Length - 4) + line.Substring(0, 8), "yyyyMMddHH:mm:ss", provider));
                }

                if (line.Contains("CASH REQUEST:"))
                {
                    string cassette = line.Remove(0, 23);
                    detail.setCassette("1:" + cassette.Substring(0, 2) + "; 2:" + cassette.Substring(2, 2) + "; 3:" + cassette.Substring(4, 2) + "; 4:" + cassette.Substring(6, 2));

                }
                if (detail.getCassetteo() != null)
                {
                    if (line.Contains("TRANS NO"))
                    {
                        detail.setTransNo(line.Remove(0, 14));
                    }
                    if (line.Contains("DATE  TIME"))
                    {
                        string dateTime = line.Substring(13, 20);
                        detail.setCurrentDate(dateTime.Substring(1, 10));
                        detail.setCurrentTime(dateTime.Substring(12, 8));
                    }
                }

                if (line.Contains("TRANSACTION END"))
                {
                    detail.setEndTime(DateTime.ParseExact(getFilName.Remove(getFilName.Length - 4) + line.Substring(0, 8), "yyyyMMddHH:mm:ss", provider));
                    listDetail.Add(detail);


                }

            }

            return listDetail;
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


        public static List<string> listFileText(string path)

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

