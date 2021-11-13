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
        private string fileName;
        private string transactionName;
        private string cassette;
        private string transNo;
        private string currentTime;
        private string currentDate;
        private string terminalId;
        private DateTime startTime;
        private DateTime endTime;

        public string getFileName()
        {
            return fileName;
        }

        public void setFileName(string value)
        {
            fileName = value;

        }
        public string getTransactionName()
        {
            return transactionName;
        }

        public void setTransactionName(string value)
        {
            transactionName = value;
        }
        public string getTerminalId()
        {
            return terminalId;
        }

        public void setTerminalIdy(string value)
        {
            terminalId = value;
        }

        public string getCassette()
        {
            return cassette;
        }

        public void setCassette(string value)
        {
            cassette = value;
            
        }

        public string getTransNo()
        {
            return transNo;
        }

        public void setTransNo(string value)
        {
            if (value == null)
            {
                transNo = "";
            }
            else
            {
                transNo = value;
            }
           
        }
        public string getCurrentTime()
        {
            return currentTime;
        }

        public void setCurrentTime(string value)
        {
            if (value == null)
            {
                currentTime = "";
            }
            else
            {
                currentTime = value;
            }
           
           
        }
        public string getCurrentDate()
        {
            return currentDate;
        }

        public void setCurrentDate(string value)
        {
            if (value == null)
            {
                currentDate = "";
            }
            else
            {
                currentDate = value;
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
