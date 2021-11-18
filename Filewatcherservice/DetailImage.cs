using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filewatcherservice
{
   public class DetailImage
    {


        private string pathImage;
        private string description;
        private string descriptionItem;
        private DateTime startTime;
        private DateTime endTime;
        private string camera;
        private string cardNo;
        private DateTime currentDate;
        private string terminalId;
        // DateTime.ToString("MM/dd/yyyy HH:mm:ss")
        public string getDescriptionItem()
        {
            return descriptionItem;
        }

        public void setDescriptioItem(string value)
        {
            descriptionItem = value;
        }
        public string getDescription()
        {
            return description;
        }

        public void setDescription(string value)
        {
            description = value;
        }
        public string getTerminalId()
        {
            return terminalId;
        }

        public void setTerminalIdy(string value)
        {
            terminalId = value;
        }

        public string getCamera()
        {
            return camera;
        }

        public void setCamera(string value)
        {
            camera = value;
        }

        public string getCardNo()
        {
            return cardNo;
        }

        public void setCardNo(string value)
        {
            cardNo = value;
        }
        public DateTime getCurrentDate()
        {
            return currentDate;
        }

        public void setCurrentDate(DateTime value)
        {
            currentDate = value;
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
