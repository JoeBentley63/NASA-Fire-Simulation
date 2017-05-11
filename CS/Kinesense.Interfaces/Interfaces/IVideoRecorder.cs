using Kinesense.Interfaces.Bitmaps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.ImportExport
{
    public interface IVideoRecorder : IDisposable
    {
        void EnqueueFrameForRecording(IVideoFrame frame);

        int JobQueueLength { get; }
    }
}
