using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Bitmaps
{
    public class NewBitmapEventArgs : EventArgs
    {
        public ByteArrayBitmap Bitmap { get; set; }
        public TimeSpan Time { get; set; }
        public bool IsLastFrame { get; set; }
    }
}
