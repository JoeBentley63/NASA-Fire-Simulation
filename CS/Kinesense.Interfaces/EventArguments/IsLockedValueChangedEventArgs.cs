using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.EventArguments
{
    public class IsLockedValueChangedEventArgs : EventArgs
    {
        public bool IsLocked { get; set; }
        public bool IsImporting { get; set; }
        public string LockedBy { get; set; }
    }
}
