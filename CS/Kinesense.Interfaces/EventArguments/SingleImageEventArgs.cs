using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces
{
    public class SingleImageEventArgs : EventArgs
    {
        public ByteArrayBitmap Image;
        public short VidID;
        public DateTime Time;
    }
}
