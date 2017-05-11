using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Useful
{
    //
    // These are some simple functions for doing a basic image combine.
    //
    // Useful for development purposes. The are to be considered too slow for algorithm 
    // use, but you may find use for them elsewhere
    //
    // Note they are a little rough and ready

    public static class ImageFilters
    {
        /// <summary>
        /// Returns a BAB containing the values where the two images match.
        /// Images must be same size etc...
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static ByteArrayBitmap VoteCombiner(ByteArrayBitmap A, ByteArrayBitmap B)
        {
            try
            {
                if (A.Width != B.Width)
                    throw new Exception("image error");

                if (A.Height != B.Height)
                    throw new Exception("image error");

                if (A.Data.Length != B.Data.Length)
                    throw new Exception("image error");

                byte[] data = new byte[A.Data.Length];

                for (int i = 0; i < A.Data.Length; i++)
                {
                    if (A.Data[i] == B.Data[i])
                        data[i] = A.Data[i];
                    else
                        data[i] = 0;
                }

                return new ByteArrayBitmap(data, A.Width, A.Height, A.Format);
            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
                return null;
            }
        }

        /// <summary>
        /// Returns an image that is the result of a simple regional analysis on the source images
        /// Look at the code to see what it really does
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="reqScore"></param>
        /// <param name="centreScore"></param>
        /// <returns></returns>
        public static ByteArrayBitmap VoteCombinerLocal(ByteArrayBitmap A, ByteArrayBitmap B, int reqScore, int centreScore)
        {
            try
            {
                if (A.Width != B.Width)
                    throw new Exception("image error");

                if (A.Height != B.Height)
                    throw new Exception("image error");

                if (A.Data.Length != B.Data.Length)
                    throw new Exception("image error");

                byte[] data = new byte[A.Data.Length];

                unsafe
                {
                    fixed (byte* AStart = &A.Data[0], BStart = &B.Data[0], DStart = &data[0])
                    {
                        // a,b,c
                        // d,e,f
                        // g,h,i

                        int span = A.Width * A.BytesPerPixel;

                        byte*[] a = new byte*[9];
                        byte*[] b = new byte*[9];

                        a[0] = AStart;
                        a[1] = AStart + 1 * A.BytesPerPixel;
                        a[2] = AStart + 2 * A.BytesPerPixel;
                        a[3] = AStart + span;
                        a[4] = AStart + span + 1 * A.BytesPerPixel;
                        a[5] = AStart + span + 2 * A.BytesPerPixel;
                        a[6] = AStart + span + span;
                        a[7] = AStart + span + span + 1 * A.BytesPerPixel;
                        a[8] = AStart + span + span + 2 * A.BytesPerPixel;

                        b[0] = BStart;
                        b[1] = BStart + 1 * A.BytesPerPixel;
                        b[2] = BStart + 2 * A.BytesPerPixel;
                        b[3] = BStart + span;
                        b[4] = BStart + span + 1 * A.BytesPerPixel;
                        b[5] = BStart + span + 2 * A.BytesPerPixel;
                        b[6] = BStart + span + span;
                        b[7] = BStart + span + span + 1 * A.BytesPerPixel;
                        b[8] = BStart + span + span + 2 * A.BytesPerPixel;

                        byte* dest = DStart + span + A.BytesPerPixel;

                        for (int z = 1; z < A.Height - 1; z++)
                        {
                            for (int y = 1; y < A.Width - 1; y++)
                            {
                                for (int x = 0; x < A.BytesPerPixel; x++)
                                {
                                    int score1 = 0;
                                    int score2 = 0;

                                    for (int i = 0; i < 9; i++)
                                    {
                                        if (*a[i] > 200)
                                        {
                                            if (i != 4)
                                                score1++;
                                            else
                                                score1 += centreScore;
                                        }

                                        if (*b[i] > 200)
                                        {
                                            if (i != 4)
                                                score2++;
                                            else
                                                score2 += centreScore;
                                        }

                                        if (z != A.Height - 2 || y != A.Width - 2)
                                        {
                                            a[i]++;
                                            b[i]++;
                                        }

                                    }
                                    if (score1 >= reqScore && score2 >= reqScore)
                                        *dest = 255;
                                    else
                                        *dest = 0;

                                    dest++;
                                }
                            }

                            if (z != A.Height - 2)
                            {
                                for (int i = 0; i < 9; i++)
                                {
                                    a[i] += 2 * A.BytesPerPixel;
                                    b[i] += 2 * A.BytesPerPixel;
                                }
                                dest += 2 * A.BytesPerPixel;
                            }

                        }
                    }
                }

                return new ByteArrayBitmap(data, A.Width, A.Height, A.Format);
            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
                return null;
            }
        }

        /// <summary>
        /// A simple filter to strip out lone pixels, and strengthen grouped pixles.
        /// Look at the code.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="reqScore"></param>
        /// <param name="centreScore"></param>
        /// <param name="requireCentre"></param>
        /// <returns></returns>
        public static ByteArrayBitmap SingleImageLocal(ByteArrayBitmap A, int reqScore, int centreScore, bool requireCentre)
        {
            try
            {
                byte[] data = new byte[A.Data.Length];

                unsafe
                {
                    fixed (byte* AStart = &A.Data[0], DStart = &data[0])
                    {
                        // a,b,c
                        // d,e,f
                        // g,h,i

                        int span = A.Width * A.BytesPerPixel;

                        byte*[] a = new byte*[9];

                        a[0] = AStart;
                        a[1] = AStart + 1 * A.BytesPerPixel;
                        a[2] = AStart + 2 * A.BytesPerPixel;
                        a[3] = AStart + span;
                        a[4] = AStart + span + 1 * A.BytesPerPixel;
                        a[5] = AStart + span + 2 * A.BytesPerPixel;
                        a[6] = AStart + span + span;
                        a[7] = AStart + span + span + 1 * A.BytesPerPixel;
                        a[8] = AStart + span + span + 2 * A.BytesPerPixel;


                        byte* dest = DStart + span + A.BytesPerPixel;

                        for (int z = 1; z < A.Height - 1; z++)
                        {
                            for (int y = 1; y < A.Width - 1; y++)
                            {
                                if (*a[4] < 200 && requireCentre)
                                {
                                    for (int x = 0; x < A.BytesPerPixel; x++)
                                    {
                                        for (int i = 0; i < 9; i++)
                                            if (z != A.Height - 2 || y != A.Width - 2)
                                                a[i]++;

                                        *dest = 0;
                                        dest++;
                                    }
                                }
                                else
                                {
                                    for (int x = 0; x < A.BytesPerPixel; x++)
                                    {
                                        int score1 = 0;
                                        int score2 = 0;

                                        for (int i = 0; i < 9; i++)
                                        {
                                            if (*a[i] > 200)
                                            {
                                                if (i != 4)
                                                    score1++;
                                                else
                                                    score1 += centreScore;
                                            }

                                            if (z != A.Height - 2 || y != A.Width - 2)
                                            {
                                                a[i]++;
                                            }

                                        }
                                        if (score1 >= reqScore)
                                            *dest = 255;
                                        else
                                            *dest = 0;

                                        dest++;
                                    }
                                }
                            }

                            if (z != A.Height - 2)
                            {
                                for (int i = 0; i < 9; i++)
                                {
                                    a[i] += 2 * A.BytesPerPixel;
                                }
                                dest += 2 * A.BytesPerPixel;
                            }

                        }
                    }
                }

                return new ByteArrayBitmap(data, A.Width, A.Height, A.Format);
            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
                return null;
            }
        }

        public static ByteArrayBitmap SingleImage_Enforcer(ByteArrayBitmap A)
        {
            try
            {
                byte[] data = A.Data.Clone() as byte[];
                unsafe
                {
                    int newHeight = A.Height;
                    int newWidth = A.Width;
                    int bytesPerPixel = A.BytesPerPixel;

                    fixed (byte* destDataStart = &data[0])
                    {
                        fixed (byte* sourceDataStart = &A.Data[0])
                        {
                            byte* destByte = destDataStart + (int)(newHeight * bytesPerPixel * 2) + 2 * bytesPerPixel;

                            byte*[] sources = new byte*[25];
                            sources[00] = sourceDataStart;
                            sources[01] = sourceDataStart + 1 * bytesPerPixel;
                            sources[02] = sourceDataStart + 2 * bytesPerPixel;
                            sources[03] = sourceDataStart + 3 * bytesPerPixel;
                            sources[04] = sourceDataStart + 4 * bytesPerPixel;
                            sources[05] = sourceDataStart + (int)(newHeight * bytesPerPixel);
                            sources[06] = sourceDataStart + (int)(newHeight * bytesPerPixel) + 1 * bytesPerPixel;
                            sources[07] = sourceDataStart + (int)(newHeight * bytesPerPixel) + 2 * bytesPerPixel;
                            sources[08] = sourceDataStart + (int)(newHeight * bytesPerPixel) + 3 * bytesPerPixel;
                            sources[09] = sourceDataStart + (int)(newHeight * bytesPerPixel) + 4 * bytesPerPixel;
                            sources[10] = sourceDataStart + (int)(newHeight * bytesPerPixel * 2);
                            sources[11] = sourceDataStart + (int)(newHeight * bytesPerPixel * 2) + 1 * bytesPerPixel;
                            sources[12] = sourceDataStart + (int)(newHeight * bytesPerPixel * 2) + 2 * bytesPerPixel;
                            sources[13] = sourceDataStart + (int)(newHeight * bytesPerPixel * 2) + 3 * bytesPerPixel;
                            sources[14] = sourceDataStart + (int)(newHeight * bytesPerPixel * 2) + 4 * bytesPerPixel;
                            sources[15] = sourceDataStart + (int)(newHeight * bytesPerPixel * 3);
                            sources[16] = sourceDataStart + (int)(newHeight * bytesPerPixel * 3) + 1 * bytesPerPixel;
                            sources[17] = sourceDataStart + (int)(newHeight * bytesPerPixel * 3) + 2 * bytesPerPixel;
                            sources[18] = sourceDataStart + (int)(newHeight * bytesPerPixel * 3) + 3 * bytesPerPixel;
                            sources[19] = sourceDataStart + (int)(newHeight * bytesPerPixel * 3) + 4 * bytesPerPixel;
                            sources[20] = sourceDataStart + (int)(newHeight * bytesPerPixel * 4);
                            sources[21] = sourceDataStart + (int)(newHeight * bytesPerPixel * 4) + 1 * bytesPerPixel;
                            sources[22] = sourceDataStart + (int)(newHeight * bytesPerPixel * 4) + 2 * bytesPerPixel;
                            sources[23] = sourceDataStart + (int)(newHeight * bytesPerPixel * 4) + 3 * bytesPerPixel;
                            sources[24] = sourceDataStart + (int)(newHeight * bytesPerPixel * 4) + 4 * bytesPerPixel;

                            //int count = 2;
                            //int countStop = (int)newHeight - 1;

                            for (int i = 2; i < (newWidth - 2); i++)
                            {
                                for (int j = 2; j < newHeight - 2; j++)
                                {
                                    for (int k = 0; k < bytesPerPixel - 1; k++)
                                    {
                                        if (*sources[12] > 200)
                                        {
                                            bool found = false;
                                            for (int m = 0; m < 12; m++)
                                            {
                                                if (*sources[m] > 200)
                                                {
                                                    found = true;
                                                    break;
                                                }
                                            }
                                            if (!found)
                                            {
                                                for (int m = 13; m < 25; m++)
                                                {
                                                    if (*sources[m] > 200)
                                                    {
                                                        found = true;
                                                        break;
                                                    }
                                                }
                                            }
                                            if (!found)
                                                *destByte = 0;
                                        }
                                        else
                                        {
                                            int localCount = 0;
                                            if (*sources[6] > 200)
                                                localCount++;
                                            if (*sources[7] > 200)
                                                localCount++;
                                            if (*sources[8] > 200)
                                                localCount++;
                                            if (*sources[11] > 200)
                                                localCount++;
                                            if (*sources[13] > 200)
                                                localCount++;
                                            if (*sources[16] > 200)
                                                localCount++;
                                            if (*sources[17] > 200)
                                                localCount++;
                                            if (*sources[18] > 200)
                                                localCount++;

                                            if (localCount > 5)
                                                *destByte = 255;
                                        }
                                        int tot = 0;

                                        for (int q = 0; q < 25; q++)
                                        {
                                            sources[q]++;
                                        }
                                            destByte++;
                                    }
                                        if (i != (newWidth - 3) || j != newHeight - 3)
                                            for (int q = 0; q < 25; q++)
                                                sources[q]++;

                                        destByte++;
                                    
                                }
                                if (i != (newWidth - 3))
                                {
                                    //destByte++;

                                    for (int k = 0; k < 25; k++)
                                        sources[k] += (4 * bytesPerPixel);

                                    destByte += (4 * bytesPerPixel);
                                    //count = 2;

                                }
                            }
                        }
                    }

                    return new ByteArrayBitmap(data, A.Width, A.Height, A.Format);
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
