using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces
{
    public class FilterChangedEventArgs : EventArgs
    {
        public int videoID { get; set; }
        public int filterID { get; set; }
        public bool hasChanged = false;
        public bool hasBeenDeleted = false;
    }
}
