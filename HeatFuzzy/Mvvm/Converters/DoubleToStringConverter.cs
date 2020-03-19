using System;
using System.Globalization;
using System.Windows.Data;

namespace HeatFuzzy.Mvvm.Converters
{
    public class DoubleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = ((double)value).ToString("#0.0");
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return double.Parse(value as string);
        }
    }
}
