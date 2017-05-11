using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.EventArguments
{
    public class ReportMarkerListChangeEventArgs : EventArgs
    {
        public List<KeyValuePair<short, DateTime>> Entries { get; set; }
    }
}
