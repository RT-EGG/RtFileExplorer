using RtFileExplorer.Model.FileInformation;
using RtFileExplorer.ViewModel.Wpf.PathInformation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RtFileExplorer.View.Columns
{
    internal class FileInformationRatingDataColumn : DataGridTemplateColumn, IFileInformationDataColumn
    {
        public FileInformationRatingDataColumn(FilePropertyItemType inType)
        {
            ItemType = inType;

            var propertyPath = PathInformationColumnViewModel.GetPropertyPath(ItemType);

            var panel = new FrameworkElementFactory(typeof(StackPanel));
            panel.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            panel.SetBinding(StackPanel.VisibilityProperty, new Binding
            {
                Path = new PropertyPath(string.Empty),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.Explicit,
                Converter = new FileInformationDataColumn.SupportedPathConverter(),
                ConverterParameter = PathInformationColumnViewModel.GetPropertyName(ItemType),
            });

            for (uint i = 0; i < 5; ++i)
            {
                var button = new RatingButtonElementFactory(propertyPath, i + 1);
                panel.AppendChild(button);
            }

            CellTemplate = new DataTemplate() { VisualTree = panel };
        }

        public FilePropertyItemType ItemType { get; }

        private class RatingButtonElementFactory : FrameworkElementFactory
        {
            public RatingButtonElementFactory(PropertyPath inPropertyPath, uint inRatingNumber)
                : base (typeof(Button))
            {
                RatingNumber = inRatingNumber;

                SetBinding(Button.ContentProperty, new Binding()
                {
                    Path = inPropertyPath,
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Converter = new RatingToTextConverter(),
                    ConverterParameter = inRatingNumber,
                });
                AddHandler(Button.ClickEvent, new RoutedEventHandler(OnClick));
            }

            private void OnClick(object sender, RoutedEventArgs e)
            {
                if (e.Source is Button button)
                {
                    var bindingExpression = button.GetBindingExpression(Button.ContentProperty);
                    if (bindingExpression is not null)
                    {
                        bindingExpression.UpdateSource();
                    }
                }
            }

            private readonly uint RatingNumber;
        }

        private class RatingToTextConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (parameter is not uint)
                    throw new InvalidProgramException();

                if (value is null)
                    return "☆";

                if (value is uint)
                {
                    var rating = (uint)value;
                    return rating >= (uint)parameter ? "★" : "☆";
                }

                return "";
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (parameter is not uint)
                    throw new InvalidProgramException();

                return (uint)parameter;
            }
        }
    }
}
