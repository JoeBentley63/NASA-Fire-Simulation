using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Classes
{
    public class PollyRecoveredFrame 
    {
        public PollyRecoveredFrame(VideoFrame rFrame, int disposeKey)
        {
            this.Fdata = rFrame;
            this.DisposeKey = disposeKey;
        }


        public VideoFrame Fdata { get; set; }
        public int DisposeKey { get; set; }


        public void DisposeWithKey()
        {
            try
            {
                if (Fdata != null)
                    Fdata.Dispose();//.Dispose(DisposeKey);
            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
            }

        }

    }
}
