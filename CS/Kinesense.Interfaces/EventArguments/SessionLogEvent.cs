using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.EventArguments
{
    public class SessionLogEvent : EventArgs
    {
        // simple version
        public string Description { get; set; }
        // complex version
        public string Formatstring { get; set; }
        public object[] Objects { get; set; }
    }
}
