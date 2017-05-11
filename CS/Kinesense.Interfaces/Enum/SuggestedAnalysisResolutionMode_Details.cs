using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Enum
{
    public static class SuggestedAnalysisResolutionMode_Details
    {
        public static string LookupDetails(SuggestedAnalysisResolutionMode sarm)
        {
            switch (sarm)
            {
                case SuggestedAnalysisResolutionMode.VeryBasic:
                    return "(64,64)";
                    break;
                case SuggestedAnalysisResolutionMode.Basic:
                    return "(96,96)";
                    break;
                case SuggestedAnalysisResolutionMode.Standard128:
                    return "(128,128)";
                    break;
                case SuggestedAnalysisResolutionMode.Standard144:
                    return "(144,144)";
                    break;
                case SuggestedAnalysisResolutionMode.Intense192:
                    return "(192,192)";
                    break;
                case SuggestedAnalysisResolutionMode.Intense256:
                    return "(256,256)";
                    break;
                case SuggestedAnalysisResolutionMode.FullScale:
                    return "Full";
                    break;
                case SuggestedAnalysisResolutionMode.Auto:
                default:
                    return "Default";
                    break;
            }
        }
    }
}
