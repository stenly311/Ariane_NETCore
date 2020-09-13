using System;
using System.Globalization;
using Catel.MVVM.Converters;

namespace Ariane.Converters
{
    public class BoolToImageConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            bool val;
            if(value != null && bool.TryParse(value.ToString(), out val))
            {
                return val ? @"..\Resources\Images\red.png" : @"..\Resources\Images\green.png";
            }
            return @"..\Resources\Images\gray.png";
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
