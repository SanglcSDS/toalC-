using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filewatcherservice
{
   public  class TextLine
    {
        private int indexLineStart;
        private int indexLineEnd;
        private List<string> line;
        public int getIndexLineStart()
        {
            return indexLineStart;
        }

        public void setIndexLineStart(int value)
        {
            indexLineStart = value;
        }
        public int getIndexLineEnd()
        {
            return indexLineEnd;
        }

        public void setIndexLineEnd(int value)
        {
            indexLineEnd = value;
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
