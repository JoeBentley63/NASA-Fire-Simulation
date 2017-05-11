using System;

namespace Kinesense.Interfaces.Useful
{
    public static class DataConverters
    {
        public static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        //these are used for sanity checking datetimes being returned from decoded files
        public static DateTime DVREpochStart = new DateTime(1970, 1, 1, 0, 0, 0);
        public static DateTime DVREpochEnd = new DateTime(2070, 1, 1, 0, 0, 0);


        public static bool IsDateTimeSane(DateTime dt)
        {
            return (dt >= DVREpochStart && dt <= DVREpochEnd);
        }
        /// <summary>
        /// Converts UnixTimeStamp into DateTime
        /// </summary>
        /// <param name="uts"></param>
        /// <returns></returns>
        public static DateTime UnixTimeStampTODateTime_safe(int uts)
        {
            TimeSpan ts = new TimeSpan(0, 0, 0, uts);
            DateTime dt = UnixEpoch;
            return dt + ts;
        }

        /// <summary>
        /// Converts DateTime into Unix time Stamp
        /// </summary>
        public static double DateTimeTOUnixTimeStamp_safe(DateTime DT)
        {
            DateTime dt = UnixEpoch;
            TimeSpan ts = DT - dt;
            return ts.TotalSeconds;
        }

        /// <summary>
        /// Converts UnixTimeStamp into DateTime
        /// </summary>
        /// <param name="uts"></param>
        /// <returns></returns>
        public static DateTime UnixTimeStampTODateTime_safe(double uts)
        {
            DateTime dt = UnixEpoch.AddSeconds(uts);
            return dt;
        }




        /// <summary>
        /// Reverses the byte order of an array
        /// </summary>
        /// <param name="inArray"></param>
        /// <returns></returns>
        public static byte[] ReverseBytes_safe(byte[] inArray)
        {
            byte temp;
            int highCtr = inArray.Length - 1;

            for (int ctr = 0; ctr < inArray.Length / 2; ctr++)
            {
                temp = inArray[ctr];
                inArray[ctr] = inArray[highCtr];
                inArray[highCtr] = temp;
                highCtr -= 1;
            }
            return inArray;
        }





        public static Char ByteStreamToCharWithEndianReverse_unsafe(byte[] source, int sourceStart)
        {
            byte[] buff = new byte[2];
            Array.Copy(source, sourceStart, buff, 0, 2);
            buff = ReverseBytes_safe(buff);
            return BitConverter.ToChar(buff, 0);
        }
        public static Int16 ByteStreamToInt16WithEndianReverse_unsafe(byte[] source, int sourceStart)
        {
            byte[] buff = new byte[2];
            Array.Copy(source, sourceStart, buff, 0, 2);
            buff = ReverseBytes_safe(buff);
            return BitConverter.ToInt16(buff, 0);
        }
        public static UInt16 ByteStreamToUInt16WithEndianReverse_unsafe(byte[] source, int sourceStart)
        {
            byte[] buff = new byte[2];
            Array.Copy(source, sourceStart, buff, 0, 2);
            buff = ReverseBytes_safe(buff);
            return BitConverter.ToUInt16(buff, 0);
        }
        public static Int32 ByteStreamToInt32WithEndianReverse_unsafe(byte[] source, int sourceStart)
        {
            byte[] buff = new byte[4];
            Array.Copy(source, sourceStart, buff, 0, 4);
            buff = ReverseBytes_safe(buff);
            return BitConverter.ToInt32(buff, 0);
        }
        public static UInt32 ByteStreamToUInt32WithEndianReverse_unsafe(byte[] source, int sourceStart)
        {
            byte[] buff = new byte[4];
            Array.Copy(source, sourceStart, buff, 0, 4);
            buff = ReverseBytes_safe(buff);
            return BitConverter.ToUInt32(buff, 0);
        }

        public static byte[] UInt32ToBigEndianBytes(UInt32 uint32)
        {
            var result = BitConverter.GetBytes(uint32);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(result);

            return result;
        }
    }
}
