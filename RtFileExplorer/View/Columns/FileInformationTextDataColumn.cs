using RtFileExplorer.Model.FileInformation;
using System.Windows;
using System.Windows.Controls;

namespace RtFileExplorer.View.Columns
{
    internal class FileInformationTextDataColumn : DataGridTextColumn, IFileInformationDataColumn
    {
        public FileInformationTextDataColumn(FilePropertyItemType inType)
        {
            ItemType = inType;
            Binding = FileInformationDataColumn.CreateBinding(inType, System.Windows.Data.UpdateSourceTrigger.Explicit);            
        }

        public FilePropertyItemType ItemType { get; }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var element = base.GenerateElement(cell, dataItem);

            if (element is TextBox textBox)
            {
                textBox.LostFocus += (sender, e) =>
                {
                    var textBox = sender as TextBox;
                    if (textBox != null)
                    {
                        var bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
                        if (bindingExpression != null)
                        {
                            bindingExpression.UpdateSource();
                        }
                    }
                };
            }

            return element;
        }
    }
}
