using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing;

namespace Kinesense.Interfaces
{

    /// <summary>
    /// compresses and decompresses ByteArrayBitmap Data on demand
    /// </summary>
    public class CompressedByteArrayBitmap : ByteArrayBitmap
    {
        public override byte[] Data
        {
            get
            {
                return Decompress();
            }
            protected set
            {
                if (value == null)
                    _CompressedData = null;
                else 
                    _CompressedData = Compress(value);
            }
        }

        public CompressedByteArrayBitmap(byte[] jpegdata) :
            base(jpegdata, false)
        {
            _CompressedData = jpegdata;
        }

        public CompressedByteArrayBitmap(ByteArrayBitmap sourceBmp) :
            base( sourceBmp.Width, sourceBmp.Height, sourceBmp.Format)
        {
            this.BytesPerPixel = sourceBmp.BytesPerPixel;
            this.Width = sourceBmp.Width;
            this.Height = sourceBmp.Height;
            this.Stride = sourceBmp.Stride;
            this.Format = sourceBmp.Format;

            //now that everything needed is set, compress
            this.Data = sourceBmp.Data;

        }

        public void Save(string path, System.Drawing.Imaging.ImageFormat format)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(this._CompressedData))
                {

                    Bitmap bmp = new Bitmap(ms);

                    bmp.Save(path, format);

                    //Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                    //System.Drawing.Imaging.BitmapData bmpData =
                    //    bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    //    bmp.PixelFormat);

                    //// Get the address of the first line.
                    //IntPtr ptr = bmpData.Scan0;

                    //// Declare an array to hold the bytes of the bitmap.
                    //int bytes = bmpData.Stride * bmp.Height;

                    //System.Runtime.InteropServices.Marshal.Copy(Data, 0, ptr, bytes);

                    //// Unlock the bits.
                    //bmp.UnlockBits(bmpData);
                }
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }
        }

        public virtual System.Drawing.Bitmap ToBitmap()
        {
            try
            {
                Bitmap bmp = null;
                using (MemoryStream ms = new MemoryStream(this._CompressedData))
                {

                    bmp = new Bitmap(ms);

                    Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                    System.Drawing.Imaging.BitmapData bmpData =
                        bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                        bmp.PixelFormat);

                    // Get the address of the first line.
                    IntPtr ptr = bmpData.Scan0;

                    // Declare an array to hold the bytes of the bitmap.
                    int bytes = bmpData.Stride * bmp.Height;

                    System.Runtime.InteropServices.Marshal.Copy(Data, 0, ptr, bytes);

                    // Unlock the bits.
                    bmp.UnlockBits(bmpData);
                }

                return bmp;
            }
            catch (Exception er)
            {
                if (this.Disposed)
                    return null;

                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
                throw;
            }
        }

        protected byte[] _CompressedData;
        protected byte[] Compress(byte[] data)
        {
            byte[] encodedData = null;
            try
            {

                var encoder = new JpegBitmapEncoderFactory(JpegBitmapEncoderFactory.DefaultJpegQuality).GetBitmapEncoder();

                var bitmapSource = BitmapSource.Create(this.Width, this.Height, 96, 96, this.Format, null, data, this.Stride);
                bitmapSource.Freeze();

                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                using (MemoryStream imageMemoryStream = new MemoryStream())
                {
                    encoder.Save(imageMemoryStream);
                    encodedData = imageMemoryStream.ToArray();
                }


            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }

            return encodedData;
        }

        protected byte[] Decompress()
        {
            byte[] data = null;
            try
            {
                BitmapDecoder decoder;
                using (MemoryStream memoryStream = new MemoryStream(_CompressedData))
                    decoder = BitmapDecoder.Create(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                BitmapFrame bitmapFrame = decoder.Frames[0];

                data = new ByteArrayBitmap(bitmapFrame).Data;
                //bitmapFrame.CopyPixels(data, this.Stride, 0);
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }
            return data;
        }

        public override void Dispose()
        {
           if (!this.Disposed)
            {
                if (_CompressedData != null)
                    _CompressedData = null;

                if (this.DataGrayscale != null)
                    this.DataGrayscale = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}
