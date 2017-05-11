using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.EventArguments
{
    public class VideoDecoderCompleteEventArgs : EventArgs
    {
        public int CountFramesSent { get; set; }
    }
}
