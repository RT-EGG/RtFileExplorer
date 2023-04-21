using RtFileExplorer.Model.FileInformation;
using System.Windows.Controls;

namespace RtFileExplorer.View.Columns
{
    internal class FileInformationTextDataColumn : DataGridTextColumn, IFileInformationDataColumn
    {
        public FileInformationTextDataColumn(FilePropertyItemType inType)
        {
            ItemType = inType;
            Binding = FileInformationDataColumn.CreateBinding(inType);
        }

        public FilePropertyItemType ItemType { get; }
    }
}
