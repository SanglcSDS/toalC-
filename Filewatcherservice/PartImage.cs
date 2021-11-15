using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filewatcherservice
{
    public class PartImage
    {
        private string pasrtSave;
        private string pathImage;
        private string currentDateTime;
        private string transNo;
        private string cassette;

        public string getPasrtSave()
        {
            return pasrtSave;
        }

        public void setPasrtSave(string value)
        {
            if (value == null)
            {
                pasrtSave = "#";
            }
            else
            {
                pasrtSave = value;
            }
          

        }
        public string getCassette()
        {
            return cassette;
        }

        public void setCassette(string value)
        {
            if (value == null)
            {
                cassette = "#";
            }
            else
            {
                cassette = value;
            }


        }
        public string getCurrentDateTime()
        {
            return currentDateTime;
        }

        public void setCurrentDateTime(string value)
        {
            if (value == null)
            {
                currentDateTime = "#";
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
                transNo = "#";
            }
            else
            {
                transNo = value;
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

    }

}
