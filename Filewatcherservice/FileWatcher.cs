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
                        List<TextLine> listlisDetailText = listDetailText(e.FullPath, e.Name, "CASH REQUEST:", "CASH TAKEN");
                        // TextLine,string namFile, string cassette, string transNo, string dateTime, string CashRequest, string CashTake
                        List<DetailText> listlisDetailTe= listDetailText(listlisDetailText,e.Name, "TRANS NO", "DATE  TIME", "CASH REQUEST:", "CASH TAKEN");
                        //  camera(listlisDetailText, cam1, ouputCam1, e.Name);
                        //  camera(listlisDetailText, cam2, ouputCam2, e.Name);
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
        /*ghi chử vào ảnh*/





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
                    Color stringColor2 = ColorTranslator.FromHtml("#000000");
                    graphicImage.DrawImage(ImageFromText(itemText.getCassette(), new Font("Tahoma", 10, FontStyle.Bold), stringColor1, stringColor2, 4), new Point(5, 30));
                    graphicImage.DrawImage(ImageFromText(itemText.getCurrentDate(), new Font("Tahoma", 10, FontStyle.Bold), stringColor1, stringColor2, 4), new Point(1150, 660));
                    graphicImage.DrawImage(ImageFromText(itemText.getCurrentTime(), new Font("Tahoma", 10, FontStyle.Bold), stringColor1, stringColor2, 4), new Point(1150, 690));
                    graphicImage.DrawImage(ImageFromText("Trans No: " + itemText.getTransNo(), new Font("Tahoma", 10, FontStyle.Bold), stringColor1, stringColor2, 4), new Point(600, 690));


                    img.Save(pasrtSave);

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("The process failed: {0}", ex.ToString()));
                Logger.Log(string.Format("The process failed: {0}", ex.ToString()));

            }
        }
        public static Image ImageFromText(string strText, Font fnt, Color clrFore, Color clrBack, int blurAmount)
        {
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


        public static List<DetailText> listDetailText(List<TextLine> TextLine,string namFile, string transNo, string dateTime, string CashRequest, string CashTake)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            List<DetailText> listDetail = new List<DetailText>();
           
            foreach (TextLine itemTextLine in TextLine)
            {
                DetailText detail = new DetailText();

                foreach (string itemline in itemTextLine.getLine())
                {
                   

                    if (itemline.Contains(CashRequest))
                    {
                        string itemcassette = itemline.Substring(itemline.LastIndexOf(@":") + 1);

                        detail.setStartTime(DateTime.ParseExact(namFile.Remove(namFile.Length - 4) + itemline.Substring(0, 8), "yyyyMMddHH:mm:ss", provider));
                     
                        detail.setCassette("1:" + itemcassette.Substring(0, 2) + "; 2:" + itemcassette.Substring(2, 2) + "; 3:" + itemcassette.Substring(4, 2) + "; 4:" + itemcassette.Substring(6, 2));

                    }
                    if (itemline.Contains(transNo))
                    {
                        detail.setTransNo(itemline.Substring(itemline.LastIndexOf(@":") + 1));
                    }
                    if (itemline.Contains(dateTime))
                    {
                        string itemdateTime = itemline.Substring(itemline.LastIndexOf(@":") + 1);

                        detail.setCurrentDate(itemline.Substring(itemdateTime.IndexOf(@" ")));
                        detail.setCurrentTime(itemline.Substring(itemdateTime.LastIndexOf(@" ")));
                    }
                    if (itemline.Contains(CashTake))
                    {
                        detail.setEndTime(DateTime.ParseExact(namFile.Remove(namFile.Length - 4) + itemline.Substring(0, 8), "yyyyMMddHH:mm:ss", provider));
                     
                    }


                }
                listDetail.Add(detail);

            }

            return listDetail;
        }

        /*lấy ra danh sách text */
        /*    public static List<DetailText> listDetailText(string fullPath, string name)
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
                               string test = line.Substring(line.LastIndexOf(@":") + 1);
                                Console.WriteLine(test);

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
            }*/

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

            //  Thread.Sleep(300);
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

