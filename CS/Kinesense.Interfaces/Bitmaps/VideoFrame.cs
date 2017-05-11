using Kinesense.Interfaces.Bitmaps;
using System;
using System.Drawing;
using System.IO;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Windows.Media.Imaging;

namespace Kinesense.Interfaces
{
    [Serializable]
    public class VideoFrame : IDisposable, ICloneable, IVideoFrame
    {

        protected void Initialize(ByteArrayBitmap bitmap, byte[] encodedData, ContentType encodedDataContentType, DateTime dbtime, bool decode)
        {
            if ((bitmap == null) && (encodedData == null))
                return;

            this.DBTime = dbtime;
            _bitmap = bitmap;
            this.EncodedData = encodedData;
            this.EncodedDataContentType = encodedDataContentType;

            if (bitmap == null && decode)
                this.TryToDecodeEncodedDataTo_bitmap();

            if (encodedData == null)
                this.EncodeBitmap(new JpegBitmapEncoder { QualityLevel = JpegBitmapEncoderFactory.DefaultJpegQuality });

            TrySetWidthHeight();
        }

        public VideoFrame(MemoryStream memoryStream, ContentType encodedDataContentType, DateTime dbtime)
        {
            if (memoryStream == null)
                throw new ArgumentNullException("memoryStream");
            this.Initialize(null, memoryStream.ToArray(), encodedDataContentType, dbtime, false);
        }

        public VideoFrame(byte[] encodedData, ContentType encodedDataContentType, DateTime dbtime, bool decode)
        {
            if (encodedData == null)
                throw new ArgumentNullException("encodedData");
            this.Initialize(null, (byte[])encodedData.Clone(), encodedDataContentType, dbtime, decode);
        }

        public VideoFrame(byte[] encodedData, ContentType encodedDataContentType, DateTime dbtime)
        {
            if (encodedData == null)
                throw new ArgumentNullException("encodedData");
            this.Initialize(null, (byte[])encodedData.Clone(), encodedDataContentType, dbtime, false);
        }

        public VideoFrame(BitmapSource bitmapSource, DateTime dbtime)
        {
            if (bitmapSource == null)
                throw new ArgumentNullException("bitmapSource");

            this.Initialize(new ByteArrayBitmap(bitmapSource), null, null, dbtime, false);
        }

        public VideoFrame(ByteArrayBitmap bitmap, DateTime dbtime, bool clone)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            ByteArrayBitmap bmp = (clone ? bitmap.Clone() : bitmap);

            this.Initialize(bmp, null, null, dbtime, false);
        }

        public VideoFrame(bool isH264, DateTime dt, int frameNo, GetH264Frame getframe)
        {
            IsH264Frame = true;
            H264FramePosition = frameNo;
            _GetH264Frame = getframe;
            this.DBTime = dt;
        }

        public VideoFrame(ByteArrayBitmap bitmap, DateTime dbtime)
            : this(bitmap, dbtime, false)
        {
        }

        private void TrySetWidthHeight()
        {
            if (_bitmap != null)
            {
                _Width = _bitmap.Width;
                _Height = _bitmap.Height;
            }
        }

        private int _Width;
        public int Width
        {
            get
            {
                if (_bitmap != null)
                {
                    return _bitmap.Width;
                }
                else if (_Width == 0)
                {
                    if(EncodedData != null)
                        ByteArrayUtils.GetDimensionsOfImageFromJpegData(EncodedData, out _Width, out _Height);

                    return _Width;
                }
                else
                    return _Width;
            }
        }

        public void Rotate(System.Drawing.RotateFlipType rotate)
        {
            try
            {
                System.Drawing.Bitmap B = this.Bitmap.ToBitmap();
                B.RotateFlip(rotate);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    B.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                    B.Dispose();
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    byte[] data = new byte[memoryStream.Length];
                    memoryStream.Read(data, 0, (int)memoryStream.Length);
                    _bitmap = new ByteArrayBitmap(data);
                }
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }
        }
        
        private int _Height;
        public int Height
        {
            get
            {
                if (_bitmap != null)
                {
                    return _bitmap.Height;
                }
                else if (_Height == 0)
                {
                    if (EncodedData != null)
                        ByteArrayUtils.GetDimensionsOfImageFromJpegData(EncodedData, out _Width, out _Height);

                    return _Height;
                }
                else return _Height;
            }
        }

        public virtual byte[] EncodedData { get; private set; }
        public virtual ContentType EncodedDataContentType { get; set; }

        Bitmaps.FrameHashObject _FrameHash = new Bitmaps.FrameHashObject();
        public Bitmaps.FrameHashObject FrameHash { get { return _FrameHash; } set { _FrameHash = value; } }
        
        public DateTime? NextFrameTime { get; set; }
        public DateTime? PreviousFrameTime { get; set; }

        public DateTime DBTime { get; set; }
        public DateTime? UITime { get; set; }

        //public DateTime PingTime;

        /// <summary>
        /// return UITime if not null, otherwise, return DBTime
        /// </summary>
        /// <returns></returns>
        public DateTime GetUIorDBTime()
        {
            if (UITime != null && UITime.HasValue)
                return UITime.Value;
            else
                return DBTime;
        }

        public bool HasBitmap { get { return _bitmap != null; } }

        public bool RetainBitmap = true;


        /// <summary>
        /// if true, prevent expansion of Bitmap
        /// </summary>
        public bool StaySmall { get; set; }


        public ByteArrayBitmap Bitmap
        {
            get
            {
                if (StaySmall)
                {
                    return _bitmap;
                }

                if (_bitmap == null)
                    TryToDecodeEncodedDataTo_bitmap();
                if (RetainBitmap)
                {
                    return _bitmap;
                }
                else
                {
                    ByteArrayBitmap bmp = _bitmap.Clone() as ByteArrayBitmap;
                    this.TryDropBitmapData();
                    return bmp;
                }
            }
        }

        /// <summary>
        /// Returns current BitmaapSource scaled below the given dimensions
        /// WILL NOT store if it has to build it on the fly
        /// </summary>
        public BitmapSource BitmapSourceScaledToBelow(int width, int height)
        {
            try
            {
                ByteArrayBitmap bab;
                if (this._bitmap != null)
                {
                    bab = this._bitmap;
                }
                else
                {
                    bab = TryToDecodeEncodedData();
                    if (bab == null)
                    {
                        return null;
                    }
                }

                return bab.ToBitmapSourceScaledToBelow(width, height);
            }
            catch (Exception er)
            {
                DebugMessageLogger.LogError(er);
                return null;
            }
        }

        private ByteArrayBitmap _bitmap;
        private ByteArrayBitmap _bitmapDeinterlaced;
        private BitmapSource _bitmapSource;

        /// <summary>
        /// fills the bitmap data if it is not currently generated.
        /// </summary>
        public void DecodeToBitmapOnly()
        {
            if (RetainBitmap)
            {
                if (_bitmap == null || _bitmap.Data == null)
                    TryToDecodeEncodedDataTo_bitmap();

                // log access for drop on age calc
                _lastDecodedDataAccess = DateTime.Now;
            }
            else
            {
                throw new Exception();
            }
        }

        private DateTime _lastDecodedDataAccess = DateTime.Now;

        /// <summary>
        /// Drops Decoded data if found to be last accessed before given time
        /// </summary>
        /// <param name="oldIfLastAccessBefore"></param>
        public void TryDropDecodedDataIfOld(DateTime oldIfLastAccessBefore)
        {
            if (oldIfLastAccessBefore > _lastDecodedDataAccess)
                this.TryDropDecodedData();
        }


        /// <summary>
        /// Will Drop data if there is suitable encoded data available
        /// </summary>
        /// <returns></returns>
        public bool TryDropDecodedData()
        {
            if (this.EncodedData != null)
            {
                if (_bitmap != null)
                    _bitmap.Dispose();

                _bitmap = null;

                if (_bitmapDeinterlaced != null)
                    _bitmapDeinterlaced.Dispose();

                _bitmapDeinterlaced = null;

                _bitmapSource = null;
            }
            return (_bitmap == null);
        }
        /// <summary>
        /// Will Drop data if there is suitable encoded data available
        /// </summary>
        /// <returns></returns>
        public bool TryDropBitmapData()
        {
            if (this.EncodedData != null)
            {
                if (_bitmap != null)
                    _bitmap.Dispose();
                _bitmap = null;
            }
            return (_bitmap == null);
        }
        /// <summary>
        /// Will Drop data if there is suitable encoded data available
        /// </summary>
        /// <returns></returns>
        public bool TryDropBitmapDeinterlacedData()
        {
            if (this.EncodedData != null)
            {
                _bitmapDeinterlaced = null;
            }
            return (_bitmap == null);
        }

        /// <summary>
        /// Will Drop data if it exists
        /// </summary>
        /// <returns></returns>
        public bool TryDropBitmapSourceData()
        {
            if (_bitmapSource != null)
            {
                _bitmapSource = null;
            }

            return (_bitmapSource == null);
        }

        /// <summary>
        /// retuns the number of bytes used for bitmap data. Returns 0 if no bitmaps decoded
        /// </summary>
        /// <returns></returns>
        public long GetTotalSpaceCurrentlyUsedByBMPData()
        {
            long o = 0;
            if (_bitmap != null)
                o += _bitmap.Data.Length;
            if (_bitmapDeinterlaced != null)
                o += _bitmapDeinterlaced.Data.Length;

            return o;
        }

        //public void EncodeBitmap(BitmapEncoder encoder, WriteableBitmap wbmp)
        //{
        //    if (this.EncodedData == null)
        //    {
        //        if (wbmp == null 
        //            || wbmp.Width != this.Width
        //            || wbmp.Height != this.Height
        //            || wbmp.Format != this.Bitmap.Format)
        //        {
        //            this.EncodeBitmap(encoder);
        //            return;
        //        }

        //        wbmp.WritePixels(new System.Windows.Int32Rect(0, 0, Bitmap.Width, Bitmap.Height), this.Bitmap.Data, this.Bitmap.Stride, 0);
        //        encoder.Frames.Add(BitmapFrame.Create(wbmp));
        //        using (MemoryStream imageMemoryStream = new MemoryStream())
        //        {
        //            encoder.Save(imageMemoryStream);
        //            this.EncodedData = imageMemoryStream.ToArray();
        //            this.EncodedDataContentType =
        //                new ContentType(
        //                    encoder.CodecInfo.MimeTypes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0]);
        //        }
        //    }
        //}


        public void EncodeBitmap(BitmapEncoder encoder)
        {
            // no longer needed as we would often expect this
            //if (this.Bitmap == null)
            //    throw new InvalidOperationException("No bitmap to encode.");
            if (this.EncodedData == null)
            {
                encoder.Frames.Add(BitmapFrame.Create(this.Bitmap.ToBitmapSource()));
                using (MemoryStream imageMemoryStream = new MemoryStream())
                {
                    encoder.Save(imageMemoryStream);
                    this.EncodedData = imageMemoryStream.ToArray();
                    this.EncodedDataContentType =
                        new ContentType(
                            encoder.CodecInfo.MimeTypes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                }
            }
        }

        

        static int _countGCs = 0;

        public BitmapSource GetImageFromEncodedData()
        {
            BitmapSource bmp = null;
            try
            {
                BitmapDecoder decoder;
                using (MemoryStream memoryStream = new MemoryStream(this.EncodedData))
                    decoder = BitmapDecoder.Create(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

                BitmapFrame bitmapFrame = decoder.Frames[0];

                bmp = bitmapFrame;

                //this prevents big memory spikes on playback of HD video. 
                //if(_countGCs++ % 5 == 0)
                  //  GC.Collect();
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }
            return bmp;
        }
        
        private void TryToDecodeEncodedDataTo_bitmap()
        {
            try
            {
                lock (_disposelock)
                {
                    if (Disposed)
                        return;

                using (MemoryStream memoryStream = new MemoryStream(this.EncodedData))
                {
                    System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(memoryStream);
                    _bitmap = ByteArrayBitmap.FromBitmapForce4bpp(bmp);
                }

                    //this was the way it was done in 2.5 and below
                    //BitmapDecoder decoder;
                    //using (MemoryStream memoryStream = new MemoryStream(this.EncodedData))
                    //    decoder = BitmapDecoder.Create(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    //BitmapFrame bitmapFrame = decoder.Frames[0];
                    //_bitmap = new ByteArrayBitmap(bitmapFrame);

                    //string[] mimeTypes = decoder.CodecInfo.MimeTypes.Split(',');
                    //if (mimeTypes.Length > 0)
                    //   this.EncodedDataContentType = new ContentType(mimeTypes[0]);
                    // log access for drop on age calc

                    this.EncodedDataContentType = new ContentType("image/jpeg");
                    _lastDecodedDataAccess = DateTime.Now;

                    TrySetWidthHeight();
                }
            }
            catch (Exception ee)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("No Bitmap and No EncodedData: {0}.{1}", DBTime, DBTime.Millisecond));
                DebugMessageLogger.LogError(ee);
                _bitmap = null;
            }
        }

        private ByteArrayBitmap TryToDecodeEncodedData()
        {
            try
            {
                if (this.EncodedData == null)
                    return null;

                BitmapDecoder decoder;
                using (MemoryStream memoryStream = new MemoryStream(this.EncodedData))
                    decoder = BitmapDecoder.Create(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                BitmapFrame bitmapFrame = decoder.Frames[0];
                return new ByteArrayBitmap(bitmapFrame);

                string[] mimeTypes = decoder.CodecInfo.MimeTypes.Split(',');
                if (mimeTypes.Length > 0)
                    this.EncodedDataContentType = new ContentType(mimeTypes[0]);
                // log access for drop on age calc
                _lastDecodedDataAccess = DateTime.Now;

                TrySetWidthHeight();
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
                return null;
            }
        }

        public const string TimeStampFormat = "yyyy-MM-dd HH:mm:ss.fffffff";

        public override string ToString()
        {
            return this.DBTime.ToString(VideoFrame.TimeStampFormat);
        }

        Bitmap ConvertToBitmap(byte[] bytes)
        {
            Bitmap bitmap;
            using (MemoryStream bmpStream = new MemoryStream(bytes))
            {
                Image image = Image.FromStream(bmpStream);

                bitmap = new Bitmap(image);

            }
            return bitmap;
        }

        public BitmapVideoFrame ToBitmapVideoFrame()
        {
            
            BitmapVideoFrame bvf = new BitmapVideoFrame();
            Bitmap bmp = ConvertToBitmap(this.EncodedData);
            bvf.Bitmap = bmp;
            bvf.DBTime = this.DBTime;
            return bvf;

        }

        #region IDisposable Members
        public bool Disposed = false;
        object _disposelock = new object();

        public string _disposeCallLog;

        public static bool SuppressDispose = false;
        
        public void Dispose()
        {
            

            lock (_disposelock)
            {
                if (!this.Disposed)
                {
                    _disposeCallLog = Environment.StackTrace;
                    if (SuppressDispose)
                        return;

                    if (this._bitmap != null)
                    {
                        this._bitmap.Dispose();
                        this._bitmap = null;
                    }
                    if (this._bitmapDeinterlaced != null)
                    {
                        this._bitmapDeinterlaced.Dispose();
                        this._bitmapDeinterlaced = null;
                    }
                    if (this._bitmapSource != null)
                    {
                        this._bitmapSource = null;
                    }

                    //not needed.
                    //if (EncodedData != null)
                    //    ByteArrayBitmap.DeAllocateBytes(EncodedData);

                    this.FrameHash.Dispose();
                    
                    //this.NextFrameTime = null;
                    //this.PreviousFrameTime = null;
                    this.Disposed = true;
                }
            }
            GC.SuppressFinalize(this);
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Makes a copy, except for decoded pixels. 
        /// </summary>
        /// <returns></returns>
        public VideoFrame CloneEncodedOnly()
        {
            VideoFrame V;
            if (this.EncodedData != null)
                V = new VideoFrame(this.EncodedData.Clone() as byte[], this.EncodedDataContentType, this.DBTime);
            //else if (this.Bitmap != null)
            //  V = new VideoFrame(this.Bitmap, this.DBTime);
            else if (this.IsH264Frame)
                V = new VideoFrame(true, this.DBTime, this.H264FramePosition, _GetH264Frame);
            else
                return null;

            V.FrameHash = (FrameHashObject)this.FrameHash.Clone();


            V.H264FramePosition = this.H264FramePosition;
            V.IsH264Frame = this.IsH264Frame;

            //if (this._bitmap != null)
            //    V._bitmap = this._bitmap.Clone();

            //if (this._bitmapDeinterlaced != null)
            //    V._bitmapDeinterlaced = this._bitmapDeinterlaced.Clone();

            V._lastDecodedDataAccess = this._lastDecodedDataAccess;
            

            V.NextFrameTime = this.NextFrameTime;
            V.PreviousFrameTime = this.PreviousFrameTime;

            V._Width = this.Width;
            V._Height = this.Height;

            V.UITime = this.UITime;

            //V.TrySetWidthHeight();

            //V.PingTime = this.PingTime;

            return V;
        }

        public object Clone()
        {

            VideoFrame V;
            if (this.EncodedData != null)
                V = new VideoFrame(this.EncodedData.Clone() as byte[], this.EncodedDataContentType, this.DBTime);
            else if (this.HasBitmap && this.Bitmap != null)
                V = new VideoFrame(this.Bitmap, this.DBTime);
            else if (this.IsH264Frame)
                V = new VideoFrame(true, this.DBTime, this.H264FramePosition, _GetH264Frame);
            else
                return null;

            V.FrameHash = (FrameHashObject)this.FrameHash.Clone();

            V.H264FramePosition = this.H264FramePosition;
            V.IsH264Frame = this.IsH264Frame;

            if (this._bitmap != null)
                V._bitmap = this._bitmap.Clone();

            if (this._bitmapDeinterlaced != null)
                V._bitmapDeinterlaced = this._bitmapDeinterlaced.Clone();

            V._lastDecodedDataAccess = this._lastDecodedDataAccess;
            
            V.NextFrameTime = this.NextFrameTime;
            V.PreviousFrameTime = this.PreviousFrameTime;

            V._Width = this.Width;
            V._Height = this.Height;

            V.UITime = this.UITime;

            //V.TrySetWidthHeight();

            //V.PingTime = this.PingTime;

            return V;
        }

        #endregion

        public bool IsH264Frame { get; set; }
        public int  H264FramePosition { get; set; }
        public GetH264Frame _GetH264Frame;

        public void SetH264GetFrameDelegate(GetH264Frame dH264GetFrame)
        {
            _GetH264Frame = dH264GetFrame;
        }

        public BitmapSource GetFrameH264()
        {
            return _GetH264Frame(H264FramePosition);
        }
    }

    public delegate BitmapSource GetH264Frame(int framePos);
}

