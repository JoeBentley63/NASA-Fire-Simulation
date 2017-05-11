using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Enum
{
    public class DefaultAnalysisResolutionMode
    {
        public static SuggestedAnalysisResolutionMode Default = SuggestedAnalysisResolutionMode.Auto;

        public static void SetDefault(string mode)
        {
            try
            {

                SuggestedAnalysisResolutionMode m = SuggestedAnalysisResolutionMode.Standard128;
                System.Enum.TryParse(mode, out m);
                Default = m;
            }
            catch (System.Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }
        }
    }
}
