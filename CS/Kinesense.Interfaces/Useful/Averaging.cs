using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Useful
{
    namespace Averaging
    {
        /// <summary>
        /// Provides a running average of int values
        /// This version is designed to be a play-off of speed and accuracy. Once the sample buffer 
        /// has been 100% filled, it is 100% accurate, but until that point it will under report as
        /// the unassigned members of the buffer are 0
        /// </summary>
        public class RunningAverageOfInts
        {
            private int _Samples;
            private int[] _Data;
            private int _Pos = 0;
            private int _sum = 0;
            public bool RestrictPoorMeans = true;
            private bool _Cycled = false;

            public RunningAverageOfInts(int sampleSize)
            {
                _Samples = sampleSize;
                _Data = new int[sampleSize];
            }

            public void AddSample(int sample)
            {
                _sum -= _Data[_Pos];
                _sum += sample;
                _Data[_Pos] = sample;
                _Pos = (_Pos + 1) % _Samples;
                if (!_Cycled)
                    if (_Pos == 0)
                        _Cycled = true;
            }

            public double Result
            {
                get
                {
                    if (RestrictPoorMeans)
                        if (!_Cycled)
                            return 0;

                    return _sum / _Samples;
                }
            }

            public double Result_ThusFar
            {
                get
                {
                if (RestrictPoorMeans)
                    if (!_Cycled)
                        return _sum / (_Pos + 1);


                return Result;
                }
            }

            public double AddSampleAndGiveResult(int sample)
            {
                AddSample(sample);
                return Result;
            }

            public void Clear()
            {
                _Pos = 0;
                _sum = 0;
                _Data = new int[_Samples];
                _Cycled = false;
            }
        }
        /// <summary>
        /// Provides a running average of double values
        /// This version is designed to be a play-off of speed and accuracy. Once the sample buffer 
        /// has been 100% filled, it is 100% accurate, but until that point it will under report as
        /// the unassigned members of the buffer are 0
        /// </summary>
        public class RunningAverageOfDoubles
        {
            private int _Samples;
            private double[] _Data;
            private int _Pos = 0;
            private double _sum = 0;
            public bool RestrictPoorMeans = true;
            private bool _Cycled = false;

            public RunningAverageOfDoubles(int sampleSize)
            {
                _Samples = sampleSize;
                _Data = new double[sampleSize];
            }

            public bool BufferFull
            {
                get { return _Cycled; }
            }

            public void AddSample(double sample)
            {
                _sum -= _Data[_Pos];
                _sum += sample;
                _Data[_Pos] = sample;
                _Pos = (_Pos + 1) % _Samples;
                if (!_Cycled)
                    if (_Pos == 0)
                        _Cycled = true;
            }

            public double Result
            {                get
                {
                if (RestrictPoorMeans)
                    if (!_Cycled)
                        return 0;

                return _sum / _Samples;}
            }

            public double Result_ThusFar
            {
                                get
                {
                if (RestrictPoorMeans)
                    if (!_Cycled)
                        return _sum / (_Pos + 1);
                                
                
                return Result;
                }
            }

            public double AddSampleAndGiveResult(double sample)
            {
                AddSample(sample);
                return Result;
            }

            public double AddSampleAndGiveResult_ThusFar(double sample)
            {
                AddSample(sample);
                return Result_ThusFar;
            }

            public void Clear()
            {
                _Pos = 0;
                _sum = 0;
                _Data = new double[_Samples];
                _Cycled = false;
            }
        }


    }
}
