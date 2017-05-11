using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Clarification
{
    public delegate void ErrorStateDelegate(bool state);
    public delegate void RequestForClipPreviewFramesDelegate(int numberOfFrames, bool isPreviewVideo);

}
