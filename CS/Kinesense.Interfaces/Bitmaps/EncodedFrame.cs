using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces
{
    /// <summary>
    /// low weight frame data for use in InterProcess Communication
    /// </summary>
    public class EncodedFrame : IDisposable
    {
        public EncodedFrame(byte[] jpegData, byte[] timeData)
        {
            TimeStamp = DateTime.FromBinary(BitConverter.ToInt64(timeData, 0));
            JpegData = jpegData;
        }

        public EncodedFrame(byte[] jpegData, DateTime time)
        {
            TimeStamp = time;
            JpegData = jpegData;
        }

        public EncodedFrame(VideoFrame frame)
        {
            TimeStamp = frame.DBTime;
            JpegData = frame.EncodedData;
        }

        public EncodedFrame(byte[] jpegData, DateTime time, bool finalFrame)
        {
            TimeStamp = time;
            JpegData = jpegData;
            FinalFrame = finalFrame;
        }
        
        public int Channel { get; set; }

        public byte[] JpegData { get; set; }
        
        public DateTime TimeStamp { get; set; }

        public long RawTimeStamp { get; set; }

        public bool FinalFrame { get; set; }

        public int LengthOfJpegData
        {
            get
            {
                return (JpegData != null) ? JpegData.Length : 0;
            }
        }

        public byte[] TimeStampBytes
        {
            get
            {
                return BitConverter.GetBytes(TimeStamp.ToBinary());
            }
        }

        ~EncodedFrame()
        {
            Dispose();
        }

        private bool _disposed = false;
        public void Dispose()
        {
            if (!_disposed)
            {
                JpegData = null;
                GC.SuppressFinalize(this);
                _disposed = true;
            }
        }
    }
}
