using System;
using System.Globalization;
using System.Windows;
using Catel.MVVM.Converters;

namespace Ariane.Converters
{    
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            bool val;
            bool.TryParse(value.ToString(), out val);

            return val ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
