using RtFileExplorer.Model.FileInformation;
using RtFileExplorer.ViewModel.Wpf.PathInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RtFileExplorer.View.Components
{
    internal class ColumnVisibilityMenuItem : MenuItem
    {
        public FilePropertyItemType? ItemType
        {
            get => (FilePropertyItemType?)GetValue(ItemTypeProperty);
            set => SetValue(ItemTypeProperty, value);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property.Name == DataContextProperty.Name)
                ResetBindings();
        }

        private void RefreshItemType()
            => ResetBindings();

        private void ResetBindings()
        {
            if (ItemType is null || DataContext is not PathInformationListViewModel viewModel)
                return;

            var type = ItemType.Value;
            var itemAtt = type.GetFilePropertyAttribute();
            var column = viewModel.GetColumn(type);

            Header = itemAtt.DisplayText;
            Command = viewModel.SwitchColumnVisibilityCommand;
            CommandParameter = type;

            BindingOperations.SetBinding(this, IsCheckedProperty,
                new Binding(nameof(PathInformationColumnViewModel.IsVisible))
                {
                    Source = column,
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                });
        }

        private static void OnItemTypeChanged(DependencyObject inSender, DependencyPropertyChangedEventArgs inArgs)
        {
            if (inSender is not ColumnVisibilityMenuItem sender)
                throw new ArgumentException("", nameof(inSender));

            sender.RefreshItemType();
        }
       
        public static readonly DependencyProperty ItemTypeProperty = DependencyProperty.Register(
                "ItemType", typeof(FilePropertyItemType?), typeof(ColumnVisibilityMenuItem),
                new PropertyMetadata(null, OnItemTypeChanged)
            );
    }
}
