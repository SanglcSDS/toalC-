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
using System.Drawing.Drawing2D;
using System.Collections;

namespace Filewatcherservice
{
    public class FileWatcher
    {

        private FileSystemWatcher _filewatcher;
        private static string INPUT_TEXT = ConfigurationManager.AppSettings["text"];
        private static string CAM1 = ConfigurationManager.AppSettings["cam1"];
        private static string CAM2 = ConfigurationManager.AppSettings["cam2"];
        private static string OUPUT_CAM1 = ConfigurationManager.AppSettings["ouputCam1"];
        private static string OUPUT_CAM2 = ConfigurationManager.AppSettings["ouputCam2"];
        private static string CASH_REQUEST = ConfigurationManager.AppSettings["cashRequest"];
        private static string CASH_TAKEN = ConfigurationManager.AppSettings["cashTake"];
        private static string TRANS_NO = ConfigurationManager.AppSettings["transNo"];
        private static string DATE_TIME = ConfigurationManager.AppSettings["dateTime"];
        private static string TRANSACTION_START = ConfigurationManager.AppSettings["transactionStart"];
        private static string TRANSACTION_END = ConfigurationManager.AppSettings["transactionEnd"];
        private static string DELAY_SECONDS = ConfigurationManager.AppSettings["delaySeconds"];

        private static int indexline;
        private static string nameText = null;
        public FileWatcher()
        {
            PathLocation(CAM1);
            PathLocation(CAM2);
            PathLocation(OUPUT_CAM1);
            PathLocation(OUPUT_CAM2);
            PathLocation(INPUT_TEXT);
            _filewatcher = new FileSystemWatcher(INPUT_TEXT);
            _filewatcher.Changed += new FileSystemEventHandler(_filewatcher_Changed);
            //    _filewatcher.NotifyFilter = NotifyFilters.FileName;
            _filewatcher.EnableRaisingEvents = true;

        }

        Queue queueFullPath = new Queue();


        public void _filewatcher_Changed(object sender, FileSystemEventArgs e)
        {


            try
            {
                _filewatcher.Changed -= _filewatcher_Changed;
                _filewatcher.EnableRaisingEvents = false;


                if (IsTextJrn(e.Name))
                {


                    queueFullPath.Enqueue(e.FullPath);

                    int Length = queueFullPath.Count;
                    for (int i = 0; i < Length; i++)
                    {
                        string fullPath = (string)queueFullPath.Peek();


                        string name = fullPath.Substring(fullPath.LastIndexOf(@"\") + 1);
                        string fileName = name.Remove(name.Length - 4);
                        List<string> liststring = listDetailText(fullPath, fileName, TRANSACTION_START, TRANSACTION_END);
                        List<TextLine> listlisTextLine = textLine(liststring, CASH_REQUEST, CASH_TAKEN);
                        List<DetailText> listlisDetailText = listDetailText(listlisTextLine, fileName, TRANS_NO, DATE_TIME, CASH_REQUEST, CASH_TAKEN);
                        Thread t = new Thread(() =>
                        {
                            List<PartImage> listItemCamera1 = listItemCamera(listlisDetailText, CAM1, OUPUT_CAM1, fileName);
                            camera(listItemCamera1);
                            Logger.Log($"end camera1");
                        });
                        t.Start();
                       


                        Thread t1 = new Thread(() =>
                        {
                            List<PartImage> listItemCamera2 = listItemCamera(listlisDetailText, CAM2, OUPUT_CAM2, fileName);
                            camera(listItemCamera2);
                            Logger.Log($"end camera2" );
                        });
                        t1.Start();
                       



                        queueFullPath.Dequeue();

                    }



                }
                else
                {

                    Logger.Log(string.Format(_filewatcher.Path));
                }
                Logger.Log(string.Format("File:{0} Changed at time:{1}", e.Name, DateTime.Now.ToLocalTime()));
                Logger.Log($"File Changed. Name: {e.Name}" + " index line end:" + indexline);
                Logger.Log($"---------------------------------------------------------------");
                Console.WriteLine($"File Changed. Name: {e.Name}" + " index line:" + indexline);


            }
            catch (Exception ex)
            {

                Logger.Log(string.Format("The process failed: {0}", ex.ToString()));
                Logger.Log(string.Format("The process failed: {0}", ex.StackTrace));
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                _filewatcher.Changed += _filewatcher_Changed;
                _filewatcher.EnableRaisingEvents = true;
            }



        }

        public static List<PartImage> listItemCamera(List<DetailText> listlisDetailText, string camera, string ouputCam, string fileName)
        {
            List<PartImage> listDetailText = new List<PartImage>();
            foreach (DetailText itemText in listlisDetailText)

            {

                List<DetailImage> listImageTran = listImageTransaction(camera + fileName + @"\\", itemText.getStartTime(), itemText.getEndTime());

                foreach (DetailImage itemimage in listImageTran)
                {
                    PartImage itemDetailText = new PartImage();
                    string getFilName = Path.GetFileName(itemimage.getPathImage());

                    itemDetailText.setCassette(itemText.getCassette());
                    itemDetailText.setTransNo(itemText.getTransNo());
                    itemDetailText.setCurrentDateTime(itemText.getCurrentDate());
                    itemDetailText.setPathImage(itemimage.getPathImage());
                    itemDetailText.setPasrtSave(PathLocation(ouputCam + fileName + @"\\") + getFilName.Remove(getFilName.Length - 4) + @"_" + itemText.getTransNo() + @".jpg");
                    listDetailText.Add(itemDetailText);
                }
            }
            return listDetailText;


        }
        public void camera(List<PartImage> listItemCamera)

        {
            foreach (PartImage itemimage in listItemCamera)
            {
                Thread th_one = new Thread(() =>
                {
                    textToImage(itemimage);

                });
                th_one.Start();
                th_one.Join();
            }


        }
        /*     public void camerawwww(List<DetailText> listlisDetailText, string camera, string ouputCam, string fileName)
             {
                 try
                 {


                     foreach (DetailText itemText in listlisDetailText)

                     {

                         List<DetailImage> listImageTran = listImageTransaction(camera + fileName + @"\\", itemText.getStartTime(), itemText.getEndTime());

                         foreach (DetailImage itemimage in listImageTran)
                         {


                             string getFilName = Path.GetFileName(itemimage.getPathImage());

                             string pasrtSave = PathLocation(ouputCam + fileName + @"\\") + getFilName.Remove(getFilName.Length - 4) + @"_" + itemText.getTransNo() + @".jpg";

                             textToImage(itemimage.getPathImage(), pasrtSave, itemText);

                         }
                     }
                 }
                 catch (Exception ex)
                 {
                     Logger.Log(string.Format("The process failed: {0}", ex.StackTrace));
                     Logger.Log(string.Format("The process failed: {0}", ex.ToString()));
                     Console.WriteLine(ex.ToString());
                 }


             }*/


        /* tìm kiếm danh sánh ảnh trong khoảng thời gian*/
        public static List<DetailImage> listImageTransaction(string partInputImage, DateTime startDateTransaction, DateTime endDateDateTransaction)
        {
            List<DetailImage> listDetailImage = new List<DetailImage>();
            try
            {

                //  List<string> listFilename = listNameFileImage(partInputImage);
              

                var files = Directory.GetFiles(partInputImage, "*.*", SearchOption.AllDirectories);
                if (files != null)
                {
                    Console.WriteLine("-------------------------------------------------------------------------");
                    foreach (string filename in files)
                    {
                        Thread th_one = new Thread(() =>
                        {
                            if (filename != null)
                            {


                                DetailImage itemDetail = new DetailImage();
                                CultureInfo provider = CultureInfo.InvariantCulture;
                                string getFilName = Path.GetFileName(filename);

                                string[] arrListStr = getFilName.Split(new char[] { '_' });
                                DateTime currentDate = DateTime.ParseExact(arrListStr[1], "yyyyMMddHHmmss", provider);
                                if (startDateTransaction <= currentDate && endDateDateTransaction >= currentDate)
                                {
                                  
                                    Thread.Sleep(TimeSpan.FromSeconds(Int32.Parse(DELAY_SECONDS)));
                                    Console.WriteLine(filename);
                                    itemDetail.setPathImage(filename);
                                    Logger.Log(string.Format(filename));
                                    listDetailImage.Add(itemDetail);
                                }




                            }
                        });
                        th_one.Start();
                        th_one.Join();

                    }
                }


               


            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("The process failed: {0}", ex.StackTrace));
                Logger.Log(string.Format("The process failed: {0}", ex.ToString()));
                Console.WriteLine(ex.ToString());
                
            }
            return listDetailImage;
        }
        /*ghi chử vào ảnh*/


        private static void ThreadOne(string filename)
        {
            Thread.Sleep(5000);
            Console.WriteLine("ThreadOne");
        }


        public static void textToImage(PartImage itemPartImage)
        {


            try
            {


                Image img;

                using (var bmpTemp = new Bitmap(itemPartImage.getPathImage()))
                using (Font brFore = new Font("Arial", 10, FontStyle.Bold))
                {
                    img = new Bitmap(bmpTemp);
                    string Cassette = "";
                    if (itemPartImage.getCassette().Length >= 8)
                    {
                        string itemcassette = itemPartImage.getCassette();
                        Cassette = "1:" + itemcassette.Substring(0, 2) + "; 2:" + itemcassette.Substring(2, 2) + "; 3:" + itemcassette.Substring(4, 2) + "; 4:" + itemcassette.Substring(6, 2);

                    }

                    Graphics graphicImage = Graphics.FromImage(img);
                    StringFormat stringformat1 = new StringFormat();
                    stringformat1.Alignment = StringAlignment.Far;
                    Color stringColor1 = ColorTranslator.FromHtml("#e3e22d");
                    Color stringColor2 = ColorTranslator.FromHtml("#000000");

                    graphicImage.DrawImage(ImageFromText(Cassette, brFore, stringColor1, stringColor2, 4), new Point(5, 30));
                    graphicImage.DrawImage(ImageFromText(itemPartImage.getCurrentDateTime(), brFore, stringColor1, stringColor2, 4), new Point(1100, 690));
                    graphicImage.DrawImage(ImageFromText("Trans No: " + itemPartImage.getTransNo(), brFore, stringColor1, stringColor2, 4), new Point(600, 690));


                    img.Save(itemPartImage.getPasrtSave());

                }

            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("The process failed: {0}", ex.StackTrace));
                Console.WriteLine(string.Format("The process failed: {0}", ex.ToString()));
                Logger.Log(string.Format("The process failed: {0}", ex.ToString()));

            }
        }
        public static Image ImageFromText(string strText, Font fnt, Color clrFore, Color clrBack, int blurAmount)
        {
            if (strText is null)
                strText = "#";

            Bitmap bmpOut = null;

            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                SizeF sz = g.MeasureString(strText, fnt);
                using (Bitmap bmp = new Bitmap((int)sz.Width, (int)sz.Height))
                using (Graphics gBmp = Graphics.FromImage(bmp))
                using (SolidBrush brBack = new SolidBrush(Color.FromArgb(clrBack.R, clrBack.G, clrBack.B)))
                using (SolidBrush brFore = new SolidBrush(clrFore))
                {
                    gBmp.SmoothingMode = SmoothingMode.HighQuality;
                    gBmp.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    gBmp.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                    gBmp.DrawString(strText, fnt, brBack, 0, 0);

                    // make bitmap we will return.
                    bmpOut = new Bitmap(bmp.Width + blurAmount, bmp.Height + blurAmount);
                    using (Graphics gBmpOut = Graphics.FromImage(bmpOut))
                    {
                        gBmpOut.SmoothingMode = SmoothingMode.HighQuality;
                        gBmpOut.InterpolationMode = InterpolationMode.HighQualityBilinear;
                        gBmpOut.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                        // smear image of background of text about to make blurred background "halo"
                        for (int x = 0; x <= blurAmount; x++)
                            for (int y = 0; y <= blurAmount; y++)
                                gBmpOut.DrawImageUnscaled(bmp, x, y);

                        // draw actual text
                        gBmpOut.DrawString(strText, fnt, brFore, blurAmount / 2, blurAmount / 2);
                    }
                }
            }

            return bmpOut;
        }


        public static List<DetailText> listDetailText(List<TextLine> TextLine, string fileName, string transNo, string dateTime, string cashRequest, string cashTake)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            List<DetailText> listDetail = new List<DetailText>();

            foreach (TextLine itemTextLine in TextLine)
            {
                DetailText detail = new DetailText();

                foreach (string itemline in itemTextLine.getLine())
                {
                    if (itemline.Contains(cashRequest))
                    {
                        detail.setStartTime(DateTime.ParseExact(fileName + itemline.Substring(0, 8), "yyyyMMddHH:mm:ss", provider));
                        detail.setCassette(itemline.Substring(itemline.LastIndexOf(@":") + 1).Replace(@"\", @"").Trim());

                    }
                    if (itemline.Contains(transNo))
                    {
                        detail.setTransNo(itemline.Substring(itemline.LastIndexOf(@":") + 1).Replace(@"\", @"").Trim());

                    }
                    if (itemline.Contains(dateTime))
                    {

                        detail.setCurrentDate(itemline.Substring(itemline.LastIndexOf(@": ") + 1).Replace(@"\", @"").Trim());
                    }
                    if (itemline.Contains(cashTake))
                    {
                        detail.setEndTime(DateTime.ParseExact(fileName + itemline.Substring(0, 8), "yyyyMMddHH:mm:ss", provider));

                    }

                }
                listDetail.Add(detail);

            }

            return listDetail;
        }

        public static List<TextLine> textLine(List<string> text, string textStart, string textEnd)
        {
            List<TextLine> listTextLine = new List<TextLine>();
            List<string> listString = new List<string>();
            TextLine textLine = new TextLine();
            string checkItemtext = null;
            foreach (string itemtext in text)
            {
                if (itemtext.Contains(textStart))
                {
                    textLine.setTextStart(itemtext);
                    checkItemtext = itemtext;

                }
                if (checkItemtext != null)
                {
                    listString.Add(itemtext);
                }
                if (itemtext.Contains(textEnd))
                {
                    textLine.setTextEnd(itemtext);
                    checkItemtext = null;
                    textLine.setLine(listString);
                    listTextLine.Add(textLine);
                    listString = new List<string>();
                    textLine = new TextLine();
                }

            }



            return listTextLine;


        }
        public static List<string> listDetailText(string fullPath, string name, string textStart, string textEnd)
        {


            List<string> listString = new List<string>();

            if (nameText == null)
            {
                nameText = name;
            }
            if (!nameText.Equals(name))
            {

                nameText = name;
                indexline = 0;


            }
            Logger.Log($"File Changed. Name: {name}" + " index line start:" + indexline);

            using (var stream = new FileStream(path: fullPath, mode: FileMode.Open, access: FileAccess.ReadWrite, share: FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    for (int i = 1; i <= indexline; i++)
                    {

                        reader.ReadLine();
                    }
                    string line;
                    string itemtext = null;
                    while (!reader.EndOfStream)
                    {

                        indexline = indexline + 1;

                        line = reader.ReadLine();
                       
                        if (line.Contains(textStart))
                        {
                            itemtext = line;

                        }
                        if (itemtext != null)
                        {
                            listString.Add(line);
                        }
                        if (line.Contains(textEnd))
                        {
                            itemtext = null;
                        }

                    }


                }
            }

            return listString;
        }

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

