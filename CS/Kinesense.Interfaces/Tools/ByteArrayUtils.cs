using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Kinesense.Interfaces
{
    /// <summary>
    /// Some internal utilities for handling arrays.
    /// </summary>
    /// 
    public class ByteArrayUtils
    {



        public static bool AreEqualOld(byte[] array1, byte[] array2)
        {
            if (array2 == null || array1.Length != array2.Length)
                return false;

            for (int i = 0; i < array1.Length; ++i)
                if (array1[i] != array2[i])
                {
                    return false;
                }

            return true;
        }

        public static unsafe bool AreEqual(byte[] array1, byte[] array2)
        {
            if (array1 == null || array2 == null || array1.Length != array2.Length && array1.Length > 0)
                return false;
        
            unsafe
            {
                fixed (byte* sourceDataStart = &array1[0])
                {
                    fixed (byte* destDataStart = &array2[0])
                    {
                        byte* sourceByte = sourceDataStart;
                        byte* destByte = destDataStart;
                        for (int i = 0; i < array1.Length-1; ++i)
                        {
                            if (*sourceByte != *destByte)
                                return false;

                            sourceByte++;
                            destByte++;
                        }
                        if (*sourceByte != *destByte)
                            return false;
                    }
                }
            }
            return true;
        }

        public static bool GetDimensionsOfImageFromJpegData(byte[] jpegdata, out int width, out int height)
        {
            bool success = false;
            width = 0;
            height = 0;
            int loc = 0;
            try
            {
                int trys = 0;
                while (success == false && loc != -1 && loc < 2000 && trys < 20)
                {

                    loc = ByteArrayUtils.IndexOf(jpegdata, new byte[] { 0xFF, 0xC0 }, loc, 2000);

                    if (loc > 0)
                    {
                        byte[] heightbyte = new byte[] { jpegdata[loc + 6], jpegdata[loc + 5] }; //make BigEndian
                        byte[] widthbyte = new byte[] { jpegdata[loc + 8], jpegdata[loc + 7] };

                        height = BitConverter.ToInt16(heightbyte, 0);

                        width = BitConverter.ToInt16(widthbyte, 0);

                        if (height > 0 && height < 65535
                            && width > 0 && width < 65535)
                            success = true;
                        else
                        {
                            success = false;
                            loc += 2;
                            trys++;
                        }
                    }
                }

                if (success == false)
                    DebugMessageLogger.LogEvent("Failed to recover good height ({0}) and width ({1}) from jpeg data. Tried {2} times", width, height, trys);
            }
            catch (Exception ee)
            {
                success = false;
                DebugMessageLogger.LogError(ee);
            }

            return success;
        }

        public static void CopyToBuffer(byte[] destination, int val, int loc)
        {
            Array.Copy(BitConverter.GetBytes(val), 0, destination, loc, 4);
        }

        public static void CopyToBuffer(byte[] destination, uint val, int loc)
        {
            Array.Copy(BitConverter.GetBytes(val), 0, destination, loc, 4);
        }

        public static void CopyToBuffer(byte[] destination, short val, int loc)
        {
            Array.Copy(BitConverter.GetBytes(val), 0, destination, loc, 2);
        }

        public static void CopyToBuffer(byte[] destination, ushort val, int loc)
        {
            Array.Copy(BitConverter.GetBytes(val), 0, destination, loc, 2);
        }


        /// <summary>
        /// Check if the array contains needle at specified position.
        /// </summary>
        /// 
        /// <param name="array">Source array to check for needle.</param>
        /// <param name="needle">Needle we are searching for.</param>
        /// <param name="startIndex">Start index in source array.</param>
        /// 
        /// <returns>Returns <b>true</b> if the source array contains the needle at
        /// the specified index. Otherwise it returns <b>false</b>.</returns>
        /// 
        public static bool Compare(byte[] array, byte[] needle, int startIndex)
        {
            int needleLen = needle.Length;
            // compare
            for (int i = 0, p = startIndex; i < needleLen; i++, p++)
                if (array[p] != needle[i])
                    return false;
            return true;
        }

        /// <summary>
        /// Returns the index of the first occurence of an array of elements within another array.
        /// If the search fails, returns -1;
        /// </summary>
        /// 
        /// <param name="array">Source array to search for needle.</param>
        /// <param name="elements">Needle we are searching for.</param>
        /// <param name="startIndex">Start index in source array.</param>
        /// <param name="count">Number of bytes to search in the source array.</param>
        /// 
        /// <returns>Returns starting position of the needle if it was found or <b>-1</b> otherwise.</returns>
        /// 
        public static int IndexOf(byte[] array, byte[] elements, int startIndex, int count)
        {
            int elementsLength = elements.Length;

            while (count >= elementsLength)
            {
                // find first element
                int index = Array.IndexOf(array, elements[0], startIndex, count - elementsLength + 1);

                // if we did not find even the first element, then the search is failed
                if (index == -1)
                    return -1;

                int i, p;
                // check for elements
                for (i = 0, p = index; i < elementsLength; ++i, ++p)
                {
                    if (array[p] != elements[i])
                        break;
                }

                if (i == elementsLength)
                    // elements found
                    return index;

                // continue to search for elements
                count -= index - startIndex + 1;
                startIndex = index + 1;
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of the first occurence of an array of elements within another array.
        /// If the search fails, returns -1;
        /// </summary>
        /// 
        /// <param name="array">Source array to search for needle.</param>
        /// <param name="elements">Needle we are searching for.</param>
        /// <param name="startIndex">Start index in source array.</param>
        /// <param name="count">Number of bytes to search in the source array.</param>
        /// 
        /// <returns>Returns starting position of the needle if it was found or <b>-1</b> otherwise.</returns>
        /// 
        public static int IndexOf(byte[] array, byte?[] elements, int startIndex, int count)
        {
            int elementsLength = elements.Length;

            if (elements == null || elements.Length == 0 || !elements[0].HasValue)
                throw new Exception("Elements to search for not valid");

            while (count >= elementsLength)
            {
                // find first element
                int index = Array.IndexOf(array, elements[0].Value, startIndex, count - elementsLength + 1);

                // if we did not find even the first element, then the search is failed
                if (index == -1)
                    return -1;

                int i, p;
                // check for elements
                for (i = 0, p = index; i < elementsLength; ++i, ++p)
                {
                    if (elements[i].HasValue)
                        if (array[p] != elements[i])
                            break;
                }

                if (i == elementsLength)
                    // elements found
                    return index;

                // continue to search for elements
                count -= index - startIndex + 1;
                startIndex = index + 1;
            }
            return -1;
        }


        /// <summary>
        /// Returns the index of the first occurrence of an array of elements within another array,
        /// or returns the index of that array of elements clipped by the end of the search.
        /// If the search fails, returns -1;
        /// </summary>
        /// 
        /// <param name="array">Source array to search for needle.</param>
        /// <param name="elements">Needle we are searching for.</param>
        /// <param name="startIndex">Start index in source array.</param>
        /// <param name="count">Number of bytes to search in the source array.</param>
        /// 
        /// <returns>Returns starting position of the needle if it was found or <b>-1</b> otherwise.</returns>
        /// 
        public static int IndexOfClipped(byte[] array, byte[] elements, int startIndex, int count)
        {
            int elementsLength = elements.Length;
            int searchIndexLimit = startIndex + count;

            while (startIndex < searchIndexLimit)
            {
                // find first element
                int index = Array.IndexOf(array, elements[0], startIndex, count);

                // if we did not find even the first element, then the search is failed
                if (index == -1)
                    return -1;

                int i, p;
                // check for elements
                for (i = 0, p = index; (i < elementsLength) && (p < searchIndexLimit); ++i, ++p)
                {
                    if (array[p] != elements[i])
                        break;
                }

                if (i == elementsLength || p == searchIndexLimit)
                    // elements or elements clipped by end of search found
                    return index;

                // continue to search for elements
                count -= index - startIndex + 1;
                startIndex = index + 1;
            }
            return -1;
        }

        public static int IndexOfClipped(byte[] array, string elements, int startIndex, int count, Encoding encoding)
        {
            return ByteArrayUtils.IndexOfClipped(array, encoding.GetBytes(elements), startIndex, count);
        }

        public static int LastIndexOf(byte[] array, byte[] elements, int startIndex, int count)
        {
            int elementsLength = elements.Length;

            while (count >= elementsLength)
            {
                // find first element
                int index = Array.LastIndexOf(array, elements[elementsLength - 1], startIndex, count - elementsLength + 1);

                // if we did not find even the first element, then the search is failed
                if (index == -1)
                    return -1;

                int i, p;
                // check for elements
                for (i = elementsLength - 1, p = index; i >= 0; --i, --p)
                {
                    if (array[p] != elements[i])
                        break;
                }

                if (i < 0)
                    // elements found
                    return index;

                // continue to search for elements
                count -= startIndex - index + 1;
                startIndex = index - 1;
            }
            return -1;
        }

        public static string GetLine(byte[] array, int startIndex, int count, Encoding encoding, out int lineLimit)
        {
            lineLimit = Array.IndexOf<byte>(array, 10, startIndex, count);
            if (lineLimit < 0)
                return null;
            // if there is a carriage return (CR) character before, adjust the end position
            if (array[lineLimit - 1] == 13)
                --lineLimit;
            return encoding.GetString(array, startIndex, lineLimit - startIndex);
        }

        public static string GetLine(byte[] array, int startIndex, int count, Encoding encoding)
        {
            int lineLimit;
            return ByteArrayUtils.GetLine(array, startIndex, count, encoding, out lineLimit);
        }

        public static string GetPrefixedLine(byte[] array, string prefix, int startIndex, int count, Encoding encoding, out int lineLimit)
        {
            lineLimit = -1;
            byte[] prefixBytes = encoding.GetBytes(prefix);

            int prefixPosition = ByteArrayUtils.IndexOf(array, prefixBytes, startIndex, count);
            if (prefixPosition == -1)
                return null;
            return ByteArrayUtils.GetLine(array, prefixPosition + prefixBytes.Length,
                                          count - prefixPosition - prefixBytes.Length + startIndex, encoding, out lineLimit);
        }

        public static string GetPrefixedLine(byte[] array, string prefix, int startIndex, int count, Encoding encoding)
        {
            int lineLimit;
            return ByteArrayUtils.GetPrefixedLine(array, prefix, startIndex, count, encoding, out lineLimit);
        }

        public static string ToHexString(ICollection<byte> bytes)
        {
            StringBuilder hex = new StringBuilder(bytes.Count * 2);
            foreach (byte b in bytes)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static byte[] FromHexString(String hex)
        {
            int numberOfChars = hex.Length;
            byte[] bytes = new byte[numberOfChars / 2];
            for (int i = 0; i < numberOfChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static int CountJpegsFrames(string filename)
        {
            int count = 0;
            try
            {
                int batchsize = 1000;
                int linelimit = 0;
                byte?[] jpegHeaderpattern =
                    new byte?[] { 0x69, 0x6D, 0x61, 0x67, 0x65, 0x2F, 0x6A, 0x70, 0x65, 0x67 };
                    //new byte?[] { 0x43, 0x6F, 0x6E, 0x74, 0x65, 0x6E, 0x74, 0x2D, 0x6C, 0x65, 0x6E, 0x67, 0x74, 0x68, 0x3A };
                    //new byte?[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46 };
                    //new byte?[]{0x2D, 0x2D, 0x76, 0x69, 0x64, 0x65, 0x6F, 0x62, 0x6F, 0x75, 0x6E, 0x64, 0x61, 0x72, 0x79};
                    //new byte?[] { 0xFF, null, 0xFF, null };
                byte[] buffer = new byte[batchsize];
                using (BinaryReader br = new BinaryReader(File.Open(filename, FileMode.Open, FileAccess.Read)))
                {
                    while (br.BaseStream.Length > br.BaseStream.Position)
                    {
                        int datastart = ByteArrayUtils.IndexOf(buffer, jpegHeaderpattern, linelimit, batchsize);
                        if (datastart != -1)
                            count++;
                        Array.Copy(br.ReadBytes(batchsize), buffer, batchsize);
                    }
                }
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }
            return count;
        }

        public static DateTime ChangeDateTimeKind(DateTime t, DateTimeKind kind)
        {
            return new DateTime(
                t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second, t.Millisecond, kind);
        }

        /// <summary>
        /// This method looks for the index at the end of the frame packet. This should be in the format DateTime.Int
        /// following a final "--videoboundary"
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="stampA"></param>
        /// <param name="stampB"></param>
        /// <returns></returns>
        public static List<KeyValuePair<DateTime, int>> GetTimeStampsFromFrameIndex(byte[] buffer, DateTime start, TimeSpan duration)
        {
            List<KeyValuePair<DateTime, int>> times = new List<KeyValuePair<DateTime, int>>();
            long utcmask = ((long)DateTimeKind.Utc << 62);
            long localantimask = ~((long)DateTimeKind.Local << 62);

            DateTime end = start + duration;

            //this is because the gob start time from the db can be slightly off the first frame time
            DateTime startLimit = start.AddSeconds(-120);
            DateTime endLimit = end.AddSeconds(120);
            TimeSpan? offset = null;

            //long unspmask = ~((long)3 << 62);

            try
            {
                ASCIIEncoding encoder = new ASCIIEncoding();
                string stamp = "--videoboundary--";
                byte[] stampbytes = encoder.GetBytes(stamp);

                int index_of_index = -1;
                int pos = buffer.Length;

                while (index_of_index == -1 && pos > 0)
                {
                    pos -= 5000;
                    index_of_index = ByteArrayUtils.IndexOf(buffer, stampbytes, pos, buffer.Length - pos);
                }
                if (index_of_index == -1)
                    throw new IOException("Invalid buffer");

                pos = index_of_index + stampbytes.Length;
                while (pos < buffer.Length - 12)
                {
                    long bytes = BitConverter.ToInt64(buffer, pos);
                    bytes = bytes | utcmask;
                    bytes = bytes & localantimask;
                    //bytes = bytes & unspmask;
                    DateTime time = DateTime.FromBinary(bytes);

                    //this fixes the timezone issue
                    //time = ChangeDateTimeKind(time, DateTimeKind.Utc);

                    //rationality check, for timezone and daylight savings problems
                    if (time < startLimit || time > endLimit)
                    {
                         if (offset.HasValue == false)
                         {
                             long roundedticks = (long)((time.Ticks / 10000) * 10000);
                             long diff = time.Ticks - roundedticks;
                             if (diff > 5000) ///to unround the number (workaround for a bug on some pcs)
                                 roundedticks += 10000;
                             DateTime roundedTime = new DateTime(roundedticks);
                             //assume first frame
                             offset = start - roundedTime;

                            DebugMessageLogger.LogEvent("Time is off in this Frame Packet - adjusting by {0}", offset);
                        }
                        time = time + offset.Value;
                    }

                    int bytepos = BitConverter.ToInt32(buffer, pos + 8);

                    //sometimes there are blank bytes at the end of the file. Don't record these as index entries.
                    //if (bytepos > 0)
                    times.Add(new KeyValuePair<DateTime, int>(time, bytepos));

                    pos += 12;
                }
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }

            return times;
        }

        /// <summary>
        /// This method looks for the index at the end of the frame packet. This should be in the format DateTime.Int
        /// following a final "--videoboundary"
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="stampA"></param>
        /// <param name="stampB"></param>
        /// <returns></returns>
        public static List<KeyValuePair<DateTime, int>> GetTimeStampsFromFrameIndex(byte[] buffer)
        {
            List<KeyValuePair<DateTime, int>> times = new List<KeyValuePair<DateTime, int>>();
            long utcmask = ((long)DateTimeKind.Utc << 62);
            long localantimask = ~((long)DateTimeKind.Local << 62);

            long unspmask = ~((long)3 << 62);

            try
            {
                ASCIIEncoding encoder = new ASCIIEncoding();
                string stamp = "--videoboundary--";
                byte[] stampbytes = encoder.GetBytes(stamp);

                int index_of_index = -1;
                int pos = buffer.Length;

                while (index_of_index == -1 && pos > 0)
                {
                    pos -= 5000;
                    index_of_index = ByteArrayUtils.IndexOf(buffer, stampbytes, pos, buffer.Length - pos);
                }
                if (index_of_index == -1)
                    throw new IOException("Invalid buffer");

                pos = index_of_index + stampbytes.Length;
                while (pos < buffer.Length - 12)
                {
                    long bytes = BitConverter.ToInt64(buffer, pos);
                    //bytes = bytes | utcmask;
                    //bytes = bytes & localantimask;
                    bytes = bytes & unspmask;
                    DateTime time = DateTime.FromBinary(bytes);

                    //this fixes the timezone issue
                    time = ChangeDateTimeKind(time, DateTimeKind.Utc);

                    int bytepos = BitConverter.ToInt32(buffer, pos + 8);

                    //sometimes there are blank bytes at the end of the file. Don't record these as index entries.
                    //if (bytepos > 0)
                    times.Add(new KeyValuePair<DateTime, int>(time, bytepos));

                    pos += 12;
                }
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }

            return times;
        }
    }
}
