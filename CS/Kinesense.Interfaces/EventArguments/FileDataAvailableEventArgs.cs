using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.EventArguments
{
    public class FileDataAvailableEventArgs : EventArgs
    {
        public byte[] Data { get; set; }
    }
}
