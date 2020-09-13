using Ariane.Model;
using Catel.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ariane.ViewModel.Win
{
    public class ReportViewModel : ViewModelBase
    {
        public ReportViewModel(IEnumerable<Process> processes)
        {
            var first = true;
            foreach (Process p in processes)
            { 
                ProcessSettings.Add(new ReportGroupProcessMSettingViewModel(p.DisplayName, p.MeasureSettings.Where(x=>x.MeasuredUnits.Any()).ToList(), first));
                first = false;
            }            
        }
                
        public DateTime SelectedDate { get; set; } = DateTime.Now;

        public void FilterByDate()
        {
            ProcessSettings
                .ForEach(x => x.MeasureSettings
                    .ForEach(a => a.FilterByDate(new DateTime(SelectedDate.Year, SelectedDate.Month, SelectedDate.Day), new DateTime(SelectedDate.Year, SelectedDate.Month, SelectedDate.Day+1))));
        }

        public List<ReportGroupProcessMSettingViewModel> ProcessSettings { get; private set; } = new List<ReportGroupProcessMSettingViewModel>();        
    }
}
