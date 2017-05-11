using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Kinesense.Interfaces.Serializers
{
    public static class SerializeImage
    {
        public static string SerializeControlsImageToPNGHexString(System.Windows.Controls.Image img)
        {
            //bitmap
            if (img != null && img.Source != null)
            {
                BitmapSource source = (BitmapSource)img.Source;
                PngBitmapEncoder encoder = new PngBitmapEncoder();

                byte[] data;
                encoder.Frames.Add(BitmapFrame.Create(source));

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    encoder.Save(memoryStream);
                    data = memoryStream.ToArray();
                }

                StringBuilder hex = new StringBuilder(data.Length * 2);
                foreach (byte b in data)
                    hex.AppendFormat("{0:x2}", b);
                return hex.ToString();
            }

            return "";
        }


        public static string SerializeControlsImageToJpegHexString(System.Windows.Controls.Image img)
        {
            //bitmap
            if (img != null && img.Source != null)
            {
                BitmapSource source = (BitmapSource)img.Source;

                JpegBitmapEncoder jpegEncoder = new JpegBitmapEncoder { QualityLevel = 95 };
                byte[] jpegdata;
                jpegEncoder.Frames.Add(BitmapFrame.Create(source));

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    jpegEncoder.Save(memoryStream);
                    jpegdata = memoryStream.ToArray();
                }

                StringBuilder hex = new StringBuilder(jpegdata.Length * 2);
                foreach (byte b in jpegdata)
                    hex.AppendFormat("{0:x2}", b);
                return hex.ToString();
            }

            return "";
        }

        public static System.Windows.Controls.Image DeserializeControlsImageFromHexString(string hex)
        {
            try
            {
                if (string.IsNullOrEmpty(hex))
                    return null;

                int numberOfChars = hex.Length;
                byte[] jpgdata = new byte[numberOfChars / 2];
                for (int i = 0; i < numberOfChars; i += 2)
                    jpgdata[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);

                BitmapDecoder decoder;
                using (MemoryStream memoryStream = new MemoryStream(jpgdata))
                    decoder = BitmapDecoder.Create(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

                BitmapSource bitmapsource = decoder.Frames[0];

                System.Windows.Controls.Image img = new System.Windows.Controls.Image() { Source = bitmapsource };
                return img;
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }
            return null;
        }
    }
}
