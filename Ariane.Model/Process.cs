using System.Collections.Generic;

namespace Ariane.Model
{
    public class Process
    {
        public string DisplayName { get; set; }
        public string ProcessFileName { get; set; }
        public string Arguments { get; set; }
        public string RootPath { get; set; }
        public ICollection<MeasureSetting> MeasureSettings { get; set; } = new HashSet<MeasureSetting>();
    }
}
