
namespace Kinesense.Interfaces.Classes
{
    public class AnalyzeOptions
    {
        private int _width = 128;
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        private int _height = 128;
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        private AnalyzeScenarioAlgorithm _analyzeScenarioAlgorithmToUse = AnalyzeScenarioAlgorithm.Standard;
        public AnalyzeScenarioAlgorithm AnalyzeScenarioAlgorithmToUse
        {
            get { return _analyzeScenarioAlgorithmToUse; }
            set { _analyzeScenarioAlgorithmToUse = value; }
        }

        public bool UseDoorwayMode { get; set; }

        private bool? _analyzeForColour = null;
        private bool _analyzeForColour_default = true;
        public bool AnalyzeForColour
        {
            get
            {
                if (_analyzeForColour.HasValue)
                    return _analyzeForColour.Value;
                else
                    return _analyzeForColour_default;
            }
            set
            {
                _analyzeForColour = value;
            }
        }

        private bool? _analyzeForType = null;
        private bool _analyzeForType_default = true;
        public bool AnalyzeForType
        {
            get
            {
                if (_analyzeForType.HasValue)
                    return _analyzeForType.Value;
                else
                    return _analyzeForType_default;
            }
            set
            {
                _analyzeForType = value;
            }
        }

        private bool? _useNoiseFilter = null;
        private bool _useNoiseFilter_default = true;
        public bool UseNoiseFilter
        {
            get
            {
                if (_useNoiseFilter.HasValue)
                    return _useNoiseFilter.Value;
                else
                    return _useNoiseFilter_default;
            }
            set
            {
                _useNoiseFilter = value;
            }
        }

        /// <summary>
        /// This is used for when we only want to analyses a smaller areas of the image field, or have
        /// passed a cropped area and need to tell the algorithm to adjust its positions accordingly
        /// </summary>
        public RelativeRegion RelativeRegion { get; set; }

    }
}
