using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ariane.Model
{
    public class MeasuredUnit
    {
        public DateTime StartTime { get; set; }

        public DateTime? StopTime { get; set; }

        public bool WasCanceled { get; set; }
        
        public int ElapseTimeInSeconds{ get; set; }

        public virtual MeasureSetting MeasureSettings { get; set; }

        public string MatchKey { get; set; }

    }
}
