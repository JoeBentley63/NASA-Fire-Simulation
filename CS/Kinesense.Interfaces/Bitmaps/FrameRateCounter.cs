using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinesense.Interfaces.Useful.Averaging;

namespace Kinesense.Interfaces.Classes
{
    public class FrameRateCounter
    {
        DateTime _lastupdate = DateTime.MinValue;

        private RunningAverageOfInts _RunningImportFrameRate = new RunningAverageOfInts(40);

        double _frameRate = 0;
        public double FrameRate
        {
            get
            {
                return _frameRate;
            }
        }

        int _RunningFrameRateCount = 0;
        public void Log(DateTime newframe)
        {
            if(_lastupdate == DateTime.MinValue)
                _lastupdate = newframe;

            if ((newframe - _lastupdate).TotalSeconds >= 1)
            {
                _lastupdate = newframe;
                _RunningImportFrameRate.AddSample(_RunningFrameRateCount);
                _frameRate = _RunningImportFrameRate.Result;
                _RunningFrameRateCount = 0;
            }

            _RunningFrameRateCount++;
        }
    }
}
