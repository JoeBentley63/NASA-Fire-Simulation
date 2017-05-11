using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Bitmaps
{
    public class NewSystemDrawingBitmapEventArgs : EventArgs
    {
        public System.Drawing.Bitmap Bitmap { get; set; }
    }
}
