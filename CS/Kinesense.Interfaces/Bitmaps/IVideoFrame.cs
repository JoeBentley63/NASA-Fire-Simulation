using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Bitmaps
{
    public interface IVideoFrame
    {
        DateTime DBTime { get; set; }
        int Width { get; }
        int Height { get; }
    }
}
