using RtFileExplorer.Model.FileInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RtFileExplorer.View.Columns
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

            CellTemplate = new DataTemplate() { VisualTree = image };
        }

        public FilePropertyItemType ItemType { get; }
    }
}
