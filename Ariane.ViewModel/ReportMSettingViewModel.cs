using Ariane.Model;
using Ariane.Types;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Catel.MVVM;
using System;

namespace Ariane.ViewModel.Win
{
    public class ReportMSettingViewModel : ViewModelBase
    {
        public ReportMSettingViewModel(MeasureSetting setting)
        {
            Setting = setting;            
        }

        #region Properties

        public int Number { get; private set; }

        public MeasureSetting Setting { get; }

        IEnumerable<RPoint> _segmentData;
        public IEnumerable<RPoint> SegmentData
        {
            get
            {                
                return _segmentData ?? GetSegmentData(Setting.ThresholdMaxTimeInSec, Setting.MeasuredUnits);
            }
        }

        #endregion

        #region Public methods

        public void FilterByDate(DateTime from, DateTime to)
        {
            _segmentData = GetSegmentData(Setting.ThresholdMaxTimeInSec, Setting.MeasuredUnits.Where(x => x.StartTime >= from && x.StartTime < to));
            RaisePropertyChanged(() => SegmentData);
        }

        #endregion

        /// <summary>
        /// 4 segments, each per 25% rating from threshold MAX value
        /// </summary>
        /// <returns></returns>
        private IEnumerable<RPoint> GetSegmentData(int thresholdMaxTimeInSec, IEnumerable<MeasuredUnit> measureUnits)
        {
            var segData = new List<RPoint> {
                new RPoint(0, "Fast", Color.Green), new RPoint(0, "Medium", Color.BlueViolet), new RPoint(0, "Slow", Color.Salmon), new RPoint(0, "Very Slow", Color.DarkRed) };

            if (measureUnits!= null && measureUnits.Any())
            {
                var max = thresholdMaxTimeInSec;
                foreach (var unit in measureUnits)
                {
                    var m = (unit.ElapseTimeInSeconds * 100)/max;
                    if (m <= 25)
                    {
                        segData[0].Count++;
                    }
                    else if (25 < m && m <= 50)
                    {
                        segData[1].Count++;
                    }
                    else if (50 < m && m <= 75)
                    {
                        segData[2].Count++;
                    }
                    else if (75 < m)
                    {
                        segData[3].Count++;
                    }
                }
            }
            return segData;
        }
    }
}
