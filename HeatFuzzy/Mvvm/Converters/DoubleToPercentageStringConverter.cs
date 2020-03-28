using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace HeatFuzzy.Mvvm.Converters
{
    public class DoubleToPercentageStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<double> doubleValues)
            {
                IList<string> resultList = new List<string>();
                foreach (var doubleValue in doubleValues)
                {
                    resultList.Add(Convert(doubleValue, targetType, parameter, culture) as string);
                }
                return resultList;
            }
            return ((double)value * 100.0).ToString("#0") + " %";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

