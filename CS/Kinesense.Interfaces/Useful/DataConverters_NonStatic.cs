using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Useful
{
    public class DataConverters_NonStatic
    {
        public unsafe short[,] OneDArrayToTwoDArray(short[] source, int width, int height)
        {
            try
            {
                if ((width * height) != source.Length)
                    throw new Exception("Missmatched source and output");

                unsafe
                {
                    short[,] d = new short[width, height];
                    fixed (short* sourceDataStart = &source[0])
                    {
                        fixed (short* destDataStart = &d[0, 0])
                        {
                            short* sourceByte = sourceDataStart;
                            short* destByte = destDataStart;


                            for (int i = 0; i < height; i++)
                            {
                                destByte = destDataStart;
                                destByte += i;
                                for (int k = 0; k < width; k++)
                                {
                                    *destByte += *sourceByte;
                                    destByte += height;
                                    sourceByte++;
                                }

                            }
                        }
                    }
                    return d;
                }

            }
            catch (Exception er)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
            }
            return null;
        }

        public unsafe byte[,] OneDArrayToTwoDArray(byte[] source, int width, int height)
        {
            try
            {
                if ((width * height) != source.Length)
                    throw new Exception("Missmatched source and output");

                unsafe
                {
                    byte[,] d = new byte[width, height];
                    fixed (byte* sourceDataStart = &source[0])
                    {
                        fixed (byte* destDataStart = &d[0, 0])
                        {
                            byte* sourceByte = sourceDataStart;
                            byte* destByte = destDataStart;


                            for (int i = 0; i < height-1; i++)
                            {
                                destByte = destDataStart;
                                destByte += i;
                                for (int k = 0; k < width; k++)
                                {
                                    *destByte += *sourceByte;
                                    destByte += height;
                                    sourceByte++;
                                }

                            }
                            destByte = destDataStart;
                            destByte += height - 1;
                            for (int k = 0; k < width - 1; k++)
                            {
                                *destByte = *sourceByte;
                                destByte += height;
                                sourceByte++;
                            }
                            *destByte = *sourceByte;
                        }
                    }
                    return d;
                }

            }
            catch (Exception er)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
            }
            return null;
        }



    }
}
