using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Enum
{
    public enum DeinterlaceMode
    {
        SkipField1,
        SkipField2,
        RepeatField1,
        RepeatField2,
        SideBySideSkipFields,
        Normal,                          //No deinterlace
        SideBySideRepeatFields
    };


}
