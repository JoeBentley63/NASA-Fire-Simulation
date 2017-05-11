using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.EventArguments
{
    public class ShowDBConnectionStatusEventArgs : System.EventArgs
    {
        public string Message { get; set; }
        public bool ShowMessage { get; set; }
    }
}
