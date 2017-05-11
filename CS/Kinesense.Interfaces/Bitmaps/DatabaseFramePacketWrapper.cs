using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Kinesense.Interfaces.Bitmaps
{
    public class DatabaseFramePacketWrapper : IDisposable
    {
        private const string ContentLengthPrefix = "Content-length: ";
        private const string ContentTypePrefix = "Content-type: ";
        private const string ContentTimeStampPrefix = "Content-timestamp: ";
        private const string ContentHashPrefix = "Content-hash: ";
        //private const string TimeStampFormat = "yyyy-MM-dd HH:mm:ss.fffffff";
        private const string ContentPrivateHashPrefix = "Content-privathash: ";
        private const string FrameHeaderFormat = "--videoboundary\r\nContent-type: image/jpeg\r\nContent-length: {0}\r\nContent-timestamp: {1}";

        /// <summary>
        /// pass in the full byte content of a Frame packet from the database.
        /// </summary>
        /// <param name="buffer"></param>
        public DatabaseFramePacketWrapper(byte[] buffer)
        {
            _timePosIndex = ByteArrayUtils.GetTimeStampsFromFrameIndex(buffer);
            _timePosDictionary = new Dictionary<DateTime, int>();
            _times = new List<DateTime>();
            _buffer = buffer;

            foreach (KeyValuePair<DateTime, int> kvp in _timePosIndex)
            {
                _timePosDictionary.Add(kvp.Key, kvp.Value);
                _times.Add(kvp.Key);
                _positions.Add(kvp.Value);
            }

            StartTime = _times.FirstOrDefault();
            EndTime = _times.LastOrDefault();
        }

        public void AddFrame(VideoFrame frame)
        {
            if (StartTime < frame.DBTime)
                StartTime = frame.DBTime;
            if (EndTime > frame.DBTime)
                EndTime = frame.DBTime;

            _timePosDictionary.Add(frame.DBTime, 0);
            _timePosIndex.Add(new KeyValuePair<DateTime, int>(frame.DBTime, 0));
            _times.Add(frame.DBTime);
            _jpegDataIndex.Add(new KeyValuePair<DateTime, byte[]>(frame.DBTime, frame.EncodedData));
        }

        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

        public DatabaseFramePacketWrapper(byte[] buffer, DateTime startTime, DateTime endTime)
        {
            _timePosIndex = ByteArrayUtils.GetTimeStampsFromFrameIndex(buffer, startTime, endTime - startTime);
            _timePosDictionary = new Dictionary<DateTime, int>();
            _times = new List<DateTime>();
            _buffer = buffer;

            foreach (KeyValuePair<DateTime, int> kvp in _timePosIndex)
            {
                _timePosDictionary.Add(kvp.Key, kvp.Value);
                _times.Add(kvp.Key);
            }

        }
        
        /// <summary>
        /// new object for building a FramePacket
        /// </summary>
        public DatabaseFramePacketWrapper()
        {
            _timePosDictionary = new Dictionary<DateTime, int>();
            _timePosIndex = new List<KeyValuePair<DateTime, int>>();
            _times = new List<DateTime>();
            _jpegDataIndex = new List<KeyValuePair<DateTime, byte[]>>();
        }

        public List<KeyValuePair<DateTime, byte[]>> JpegDataIndex { get { return _jpegDataIndex; } }

        public void AddNewFrameData(DateTime time, ByteArrayBitmap bitmap)
        {
            ///debug
            _countAdded++;

            byte[] jpegdata = null;

            JpegBitmapEncoder encoder = new JpegBitmapEncoder { QualityLevel = 96 };

            encoder.Frames.Add(BitmapFrame.Create(bitmap.ToBitmapSource()));
            using (MemoryStream imageMemoryStream = new MemoryStream())
            {
                encoder.Save(imageMemoryStream);
                jpegdata = imageMemoryStream.ToArray();
            }

            _jpegDataIndex.Add(new KeyValuePair<DateTime, byte[]>(time, jpegdata));
        }

        /// <summary>
        /// Like ConvertJpegDataToMJpegByteArray but with an index at the end
        /// </summary>
        /// <returns></returns>
        public byte[] MakeBuffer(bool writeindex)
        {
            byte[] buffer = null;
            try
            {
                //sort by time, ascending
                _jpegDataIndex.Sort(new IndexEntryComparer());

                FramePacketIndex index = new FramePacketIndex();

                using (MemoryStream memoryStream = new MemoryStream())
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                {
                    foreach (KeyValuePair<DateTime, byte[]> kvp in _jpegDataIndex)
                    {
                        ///debug
                        _countExported++;

                        index.Add(kvp.Key, (int)memoryStream.Position);

                        if (kvp.Value != null)
                        {

                            binaryWriter.Write(
                                Encoding.ASCII.GetBytes(
                                string.Format(DatabaseFramePacketWrapper.FrameHeaderFormat, kvp.Value.Length, kvp.Key)));
                            binaryWriter.Write(kvp.Value);
                        }
                        //memoryStream.WriteTo(binaryWriter.BaseStream);
                    }

                    if (writeindex)
                    {
                        byte[] indexbytes = index.ToBytes();

                        binaryWriter.Seek(0, SeekOrigin.End);
                        binaryWriter.Write(Encoding.ASCII.GetBytes("--videoboundary--"));

                        int indexPosition = (int)(binaryWriter.BaseStream.Position);

                        binaryWriter.Write(indexbytes);

                        binaryWriter.Write(BitConverter.GetBytes(indexPosition));
                    }

                    buffer = memoryStream.ToArray();
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee);

            }

            return buffer;
        }

        public void AddNewFrameData(DateTime time, byte[] jpegdata)
        {
            ///debug
            _countAdded++;

            //byte[] jpegdata = null;

            //JpegBitmapEncoder encoder = new JpegBitmapEncoder { QualityLevel = 90 };

            //encoder.Frames.Add(BitmapFrame.Create(bitmap.ToBitmapSource()));
            //using (MemoryStream imageMemoryStream = new MemoryStream())
            //{
            //    encoder.Save(imageMemoryStream);
            //    jpegdata = imageMemoryStream.ToArray();
            //}

            _jpegDataIndex.Add(new KeyValuePair<DateTime, byte[]>(time, jpegdata));
        }

        private class IndexEntryComparer : IComparer<KeyValuePair<DateTime, byte[]>>
        {
            #region IComparer<Interval> Members

            public int Compare(KeyValuePair<DateTime, byte[]> x, KeyValuePair<DateTime, byte[]> y)
            {
                //this line prevents argument exception due to a x==x test not returning 0
                if (x.Key == y.Key)
                    return 0;

                if (x.Key < y.Key)
                    return -1;
                if (x.Key > y.Key)
                    return 1;
                return 0;
            }

            #endregion
        }

        //debug
        static public int _countAdded = 0;
        static public int _countExported = 0;

        public byte[] ConvertJpegDataToMJpegByteArray()
        {
            byte[] buffer = null;
            try
            {
                //sort by time, ascending
                _jpegDataIndex.Sort(new IndexEntryComparer());

                using (MemoryStream memoryStream = new MemoryStream())
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                {
                    foreach (KeyValuePair<DateTime, byte[]> kvp in _jpegDataIndex)
                    {
                        ///debug
                        _countExported++;

                        if (kvp.Value != null)
                        {

                            binaryWriter.Write(
                                Encoding.ASCII.GetBytes(
                                string.Format(DatabaseFramePacketWrapper.FrameHeaderFormat, kvp.Value.Length, kvp.Key)));
                            binaryWriter.Write(kvp.Value);
                        }
                        //memoryStream.WriteTo(binaryWriter.BaseStream);
                    }
                    
                    buffer = memoryStream.ToArray();
                }
            }
            catch(OutOfMemoryException ee)
            {
                throw;
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }

            return buffer;
        }

        byte[] _buffer;
        List<KeyValuePair<DateTime, int>> _timePosIndex;
        Dictionary<DateTime, int> _timePosDictionary;
        List<DateTime> _times;
        List<KeyValuePair<DateTime, byte[]>> _jpegDataIndex;

        List<int> _positions = new List<int>();
        public List<int> Positions { get { return _positions; } }

        public byte[] Buffer { get { return _buffer; } }

        public List<DateTime> GetDateTimeList()
        {
            List<DateTime> _list = new List<DateTime>();
            foreach (KeyValuePair<DateTime, int> kvp in _timePosIndex)
                _list.Add(kvp.Key);
            return _list;
        }

        public ByteArrayBitmap GetByteArrayBitmap(DateTime time)
        {
            byte[] jpgbuffer = GetFrameData(time);
            BitmapDecoder decoder;
            using (MemoryStream memoryStream = new MemoryStream(jpgbuffer))
                decoder = BitmapDecoder.Create(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            BitmapFrame bitmapFrame = decoder.Frames[0];

            return new ByteArrayBitmap(bitmapFrame);
        }

        public byte[] GetFrameData(int frameNumber)
        {
            byte[] jpgBuffer = null;

            try
            {
                int pos = _positions[frameNumber];

                //Read Frame Header:
                int linelimit = 0;
                //content Length
                string contentLengthString = ByteArrayUtils.GetPrefixedLine(
                    _buffer, ContentLengthPrefix, pos, 400, Encoding.ASCII, out linelimit);
                //Hash code 
                //string hashString = ByteArrayUtils.GetPrefixedLine(
                //    _buffer, ContentHashPrefix, linelimit, 400, Encoding.ASCII, out linelimit);

                //string privatehashString = ByteArrayUtils.GetPrefixedLine(
                //    _buffer, ContentPrivateHashPrefix, linelimit, 400, Encoding.ASCII, out linelimit);

                //use linelimit as start point to copy the jpeg data
                int contentLength = int.Parse(contentLengthString);

                //This should work always, but its possible that the bytes 0x00 and 0x10 could be different sometimes. 
                //keep an eye out for weird behaviour. See http://www.garykessler.net/library/file_sigs.html
                //
                // Mark you fool, they are different. Why didn't you just modify the search algo to begin with
                //
                //byte[] jpegHeaderpattern = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46 };
                //byte?[] jpegHeaderpattern = new byte?[] { 0xFF, 0xD8, 0xFF, 0xE0, null, null, 0x4A, 0x46, 0x49, 0x46 };
                int datastart = ByteArrayUtils.IndexOf(_buffer, JpegBitmapEncoderFactory.JpegHeaderPattern, linelimit, 400);
                if (datastart == -1)
                    Console.WriteLine("Search for jpeg header failed; MultipartVideoDecoder.GetAllFrames()");

                //return bytes
                jpgBuffer = new byte[contentLength];
                Array.Copy(_buffer, datastart, jpgBuffer, 0, contentLength);
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee);
            }

            return jpgBuffer;
        }

        public byte[] GetFrameData(DateTime time)
        {
            byte[] jpgBuffer = null;

            try
            {
                //parse header
                int pos = -1;
                if (!_timePosDictionary.TryGetValue(time, out pos))
                    return null;

                //Read Frame Header:
                int linelimit = 0;
                //content Length
                string contentLengthString = ByteArrayUtils.GetPrefixedLine(
                    _buffer, ContentLengthPrefix, pos, 400, Encoding.ASCII, out linelimit);
                //Hash code 
                //string hashString = ByteArrayUtils.GetPrefixedLine(
                //    _buffer, ContentHashPrefix, linelimit, 400, Encoding.ASCII, out linelimit);

                //string privatehashString = ByteArrayUtils.GetPrefixedLine(
                //    _buffer, ContentPrivateHashPrefix, linelimit, 400, Encoding.ASCII, out linelimit);

                //use linelimit as start point to copy the jpeg data
                int contentLength = int.Parse(contentLengthString);

                //This should work always, but its possible that the bytes 0x00 and 0x10 could be different sometimes. 
                //keep an eye out for weird behaviour. See http://www.garykessler.net/library/file_sigs.html
                //
                // Gerrr Mark
                //
                //byte[] jpegHeaderpattern = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46 };
                byte?[] jpegHeaderpattern = new byte?[] { 0xFF, null, 0xFF, null };
                int datastart = ByteArrayUtils.IndexOf(_buffer, jpegHeaderpattern, linelimit, 400);
                if (datastart == -1)
                    DebugMessageLogger.LogEvent("Search for jpeg header failed; MultipartVideoDecoder.GetAllFrames()");

                //return bytes
                jpgBuffer = new byte[contentLength];
                Array.Copy(_buffer, datastart, jpgBuffer, 0, contentLength);
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }

            return jpgBuffer;
        }

        #region IDisposable Members

        public void Dispose()
        {
            _buffer = null;
            _jpegDataIndex = null;
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    //public class DatabaseFramePacketWrapper_old : IDisposable
    //{
    //    private const string ContentLengthPrefix = "Content-length: ";
    //    private const string ContentTypePrefix = "Content-type: ";
    //    private const string ContentTimeStampPrefix = "Content-timestamp: ";
    //    private const string ContentHashPrefix = "Content-hash: ";
    //    //private const string TimeStampFormat = "yyyy-MM-dd HH:mm:ss.fffffff";
    //    private const string ContentPrivateHashPrefix = "Content-privathash: ";
    //    private const string FrameHeaderFormat = "--videoboundary\r\nContent-type: image/jpeg\r\nContent-length: {0}\r\nContent-timestamp: {1}";

    //    List<int> _positions = new List<int>();
    //    public List<int> Positions { get { return _positions; } }
    //    /// <summary>
    //    /// pass in the full byte content of a Frame packet from the database.
    //    /// </summary>
    //    /// <param name="buffer"></param>
    //    public DatabaseFramePacketWrapper_old(byte[] buffer)
    //    {
    //        _timePosIndex = ByteArrayUtils.GetTimeStampsFromFrameIndex(buffer);
    //        _timePosDictionary = new Dictionary<DateTime, int>();
    //        _times = new List<DateTime>();
    //        _buffer = buffer;

    //        foreach (KeyValuePair<DateTime, int> kvp in _timePosIndex)
    //        {
    //            _timePosDictionary.Add(kvp.Key, kvp.Value);
    //            _times.Add(kvp.Key);
    //            _positions.Add(kvp.Value);
    //        }

    //        StartTime = _times.FirstOrDefault();
    //        EndTime = _times.LastOrDefault();
    //    }

    //    public void AddFrame(VideoFrame frame)
    //    {
    //        if (StartTime < frame.DBTime)
    //            StartTime = frame.DBTime;
    //        if (EndTime > frame.DBTime)
    //            EndTime = frame.DBTime;

    //        _timePosDictionary.Add(frame.DBTime, 0);
    //        _timePosIndex.Add(new KeyValuePair<DateTime, int>(frame.DBTime, 0));
    //        _times.Add(frame.DBTime);
    //        _jpegDataIndex.Add(new KeyValuePair<DateTime, byte[]>(frame.DBTime, frame.EncodedData));
    //    }

    //    public DateTime StartTime { get; private set; }
    //    public DateTime EndTime { get; private set; }

    //    public byte[] Buffer { get { return _buffer; } }

    //    public double GetFrameRate()
    //    {
    //        TimeSpan duration = _times.Last() - _times.First();
    //        return _times.Count / duration.TotalSeconds;
    //    }

    //    /// <summary>
    //    /// new object for building a FramePacket
    //    /// </summary>
    //    public DatabaseFramePacketWrapper_old()
    //    {
    //        _timePosDictionary = new Dictionary<DateTime, int>();
    //        _timePosIndex = new List<KeyValuePair<DateTime, int>>();
    //        _times = new List<DateTime>();
    //        _jpegDataIndex = new List<KeyValuePair<DateTime, byte[]>>();
    //    }

    //    //public void AddNewFrameData(DateTime time, ByteArrayBitmap bitmap)
    //    //{
    //    //    ///debug
    //    //    _countAdded++;

    //    //    byte[] jpegdata = null;

    //    //    JpegBitmapEncoder encoder = new JpegBitmapEncoder {QualityLevel = 90};

    //    //    encoder.Frames.Add(BitmapFrame.Create(bitmap.ToBitmapSource()));
    //    //    using (MemoryStream imageMemoryStream = new MemoryStream())
    //    //    {
    //    //        encoder.Save(imageMemoryStream);
    //    //        jpegdata = imageMemoryStream.ToArray();
    //    //    }

    //    //    _jpegDataIndex.Add(new KeyValuePair<DateTime, byte[]>(time, jpegdata));
    //    //}

    //    private class IndexEntryComparer : IComparer<KeyValuePair<DateTime, byte[]>>
    //    {
    //        #region IComparer<Interval> Members

    //        public int Compare(KeyValuePair<DateTime, byte[]> x, KeyValuePair<DateTime, byte[]> y)
    //        {
    //            //this line prevents argument exception due to a x==x test not returning 0
    //            if (x.Key == y.Key)
    //                return 0;

    //            if (x.Key < y.Key)
    //                return -1;
    //            if (x.Key > y.Key)
    //                return 1;
    //            return 0;
    //        }

    //        #endregion
    //    }

    //    /// <summary>
    //    /// Like ConvertJpegDataToMJpegByteArray but with an index at the end
    //    /// </summary>
    //    /// <returns></returns>
    //    public byte[] MakeBuffer(bool writeindex)
    //    {
    //        byte[] buffer = null;
    //        try
    //        {
    //            //sort by time, ascending
    //            _jpegDataIndex.Sort(new IndexEntryComparer());

    //            FramePacketIndex index = new FramePacketIndex();

    //            using (MemoryStream memoryStream = new MemoryStream())
    //            using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
    //            {
    //                foreach (KeyValuePair<DateTime, byte[]> kvp in _jpegDataIndex)
    //                {
    //                    ///debug
    //                    _countExported++;

    //                    index.Add(kvp.Key, (int)memoryStream.Position);

    //                    if (kvp.Value != null)
    //                    {

    //                        binaryWriter.Write(
    //                            Encoding.ASCII.GetBytes(
    //                            string.Format(DatabaseFramePacketWrapper_old.FrameHeaderFormat, kvp.Value.Length, kvp.Key)));
    //                        binaryWriter.Write(kvp.Value);
    //                    }
    //                    //memoryStream.WriteTo(binaryWriter.BaseStream);
    //                }

    //                if (writeindex)
    //                {
    //                    byte[] indexbytes = index.ToBytes();

    //                    binaryWriter.Seek(0, SeekOrigin.End);
    //                    binaryWriter.Write(Encoding.ASCII.GetBytes("--videoboundary--"));

    //                    int indexPosition = (int)(binaryWriter.BaseStream.Position);

    //                    binaryWriter.Write(indexbytes);

    //                    binaryWriter.Write(BitConverter.GetBytes(indexPosition));
    //                }

    //                buffer = memoryStream.ToArray();
    //            }
    //        }
    //        catch (Exception ee)
    //        {
    //            Console.WriteLine(ee);

    //        }

    //        return buffer;
    //    }

    //    //debug
    //    static public int _countAdded = 0;
    //    static public int _countExported = 0;

    //    public byte[] ConvertJpegDataToMJpegByteArray()
    //    {
    //        return MakeBuffer(false);
    //    }

    //    byte[] _buffer;
    //    List<KeyValuePair<DateTime, int>> _timePosIndex;
    //    Dictionary<DateTime, int> _timePosDictionary;
    //    List<DateTime> _times;
    //    List<KeyValuePair<DateTime, byte[]>> _jpegDataIndex;

    //    public List<DateTime> GetDateTimeList()
    //    {
    //        List<DateTime> _list = new List<DateTime>();
    //        foreach (KeyValuePair<DateTime, int> kvp in _timePosIndex)
    //            _list.Add(kvp.Key);
    //        return _list;
    //    }

    //    public BitmapFrame GetFrame(int pos)
    //    {
    //        byte[] jpgbuffer = GetFrameData(pos);
    //        BitmapDecoder decoder;
    //        using (MemoryStream memoryStream = new MemoryStream(jpgbuffer))
    //            decoder = BitmapDecoder.Create(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
    //        BitmapFrame bitmapFrame = decoder.Frames[0];

    //        return bitmapFrame;
    //    }


    //    public BitmapFrame GetFrame(DateTime time)
    //    {
    //        byte[] jpgbuffer = GetFrameData(time);
    //        BitmapDecoder decoder;
    //        using (MemoryStream memoryStream = new MemoryStream(jpgbuffer))
    //            decoder = BitmapDecoder.Create(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
    //        BitmapFrame bitmapFrame = decoder.Frames[0];

    //        return bitmapFrame;
    //    }

    //    public byte[] GetFrameData(int frameNumber)
    //    {
    //        byte[] jpgBuffer = null;

    //        try
    //        {
    //            int pos = _positions[frameNumber];

    //            //Read Frame Header:
    //            int linelimit = 0;
    //            //content Length
    //            string contentLengthString = ByteArrayUtils.GetPrefixedLine(
    //                _buffer, ContentLengthPrefix, pos, 400, Encoding.ASCII, out linelimit);
    //            //Hash code 
    //            //string hashString = ByteArrayUtils.GetPrefixedLine(
    //            //    _buffer, ContentHashPrefix, linelimit, 400, Encoding.ASCII, out linelimit);

    //            //string privatehashString = ByteArrayUtils.GetPrefixedLine(
    //            //    _buffer, ContentPrivateHashPrefix, linelimit, 400, Encoding.ASCII, out linelimit);

    //            //use linelimit as start point to copy the jpeg data
    //            int contentLength = int.Parse(contentLengthString);

    //            //This should work always, but its possible that the bytes 0x00 and 0x10 could be different sometimes. 
    //            //keep an eye out for weird behaviour. See http://www.garykessler.net/library/file_sigs.html
    //            //
    //            // Mark you fool, they are different. Why didn't you just modify the search algo to begin with
    //            //
    //            //byte[] jpegHeaderpattern = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46 };
    //            //byte?[] jpegHeaderpattern = new byte?[] { 0xFF, 0xD8, 0xFF, 0xE0, null, null, 0x4A, 0x46, 0x49, 0x46 };
    //            int datastart = ByteArrayUtils.IndexOf(_buffer, JpegBitmapEncoderFactory.JpegHeaderPattern, linelimit, 400);
    //            if (datastart == -1)
    //                Console.WriteLine("Search for jpeg header failed; MultipartVideoDecoder.GetAllFrames()");

    //            //return bytes
    //            jpgBuffer = new byte[contentLength];
    //            Array.Copy(_buffer, datastart, jpgBuffer, 0, contentLength);
    //        }
    //        catch (Exception ee)
    //        {
    //            Console.WriteLine(ee);
    //        }

    //        return jpgBuffer;
    //    }

    //    public byte[] GetFrameData(DateTime time)
    //    {
    //        byte[] jpgBuffer = null;

    //        try
    //        {
    //            //parse header
    //            int pos = -1;
    //            if (!_timePosDictionary.TryGetValue(time, out pos))
    //                return null;

    //            //Read Frame Header:
    //            int linelimit = 0;
    //            //content Length
    //            string contentLengthString = ByteArrayUtils.GetPrefixedLine(
    //                _buffer, ContentLengthPrefix, pos, 400, Encoding.ASCII, out linelimit);
    //            //Hash code 
    //            //string hashString = ByteArrayUtils.GetPrefixedLine(
    //            //    _buffer, ContentHashPrefix, linelimit, 400, Encoding.ASCII, out linelimit);

    //            //string privatehashString = ByteArrayUtils.GetPrefixedLine(
    //            //    _buffer, ContentPrivateHashPrefix, linelimit, 400, Encoding.ASCII, out linelimit);

    //            //use linelimit as start point to copy the jpeg data
    //            int contentLength = int.Parse(contentLengthString);

    //            //This should work always, but its possible that the bytes 0x00 and 0x10 could be different sometimes. 
    //            //keep an eye out for weird behaviour. See http://www.garykessler.net/library/file_sigs.html
    //            //
    //            // There are far too many versions of this code
    //            //
    //            //byte[] jpegHeaderpattern = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46 };
    //            //byte?[] jpegHeaderpattern = new byte?[] { 0xFF, 0xD8, 0xFF, 0xE0, null, null, 0x4A, 0x46, 0x49, 0x46 };
    //            int datastart = ByteArrayUtils.IndexOf(_buffer, JpegBitmapEncoderFactory.JpegHeaderPattern, linelimit, 400);
    //            if (datastart == -1)
    //                Console.WriteLine("Search for jpeg header failed; MultipartVideoDecoder.GetAllFrames()");

    //            //return bytes
    //            jpgBuffer = new byte[contentLength];
    //            Array.Copy(_buffer, datastart, jpgBuffer, 0, contentLength);
    //        }
    //        catch (Exception ee)
    //        {
    //            Console.WriteLine(ee);
    //        }

    //        return jpgBuffer;
    //    }

    //    #region IDisposable Members

    //    public void Dispose()
    //    {
    //        _buffer = null;
    //        _jpegDataIndex = null;
    //        GC.SuppressFinalize(this);
    //    }

    //    #endregion
    //}
}
