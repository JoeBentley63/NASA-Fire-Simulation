
namespace Kinesense.Interfaces.Enum
{
    public enum SuggestedAnalysisResolutionMode
    {
        //TODO: new mode for upscale when image tiny (needed for screen capture)
        VeryBasic, // 64,64
        Basic, // 96,96
        Standard128, // 128, 128 if image image is > 128,128
        Standard144, // 144,144
        Intense192, // 192,192 if image > 192,192
        Intense256, // 256, 256
        FullScale, // full, full
        Auto
    }    
}
