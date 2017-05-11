using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Enum
{
    public class EnumLocalizer
    {
        public delegate string EnumToLocalStringDelegate(System.Enum e);

        public static EnumToLocalStringDelegate EnumLocalizerDelegate;

        public delegate string ToLocalStringDelegate(string e);

        public static ToLocalStringDelegate StringLocalizer;

        public static string ToLocalString(string e)
        {
            if (StringLocalizer == null)
                return e.ToString();
            else
                return StringLocalizer(e);
        }

        public static string ToLocalString(System.Enum e)
        {
            if (EnumLocalizerDelegate == null)
                return e.ToString();
            else
                return EnumLocalizerDelegate(e);
        }
    }
}
