using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Classes
{
    public class DateTimeConverters
    {
        public delegate string FormatTimeSpanDelegate(TimeSpan ts);
        public static FormatTimeSpanDelegate FormatTimeSpan;

        public static string Format(double ms)
        {
            return Format(TimeSpan.FromMilliseconds(ms));
        }

        public static string Format(TimeSpan ts)
        {
            if (FormatTimeSpan != null)
                return FormatTimeSpan(ts);
            else
                return TimeSpanToString(ts);
        }
        
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0,
                                                      DateTimeKind.Utc);

        public static DateTime HexUnixTimeStampToDateTime(string hexValue)
        {
            int decValue = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
            return Epoch.AddSeconds(decValue);
        }        

        private static string TimeSpanToString(TimeSpan span)
        {
            StringBuilder sb = new StringBuilder();
            if (span.Days > 0)
                sb.Append(span.Days.ToString("00") + " day ");//"d ");
            if (span.Hours > 0 || span.TotalHours >= 1)
                sb.Append(span.Hours.ToString() + " hours ");//"h ");
            if (span.Minutes > 0 || span.TotalMinutes >= 1)
                sb.Append(span.Minutes.ToString("00") + " min ");//"min ");
            if (span.TotalHours < 1)
                sb.Append(span.Seconds.ToString("00") + " sec ");//"sec");
            return sb.ToString();
        }
    }
}
