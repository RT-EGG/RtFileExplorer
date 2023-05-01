using RtFileExplorer.Model.FileInformation;
using RtFileExplorer.ViewModel.Wpf.PathInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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

            if (element is TextBlock textBlock)
            {
                textBlock.LostFocus += (sender, e) =>
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

                textBlock.SetBinding(TextBox.VisibilityProperty, new Binding
                {
                    Path = new PropertyPath(string.Empty),
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.Explicit,
                    Converter = new FileInformationDataColumn.SupportedPathConverter(),
                    ConverterParameter = PathInformationColumnViewModel.GetPropertyName(ItemType),
                });
            }

            return element;
        }
    }
}
