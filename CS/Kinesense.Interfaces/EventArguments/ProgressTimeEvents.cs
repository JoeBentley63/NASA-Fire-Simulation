using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.EventArguments
{
    public class ProgressTimeEventArgs : EventArgs
    {
        public int ClipPosition { get; set; }
        public double FractionComplete { get; set; }
    }
}
