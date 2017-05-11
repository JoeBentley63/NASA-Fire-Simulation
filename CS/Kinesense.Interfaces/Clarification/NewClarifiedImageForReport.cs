using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Clarification
{
    public class NewClarifiedImageForReport : EventArgs
    {
        public ByteArrayBitmap Image { get; set; }
        public string AppliedClarifications { get; set; }
        public string SourceName { get; set; }
        public DateTime Time { get; set; }
    }
}
