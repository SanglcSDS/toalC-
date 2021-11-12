using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filewatcherservice
{
   public  class TextLine
    {
        private string textStart;
        private string textEnd;
        private List<string> line;
        public string getTextStart()
        {
            return textStart;
        }

        public void setTextStart(string value)
        {
            textStart = value;
        }
        public string getTextEnd()
        {
            return textEnd;
        }

        public void setTextEnd(string value)
        {
            textEnd = value;
        }

        public List<string> getLine()
        {
            return line;
        }

        public void setLine(List<string> value)
        {
            line = value;
        }

    }
}
