using RtFileExplorer.Model.FileInformation;
using System;
using System.Globalization;
using System.Windows.Data;

namespace RtFileExplorer.Converter
{
    internal class FilePropertyItemTypeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not FilePropertyItemType)
                throw new InvalidProgramException();

            var type = (FilePropertyItemType)parameter;
            var att = type.GetFilePropertyAttribute();

            return att.DisplayText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
