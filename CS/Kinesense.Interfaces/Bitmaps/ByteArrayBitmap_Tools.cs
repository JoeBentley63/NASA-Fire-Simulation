using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Classes
{
    public static class ByteArrayBitmap_Tools
    {

        /// <summary>
        /// Converts data that is in ImageOrder (1D array, row major) into c# standard 2D data
        /// </summary>
        /// <param name="source">Source Data in ImageOrder</param>
        /// <param name="width">Target width</param>
        /// <param name="height">Target Height</param>
        /// <returns>Result 2D array</returns>
        public static bool[,] ImageDataOrderTo2D(bool[] source, int width, int height)
        {
            try
            {
                if ((width * height) != source.Length)
                    throw new Exception("Mismatched source and output");

                unsafe
                {
                    bool[,] op = new bool[width, height];
                    fixed (bool* sourceDataStart = &source[0])
                    {
                        fixed (bool* destDataStart = &op[0, 0])
                        {
                            bool* sourceByte = sourceDataStart;
                            bool* destByte = destDataStart;


                            for (int i = 0; i < height - 1; i++)
                            {
                                destByte = destDataStart;
                                destByte += i;
                                for (int k = 0; k < width; k++)
                                {
                                    *destByte = *sourceByte;
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
                    return op;
                }


            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
                return null;
            }

        }

        /// <summary>
        /// Converts data that is in ImageOrder (1D array, row major) into c# standard 2D data
        /// </summary>
        /// <param name="source">Source Data in ImageOrder</param>
        /// <param name="width">Target width</param>
        /// <param name="height">Target Height</param>
        /// <returns>Result 2D array</returns>
        public static byte[,] ImageDataOrderTo2D(byte[] source, int width, int height)
        {
            try
            {
                if ((width * height) != source.Length)
                    throw new Exception("Mismatched source and output");

                unsafe
                {
                    byte[,] op = new byte[width, height];
                    fixed (byte* sourceDataStart = &source[0])
                    {
                        fixed (byte* destDataStart = &op[0, 0])
                        {
                            byte* sourceByte = sourceDataStart;
                            byte* destByte = destDataStart;


                            for (int i = 0; i < height - 1; i++)
                            {
                                destByte = destDataStart;
                                destByte += i;
                                for (int k = 0; k < width; k++)
                                {
                                    *destByte = *sourceByte;
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
                    return op;
                }


            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
                return null;
            }

        }

        /// <summary>
        /// Converts data that is in ImageOrder (1D array, row major) into c# standard 2D data
        /// </summary>
        /// <param name="source">Source Data in ImageOrder</param>
        /// <param name="width">Target width</param>
        /// <param name="height">Target Height</param>
        /// <returns>Result 2D array</returns>
        public static byte[] Convert2DToImageDataOrder(byte[,] source)
        {
            try
            {
                int stride = source.GetLength(0);
                int height = source.GetLength(1);

                unsafe
                {
                    byte[] op = new byte[stride* height];
                    fixed (byte* sourceDataStart = &op[0])
                    {
                        fixed (byte* destDataStart = &source[0, 0])
                        {
                            byte* sourceByte = sourceDataStart;
                            byte* destByte = destDataStart;

                            for (int i = 0; i < height - 1; i++)
                            {
                                destByte = destDataStart;
                                destByte += i;
                                for (int k = 0; k < stride; k++)
                                {
                                    *sourceByte  = * destByte;
                                    destByte += height;
                                    sourceByte++;
                                }
                            }
                            destByte = destDataStart;
                            destByte += height - 1;
                            for (int k = 0; k < stride - 1; k++)
                            {
                                *sourceByte  = * destByte;
                                destByte += height;
                                sourceByte++;
                            }
                            *sourceByte  = *destByte;
                        }
                    }
                    return op;
                }
            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
                return null;
            }

        }
    }
}
