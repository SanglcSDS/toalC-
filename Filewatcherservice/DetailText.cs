using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filewatcherservice
{
    public class DetailText
    {
        private string pathImage;
        private string pasrtSave;
        private string transactionName;
        private string cassette;
   
        private string dateTimeRequest;
      //  private string dateTimeTake;
        private string transNoRequest;
      //  private string transNoTake;
        private DateTime startTime;
        private DateTime endTime;

        public string getPasrtSave()
        {
            return pasrtSave;
        }

        public void setPasrtSave(string value)
        {
            pasrtSave = value;

        }
        public string getTransactionName()
        {
            return transactionName;
        }

        public void setTransactionName(string value)
        {
            transactionName = value;
        }


        public string getCassette()
        {
            return cassette;
        }

        public void setCassette(string value)
        {
            cassette = value;
            
        }

        public string getTransNoRequest()
        {
            return transNoRequest;
        }

        public void setTransNoRequest(string value)
        {
            if (value == null)
            {
                transNoRequest = "";
            }
            else
            {
                transNoRequest = value;
            }
           
        }
    /*    public string getTransNoTake()
        {
            return transNoTake;
        }

        public void setTransNoTake(string value)
        {
            if (value == null)
            {
                transNoTake = "";
            }
            else
            {
                transNoTake = value;
            }

        }
        public string getDateTimeTake()
        {
            return dateTimeTake;
        }

        public void setDateTimeTake(string value)
        {
            if (value == null)
            {
                dateTimeTake = "";
            }
            else
            {
                dateTimeTake = value;
            }
           
           
        }*/
        public string getDateTimeRequest()
        {
            return dateTimeRequest;
        }

        public void setDateTimeRequest(string value)
        {
            if (value == null)
            {
                dateTimeRequest = "";
            }
            else
            {
                dateTimeRequest = value;
            }
           
        }

        public string getPathImage()
        {
            return pathImage;
        }

        public void setPathImage(string value)
        {
            pathImage = value;
        }
        public DateTime getEndTime()
        {
            return endTime;
        }

        public void setEndTime(DateTime value)
        {
            endTime = value;
        }
        public DateTime getStartTime()
        {
            return startTime;
        }

        public void setStartTime(DateTime value)
        {
            startTime = value;
        }

    }
}
