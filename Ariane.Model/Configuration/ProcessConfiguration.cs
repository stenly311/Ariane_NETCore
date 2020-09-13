using System.Collections.Generic;

namespace Ariane.Type.Configuration
{
    public class ProcessConfiguration
    {
        public string DisplayName { get; set; }
        public string ProcessFileName { get; set; }
        public string Arguments { get; set; }
        public string RootPath { get; set; }
        public string LoggingSourceType { get; set; }
        public string RabbitMQTopicName { get; set; }
        public List<MeasureSettingsConfiguration> MeasureSettings { get; set; } = new List<MeasureSettingsConfiguration>();        
    }
}
