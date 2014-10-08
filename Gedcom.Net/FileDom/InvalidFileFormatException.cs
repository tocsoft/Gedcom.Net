using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gedcom.Net.FileDom
{
    public class InvalidFileFormatException : Exception
    {
        public int LineNumber { get; private set; }

        public InvalidFileFormatException(int linenumber, string message) : base(message)
        {
            LineNumber = linenumber;
        }
    }
}
