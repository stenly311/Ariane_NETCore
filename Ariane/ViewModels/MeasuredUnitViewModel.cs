using Ariane.Attributes;
using System;
using System.ComponentModel;

namespace Ariane.ViewModels
{
    public partial class MeasuredUnitViewModel
    {        
        public MeasuredUnitViewModel(MeasureSettingViewModel setting, string matchKey = null)
        {
            MeasureSetting = setting;
            MatchKey = matchKey?.Replace("#", "");
        }

        [Visible(true)]
        [DisplayName("Start")]
        public DateTime StartTime { get; set; }

        [Visible(true)]
        [DisplayName("Stop")]
        public DateTime? StopTime { get; set; }

        [DisplayName("Is Canceled")]
        public bool WasCanceled { get; set; }

        [Visible(true)]
        [DisplayName("Elapse Time [s]")]
        public int ElapseTimeInSeconds
        {
            get
            {
                return WasCanceled ? 0 : (int)ElapseTime.TotalSeconds;
            }
        }

        [Visible(true)]
        [DisplayName("Is Valid")]
        public bool IsRunning
        {
            get
            {
                return StopTime == null;
            }
        }

        public string MatchKey { get; private set; }

        public TimeSpan ElapseTime
        {
            get
            {
                if(StopTime != null)
                    return ((DateTime)StopTime - StartTime);
                return TimeSpan.Parse("0");
            }
        }

        public MeasureSettingViewModel MeasureSetting { get; set; }

        public override string ToString()
        {
            return ElapseTime.ToString(@"hh:\mm:\ss");
        }
    }
}
