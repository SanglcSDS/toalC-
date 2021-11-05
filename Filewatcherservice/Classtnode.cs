using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filewatcherservice
{
    class Classtnode
    {
        /*  public static List<DetailText> listDetailText(string fullPath, string name)
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

           using (StreamReader reader = new StreamReader(File.OpenRead(fullPath)))
           {


               for (int i = 1; i <= indexline; i++)
               {

                   reader.ReadLine();
               }

               string line;

               while ((line = reader.ReadLine()) != null)
               {
                   indexline = indexline + 1;


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

               reader.Close();
           }

           return listDetail;
       }*/



        /*----------------------------------------------------*/

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

    }
}
