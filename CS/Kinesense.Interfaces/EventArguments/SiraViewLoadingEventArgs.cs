using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.EventArguments
{
    public class SiraViewLoadingProgressEventArgs : EventArgs
    {
        public int DecoderID { get; set; }
        public string CurrentDecoderTested { get; set; }

        public bool OpenComplete { get; set; }
        public bool OpenSuccess { get; set; }
    }
}
