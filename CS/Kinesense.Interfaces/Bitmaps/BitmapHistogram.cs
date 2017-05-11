using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Kinesense.Interfaces.Classes
{
    public class BitmapHistogram
    {
        public BitmapHistogram(ByteArrayBitmap bitmap)
        {
            _Channels[0] = new BitmapHistogramChannel();
            _Channels[1] = new BitmapHistogramChannel();
            _Channels[2] = new BitmapHistogramChannel();
            //MapImage_RGB(bitmap);
            MapImage_HLS(bitmap);
        }

        public BitmapHistogram(ByteArrayBitmap bitmap, int x, int y, int width, int height)
        {
            _Channels[0] = new BitmapHistogramChannel();
            _Channels[1] = new BitmapHistogramChannel();
            _Channels[2] = new BitmapHistogramChannel();
            //MapImage_RGB(bitmap);
            MapImage_HLS(bitmap, x, y, width, height);
        }

        public void MapImage_HLS(ByteArrayBitmap bitmap, int x, int y, int width, int height)
        {
            //if bitmap is 4 byte per pixel, then ignore transparent pixels;

            bool ignoreTransparent = bitmap.BytesPerPixel == 4;

            double countsamples = 0;
            int bindiv = 256 / 8;
            for (int i = x; i < bitmap.Width && i < width; i++)
                for (int j = y; j < bitmap.Height && j < height; j++)
                {
                    byte[] pixel = bitmap.GetColor(i, j);
                    if (ignoreTransparent && pixel[3] != 255)
                    {
                        //ignore
                    }
                    else
                    {

                        byte[] hls = HLSImage.RGBToHLS(pixel);

                        countsamples++;

                        for (int c = 0; c < 3; c++)
                        {
                            int p = hls[c] / bindiv;
                            Channels[c].Bins[p]++;
                        }
                    }
                }

            for (int c = 0; c < 3; c++)

                for (int p = 0; p < Channels[c].NumberOfBins; p++)
                {
                    Channels[c].Bins[p] /= countsamples;
                }
        }

        public void MapImage_HLS(ByteArrayBitmap bitmap)
        {
            //if bitmap is 4 byte per pixel, then ignore transparent pixels;

            bool ignoreTransparent = bitmap.BytesPerPixel == 4;

            double countsamples = 0;
            int bindiv = 256 / 8;
            for (int i = 0; i < bitmap.Width; i++)
                for (int j = 0; j < bitmap.Height; j++)
                {
                    byte[] pixel = bitmap.GetColor(i, j);
                    if (ignoreTransparent && pixel[3] != 255)
                    {
                        //ignore
                    }
                    else
                    {

                        byte[] hls = HLSImage.RGBToHLS(pixel);

                        countsamples++;

                        for (int c = 0; c < 3; c++)
                        {
                            int p = hls[c] / bindiv;
                            Channels[c].Bins[p]++;
                        }
                    }
                }

            for (int c = 0; c < 3; c++)

                for (int p = 0; p < Channels[c].NumberOfBins; p++)
            {
                Channels[c].Bins[p]/= countsamples;
            }
        }

        private void MapImage_RGB(ByteArrayBitmap bitmap)
        {
            int bindiv = 256 / 8;
            for(int i = 0; i < bitmap.Width; i++)
                for (int j = 0; j < bitmap.Height; j++)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        int p = bitmap.GetColorValue(i, j, c) / bindiv;
                        Channels[c].Bins[p]++;
                    }
                }
        }

        private BitmapHistogramChannel[] _Channels = new BitmapHistogramChannel[3];
        public BitmapHistogramChannel[] Channels { get { return _Channels; } }

        public static double[] ComputeDifference(BitmapHistogram a, BitmapHistogram b)
        {
            double[] res = new double[3];

            int numberChannels = a.Channels.Length;

            for (int i = 0; i < numberChannels; i++)
                for (int j = 0; j < a.Channels[i].NumberOfBins; j++)
                    res[i] += Math.Abs(a.Channels[i].Bins[j] - b.Channels[i].Bins[j]);

            return res;

        }
    }

    public class BitmapHistogramChannel
    {
        public int NumberOfBins { get { return 8; } }
        private double[] _bins = new double[8];
        public double[] Bins { get { return _bins; } }
    }
}
