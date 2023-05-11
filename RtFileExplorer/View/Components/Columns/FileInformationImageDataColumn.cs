using RtFileExplorer.Model.FileInformation;
using RtFileExplorer.ViewModel.Wpf.PathInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace RtFileExplorer.View.Components.Columns
{
    internal class FileInformationImageDataColumn : DataGridTemplateColumn, IFileInformationDataColumn
    {
        public FileInformationImageDataColumn(FilePropertyItemType inType)
        {
            ItemType = inType;

            var image = new FrameworkElementFactory(typeof(Image));
            image.SetValue(Image.HeightProperty, 16.0);
            image.SetValue(Image.StretchProperty, Stretch.UniformToFill);
            image.SetValue(Image.SourceProperty, FileInformationDataColumn.CreateBinding(inType));
            image.SetBinding(StackPanel.VisibilityProperty, new Binding
            {
                Path = new PropertyPath(string.Empty),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.Explicit,
                Converter = new FileInformationDataColumn.SupportedPathConverter(),
                ConverterParameter = PathInformationColumnViewModel.GetPropertyName(ItemType),
            });

            CellTemplate = new DataTemplate() { VisualTree = image };
        }

        public FilePropertyItemType ItemType { get; }
    }
}
