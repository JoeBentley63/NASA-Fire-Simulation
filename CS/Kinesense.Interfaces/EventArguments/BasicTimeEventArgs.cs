using System;

namespace Kinesense.Interfaces
{
    public class BasicTimeEventArgs : EventArgs
    {
        public DateTime? Time { get; set; }
        public Guid VidGUID {get; set;}
        public string VidName { get; set; }
    }
}
