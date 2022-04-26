using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Filewatcherservice
{
    public class TransactionItem
    {
        private DateTime startTime;
        private DateTime endTime;
        private int lineNumber;
     


       
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
