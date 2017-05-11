using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Globalization;
using System.IO;

namespace Kinesense.Interfaces
{
    /// <summary>
    /// Converter class to convert a jpeg to bitmap source for display in Filter ID card thumbnail
    /// </summary>	
    public class JpegDataToBitmapSourceConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte[] data = value as byte[];
            if (data != null && data.Length > 10 && data.Length < 300000) //to avoid exceptions thown on null frames, and memory leaks from giant frames
            {
                BitmapDecoder decoder;
                using (MemoryStream memoryStream = new MemoryStream(data))
                    decoder = BitmapDecoder.Create(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                return decoder.Frames[0];
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
            //throw new NotImplementedException();
        }

        #endregion
    }
}
