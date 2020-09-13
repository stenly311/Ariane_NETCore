namespace Ariane.Type.Configuration
{
    public class MeasureSettingsConfiguration
    {
        public string Name { get; set; }

        public string StartCode { get; set; }

        public string StopCode { get; set; }

        public bool IsActive { get; set; }

        public int ThresholdMaxTimeInSec { get; set; }

        public bool UseContainsEvaluation { get; set; }

        public bool UseKeyLockedMatch { get; set; }
    }
}
