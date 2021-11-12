using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filewatcherservice
{
    public class PartImage
    {
        private string fileName;
        private string pathImage;
        private string currentDateTime;
        private string transNo;
        private string cassette;
        private List<DetailImage> detailImage;
        private DetailText detailText;
        public string getFileName()
        {
            return fileName;
        }

        public void setFileName(string value)
        {
            fileName = value;

        }
        public string getCassette()
        {
            return cassette;
        }

        public void setCassette(string value)
        {
            cassette = value;

        }
        public string getCurrentDateTime()
        {
            return currentDateTime;
        }

        public void setCurrentDateTime(string value)
        {
            if (value == null)
            {
                currentDateTime = "";
            }
            else
            {
                currentDateTime = value;
            }

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
        public DetailText getDetailText()
        {
            return detailText;
        }

        public void setDetailText(DetailText value)
        {
            detailText = value;
        }
        public List<DetailImage> getListDetailImage()
        {
            return detailImage;
        }

        public void setListDetailImage(List<DetailImage> value)
        {
            detailImage = value;
        }
        public string getPathImage()
        {
            return pathImage;
        }

        public void setPathImage(string value)
        {
            pathImage = value;
        }

    }

}
