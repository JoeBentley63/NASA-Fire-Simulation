using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Bitmaps
{
    public class TimedByteArrayBitmap : ByteArrayBitmap
    {
        public TimedByteArrayBitmap(ByteArrayBitmap bmp, DateTime time) 
            : base(bmp)
        {
            Time = time;
        }

        public DateTime Time { get; set; }

        public TimedByteArrayBitmap Clone()
        {
            return new TimedByteArrayBitmap(this, Time);
        } 
    }
}
