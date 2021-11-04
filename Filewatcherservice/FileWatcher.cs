using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Timers;
using System.Threading;

namespace Filewatcherservice
{
    public class FileWatcher
    {

        private FileSystemWatcher _filewatcher;
        private static string partInputTexts = ConfigurationManager.AppSettings["text"];
        private static string cam1 = ConfigurationManager.AppSettings["cam1"];
        private static string cam2 = ConfigurationManager.AppSettings["cam2"];
        private static string ouputCam1 = ConfigurationManager.AppSettings["ouputCam1"];
        private static string ouputCam2 = ConfigurationManager.AppSettings["ouputCam2"];
        private static int indexline;
        private static string nameText = null;
        public FileWatcher()
        {
            PathLocation(cam1);
            PathLocation(cam2);
            PathLocation(ouputCam1);
            PathLocation(ouputCam2);
            _filewatcher = new FileSystemWatcher(PathLocation(partInputTexts));
            _filewatcher.Changed += new FileSystemEventHandler(_filewatcher_Changed);
            _filewatcher.EnableRaisingEvents = true;

        }

        List<DetailText> listlisDetailText = new List<DetailText>();
        Queue<QueueName> queueName = new Queue<QueueName>();
        public void _filewatcher_Changed(object sender, FileSystemEventArgs e)
        {


            try
            {
                _filewatcher.Changed -= _filewatcher_Changed;

                _filewatcher.EnableRaisingEvents = false;
                Console.WriteLine("dòng thứ:" + indexline);

                if (IsTextJrn(e.Name))
                {
                    QueueName queueNameitem = new QueueName();
                    queueNameitem.setName(e.Name);
                    queueNameitem.setFullPath(e.FullPath);
                    queueName.Enqueue(queueNameitem);

                    while (queueName.Count > 0)
                    {
                        listlisDetailText = listDetailText(e.FullPath, e.Name);
                        camera(listlisDetailText, cam1, ouputCam1, e.Name);
                        camera(listlisDetailText, cam2, ouputCam2, e.Name);
                        queueName.Dequeue();

                    }

                }
                else
                {
                    Logger.Log(string.Format(_filewatcher.Path));
                }
                Logger.Log(string.Format("File:{0} Changed at time:{1}", e.Name, DateTime.Now.ToLocalTime()));
                Logger.Log($"File Changed. Name: {e.Name}" + " index line:" + indexline);
                Console.WriteLine($"File Changed. Name: {e.Name}" + " index line:" + indexline);


            }
            catch (Exception ex)
            {

                Logger.Log(string.Format("The process failed: {0}", ex.ToString()));
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                _filewatcher.Changed += _filewatcher_Changed;
                _filewatcher.EnableRaisingEvents = true;
            }



        }

        public void camera(List<DetailText> listlisDetailText, string camera, string ouputCam, string name)
        {
            try
            {

                foreach (DetailText itemText in listlisDetailText)

                {


                    List<DetailImage> listImageTran = listImageTransaction(camera + name.Remove(name.Length - 4) + "\\", itemText.getStartTime(), itemText.getEndTime());



                    foreach (DetailImage itemimage in listImageTran)
                    {
                        string getFilName = Path.GetFileName(itemimage.getPathImage());
                       
                            string pasrtSave = PathLocation(ouputCam + name.Remove(name.Length - 4) + "\\") + getFilName.Remove(getFilName.Length - 4) + "_" + itemText.getTransNo() + ".jpg";

                            textToImage(itemimage.getPathImage(), pasrtSave, itemText);
                        
                     


                    }
                }
            }
            catch (Exception ex)
            {

                Logger.Log(string.Format("The process failed: {0}", ex.ToString()));
                Console.WriteLine(ex.ToString());
            }


        }


        public static List<DetailImage> listImageTransaction(string partInputImage, DateTime startDateTransaction, DateTime endDateDateTransaction)
        {




            List<DetailImage> listDetailImage = new List<DetailImage>();

            List<string> listFilename = listNameFileImage(partInputImage);

            foreach (string filename in listFilename)
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

                        if (arrListStr.Length <= 4)
                        {
                         /*   itemDetail.setDescription(description[1].Remove(arrListStr[1].Length - 4));
                            itemDetail.setCardNo("");*/

                        }

                        else
                        {
                            if (IsImage(arrListStr[4]))
                            {
                                itemDetail.setDescription(description[1] + "_" + arrListStr[4].Remove(arrListStr[4].Length - 4));
                            }
                            else
                            {
                                itemDetail.setDescription(description[1] + "_" + arrListStr[4]);
                                itemDetail.setCardNo(arrListStr[5].Remove(arrListStr[5].Length - 4));

                            }

                        }

                        listDetailImage.Add(itemDetail);
                    }



                }



            }
            return listDetailImage;




        }
        public static void textToImage(string partImage, string pasrtSave, DetailText itemText)
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
                    Color stringColor1 = ColorTranslator.FromHtml("#e3e22d");
                    graphicImage.DrawString(itemText.getCurrentDate(), new Font("arail", 12, FontStyle.Bold), new SolidBrush(stringColor1), new Point(5, 30));
                    graphicImage.DrawString(itemText.getCurrentDate(), new Font("arail", 12, FontStyle.Bold), new SolidBrush(stringColor1), new Point(1150, 660));
                    graphicImage.DrawString(itemText.getCurrentTime(), new Font("arail", 12, FontStyle.Bold), new SolidBrush(stringColor1), new Point(1150, 690));
                    graphicImage.DrawString("Trans No: " + itemText.getTransNo(), new Font("arail", 12, FontStyle.Bold), new SolidBrush(stringColor1), new Point(600, 690));
                    img.Save(pasrtSave);

                }

            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("The process failed: {0}", ex.ToString()));

            }
        }
        public static List<DetailText> listDetailText(string fullPath, string name)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            List<DetailText> listDetail = new List<DetailText>();
            DetailText detail = new DetailText();
            if (nameText == null)
            {
                nameText = name;
            }
            if (!nameText.Equals(name))
            {
                File.Delete(fullPath);
                nameText = name;
                indexline = 0;

            }

           Thread.Sleep(300);
            using (var stream = new FileStream(path: fullPath, mode: FileMode.Open, access: FileAccess.ReadWrite, share: FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    for (int i = 1; i <= indexline; i++)
                    {

                        reader.ReadLine();
                    }
                    string line;

                    while (!reader.EndOfStream)
                    {

                        //   Console.WriteLine(reader.ReadLine());
                        indexline = indexline + 1;
                        line = reader.ReadLine();

                        if (line.Contains("CASH REQUEST:"))
                        {
                            detail.setStartTime(DateTime.ParseExact(name.Remove(name.Length - 4) + line.Substring(0, 8), "yyyyMMddHH:mm:ss", provider));
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
                        if (line.Contains("CASH TAKEN"))
                        {
                            detail.setEndTime(DateTime.ParseExact(name.Remove(name.Length - 4) + line.Substring(0, 8), "yyyyMMddHH:mm:ss", provider));
                            listDetail.Add(detail);
                            detail = new DetailText();
                        }
                    }


                }
            }

            return listDetail;
        }


        /*      public static List<DetailText> listDetailText(string fullPath, string name)
              {



                  using (var stream = new FileStream(path: fullPath, mode: FileMode.Open, access: FileAccess.ReadWrite, share: FileShare.ReadWrite))
                  {
                      using (StreamReader reader = new StreamReader(stream))
                      {


                          while (!reader.EndOfStream)
                          {

                              Console.WriteLine(reader.ReadLine());



                          }
                      }
                  }

                  return listDetail;
              }*/
        /*Hàm tạo mới folder*/
        public static string PathLocation(string value)
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
        /*Hàm kiểm tram imge file jpg*/
        public static bool IsImage(string fileName)
        {
            bool isThere = false;
            if (fileName.ToLower().EndsWith(".jpg"))
            {
                isThere = true;
            }
            return isThere;

        }


        /*Hàm tìm tên file image*/
        public static List<string> listNameFileImage(string path)

        {
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            List<string> imageFiles = new List<string>();
            foreach (string filename in files)
            {
                if (IsImage(filename))
                {
                    imageFiles.Add(filename);
                }

            }

            return imageFiles;
        }


        /*Hàm kiểm tra file text jrn*/
        public static bool IsTextJrn(string fileName)
        {
            bool isThere = false;
            if (fileName.ToLower().EndsWith(".jrn"))
            {
                isThere = true;
            }
            return isThere;

        }
    }

      
}

