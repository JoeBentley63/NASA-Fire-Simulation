using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinesense.Interfaces.Enum;

namespace Kinesense.Interfaces.EventArguments
{
    public class DeinterlaceModeEventArgs : EventArgs
    {
        public DeinterlaceMode Mode { get; set; }
    }
}
