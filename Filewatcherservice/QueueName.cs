using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filewatcherservice
{
   public class QueueName
    {
        private string name;
        private string fullPath;
        private int indexList;
        public int getIndexList()
        {
            return indexList;
        }

        public void setIndexList(int value)
        {
            indexList = value;
        }
        public string getName()
        {
            return name;
        }

        public void setName(string value)
        {
            name = value;
        }
        public string getFullPath()
        {
            return fullPath;
        }

        public void setFullPath(string value)
        {
            fullPath = value;
        }

    }
}
