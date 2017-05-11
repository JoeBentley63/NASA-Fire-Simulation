using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireApp
{
    // Wind classification
    // See http://www.windows2universe.org/earth/Atmosphere/wind_speeds.html for details
    // Each int value corresponds to maximum speed of a corresponding type
    // Units are km/h
    public enum WindTypes
    {
        NO_WIND = 1,
        LIGHT_WIND = 11,
        MODERATE_WIND = 38,
        STRONG_WIND = 61
    }
}
