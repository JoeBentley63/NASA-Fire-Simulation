using System;

namespace Kinesense.Interfaces.Misc
{
    // NOTE
    //
    // The end measurements should be taken as inclusive ends!
    // i.e. <= NOT <
    //

    public class SubRegionDefinition
    {
        private int _sourceRegionWidth = 0;
        private int _sourceRegionHeight = 0;

        private int _x_max { get { return _sourceRegionWidth - 1; } }
        private int _y_max { get { return _sourceRegionHeight - 1; } }

        private double _x_start_double = 0;
        private double _x_end_double = 0;
        private double _y_start_double = 0;
        private double _y_end_double = 0;

        private bool fixedMode = true;
        private bool xyLocked = false;

        public SubRegionDefinition(int sourceRegionWidth, int sourceRegionHeight)
        {
            _sourceRegionWidth = sourceRegionWidth;
            _sourceRegionHeight = sourceRegionHeight;
            xyLocked = true;
        }

        /// <summary>
        /// Initializing like this restrict what you can do until a width height is set, use only for
        /// coping via proportional estimation
        /// </summary>
        public SubRegionDefinition()
        {
            xyLocked = false;
        }

        public void SetWidthHeight(int sourceRegionWidth, int sourceRegionHeight)
        {
            if (xyLocked)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogEventLevel("SubRefionDefinition.SetWidthHeight - WidthHeight Already Locked", 1);
                throw new Exception("WidthHeight Already Locked");
            }

            _sourceRegionWidth = sourceRegionWidth;
            _sourceRegionHeight = sourceRegionHeight;
            xyLocked = true;
        }

        public bool DoNeedWidthHeightSetting
        {
            get
            {
                return !xyLocked;
            }
        }


        /// <summary>
        /// defines the subregion to be analyzed
        /// </summary>
        /// <param name="wS">Start width</param>
        /// <param name="wE">End Width</param>
        /// <param name="hS">Start Height</param>
        /// <param name="hE">End Height</param>
        /// <returns></returns>
        //public void SetRegionAbsolute(int wS, int wE, int hS, int hE)
        //{
        //    if (xyLocked)
        //    {

        //        if (wS >= 0 && wS <= _x_max)
        //            _x_start = wS;
        //        else
        //        {
        //            Kinesense.Interfaces.DebugMessageLogger.LogEventLevel("SubRegionDefinition - Invalid region parameter", 1);
        //            throw new Exception("Invalid Values");
        //        }

        //        if (wE > wS && wE <= _x_max)
        //            _x_end = wE;
        //        else
        //        {
        //            Kinesense.Interfaces.DebugMessageLogger.LogEventLevel("SubRegionDefinition - Invalid region parameter", 1);
        //            throw new Exception("Invalid Values");
        //        }

        //        if (hS >= 0 && hS <= _y_max)
        //            _y_start = hS;
        //        else
        //        {
        //            Kinesense.Interfaces.DebugMessageLogger.LogEventLevel("SubRegionDefinition - Invalid region parameter", 1);
        //            throw new Exception("Invalid Values");
        //        }

        //        if (hE > hS && hE <= _y_max)
        //            _y_end = hE;
        //        else
        //        {
        //            Kinesense.Interfaces.DebugMessageLogger.LogEventLevel("SubRegionDefinition - Invalid region parameter", 1);
        //            throw new Exception("Invalid Values");
        //        }

        //        fixedMode = true;
        //    }
        //    else
        //    {
        //        throw new Exception("Width Height has not been set");
        //    }
        //}

        /// <summary>
        /// defines the subregion to be analyzed as a ratio of the original image
        /// </summary>
        /// <param name="wS">Start width multiplier (0-1)</param>
        /// <param name="wE">End Width multiplier (0-1)</param>
        /// <param name="hS">Start Height multiplier (0-1)</param>
        /// <param name="hE">End Height multiplier (0-1)</param>
        /// <returns></returns>
        //public void SetRegionRatio(double wS, double wE, double hS, double hE)
        //{
        //    if (xyLocked)
        //    {
        //        if (wS >= 0 && wS <= 1)
        //            _x_start_double = wS;
        //        else
        //        {
        //            Kinesense.Interfaces.DebugMessageLogger.LogEventLevel("SubRegionDefinition - Invalid region parameter", 1);
        //            throw new Exception("Invalid Values");
        //        }

        //        if (wE > wS && wE <= 1 && ((wS * _sourceRegionWidth) - (wE * _sourceRegionWidth) >= 1))
        //            _x_end_double = wE;
        //        else
        //        {
        //            Kinesense.Interfaces.DebugMessageLogger.LogEventLevel("SubRegionDefinition - Invalid region parameter", 1);
        //            throw new Exception("Invalid Values");
        //        }

        //        if (hS >= 0 && hS <= 1)
        //            _y_start_double = hS;
        //        else
        //        {
        //            Kinesense.Interfaces.DebugMessageLogger.LogEventLevel("SubRegionDefinition - Invalid region parameter", 1);
        //            throw new Exception("Invalid Values");
        //        }

        //        if (hE > hS && hE <= 1 && ((hS * _sourceRegionWidth) - (hE * _sourceRegionWidth) >= 1))
        //            _y_end_double = hE;
        //        else
        //        {
        //            Kinesense.Interfaces.DebugMessageLogger.LogEventLevel("SubRegionDefinition - Invalid region parameter", 1);
        //            throw new Exception("Invalid Values");
        //        }

        //        fixedMode = false;

        //    }
        //    else
        //    {
        //        throw new Exception("Width Height has not been set");
        //    }

        //}

        /// <summary>
        /// Sets the ratios of where to sub-select the data from, does not require width and height to be pre-set
        /// </summary>
        /// <param name="wS"></param>
        /// <param name="wE"></param>
        /// <param name="hS"></param>
        /// <param name="hE"></param>
        public void SetRegionRatio_WithoutPresetWidthHeight(double wS, double wE, double hS, double hE)
        {
            if (wS >= 0 && wE <= 1 && wE > wS)
            {
                _x_end_double = wE;
                _x_start_double = wS;
            }
            else
                throw new Exception("Invalid Values");

            if (hS >= 0 && hE <= 1 && hE > hS)
            {
                _y_end_double = hE;
                _y_start_double = hS;
            }
            else
                throw new Exception("Invalid Values");
        }

        public int WidthStart
        {
            get
            {
                if (xyLocked)
                {
                    //if (fixedMode)
                    //{
                    //    return _x_start;
                    //}
                    //else
                    //{
                        return (int)Math.Floor(_x_max * _x_start_double);
                    //}
                }
                else
                {
                    throw new Exception("Width Height net set, so value meaningless");
                }
            }
        }

        public int WidthEnd
        {
            get
            {
                if (xyLocked)
                {
                    //if (fixedMode)
                    //{
                    //    return _x_end;
                    //}
                    //else
                    //{
                        return Math.Min((int)Math.Ceiling(_x_max * _x_end_double), _x_max);
                    //}
                }
                else
                {
                    throw new Exception("Width Height net set, so value meaningless");
                }
            }
        }

        public int Width
        {
            get
            {
                return WidthEnd - WidthStart + 1;
            }

        }

        public int HeightStart
        {
            get
            {
                if (xyLocked)
                {
                    //if (fixedMode)
                    //{
                    //    return _y_start;
                    //}
                    //else
                    //{
                        return (int)Math.Floor(_y_max * _y_start_double);
                    //}
                }
                else
                {
                    throw new Exception("Width Height net set, so value meaningless");
                }
            }
        }

        public int HeightEnd
        {
            get
            {
                if (xyLocked)
                {
                    //if (fixedMode)
                    //{
                    //    return _y_end;
                    //}
                    //else
                    //{
                        return Math.Min((int)Math.Ceiling(_y_max * _y_end_double), _y_max);
                    //}
                }
                else
                {
                    throw new Exception("Width Height net set, so value meaningless");
                }
            }
        }

        public int Height
        {
            get
            {
                return HeightEnd - HeightStart+1;
            }
        }


        /// <summary>
        /// Takes the short.Max scaled values from a subregion and re-interprets them
        /// as values relative to the source image
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <param name="res_x1"></param>
        /// <param name="res_x2"></param>
        /// <param name="res_y1"></param>
        /// <param name="res_y2"></param>
        public void ConvertSubregionValuesIntoSourceValues(short x1, short x2, short y1, short y2, ref short res_x1, ref short res_x2, ref short res_y1, ref short res_y2)
        {

            try
            {
                if (DoNeedWidthHeightSetting)
                    throw new Exception("Width and Height of Source not set!");


                short x_step = (short)(((double)WidthStart / _x_max) * short.MaxValue);
                short y_step = (short)(((double)HeightStart / _y_max) * short.MaxValue);

                short width_step = (short)(((double)Width / _sourceRegionWidth) * short.MaxValue);
                short height_step = (short)(((double)Height / _sourceRegionHeight) * short.MaxValue);



                res_x1 = (short)((short)x_step + (short)(((double)x1 / short.MaxValue) * width_step));
                res_x2 = (short)((short)x_step + (short)(((double)x2 / short.MaxValue) * width_step));
                res_y1 = (short)((short)y_step + (short)(((double)y1 / short.MaxValue) * height_step));
                res_y2 = (short)((short)y_step + (short)(((double)y2 / short.MaxValue) * height_step));

            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
            }

        }


    }
}
