using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Timers;
using System.Threading;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Drawing.Imaging;

namespace Filewatcherservice
{

    public class FileWatcher
    {



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
        private static string TEXT_END = ConfigurationManager.AppSettings["listtexEnd"];
        private static string[] LIST_END = TEXT_END.Split(new char[] { ',' });
        Queue<List<DetailText>> queueDetailText = new Queue<List<DetailText>>();
        private static int indexline;
        private static int indexlinEnd;
        private static string nameText = null;

        public void fileWatcher(string fileName)
        {

            try
            {
                string date = "//" + fileName;
                string fileNamejrn = "//" + fileName + ".jrn";
                PathLocation(CAM1 + date);
                PathLocation(CAM2 + date);
                PathLocation(OUPUT_CAM1 + date);
                PathLocation(OUPUT_CAM2 + date);
                PathLocation(INPUT_TEXT);


                if (IsTextJrn(fileNamejrn))
                {
                    List<string> liststring = listDetailText(INPUT_TEXT + fileNamejrn, fileName, TRANSACTION_START, TRANSACTION_END);

                    List<TextLine> listlisTextLine = textLine(liststring, CASH_REQUEST, LIST_END);


                       List<DetailText> listlisDetailText = listDetailTexts(listlisTextLine, fileName, TRANS_NO);
                    queueDetailText.Enqueue(listlisDetailText);

                    if (queueDetailText.Count >= 2)
                    {
                        Thread th_one = new Thread(() =>
                        {
                            List<PartImage> listItemCamera1 = listItemCamera(queueDetailText.Peek(), CAM1, OUPUT_CAM1, fileName);
                            camera(listItemCamera1);

                            List<PartImage> listItemCamera2 = listItemCamera(queueDetailText.Peek(), CAM2, OUPUT_CAM2, fileName);
                            camera(listItemCamera2);


                        });
                        th_one.Start();
                        th_one.Join();



                        queueDetailText.Dequeue();
                    }



                }
                else
                {

                    Logger.Log(string.Format(fileNamejrn));
                }


                Logger.Log($"---------------------------------------------------------------");
                Console.WriteLine($"File Changed. Name: {fileNamejrn}" + " index line:" + indexline);


            }
            catch (Exception ex)
            {

                Logger.Log(string.Format("The process failed: {0}", ex.ToString()));
                Logger.Log(string.Format("The process failed: {0}", ex.StackTrace));
                Console.WriteLine(ex.ToString());
            }

        }






        public static List<PartImage> listItemCamera(List<DetailText> listlisDetailText, string camera, string ouputCam, string fileName)
        {

            List<PartImage> listDetailText = new List<PartImage>();
            foreach (DetailText itemText in listlisDetailText)

            {
                Thread th_one = new Thread(() =>
                {

                    List<DetailImage> listImageTran = listImageTransaction(camera + fileName + @"\\", itemText.getStartTime(), itemText.getEndTime());

                    foreach (DetailImage itemimage in listImageTran)
                    {
                        PartImage itemDetailText = new PartImage();
                        string getFilName = Path.GetFileName(itemimage.getPathImage());
                        itemDetailText.setTransNo(itemText.getCassette());
                        itemDetailText.setCurrentDateTime(itemText.getDateTimeRequest());
                        itemDetailText.setPathImage(itemimage.getPathImage());
                        itemDetailText.setPasrtSave(PathLocation(ouputCam + fileName + @"\\") + getFilName.Remove(getFilName.Length - 4) + @"_" + itemText.getCassette() + @".jpg");
                        listDetailText.Add(itemDetailText);
                    }
                });
                th_one.Start();
                th_one.Join();
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


        /* tìm kiếm danh sánh ảnh trong khoảng thời gian*/
        public static List<DetailImage> listImageTransaction(string partInputImage, DateTime startDateTransaction, DateTime endDateDateTransaction)
        {
            List<DetailImage> listDetailImage = new List<DetailImage>();
            try
            {


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
                                Console.WriteLine(arrListStr[1].Length);
                                string dateTime = "20210101000000";
                                if (arrListStr[1].Length >= 14)
                                {
                                    dateTime = arrListStr[1].Substring(0, 14);

                                }
                                DateTime currentDate = DateTime.ParseExact(dateTime, "yyyyMMddHHmmss", provider);

                                if (startDateTransaction <= currentDate && endDateDateTransaction >= currentDate)
                                {

                                    string name = getFilName.Substring(getFilName.LastIndexOf(@"-") + 1);
                                    itemDetail.setDescription(name.Remove(name.Length - name.Substring(name.LastIndexOf(@"_")).Length, name.Substring(name.LastIndexOf(@"_")).Length).Replace(@"_", @" ").Trim());
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
                    Graphics graphicImage = Graphics.FromImage(img);
                    StringFormat stringformat1 = new StringFormat();
                    stringformat1.Alignment = StringAlignment.Far;
                    Color stringColor1 = ColorTranslator.FromHtml("#e3e22d");
                    Color stringColor2 = ColorTranslator.FromHtml("#000000");

                    graphicImage.DrawImage(ImageFromText("Trans No: " + itemPartImage.getTransNo(), brFore, stringColor1, stringColor2, 4), new Point(600, 690));
                    Int32 quality = 80;
                    ImageCodecInfo jpegEncoder = ImageCodecInfo.GetImageDecoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                    EncoderParameters encparams = new EncoderParameters(1);
                    encparams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

                    ConvertTo24bpp(img).Save(itemPartImage.getPasrtSave(), jpegEncoder, encparams);
                    // img.Save(itemPartImage.getPasrtSave());

                }

            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("The process failed: {0}", ex.StackTrace));
                Console.WriteLine(string.Format("The process failed: {0}", ex.ToString()));
                Logger.Log(string.Format("The process failed: {0}", ex.ToString()));

            }
        }
        public static Bitmap ConvertTo24bpp(Image img)
        {
            var bmp = new Bitmap(img.Width, img.Height,
                          System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (var gr = Graphics.FromImage(bmp))
                gr.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
            return bmp;
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
        public static List<DetailText> listDetailTexts(List<TextLine> TextLine, string fileName, string transNo)
        {
            
            CultureInfo provider = CultureInfo.InvariantCulture;
            List<DetailText> listDetail = new List<DetailText>();

            foreach (TextLine itemTextLine in TextLine)
            {
                DetailText detail = new DetailText();


                string datetimestart = "00:00:00";
                if (itemTextLine.getTextStart().Trim().Length >= 8)
                {
                    datetimestart = itemTextLine.getTextStart().Substring(0, 8);
                }
                detail.setStartTime(DateTime.ParseExact(fileName + datetimestart, "yyyyMMddHH:mm:ss", provider));
                string datetimeend = "00:00:00";
                if (itemTextLine.getTextEnd().Trim().Length >= 8)
                {
                    datetimeend = itemTextLine.getTextEnd().Substring(0, 8);
                }

                detail.setEndTime(DateTime.ParseExact(fileName + datetimeend, "yyyyMMddHH:mm:ss", provider));
                foreach (String item in itemTextLine.getLine())
                {
                    if (item.Contains(transNo))
                    {
                            detail.setCassette(item.Split(new char[] { ':' })[1].Trim());
                        break;

                    }

                }



                listDetail.Add(detail);

            }

            return listDetail;
        }



        /*  public static List<DetailText> listDetailTexts(List<TextLine> TextLine, string fileName, string transNo, string dateTime, string cashRequest, Array listEnd)
          {
              CultureInfo provider = CultureInfo.InvariantCulture;
              List<DetailText> listDetail = new List<DetailText>();

              foreach (TextLine itemTextLine in TextLine)
              {
                  DetailText detail = new DetailText();
                  var itemline = itemTextLine.getLine();
                  int Length = itemline.Count;



                  for (int i = 0; i < Length; i++)
                  {
                      if (itemline[i].Contains(cashRequest))
                      {

                          string[] arrListStrStart = itemline[i].Split(new char[] { ' ' });
                          string datetime = "00:00:00";
                          if (arrListStrStart[0].Trim().Length >= 8)
                          {
                              datetime = arrListStrStart[0].Remove(0, arrListStrStart[0].Length - 8);
                          }
                          detail.setStartTime(DateTime.ParseExact(fileName + datetime, "yyyyMMddHH:mm:ss", provider));
                          detail.setCassette(itemline[i].Substring(itemline[i].LastIndexOf(@":") + 1).Replace(@"\", @"").Trim());

                      }

                      if (itemline[i].Contains(transNo))
                      {
                          detail.setTransNoRequest(itemline[i].Substring(itemline[i].LastIndexOf(@":") + 1).Replace(@"\", @"").Trim());

                      }
                      if (itemline[i].Contains(dateTime))
                      {

                          detail.setDateTimeRequest(itemline[i].Substring(itemline[i].LastIndexOf(@": ") + 1).Replace(@"\", @"").Trim());
                      }



                      if (checkEnd(itemline[i], listEnd))
                      {

                          string[] arrListStrEnd = itemline[i].Split(new char[] { ' ' });
                          string datetime = "00:00:00";
                          if (arrListStrEnd[0].Trim().Length >= 8)
                          {
                              datetime = arrListStrEnd[0].Remove(0, arrListStrEnd[0].Length - 8);
                          }

                          detail.setEndTime(DateTime.ParseExact(fileName + datetime, "yyyyMMddHH:mm:ss", provider));

                      }


                  }
                  listDetail.Add(detail);

              }

              return listDetail;
          }*/

        public static bool checkEnd(string indexline, Array textEnd)
        {
            foreach (string item in textEnd)
            {
                if (indexline.Contains(item))
                {
                    return true;
                }

            }
            return false;
        }

        /*  public static List<TextLine> textLine(List<string> text, string textStart, Array textEnd)
          {

              List<TextLine> listTextLine = new List<TextLine>();
              List<string> listStringitem = new List<string>();

              foreach (string itemtext in text)
              {

                  listStringitem.Add(itemtext);

                  if (checkEnd(itemtext, textEnd))
                  {

                      TextLine textLine = new TextLine();
                      List<string> listStringReverse = new List<string>();
                      listStringitem.Reverse();
                      foreach (string itemReverse in listStringitem)
                      {
                          listStringReverse.Add(itemReverse);
                          if (itemReverse.Contains(textStart))
                          {
                              textLine.setTextStart(itemReverse);
                              listStringReverse.Reverse();
                              textLine.setLine(listStringReverse);
                              listTextLine.Add(textLine);
                              listStringitem = new List<string>();
                              break;
                          }



                      }

                  }
              }

              return listTextLine;
          }*/
        public static string TransactionStart = "";
        public static bool TransactionEnd = false;
        public static List<TextLine> textLine(List<string> text, string textStart, Array textEnd)
        {
            // TRANSACTION_START, TRANSACTION_END
            List<TextLine> listTextLine = new List<TextLine>();
            List<string> listStringitem = new List<string>();

            foreach (string itemtext in text)
            {


                listStringitem.Add(itemtext);
                if (itemtext.Contains(TRANSACTION_START))
                {
                    TransactionStart = itemtext;
                }



                if (checkEnd(itemtext, textEnd))
                {

                    TextLine textLine = new TextLine();
                    List<string> listStringReverse = new List<string>();
                    listStringitem.Reverse();
                    foreach (string itemReverse in listStringitem)
                    {
                        listStringReverse.Add(itemReverse);


                        if (itemReverse.Contains(textStart))
                        {
                            textLine.setTextStart(TransactionStart);
                            List<string> listStringEnd = new List<string>();

                            foreach (string itemend in text)
                            {
                                if (itemend.Equals(TransactionStart))
                                {
                                    TransactionEnd = true;

                                }
                                if (TransactionEnd && itemend.Contains(TRANSACTION_END))
                                {
                                    textLine.setTextEnd(itemend);
                                    TransactionEnd = false;
                                    break;

                                }
                            }

                            listStringReverse.Reverse();
                            textLine.setLine(listStringReverse);
                            listTextLine.Add(textLine);
                            listStringitem = new List<string>();
                            break;
                        }



                    }

                }
            }

            return listTextLine;
        }


        public static List<string> listDetailText(string fullPath, string name, string textStart, string textEnd)
        {


            List<string> listString = new List<string>();
            List<string> listStringItem = new List<string>();

            if (nameText == null)
            {
                nameText = name;
            }
            if (!nameText.Equals(name))
            {

                nameText = name;
                indexline = 0;
                indexlinEnd = 0;


            }
            Logger.Log($"File Changed. Name: {name}" + " index line start:" + indexlinEnd);
            // Thread.Sleep(300);

            using (var stream = new FileStream(path: fullPath, mode: FileMode.Open, access: FileAccess.ReadWrite, share: FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    for (int i = 1; i <= indexlinEnd; i++)
                    {

                        reader.ReadLine();
                    }
                    indexline = indexlinEnd;
                    string line;
                    string checkItemtext = null;

                    while (!reader.EndOfStream)
                    {

                        indexline = indexline + 1;

                        line = reader.ReadLine();

                        if (line.Contains(textStart) && checkItemtext == null)
                        {
                            checkItemtext = line;
                            listStringItem.Add(line);
                            continue;

                        }
                        if (line.Contains(textStart) && checkItemtext != null)
                        {
                            listStringItem = new List<string>();
                            checkItemtext = line;
                            listStringItem.Add(line);
                            continue;

                        }

                        if (checkItemtext != null)
                        {
                            listStringItem.Add(line);
                        }

                        if (line.Contains(textEnd))
                        {
                            listString.AddRange(listStringItem);
                            listStringItem = new List<string>();
                            checkItemtext = null;

                            indexlinEnd = indexline;

                            Console.WriteLine("------------------" + indexlinEnd);
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

