using System;
using System.Globalization;
using Catel.MVVM.Converters;

namespace Ariane.Converters
{
    public class BoolToImageConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            if(value != null && bool.TryParse(value.ToString(), out var val))
            {
                return val ? @"pack://application:,,,/Resources/Images/red.png" : @"pack://application:,,,/Resources/Images/green.png";
            }

            return @"pack://application:,,,/Resources/Images/gray.png";            
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
