using System;
using System.Collections.Generic;

namespace Kinesense.Interfaces
{
	public class MathsExtra
	{
        public static void RescaleToMaxWidthHeight(int maxWidth, int maxHeight, ref int width, ref int height)
        {
            double aspectratio = (double) width / (double) height;
            width = (width > maxWidth ? maxWidth : width);
            height = (int) (width / aspectratio);
            height = (height > maxHeight ? maxHeight : height);
            width = (int)(height * aspectratio);
        }

		public static int HLSMax = 255;
		public static int RGBMax = 255;

        public static DateTime TryToLocalTime(DateTime dt)
        {
            if (dt.Kind == DateTimeKind.Unspecified)
                return dt;
            else
                return dt.ToLocalTime();
        }

        public static DateTime TryToUniversalTime(DateTime dt)
        {
            if (dt.Kind == DateTimeKind.Unspecified)
                return dt;
            else
                return dt.ToUniversalTime();
        }

		/// <summary>
		/// ellipse is in form of rect x1, x2, w, h
		/// </summary>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		/// <param name="ellipse"></param>
		/// <returns></returns>
		public static bool IsPointInsideEllipse(double X, double Y, double[] ellipse)
		{
			double a = ellipse[2] / 2;
			double b = ellipse[3] / 2;

			double x_shift = X - ellipse[0] - a;
			double y_shift = Y - ellipse[1] - b;

			double test = ((x_shift * x_shift) / (a * a)) + ((y_shift * y_shift) / (b * b));

			return (test <= 1d);
		}

		public static byte Mean(IList<byte> list)
		{
			int sum = 0;
			foreach (byte b in list)
				sum += b;
			return (byte) (sum/list.Count);
		}

		public static double Mean(IList<double> list)
		{
			return Sum(list) / list.Count;
		}

		public static double Mean(IList<double> list, int lastfewentries)
		{
			return Sum(list, lastfewentries)/((list.Count > lastfewentries) ? lastfewentries : list.Count);
		}

        public static float Max(IList<float> list)
        {
            if (list.Count == 0)
                return 0;

            int L = 0;
            for (int i = 1; i < list.Count; i++)
                L = (list[i] > list[L]) ? i : L;
            return list[L];
        }

        public static float Min(IList<float> list)
        {
            if (list.Count == 0)
                return 0;

            int L = 0;
            for (int i = 1; i < list.Count; i++)
                L = (list[i] < list[L]) ? i : L;
            return list[L];
        }

		public static int Max(IList<int> list)
		{
			if (list.Count == 0)
				return 0;

			int L = 0;
			for (int i = 1; i < list.Count; i++)
				L = (list[i] > list[L]) ? i : L;
			return list[L];
		}

		public static int Min(IList<int> list)
		{
			if (list.Count == 0)
				return 0;

			int L = 0;
			for (int i = 1; i < list.Count; i++)
				L = (list[i] < list[L]) ? i : L;
			return list[L];
        }

        public static byte Max(IList<byte> list)
        {
            if (list.Count == 0)
                return 0;

            int L = 0;
            for (int i = 1; i < list.Count; i++)
                L = (list[i] > list[L]) ? i : L;
            return list[L];
        }

        public static byte Min(IList<byte> list)
        {
            if (list.Count == 0)
                return 0;

            int L = 0;
            for (int i = 1; i < list.Count; i++)
                L = (list[i] < list[L]) ? i : L;
            return list[L];
        }

        public static byte Max(IList<byte> list, int countTo)
        {
            countTo = (countTo < list.Count ? countTo : list.Count);
            if (countTo == 0)
                return 0;

            int L = 0;
            for (int i = 1; i < countTo; i++)
                L = (list[i] > list[L]) ? i : L;
            return list[L];
        }

        public static byte Min(IList<byte> list, int countTo)
        {
            countTo = (countTo < list.Count ? countTo : list.Count);
            if (countTo == 0)
                return 0;

            int L = 0;
            for (int i = 1; i < countTo; i++)
                L = (list[i] < list[L]) ? i : L;
            return list[L];
        }

        public static byte Average(IList<byte> list, int countTo)
        {
            countTo = (countTo < list.Count ? countTo : list.Count);
            if (countTo == 0)
                return 0;

            double sum = 0;
            for (int i = 0; i < countTo; i++)
                sum += list[i];
            return (byte)(sum / countTo);
        }

        public static byte Median(byte[] list, int countTo)
        {
            countTo = (countTo < list.Length ? countTo : list.Length);

            byte max = Max(list, countTo);
            byte min = Min(list, countTo);

            //populate the histogram
            int[] hist = new int[max + 1 - min];
            for (int i = 0; i < countTo; i++)
                hist[list[i] - min]++;

            int median = 0;
            for (int sum = 0; median < hist.Length && sum < countTo / 2; median++)
                sum += hist[median];

            return (byte)((median - 1) + min);
        }

        public static byte Median(byte[] list)
        {
            byte max = Max(list);
            byte min = Min(list);

            //populate the histogram
            int[] hist = new int[max + 1 - min];
            for (int i = 0; i < list.Length; i++)
                hist[list[i] - min]++;

            int median = 0;
            for (int sum = 0; median < hist.Length && sum < list.Length / 2; median++)
                sum += hist[median];

            return (byte)((median - 1) + min);
        }

		public static int Median(int[] list)
		{
			int max = Max(list);
			int min = MathsExtra.Min(list);

			//populate the histogram
			int[] hist = new int[max + 1 - min];
			for (int i = 0; i < list.Length; i++)
				hist[list[i] - min]++;

			int median = 0;
			for (int sum = 0; median < hist.Length && sum < list.Length / 2; median++)
				sum += hist[median];

			return (median - 1) + min;
		}

        /// <summary>
        /// calculate median with resolution of .01
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static float fMedian_100(float[] list)
        {
            //convert to int by multiplying by 100
            float maxf = Max(list);
            float minf = MathsExtra.Min(list);

            int[] ilist = new int[list.Length];
            for (int i = 0; i < list.Length; i++)
                ilist[i] = (int)((list[i] - minf) * 100);

            int imedian = MathsExtra.Median(ilist);

            return (((float) imedian + minf) / 100f);

            ////resolution of 0.01


            ////populate the histogram
            //float[] hist = new float[max + 1 - min];
            //for (int i = 0; i < list.Length; i++)
            //    hist[list[i] - min]++;

            //int median = 0;
            //for (int sum = 0; median < hist.Length && sum < list.Length / 2; median++)
            //    sum += hist[median];

            //return (median - 1) + min;
        }

        public static float HueToRGB(float n1, float n2, float hue)
		{
			if (hue < 0) hue += HLSMax;
			if (hue > HLSMax) hue -= HLSMax;

			/* return r,g, or b value from this tridrant */
			if (hue < (HLSMax / 6f))
				return (n1 + (((n2 - n1) * hue + (HLSMax / 12f)) / (HLSMax / 6f)));
			if (hue < (HLSMax / 2f))
				return n2;
			if (hue < ((HLSMax * 2f) / 3f))
				return (n1 + (((n2 - n1) * (((HLSMax * 2f) / 3f) - hue) + (HLSMax / 12f)) / (HLSMax / 6f)));
			else
				return n1;
		}


		public static byte[] HLS2RGB(byte hue, byte lum, byte sat)
		{
			byte[] C = new byte[3];
			float Magic1, Magic2;
			float R, G, B;

			if (sat == 0)
			{     /* achromatic case */
				C[0] = (byte)((lum * RGBMax) / HLSMax);
				C[1] = C[0];
				C[2] = C[0];

				/* Small fix */
				R = C[0];
				G = C[1];
				B = C[2];
			}
			else
			{       /* chromatic case */
				/* set up magic numbers */
				if (lum <= (HLSMax / 2))
					Magic2 = (lum * (HLSMax + sat) + (HLSMax / 2f)) / HLSMax;
				else
					Magic2 = lum + sat - ((lum * sat) + (HLSMax / 2)) / HLSMax;

				Magic1 = 2 * lum - Magic2;

				/* get RGB, change units from HLSMax to RGBMax */
				R = (HueToRGB(Magic1, Magic2, hue + (HLSMax / 3f))
				     * RGBMax + (HLSMax / 2f)) / HLSMax;
				G = (HueToRGB(Magic1, Magic2, hue)
				     * RGBMax + (HLSMax / 2f)) / HLSMax;
				B = (HueToRGB(Magic1, Magic2, hue - (HLSMax / 3))
				     * RGBMax + (HLSMax / 2f)) / HLSMax;
			}

			C[0] = (byte)R;
			C[1] = (byte)G;
			C[2] = (byte)B;
			return C;
		}

		public static byte[] HLS2RGB(byte[] hlsbytes)
		{
			byte hue = hlsbytes[0], lum = hlsbytes[1], sat = hlsbytes[2];
			byte[] C = new byte[3];
			float Magic1, Magic2;
			float R, G, B;

			if (sat == 0)
			{     /* achromatic case */
				C[0] = (byte)((lum * RGBMax) / HLSMax);
				C[1] = C[0];
				C[2] = C[0];

				/* Small fix */
				R = C[0];
				G = C[1];
				B = C[2];
			}
			else
			{       /* chromatic case */
				/* set up magic numbers */
				if (lum <= (HLSMax / 2))
					Magic2 = (lum * (HLSMax + sat) + (HLSMax / 2f)) / HLSMax;
				else
					Magic2 = lum + sat - ((lum * sat) + (HLSMax / 2f)) / HLSMax;

				Magic1 = 2 * lum - Magic2;

				/* get RGB, change units from HLSMax to RGBMax */
				R = (HueToRGB(Magic1, Magic2, hue + (HLSMax / 3))
				     * RGBMax + (HLSMax / 2f)) / HLSMax;
				G = (HueToRGB(Magic1, Magic2, hue)
				     * RGBMax + (HLSMax / 2f)) / HLSMax;
				B = (HueToRGB(Magic1, Magic2, hue - (HLSMax / 3))
				     * RGBMax + (HLSMax / 2f)) / HLSMax;
			}

			C[0] = (byte)R;
			C[1] = (byte)G;
			C[2] = (byte)B;
			return C;
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

			//normalise
			return new byte[] { (byte)HVS[0], (byte)HVS[1], (byte)(HVS[2] * 255.0) };
		}


		public static byte GetHue(double B, double G, double R)
		{
			double[] HVS = new double[3];
			double max = 0, min = 0;

			//get max color
			max = (G >= R) ?
			               	((G >= B) ? G : B)
			      	: ((R >= B) ? R : B);

			HVS[1] = max;

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

			return (byte)(HVS[0]);

		}

		public static double Sum(IList<double> list)
		{
			double sum = 0;
			for (int i = 0; i < list.Count - 1; i++)
			{
				sum += list[i];
			}

			return sum;
		}

		public static double Sum(IList<short> list)
		{
			double sum = 0;
			for (int i = 0; i < list.Count - 1; i++)
			{
				sum += list[i];
			}

			return sum;
		}

		public static double Sum(IList<double> list, int lastfewentries)
		{
			double sum = 0;
			int startat = ((list.Count > lastfewentries) ? list.Count - lastfewentries : 0);
			int length = list.Count - startat;

			for (int i = startat; i < length; i++)
				sum += list[i];

			return sum;
		}

        public static double StandardDeviation(IList<double> list, double ignoreHigherThan)
        {
            double mean = MathsExtra.Mean(list);

            double sqsum = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if(list[i] < ignoreHigherThan)
                    sqsum += (list[i] - mean) * (list[i] - mean);
            }

            return Math.Sqrt(sqsum / (list.Count - 1));
        }

		public static double StandardDeviation(IList<double> list)
		{
			double mean = MathsExtra.Mean(list);

			double sqsum = 0;
            for (int i = 0; i < list.Count; i++)
            {
                sqsum += (list[i] - mean) * (list[i] - mean);
            }

			return Math.Sqrt(sqsum / (list.Count - 1));
		}

		/// <summary>
		/// replace any NAN or Infinity with 0;
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		public static double[] RemoveNANInfinity(double[] list)
		{
			double[] retlist = new double[list.Length];
			for (int i = 0; i < list.Length; i++)
			{
				if (double.IsInfinity(list[i])
				    || double.IsNaN(list[i]))
					retlist[i] = 0;
				else
					retlist[i] = list[i];
			}

			return retlist;
		}

		/// <summary>
		/// defined as more than 3 std from the mean. Replace with mean
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		public static double[] RemoveOutliers(double[] list)
		{
			double mean = MathsExtra.Mean(list);
			double stdDev = MathsExtra.StandardDeviation(list);

			double[] retlist = new double[list.Length];
			for (int i = 0; i < list.Length; i++)
			{
				retlist[i] = Math.Abs(list[i] - mean) > stdDev * 3 ? mean : list[i];
			}

			return retlist;
		}

		public static double[] Change(double[] list)
		{
			double[] retlist = new double[list.Length];
			for (int i = 0; i < list.Length - 1; i++)
				retlist[i] = list[i + 1] - list[i];

			return retlist;
		}

		public static double[] AbsChange(double[] list)
		{
			double[] retlist = new double[list.Length];
			for (int i = 0; i < list.Length - 1; i++)
				retlist[i] = Math.Abs(list[i + 1] - list[i]);

			return retlist;
		}

		public static int CountThresholdCrossings(IList<byte> list, byte thresh)
		{
			int count = 0;

			for (int i = 0; i < list.Count - 1; i++)
				if ((list[i + 1] >= thresh && list[i] < thresh)
					|| (list[i + 1] <= thresh && list[i] > thresh))
					count++;
			return count;

		}

		public static int CountThresholdCrossings(IList<byte> list, byte risingthresh, byte fallingthresh)
		{
			int count = 0;

			for (int i = 0; i < list.Count - 1; i++)
				if ((list[i + 1] >= fallingthresh && list[i] < fallingthresh)
				    || (list[i + 1] <= risingthresh && list[i] > risingthresh))
					count++;
			return count;

		}


		

		/// <summary>
		/// returns the least square fit of data[length, 2]
		/// </summary>
		/// <param name="data"></param>
		/// <returns>slope, b, r^2</returns>
		public static double[] FitLine(double[][] data)
		{
			double[,] nData = new double[data.Length, data[0].Length];
			for (int i = 0; i < data.Length; i++)
				for (int j = 0; j < data[0].Length; j++)
					nData[i, j] = data[i][j];

			return FitLine(nData);
		}

		/// <summary>
		/// returns the least square fit of data[length, 2]
		/// </summary>
		/// <param name="data"></param>
		/// <param name="useMaxLastEntries"></param>
		/// <returns>slope, b, r^2</returns>
		public static double[] FitLine(int[][] data, int useMaxLastEntries)
		{
			int length = (useMaxLastEntries < data.Length ? useMaxLastEntries : data.Length);
			int startpos = data.Length - length;
			double[,] nData = new double[length, data[0].Length];
			for (int i = startpos; i < length; i++)
				for (int j = 0; j < data[0].Length; j++)
					nData[i, j] = data[i][j];

			return FitLine(nData);
		}

		/// <summary>
		/// returns the least square fit of data[length, 2]
		/// </summary>
		/// <param name="data"></param>
		/// <param name="useMaxLastEntries"></param>
		/// <returns>slope, b, r^2</returns>
		public static double[] FitLine(short[][] data, int useMaxLastEntries)
		{
			int length = (useMaxLastEntries < data.Length ? useMaxLastEntries : data.Length);
			int startpos = data.Length - length;
			double[,] nData = new double[length, data[0].Length];
			for (int i = startpos; i < length; i++)
				for (int j = 0; j < data[0].Length; j++)
					nData[i, j] = data[i][j];

			return FitLine(nData);
		}

		/// <summary>
		/// returns the least square fit of data[length, 2]
		/// </summary>
		/// <param name="data"></param>
		/// <returns>slope, b, r^2</returns>
		public static double[] FitLine(double[,] data)
		{
			double[] avg = Mean(data);
			double SSxy = SSnm(data, avg, 0, 1);
			double SSxx = SSnm(data, avg, 0, 0);
			double SSyy = SSnm(data, avg, 1, 1);

			double slope = SSxy / SSxx;
			double b = avg[1] - slope * avg[0];
			double r_sqrd = (SSxy * SSxy) / (SSxx * SSyy);

			return new double[] { slope, b, r_sqrd };
		}



		/// <summary>
		/// mean of each column of data[ number of points, number of axes ]
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private static double[] Mean(double[,] data)
		{
			int len = data.GetUpperBound(0) + 1;
			int width = data.GetUpperBound(1) + 1;

			double[] avg = new double[width];

			for (int w = 0; w < width; w++)
			{
				for (int i = 0; i < len; i++)
					avg[w] += data[i, w];
				avg[w] /= len;
			}

			return avg;
		}

		/// <summary>
		/// computes the SS of the column of numbers in pos n of data array
		/// </summary>
		/// <param name="data"></param>
		/// <param name="avg"></param>
		/// <param name="n"></param>
		/// <param name="m"></param>
		/// <returns></returns>
		private static double SSnm(double[,] data, double[] avg, int n, int m)
		{
			double square = 0.0;
			int len = data.GetUpperBound(0) + 1;

			for (int i = 0; i < len; i++)
				square += data[i, n] * data[i, m];

			return square - len * avg[n] * avg[m];
		}

		public static int[] XYWH_to_XY1XY2(int[] xywhBounds)
		{
			return new int[] {xywhBounds[0], xywhBounds[1], xywhBounds[0] + xywhBounds[2], xywhBounds[1] + xywhBounds[3]};
		}

		public static int[] XY1XY2_to_XYWH(int[] xy1xy2Bounds)
		{
			return new int[] { xy1xy2Bounds[0], xy1xy2Bounds[1], xy1xy2Bounds[2] - xy1xy2Bounds[0], xy1xy2Bounds[3] - xy1xy2Bounds[1] };
		}

		/// <summary>
		/// rescales a position object from current max (eg. 100) to destination max (eg. Short.MaxValue)
		/// </summary>
		/// <param name="position">X1, Y1, X2, Y2</param>
		/// <param name="currentMax"></param>
		/// <param name="destinationMax"></param>
		/// <returns></returns>
		public static int[][] RescaleMany(int[][] position,
		                                  int currentMaxX, int currentMaxY,
		                                  int destinationMaxX, int destinationMaxY)
		{
			float fcMaxX = currentMaxX, fcMaxY = currentMaxY, fdMaxX = destinationMaxX, fdMaxY = destinationMaxY;
			int[][] np = new int[position.Length][];
			for (int i = 0; i < position.Length; i++)
				np[i] = new int[]{            
				                 	(int)Math.Round((float)position[i][0] * fdMaxX / fcMaxX),
				                 	(int)Math.Round((float)position[i][1] * fdMaxY / fcMaxY),
				                 	(int)Math.Round((float)position[i][2] * fdMaxX / fcMaxX),
				                 	(int)Math.Round((float)position[i][3] * fdMaxY / fcMaxY)};

			return np;
		}


		/// <summary>
		/// rescales a position object from current max (eg. 100) to destination max (eg. Short.MaxValue)
		/// </summary>
		/// <param name="position">X1, Y1, X2, Y2</param>
		/// <param name="currentMax"></param>
		/// <param name="destinationMax"></param>
		/// <returns></returns>
		public static int[] Rescale(int[] position, int currentMaxX, int currentMaxY, int destinationMaxX, int destinationMaxY)
		{
			float fcMaxX = currentMaxX, fcMaxY = currentMaxY, fdMaxX = destinationMaxX, fdMaxY = destinationMaxY;
			return new int[]{            
			                	(int)Math.Round((float)position[0] * fdMaxX / fcMaxX),
			                	(int)Math.Round((float)position[1] * fdMaxY / fcMaxY),
			                	(int)Math.Round((float)position[2] * fdMaxX / fcMaxX),
			                	(int)Math.Round((float)position[3] * fdMaxY / fcMaxY)};
		}

		/// <summary>
		/// rescales a position object from current max (eg. 100) to destination max (eg. Short.MaxValue)
		/// </summary>
		/// <param name="position">X1, Y1, X2, Y2</param>
		/// <param name="currentMax"></param>
		/// <param name="destinationMax"></param>
		/// <returns></returns>
		public static short[] Rescale(int[] position, int currentMaxX, int currentMaxY, short destinationMaxX, short destinationMaxY)
		{
			float fcMaxX = currentMaxX, fcMaxY = currentMaxY, fdMaxX = destinationMaxX, fdMaxY = destinationMaxY;
			return new short[]{            
			                	(short)Math.Round((float)position[0] * fdMaxX / fcMaxX),
			                	(short)Math.Round((float)position[1] * fdMaxY / fcMaxY),
			                	(short)Math.Round((float)position[2] * fdMaxX / fcMaxX),
			                	(short)Math.Round((float)position[3] * fdMaxY / fcMaxY)};
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="bounds">X1, Y1, X2, Y2</param>
		/// <returns>int[]{(x1 + x2)/2, (y1+y2)/2) </returns>
		public static int[] CalculateCenterPoint(int[] bounds)
		{
			return new int[] { (bounds[2] + bounds[0]) / 2, (bounds[1] + bounds[3]) / 2 };
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="bounds">X1, Y1, X2, Y2</param>
		/// <returns>int[]{(x1 + x2)/2, (y1+y2)/2) </returns>
		public static short[] CalculateCenterPoint(short[] bounds)
		{
			return new short[] { (short)(((int)bounds[2] + (int)bounds[0]) / 2), (short)(((int)bounds[3] + (int)bounds[1]) / 2) };
		}


		/// <summary>
		/// rescales a position object from current max (eg. 100) to destination max (eg. Short.MaxValue)
		/// </summary>
		/// <param name="position">X1, Y1, X2, Y2</param>
		/// <returns></returns>
		public static int[] Rescale(short[] position, int currentMaxX, int currentMaxY, int destinationMaxX, int destinationMaxY)
		{
			float fcMaxX = currentMaxX, fcMaxY = currentMaxY, fdMaxX = destinationMaxX, fdMaxY = destinationMaxY;
			return new int[]{            
			                	(int)Math.Round((float)position[0] * fdMaxX / fcMaxX),
			                	(int)Math.Round((float)position[1] * fdMaxY / fcMaxY),
			                	(int)Math.Round((float)position[2] * fdMaxX / fcMaxX),
			                	(int)Math.Round((float)position[3] * fdMaxY / fcMaxY)};
		}

		// <summary>
		/// encode ratio as x/y 0-1 => 0-127, y/x => 0-1 => 128-255
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static byte EncodeRatioAsByte(double x, double y)
		{
			return (byte)((y > x) ?
				(x / y) * 128d :
				255 - ((y / x) * 128d));
		}

		public static double DecodeByteToRatio(byte ratiobyte)
		{
			return (ratiobyte < 128) ? (double)ratiobyte / 128d : 128d / (255d - ratiobyte);
		}

		public static byte CalculateNoiseLevel(IList<byte> list, int ignoreStart, int ignoreEnd)
		{
			double sqsum = 0;
			if (list.Count < ignoreStart + ignoreEnd)
				return (byte)0;

			int count = 0;
			double highest = double.MinValue, lowest = double.MaxValue;

			for (int i = ignoreStart + 1; i < list.Count - ignoreEnd; i++)
			{
				sqsum += (list[i] - list[i - 1]) * (list[i] - list[i - 1]);
				if (highest < list[i])
					highest = list[i];
				if (lowest > list[i])
					lowest = list[i];
				count++;
			}
			//max noise is 1;
			return (byte)(255 * Math.Sqrt(sqsum / (count * (highest - lowest) * (highest - lowest))));
		}

		public static byte CalculateNoiseLevel(IList<short> list, int ignoreStart, int ignoreEnd)
		{
			double sqsum = 0;
			if (list.Count < ignoreStart + ignoreEnd)
				return (byte)0;

			int count = 0;
			double highest = double.MinValue, lowest = double.MaxValue;

			for (int i = ignoreStart + 1; i < list.Count - ignoreEnd; i++)
			{
				sqsum += (list[i] - list[i - 1]) * (list[i] - list[i - 1]);
				if (highest < list[i])
					highest = list[i];
				if (lowest > list[i])
					lowest = list[i];
				count++;
			}
			//max noise is 1;
			return (byte)(255 * Math.Sqrt(sqsum / (count * (highest - lowest) * (highest - lowest))));
		}

	}
}