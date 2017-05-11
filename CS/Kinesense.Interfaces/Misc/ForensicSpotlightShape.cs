using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Classes
{
    public enum ForensicSpotlightShape
    {
        Circle, Rectangle, Line, ArrowLeft, ArrowRight
    }

    public class ForensicSpotlightShapePosition
    {
        public int[] Rect { get; set; }
        public ForensicSpotlightShape Shape { get; set; }
    }

    //public enum ArrowShape
    //{
    //    Line, ArrowLeft, ArrowRight
    //}

    //public class ArrowShapePosition
    //{
    //    public int[] Rect { get; set; }
    //    public ArrowShape Shape { get; set; }
    //}
}
