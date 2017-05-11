using System;

namespace Kinesense.Interfaces.Useful
{
    public static class KinesenseInternationalTimeStringFormat
    {
        /// <summary>
        /// Culture based shotr date time pattern
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ShortTimeDateString(DateTime time)
        {
            return time.ToString(System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortTimePattern
                                + " "
                                + System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern);
        }

        /// <summary>
        /// culture based long date time pattern. If the time is MinValue, return ""
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string LongTimeDateString_EmptyIfZero(DateTime time)
        {
            if (time == DateTime.MinValue)
                return "";
            else
                return time.ToString(System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.LongTimePattern
                               + " "
                               + System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.LongDatePattern);
        }

        /// <summary>
        /// culture based long date time pattern
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string LongTimeDateString(DateTime time)
        {
            return LongTimeDateString_EmptyIfZero(time);
        }

        /// <summary>
        /// full time string in standard format, no date, HH:mm:ss.ff
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string StandardLongTimeString(DateTime time)
        {
            return time.ToString("HH:mm:ss.ff");
        }

        /// <summary>
        /// full time string in standard format, no date, HH:mm:ss.ff
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string StandardTimeString(DateTime time)
        {
            return time.ToString("HH:mm:ss");
        }

        /// <summary>
        /// full date string in standard format, no time, yyyy-MM-dd
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string StandardLongDateString(DateTime time)
        {
            return time.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// full time string in standard format "dd/MM/yyyy HH:mm:ss.fff"
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string StandardLongTimeDateString(DateTime time)
        {
            return time.ToString("dd/MM/yyyy HH:mm:ss.fff");
        }

        /// <summary>
        /// full time string in standard format "yyyy-MM-dd HH:mm:ss.ff"
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string StandardFullTimeDateString(DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss.ff");
        }

        /// <summary>
        /// full time string in standard format "yyyy-MM-dd HH:mm:ss.ff"
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string StandardFullTimeDateStringNoMilliseconds(DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// full time string in standard format "dd-MM-yyyy HH.mm.ss"
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string StandardISO8601ishString(DateTime time)
        {
            return time.ToString("dd-MM-yyyy HH.mm.ss");
        }

        public static string ShortTimeSpanString(TimeSpan time)
        {
            return time.ToString("c");
        }
    }
}
