using Catel.MVVM.Converters;
using System;
using System.Globalization;
using System.Windows;

namespace Ariane.Converters
{    
    public class ReverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            bool val;
            bool.TryParse(value.ToString(), out val);

            return !val ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
