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
        private string sortName;
        private string startTime;
        private string endTime;
        private string cassette;
        private string transNo;
        private string timeDay;
        private string terminalId;
        // DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")
      

        public string getTerminalId()
        {
            return terminalId;
        }

        public void setTerminalIdy(string value)
        {
            terminalId = value;
        }

        public string getCassetteo()
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
            transNo = value;
        }
        public string getTimeDay()
        {
            return timeDay;
        }

        public void setTimeDay(string value)
        {
            timeDay = value;
        }

        public string getPathImage()
        {
            return pathImage;
        }

        public void setPathImage(string value)
        {
            pathImage = value;
        }
        public string getEndTime()
        {
            return endTime;
        }

        public void setEndTime(string value)
        {
            endTime = value;
        }
        public string getStartTime()
        {
            return startTime;
        }

        public void setStartTime(string value)
        {
            startTime = value;
        }

    }
}
