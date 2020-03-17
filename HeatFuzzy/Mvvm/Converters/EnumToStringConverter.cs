using HeatFuzzy.Logic.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace HeatFuzzy.Mvvm.Converters
{
    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<FuzzyRadiatorControlChangeTypes> enumValues)
            {
                IList<string> resultList = new List<string>();
                foreach (var enumValue in enumValues)
                {
                    resultList.Add(Convert(enumValue, targetType, parameter, culture) as string);
                }
                return resultList;
            }
            return ((Enum)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<string> stringValues)
            {
                IList<Enum> resultList = new List<Enum>();
                foreach (var stringValue in stringValues)
                {
                    resultList.Add((Enum)ConvertBack(stringValue, targetType, parameter, culture));
                }
                return resultList;
            }
            return Enum.Parse(targetType, value as string);
        }
    }
}
