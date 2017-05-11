using System;
using System.Linq;

namespace Kinesense.Interfaces.Useful
{
    public class FrameRateNormaliser
    {
        public FrameRateNormaliser(int requredFrameRate)
        {
            RequiredFrameRate = requredFrameRate;
        }

        public int RequiredFrameRate { get; private set; }



        public int NormaliseMeTheseFrames(DateTime[] sourceFrames, out int[] framePositions)
        {
            if (sourceFrames == null || sourceFrames.Length < 2)
            {
                framePositions = new int[1];
                return -1;
            }

            try
            {
                TimeSpan Duration = sourceFrames[sourceFrames.Length - 1] - sourceFrames[0];

                TimeSpan AverageDuration = new TimeSpan(0,0,0,(int)(Duration.TotalMilliseconds / sourceFrames.Count()));
                if (AverageDuration.TotalSeconds > 1)
                    AverageDuration = new TimeSpan(0, 0, 1);


                int framesNeeded = (int)Math.Ceiling(RequiredFrameRate*(Duration.TotalSeconds + AverageDuration.TotalSeconds));

                int[] positions = new int[framesNeeded];






            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
            }


            framePositions = new int[1];
            return -1;
        }


    }
}
