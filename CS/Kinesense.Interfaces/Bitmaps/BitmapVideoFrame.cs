using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Bitmaps
{
    public class BitmapVideoFrame : IVideoFrame, IDisposable
    {
        public Bitmap Bitmap { get; set; }
        public DateTime DBTime { get; set; }

        public int Height
        {
            get
            {
                if (Bitmap == null)
                    return 0;
                return Bitmap.Height;
            }
        }

        public int Width
        {
            get
            {
                if (Bitmap == null)
                    return 0;
                return Bitmap.Width;
            }
        }

        public void Dispose()
        {
            if (Bitmap != null)
                Bitmap.Dispose();
        }
    }
}
