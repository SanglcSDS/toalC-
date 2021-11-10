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
        private static string cashRequest = ConfigurationManager.AppSettings["cashRequest"];
        private static string cashTake = ConfigurationManager.AppSettings["cashTake"];
        private static string transNo = ConfigurationManager.AppSettings["transNo"];
        private static string dateTime = ConfigurationManager.AppSettings["dateTime"];

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
                        string fileName = e.Name.Remove(e.Name.Length - 4);
                        List<TextLine> listlisTextLine = listDetailText(e.FullPath, fileName, cashRequest, cashTake);
                        List<DetailText> listlisDetailText = listDetailText(listlisTextLine, fileName, transNo, dateTime, cashRequest, cashTake);
                        camera(listlisDetailText, cam1, ouputCam1, fileName);
                        camera(listlisDetailText, cam2, ouputCam2, fileName);
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
                Logger.Log(string.Format("The process failed: {0}", ex.StackTrace));
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                _filewatcher.Changed += _filewatcher_Changed;
                _filewatcher.EnableRaisingEvents = true;
            }



        }

        public void camera(List<DetailText> listlisDetailText, string camera, string ouputCam, string fileName)
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


        }

        /* tìm kiếm danh sánh ảnh trong khoảng thời gian*/
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
                    DateTime currentDate = DateTime.ParseExact(arrListStr[1], "yyyyMMddHHmmss", provider);
                    if (startDateTransaction <= currentDate && endDateDateTransaction >= currentDate)
                    {
                        itemDetail.setPathImage(filename);
                        listDetailImage.Add(itemDetail);
                    }


                }

            }
            return listDetailImage;

        }
        /*ghi chử vào ảnh*/





        public static void textToImage(string partImage, string pasrtSave, DetailText itemText)
        {


            try
            {


                Image img;

                using (var bmpTemp = new Bitmap(partImage))
                using (Font brFore = new Font("Arial", 10, FontStyle.Bold))
                {
                    img = new Bitmap(bmpTemp);
                    string Cassette = "";
                    if (itemText.getCassette().Length >= 8)
                    {
                        string itemcassette = itemText.getCassette();
                        Cassette = "1:" + itemcassette.Substring(0, 2) + "; 2:" + itemcassette.Substring(2, 2) + "; 3:" + itemcassette.Substring(4, 2) + "; 4:" + itemcassette.Substring(6, 2);

                    }

                    Graphics graphicImage = Graphics.FromImage(img);
                    StringFormat stringformat1 = new StringFormat();
                    stringformat1.Alignment = StringAlignment.Far;
                    Color stringColor1 = ColorTranslator.FromHtml("#e3e22d");
                    Color stringColor2 = ColorTranslator.FromHtml("#000000");
                    graphicImage.DrawImage(ImageFromText(Cassette, brFore, stringColor1, stringColor2, 4), new Point(5, 30));
                    graphicImage.DrawImage(ImageFromText(itemText.getCurrentDate(), brFore, stringColor1, stringColor2, 4), new Point(1100, 690));
                    graphicImage.DrawImage(ImageFromText("Trans No: " + itemText.getTransNo(), brFore, stringColor1, stringColor2, 4), new Point(600, 690));


                    img.Save(pasrtSave);

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

        /*lấy ra danh sách text */


        public static List<TextLine> listDetailText(string fullPath, string name, string CashRequest, string CashTake)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            List<TextLine> listTextLine = new List<TextLine>();
            List<string> listString = new List<string>();
            TextLine detailitem = new TextLine();
            if (nameText == null)
            {
                nameText = name;
            }
            if (!nameText.Equals(name))
            {

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

                        indexline = indexline + 1;
                        line = reader.ReadLine();

                        if (line.Contains(CashRequest))
                        {
                            detailitem.setIndexLineStart(indexline);
                            listString.Add(line);
                        }
                        if (listString.Count > 0)
                        {
                            listString.Add(line);
                        }
                        if (line.Contains(CashTake))
                        {
                            detailitem.setIndexLineEnd(indexline);
                            detailitem.setLine(listString);
                            listTextLine.Add(detailitem);
                            detailitem = new TextLine();
                            listString = new List<string>();
                        }
                    }


                }
            }

            return listTextLine;
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

