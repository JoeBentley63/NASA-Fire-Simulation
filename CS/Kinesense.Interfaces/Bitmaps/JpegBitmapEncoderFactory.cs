using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Kinesense.Interfaces
{
    public class JpegBitmapEncoderFactory : IBitmapEncoderFactory
    {
        private readonly int _qualityLevel;
        private readonly BitmapCodecInfo _codecInfo;

        public JpegBitmapEncoderFactory(int qualityLevel)
        {
            _qualityLevel = qualityLevel;
            _codecInfo = (new JpegBitmapEncoder()).CodecInfo;
        }

        #region IBitmapEncoderProvider Members

        public BitmapEncoder GetBitmapEncoder()
        {
            return new JpegBitmapEncoder { QualityLevel = _qualityLevel };
        }

        public BitmapCodecInfo CodecInfo
        {
            get { return _codecInfo; }
        }

        public static int DefaultJpegQuality = 85;

        public static byte?[] JpegHeaderPattern = new byte?[] { 0xFF, null, 0xFF, null };                

        #endregion
    }

    public interface IBitmapEncoderFactory
    {
        BitmapEncoder GetBitmapEncoder();
        BitmapCodecInfo CodecInfo { get; }
    }
}
