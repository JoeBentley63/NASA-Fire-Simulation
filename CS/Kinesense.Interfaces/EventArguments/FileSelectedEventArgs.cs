using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces
{
    public class FileSelectedEventArgs : EventArgs
    {
        public string FilePath { get; set; }
    }
}
