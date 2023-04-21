using System;
using System.Globalization;
using System.Windows.Data;

namespace RtFileExplorer.Converter
{
    internal class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => ((DateTime)value).ToString("yyyy/MM/dd HH:mm");

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
