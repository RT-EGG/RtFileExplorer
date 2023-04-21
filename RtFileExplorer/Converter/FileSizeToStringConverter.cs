using System;
using System.Globalization;
using System.Windows.Data;

namespace RtFileExplorer.Converter
{
    internal class FileSizeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetFileSizeText((long)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static string GetFileSizeText(long inFileSize)
        {
            int unit = 1024;
            float size = inFileSize;
            int index = 0;

            while (size >= unit)
            {
                size /= unit;
                index++;
            }

            return $"{size.ToString("#,##0.##")}{_suffix[index]}";
        }

        private static readonly string[] _suffix = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
    }
}
