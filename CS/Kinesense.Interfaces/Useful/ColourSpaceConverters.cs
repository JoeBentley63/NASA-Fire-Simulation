using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Useful
{
    // These are all to be considerd slow and should not be used in the algorithm chain
    //
    // If you need one, tell me (Daniel) and I will write a faster version, or write 
    // your own faster version
    //


    public static class ColourSpaceConverters
    {
        /// <summary>
        /// byte[3] rgb to double[3] hsv
        /// </summary>
        /// <param name="rgb"></param>
        /// <returns></returns>
        public static double[] RGBtoHSV(byte[] rgb)
        {

            try
            {
                double vr = (double)rgb[0] / 255;
                double vg = (double)rgb[1] / 255;
                double vb = (double)rgb[2] / 255;

                double max = Math.Max(Math.Max(vr, vg), vb);
                double min = Math.Min(Math.Min(vr, vg), vb);
                double delta = max - min;

                double[] result = new double[3] { 0, 0, max };

                if (delta == 0)
                {
                }
                else
                {
                    result[1] = delta / max;

                    if (vr == max)
                        result[0] = (vg - vb) / (delta) % 6;
                    else if (vg == max)
                        result[0] = 2 + (vb - vr) / delta;
                    else if (vb == max)
                        result[0] = 4 + (vr - vg) / delta;
                    result[0] = result[0] / (6);

                    if (result[0] < 0) result[0] += 1;
                    if (result[0] > 1) result[0] -= 1;

                }
                return result;
            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
                return null;
            }

        }
        /// <summary>
        /// byte[3]rgb to byte[3]hsv_NormalisedTo255
        /// </summary>
        /// <param name="rgb"></param>
        /// <returns></returns>
        public static byte[] RGBtoHSV_ResultNormalisedToByte(byte[] rgb)
        {
            double[] hsv = RGBtoHSV(rgb);

            //byte[] T = HSVtoRGB(hsv);

            return new byte[3] { (byte)(hsv[0] * 255), (byte)(hsv[1] * 255), (byte)(hsv[2] * 255) };
        }
        /// <summary>
        /// double[3]hsv to byte[3]rgb
        /// </summary>
        /// <param name="hsv"></param>
        /// <returns></returns>
        public static byte[] HSVtoRGB(double[] hsv)
        {

            try
            {
                byte[] rgb = new byte[3];

                if (hsv[1] == 0)                       //HSV from 0 to 1
                {
                    rgb[0] = (byte)(hsv[2] * 255);
                    rgb[1] = (byte)(hsv[2] * 255);
                    rgb[2] = (byte)(hsv[2] * 255);
                }
                else
                {
                    double vh = hsv[0] * 6;
                    if (vh == 6)
                        vh = 0;     //H must be < 1

                    int vi = (int)(vh);         //Or ... vi = floor( vh )
                    double v1 = hsv[2] * (1 - hsv[1]);
                    double v2 = hsv[2] * (1 - hsv[1] * (vh - vi));
                    double v3 = hsv[2] * (1 - hsv[1] * (1 - (vh - vi)));
                    double vr, vg, vb;

                    if (vi == 0)
                    {
                        vr = hsv[2];
                        vg = v3;
                        vb = v1;
                    }
                    else if (vi == 1)
                    {
                        vr = v2;
                        vg = hsv[2];
                        vb = v1;
                    }
                    else if (vi == 2)
                    {
                        vr = v1;
                        vg = hsv[2];
                        vb = v3;
                    }
                    else if (vi == 3)
                    {
                        vr = v1;
                        vg = v2;
                        vb = hsv[2];
                    }
                    else if (vi == 4)
                    {
                        vr = v3;
                        vg = v1;
                        vb = hsv[2];
                    }
                    else { vr = hsv[2]; vg = v1; vb = v2; }

                    rgb[0] = (byte)(vr * 255);               //RGB results from 0 to 255
                    rgb[1] = (byte)(vg * 255);
                    rgb[2] = (byte)(vb * 255);
                }
                return rgb;
            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
                return null;
            }

        }
        /// <summary>
        /// byte[3]hsv to byte[3]rgb where source is normalised to 255
        /// </summary>
        /// <param name="hsv"></param>
        /// <returns></returns>
        public static byte[] HSVtoRGB_SourceNormalisedToByte(byte[] hsv)
        {
            double[] h = new double[3] { (double)hsv[0] / 255, (double)hsv[1] / 255, (double)hsv[2] / 255 };
            return HSVtoRGB(h);
        }


        /// <summary>
        /// converts an array in the format hsvhsvhsv to the format rgbrgbrgbrgb
        /// The source has been normalised to byte
        /// </summary>
        /// <param name="hsv"></param>
        /// <returns></returns>
        public static byte[] Sequence_HSVtoRGB_SourceNormalisedToByte(byte[] hsv)
        {

            try
            {
                byte[] rgb = new byte[hsv.Length];

                int count = 0;
                while (count < hsv.Length)
                {
                    byte[] res = HSVtoRGB_SourceNormalisedToByte(new byte[3] { hsv[count], hsv[count + 1], hsv[count + 2] });
                    Array.Copy(res, 0, rgb, count, 3);
                    count += 3;
                }

                return rgb;
            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
                return null;
            }

        }


        /// <summary>
        /// converts an array in the format hsvhsvhsv to the format rgbtrgbtrgbtrgbt
        /// The source has been normalised to byte
        /// </summary>
        /// <param name="hsv"></param>
        /// <returns></returns>
        public static byte[] Sequence_HSVtoRGBT_SourceNormalisedToByte(byte[] hsv)
        {
            try
            {
                int length = (int)((hsv.Length / 3) * 4);
                byte[] rgbt = new byte[length];

                int countIn = 0;
                int countOut = 0;
                while (countIn < hsv.Length)
                {
                    byte[] res = HSVtoRGB_SourceNormalisedToByte(new byte[3] { hsv[countIn], hsv[countIn + 1], hsv[countIn + 2] });
                    Array.Copy(res, 0, rgbt, countOut, 3);
                    countIn += 3;
                    countOut += 4;
                }
                return rgbt;
            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
                return null;
            }
        }
    }
}
