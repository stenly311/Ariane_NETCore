using System;
using System.Globalization;
using Catel.MVVM.Converters;

namespace Ariane.Converters
{
    public class RadioBoolToIntConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            int integer = (int)value;
            if (integer == int.Parse(parameter.ToString()))
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}
