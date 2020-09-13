using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Ariane.Attributes;
using Ariane.Model;
using Ariane.ViewModels;
namespace Ariane.Views
{
    /// <summary>
    /// Interaction logic for WatcherSettingsWindow.xaml
    /// </summary>
    public partial class WatcherSettingsWindow : Window
    {
        public ProcessViewModel ProcessViewModel { get; private set; }

        public WatcherSettingsWindow(ProcessViewModel processViewModel)
        {
            ProcessViewModel = processViewModel;
            InitializeComponent();
        }

        public ClosingOptionEnum ClosingOptionSelected { get; set; }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            ClosingOptionSelected = ClosingOptionEnum.SaveClose;
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {           
            e.Column.Visibility = Visibility.Collapsed;

            if (GetVisibleColumns().Contains(e.Column.Header.ToString()))
                e.Column.Visibility = Visibility.Visible;

            e.Column.Header = ((PropertyDescriptor)e.PropertyDescriptor).DisplayName;

            //if (GetColumnDisplayNames().Keys.Contains(e.Column.Header.ToString()))
            //{
            //    e.Column.Header = GetColumnDisplayNames()[e.Column.Header.ToString()];
            //}
        }

        // cached columns visibility info
        private static List<string> GetVisibleColumns()
        {
            return typeof(MeasureSettingViewModel).GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(Visible), true).Where(ca => ((Visible)ca).IsVisible).Any())
                .Select(s => s.Name)
                .ToList();
        }

        // cached columns header info
        //private static Dictionary<string, string> GetColumnDisplayNames()
        //{
        //    return typeof(MeasureSetting).GetProperties()
        //        .Select(x => new { Key = x.Name, Attributes = x.CustomAttributes.Where(xx => xx.AttributeType == typeof(DisplayNameAttribute)) })
        //        .ToDictionary(x => x.Key, y => ((DisplayNameAttribute)y.Attributes.));            
        //}
    }
    public enum ClosingOptionEnum
    {
        Close, SaveClose
    }
}
