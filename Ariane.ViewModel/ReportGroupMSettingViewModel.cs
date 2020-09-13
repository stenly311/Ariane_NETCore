using Ariane.Model;
using Catel.MVVM;
using System.Collections.Generic;
using System.Linq;

namespace Ariane.ViewModel.Win
{
    public class ReportGroupProcessMSettingViewModel : ViewModelBase
    {
        public ReportGroupProcessMSettingViewModel(string processName, List<MeasureSetting> mSettingViewModels, bool isExpanded)
        {
            ProcessName = processName;            
            MeasureSettings = mSettingViewModels.Select(x=>new ReportMSettingViewModel(x)).ToList();
            IsExpanded = isExpanded;
        }
        public string ProcessName { get; private set; }

        public List<ReportMSettingViewModel> MeasureSettings { get; set; } = new List<ReportMSettingViewModel>();

        public bool IsExpanded { get; private set; }
    }
}
