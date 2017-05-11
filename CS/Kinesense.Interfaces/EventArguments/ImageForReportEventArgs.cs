using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces
{
    public class ImageForReportEventArgs:EventArgs
    {
        public ByteArrayBitmap Image { get; set; }
        public string Title { get; set; }
        public string Notes { get; set; }
        public string VideoSource { get; set; }
        public Guid VideoGUID { get; set; }
        public DateTime StartTime { get; set; }
    }
}
