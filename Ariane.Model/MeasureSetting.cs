using System.Collections.Generic;

namespace Ariane.Model
{
    public class MeasureSetting
    {
        public string Name { get; set; }
        public int ThresholdMaxTimeInSec { get; set; }
        public virtual ICollection<MeasuredUnit> MeasuredUnits { get; set; }
    }
}
