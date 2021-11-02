using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filewatcherservice
{
    public class TransactionItem
    {
        private DateTime startTime;
        private DateTime endTime;
        private int lineNumber;
        private List<DetailText> listDetailText;
        private string transNo;

        public void setTransNo(string value)
        {
            transNo = value;
        }

        public string getTransNo()
        {
            return transNo;
        }

        public List<DetailText> getListDetailText()
        {
            return listDetailText;
        }

        public void setListDetailText(List<DetailText> value)
        {
            listDetailText = value;
        }
        public DateTime getStartTime()
        {
            return startTime;
        }
        public DateTime getEndTime()
        {
            return endTime;
        }

        public void setEndTime(DateTime value)
        {
            endTime = value;
        }
        //public DateTime getStartTime()
        //{
        //    return startTime;
        //}

        public void setStartTime(DateTime value)
        {
            startTime = value;
        }

        public void setLineNumber(int value)
        {
            lineNumber = value;
        }

        public int getLineNumber()
        {
            return lineNumber;
        }

    }
}
