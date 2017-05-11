using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Cleanup
{
    public class GlobalThreadQuitter
    {
        private static bool _IsExiting = false;
        public static bool IsExiting { get { return _IsExiting; } set { _IsExiting = value; } }
    }
}
