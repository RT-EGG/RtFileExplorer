using System;
using System.Globalization;
using System.Windows.Data;

namespace RtFileExplorer.Converter
{
    using PropConverter = RtFileExplorer.Model.ValueConverter.IValueConverter;

    internal class FilePropertyConverter : IValueConverter
    {
        public FilePropertyConverter(PropConverter inInternalConverter)
        {
            _internalConverter = inInternalConverter;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => _internalConverter.Convert(value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private readonly PropConverter _internalConverter;
    }
}
