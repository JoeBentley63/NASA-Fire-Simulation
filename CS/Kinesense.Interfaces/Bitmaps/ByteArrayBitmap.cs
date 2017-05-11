using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using Kinesense.Interfaces.Classes;
using Kinesense.Interfaces.Enum;
using System.Diagnostics;
using System.Threading;

namespace Kinesense.Interfaces
{
    public class ByteArrayBitmap : ICloneable, IDisposable
    {
        protected int _bytesPerPixel;
        public int BytesPerPixel
        {
            get { return _bytesPerPixel; }
            protected set { _bytesPerPixel = value; }
        }

        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public PixelFormat Format { get; protected set; }
        public BitmapPalette Pallet { get; protected set; }

        //private byte[][] _dataParts
        
        public virtual byte[] Data 
        {
            get; 
            protected set;
        }

        public byte[] DataGrayscale { get; protected set; }
        public byte[] DataHSV { get; protected set; }
        public byte[] DataLocal { get; protected set; }
        public HLSImage HLSImage { get; protected set; }
        private int DataGrayscaleDenominator = 3;
        public int Stride { get; protected set; }
        public bool IsDeinterlaced { get; set; }


        public static long CountByteActions = 0;
        public static long CountBytesAllocated = 0, CountBytesDeallocated = 0;

        //static Dictionary<int, int> AllocationMap = new Dictionary<int, int>();
        //static Dictionary<int, int> DeAllocationMap = new Dictionary<int, int>();

        //static System.ServiceModel.Channels.BufferManager BufferManager_1MB =
        //    System.ServiceModel.Channels.BufferManager.CreateBufferManager(100000000, 1000000);
        //static BufferManager _bufferManager = new BufferManager();

        public static BufferManagerManager BuffManMan = new BufferManagerManager();

        static Process p = Process.GetCurrentProcess();

        public static byte[] MakeBytes_NoBuffer(int length)
        {
            //System.Diagnostics.Debug.WriteLine("Make Bytes: {0} MB", length / 1000000);
            return new byte[length];
        }

        static Dictionary<string, int> Countpaths = new Dictionary<string, int>();
        DateTime _wasBorn = DateTime.Now;
        string OriginThread = Thread.CurrentThread.Name;
        string _stacktrace = System.Environment.StackTrace;

        public static byte[] MakeBytes(int length)
        {
            CountByteActions++;

            
            //{
            //    long diff = (CountBytesAllocated - CountBytesDeallocated) / 1000000;
            //    long mb = 1000000;
            //    p = Process.GetCurrentProcess();
            //    string s = string.Format("Byte Action {0}: Total Allocated {1} Total Deallocated {2} Balance {3} MB Mem {4} MB GC {5} MB",
            //        CountByteActions, CountBytesAllocated/mb, CountBytesDeallocated/mb, diff, p.PrivateMemorySize64 / mb, GC.GetTotalMemory(false)/mb);
                
            //    Debug.WriteLine(s);
            //    if (CountByteActions % 500 == 0)
            //        DebugMessageLogger.LogEvent(s);
            //}
            
            //if(AllocationMap.ContainsKey(length))
            //    AllocationMap[length]++;
            //else
            //    AllocationMap.Add(length, 1);

            CountBytesAllocated += length;

            //if (length > (1000000))
            //{

            //    string stacktrace = System.Environment.StackTrace;
            //    string name = Thread.CurrentThread.Name;

            //    if (Countpaths.ContainsKey(stacktrace))
            //        Countpaths[stacktrace]++;
            //    else
            //        Countpaths.Add(stacktrace, 1);

            //    double memdiff = (double)(CountBytesAllocated - CountBytesDeallocated) / 1000000d;

            //    if (memdiff > 150)
            //    {
            //        Console.WriteLine("Force Cleanup because Memdiff=" + memdiff);
            //        //Kinesense.Interfaces.Threading.ThreadSleepMonitor.Sleep(2500);
            //        GC.Collect();
            //    }

            //    Console.WriteLine(string.Format("Bytes {0} : {1:n}MB deal {2:n}MB GC {3:n}MB",
            //        name,
            //        memdiff,
            //        (double)CountBytesDeallocated / 1000000d,
            //        GC.GetTotalMemory(false) / 1000000d));
            //    //Console.WriteLine(stacktrace);
            //}

            ////byte[] bytes = _bufferManager.GetBuffer(length);
            //    //BufferManager_1MB.TakeBuffer(length);

            byte[] bytes =// BuffManMan.GetBuffer(length);

                new byte[length];
            return bytes;
        }

        public ByteArrayBitmap AddMargin(int xleft, int ytop, int xright, int ybottom)
        {
            int nwidth = this.Width + xleft + xright;
            int nheight = this.Height + ytop + ybottom;

            ByteArrayBitmap nbmp = new ByteArrayBitmap(nwidth, nheight, this.Format);
            for(int y = 0; y < this.Height; y++)
            {
                int lineoffset = (xleft * this.BytesPerPixel) + nbmp.Stride * y;
                Array.Copy(this.Data, (this.Stride * y), nbmp.Data, lineoffset, this.Stride);
            }

            return nbmp;
        }

        public static void LogDicts()
        {
            foreach(var d in Countpaths)
            {
                DebugMessageLogger.LogEvent(d.Value + " " + d.Key);
            }

        }

        public static byte[] CloneBytes(byte[] b)
        {
            byte[] cl = MakeBytes(b.Length);
            Array.Copy(b, cl, b.Length);
            return b.Clone() as byte[];
        }

        public static byte[] DeAllocateBytes(byte[] data)
        {
            CountBytesDeallocated += (data !=null?data.Length:0);
            return null;

            //if (data != null)
            //{
            //    int length = data.Length;
            //    BuffManMan.ReturnBuffer(data);
            //    //_bufferManager.ReturnBuffer(data);
            //    //BufferManager_1MB.ReturnBuffer(data);
            //    CountBytesDeallocated += length;
            //    //if (DeAllocationMap.ContainsKey(length))
            //    //    DeAllocationMap[length]++;
            //    //else
            //    //    DeAllocationMap.Add(length, 1);
            //}
            //return null;
        }

        //static int datacount = 0;

        public ByteArrayBitmap Clone()
        {
            return new ByteArrayBitmap(this);
        }

        public static int[,] Subtract_unsafe_bigdiff(ByteArrayBitmap one, ByteArrayBitmap two)
        {
            //byte[] data = new byte[one.Data.Length];
            int[,] intarray = new int[one.Width, one.Height];
            int maxpixel = one._bytesPerPixel;
            try
            {
                int xpos = 0, ypos = 0, pixelcount = 0;
                unsafe
                {
                    //fixed (byte* destDataStart = &data[0])
                    {
                        fixed (byte* sourceDataStartA = &one.Data[0])
                        {
                            fixed (byte* sourceDataStartB = &two.Data[0])
                            {
                                byte* sourceByteA = sourceDataStartA;
                                byte* sourceByteB = sourceDataStartB;
                                //byte* destByte = destDataStart;

                                for (int i = 0; i < one.Data.Length; i++)
                                {
                                    int val = Math.Abs(*sourceByteA - *sourceByteB);
                                    //*destByte = (byte)

                                    sourceByteA++;
                                    sourceByteB++;
                                    //destByte++;

                                    if (intarray[xpos, ypos] < val)
                                        intarray[xpos, ypos] = val;

                                    pixelcount++;
                                    if (pixelcount == maxpixel)
                                    {
                                        xpos++;
                                        pixelcount = 0;
                                    }
                                    if (xpos == one.Width)
                                    {
                                        ypos++;
                                        xpos = 0;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception er)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
            }
            return intarray;
        }

        /// <summary>
        /// Dear Dr Dan. Please pointer-ize me.
        /// </summary>
        /// <param name="toBlur"></param>
        /// <returns></returns>
        public static ByteArrayBitmap Blur(ByteArrayBitmap toBlur)
        {
            ByteArrayBitmap bmp = new ByteArrayBitmap(toBlur.Width, toBlur.Height, toBlur.Format);

            int[,] gaussian = new int[,] { 
            { 0, 0, 4 }, { 1, 0, 2 }, { 1, -1, 1 }, { 0, -1, 2 }, { -1, -1, 1 }, { -1, 0, 2 }, { -1, 1, 1 }, { 0, 1, 2 }, { 1, 1, 1 } };

            int bytesperpixel = toBlur._bytesPerPixel;
            for (int i = 1; i < bmp.Width - 1; i++)
                for (int j = 1; j < bmp.Height - 1; j++)
                {
                    for (int c = 0; c < bytesperpixel; c++)
                    {
                        int sum = 0;
                        for (int n = 0; n < 9; n++)
                            sum += toBlur.GetColorValue(i + gaussian[n, 0], j + gaussian[n, 1], c) * gaussian[n, 2];

                        bmp.SetColorValue(i, j, c, (byte)(sum / 16));
                    }
                }

            return bmp;
        }

        public ByteArrayBitmap(byte[] encodedData, bool decode)
        {
            if (decode)
                throw new IOException("Cant decode with this constructor");
            try
            {
                //BitmapDecoder decoder;
                //using (MemoryStream memoryStream = new MemoryStream(encodedData))
                //    decoder = BitmapDecoder.Create(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

                //BitmapFrame bitmapFrame = decoder.Frames[0];

                //this.Width = bitmapFrame.PixelWidth;
                //this.Height = bitmapFrame.PixelHeight;
                //this.Format = bitmapFrame.Format;
                //this.Pallet = bitmapFrame.Palette;
                //this.Stride = (bitmapFrame.PixelWidth * bitmapFrame.Format.BitsPerPixel) / 8;
                //_bytesPerPixel = bitmapFrame.Format.BitsPerPixel / 8;
                //this.Data = MakeBytes(this.Stride * this.Height);
                //bitmapFrame.CopyPixels(this.Data, this.Stride, 0);
            }
            catch (Exception er)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
                throw new Exception("Bad Encoded Data");
            }
        }

        public ByteArrayBitmap(byte[] encodedData)
        {
            try
            {
                BitmapDecoder decoder;
                using (MemoryStream memoryStream = new MemoryStream(encodedData))
                    decoder = BitmapDecoder.Create(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

                BitmapFrame bitmapFrame = decoder.Frames[0];



                this.Width = bitmapFrame.PixelWidth;
                this.Height = bitmapFrame.PixelHeight;
                this.Format = bitmapFrame.Format;
                this.Pallet = bitmapFrame.Palette;
                this.Stride = (bitmapFrame.PixelWidth * bitmapFrame.Format.BitsPerPixel) / 8;
                _bytesPerPixel = bitmapFrame.Format.BitsPerPixel / 8;
                this.Data = MakeBytes(this.Stride * this.Height);
                bitmapFrame.CopyPixels(this.Data, this.Stride, 0);
            }
            catch (Exception er)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
                throw new Exception("Bad Encoded Data");
            }
        }

        protected ByteArrayBitmap(ByteArrayBitmap bab)
        {
            this._bytesPerPixel = bab._bytesPerPixel;
            this.CropOffsetY = bab.CropOffsetY;
            this.CropOffsetX = bab.CropOffsetX;
            this.IsCropped = bab.IsCropped;
            this.Width = bab.Width;
            this.Height = bab.Height;
            this.Format = bab.Format;
            this.Pallet = bab.Pallet;
            this.Stride = bab.Stride;
            this.IsDeinterlaced = bab.IsDeinterlaced;

            if (bab.Data != null)
                this.Data = CloneBytes( bab.Data);//.Clone() as Byte[];
            else
                this.Data = null;

            if (bab.DataGrayscale != null)
                this.DataGrayscale = CloneBytes(bab.DataGrayscale);//.Clone() as Byte[];
            else
                this.DataGrayscale = null;

            if (bab.DataHSV != null)
                this.DataHSV = CloneBytes(bab.DataHSV);//.Clone() as Byte[];
            else
                this.DataHSV = null;

            if (bab.DataLocal != null)
                this.DataLocal = CloneBytes(bab.DataLocal);//.Clone() as Byte[];
            else
                this.DataLocal = null;

        }

        public ByteArrayBitmap(byte[] data, int stride, int width, int height, PixelFormat format)
        {
            if (stride < width)
                throw new ArgumentException("The stride cannot be less than the width", "stride");
            if (format.BitsPerPixel < 8)
                throw new NotSupportedException("The format must be at least 1 byte per pixel");

            this.Width = width;
            this.Height = height;
            this.Format = format;
            this.Stride = stride;
            _bytesPerPixel = format.BitsPerPixel / 8;

            //datacount++;
            //if (datacount % 100 == 0)
            //    System.Diagnostics.Debug.WriteLine("Datacount " + datacount.ToString());

            this.Data = data ?? MakeBytes(this.Stride * this.Height);
        }

        public ByteArrayBitmap(byte[] data, int stride, int width, int height, PixelFormat format, BitmapPalette pallet)
        {
            if (stride < width)
                throw new ArgumentException("The stride cannot be less than the width", "stride");
            if (format.BitsPerPixel < 8)
                throw new NotSupportedException("The format must be at least 1 byte per pixel");

            this.Width = width;
            this.Height = height;
            this.Format = format;
            this.Pallet = pallet;
            this.Stride = stride;
            _bytesPerPixel = format.BitsPerPixel / 8;

            //datacount++;
            //if (datacount % 100 == 0)
            //    System.Diagnostics.Debug.WriteLine("Datacount " + datacount.ToString());

            this.Data = data ?? MakeBytes(this.Stride * this.Height);
        }

        public ByteArrayBitmap(byte[] data, int width, int height, PixelFormat format)
            : this(data, data != null ? data.Length / height : (width * format.BitsPerPixel) / 8, width, height, format)
        {
        }

        public static ByteArrayBitmap FromFile(string filename)
        {
            return FromBitmap((Bitmap)Bitmap.FromFile(filename));
        }

        static int count = 0;
        public static ByteArrayBitmap FromBitmap(System.Drawing.Bitmap bmp)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                             bmp.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = bmpData.Stride * bmp.Height;
            byte[] data = MakeBytes(bytes);

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, data, 0, bytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            System.Windows.Media.PixelFormat wpfPixelformat = System.Windows.Media.PixelFormats.Bgr32;
            BitmapPalette wpfBitmapPallet = null;
            switch (bmpData.PixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    wpfPixelformat = PixelFormats.Bgra32;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    wpfPixelformat = PixelFormats.Bgr24;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                    wpfPixelformat = PixelFormats.Gray8;
                    wpfBitmapPallet = BitmapPalettes.Gray256Transparent;
                    break;
            }

            var b = new ByteArrayBitmap(data, bmpData.Stride, bmpData.Width, bmpData.Height, wpfPixelformat, wpfBitmapPallet);
            return b;
        }

        public static ByteArrayBitmap FromBitmapForce4bpp(System.Drawing.Bitmap bmp)
        {
            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                return FromBitmap(bmp);
            

            else
            {
                ByteArrayBitmap rtnbmp = null;
                //this could be done better by using c# code to paste around bytes
                using (Bitmap clone = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    using (Graphics g = Graphics.FromImage(clone))
                        g.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                    rtnbmp = FromBitmap(clone);
                }
                return rtnbmp;
            }
        }

        public ByteArrayBitmap(BitmapSource bitmapSource)
            : this(bitmapSource.PixelWidth, bitmapSource.PixelHeight, bitmapSource.Format)
        {
            try
            {


                //int bitmapsourcedim = (int)( bitmapSource.Width * bitmapSource.Height * (bitmapSource.Format.BitsPerPixel == 32 ? 4 : 3));
                //if (this.Data.Length < bitmapsourcedim)
                //{
                //    DeAllocateBytes(this.Data);
                //    this.Data = MakeBytes(bitmapsourcedim);
                //}
                
                bitmapSource.CopyPixels(this.Data, this.Stride, 0);
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }
        }

        public ByteArrayBitmap(int width, int height, PixelFormat format)
            : this(null, width, height, format)
        {
        }

        public ByteArrayBitmap(IntPtr dataPointer, int stride, int width, int height, PixelFormat format)
            : this(null, stride, width, height, format)
        {
            Marshal.Copy(dataPointer, this.Data, 0, this.Data.Length);
        }

        public byte GetColorValue(int i, int j, int c)
        {
            return this.Data[(j * this.Stride) + (i * _bytesPerPixel) + c];
        }

        public byte[] GetColor(int i, int j)
        {
            byte[] color = new byte[_bytesPerPixel];
            int pos = (j * this.Stride) + (i * _bytesPerPixel);

            for(int c = 0; c < _bytesPerPixel; c++)
                color[c] = this.Data[pos + c];
            
            return color;
        }

        public void GetColor(int i, int j, int[] color)
        {
            int pos = (j * this.Stride) + (i * _bytesPerPixel);
            color[0] = this.Data[pos + 0];
            color[1] = this.Data[pos + 1];
            color[2] = this.Data[pos + 2];
        }

        public void GetColor(int i, int j, byte[] color)
        {
            int pos = (j * this.Stride) + (i * _bytesPerPixel);
            color[0] = this.Data[pos + 0];
            color[1] = this.Data[pos + 1];
            color[2] = this.Data[pos + 2];
        }

        public float[] GetAverageColorData()
        {
            float[] avgcolor = new float[6];

            for (int i = 0; i < Data.Length; )
            {
                for (int c = 0; c < 3; c++)
                    avgcolor[c] += Data[i++];

                if(this.BytesPerPixel > 3)
                    i++;
            }

            float max = 0;
            for (int c = 0; c < 3; c++)
            {
                avgcolor[c] /= (this.Width * this.Height);
                if(max < avgcolor[c])
                    max = avgcolor[c];
            }

            //get max of 3
            avgcolor[3] = max;

            return avgcolor;
        }

        public float[] GetAverageColor()
        {
            float[] avgcolor = new float[3];
            int step = 5;
            for (int i = 0; i < this.Width; i += step)
                for (int j = 0; j < this.Height; j += step)
                {
                    byte[] color = this.GetColor(i, j);
                    for (int c = 0; c < 3; c++)
                        avgcolor[c] += color[c];

                }

            for (int c = 0; c < 3; c++)
                avgcolor[c] /= (this.Width * this.Height) / (step * step);

            return avgcolor;
        }

        public void SetColorValue(int i, int j, int color, byte val)
        {
            this.Data[(j * this.Stride) + (i * _bytesPerPixel) + color] = val;
        }

        public ByteArrayBitmap ToGrayScale()
        {
            ByteArrayBitmap grayscaleImage = new ByteArrayBitmap(this.Width, this.Height, PixelFormats.Gray8);
            for (int i = 0; i < this.Width; i++)
                for (int j = 0; j < this.Height; j++)
                    grayscaleImage.SetColorValue(i, j, 0, (byte)this.GetGrayscaleValue(i, j));
            return grayscaleImage;
        }


        public ByteArrayBitmap ToMaxColorChannel()
        {
            ByteArrayBitmap grayscaleImage = new ByteArrayBitmap(this.Width, this.Height, PixelFormats.Gray8);
            for (int i = 0; i < this.Width; i++)
                for (int j = 0; j < this.Height; j++)
                {
                    byte[] pixel = this.GetColor(i, j);
                    grayscaleImage.SetColorValue(i, j, 0, (byte)Math.Max(pixel[0], Math.Max(pixel[1], pixel[2])));
                }
            return grayscaleImage;
        }

        /// <summary>
        /// replaces every second row of pixels with the previous row
        /// </summary>
        public ByteArrayBitmap Deinterlace()
        {
            ByteArrayBitmap deinterlacedImage = new ByteArrayBitmap(this.Width, this.Height, this.Format);
            for (int i = 0; i < this.Width; i++)
                for (int j = 1; j < this.Height; j += 2)
                    for (int c = 0; c < 3; c++)
                    {
                        deinterlacedImage.SetColorValue(i, j, c, GetColorValue(i, j, c));
                        deinterlacedImage.SetColorValue(i, j - 1, c, GetColorValue(i, j, c));
                    }
            return deinterlacedImage;
        }

        public ByteArrayBitmap DoubleHeight()
        {
            int height = this.Height * 2;
            ByteArrayBitmap deinterlacedImage = new ByteArrayBitmap(this.Width, height, this.Format);

            try
            {

                for (int j = 0; j < height; j += 2)
                {
                    int linestart = (j / 2) * this.Stride;
                    Array.Copy(this.Data, linestart, deinterlacedImage.Data, (j * this.Stride), this.Stride);
                    Array.Copy(this.Data, linestart, deinterlacedImage.Data, (j + 1) * this.Stride, this.Stride);
                }
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }
            return deinterlacedImage;

        }


        /// <summary>
        /// replaces every second row of pixels with the previous row
        /// </summary>
        public ByteArrayBitmap Deinterlace(DeinterlaceMode mode)
        {
            return Deinterlace(mode, false, true);
        }

        public ByteArrayBitmap Deinterlace(DeinterlaceMode mode, bool ignoreCachedImage, bool maintainImageSize)
        {
            if(ignoreCachedImage)
            if (IsDeinterlaced)
                return this;

            ByteArrayBitmap deinterlacedImage = null;
            switch (mode)
            {
                case DeinterlaceMode.SkipField1:
                    //fast method
                    {
                        int offset = 0;
                        if (!maintainImageSize)
                        {
                            int height = 0;
                            for (int j = 0; j < this.Height; j += 2)
                                height++;
                            
                            deinterlacedImage = new ByteArrayBitmap(this.Width, height, this.Format);
                        }
                        else
                        {
                            offset = this.Height / 4;
                            deinterlacedImage = new ByteArrayBitmap(this.Width, this.Height, this.Format);
                        }

                        for (int j = 0; j < this.Height; j += 2)
                        {
                            Array.Copy(this.Data, j * this.Stride, deinterlacedImage.Data, (j / 2 + offset) * this.Stride, this.Stride);
                        }
                    }
                    break;
                case DeinterlaceMode.SkipField2:
                    //fast method
                    {
                        int offset = 0;
                        if (!maintainImageSize)
                        {
                            int height = 0;
                            for (int j = 0; j < this.Height; j += 2)
                                height++;

                            deinterlacedImage = new ByteArrayBitmap(this.Width, height, this.Format);
                        }
                        else
                        {
                            offset = this.Height / 4;
                            deinterlacedImage = new ByteArrayBitmap(this.Width, this.Height, this.Format);
                        }

                        //fix for non-even heights
                        int maxJ = this.Height;
                        if (maxJ % 2 == 1)
                            maxJ--;

                        for (int j = 0; j < maxJ; j += 2)
                        {
                            Array.Copy(this.Data, (j + 1) * this.Stride, deinterlacedImage.Data, (j / 2 + offset) * this.Stride, this.Stride);
                        }
                    }
                    break;
                case DeinterlaceMode.RepeatField1:
                    //fast method
                    {
                        deinterlacedImage = new ByteArrayBitmap(this.Width, this.Height, this.Format);

                        //fix for non-even heights
                        int maxJ = this.Height;
                        if (maxJ % 2 == 1)
                            maxJ--;

                        for (int j = 0; j < maxJ; j += 2)
                        {
                            Array.Copy(this.Data, j * this.Stride, deinterlacedImage.Data, j * this.Stride, this.Stride);
                            Array.Copy(this.Data, j * this.Stride, deinterlacedImage.Data, (j + 1) * this.Stride, this.Stride);
                        }
                    }
                    break;
                case DeinterlaceMode.RepeatField2:
                    //fast method
                    {
                        deinterlacedImage = new ByteArrayBitmap(this.Width, this.Height, this.Format);

                        //fix for non-even heights
                        int maxJ = this.Height;
                        if (maxJ % 2 == 1)
                            maxJ--;

                        for (int j = 0; j < maxJ; j += 2)
                        {
                            Array.Copy(this.Data, (j + 1) * this.Stride, deinterlacedImage.Data, j * this.Stride, this.Stride);
                            Array.Copy(this.Data, (j + 1) * this.Stride, deinterlacedImage.Data, (j + 1) * this.Stride, this.Stride);
                        }
                    }
                    break;
                case DeinterlaceMode.SideBySideSkipFields:
                    {
                        //Horizontally side-by-side

                        //deinterlacedImage = new ByteArrayBitmap(this.Width * 2, this.Height/2, this.Format);
                        //for (int i = 0; i < this.Width; i++)
                        //    for (int j = 0; j < this.Height; j += 2)
                        //        for (int c = 0; c < 3; c++)
                        //        {
                        //            deinterlacedImage.SetColorValue(i, j/2, c, GetColorValue(i, j, c));
                        //            deinterlacedImage.SetColorValue(i + this.Width, j/2, c, GetColorValue(i, j + 1, c));
                        //        }

                        //vertically side-by-side. Makes more sense
                        deinterlacedImage = new ByteArrayBitmap(this.Width, this.Height, this.Format);
                        //fast method
                        {
                            deinterlacedImage = new ByteArrayBitmap(this.Width, this.Height, this.Format);
                            int step = this.Height / 2;

                            //fix for non-even heights
                            int maxJ = this.Height;
                            if (maxJ % 2 == 1)
                                maxJ--;

                            for (int j = 0; j < maxJ; j += 2)
                            {
                                Array.Copy(this.Data, j * this.Stride, deinterlacedImage.Data, (j / 2) * this.Stride, this.Stride);
                                Array.Copy(this.Data, (j + 1) * this.Stride, deinterlacedImage.Data, (j / 2 + step) * this.Stride, this.Stride);
                            }
                        }

                        //slow method
                        //int step = this.Height / 2;
                        //for (int i = 0; i < this.Width; i++)
                        //    for (int j = 0; j < this.Height; j += 2)
                        //        for (int c = 0; c < 3; c++)
                        //        {
                        //            deinterlacedImage.SetColorValue(i, j / 2, c, GetColorValue(i, j, c));
                        //            deinterlacedImage.SetColorValue(i, j / 2 + step, c, GetColorValue(i, j + 1, c));
                        //        }
                    }
                    break;
                case DeinterlaceMode.SideBySideRepeatFields:
                    ByteArrayBitmap bab1 = this.Deinterlace(DeinterlaceMode.RepeatField1);
                    ByteArrayBitmap bab2 = this.Deinterlace(DeinterlaceMode.RepeatField2);

                    deinterlacedImage = new ByteArrayBitmap(this.Width * 2, Math.Min(bab1.Height, bab2.Height), this.Format);

                    for (int j = 0; j < Math.Min(bab1.Height, bab2.Height); j++)
                    {
                        Array.Copy(bab1.Data, j * bab1.Stride, deinterlacedImage.Data, 2*j * bab1.Stride, bab1.Stride);
                        Array.Copy(bab2.Data, j * bab2.Stride, deinterlacedImage.Data, (2*j * bab2.Stride) + bab1.Stride, bab2.Stride);
                    }

                    break;
                case DeinterlaceMode.Normal:
                    return this;
                    break;
            }
            if (ignoreCachedImage)
            {
                if (deinterlacedImage == null)
                    deinterlacedImage = this;

                deinterlacedImage.IsDeinterlaced = true;
            }
            return deinterlacedImage;
        }

        public ByteArrayBitmap ScaleAndLetterbox(int w, int h)
        {
            if (this.Width == w && this.Height == h)
                return this;

            ByteArrayBitmap outmap = null;// = new ByteArrayBitmap(w, h, this.Format);

            try
            {
                // get aspect ratio of destination
                double aspect_dest = (double)w / (double)h;
                // get aspect ratio of source
                double aspect_source = (double)this.Width / (double)this.Height;

                if (aspect_dest == aspect_source)
                {
                    //just scale
                    outmap = this.GetResized(w, h);
                }
                else if (aspect_dest < aspect_source)
                {
                    //scale and letter box (sides)
                    double scale = (double)w / (double)this.Width;
                    int newWidth = (int)(this.Width * scale);
                    int newHeight = (int)(this.Height * scale);
                    int letterbox = (h - newHeight) / 2;
                    outmap = this.GetResizedWithOffset(w, h, newWidth, newHeight, 0, letterbox);
                }
                else
                {
                    //scale and letterbox (top & bottom)
                    double scale = (double)h / (double)this.Height;
                    int newWidth = (int)(this.Width * scale);
                    int newHeight = (int)(this.Height * scale);
                    int letterbox = (w - newWidth) / 2;
                    outmap = this.GetResizedWithOffset(w, h, newWidth, newHeight, letterbox, 0);
                }
            }
            catch (Exception e)
            {
                DebugMessageLogger.LogError(e);
            }

            //outmap.ToBitmap().Save(@"D:\Databases\Demo2013\VideoClips\template" + DateTime.Now.Ticks.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);

            return outmap;
        }

        public ByteArrayBitmap Stretch(StretchMode mode)
        {
            ByteArrayBitmap stretchedImage = null;
            try
            {
                switch (mode)
                {
                    case StretchMode.DoubleHeight:
                        {
                            stretchedImage = new ByteArrayBitmap(this.Width, 2*this.Height, this.Format);
                            for (int j = 0; j < this.Height; j++)
                            {
                                Array.Copy(this.Data, j * this.Stride, stretchedImage.Data, (2*j) * this.Stride, this.Stride);
                                Array.Copy(this.Data, j * this.Stride, stretchedImage.Data, ((2 * j)+1) * this.Stride, this.Stride);
                            }
                        }
                        break;
                    case StretchMode.TrippleHeight:
                        {
                            stretchedImage = new ByteArrayBitmap(this.Width, 3 * this.Height, this.Format);
                            for (int j = 0; j < this.Height; j++)
                            {
                                Array.Copy(this.Data, j * this.Stride, stretchedImage.Data, (3 * j) * this.Stride, this.Stride);
                                Array.Copy(this.Data, j * this.Stride, stretchedImage.Data, ((3 * j) + 1) * this.Stride, this.Stride);
                                Array.Copy(this.Data, j * this.Stride, stretchedImage.Data, ((3 * j) + 2) * this.Stride, this.Stride);
                            }
                        }
                        break;
                    case StretchMode.None:
                    default:
                        break;

                }
            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
            }
            if (stretchedImage == null)
                stretchedImage = this;
            return stretchedImage;
        }


        public byte[] EncodeBitmap(BitmapEncoder encoder)
        {
            byte[] data = null;
            try
            {
                encoder.Frames.Add(BitmapFrame.Create(this.ToBitmapSource()));
                using (MemoryStream imageMemoryStream = new MemoryStream())
                {
                    encoder.Save(imageMemoryStream);
                    data = imageMemoryStream.ToArray();
                }
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }
            return data;
        }

        public ByteArrayBitmap GetSubSampledBytestream(int x, int y, int cropWidth, int cropHeight, int newWidth, int newHeight)
        {
            try
            {
                ByteArrayBitmap smallImage = new ByteArrayBitmap(newWidth, newHeight, this.Format);
                float fWidth = cropWidth, fHeight = cropHeight;
                float fNewWidth = newWidth, fNewHeight = newHeight;
                float xStep = fWidth / fNewWidth;
                float yStep = fHeight / fNewHeight;

                //subsample
                for (float i = 0; i < fNewWidth; i++)
                    for (float j = 0; j < fNewHeight; j++)

                        //todo: copy array, its faster
                        for (int c = 0; c < _bytesPerPixel; c++)
                            smallImage.SetColorValue((int)i, (int)j, c,
                                GetColorValue((int)(i * xStep) + x, (int)(j * yStep) + y, c));
                return smallImage;
            }
            catch (Exception er)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
                return null;
            }
        }



        /// <summary>
        /// test code. Doesn't work very well
        /// </summary>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public ByteArrayBitmap GetBiLinearResize(int newWidth, int newHeight)
        {
            ByteArrayBitmap smallImage = new ByteArrayBitmap(newWidth, newHeight, this.Format);
            float fWidth = this.Width, fHeight = this.Height;
            float fNewWidth = newWidth, fNewHeight = newHeight;
            float xStep = fWidth / fNewWidth;
            float yStep = fHeight / fNewHeight;

            int[][] samplepoints = new int[4][];
            samplepoints[0] = new int[] { -1, 0 };
            samplepoints[1] = new int[] { 0, -1 };
            samplepoints[2] = new int[] { +1, 0 };
            samplepoints[3] = new int[] { 0, +1 };

            for (float i = 1; i < fNewWidth - 1; i++)
                for (float j = 1; j < fNewHeight - 1; j++)
                    for (int c = 0; c < _bytesPerPixel; c++)
                    {
                        int cval = 0;
                        for (int p = 0; p < 4; p++)
                            cval += GetColorValue((int)(i * xStep) + samplepoints[p][0], (int)(j * yStep) + samplepoints[p][1], c);

                        smallImage.SetColorValue((int)i, (int)j, c, (byte)(cval / 4));
                    }


            return smallImage;

        }

        private ByteArrayBitmap GetSubSampledBytestream_old(int newWidth, int newHeight)
        {
            ByteArrayBitmap smallImage = new ByteArrayBitmap(newWidth, newHeight, this.Format);
            float fWidth = this.Width, fHeight = this.Height;
            float fNewWidth = newWidth, fNewHeight = newHeight;
            float xStep = fWidth / fNewWidth;
            float yStep = fHeight / fNewHeight;

            //subsample
            for (float i = 0; i < fNewWidth; i++)
                for (float j = 0; j < fNewHeight; j++)
                    for (int c = 0; c < _bytesPerPixel; c++)
                        smallImage.SetColorValue((int)i, (int)j, c, GetColorValue((int)(i * xStep), (int)(j * yStep), c));
            return smallImage;
        }



        public bool IsCropped { get; set; }
        public int CropOffsetX { get; set; }
        public int CropOffsetY { get; set; }

        public ByteArrayBitmap Crop(int x, int y, int width, int height)
        {
            if (x < 0 || y < 0 || x + width > this.Width || y + height > this.Height)
                throw new Exception("Dimensions for crop not within image");

            if (x == 0 && y == 0 && width == this.Width && height == this.Height)
                return this.Clone();

            ByteArrayBitmap c = new ByteArrayBitmap(width, height, this.Format);

            c.IsCropped = true;
            c.CropOffsetX = x;
            c.CropOffsetY = y;

            for (int j = 0; j < height && j < this.Height; j++)
            {
                int linestartpos = (y + j) * this.Stride + x * _bytesPerPixel;

                if (linestartpos < this.Data.Length)
                {
                    int dpos = j * c.Stride;
                    int len = width * _bytesPerPixel;
                    if (dpos < c.Data.Length)
                    {
                        if (len + dpos > c.Data.Length)
                            len = c.Data.Length - dpos;
                        if (linestartpos + len > this.Data.Length)
                            len = this.Data.Length - linestartpos;
                        //copy line of bytes
                        Array.Copy(this.Data, linestartpos, c.Data, dpos, len);
                    }
                }
            }

            return c;
        }

        public static bool CopySubSampledBytestream(
            int x,
            int y,
            int cropWidth,
            int cropHeight,
            int newWidth,
            int newHeight,
            ByteArrayBitmap inimage,
            ByteArrayBitmap outimage)
        {
            //if (inimage.Width != outimage.Width || inimage.Height != outimage.Height || inimage.Format != outimage.Format)
            //	return false;


            float fWidth = cropWidth, fHeight = cropHeight;
            float fNewWidth = newWidth, fNewHeight = newHeight;
            float xStep = fWidth / fNewWidth;
            float yStep = fHeight / fNewHeight;

            //subsample
            for (float i = 0; i < fNewWidth; i++)
                for (float j = 0; j < fNewHeight; j++)
                {
                    int read_pos = (int)((((int)(j * yStep) + y) * inimage.Stride) + (((int)(i * xStep) + x) * inimage._bytesPerPixel));
                    int write_pos = (int)((j * outimage.Stride) + (i * outimage._bytesPerPixel));
                    Array.Copy(
                        inimage.Data,
                        read_pos,
                        outimage.Data,
                        write_pos,
                        outimage._bytesPerPixel);
                }
            //for (int c = 0; c < inimage._bytesPerPixel; c++)
            //    outimage.SetColorValue((int)i, (int)j, c, 
            //        inimage.GetColorValue((int)(i * xStep) + x, (int)(j * yStep) + y, c));

            //else
            return true;
        }

        public static bool CopySubSampledBytestream(int newWidth, int newHeight, ByteArrayBitmap inimage, ByteArrayBitmap outimage)
        {
            float fWidth = inimage.Width, fHeight = inimage.Height;
            float fNewWidth = newWidth, fNewHeight = newHeight;
            float xStep = fWidth / fNewWidth;
            float yStep = fHeight / fNewHeight;

            //subsample
            for (float i = 0; i < fNewWidth; i++)
                for (float j = 0; j < fNewHeight; j++)
                {
                    int read_pos = (int)(((int)(j * yStep) * inimage.Stride) + ((int)(i * xStep) * inimage._bytesPerPixel));
                    int write_pos = (int)((j * outimage.Stride) + (i * outimage._bytesPerPixel));
                    Array.Copy(
                        inimage.Data,
                        read_pos,
                        outimage.Data,
                        write_pos,
                        outimage._bytesPerPixel);
                }

            //else
            return true;
        }

        /// <summary>
        /// fastest code in the west
        /// </summary>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public ByteArrayBitmap GetResized(int x, int y, int cropWidth, int cropHeight, int newWidth, int newHeight)
        {
            Bitmap newImage = new Bitmap(newWidth, newHeight);
            using (Graphics gr = Graphics.FromImage(newImage))
            {
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                gr.DrawImage(this.ToBitmap(), new Rectangle(0, 0, newWidth, newHeight));
            }

            return ByteArrayBitmap.FromBitmap(newImage);
        }

        /// <summary>
        /// fastest code in the west
        /// </summary>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public ByteArrayBitmap GetResized(int newWidth, int newHeight)
        {
            Bitmap newImage = new Bitmap(newWidth, newHeight);
            using (Graphics gr = Graphics.FromImage(newImage))
            {
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                gr.DrawImage(this.ToBitmap(), new Rectangle(0, 0, newWidth, newHeight));
            }

            return ByteArrayBitmap.FromBitmap(newImage);
        }

        public ByteArrayBitmap GetResizedWithOffset(int newWidth, int newHeight, int imageWidth, int imageHeight, int x_offset, int y_offset)
        {
            Bitmap newImage = new Bitmap(newWidth, newHeight);
            using (Graphics gr = Graphics.FromImage(newImage))
            {
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                gr.DrawImage(this.ToBitmap(), new Rectangle(x_offset, y_offset, imageWidth, imageHeight));
            }

            return ByteArrayBitmap.FromBitmap(newImage);
        }

        public ByteArrayBitmap EnlargeNative(int newWidth, int newHeight)
        {
            ByteArrayBitmap bigImage = new ByteArrayBitmap(newWidth, newHeight, this.Format);

            float fWidth = this.Width, fHeight = this.Height;
            float fNewWidth = newWidth, fNewHeight = newHeight;
            
            float xStep = fNewWidth / fWidth;
            float yStep = fNewHeight / fHeight;

            //nearest neighbour
                
            for (int j = 0; j < this.Height; j++)
                for (int i = 0; i < this.Width; i++)
                {
                    float jp = j * yStep;
                    float ip = i * xStep;
                    byte[] color = this.GetColor(i, j);
                    for(int x = 0; x < xStep; x++)
                        for (int y = 0; y < yStep; y++)
                        {
                            bigImage.SetColor((int)(ip + x), (int)(jp + y), color);
                        }
                }

            return bigImage;
        }

        public ByteArrayBitmap ShrinkNative(int newWidth, int newHeight)
        {
            ByteArrayBitmap smallImage = new ByteArrayBitmap(newWidth, newHeight, this.Format);

            float fWidth = this.Width, fHeight = this.Height;
            float fNewWidth = newWidth, fNewHeight = newHeight;
            int xStep = (int)(fWidth / fNewWidth);
            int yStep = (int)(fHeight / fNewHeight);

            //neighbour average
            for (float i = 0; i < fNewWidth; i++)
                for (float j = 0; j < fNewHeight; j++)
                    for (int c = 0; c < _bytesPerPixel; c++)
                    {
                        int sumcolor = 0;
                        for (int x = 0; x < xStep; x++)
                            for (int y = 0; y < yStep; y++)
                            {
                                sumcolor += this.GetColorValue((int)(i * xStep + x), (int)(j * yStep + y), c);
                            }
                        sumcolor /= (xStep * yStep);
                        sumcolor = (sumcolor > 255) ? 255 : sumcolor;
                        smallImage.SetColorValue((int)i, (int)j, c, (byte)sumcolor);
                    }

            return smallImage;

        }

        public int GetGrayscaleValue(int i, int j)
        {
            int grayscale = 0;
            for (int c = 0; c < _bytesPerPixel && c < 3; c++)
                grayscale += this.Data[(j * this.Stride) + (i * _bytesPerPixel) + c];

            return grayscale / _bytesPerPixel;
        }

        public BitmapSource ToBitmapSource()
        {
            return this.ToBitmapSource(96, 96);
        }

        public BitmapSource ToBitmapSource(double dpiX, double dpiY)
        {
            try
            {
                var bitmapSource = BitmapSource.Create(this.Width, this.Height, dpiX, dpiY, this.Format, this.Pallet, this.Data, this.Stride);
                bitmapSource.Freeze();
                return bitmapSource;
            }
            catch (Exception er)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
                return null;
            }
        }

        public void SetColor(int i, int j, byte[] median)
        {
            for (int c = 0; c < median.Length; c++)
                this.Data[(j * this.Stride) + (i * _bytesPerPixel) + c] = median[c];
        }

        public void SetColor(int i, int j, System.Drawing.Color colour)
        {
            this.Data[(j * this.Stride) + (i * _bytesPerPixel) + 0] = colour.R;
            this.Data[(j * this.Stride) + (i * _bytesPerPixel) + 1] = colour.G;
            this.Data[(j * this.Stride) + (i * _bytesPerPixel) + 2] = colour.B;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public override bool Equals(object obj)
        {
            ByteArrayBitmap other = obj as ByteArrayBitmap;
            if (other == null || other.Format != this.Format ||
                other.Height != this.Height || other.Width != this.Width ||
                other.Stride != this.Stride)
                return false;

            bool toRet = ByteArrayUtils.AreEqual(this.Data, other.Data);
            return toRet;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public byte[] ToJpegBytes()
        {
            var encoder = new JpegBitmapEncoderFactory(JpegBitmapEncoderFactory.DefaultJpegQuality).GetBitmapEncoder();
            byte[] encodedData = this.EncodeBitmap(encoder);
            return encodedData;
        }

        public System.Drawing.Bitmap ToBitmap()
        {
            try
            {
                //BitmapSource.Create(this.Width, this.Height, 96d, 96d, this.Format, null, this.Data, this.Stride);
                System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format32bppRgb;

                //if (this._bytesPerPixel == 3)
                if (this.Format == PixelFormats.Gray8)
                {
                    //this isn't greyscale, but System Drawing doesn't have an 8bit Greyscale 
                    format = System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
                }
                else if (this.Format == PixelFormats.Rgb24)
                {
                    format = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                }
                else if (this.Format == PixelFormats.Bgr24)
                {
                    format = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                }
                else if (this.Format == PixelFormats.Indexed8)
                {
                    format = System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
                }

                Bitmap bmp = new Bitmap(this.Width, this.Height, format);

                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                System.Drawing.Imaging.BitmapData bmpData =
                    bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    bmp.PixelFormat);

                // Get the address of the first line.
                IntPtr ptr = bmpData.Scan0;

                // Declare an array to hold the bytes of the bitmap.
                int bytes = bmpData.Stride * bmp.Height;

                if (bytes > Data.Length)
                    bytes = Data.Length;

                System.Runtime.InteropServices.Marshal.Copy(Data, 0, ptr, bytes);

                // Unlock the bits.
                bmp.UnlockBits(bmpData);

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

        /// <summary>
        /// An attempt at replicating the above code using pointer arathamtic
        /// </summary>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public unsafe ByteArrayBitmap GetSubSampledBytestream(int newWidth, int newHeight)
        {
            ByteArrayBitmap smallImage = new ByteArrayBitmap(newWidth, newHeight, this.Format);
            float fWidth = this.Width, fHeight = this.Height;
            float fNewWidth = newWidth, fNewHeight = newHeight;
            float xStep = fWidth / fNewWidth;
            float yStep = fHeight / fNewHeight;

            // new
            // Due to an accurate, but insanely difficult rounding system used by mark in the
            // previous function (and sub functions) we have to do a little clever tracking to
            // ensure our floats round correctly.
            unsafe
            {
                fixed (byte* sourceDataStart = &this.Data[0])
                {
                    fixed (byte* destDataStart = &smallImage.Data[0])
                    {
                        int Pval = 0;
                        int Ptarget = 0;

                        byte* sourceByte = sourceDataStart;
                        byte* destByte = destDataStart;
                        for (float i = 0; i < fNewHeight; i++)
                        {
                            sourceByte = sourceDataStart;
                            sourceByte += (int)(i * yStep) * this.Stride;
                            Pval = -1;
                            Ptarget = 0;
                            for (float j = 0; j < fNewWidth; j++)
                            {
                                Pval++;
                                Ptarget = (int)(j * xStep);
                                sourceByte += ((Ptarget - Pval) * _bytesPerPixel);
                                Pval = Ptarget;
                                for (int c = 0; c < _bytesPerPixel; c++)
                                {
                                    *destByte = *sourceByte;
                                    destByte++;
                                    sourceByte++;
                                }
                            }
                        }
                    }
                }
            }
            return smallImage;
        }

        /// <summary>
        /// An attempt at replicating the above code using pointer arithmetic
        /// </summary>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public unsafe ByteArrayBitmap GetSubSampledBytestream_Bilinear_V1(int newWidth, int newHeight)
        {
            ByteArrayBitmap smallImage = new ByteArrayBitmap(newWidth, newHeight, this.Format);
            float fWidth = this.Width, fHeight = this.Height;
            float fNewWidth = newWidth, fNewHeight = newHeight;
            float xStep = (fWidth - 2) / fNewWidth;
            float yStep = (fHeight - 2) / fNewHeight;

            // new
            // Due to an accurate, but insanely difficult rounding system used by mark in the
            // previous function (and sub functions) we have to do a little clever tracking to
            // ensure our floats round correctly.

            byte[] intermediateData = MakeBytes(5 * newWidth * newHeight * _bytesPerPixel);

            // pixels gathered
            // 1 2
            //  3
            // 4 5
            unsafe
            {
                fixed (byte* sourceDataStart = &this.Data[0])
                {
                    fixed (byte* destDataStart = &intermediateData[0])
                    {
                        int Pval = 0;
                        int Ptarget = 0;

                        byte* sourceByte = sourceDataStart;
                        byte* destByte = destDataStart;
                        for (float i = 0; i < fNewHeight; i++)
                        {

                            // wiggle factor for interpolation
                            for (int k = -1; k < 2; k++)
                            {
                                sourceByte = sourceDataStart;
                                // move to start of row
                                //int ss = ((int)(i * yStep) + 1 + k) * this.Stride;
                                sourceByte += ((int)(i * yStep) + 1 + k) * this.Stride;
                                Pval = -1;
                                Ptarget = 0;
                                for (float j = 0; j < fNewWidth; j++)
                                {
                                    if (k == 0) // grab 3
                                    {
                                        Pval++;
                                        Ptarget = (int)(j * xStep) + 1;
                                        sourceByte += ((Ptarget - Pval) * _bytesPerPixel);
                                        Pval = Ptarget;
                                        for (int c = 0; c < _bytesPerPixel; c++)
                                        {
                                            *destByte = *sourceByte;
                                            destByte++;
                                            sourceByte++;
                                        }
                                    }
                                    else // grab 1 and 2, or 4 and 5
                                    {
                                        Pval++;
                                        Ptarget = (int)(j * xStep) + 1;
                                        sourceByte += (((Ptarget - Pval) - 1) * _bytesPerPixel);
                                        Pval = Ptarget;
                                        for (int c = 0; c < _bytesPerPixel; c++)
                                        {
                                            *destByte = *sourceByte;
                                            destByte++;
                                            sourceByte++;
                                        }
                                        sourceByte += (1 * _bytesPerPixel);
                                        for (int c = 0; c < _bytesPerPixel; c++)
                                        {
                                            *destByte = *sourceByte;
                                            destByte++;
                                            sourceByte++;
                                        }
                                        sourceByte -= (_bytesPerPixel);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // we now have our intermediate data, we now need to translate this data into the
            // final bitmap

            int[] pot = new int[newWidth * newHeight * _bytesPerPixel];
            int counter = 0;


            unsafe
            {
                fixed (byte* sourceDataStart = &intermediateData[0])
                {
                    fixed (int* destDataStart = &pot[0])
                    {
                        byte* sourceByte = sourceDataStart;
                        int* destByte = destDataStart;
                        for (int j = 0; j < fNewHeight; j++)
                        {
                            // 1 and 2
                            destByte = destDataStart;
                            destByte += j * (int)fNewWidth * _bytesPerPixel;
                            for (int i = 0; i < fNewWidth; i++)
                            {
                                for (int k = 0; k < _bytesPerPixel; k++)
                                {
                                    *destByte += *sourceByte;
                                    destByte++;
                                    sourceByte++;
                                }
                                destByte -= _bytesPerPixel;
                                for (int k = 0; k < _bytesPerPixel; k++)
                                {
                                    *destByte += *sourceByte;
                                    destByte++;
                                    sourceByte++;
                                }
                            }
                            // 3
                            destByte = destDataStart;
                            destByte += j * (int)fNewWidth * _bytesPerPixel;
                            for (int i = 0; i < fNewWidth; i++)
                            {
                                for (int k = 0; k < _bytesPerPixel; k++)
                                {
                                    *destByte += *sourceByte;
                                    destByte++;
                                    sourceByte++;
                                }
                            }
                            // 4 and 5
                            destByte = destDataStart;
                            destByte += j * (int)fNewWidth * _bytesPerPixel;
                            for (int i = 0; i < fNewWidth; i++)
                            {
                                for (int k = 0; k < _bytesPerPixel; k++)
                                {
                                    *destByte += *sourceByte;
                                    destByte++;
                                    sourceByte++;
                                }
                                destByte -= _bytesPerPixel;
                                for (int k = 0; k < _bytesPerPixel; k++)
                                {
                                    *destByte += *sourceByte;
                                    destByte++;
                                    sourceByte++;
                                }
                            }

                        }
                    }
                }
            }

            unsafe
            {
                fixed (int* sourceDataStart = &pot[0])
                {
                    fixed (byte* destDataStart = &smallImage.Data[0])
                    {
                        byte* destByte = destDataStart;
                        int* sourceByte = sourceDataStart;
                        for (int i = 0; i < pot.Length; i++)
                        {
                            *destByte = Convert.ToByte((int)(*sourceByte / 5));
                            destByte++;
                            sourceByte++;
                        }
                    }
                }
            }

            //for(int i = 0; i<pot.Length;i++)
            //    smallImage.Data[i] = Convert.ToByte((int)(pot[i] / 5));


            return smallImage;
        }

        /// <summary>
        /// An attempt at replicating the above code using pointer arithmetic
        /// </summary>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public unsafe ByteArrayBitmap GetSubSampledBytestream_Bilinear_NoTransparancyCalc_V1(int newWidth, int newHeight)
        {
            ByteArrayBitmap smallImage = new ByteArrayBitmap(newWidth, newHeight, this.Format);
            float fWidth = this.Width, fHeight = this.Height;
            float fNewWidth = newWidth, fNewHeight = newHeight;
            float xStep = (fWidth - 2) / fNewWidth;
            float yStep = (fHeight - 2) / fNewHeight;

            // new
            // Due to an accurate, but insanely difficult rounding system used by mark in the
            // previous function (and sub functions) we have to do a little clever tracking to
            // ensure our floats round correctly.

            int _bytesPerPixelMinusOne = _bytesPerPixel - 1;

            byte[] intermediateData = MakeBytes(5 * newWidth * newHeight * (_bytesPerPixelMinusOne));

            // pixels gathered
            // 1 2
            //  3
            // 4 5
            unsafe
            {
                fixed (byte* sourceDataStart = &this.Data[0])
                {
                    fixed (byte* destDataStart = &intermediateData[0])
                    {
                        int Pval = 0;
                        int Ptarget = 0;

                        byte* sourceByte = sourceDataStart;
                        byte* destByte = destDataStart;
                        for (float i = 0; i < fNewHeight; i++)
                        {

                            // wiggle factor for interpolation
                            for (int k = -1; k < 2; k++)
                            {
                                sourceByte = sourceDataStart;
                                // move to start of row
                                //int ss = ((int)(i * yStep) + 1 + k) * this.Stride;
                                sourceByte += ((int)(i * yStep) + 1 + k) * this.Stride;
                                Pval = -1;
                                Ptarget = 0;
                                for (float j = 0; j < fNewWidth; j++)
                                {
                                    if (k == 0) // grab 3
                                    {
                                        Pval++;
                                        Ptarget = (int)(j * xStep) + 1;
                                        sourceByte += ((Ptarget - Pval) * _bytesPerPixel);
                                        Pval = Ptarget;
                                        for (int c = 0; c < _bytesPerPixelMinusOne; c++)
                                        {
                                            *destByte = *sourceByte;
                                            destByte++;
                                            sourceByte++;
                                        }
                                        sourceByte++;
                                    }
                                    else // grab 1 and 2, or 4 and 5
                                    {
                                        Pval++;
                                        Ptarget = (int)(j * xStep) + 1;
                                        sourceByte += (((Ptarget - Pval) - 1) * _bytesPerPixel);
                                        Pval = Ptarget;
                                        for (int c = 0; c < _bytesPerPixelMinusOne; c++)
                                        {
                                            *destByte = *sourceByte;
                                            destByte++;
                                            sourceByte++;
                                        }
                                        sourceByte += (1 * _bytesPerPixel) + 1;
                                        for (int c = 0; c < _bytesPerPixelMinusOne; c++)
                                        {
                                            *destByte = *sourceByte;
                                            destByte++;
                                            sourceByte++;
                                        }
                                        sourceByte -= (_bytesPerPixelMinusOne);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // we now have our intermediate data, we now need to translate this data into the
            // final bitmap

            short[] pot = new short[newWidth * newHeight * _bytesPerPixelMinusOne];
            int counter = 0;


            unsafe
            {
                fixed (byte* sourceDataStart = &intermediateData[0])
                {
                    fixed (short* destDataStart = &pot[0])
                    {
                        byte* sourceByte = sourceDataStart;
                        short* destByte = destDataStart;
                        for (int j = 0; j < fNewHeight; j++)
                        {
                            // 1 and 2
                            destByte = destDataStart;
                            destByte += j * (int)fNewWidth * _bytesPerPixelMinusOne;
                            for (int i = 0; i < fNewWidth; i++)
                            {
                                for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                {
                                    *destByte += *sourceByte;
                                    destByte++;
                                    sourceByte++;
                                }
                                destByte -= _bytesPerPixelMinusOne;
                                for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                {
                                    *destByte += *sourceByte;
                                    destByte++;
                                    sourceByte++;
                                }
                            }
                            // 3
                            destByte = destDataStart;
                            destByte += j * (int)fNewWidth * _bytesPerPixelMinusOne;
                            for (int i = 0; i < fNewWidth; i++)
                            {
                                for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                {
                                    *destByte += *sourceByte;
                                    destByte++;
                                    sourceByte++;
                                }
                            }
                            // 4 and 5
                            destByte = destDataStart;
                            destByte += j * (int)fNewWidth * _bytesPerPixelMinusOne;
                            for (int i = 0; i < fNewWidth; i++)
                            {
                                for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                {
                                    *destByte += *sourceByte;
                                    destByte++;
                                    sourceByte++;
                                }
                                destByte -= _bytesPerPixelMinusOne;
                                for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                {
                                    *destByte += *sourceByte;
                                    destByte++;
                                    sourceByte++;
                                }
                            }

                        }
                    }
                }
            }

            unsafe
            {
                fixed (short* sourceDataStart = &pot[0])
                {
                    fixed (byte* destDataStart = &smallImage.Data[0])
                    {
                        byte* destByte = destDataStart;
                        short* sourceByte = sourceDataStart;
                        for (int i = 0; i < (newWidth * newHeight); i++)
                        {
                            for (int j = 0; j < _bytesPerPixelMinusOne; j++)
                            {
                                *destByte = Convert.ToByte((int)(*sourceByte / 5));
                                destByte++;
                                sourceByte++;
                            }
                            *destByte = 255;
                            destByte++;
                        }
                    }
                }
            }

            return smallImage;
        }

        /// <summary>
        /// An attempt at replicating the above code using pointer arithmetic
        /// </summary>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public unsafe ByteArrayBitmap GetSubSampledBytestream_Bilinear_NoTransparancyCalc_V2(int newWidth, int newHeight)
        {
            ByteArrayBitmap smallImage = new ByteArrayBitmap(newWidth, newHeight, this.Format);
            float fWidth = this.Width, fHeight = this.Height;
            float fNewWidth = newWidth, fNewHeight = newHeight;
            float xStep = (fWidth - 2) / fNewWidth;
            float yStep = (fHeight - 2) / fNewHeight;

            // new
            // Due to an accurate, but insanely difficult rounding system used by mark in the
            // previous function (and sub functions) we have to do a little clever tracking to
            // ensure our floats round correctly.

            int _bytesPerPixelMinusOne = _bytesPerPixel - 1;

            byte[] intermediateData = MakeBytes(4 * newWidth * newHeight * (_bytesPerPixelMinusOne));

            // pixels gathered
            // 1 2
            // 4 5
            unsafe
            {
                fixed (byte* sourceDataStart = &this.Data[0])
                {
                    fixed (byte* destDataStart = &intermediateData[0])
                    {
                        int Pval = 0;
                        int Ptarget = 0;

                        byte* sourceByte = sourceDataStart;
                        byte* destByte = destDataStart;
                        for (float i = 0; i < fNewHeight; i++)
                        {

                            // wiggle factor for interpolation
                            for (int k = -1; k < 2; k += 2)
                            {
                                sourceByte = sourceDataStart;
                                // move to start of row
                                //int ss = ((int)(i * yStep) + 1 + k) * this.Stride;
                                sourceByte += ((int)(i * yStep) + 1 + k) * this.Stride;
                                Pval = -1;
                                Ptarget = 0;
                                for (float j = 0; j < fNewWidth; j++)
                                {

                                    Pval++;
                                    Ptarget = (int)(j * xStep) + 1;
                                    sourceByte += (((Ptarget - Pval) - 1) * _bytesPerPixel);
                                    Pval = Ptarget;
                                    for (int c = 0; c < _bytesPerPixelMinusOne; c++)
                                    {
                                        *destByte = *sourceByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                    sourceByte += (1 * _bytesPerPixel) + 1;
                                    for (int c = 0; c < _bytesPerPixelMinusOne; c++)
                                    {
                                        *destByte = *sourceByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                    sourceByte -= (_bytesPerPixelMinusOne);
                                }
                            }
                        }
                    }
                }
            }
            // we now have our intermediate data, we now need to translate this data into the
            // final bitmap

            short[] pot = new short[newWidth * newHeight * _bytesPerPixelMinusOne];
            int counter = 0;


            unsafe
            {
                fixed (byte* sourceDataStart = &intermediateData[0])
                {
                    fixed (short* destDataStart = &pot[0])
                    {
                        byte* sourceByte = sourceDataStart;
                        short* destByte = destDataStart;
                        for (int j = 0; j < fNewHeight; j++)
                        {
                            // 1 and 2
                            destByte = destDataStart;
                            destByte += j * (int)fNewWidth * _bytesPerPixelMinusOne;
                            for (int i = 0; i < fNewWidth; i++)
                            {
                                for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                {
                                    *destByte += *sourceByte;
                                    destByte++;
                                    sourceByte++;
                                }
                                destByte -= _bytesPerPixelMinusOne;
                                for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                {
                                    *destByte += *sourceByte;
                                    destByte++;
                                    sourceByte++;
                                }
                            }
                            // 4 and 5
                            destByte = destDataStart;
                            destByte += j * (int)fNewWidth * _bytesPerPixelMinusOne;
                            for (int i = 0; i < fNewWidth; i++)
                            {
                                for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                {
                                    *destByte += *sourceByte;
                                    destByte++;
                                    sourceByte++;
                                }
                                destByte -= _bytesPerPixelMinusOne;
                                for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                {
                                    *destByte += *sourceByte;
                                    destByte++;
                                    sourceByte++;
                                }
                            }

                        }
                    }
                }
            }

            unsafe
            {
                fixed (short* sourceDataStart = &pot[0])
                {
                    fixed (byte* destDataStart = &smallImage.Data[0])
                    {
                        byte* destByte = destDataStart;
                        short* sourceByte = sourceDataStart;
                        for (int i = 0; i < (newWidth * newHeight); i++)
                        {
                            for (int j = 0; j < _bytesPerPixelMinusOne; j++)
                            {
                                *destByte = Convert.ToByte((int)(*sourceByte / 4));
                                destByte++;
                                sourceByte++;
                            }
                            *destByte = 255;
                            destByte++;
                        }
                    }
                }
            }

            return smallImage;
        }

        /// <summary>
        /// An attempt at replicating the above code using pointer arithmetic
        /// </summary>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public unsafe ByteArrayBitmap GetSubSampledBytestream_Bilinear_NoTransparancyCalc_V3(int newWidth, int newHeight)
        {
            try
            {
                ByteArrayBitmap smallImage = new ByteArrayBitmap(newWidth, newHeight, this.Format);
                float fWidth = this.Width, fHeight = this.Height;
                float fNewWidth = newWidth, fNewHeight = newHeight;
                float xStep = (fWidth - 2) / fNewWidth;
                float yStep = (fHeight - 2) / fNewHeight;

                // new
                // Due to an accurate, but insanely difficult rounding system used by mark in the
                // previous function (and sub functions) we have to do a little clever tracking to
                // ensure our floats round correctly.

                int _bytesPerPixelMinusOne = _bytesPerPixel - 1;

                byte[] intermediateData = MakeBytes(4 * newWidth * newHeight * (_bytesPerPixelMinusOne));

                // pixels gathered
                // 1 2
                // 4 5
                unsafe
                {
                    fixed (byte* sourceDataStart = &this.Data[0])
                    {
                        fixed (byte* destDataStart = &intermediateData[0])
                        {
                            int Pval = 0;
                            int Ptarget = 0;

                            byte* sourceByte = sourceDataStart;
                            byte* destByte = destDataStart;
                            for (float i = 0; i < fNewHeight; i++)
                            {

                                // wiggle factor for interpolation
                                for (int k = -1; k < 2; k += 2)
                                {
                                    sourceByte = sourceDataStart;
                                    // move to start of row
                                    //int ss = ((int)(i * yStep) + 1 + k) * this.Stride;
                                    sourceByte += ((int)(i * yStep) + 1 + k) * this.Stride;
                                    Pval = -1;
                                    Ptarget = 0;
                                    for (float j = 0; j < fNewWidth; j++)
                                    {

                                        Pval++;
                                        Ptarget = (int)(j * xStep) + 1;
                                        sourceByte += (((Ptarget - Pval) - 1) * _bytesPerPixel);
                                        Pval = Ptarget;
                                        for (int c = 0; c < _bytesPerPixelMinusOne; c++)
                                        {
                                            *destByte = *sourceByte;
                                            destByte++;
                                            sourceByte++;
                                        }
                                        sourceByte += (1 * _bytesPerPixel) + 1;
                                        for (int c = 0; c < _bytesPerPixelMinusOne; c++)
                                        {
                                            *destByte = *sourceByte;
                                            destByte++;
                                            sourceByte++;
                                        }
                                        sourceByte -= (_bytesPerPixelMinusOne);
                                    }
                                }
                            }
                        }
                    }
                }
                // we now have our intermediate data, we now need to translate this data into the
                // final bitmap

                short[] pot = new short[newWidth * newHeight * _bytesPerPixelMinusOne];
                int counter = 0;


                unsafe
                {
                    fixed (byte* sourceDataStart = &intermediateData[0])
                    {
                        fixed (short* destDataStart = &pot[0])
                        {
                            byte* sourceByte = sourceDataStart;
                            short* destByte = destDataStart;
                            for (int j = 0; j < fNewHeight; j++)
                            {
                                // 1 and 2
                                destByte = destDataStart;
                                destByte += j * (int)fNewWidth * _bytesPerPixelMinusOne;
                                for (int i = 0; i < fNewWidth; i++)
                                {
                                    for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                    {
                                        *destByte += *sourceByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                    destByte -= _bytesPerPixelMinusOne;
                                    for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                    {
                                        *destByte += *sourceByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                }
                                // 4 and 5
                                destByte = destDataStart;
                                destByte += j * (int)fNewWidth * _bytesPerPixelMinusOne;
                                for (int i = 0; i < fNewWidth; i++)
                                {
                                    for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                    {
                                        *destByte += *sourceByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                    destByte -= _bytesPerPixelMinusOne;
                                    for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                    {
                                        *destByte += *sourceByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                }

                            }
                        }
                    }
                }

                unsafe
                {
                    fixed (short* sourceDataStart = &pot[0])
                    {
                        fixed (byte* destDataStart = &smallImage.Data[0])
                        {
                            byte* destByte = destDataStart;
                            short* sourceByte = sourceDataStart;
                            for (int i = 0; i < (newWidth * newHeight); i++)
                            {
                                for (int j = 0; j < _bytesPerPixelMinusOne; j++)
                                {
                                    *destByte = Convert.ToByte((*sourceByte >> 2));
                                    destByte++;
                                    sourceByte++;
                                }
                                *destByte = 255;
                                destByte++;
                            }
                        }
                    }
                }

                return smallImage;
            }
            catch (Exception er)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
                return null;
            }
        }

        /// <summary>
        /// Gets the sub-sampled byte stream
        /// AND produces a grayscale version of the data
        /// </summary>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public unsafe ByteArrayBitmap GetSubSampledBytestream_Bilinear_NoTransparancyCalc_V3_WithGrayscale(int newWidth, int newHeight)
        {
            try
            {
                ByteArrayBitmap smallImage = new ByteArrayBitmap(newWidth, newHeight, this.Format);
                smallImage.DataGrayscale = MakeBytes(newWidth * newHeight);
                float fWidth = this.Width, fHeight = this.Height;
                float fNewWidth = newWidth, fNewHeight = newHeight;
                float xStep = (fWidth - 2) / fNewWidth;
                float yStep = (fHeight - 2) / fNewHeight;

                // new
                // Due to an accurate, but insanely difficult rounding system used by mark in the
                // previous function (and sub functions) we have to do a little clever tracking to
                // ensure our floats round correctly.

                int _bytesPerPixelMinusOne = _bytesPerPixel - 1;

                byte[] intermediateData = MakeBytes(4 * newWidth * newHeight * (_bytesPerPixelMinusOne));

                // pixels gathered
                // 1 2
                // 4 5
                unsafe
                {
                    fixed (byte* sourceDataStart = &this.Data[0])
                    {
                        fixed (byte* destDataStart = &intermediateData[0])
                        {
                            int Pval = 0;
                            int Ptarget = 0;

                            byte* sourceByte = sourceDataStart;
                            byte* destByte = destDataStart;
                            for (float i = 0; i < fNewHeight; i++)
                            {

                                // wiggle factor for interpolation
                                for (int k = -1; k < 2; k += 2)
                                {
                                    sourceByte = sourceDataStart;
                                    // move to start of row
                                    //int ss = ((int)(i * yStep) + 1 + k) * this.Stride;
                                    sourceByte += ((int)(i * yStep) + 1 + k) * this.Stride;
                                    Pval = -1;
                                    Ptarget = 0;
                                    for (float j = 0; j < fNewWidth; j++)
                                    {

                                        Pval++;
                                        Ptarget = (int)(j * xStep) + 1;
                                        sourceByte += (((Ptarget - Pval) - 1) * _bytesPerPixel);
                                        Pval = Ptarget;
                                        for (int c = 0; c < _bytesPerPixelMinusOne; c++)
                                        {
                                            *destByte = *sourceByte;
                                            destByte++;
                                            sourceByte++;
                                        }
                                        sourceByte += (1 * _bytesPerPixel) + 1;
                                        for (int c = 0; c < _bytesPerPixelMinusOne; c++)
                                        {
                                            *destByte = *sourceByte;
                                            destByte++;
                                            sourceByte++;
                                        }
                                        sourceByte -= (_bytesPerPixelMinusOne);
                                    }
                                }
                            }
                        }
                    }
                }
                // we now have our intermediate data, we now need to translate this data into the
                // final bitmap

                short[] pot = new short[newWidth * newHeight * _bytesPerPixelMinusOne];
                int counter = 0;


                unsafe
                {
                    fixed (byte* sourceDataStart = &intermediateData[0])
                    {
                        fixed (short* destDataStart = &pot[0])
                        {
                            byte* sourceByte = sourceDataStart;
                            short* destByte = destDataStart;
                            for (int j = 0; j < fNewHeight; j++)
                            {
                                // 1 and 2
                                destByte = destDataStart;
                                destByte += j * (int)fNewWidth * _bytesPerPixelMinusOne;
                                for (int i = 0; i < fNewWidth; i++)
                                {
                                    for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                    {
                                        *destByte += *sourceByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                    destByte -= _bytesPerPixelMinusOne;
                                    for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                    {
                                        *destByte += *sourceByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                }
                                // 4 and 5
                                destByte = destDataStart;
                                destByte += j * (int)fNewWidth * _bytesPerPixelMinusOne;
                                for (int i = 0; i < fNewWidth; i++)
                                {
                                    for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                    {
                                        *destByte += *sourceByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                    destByte -= _bytesPerPixelMinusOne;
                                    for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                    {
                                        *destByte += *sourceByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                }

                            }
                        }
                    }
                }

                unsafe
                {
                    //avg /3
                    int DataGrayscaleDenominator = 3;
                    fixed (short* sourceDataStart = &pot[0])
                    {
                        fixed (byte* destDataStart = &smallImage.Data[0])
                        {
                            fixed (byte* destDataGrayStart = &smallImage.DataGrayscale[0])
                            {
                                byte* destByte = destDataStart;
                                byte* destByteGray = destDataGrayStart;
                                short* sourceByte = sourceDataStart;
                                for (int i = 0; i < (newWidth * newHeight); i++)
                                {
                                    //*destByteGray = 0;
                                    int tot = 0;
                                    for (int j = 0; j < _bytesPerPixelMinusOne; j++)
                                    {
                                        *destByte = Convert.ToByte((*sourceByte >> 2));
                                        //*destByteGray = Math.Max(*destByte, *destByteGray);
                                        tot += *destByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                    destByte++;

                                    *destByteGray = Convert.ToByte((tot / DataGrayscaleDenominator));
                                    destByteGray++;
                                }
                            }
                        }
                    }
                    //avg - greyscale /4
                    //fixed (short* sourceDataStart = &pot[0])
                    //{
                    //    fixed (byte* destDataStart = &smallImage.Data[0])
                    //    {
                    //        fixed (byte* destDataGrayStart = &smallImage.DataGrayscale[0])
                    //        {
                    //            byte* destByte = destDataStart;
                    //            byte* destByteGray = destDataGrayStart;
                    //            short* sourceByte = sourceDataStart;
                    //            for (int i = 0; i < (newWidth * newHeight); i++)
                    //            {
                    //                int tot = 0;
                    //                for (int j = 0; j < _bytesPerPixelMinusOne; j++)
                    //                {
                    //                    *destByte = Convert.ToByte((*sourceByte >> 2));
                    //                    tot += *destByte;
                    //                    destByte++;
                    //                    sourceByte++;
                    //                }
                    //                destByte++;
                    //                *destByteGray = Convert.ToByte((tot >> 2));
                    //                destByteGray++;
                    //            }
                    //        }
                    //    }
                    //}

                    //max of channels
                    //fixed (short* sourceDataStart = &pot[0])
                    //{
                    //    fixed (byte* destDataStart = &smallImage.Data[0])
                    //    {
                    //        fixed (byte* destDataGrayStart = &smallImage.DataGrayscale[0])
                    //        {
                    //            byte* destByte = destDataStart;
                    //            byte* destByteGray = destDataGrayStart;
                    //            short* sourceByte = sourceDataStart;
                    //            for (int i = 0; i < (newWidth * newHeight); i++)
                    //            {
                    //                //int tot = 0;
                    //                for (int j = 0; j < _bytesPerPixelMinusOne; j++)
                    //                {
                    //                    *destByte = Convert.ToByte((*sourceByte >> 2));
                    //                    //tot += *destByte;
                    //                    *destByteGray = Math.Max(*destByte, *destByteGray);
                    //                    destByte++;
                    //                    sourceByte++;
                    //                }
                    //                destByte++;
                    //                //*destByteGray = Convert.ToByte((tot >> 2));
                    //                destByteGray++;
                    //            }
                    //        }
                    //    }
                    //}
                }
                return smallImage;
            }
            catch (Exception er)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
                return null;
            }
        }

        /// <summary>
        /// Gets the sub-sampled byte stream
        /// AND produces a grayscale version of the data
        /// AND produces YUV version of the data
        /// </summary>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public unsafe ByteArrayBitmap GetSubSampledBytestream_Bilinear_NoTransparancyCalc_V3_WithGrayscale_Experimental(int newWidth, int newHeight)
        {
            try
            {
                ByteArrayBitmap smallImage = new ByteArrayBitmap(newWidth, newHeight, this.Format);
                smallImage.DataGrayscale = MakeBytes(newWidth * newHeight);
                smallImage.DataHSV = MakeBytes(newHeight * newWidth * 3);
                float fWidth = this.Width, fHeight = this.Height;
                float fNewWidth = newWidth, fNewHeight = newHeight;
                float xStep = (fWidth - 2) / fNewWidth;
                float yStep = (fHeight - 2) / fNewHeight;

                // new
                // Due to an accurate, but insanely difficult rounding system used by mark in the
                // previous function (and sub functions) we have to do a little clever tracking to
                // ensure our floats round correctly.

                int _bytesPerPixelMinusOne = _bytesPerPixel - 1;

                byte[] intermediateData = MakeBytes(4 * newWidth * newHeight * (_bytesPerPixelMinusOne));

                // pixels gathered
                // 1 2
                // 4 5
                unsafe
                {
                    fixed (byte* sourceDataStart = &this.Data[0])
                    {
                        fixed (byte* destDataStart = &intermediateData[0])
                        {
                            int Pval = 0;
                            int Ptarget = 0;

                            byte* sourceByte = sourceDataStart;
                            byte* destByte = destDataStart;
                            for (float i = 0; i < fNewHeight; i++)
                            {

                                // wiggle factor for interpolation
                                for (int k = -1; k < 2; k += 2)
                                {
                                    sourceByte = sourceDataStart;
                                    // move to start of row
                                    //int ss = ((int)(i * yStep) + 1 + k) * this.Stride;
                                    sourceByte += ((int)(i * yStep) + 1 + k) * this.Stride;
                                    Pval = -1;
                                    Ptarget = 0;
                                    for (float j = 0; j < fNewWidth; j++)
                                    {

                                        Pval++;
                                        Ptarget = (int)(j * xStep) + 1;
                                        sourceByte += (((Ptarget - Pval) - 1) * _bytesPerPixel);
                                        Pval = Ptarget;
                                        for (int c = 0; c < _bytesPerPixelMinusOne; c++)
                                        {
                                            *destByte = *sourceByte;
                                            destByte++;
                                            sourceByte++;
                                        }
                                        sourceByte += (1 * _bytesPerPixel) + 1;
                                        for (int c = 0; c < _bytesPerPixelMinusOne; c++)
                                        {
                                            *destByte = *sourceByte;
                                            destByte++;
                                            sourceByte++;
                                        }
                                        sourceByte -= (_bytesPerPixelMinusOne);
                                    }
                                }
                            }
                        }
                    }
                }
                // we now have our intermediate data, we now need to translate this data into the
                // final bitmap

                short[] pot = new short[newWidth * newHeight * _bytesPerPixelMinusOne];
                int counter = 0;


                unsafe
                {
                    fixed (byte* sourceDataStart = &intermediateData[0])
                    {
                        fixed (short* destDataStart = &pot[0])
                        {
                            byte* sourceByte = sourceDataStart;
                            short* destByte = destDataStart;
                            for (int j = 0; j < fNewHeight; j++)
                            {
                                // 1 and 2
                                destByte = destDataStart;
                                destByte += j * (int)fNewWidth * _bytesPerPixelMinusOne;
                                for (int i = 0; i < fNewWidth; i++)
                                {
                                    for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                    {
                                        *destByte += *sourceByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                    destByte -= _bytesPerPixelMinusOne;
                                    for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                    {
                                        *destByte += *sourceByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                }
                                // 4 and 5
                                destByte = destDataStart;
                                destByte += j * (int)fNewWidth * _bytesPerPixelMinusOne;
                                for (int i = 0; i < fNewWidth; i++)
                                {
                                    for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                    {
                                        *destByte += *sourceByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                    destByte -= _bytesPerPixelMinusOne;
                                    for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                    {
                                        *destByte += *sourceByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                }

                            }
                        }
                    }
                }

                // produce our grayscale data
                #region grayscale & scaled

                unsafe
                {
                    //avg /3
                    int DataGrayscaleDenominator = 3;
                    fixed (short* sourceDataStart = &pot[0])
                    {
                        fixed (byte* destDataStart = &smallImage.Data[0])
                        {
                            fixed (byte* destDataGrayStart = &smallImage.DataGrayscale[0])
                            {
                                byte* destByte = destDataStart;
                                byte* destByteGray = destDataGrayStart;
                                short* sourceByte = sourceDataStart;
                                for (int i = 0; i < (newWidth * newHeight); i++)
                                {
                                    //*destByteGray = 0;
                                    int tot = 0;
                                    for (int j = 0; j < _bytesPerPixelMinusOne; j++)
                                    {
                                        *destByte = Convert.ToByte((*sourceByte >> 2));
                                        //*destByteGray = Math.Max(*destByte, *destByteGray);
                                        tot += *destByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                    destByte++;

                                    *destByteGray = Convert.ToByte((tot / DataGrayscaleDenominator));
                                    destByteGray++;
                                }
                            }
                        }
                    }
                    //avg - greyscale /4
                    //fixed (short* sourceDataStart = &pot[0])
                    //{
                    //    fixed (byte* destDataStart = &smallImage.Data[0])
                    //    {
                    //        fixed (byte* destDataGrayStart = &smallImage.DataGrayscale[0])
                    //        {
                    //            byte* destByte = destDataStart;
                    //            byte* destByteGray = destDataGrayStart;
                    //            short* sourceByte = sourceDataStart;
                    //            for (int i = 0; i < (newWidth * newHeight); i++)
                    //            {
                    //                int tot = 0;
                    //                for (int j = 0; j < _bytesPerPixelMinusOne; j++)
                    //                {
                    //                    *destByte = Convert.ToByte((*sourceByte >> 2));
                    //                    tot += *destByte;
                    //                    destByte++;
                    //                    sourceByte++;
                    //                }
                    //                destByte++;
                    //                *destByteGray = Convert.ToByte((tot >> 2));
                    //                destByteGray++;
                    //            }
                    //        }
                    //    }
                    //}

                    //max of channels
                    //fixed (short* sourceDataStart = &pot[0])
                    //{
                    //    fixed (byte* destDataStart = &smallImage.Data[0])
                    //    {
                    //        fixed (byte* destDataGrayStart = &smallImage.DataGrayscale[0])
                    //        {
                    //            byte* destByte = destDataStart;
                    //            byte* destByteGray = destDataGrayStart;
                    //            short* sourceByte = sourceDataStart;
                    //            for (int i = 0; i < (newWidth * newHeight); i++)
                    //            {
                    //                //int tot = 0;
                    //                for (int j = 0; j < _bytesPerPixelMinusOne; j++)
                    //                {
                    //                    *destByte = Convert.ToByte((*sourceByte >> 2));
                    //                    //tot += *destByte;
                    //                    *destByteGray = Math.Max(*destByte, *destByteGray);
                    //                    destByte++;
                    //                    sourceByte++;
                    //                }
                    //                destByte++;
                    //                //*destByteGray = Convert.ToByte((tot >> 2));
                    //                destByteGray++;
                    //            }
                    //        }
                    //    }
                    //}
                }
                #endregion
                #region HSV
                //
                // This region was used for a HSV BG model
                // the model was not an improvement and dropped
                // This code will be re-used when colour is moved up the tree
                // to here, but currently is currently not needed
                //



                //unsafe
                //{
                //    fixed (short* sourceDataStart = &pot[0])
                //    {
                //        fixed (byte* destDataYUVStart = &smallImage.DataHSV[0])
                //        {
                //            byte* destByteY = destDataYUVStart;
                //            byte* destByteU = destDataYUVStart + 1;
                //            byte* destByteV = destDataYUVStart + 2;
                //            short* sourceByteR = sourceDataStart;
                //            short* sourceByteG = sourceDataStart + 1;
                //            short* sourceByteB = sourceDataStart + 2;
                //            for (int i = 0; i < (newWidth * newHeight); i++)
                //            {
                //                // these values should be / 1020
                //                double vr = ((double)(*sourceByteR)) / 1024;
                //                double vg = ((double)(*sourceByteG)) / 1024;
                //                double vb = ((double)(*sourceByteB)) / 1024;

                //                double max = Math.Max(Math.Max(vr, vg), vb);
                //                double min = Math.Min(Math.Min(vr, vg), vb);
                //                double delta = max - min;

                //                *destByteV = (byte)(max * 255);
                //                if (delta == 0)
                //                {
                //                    *destByteU = 0;
                //                    *destByteY = 0;
                //                }
                //                else
                //                {
                //                    *destByteU = (byte)((delta / max) * 255);

                //                    double result = 0;
                //                    if (vr == max)
                //                        result = (vg - vb) / (delta) % 6;
                //                    else if (vg == max)
                //                        result = 2 + (vb - vr) / delta;
                //                    else if (vb == max)
                //                        result = 4 + (vr - vg) / delta;
                //                    result = result / (6);

                //                    if (result < 0) result += 1;
                //                    if (result > 1) result -= 1;

                //                    *destByteY = (byte)(result * 255);
                //                }

                //                if (i != (newWidth * newHeight) - 1)
                //                {
                //                    destByteY += 3;
                //                    destByteU += 3;
                //                    destByteV += 3;
                //                    sourceByteR += 3;
                //                    sourceByteG += 3;
                //                    sourceByteB += 3;
                //                }

                //            }
                //        }
                //    }
                //}
                #endregion

                #region Data Local

                // This area was used for data for an alternate BG model
                // performance weas good, except for in environments with multiple 
                // changes of luminosity (i.e. clouds). As such the extra expense
                // vs. the gain was not deemed to be enough
                // Was paired with HistogramMK3Alt2
                //
                // Note, it was one or the other fixed statement, never both
                //



                //smallImage.DataLocal = new byte[smallImage.Data.Length];

                //fixed (byte* destDataStart = &smallImage.DataLocal[0])
                //{
                //    fixed (byte* sourceDataStart = &smallImage.Data[0])
                //    {
                //        byte* destByte = destDataStart + (int)(fNewHeight * 4) + 1;

                //        byte*[] sources = new byte*[8];
                //        sources[0] = sourceDataStart;
                //        sources[1] = sourceDataStart + 1;
                //        sources[2] = sourceDataStart + 2;
                //        sources[3] = sourceDataStart + (int)(fNewHeight * 4);
                //        sources[4] = sourceDataStart + (int)(fNewHeight * 4) + 2;
                //        sources[5] = sourceDataStart + (int)(fNewHeight * 8);
                //        sources[6] = sourceDataStart + (int)(fNewHeight * 8) + 1;
                //        sources[7] = sourceDataStart + (int)(fNewHeight * 8) + 2;

                //        int count = 1;
                //        int countStop = (int)newHeight;

                //        for (int i = (int)fNewHeight +1; i < (newWidth * (newHeight-1) -1); i++)
                //        {
                //            //*destByteGray = 0;
                //            int tot = 0;
                //            for (int j = 0; j < _bytesPerPixelMinusOne; j++)
                //            {
                //                tot = 0;
                //                for (int k = 0; k < 8; k++)
                //                {
                //                    tot += *sources[k];

                //                    if(i != (newWidth * (newHeight-1) -2))
                //                        sources[k]++;
                //                }

                //                *destByte = Convert.ToByte((tot / 8));
                //                destByte++;

                //            }
                //            if (i != (newWidth * (newHeight - 1) - 2))
                //            {
                //                destByte++;
                //                count++;
                //                if (count == countStop)
                //                {
                //                    for (int k = 0; k < 8; k++)
                //                        sources[k]+=2;

                //                    destByte += 2;
                //                    count = 1;
                //                }
                //            }
                //        }
                //    }
                //}

                //fixed (byte* destDataStart = &smallImage.DataLocal[0])
                //{
                //    fixed (byte* sourceDataStart = &smallImage.Data[0])
                //    {
                //        byte* destByte = destDataStart + (int)(fNewHeight * _bytesPerPixel * 2) + 2 * _bytesPerPixel;

                //        byte*[] sources = new byte*[25];
                //        sources[00] = sourceDataStart;
                //        sources[01] = sourceDataStart + 1 * _bytesPerPixel;
                //        sources[02] = sourceDataStart + 2 * _bytesPerPixel;
                //        sources[03] = sourceDataStart + 3 * _bytesPerPixel;
                //        sources[04] = sourceDataStart + 4 * _bytesPerPixel;
                //        sources[05] = sourceDataStart + (int)(fNewHeight * _bytesPerPixel);
                //        sources[06] = sourceDataStart + (int)(fNewHeight * _bytesPerPixel) + 1 * _bytesPerPixel;
                //        sources[07] = sourceDataStart + (int)(fNewHeight * _bytesPerPixel) + 2 * _bytesPerPixel;
                //        sources[08] = sourceDataStart + (int)(fNewHeight * _bytesPerPixel) + 3 * _bytesPerPixel;
                //        sources[09] = sourceDataStart + (int)(fNewHeight * _bytesPerPixel) + 4 * _bytesPerPixel;
                //        sources[10] = sourceDataStart + (int)(fNewHeight * _bytesPerPixel * 2);
                //        sources[11] = sourceDataStart + (int)(fNewHeight * _bytesPerPixel * 2) + 1 * _bytesPerPixel;
                //        sources[12] = sourceDataStart + (int)(fNewHeight * _bytesPerPixel * 2) + 2 * _bytesPerPixel;
                //        sources[13] = sourceDataStart + (int)(fNewHeight * _bytesPerPixel * 2) + 3 * _bytesPerPixel;
                //        sources[14] = sourceDataStart + (int)(fNewHeight * _bytesPerPixel * 2) + 4 * _bytesPerPixel;
                //        sources[15] = sourceDataStart + (int)(fNewHeight * _bytesPerPixel * 3);
                //        sources[16] = sourceDataStart + (int)(fNewHeight * _bytesPerPixel * 3) + 1 * _bytesPerPixel;
                //        sources[17] = sourceDataStart + (int)(fNewHeight * _bytesPerPixel * 3) + 2 * _bytesPerPixel;
                //        sources[18] = sourceDataStart + (int)(fNewHeight * _bytesPerPixel * 3) + 3 * _bytesPerPixel;
                //        sources[19] = sourceDataStart + (int)(fNewHeight * _bytesPerPixel * 3) + 4 * _bytesPerPixel;
                //        sources[20] = sourceDataStart + (int)(fNewHeight * _bytesPerPixel * 4);
                //        sources[21] = sourceDataStart + (int)(fNewHeight * _bytesPerPixel * 4) + 1 * _bytesPerPixel;
                //        sources[22] = sourceDataStart + (int)(fNewHeight * _bytesPerPixel * 4) + 2 * _bytesPerPixel;
                //        sources[23] = sourceDataStart + (int)(fNewHeight * _bytesPerPixel * 4) + 3 * _bytesPerPixel;
                //        sources[24] = sourceDataStart + (int)(fNewHeight * _bytesPerPixel * 4) + 4 * _bytesPerPixel;

                //        //int count = 2;
                //        //int countStop = (int)newHeight - 1;

                //        for (int i = 2; i < (newWidth - 2); i++)
                //        {
                //            for (int j = 2; j < newHeight - 2; j++)
                //            {
                //                //*destByteGray = 0;
                //                int tot = 0;
                //                for (int m = 0; m < _bytesPerPixelMinusOne; m++)
                //                {
                //                    tot = 0;
                //                    tot += *sources[7] + *sources[11] + *sources[13] + *sources[17] + (4 * *sources[12]);
                //                    //tot += 3 * (*sources[7] + *sources[11] + *sources[13] + *sources[17]);
                //                    //tot = (int)(tot/64);

                //                    for (int k = 0; k < 25; k++)
                //                    {
                //                        //tot += *sources[k];

                //                        //if (i != (newWidth - 3) && j != newHeight - 3)
                //                            sources[k]++;
                //                    }

                //                    *destByte = Convert.ToByte((tot/8));
                //                    destByte++;
                //                }
                //                    if (i != (newWidth - 3) || j != newHeight - 3)
                //                        for (int k = 0; k < 25; k++)
                //                            sources[k]++;

                //                    destByte++;
                //            }
                //            if (i != (newWidth - 3))
                //            {
                //                //destByte++;

                //                for (int k = 0; k < 25; k++)
                //                    sources[k] += (4 * _bytesPerPixel);

                //                destByte += (4 * _bytesPerPixel);
                //                //count = 2;

                //            }
                //        }
                //    }
                //}

                #endregion

                //(new ByteArrayBitmap(smallImage.DataLocal, smallImage.Width, smallImage.Height, smallImage.Format)).ToBitmap().Save("C:\\TEST\\C\\kerneled.png");

                return smallImage;
            }
            catch (Exception er)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
                return null;
            }
        }

        public static ByteArrayBitmap GetSubSampledBytestream_Bilinear_NoTransparancyCalc_V3_WithGrayscale_HLS_fromEncodedData(int newWidth, int newHeight, byte[] encodedJpegData)
        {
            ByteArrayBitmap smallImage;


            using (MemoryStream memoryStream = new MemoryStream(encodedJpegData))
            {
                System.Drawing.Bitmap lbmp = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(memoryStream);
                System.Drawing.Bitmap sbmp = new Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(sbmp))
                    g.DrawImage(lbmp, new Rectangle(0, 0, sbmp.Width, sbmp.Height));

                smallImage = ByteArrayBitmap.FromBitmap(sbmp);
            }

            // produce our grayscale data
            #region grayscale & scaled
            int _bytesPerPixelMinusOne = smallImage.BytesPerPixel - 1;
            short[] pot = new short[newWidth * newHeight * _bytesPerPixelMinusOne];

            smallImage.DataGrayscale = MakeBytes(newWidth * newHeight);
            smallImage.DataHSV = MakeBytes(newHeight * newWidth * 3);

            unsafe
            {
                //avg /3
                int DataGrayscaleDenominator = 3;
                fixed (byte* destDataStart = &smallImage.Data[0])
                {
                    fixed (byte* destDataGrayStart = &smallImage.DataGrayscale[0])
                    {
                        byte* destByte = destDataStart;
                        byte* destByteGray = destDataGrayStart;
                        for (int i = 0; i < (newWidth * newHeight); i++)
                        {
                            //*destByteGray = 0;
                            int tot = 0;
                            for (int j = 0; j < _bytesPerPixelMinusOne; j++)
                            {
                                tot += *destByte;
                                destByte++;
                            }
                            destByte++;

                            *destByteGray = Convert.ToByte((tot / DataGrayscaleDenominator));
                            destByteGray++;
                        }
                    }
                }
                
            }
            #endregion
            #region HLS
            //
            // This region was used for a HSV BG model
            // the model was not an improvement and dropped
            // This code will be re-used when colour is moved up the tree
            // to here, but currently is currently not needed
            //
            int bytes = smallImage._bytesPerPixel;
            smallImage.HLSImage = new HLSImage(newWidth, newHeight, 3);
            unsafe
            {
                fixed (byte* sourceDataStart = &smallImage.Data[0])
                {
                    fixed (byte* destDataYUVStart = &smallImage.HLSImage.Data[0])
                    {
                        byte* destByteH = destDataYUVStart;
                        byte* destByteL = destDataYUVStart + 1;
                        byte* destByteS = destDataYUVStart + 2;
                        byte* sourceByteB = sourceDataStart;
                        byte* sourceByteG = sourceDataStart + 1;
                        byte* sourceByteR = sourceDataStart + 2;

                        double count = newWidth * newHeight;
                        double sumH = 0, sumL = 0, sumS = 0;
                        for (int i = 0; i < count; i++)
                        {
                            HLSImage.RGBToHLS(*sourceByteR, *sourceByteG, *sourceByteB, ref *destByteH, ref *destByteL, ref *destByteS);
                            //HLSImage.RGBToHLS(255, 255, 0, ref *destByteH, ref *destByteL, ref *destByteS);

                            sumH += *destByteH;
                            sumL += *destByteL;
                            sumS += *destByteS;

                            if (i != (count) - 1)
                            {
                                destByteH += 3;
                                destByteL += 3;
                                destByteS += 3;
                                sourceByteR += bytes;
                                sourceByteG += bytes;
                                sourceByteB += bytes;
                            }

                        }
                        smallImage.HLSImage.GlobalHue = sumH / count;
                        smallImage.HLSImage.GlobalLightness = sumL / count;
                        smallImage.HLSImage.GlobalSaturation = sumS / count;
                        smallImage.HLSImage.GlobalSatLightnessDistance = HLSImage.CalculateSaturationLightnessDistance(
                            smallImage.HLSImage.GlobalSaturation, smallImage.HLSImage.GlobalLightness);
                    }
                }
            }
            #endregion

            return smallImage;
        }

        /// <summary>
        /// Gets the sub-sampled byte stream
        /// AND produces a grayscale version of the data
        /// AND produces HLS version of the data
        /// </summary>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public unsafe ByteArrayBitmap GetSubSampledBytestream_Bilinear_NoTransparancyCalc_V3_WithGrayscale_HLS(int newWidth, int newHeight)
        {
            try
            {
                ByteArrayBitmap smallImage = new ByteArrayBitmap(newWidth, newHeight, this.Format);
                smallImage.DataGrayscale = MakeBytes(newWidth * newHeight);
                smallImage.DataHSV = MakeBytes(newHeight * newWidth * 3);
                float fWidth = this.Width, fHeight = this.Height;
                float fNewWidth = newWidth, fNewHeight = newHeight;
                float xStep = (fWidth - 2) / fNewWidth;
                float yStep = (fHeight - 2) / fNewHeight;

                // new
                // Due to an accurate, but insanely difficult rounding system used by mark in the
                // previous function (and sub functions) we have to do a little clever tracking to
                // ensure our floats round correctly.

                int _bytesPerPixelMinusOne = _bytesPerPixel - 1;

                byte[] intermediateData = MakeBytes(4 * newWidth * newHeight * (_bytesPerPixelMinusOne));

                // pixels gathered
                // 1 2
                // 4 5
                unsafe
                {
                    fixed (byte* sourceDataStart = &this.Data[0])
                    {
                        fixed (byte* destDataStart = &intermediateData[0])
                        {
                            int Pval = 0;
                            int Ptarget = 0;

                            byte* sourceByte = sourceDataStart;
                            byte* destByte = destDataStart;
                            for (float i = 0; i < fNewHeight; i++)
                            {

                                // wiggle factor for interpolation
                                for (int k = -1; k < 2; k += 2)
                                {
                                    sourceByte = sourceDataStart;
                                    // move to start of row
                                    //int ss = ((int)(i * yStep) + 1 + k) * this.Stride;
                                    sourceByte += ((int)(i * yStep) + 1 + k) * this.Stride;
                                    Pval = -1;
                                    Ptarget = 0;
                                    for (float j = 0; j < fNewWidth; j++)
                                    {

                                        Pval++;
                                        Ptarget = (int)(j * xStep) + 1;
                                        sourceByte += (((Ptarget - Pval) - 1) * _bytesPerPixel);
                                        Pval = Ptarget;
                                        for (int c = 0; c < _bytesPerPixelMinusOne; c++)
                                        {
                                            *destByte = *sourceByte;
                                            destByte++;
                                            sourceByte++;
                                        }
                                        sourceByte += (1 * _bytesPerPixel) + 1;
                                        for (int c = 0; c < _bytesPerPixelMinusOne; c++)
                                        {
                                            *destByte = *sourceByte;
                                            destByte++;
                                            sourceByte++;
                                        }
                                        sourceByte -= (_bytesPerPixelMinusOne);
                                    }
                                }
                            }
                        }
                    }
                }
                // we now have our intermediate data, we now need to translate this data into the
                // final bitmap

                short[] pot = new short[newWidth * newHeight * _bytesPerPixelMinusOne];
                int counter = 0;


                unsafe
                {
                    fixed (byte* sourceDataStart = &intermediateData[0])
                    {
                        fixed (short* destDataStart = &pot[0])
                        {
                            byte* sourceByte = sourceDataStart;
                            short* destByte = destDataStart;
                            for (int j = 0; j < fNewHeight; j++)
                            {
                                // 1 and 2
                                destByte = destDataStart;
                                destByte += j * (int)fNewWidth * _bytesPerPixelMinusOne;
                                for (int i = 0; i < fNewWidth; i++)
                                {
                                    for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                    {
                                        *destByte += *sourceByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                    destByte -= _bytesPerPixelMinusOne;
                                    for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                    {
                                        *destByte += *sourceByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                }
                                // 4 and 5
                                destByte = destDataStart;
                                destByte += j * (int)fNewWidth * _bytesPerPixelMinusOne;
                                for (int i = 0; i < fNewWidth; i++)
                                {
                                    for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                    {
                                        *destByte += *sourceByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                    destByte -= _bytesPerPixelMinusOne;
                                    for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                    {
                                        *destByte += *sourceByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                }

                            }
                        }
                    }
                }

                // produce our grayscale data
                #region grayscale & scaled

                unsafe
                {
                    //avg /3
                    int DataGrayscaleDenominator = 3;
                    fixed (short* sourceDataStart = &pot[0])
                    {
                        fixed (byte* destDataStart = &smallImage.Data[0])
                        {
                            fixed (byte* destDataGrayStart = &smallImage.DataGrayscale[0])
                            {
                                byte* destByte = destDataStart;
                                byte* destByteGray = destDataGrayStart;
                                short* sourceByte = sourceDataStart;
                                for (int i = 0; i < (newWidth * newHeight); i++)
                                {
                                    //*destByteGray = 0;
                                    int tot = 0;
                                    for (int j = 0; j < _bytesPerPixelMinusOne; j++)
                                    {
                                        *destByte = Convert.ToByte((*sourceByte >> 2));
                                        //*destByteGray = Math.Max(*destByte, *destByteGray);
                                        tot += *destByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                    destByte++;

                                    *destByteGray = Convert.ToByte((tot / DataGrayscaleDenominator));
                                    destByteGray++;
                                }
                            }
                        }
                    }
                }
                #endregion
                #region HLS
                //
                // This region was used for a HSV BG model
                // the model was not an improvement and dropped
                // This code will be re-used when colour is moved up the tree
                // to here, but currently is currently not needed
                //
                int bytes = smallImage._bytesPerPixel;
                smallImage.HLSImage = new HLSImage(newWidth, newHeight, 3);                
                unsafe
                {
                    fixed (byte* sourceDataStart = &smallImage.Data[0])
                    {
                        fixed (byte* destDataYUVStart = &smallImage.HLSImage.Data[0])
                        {
                            byte* destByteH = destDataYUVStart;
                            byte* destByteL = destDataYUVStart + 1;
                            byte* destByteS = destDataYUVStart + 2;
                            byte* sourceByteB = sourceDataStart;
                            byte* sourceByteG = sourceDataStart + 1;
                            byte* sourceByteR = sourceDataStart + 2;

                            double count = newWidth * newHeight;
                            double sumH = 0, sumL = 0, sumS = 0;
                            for (int i = 0; i < count; i++)
                            {
                                HLSImage.RGBToHLS(*sourceByteR, *sourceByteG, *sourceByteB, ref *destByteH, ref *destByteL, ref *destByteS);
                                //HLSImage.RGBToHLS(255, 255, 0, ref *destByteH, ref *destByteL, ref *destByteS);

                                sumH += *destByteH;
                                sumL += *destByteL;
                                sumS += *destByteS;

                                if (i != (count) - 1)
                                {
                                    destByteH += 3;
                                    destByteL += 3;
                                    destByteS += 3;
                                    sourceByteR += bytes;
                                    sourceByteG += bytes;
                                    sourceByteB += bytes;
                                }

                            }
                            smallImage.HLSImage.GlobalHue = sumH / count;
                            smallImage.HLSImage.GlobalLightness = sumL / count;
                            smallImage.HLSImage.GlobalSaturation = sumS / count;
                            smallImage.HLSImage.GlobalSatLightnessDistance = HLSImage.CalculateSaturationLightnessDistance(
                                smallImage.HLSImage.GlobalSaturation, smallImage.HLSImage.GlobalLightness);
                        }
                    }
                }
                #endregion                

                intermediateData = ByteArrayBitmap.DeAllocateBytes(intermediateData);
                //smallImage.ToBitmap().Save(@"D:\temp\small.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                return smallImage;
            }
            catch (Exception er)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
                if (this.Disposed)
                    Console.WriteLine(_disposeCallLog);
                return null;
            }
        }


        static int _countsaves = 0;
        /// <summary>
        /// An attempt at replicating the above code using pointer arathamtic
        /// </summary>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public unsafe ByteArrayBitmap GetSubSampledBytestream_Bilinear_NoTransparancyCalc_V3_WithMax_retBytemap(int newWidth, int newHeight)
        {
            try
            {
                ByteArrayBitmap smallImage = new ByteArrayBitmap(newWidth, newHeight, this.Format);
                smallImage.DataGrayscale = MakeBytes(newWidth * newHeight);
                float fWidth = this.Width, fHeight = this.Height;
                float fNewWidth = newWidth, fNewHeight = newHeight;
                float xStep = (fWidth - 2) / fNewWidth;
                float yStep = (fHeight - 2) / fNewHeight;

                // new
                // Due to an accurate, but insanely difficult rounding system used by mark in the
                // previous function (and sub functions) we have to do a little clever tracking to
                // ensure our floats round correctly.

                int _bytesPerPixelMinusOne = _bytesPerPixel - 1;

                byte[] intermediateData = MakeBytes(4 * newWidth * newHeight * (_bytesPerPixelMinusOne));

                // pixels gathered
                // 1 2
                // 4 5
                unsafe
                {
                    fixed (byte* sourceDataStart = &this.Data[0])
                    {
                        fixed (byte* destDataStart = &intermediateData[0])
                        {
                            int Pval = 0;
                            int Ptarget = 0;

                            byte* sourceByte = sourceDataStart;
                            byte* destByte = destDataStart;
                            for (float i = 0; i < fNewHeight; i++)
                            {

                                // wiggle factor for interpolation
                                for (int k = -1; k < 2; k += 2)
                                {
                                    sourceByte = sourceDataStart;
                                    // move to start of row
                                    //int ss = ((int)(i * yStep) + 1 + k) * this.Stride;
                                    sourceByte += ((int)(i * yStep) + 1 + k) * this.Stride;
                                    Pval = -1;
                                    Ptarget = 0;
                                    for (float j = 0; j < fNewWidth; j++)
                                    {

                                        Pval++;
                                        Ptarget = (int)(j * xStep) + 1;
                                        sourceByte += (((Ptarget - Pval) - 1) * _bytesPerPixel);
                                        Pval = Ptarget;
                                        for (int c = 0; c < _bytesPerPixelMinusOne; c++)
                                        {
                                            *destByte = *sourceByte;
                                            destByte++;
                                            sourceByte++;
                                        }
                                        sourceByte += (1 * _bytesPerPixel) + 1;
                                        for (int c = 0; c < _bytesPerPixelMinusOne; c++)
                                        {
                                            *destByte = *sourceByte;
                                            destByte++;
                                            sourceByte++;
                                        }
                                        sourceByte -= (_bytesPerPixelMinusOne);
                                    }
                                }
                            }
                        }
                    }
                }
                // we now have our intermediate data, we now need to translate this data into the
                // final bitmap

                short[] pot = new short[newWidth * newHeight * _bytesPerPixelMinusOne];
                int counter = 0;


                unsafe
                {
                    fixed (byte* sourceDataStart = &intermediateData[0])
                    {
                        fixed (short* destDataStart = &pot[0])
                        {
                            byte* sourceByte = sourceDataStart;
                            short* destByte = destDataStart;
                            for (int j = 0; j < fNewHeight; j++)
                            {
                                // 1 and 2
                                destByte = destDataStart;
                                destByte += j * (int)fNewWidth * _bytesPerPixelMinusOne;
                                for (int i = 0; i < fNewWidth; i++)
                                {
                                    for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                    {
                                        *destByte += *sourceByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                    destByte -= _bytesPerPixelMinusOne;
                                    for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                    {
                                        *destByte += *sourceByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                }
                                // 4 and 5
                                destByte = destDataStart;
                                destByte += j * (int)fNewWidth * _bytesPerPixelMinusOne;
                                for (int i = 0; i < fNewWidth; i++)
                                {
                                    for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                    {
                                        *destByte += *sourceByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                    destByte -= _bytesPerPixelMinusOne;
                                    for (int k = 0; k < _bytesPerPixelMinusOne; k++)
                                    {
                                        *destByte += *sourceByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                }

                            }
                        }
                    }
                }

                unsafe
                {
                    int DataGrayscaleDenominator = 3;
                    //avg - greyscale
                    fixed (short* sourceDataStart = &pot[0])
                    {
                        fixed (byte* destDataStart = &smallImage.Data[0])
                        {
                            fixed (byte* destDataGrayStart = &smallImage.DataGrayscale[0])
                            {
                                byte* destByte = destDataStart;
                                byte* destByteGray = destDataGrayStart;
                                short* sourceByte = sourceDataStart;
                                for (int i = 0; i < (newWidth * newHeight); i++)
                                {
                                    int tot = 0;
                                    for (int j = 0; j < _bytesPerPixelMinusOne; j++)
                                    {
                                        *destByte = Convert.ToByte((*sourceByte >> 2));
                                        tot += *destByte;
                                        destByte++;
                                        sourceByte++;
                                    }
                                    destByte++;
                                    *destByteGray = Convert.ToByte((tot / DataGrayscaleDenominator));
                                    //*destByteGray = Convert.ToByte((tot >> 2));
                                    destByteGray++;
                                }
                            }
                        }
                    }


                    ByteArrayBitmap avgbmp = ByteArrayBitmap.FromGrayScaleData(smallImage.DataGrayscale, newWidth, newHeight);

                    //max of channels
                    fixed (short* sourceDataStart = &pot[0])
                    {
                        fixed (byte* destDataStart = &smallImage.Data[0])
                        {
                            fixed (byte* destDataGrayStart = &smallImage.DataGrayscale[0])
                            {
                                byte* destByte = destDataStart;
                                byte* destByteGray = destDataGrayStart;
                                short* sourceByte = sourceDataStart;
                                for (int i = 0; i < (newWidth * newHeight); i++)
                                {
                                    //int tot = 0;
                                    for (int j = 0; j < _bytesPerPixelMinusOne; j++)
                                    {
                                        *destByte = Convert.ToByte((*sourceByte >> 2));
                                        //tot += *destByte;
                                        *destByteGray = Math.Max(*destByte, *destByteGray);
                                        destByte++;
                                        sourceByte++;
                                    }
                                    destByte++;
                                    //*destByteGray = Convert.ToByte((tot >> 2));
                                    destByteGray++;
                                }
                            }
                        }
                    }


                    ByteArrayBitmap maxbmp = ByteArrayBitmap.FromGrayScaleData(smallImage.DataGrayscale, newWidth, newHeight);

                    ByteArrayBitmap diffmap = ByteArrayBitmap.Subtract(maxbmp, avgbmp);

                    smallImage = maxbmp;

                    _countsaves++;
                }
                //ByteArrayBitmap bmp = new ByteArrayBitmap(smallImage.DataGrayscale, newWidth, newHeight, PixelFormats.Gray8);
                //transpose to smallimage
                //for(int i = 0; i < newWidth; i++)
                //    for (int j = 0; j < newHeight; j++)
                //    {
                //        byte gsb = smallImage.DataGrayscale[i + j * newWidth];
                //        smallImage.SetColor(i, j, new byte[] { gsb, gsb, gsb });
                //    }

                return smallImage;
            }
            catch (Exception er)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
                return null;
            }
        }

        /// <summary>
        /// Slow
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static ByteArrayBitmap FromGrayScaleData(byte[] data, int width, int height)
        {
            ByteArrayBitmap bmp = new ByteArrayBitmap(width, height, PixelFormats.Bgra32);
            //transpose to smallimage
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    byte gsb = data[i + j * width];
                    bmp.SetColor(i, j, new byte[] { gsb, gsb, gsb });
                }

            return bmp;
        }
        /// <summary>
        /// Slow
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static ByteArrayBitmap FromBooleanData(bool[] data, int width, int height)
        {
            try
            {

                ByteArrayBitmap bmp = new ByteArrayBitmap(width, height, PixelFormats.Bgra32);
                //transpose to smallimage
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                    {
                        if (data[i + j * width])
                            bmp.SetColor(i, j, new byte[] { 255, 255, 255 });
                        else
                            bmp.SetColor(i, j, new byte[] { 0, 0, 0 });
                    }

                return bmp;

            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
            }
            return null;
        }
        /// <summary>
        /// Slow
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static ByteArrayBitmap FromIntMaskData(int[] data, int width, int height, int trueStartsAt)
        {
            try
            {

                ByteArrayBitmap bmp = new ByteArrayBitmap(width, height, PixelFormats.Bgra32);
                //transpose to smallimage
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                    {
                        if (data[i + j * width] >= trueStartsAt)
                            bmp.SetColor(i, j, new byte[] { 255, 255, 255 });
                        else
                            bmp.SetColor(i, j, new byte[] { 0, 0, 0 });
                    }

                return bmp;

            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
            }
            return null;
        }

        public static ByteArrayBitmap FromIntMaskData(int[,] data, int trueStartsAt)
        {
            try
            {

                ByteArrayBitmap bmp = new ByteArrayBitmap(data.GetLength(0), data.GetLength(1), PixelFormats.Bgra32);
                //transpose to smallimage
                for (int i = 0; i < data.GetLength(0); i++)
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        if (data[i, j] >= trueStartsAt)
                            bmp.SetColor(i, j, new byte[] { 255, 255, 255 });
                        else
                            bmp.SetColor(i, j, new byte[] { 0, 0, 0 });
                    }

                return bmp;

            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
            }
            return null;
        }

        public static ByteArrayBitmap FromIntRegionData(int[,] data)
        {
            try
            {
                Dictionary<int, int> lookup = new Dictionary<int, int>();
                ByteArrayBitmap bmp = new ByteArrayBitmap(data.GetLength(0), data.GetLength(1), PixelFormats.Bgra32);
                List<System.Drawing.Color> otherColours = new List<System.Drawing.Color>();
                //transpose to smallimage
                for (int i = 0; i < data.GetLength(0); i++)
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        if (data[i, j] == -1)
                            bmp.SetColor(i, j, new byte[] { 0, 0, 0 });
                        else if (lookup.Count < 20)
                        {
                            if (!(new List<int>(lookup.Keys)).Contains(data[i, j]))
                            {
                                lookup.Add(data[i, j], lookup.Count);
                            }
                                bmp.SetColor(i, j, _kellysMaxContrastSet[lookup[data[i, j]]]);
                        }
                        else
                        {
                            if (!(new List<int>(lookup.Keys)).Contains(data[i, j]))
                            {
                                lookup.Add(data[i, j], lookup.Count);
                                Random R = new Random();
                                otherColours.Add(System.Drawing.Color.FromArgb(255, R.Next(50, 255), R.Next(50, 255), R.Next(50, 255)));
                            }
                            if (lookup[data[i, j]] - 20 < 0)
                                bmp.SetColor(i, j, _kellysMaxContrastSet[lookup[data[i, j]]]);
                            else
                            bmp.SetColor(i, j, otherColours[lookup[data[i, j]] - 20]);
                        }
                    }

                return bmp;

            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
            }
            return null;
        }

        static public System.Drawing.Color UIntToColor(uint color)
        {
            var a = (byte)(color >> 24);
            var r = (byte)(color >> 16);
            var g = (byte)(color >> 8);
            var b = (byte)(color >> 0);
            return System.Drawing.Color.FromArgb(a, r, g, b);
        }

        private static readonly List<System.Drawing.Color> _kellysMaxContrastSet = new List<System.Drawing.Color>
{
    UIntToColor(0xFFFFB300), //Vivid Yellow
    UIntToColor(0xFF803E75), //Strong Purple
    UIntToColor(0xFFFF6800), //Vivid Orange
    UIntToColor(0xFFA6BDD7), //Very Light Blue
    UIntToColor(0xFFC10020), //Vivid Red
    UIntToColor(0xFFCEA262), //Grayish Yellow
    UIntToColor(0xFF817066), //Medium Gray

    //The following will not be good for people with defective color vision
    UIntToColor(0xFF007D34), //Vivid Green
    UIntToColor(0xFFF6768E), //Strong Purplish Pink
    UIntToColor(0xFF00538A), //Strong Blue
    UIntToColor(0xFFFF7A5C), //Strong Yellowish Pink
    UIntToColor(0xFF53377A), //Strong Violet
    UIntToColor(0xFFFF8E00), //Vivid Orange Yellow
    UIntToColor(0xFFB32851), //Strong Purplish Red
    UIntToColor(0xFFF4C800), //Vivid Greenish Yellow
    UIntToColor(0xFF7F180D), //Strong Reddish Brown
    UIntToColor(0xFF93AA00), //Vivid Yellowish Green
    UIntToColor(0xFF593315), //Deep Yellowish Brown
    UIntToColor(0xFFF13A13), //Vivid Reddish Orange
    UIntToColor(0xFF232C16), //Dark Olive Green
};


        public static ByteArrayBitmap FromBoolMaskData(bool[,] data)
        {
            try
            {

                ByteArrayBitmap bmp = new ByteArrayBitmap(data.GetLength(0), data.GetLength(1), PixelFormats.Bgra32);
                //transpose to smallimage
                for (int i = 0; i < data.GetLength(0); i++)
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        if (data[i, j])
                            bmp.SetColor(i, j, new byte[] { 255, 255, 255 });
                        else
                            bmp.SetColor(i, j, new byte[] { 0, 0, 0 });
                    }

                return bmp;

            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
            }
            return null;
        }

        //public static ByteArrayBitmap Grid(params ByteArrayBitmap[] bitmaps)
        //{
        //    int totalwidth = 
        //}

        public static ByteArrayBitmap Subtract(ByteArrayBitmap one, ByteArrayBitmap two)
        {
            byte[] data = MakeBytes(one.Data.Length);

            for (int i = 0; i < one.Data.Length; i++)
            {
                int v = Math.Abs(one.Data[i] - two.Data[i]);
                data[i] = (byte)Math.Min(v, 255);
            }
            return new ByteArrayBitmap(data, one.Width, one.Height, one.Format);
        }

        public static ByteArrayBitmap Subtract_unfsafe(ByteArrayBitmap one, ByteArrayBitmap two)
        {
            byte[] data = MakeBytes(one.Data.Length);
            try
            {
                unsafe
                {
                    fixed (byte* destDataStart = &data[0])
                    {
                        fixed (byte* sourceDataStartA = &one.Data[0])
                        {
                            fixed (byte* sourceDataStartB = &two.Data[0])
                            {
                                byte* sourceByteA = sourceDataStartA;
                                byte* sourceByteB = sourceDataStartB;
                                byte* destByte = destDataStart;

                                for (int i = 0; i < one.Data.Length - 1; i++)
                                {
                                    *destByte = (byte)Math.Min(Math.Abs(*sourceByteA - *sourceByteB), 255);

                                    sourceByteA++;
                                    sourceByteB++;
                                    destByte++;
                                }
                                *destByte = (byte)Math.Min(Math.Abs(*sourceByteA - *sourceByteB), 255);
                            }
                        }
                    }
                }
            }
            catch (Exception er)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
            }
            return new ByteArrayBitmap(data, one.Width, one.Height, one.Format);
        }

        private Kinesense.Interfaces.Useful.DataConverters_NonStatic IDataConverter = new Useful.DataConverters_NonStatic();

        /// <summary>
        /// Gets a grayscale byt version assuming the image is 4 bpp
        /// </summary>
        /// <returns></returns>
        private unsafe byte[,] GetGrayScale_4bpp()
        {
            try
            {
                unsafe
                {
                    byte[] dumDeDar = MakeBytes(this.Width * this.Height);
                    fixed (byte* sourceDataStart = &this.Data[0])
                    {
                        fixed (byte* destDataStart = &dumDeDar[0])
                        {
                            byte* sourceByte = sourceDataStart;
                            byte* destByte = destDataStart;

                            //sourceByte += this.Stride + _bytesPerPixel;
                            //destByte += this.Width + 1;

                            for (int i = 0; i < this.Height; i++)
                            {
                                for (int k = 0; k < this.Width; k++)
                                {
                                    int grayScale = 0;

                                    grayScale += *sourceByte;
                                    sourceByte++;
                                    grayScale += *sourceByte;
                                    sourceByte++;
                                    grayScale += *sourceByte;
                                    sourceByte++;
                                    sourceByte++;

                                    byte grayScaleShort = (byte)(grayScale / _bytesPerPixel);


                                    *destByte += grayScaleShort;
                                    destByte++;
                                }
                                //sourceByte += _bytesPerPixel + _bytesPerPixel;
                                //destByte += 2;
                            }
                        }
                    }

                    byte[,] d = IDataConverter.OneDArrayToTwoDArray(dumDeDar, this.Width, this.Height);


                    return d;
                }

            }
            catch (Exception er)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
            }
            return null;
        }

        /// <summary>
        /// Recovers the data from the origional image, and returns the HorizC data
        /// </summary>
        /// <returns></returns>
        public unsafe short[,] GetHorizCData_4BytesPerPixel()
        {
            try
            {
                unsafe
                {
                    short[] dumDeDar = new short[this.Width * this.Height];
                    fixed (byte* sourceDataStart = &this.Data[0])
                    {
                        fixed (short* destDataStart = &dumDeDar[0])
                        {
                            byte* sourceByte = sourceDataStart;
                            short* destByte = destDataStart;

                            sourceByte += this.Stride + _bytesPerPixel;
                            destByte += this.Width + 1;

                            for (int i = 1; i < this.Height - 1; i++)
                            {
                                for (int k = 1; k < this.Width - 1; k++)
                                {
                                    int grayScale = 0;

                                    grayScale += *sourceByte;
                                    sourceByte++;
                                    grayScale += *sourceByte;
                                    sourceByte++;
                                    grayScale += *sourceByte;
                                    sourceByte++;
                                    sourceByte++;

                                    short grayScaleShort = (short)(grayScale / _bytesPerPixel);

                                    destByte--;
                                    *destByte += grayScaleShort;
                                    destByte++;
                                    *destByte += grayScaleShort;
                                    *destByte += grayScaleShort;
                                    destByte++;
                                    *destByte += grayScaleShort;
                                }
                                sourceByte += _bytesPerPixel + _bytesPerPixel;
                                destByte += 2;
                            }
                        }
                    }

                    short[,] d = IDataConverter.OneDArrayToTwoDArray(dumDeDar, this.Width, this.Height);
                    return d;
                }

            }
            catch (Exception er)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
            }
            return null;
        }


        /// <summary>
        /// Returns the data using the DataGrayscale datasource if it has been filled
        /// </summary>
        /// <returns></returns>
        public unsafe short[,] GetHorizCData_4BytesPerPixel_AltGrayScale()
        {
            if (this.DataGrayscale == null || this.DataGrayscale.Length == 0)
                return GetHorizCData_4BytesPerPixel();

            try
            {
                unsafe
                {
                    short[] dumDeDar = new short[this.Width * this.Height];
                    fixed (byte* sourceDataStart = &this.DataGrayscale[0])
                    {
                        fixed (short* destDataStart = &dumDeDar[0])
                        {
                            byte* sourceByte = sourceDataStart;
                            short* destByte = destDataStart;

                            sourceByte += this.Width + 1;
                            destByte += this.Width + 1;

                            for (int i = 1; i < this.Height - 1; i++)
                            {
                                for (int k = 1; k < this.Width - 1; k++)
                                {
                                    short grayScaleShort = *sourceByte;
                                    sourceByte++;

                                    destByte--;
                                    *destByte += grayScaleShort;
                                    destByte++;
                                    *destByte += grayScaleShort;
                                    *destByte += grayScaleShort;
                                    destByte++;
                                    *destByte += grayScaleShort;
                                }
                                //sourceByte += _bytesPerPixel + _bytesPerPixel;
                                sourceByte += 2;
                                destByte += 2;
                            }
                        }
                    }

                    short[,] d = IDataConverter.OneDArrayToTwoDArray(dumDeDar, this.Width, this.Height);
                    return d;
                }

            }
            catch (Exception er)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
            }
            return null;
        }




        /// <summary>
        /// Returns a BitmapSource that has been scaled to be below the given sizes
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public BitmapSource ToBitmapSourceScaledToBelow(int width, int height)
        {
            //Stopwatch SW = new Stopwatch();
            //SW.Start();

            // check if work needs doing
            if (this.Width <= width && this.Height <= height)
                return this.ToBitmapSource(96, 96);

            ByteArrayBitmap smallBAB = null;

            try
            {
                int nWidth, nHeight;

                if ((this.Width / width) > (this.Height / height)) // scale down by width
                {
                    nWidth = width;
                    nHeight = (int)Math.Floor((double)this.Height / (double)(this.Width / width));
                }
                else // scale down by height
                {
                    nWidth = (int)Math.Floor((double)this.Width / (double)(this.Height / height));
                    nHeight = height;
                }

                smallBAB = this.GetResized(nWidth, nHeight);
                return smallBAB.ToBitmapSource(96, 96);

                //return this.GetResized(nWidth, nHeight).ToBitmapSource(96, 96);
            }
            catch (Exception er)
            {
                DebugMessageLogger.LogError(er);
                return null;
            }
            finally
            {
                if (smallBAB != null)
                    smallBAB.Dispose();

                //SW.Stop();
                //Kinesense.Interfaces.DebugMessageLogger.LogAltDebug(SW.ElapsedTicks.ToString());
            }
        }

        /// <summary>
        /// Returns the image as a Bitmap image
        /// </summary>
        /// <returns></returns>
        public BitmapImage ToBitmapImage()
        {
            try
            {
                BitmapSource bitmapSource = this.ToBitmapSource(96, 96);

                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                MemoryStream memoryStream = new MemoryStream();
                BitmapImage bImg = new BitmapImage();

                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(memoryStream);

                bImg.BeginInit();
                bImg.StreamSource = new MemoryStream(memoryStream.ToArray());
                bImg.EndInit();

                memoryStream.Close();

                return bImg;
            }
            catch (Exception er)
            {
                DebugMessageLogger.LogError(er);
                return null;
            }
        }


        public double SumAllValues()
        {
            double sum = 0;
            foreach (byte b in this.Data)
                sum += b;
            return sum;
        }



        #region IDisposable Members

        private bool _InhibitDispose = false;
        private bool _DisposeOnDeInhibit = false;

        public bool InhibitDispose
        {
            get { return _InhibitDispose; }
            set
            {
                _InhibitDispose = value;
                if (_DisposeOnDeInhibit)
                    this.Dispose();
            }
        }


        ~ByteArrayBitmap()
        {
            //double d = (Data == null ? 0 : (double)Data.Length / 1000000d);
            //if (d > 1)
            //{
            //    string s = string.Format("ByteArrayDestructor: {0} Age: {1}ms Size={2}mp {3}",
            //        OriginThread,
            //        (DateTime.Now - _wasBorn).TotalMilliseconds,
            //        d,
            //        _stacktrace
            //        );
            //    DebugMessageLogger.LogEvent(s);
            //    Console.WriteLine(s);
            //}
            Dispose();
        }

        protected bool Disposed = false;
        protected bool Disposing = false;

        static int _countDispose = 0;


        public string _disposeCallLog;

        public virtual void Dispose()
        {
            try
            {
                if (!_InhibitDispose)
                {
                    if (!this.Disposed)
                    {
                        _disposeCallLog = Environment.StackTrace;
                        Disposing = true;

                        if (this.Data != null)
                            this.Data = DeAllocateBytes(this.Data);

                        if (this.DataGrayscale != null)
                            this.DataGrayscale = DeAllocateBytes(this.DataGrayscale);

                        if (this.DataHSV != null)
                            this.DataHSV = DeAllocateBytes(this.DataHSV);

                        if (this.HLSImage != null)
                        {
                            this.HLSImage.Dispose();
                            this.HLSImage = null;
                        }
                    }
                    //GC.SuppressFinalize(this);
                    this.Disposed = true;
                    this.Disposing = false;

                    _countDispose++;

                    //if (_countDispose % 50 == 0)
                      //  GC.Collect();
                }
                _DisposeOnDeInhibit = true;
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }
        }

        #endregion
    }


}