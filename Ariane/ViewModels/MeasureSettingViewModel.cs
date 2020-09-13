using Catel.MVVM;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using Ariane.Attributes;
using System.ComponentModel;

namespace Ariane.ViewModels
{
    public partial class MeasureSettingViewModel : ViewModelBase
    {
        public const string KeyLockedPattern = "[^#]";
        public static string KeyLockedPatternFormat = "#{0}+#";

        public MeasureSettingViewModel() { }

        public MeasureSettingViewModel(string name)
        {
            Name = name;
        }

        [Visible(true)]
        [DisplayName("Named Setting")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Start Code")]
        [Visible(true)]
        public string StartCode { get; set; }

        [Required]
        [DisplayName("Stop Code")]
        [Visible(true)]
        public string StopCode { get; set; }

        [Visible(true)]
        [DisplayName("Use Key Locked Match")]
        public bool UseKeyLockedMatch { get; set; }

        [DisplayName("Use Contains")]
        [Visible(true)]
        public bool UseContainsEvaluation { get; set; }
                
        [DisplayName("Is Active")]
        [Visible(true)]
        public bool IsActive { get; set; }
        
        [DisplayName("Treshold MAX Time [s]")]
        [Visible(true)]
        public int ThresholdMaxTimeInSec { get; set; }

        public bool IsOverMeasureOveLimit
        {
            get
            {
                return IsMeasureOverLimit() ?? false;
            }
        }

        public bool IsReadyToUse
        {
            get { return !string.IsNullOrEmpty(StartCode) && !string.IsNullOrEmpty(StopCode); }
        }

        public Visibility StartVisibility { get; set; } = Visibility.Visible;
        public Visibility StopVisibility { get; set; } = Visibility.Collapsed;

        [DisplayName("Median value [s]")]
        [Visible(true)]
        public int? Median
        {
            get
            {
                var meas = GetMedianFromMeasuredUnits();
                return meas > 0 ? (System.Nullable<int>)meas : null;
            }
        }

        public int GetMedianFromMeasuredUnits()
        {
            if (MeasuredUnits != null && MeasuredUnits.Any())
            {
                var units = MeasuredUnits.Where(x => !x.IsRunning).ToList().OrderBy(x => x.ElapseTimeInSeconds).ToList();
                //even or odd?
                var count = units.Count;
                if (count > 0 && count % 2 == 0)
                {
                    var index = count / 2;

                    var middleLeft = units[index - 1].ElapseTimeInSeconds;
                    var middleRight = units[index].ElapseTimeInSeconds;

                    return (middleLeft + middleRight) / 2;
                }
                else
                {
                    if (count > 1)
                    {
                        var index = (count - 1) / 2;
                        return units[index].ElapseTimeInSeconds;
                    }

                    if(units.Count > 0) return units[0].ElapseTimeInSeconds;
                }
            }
            return 0;
        }

        public bool? IsMeasureOverLimit()
        {
            var ev = GetMedianFromMeasuredUnits();
            if (ev == 0)
            {
                return null;
            }
            return  ev > ThresholdMaxTimeInSec;
        }
        
        public ICollection<MeasuredUnitViewModel> MeasuredUnits { get; set; } = new HashSet<MeasuredUnitViewModel>();
    }
}
