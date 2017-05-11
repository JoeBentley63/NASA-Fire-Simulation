using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Classes
{
    public class HLSImage : IDisposable
    {
        public HLSImage(int width, int height, int channels)
        {
            _bytesPerPixel = channels;
            Width          = width;
            Height         = height;
            Stride         = width * channels;
            Data = ByteArrayBitmap.MakeBytes(Stride * Height);
                //new byte[Stride * Height];
        }

        public int Stride { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public byte[] Data { get; set; }

        private int _bytesPerPixel = 3;
        public int BytesPerPixel
        {
            get { return _bytesPerPixel;  }
            set { _bytesPerPixel = value; }
        }

        public double GlobalSaturation
        {
            get;
            set;
        }

        public double GlobalHue
        {
            get;
            set;
        }

        public double GlobalLightness
        {
            get;
            set;
        }

        public double GlobalSatLightnessDistance
        {
            get;
            set;
        }

        public static double CalculateSaturationLightnessDistance(double sat, double lit)
        {
            return Math.Sqrt((sat - 255) * (sat - 255) + (lit - 255) * (lit - 255));
        }

        public void GetHLSPixelData(int i, int j, byte[] color)
        {
            int pos = (j * this.Stride) + (i * _bytesPerPixel);
            color[0] = this.Data[pos + 0];
            color[1] = this.Data[pos + 1];
            color[2] = this.Data[pos + 2];
        }

        public byte[] GetHLSPixelData(int i, int j)
        {
            byte[] color = new byte[3];
            int pos = (j * this.Stride) + (i * _bytesPerPixel);
            color[0] = this.Data[pos + 0];
            color[1] = this.Data[pos + 1];
            color[2] = this.Data[pos + 2];
            return color;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RGB"></param>
        /// <returns>byte[]{H (0-255), L (0-255), S (0-255)}</returns>
        public static void RGBToHLS(double R, double G, double B, ref byte H, ref byte L, ref byte S)
        {
            //double R = RGB[0], G = RGB[1], B = RGB[2];
            double[] HVS = new double[3];
            double max = 0, min = 0;

            //get max color
            max = (G >= R) ?
                ((G >= B) ? G : B)
                : ((R >= B) ? R : B);

            HVS[1] = max;

            if (max != 0)
            {

                //normalise;
                B /= max;
                R /= max;
                G /= max;

                max = (G >= R) ?
                    ((G >= B) ? G : B)
                    : ((R >= B) ? R : B);

                min = (G <= R) ?
                    ((G <= B) ? G : B)
                    : ((R <= B) ? R : B);

                //saturation
                HVS[2] = max - min;

                if (HVS[2] != 0)
                {

                    //normalise to Sat of 1;
                    B = (B - min) / HVS[2];
                    R = (R - min) / HVS[2];
                    G = (G - min) / HVS[2];

                    max = (G >= R) ?
                       ((G >= B) ? G : B)
                       : ((R >= B) ? R : B);

                    min = (G <= R) ?
                        ((G <= B) ? G : B)
                        : ((R <= B) ? R : B);

                    //get hue
                    if (max == R)
                    {
                        HVS[0] = 0.0 + 43.0 * (G - B);
                    }
                    else if (max == G)
                    {
                        HVS[0] = 85.0 + 43.0 * (B - R);
                    }
                    else /* rgb_max == rgb.b */
                    {
                        HVS[0] = 171.0 + 43.0 * (R - G);
                    }

                    if (HVS[0] < 0) HVS[0] = 0;
                    if (HVS[0] > 255) HVS[0] = 255;
                }
            }
            H = (byte)HVS[0];
            L = (byte)HVS[1];
            S = (byte)(HVS[2] * 255.0);
            //normalise
            //return new byte[] { (byte)HVS[0], (byte)HVS[1], (byte)(HVS[2] * 255.0) };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RGB"></param>
        /// <returns>byte[]{H (0-255), L (0-255), S (0-255)}</returns>
        public static byte[] RGBToHLS(byte[] RGB)
        {
            double R = RGB[0], G = RGB[1], B = RGB[2];
            double[] HVS = new double[3];
            double max = 0, min = 0;

            //get max color
            max = (G >= R) ?
                ((G >= B) ? G : B)
                : ((R >= B) ? R : B);

            HVS[1] = max;

            if (max != 0)
            {

                //normalise;
                B /= max;
                R /= max;
                G /= max;

                max = (G >= R) ?
                    ((G >= B) ? G : B)
                    : ((R >= B) ? R : B);

                min = (G <= R) ?
                    ((G <= B) ? G : B)
                    : ((R <= B) ? R : B);

                //saturation
                HVS[2] = max - min;

                if (HVS[2] != 0)
                {

                    //normalise to Sat of 1;
                    B = (B - min) / HVS[2];
                    R = (R - min) / HVS[2];
                    G = (G - min) / HVS[2];

                    max = (G >= R) ?
                       ((G >= B) ? G : B)
                       : ((R >= B) ? R : B);

                    min = (G <= R) ?
                        ((G <= B) ? G : B)
                        : ((R <= B) ? R : B);

                    //get hue
                    if (max == R)
                    {
                        HVS[0] = 0.0 + 43.0 * (G - B);
                    }
                    else if (max == G)
                    {
                        HVS[0] = 85.0 + 43.0 * (B - R);
                    }
                    else /* rgb_max == rgb.b */
                    {
                        HVS[0] = 171.0 + 43.0 * (R - G);
                    }

                    if (HVS[0] < 0) HVS[0] = 0;
                    if (HVS[0] > 255) HVS[0] = 255;
                }
            }
            byte H = (byte)HVS[0];
            byte L = (byte)HVS[1];
            byte S = (byte)(HVS[2] * 255.0);
            //normalise
            return new byte[] { H, L, S };
        }

        public void Dispose()
        {
            if(this.Data != null)
                ByteArrayBitmap.DeAllocateBytes(this.Data);
            this.Data = null;
        }
    }
}
