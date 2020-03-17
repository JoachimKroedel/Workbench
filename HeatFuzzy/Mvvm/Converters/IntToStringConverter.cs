using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace HeatFuzzy.Mvvm.Converters
{
    public class IntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<int> intValues)
            {
                IList<string> resultList = new List<string>();
                foreach (var intValue in intValues)
                {
                    resultList.Add(Convert(intValue, targetType, parameter, culture) as string);
                }
                return resultList;
            }
            return ((int)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<string> stringValues)
            {
                IList<int> resultList = new List<int>();
                foreach (var stringValue in stringValues)
                {
                    resultList.Add((int)ConvertBack(stringValue, targetType, parameter, culture));
                }
                return resultList;
            }
            return int.Parse(value as string);
        }
    }
}
