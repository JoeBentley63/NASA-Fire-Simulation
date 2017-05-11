using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.EventArguments
{
    public class BasicPercentCompleteEventArgs : EventArgs
    {
        public double FractionComplete { get; set; }
        public string JobName { get; set; }
    }
}
