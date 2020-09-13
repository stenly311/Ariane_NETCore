using System;
using System.Globalization;
using Ariane.ViewModels;
using Catel.MVVM.Converters;

namespace Ariane.Converters
{    
    public class SelectedItemToIsEnabledConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            var v = value as ProcessViewModel;
            if(v!= null)
            {
                return !v.InProgress;
            }

            return false;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
