using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Useful
{
    //
    // Some really terrible functions for stitcing some images side by side for debuging
    // These methods are very rushed and very rough and ready
    // All images must be the same size etc..., but there is an internal function for 
    // turning rgb data into rgbt data
    //
    // USE AT YOUR OWN RISK
    //

    public static class PutImagesSideBySide
    {

        public static ByteArrayBitmap PutImages(ByteArrayBitmap A, ByteArrayBitmap B)
        {
            try
            {
                if (A.Height != B.Height)
                    throw new Exception("Image missmatch");

                if (A.Width != B.Width)
                    throw new Exception("Image missmatch");




                if (A.BytesPerPixel != B.BytesPerPixel)
                {
                    if (A.BytesPerPixel == 3 && B.BytesPerPixel == 4)
                    {
                        byte[] mash = new byte[(int)((A.Data.Length / 3) * 4)];
                        int count = 0;
                        int count2 = 0;
                        while (count < A.Data.Length)
                        {
                            Array.Copy(A.Data, count, mash, count2, 3);
                            count += 3;
                            count2 += 4;
                        }
                        A = new ByteArrayBitmap(mash, A.Width, A.Height, B.Format);
                    }
                    if (A.BytesPerPixel != B.BytesPerPixel)
                    {
                        throw new Exception("Image missmatch");
                    }
                }


                int nWidth = A.Width * 2 + 20;
                int nHeight = A.Height;

                byte[] data = new byte[nWidth * nHeight * A.BytesPerPixel];
                int sourceStart = 0;
                int destForAStart = 0;
                //int destForBStart = A.Data.Length + (20 * A.Height*A.BytesPerPixel);
                int destForBStart = A.Width * A.BytesPerPixel + (20 * A.BytesPerPixel);

                for (int i = 0; i < nHeight; i++)
                {
                    Array.Copy(A.Data, sourceStart, data, destForAStart, A.Width * A.BytesPerPixel);
                    Array.Copy(B.Data, sourceStart, data, destForBStart, B.Width * A.BytesPerPixel);
                    sourceStart += A.Width * A.BytesPerPixel;
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                }


                //Array.Copy(A.Data, sourceStart, data, destForAStart, A.Data.Length);
                //Array.Copy(B.Data, sourceStart, data, destForBStart, A.Data.Length);

                return new ByteArrayBitmap(data, nWidth, nHeight, A.Format);
            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
            }
            return null;
        }

        public static ByteArrayBitmap PutImages(ByteArrayBitmap A, ByteArrayBitmap B, ByteArrayBitmap C)
        {
            try
            {
                if (A.Height != B.Height || B.Height != C.Height)
                    throw new Exception("Image missmatch");

                if (A.Width != B.Width || B.Width != C.Width)
                    throw new Exception("Image missmatch");




                if (A.BytesPerPixel != B.BytesPerPixel)
                {
                    if (A.BytesPerPixel == 3 && B.BytesPerPixel == 4)
                    {
                        byte[] mash = new byte[(int)((A.Data.Length / 3) * 4)];
                        int count = 0;
                        int count2 = 0;
                        while (count < A.Data.Length)
                        {
                            Array.Copy(A.Data, count, mash, count2, 3);
                            count += 3;
                            count2 += 4;
                        }
                        A = new ByteArrayBitmap(mash, A.Width, A.Height, B.Format);
                    }
                    if (A.BytesPerPixel != B.BytesPerPixel)
                    {
                        throw new Exception("Image missmatch");
                    }
                }


                int nWidth = A.Width * 3 + 40;
                int nHeight = A.Height;

                byte[] data = new byte[nWidth * nHeight * A.BytesPerPixel];
                int sourceStart = 0;
                int destForAStart = 0;
                //int destForBStart = A.Data.Length + (20 * A.Height*A.BytesPerPixel);
                int destForBStart = A.Width * A.BytesPerPixel + (20 * A.BytesPerPixel);
                int destForCStart = A.Width * A.BytesPerPixel * 2 + (40 * A.BytesPerPixel);

                for (int i = 0; i < nHeight; i++)
                {
                    Array.Copy(A.Data, sourceStart, data, destForAStart, A.Width * A.BytesPerPixel);
                    Array.Copy(B.Data, sourceStart, data, destForBStart, B.Width * A.BytesPerPixel);
                    Array.Copy(C.Data, sourceStart, data, destForCStart, C.Width * A.BytesPerPixel);
                    sourceStart += A.Width * A.BytesPerPixel;
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                }


                //Array.Copy(A.Data, sourceStart, data, destForAStart, A.Data.Length);
                //Array.Copy(B.Data, sourceStart, data, destForBStart, A.Data.Length);

                return new ByteArrayBitmap(data, nWidth, nHeight, A.Format);
            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
            }
            return null;
        }

        public static ByteArrayBitmap PutImages(ByteArrayBitmap A, ByteArrayBitmap B, ByteArrayBitmap C, ByteArrayBitmap D)
        {
            try
            {
                if (A.Height != B.Height || B.Height != C.Height || B.Height != D.Height)
                    throw new Exception("Image missmatch");

                if (A.Width != B.Width || B.Width != C.Width || B.Width != D.Width)
                    throw new Exception("Image missmatch");




                if (A.BytesPerPixel != B.BytesPerPixel)
                {
                    if (A.BytesPerPixel == 3 && B.BytesPerPixel == 4)
                    {
                        byte[] mash = new byte[(int)((A.Data.Length / 3) * 4)];
                        int count = 0;
                        int count2 = 0;
                        while (count < A.Data.Length)
                        {
                            Array.Copy(A.Data, count, mash, count2, 3);
                            count += 3;
                            count2 += 4;
                        }
                        A = new ByteArrayBitmap(mash, A.Width, A.Height, B.Format);
                    }
                    if (A.BytesPerPixel != B.BytesPerPixel)
                    {
                        throw new Exception("Image missmatch");
                    }
                }


                int nWidth = A.Width * 4 + 60;
                int nHeight = A.Height;

                byte[] data = new byte[nWidth * nHeight * A.BytesPerPixel];
                int sourceStart = 0;
                int destForAStart = 0;
                //int destForBStart = A.Data.Length + (20 * A.Height*A.BytesPerPixel);
                int destForBStart = A.Width * A.BytesPerPixel + (20 * A.BytesPerPixel);
                int destForCStart = A.Width * A.BytesPerPixel * 2 + (40 * A.BytesPerPixel);
                int destForDStart = A.Width * A.BytesPerPixel * 3 + (60 * A.BytesPerPixel);

                for (int i = 0; i < nHeight; i++)
                {
                    Array.Copy(A.Data, sourceStart, data, destForAStart, A.Width * A.BytesPerPixel);
                    Array.Copy(B.Data, sourceStart, data, destForBStart, B.Width * A.BytesPerPixel);
                    Array.Copy(C.Data, sourceStart, data, destForCStart, C.Width * A.BytesPerPixel);
                    Array.Copy(D.Data, sourceStart, data, destForDStart, D.Width * A.BytesPerPixel);
                    sourceStart += A.Width * A.BytesPerPixel;
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                    destForDStart += nWidth * A.BytesPerPixel;
                }


                //Array.Copy(A.Data, sourceStart, data, destForAStart, A.Data.Length);
                //Array.Copy(B.Data, sourceStart, data, destForBStart, A.Data.Length);

                return new ByteArrayBitmap(data, nWidth, nHeight, A.Format);
            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
            }
            return null;
        }


        public static ByteArrayBitmap PutImages(ByteArrayBitmap A, ByteArrayBitmap B, ByteArrayBitmap C, ByteArrayBitmap A2, ByteArrayBitmap B2, ByteArrayBitmap C2)
        {
            try
            {
                if (A.Height != B.Height || B.Height != C.Height)
                    throw new Exception("Image missmatch");

                if (A2.Height != B2.Height || B2.Height != C2.Height)
                    throw new Exception("Image missmatch");

                if (A.Height != B2.Height || B2.Height != C.Height)
                    throw new Exception("Image missmatch");

                if (A.Width != B.Width || B.Width != C.Width)
                    throw new Exception("Image missmatch");

                if (A2.Width != B2.Width || B2.Width != C2.Width)
                    throw new Exception("Image missmatch");

                if (A2.Width != B.Width || B2.Width != C.Width)
                    throw new Exception("Image missmatch");


                if (A.BytesPerPixel != B.BytesPerPixel)
                {
                    if (A.BytesPerPixel == 3 && B.BytesPerPixel == 4)
                    {
                        byte[] mash = new byte[(int)((A.Data.Length / 3) * 4)];
                        int count = 0;
                        int count2 = 0;
                        while (count < A.Data.Length)
                        {
                            Array.Copy(A.Data, count, mash, count2, 3);
                            count += 3;
                            count2 += 4;
                        }
                        A = new ByteArrayBitmap(mash, A.Width, A.Height, B.Format);
                    }
                    if (A.BytesPerPixel != B.BytesPerPixel)
                    {
                        throw new Exception("Image missmatch");
                    }
                }


                int nWidth = A.Width * 3 + 40;
                int nHeight = A.Height * 2 + 40;

                byte[] data = new byte[nWidth * nHeight * A.BytesPerPixel];
                int sourceStart = 0;
                int destForAStart = 0;
                //int destForBStart = A.Data.Length + (20 * A.Height*A.BytesPerPixel);
                int destForBStart = A.Width * A.BytesPerPixel + (20 * A.BytesPerPixel);
                int destForCStart = A.Width * A.BytesPerPixel * 2 + (40 * A.BytesPerPixel);

                for (int i = 0; i < A.Height; i++)
                {
                    Array.Copy(A.Data, sourceStart, data, destForAStart, A.Width * A.BytesPerPixel);
                    Array.Copy(B.Data, sourceStart, data, destForBStart, B.Width * A.BytesPerPixel);
                    Array.Copy(C.Data, sourceStart, data, destForCStart, C.Width * A.BytesPerPixel);
                    sourceStart += A.Width * A.BytesPerPixel;
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                }

                for (int i = 0; i < 40; i++)
                {
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;

                }
                sourceStart = 0;

                for (int i = 0; i < A.Height; i++)
                {
                    Array.Copy(A2.Data, sourceStart, data, destForAStart, A2.Width * A.BytesPerPixel);
                    Array.Copy(B2.Data, sourceStart, data, destForBStart, B2.Width * A.BytesPerPixel);
                    Array.Copy(C2.Data, sourceStart, data, destForCStart, C2.Width * A.BytesPerPixel);
                    sourceStart += A.Width * A.BytesPerPixel;
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                }



                return new ByteArrayBitmap(data, nWidth, nHeight, A.Format);
            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
            }
            return null;
        }

        public static ByteArrayBitmap PutImages(ByteArrayBitmap A, ByteArrayBitmap B, ByteArrayBitmap C, ByteArrayBitmap D, ByteArrayBitmap A2, ByteArrayBitmap B2, ByteArrayBitmap C2, ByteArrayBitmap D2)
        {
            try
            {
                if (A.Height != B.Height || B.Height != C.Height || B.Height != D.Height)
                    throw new Exception("Image missmatch");

                if (A2.Height != B2.Height || B2.Height != C2.Height)
                    throw new Exception("Image missmatch");

                if (A.Height != B2.Height || D2.Height != C.Height)
                    throw new Exception("Image missmatch");

                if (A.Width != B.Width || B.Width != C.Width || B.Width != D.Width)
                    throw new Exception("Image missmatch");

                if (A2.Width != B2.Width || B2.Width != C2.Width)
                    throw new Exception("Image missmatch");

                if (A2.Width != B.Width || D2.Width != C.Width)
                    throw new Exception("Image missmatch");


                if (A.BytesPerPixel != B.BytesPerPixel)
                {
                    if (A.BytesPerPixel == 3 && B.BytesPerPixel == 4)
                    {
                        byte[] mash = new byte[(int)((A.Data.Length / 3) * 4)];
                        int count = 0;
                        int count2 = 0;
                        while (count < A.Data.Length)
                        {
                            Array.Copy(A.Data, count, mash, count2, 3);
                            count += 3;
                            count2 += 4;
                        }
                        A = new ByteArrayBitmap(mash, A.Width, A.Height, B.Format);
                    }
                    if (A.BytesPerPixel != B.BytesPerPixel)
                    {
                        throw new Exception("Image missmatch");
                    }
                }


                int nWidth = A.Width * 4 + 60;
                int nHeight = A.Height * 2 + 40;

                byte[] data = new byte[nWidth * nHeight * A.BytesPerPixel];
                int sourceStart = 0;
                int destForAStart = 0;
                //int destForBStart = A.Data.Length + (20 * A.Height*A.BytesPerPixel);
                int destForBStart = A.Width * A.BytesPerPixel + (20 * A.BytesPerPixel);
                int destForCStart = A.Width * A.BytesPerPixel * 2 + (40 * A.BytesPerPixel);
                int destForDStart = A.Width * A.BytesPerPixel * 3 + (60 * A.BytesPerPixel);

                for (int i = 0; i < A.Height; i++)
                {
                    Array.Copy(A.Data, sourceStart, data, destForAStart, A.Width * A.BytesPerPixel);
                    Array.Copy(B.Data, sourceStart, data, destForBStart, B.Width * A.BytesPerPixel);
                    Array.Copy(C.Data, sourceStart, data, destForCStart, C.Width * A.BytesPerPixel);
                    Array.Copy(D.Data, sourceStart, data, destForDStart, D.Width * A.BytesPerPixel);
                    sourceStart += A.Width * A.BytesPerPixel;
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                    destForDStart += nWidth * A.BytesPerPixel;
                }

                for (int i = 0; i < 40; i++)
                {
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                    destForDStart += nWidth * A.BytesPerPixel;

                }
                sourceStart = 0;

                for (int i = 0; i < A.Height; i++)
                {
                    Array.Copy(A2.Data, sourceStart, data, destForAStart, A2.Width * A.BytesPerPixel);
                    Array.Copy(B2.Data, sourceStart, data, destForBStart, B2.Width * A.BytesPerPixel);
                    Array.Copy(C2.Data, sourceStart, data, destForCStart, C2.Width * A.BytesPerPixel);
                    Array.Copy(D2.Data, sourceStart, data, destForDStart, D2.Width * A.BytesPerPixel);
                    sourceStart += A.Width * A.BytesPerPixel;
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                    destForDStart += nWidth * A.BytesPerPixel;
                }



                return new ByteArrayBitmap(data, nWidth, nHeight, A.Format);
            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
            }
            return null;
        }

        public static ByteArrayBitmap PutImages(ByteArrayBitmap A, ByteArrayBitmap B, ByteArrayBitmap C, ByteArrayBitmap D, ByteArrayBitmap E, ByteArrayBitmap A2, ByteArrayBitmap B2, ByteArrayBitmap C2, ByteArrayBitmap D2, ByteArrayBitmap E2)
        {
            try
            {
                if (A.Height != B.Height || B.Height != C.Height || B.Height != D.Height)
                    throw new Exception("Image missmatch");

                if (A2.Height != B2.Height || B2.Height != C2.Height)
                    throw new Exception("Image missmatch");

                if (A.Height != B2.Height || D2.Height != C.Height)
                    throw new Exception("Image missmatch");

                if (A.Width != B.Width || B.Width != C.Width || B.Width != D.Width)
                    throw new Exception("Image missmatch");

                if (A2.Width != B2.Width || B2.Width != C2.Width)
                    throw new Exception("Image missmatch");

                if (A2.Width != B.Width || D2.Width != C.Width)
                    throw new Exception("Image missmatch");


                if (A.BytesPerPixel != B.BytesPerPixel)
                {
                    if (A.BytesPerPixel == 3 && B.BytesPerPixel == 4)
                    {
                        byte[] mash = new byte[(int)((A.Data.Length / 3) * 4)];
                        int count = 0;
                        int count2 = 0;
                        while (count < A.Data.Length)
                        {
                            Array.Copy(A.Data, count, mash, count2, 3);
                            count += 3;
                            count2 += 4;
                        }
                        A = new ByteArrayBitmap(mash, A.Width, A.Height, B.Format);
                    }
                    if (A.BytesPerPixel != B.BytesPerPixel)
                    {
                        throw new Exception("Image missmatch");
                    }
                }


                int nWidth = A.Width * 5 + 80;
                int nHeight = A.Height * 2 + 40;

                byte[] data = new byte[nWidth * nHeight * A.BytesPerPixel];
                int sourceStart = 0;
                int destForAStart = 0;
                //int destForBStart = A.Data.Length + (20 * A.Height*A.BytesPerPixel);
                int destForBStart = A.Width * A.BytesPerPixel + (20 * A.BytesPerPixel);
                int destForCStart = A.Width * A.BytesPerPixel * 2 + (40 * A.BytesPerPixel);
                int destForDStart = A.Width * A.BytesPerPixel * 3 + (60 * A.BytesPerPixel);
                int destForEStart = A.Width * A.BytesPerPixel * 4 + (80 * A.BytesPerPixel);

                for (int i = 0; i < A.Height; i++)
                {
                    Array.Copy(A.Data, sourceStart, data, destForAStart, A.Width * A.BytesPerPixel);
                    Array.Copy(B.Data, sourceStart, data, destForBStart, B.Width * A.BytesPerPixel);
                    Array.Copy(C.Data, sourceStart, data, destForCStart, C.Width * A.BytesPerPixel);
                    Array.Copy(D.Data, sourceStart, data, destForDStart, D.Width * A.BytesPerPixel);
                    Array.Copy(E.Data, sourceStart, data, destForEStart, E.Width * A.BytesPerPixel);
                    sourceStart += A.Width * A.BytesPerPixel;
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                    destForDStart += nWidth * A.BytesPerPixel;
                    destForEStart += nWidth * A.BytesPerPixel;
                }

                for (int i = 0; i < 40; i++)
                {
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                    destForDStart += nWidth * A.BytesPerPixel;
                    destForEStart += nWidth * A.BytesPerPixel;

                }
                sourceStart = 0;

                for (int i = 0; i < A.Height; i++)
                {
                    Array.Copy(A2.Data, sourceStart, data, destForAStart, A2.Width * A.BytesPerPixel);
                    Array.Copy(B2.Data, sourceStart, data, destForBStart, B2.Width * A.BytesPerPixel);
                    Array.Copy(C2.Data, sourceStart, data, destForCStart, C2.Width * A.BytesPerPixel);
                    Array.Copy(D2.Data, sourceStart, data, destForDStart, D2.Width * A.BytesPerPixel);
                    Array.Copy(E2.Data, sourceStart, data, destForEStart, E2.Width * A.BytesPerPixel);
                    sourceStart += A.Width * A.BytesPerPixel;
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                    destForDStart += nWidth * A.BytesPerPixel;
                    destForEStart += nWidth * A.BytesPerPixel;
                }



                return new ByteArrayBitmap(data, nWidth, nHeight, A.Format);
            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
            }
            return null;
        }

        public static ByteArrayBitmap PutImages(ByteArrayBitmap A, ByteArrayBitmap B, ByteArrayBitmap C, ByteArrayBitmap D, ByteArrayBitmap A2, ByteArrayBitmap B2, ByteArrayBitmap C2, ByteArrayBitmap D2, ByteArrayBitmap A3, ByteArrayBitmap B3, ByteArrayBitmap C3, ByteArrayBitmap D3)
        {
            try
            {
                if (A.Height != B.Height || B.Height != C.Height || B.Height != D.Height)
                    throw new Exception("Image missmatch");

                if (A2.Height != B2.Height || B2.Height != C2.Height)
                    throw new Exception("Image missmatch");

                if (A.Height != B2.Height || D2.Height != C.Height)
                    throw new Exception("Image missmatch");

                if (A.Width != B.Width || B.Width != C.Width || B.Width != D.Width)
                    throw new Exception("Image missmatch");

                if (A2.Width != B2.Width || B2.Width != C2.Width)
                    throw new Exception("Image missmatch");

                if (A2.Width != B.Width || D2.Width != C.Width)
                    throw new Exception("Image missmatch");


                if (A.BytesPerPixel != B.BytesPerPixel)
                {
                    byte[] mash;
                    int count = 0;
                    int count2 = 0;
                    if (A.BytesPerPixel == 3 && B.BytesPerPixel == 4)
                    {
                        mash = new byte[(int)((A.Data.Length / 3) * 4)];

                        while (count < A.Data.Length)
                        {
                            Array.Copy(A.Data, count, mash, count2, 3);
                            count += 3;
                            count2 += 4;
                        }
                        A = new ByteArrayBitmap(mash, A.Width, A.Height, B.Format);
                    }
                    if (B.BytesPerPixel == 3 && A.BytesPerPixel == 4)
                    {
                        mash = new byte[(int)((B.Data.Length / 3) * 4)];
                        while (count < B.Data.Length)
                        {
                            Array.Copy(B.Data, count, mash, count2, 3);
                            count += 3;
                            count2 += 4;
                        }
                        B = new ByteArrayBitmap(mash, B.Width, B.Height, A.Format);
                    }

                    if (A.BytesPerPixel != B.BytesPerPixel)
                    {
                        throw new Exception("Image missmatch");
                    }
                }


                int nWidth = A.Width * 4 + 60;
                int nHeight = A.Height * 3 + 40;

                byte[] data = new byte[nWidth * nHeight * A.BytesPerPixel];
                int sourceStart = 0;
                int destForAStart = 0;
                //int destForBStart = A.Data.Length + (20 * A.Height*A.BytesPerPixel);
                int destForBStart = A.Width * A.BytesPerPixel + (20 * A.BytesPerPixel);
                int destForCStart = A.Width * A.BytesPerPixel * 2 + (40 * A.BytesPerPixel);
                int destForDStart = A.Width * A.BytesPerPixel * 3 + (60 * A.BytesPerPixel);

                for (int i = 0; i < A.Height; i++)
                {
                    Array.Copy(A.Data, sourceStart, data, destForAStart, A.Width * A.BytesPerPixel);
                    Array.Copy(B.Data, sourceStart, data, destForBStart, B.Width * A.BytesPerPixel);
                    Array.Copy(C.Data, sourceStart, data, destForCStart, C.Width * A.BytesPerPixel);
                    Array.Copy(D.Data, sourceStart, data, destForDStart, D.Width * A.BytesPerPixel);
                    sourceStart += A.Width * A.BytesPerPixel;
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                    destForDStart += nWidth * A.BytesPerPixel;
                }

                for (int i = 0; i < 20; i++)
                {
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                    destForDStart += nWidth * A.BytesPerPixel;

                }
                sourceStart = 0;

                for (int i = 0; i < A.Height; i++)
                {
                    Array.Copy(A2.Data, sourceStart, data, destForAStart, A2.Width * A.BytesPerPixel);
                    Array.Copy(B2.Data, sourceStart, data, destForBStart, B2.Width * A.BytesPerPixel);
                    Array.Copy(C2.Data, sourceStart, data, destForCStart, C2.Width * A.BytesPerPixel);
                    Array.Copy(D2.Data, sourceStart, data, destForDStart, D2.Width * A.BytesPerPixel);
                    sourceStart += A.Width * A.BytesPerPixel;
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                    destForDStart += nWidth * A.BytesPerPixel;
                }

                for (int i = 0; i < 20; i++)
                {
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                    destForDStart += nWidth * A.BytesPerPixel;

                }
                sourceStart = 0;

                for (int i = 0; i < A.Height; i++)
                {
                    Array.Copy(A3.Data, sourceStart, data, destForAStart, A3.Width * A.BytesPerPixel);
                    Array.Copy(B3.Data, sourceStart, data, destForBStart, B3.Width * A.BytesPerPixel);
                    Array.Copy(C3.Data, sourceStart, data, destForCStart, C3.Width * A.BytesPerPixel);
                    Array.Copy(D3.Data, sourceStart, data, destForDStart, D3.Width * A.BytesPerPixel);
                    sourceStart += A.Width * A.BytesPerPixel;
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                    destForDStart += nWidth * A.BytesPerPixel;
                }



                return new ByteArrayBitmap(data, nWidth, nHeight, A.Format);
            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
            }
            return null;
        }

        public static ByteArrayBitmap PutImages(ByteArrayBitmap A, ByteArrayBitmap B, ByteArrayBitmap C, ByteArrayBitmap D, ByteArrayBitmap E, ByteArrayBitmap F, ByteArrayBitmap G, ByteArrayBitmap A2, ByteArrayBitmap B2, ByteArrayBitmap C2, ByteArrayBitmap D2, ByteArrayBitmap E2, ByteArrayBitmap F2, ByteArrayBitmap G2, ByteArrayBitmap A3, ByteArrayBitmap B3, ByteArrayBitmap C3, ByteArrayBitmap D3, ByteArrayBitmap E3, ByteArrayBitmap F3, ByteArrayBitmap G3)
        {
            try
            {
                if (A.Height != B.Height || B.Height != C.Height || B.Height != D.Height)
                    throw new Exception("Image missmatch");

                if (A2.Height != B2.Height || B2.Height != C2.Height)
                    throw new Exception("Image missmatch");

                if (A.Height != B2.Height || D2.Height != C.Height)
                    throw new Exception("Image missmatch");

                if (A.Width != B.Width || B.Width != C.Width || B.Width != D.Width)
                    throw new Exception("Image missmatch");

                if (A2.Width != B2.Width || B2.Width != C2.Width)
                    throw new Exception("Image missmatch");

                if (A2.Width != B.Width || D2.Width != C.Width)
                    throw new Exception("Image missmatch");


                if (A.BytesPerPixel != B.BytesPerPixel)
                {
                    byte[] mash;
                    int count = 0;
                    int count2 = 0;
                    if (A.BytesPerPixel == 3 && B.BytesPerPixel == 4)
                    {
                        mash = new byte[(int)((A.Data.Length / 3) * 4)];

                        while (count < A.Data.Length)
                        {
                            Array.Copy(A.Data, count, mash, count2, 3);
                            count += 3;
                            count2 += 4;
                        }
                        A = new ByteArrayBitmap(mash, A.Width, A.Height, B.Format);
                    }
                    if (B.BytesPerPixel == 3 && A.BytesPerPixel == 4)
                    {
                        mash = new byte[(int)((B.Data.Length / 3) * 4)];
                        while (count < B.Data.Length)
                        {
                            Array.Copy(B.Data, count, mash, count2, 3);
                            count += 3;
                            count2 += 4;
                        }
                        B = new ByteArrayBitmap(mash, B.Width, B.Height, A.Format);
                    }

                    if (A.BytesPerPixel != B.BytesPerPixel)
                    {
                        throw new Exception("Image missmatch");
                    }
                }


                int nWidth = A.Width * 7 + 120;
                int nHeight = A.Height * 3 + 40;

                byte[] data = new byte[nWidth * nHeight * A.BytesPerPixel];
                int sourceStart = 0;
                int destForAStart = 0;
                //int destForBStart = A.Data.Length + (20 * A.Height*A.BytesPerPixel);
                int destForBStart = A.Width * A.BytesPerPixel + (20 * A.BytesPerPixel);
                int destForCStart = A.Width * A.BytesPerPixel * 2 + (40 * A.BytesPerPixel);
                int destForDStart = A.Width * A.BytesPerPixel * 3 + (60 * A.BytesPerPixel);
                int destForEStart = A.Width * A.BytesPerPixel * 4 + (80 * A.BytesPerPixel);
                int destForFStart = A.Width * A.BytesPerPixel * 5 + (100 * A.BytesPerPixel);
                int destForGStart = A.Width * A.BytesPerPixel * 6 + (120 * A.BytesPerPixel);

                for (int i = 0; i < A.Height; i++)
                {
                    Array.Copy(A.Data, sourceStart, data, destForAStart, A.Width * A.BytesPerPixel);
                    Array.Copy(B.Data, sourceStart, data, destForBStart, B.Width * A.BytesPerPixel);
                    Array.Copy(C.Data, sourceStart, data, destForCStart, C.Width * A.BytesPerPixel);
                    Array.Copy(D.Data, sourceStart, data, destForDStart, D.Width * A.BytesPerPixel);
                    Array.Copy(E.Data, sourceStart, data, destForEStart, E.Width * A.BytesPerPixel);
                    Array.Copy(F.Data, sourceStart, data, destForFStart, F.Width * A.BytesPerPixel);
                    Array.Copy(G.Data, sourceStart, data, destForGStart, G.Width * A.BytesPerPixel);
                    sourceStart += A.Width * A.BytesPerPixel;
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                    destForDStart += nWidth * A.BytesPerPixel;
                    destForEStart += nWidth * A.BytesPerPixel;
                    destForFStart += nWidth * A.BytesPerPixel;
                    destForGStart += nWidth * A.BytesPerPixel;
                }

                for (int i = 0; i < 20; i++)
                {
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                    destForDStart += nWidth * A.BytesPerPixel;
                    destForEStart += nWidth * A.BytesPerPixel;
                    destForFStart += nWidth * A.BytesPerPixel;
                    destForGStart += nWidth * A.BytesPerPixel;

                }
                sourceStart = 0;

                for (int i = 0; i < A.Height; i++)
                {
                    Array.Copy(A2.Data, sourceStart, data, destForAStart, A2.Width * A.BytesPerPixel);
                    Array.Copy(B2.Data, sourceStart, data, destForBStart, B2.Width * A.BytesPerPixel);
                    Array.Copy(C2.Data, sourceStart, data, destForCStart, C2.Width * A.BytesPerPixel);
                    Array.Copy(D2.Data, sourceStart, data, destForDStart, D2.Width * A.BytesPerPixel);
                    Array.Copy(E2.Data, sourceStart, data, destForEStart, E2.Width * A.BytesPerPixel);
                    Array.Copy(F2.Data, sourceStart, data, destForFStart, F2.Width * A.BytesPerPixel);
                    Array.Copy(G2.Data, sourceStart, data, destForGStart, G2.Width * A.BytesPerPixel);
                    sourceStart += A.Width * A.BytesPerPixel;
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                    destForDStart += nWidth * A.BytesPerPixel;
                    destForEStart += nWidth * A.BytesPerPixel;
                    destForFStart += nWidth * A.BytesPerPixel;
                    destForGStart += nWidth * A.BytesPerPixel;
                }

                for (int i = 0; i < 20; i++)
                {
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                    destForDStart += nWidth * A.BytesPerPixel;
                    destForEStart += nWidth * A.BytesPerPixel;
                    destForFStart += nWidth * A.BytesPerPixel;
                    destForGStart += nWidth * A.BytesPerPixel;
                }
                sourceStart = 0;

                for (int i = 0; i < A.Height; i++)
                {
                    Array.Copy(A3.Data, sourceStart, data, destForAStart, A3.Width * A.BytesPerPixel);
                    Array.Copy(B3.Data, sourceStart, data, destForBStart, B3.Width * A.BytesPerPixel);
                    Array.Copy(C3.Data, sourceStart, data, destForCStart, C3.Width * A.BytesPerPixel);
                    Array.Copy(D3.Data, sourceStart, data, destForDStart, D3.Width * A.BytesPerPixel);
                    Array.Copy(E3.Data, sourceStart, data, destForEStart, E3.Width * A.BytesPerPixel);
                    Array.Copy(F3.Data, sourceStart, data, destForFStart, F3.Width * A.BytesPerPixel);
                    Array.Copy(G3.Data, sourceStart, data, destForGStart, G3.Width * A.BytesPerPixel);
                    sourceStart += A.Width * A.BytesPerPixel;
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                    destForDStart += nWidth * A.BytesPerPixel;
                    destForEStart += nWidth * A.BytesPerPixel;
                    destForFStart += nWidth * A.BytesPerPixel;
                    destForGStart += nWidth * A.BytesPerPixel;
                }



                return new ByteArrayBitmap(data, nWidth, nHeight, A.Format);
            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
            }
            return null;
        }

        public static ByteArrayBitmap PutImages(ByteArrayBitmap A, ByteArrayBitmap B, ByteArrayBitmap C, ByteArrayBitmap D, ByteArrayBitmap E, ByteArrayBitmap F, ByteArrayBitmap G, ByteArrayBitmap A2, ByteArrayBitmap B2, ByteArrayBitmap C2, ByteArrayBitmap D2, ByteArrayBitmap E2, ByteArrayBitmap F2, ByteArrayBitmap G2, ByteArrayBitmap A3, ByteArrayBitmap B3, ByteArrayBitmap C3, ByteArrayBitmap D3, ByteArrayBitmap E3, ByteArrayBitmap F3, ByteArrayBitmap G3, ByteArrayBitmap A4, ByteArrayBitmap B4, ByteArrayBitmap C4, ByteArrayBitmap D4, ByteArrayBitmap E4, ByteArrayBitmap F4, ByteArrayBitmap G4)
        {
            try
            {
                if (A.Height != B.Height || B.Height != C.Height || B.Height != D.Height)
                    throw new Exception("Image missmatch");

                if (A2.Height != B2.Height || B2.Height != C2.Height)
                    throw new Exception("Image missmatch");

                if (A.Height != B2.Height || D2.Height != C.Height)
                    throw new Exception("Image missmatch");

                if (A.Width != B.Width || B.Width != C.Width || B.Width != D.Width)
                    throw new Exception("Image missmatch");

                if (A2.Width != B2.Width || B2.Width != C2.Width)
                    throw new Exception("Image missmatch");

                if (A2.Width != B.Width || D2.Width != C.Width)
                    throw new Exception("Image missmatch");


                if (A.BytesPerPixel != B.BytesPerPixel)
                {
                    byte[] mash;
                    int count = 0;
                    int count2 = 0;
                    if (A.BytesPerPixel == 3 && B.BytesPerPixel == 4)
                    {
                        mash = new byte[(int)((A.Data.Length / 3) * 4)];

                        while (count < A.Data.Length)
                        {
                            Array.Copy(A.Data, count, mash, count2, 3);
                            count += 3;
                            count2 += 4;
                        }
                        A = new ByteArrayBitmap(mash, A.Width, A.Height, B.Format);
                    }
                    if (B.BytesPerPixel == 3 && A.BytesPerPixel == 4)
                    {
                        mash = new byte[(int)((B.Data.Length / 3) * 4)];
                        while (count < B.Data.Length)
                        {
                            Array.Copy(B.Data, count, mash, count2, 3);
                            count += 3;
                            count2 += 4;
                        }
                        B = new ByteArrayBitmap(mash, B.Width, B.Height, A.Format);
                    }

                    if (A.BytesPerPixel != B.BytesPerPixel)
                    {
                        throw new Exception("Image missmatch");
                    }
                }


                int nWidth = A.Width * 7 + 120;
                int nHeight = A.Height * 4 + 60;

                byte[] data = new byte[nWidth * nHeight * A.BytesPerPixel];
                int sourceStart = 0;
                int destForAStart = 0;
                //int destForBStart = A.Data.Length + (20 * A.Height*A.BytesPerPixel);
                int destForBStart = A.Width * A.BytesPerPixel + (20 * A.BytesPerPixel);
                int destForCStart = A.Width * A.BytesPerPixel * 2 + (40 * A.BytesPerPixel);
                int destForDStart = A.Width * A.BytesPerPixel * 3 + (60 * A.BytesPerPixel);
                int destForEStart = A.Width * A.BytesPerPixel * 4 + (80 * A.BytesPerPixel);
                int destForFStart = A.Width * A.BytesPerPixel * 5 + (100 * A.BytesPerPixel);
                int destForGStart = A.Width * A.BytesPerPixel * 6 + (120 * A.BytesPerPixel);

                for (int i = 0; i < A.Height; i++)
                {
                    Array.Copy(A.Data, sourceStart, data, destForAStart, A.Width * A.BytesPerPixel);
                    Array.Copy(B.Data, sourceStart, data, destForBStart, B.Width * A.BytesPerPixel);
                    Array.Copy(C.Data, sourceStart, data, destForCStart, C.Width * A.BytesPerPixel);
                    Array.Copy(D.Data, sourceStart, data, destForDStart, D.Width * A.BytesPerPixel);
                    Array.Copy(E.Data, sourceStart, data, destForEStart, E.Width * A.BytesPerPixel);
                    Array.Copy(F.Data, sourceStart, data, destForFStart, F.Width * A.BytesPerPixel);
                    Array.Copy(G.Data, sourceStart, data, destForGStart, G.Width * A.BytesPerPixel);
                    sourceStart += A.Width * A.BytesPerPixel;
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                    destForDStart += nWidth * A.BytesPerPixel;
                    destForEStart += nWidth * A.BytesPerPixel;
                    destForFStart += nWidth * A.BytesPerPixel;
                    destForGStart += nWidth * A.BytesPerPixel;
                }

                for (int i = 0; i < 20; i++)
                {
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                    destForDStart += nWidth * A.BytesPerPixel;
                    destForEStart += nWidth * A.BytesPerPixel;
                    destForFStart += nWidth * A.BytesPerPixel;
                    destForGStart += nWidth * A.BytesPerPixel;

                }
                sourceStart = 0;

                for (int i = 0; i < A.Height; i++)
                {
                    Array.Copy(A2.Data, sourceStart, data, destForAStart, A2.Width * A.BytesPerPixel);
                    Array.Copy(B2.Data, sourceStart, data, destForBStart, B2.Width * A.BytesPerPixel);
                    Array.Copy(C2.Data, sourceStart, data, destForCStart, C2.Width * A.BytesPerPixel);
                    Array.Copy(D2.Data, sourceStart, data, destForDStart, D2.Width * A.BytesPerPixel);
                    Array.Copy(E2.Data, sourceStart, data, destForEStart, E2.Width * A.BytesPerPixel);
                    Array.Copy(F2.Data, sourceStart, data, destForFStart, F2.Width * A.BytesPerPixel);
                    Array.Copy(G2.Data, sourceStart, data, destForGStart, G2.Width * A.BytesPerPixel);
                    sourceStart += A.Width * A.BytesPerPixel;
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                    destForDStart += nWidth * A.BytesPerPixel;
                    destForEStart += nWidth * A.BytesPerPixel;
                    destForFStart += nWidth * A.BytesPerPixel;
                    destForGStart += nWidth * A.BytesPerPixel;
                }

                for (int i = 0; i < 20; i++)
                {
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                    destForDStart += nWidth * A.BytesPerPixel;
                    destForEStart += nWidth * A.BytesPerPixel;
                    destForFStart += nWidth * A.BytesPerPixel;
                    destForGStart += nWidth * A.BytesPerPixel;
                }
                sourceStart = 0;

                for (int i = 0; i < A.Height; i++)
                {
                    Array.Copy(A3.Data, sourceStart, data, destForAStart, A3.Width * A.BytesPerPixel);
                    Array.Copy(B3.Data, sourceStart, data, destForBStart, B3.Width * A.BytesPerPixel);
                    Array.Copy(C3.Data, sourceStart, data, destForCStart, C3.Width * A.BytesPerPixel);
                    Array.Copy(D3.Data, sourceStart, data, destForDStart, D3.Width * A.BytesPerPixel);
                    Array.Copy(E3.Data, sourceStart, data, destForEStart, E3.Width * A.BytesPerPixel);
                    Array.Copy(F3.Data, sourceStart, data, destForFStart, F3.Width * A.BytesPerPixel);
                    Array.Copy(G3.Data, sourceStart, data, destForGStart, G3.Width * A.BytesPerPixel);
                    sourceStart += A.Width * A.BytesPerPixel;
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                    destForDStart += nWidth * A.BytesPerPixel;
                    destForEStart += nWidth * A.BytesPerPixel;
                    destForFStart += nWidth * A.BytesPerPixel;
                    destForGStart += nWidth * A.BytesPerPixel;
                }
                for (int i = 0; i < 20; i++)
                {
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                    destForDStart += nWidth * A.BytesPerPixel;
                    destForEStart += nWidth * A.BytesPerPixel;
                    destForFStart += nWidth * A.BytesPerPixel;
                    destForGStart += nWidth * A.BytesPerPixel;
                }
                sourceStart = 0;

                for (int i = 0; i < A.Height; i++)
                {
                    Array.Copy(A4.Data, sourceStart, data, destForAStart, A4.Width * A.BytesPerPixel);
                    Array.Copy(B4.Data, sourceStart, data, destForBStart, B4.Width * A.BytesPerPixel);
                    Array.Copy(C4.Data, sourceStart, data, destForCStart, C4.Width * A.BytesPerPixel);
                    Array.Copy(D4.Data, sourceStart, data, destForDStart, D4.Width * A.BytesPerPixel);
                    Array.Copy(E4.Data, sourceStart, data, destForEStart, E4.Width * A.BytesPerPixel);
                    Array.Copy(F4.Data, sourceStart, data, destForFStart, F4.Width * A.BytesPerPixel);
                    Array.Copy(G4.Data, sourceStart, data, destForGStart, G4.Width * A.BytesPerPixel);
                    sourceStart += A.Width * A.BytesPerPixel;
                    destForAStart += nWidth * A.BytesPerPixel;
                    destForBStart += nWidth * A.BytesPerPixel;
                    destForCStart += nWidth * A.BytesPerPixel;
                    destForDStart += nWidth * A.BytesPerPixel;
                    destForEStart += nWidth * A.BytesPerPixel;
                    destForFStart += nWidth * A.BytesPerPixel;
                    destForGStart += nWidth * A.BytesPerPixel;
                }

                return new ByteArrayBitmap(data, nWidth, nHeight, A.Format);
            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
            }
            return null;
        }

    }
}
