﻿using RtFileExplorer.Converter;
using RtFileExplorer.Model.FileInformation;
using RtFileExplorer.ViewModel.Wpf.PathInformation;
using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace RtFileExplorer.View.Columns
{
    internal interface IFileInformationDataColumn
    {
        FilePropertyItemType ItemType { get; }
    }

    internal static class FileInformationDataColumn
    {
        public static DataGridColumn? CreateColumnFor(PathInformationListViewModel inParentViewModel, FilePropertyItemType inType)
        {
            var att = inType.GetFilePropertyAttribute();
            if (!att.IsSupported)
                return null;

            DataGridColumn? result = att.DataType switch
            {
                FilePropertyDataType.Icon
                    => new FileInformationImageDataColumn(inType),
                FilePropertyDataType.String or
                FilePropertyDataType.Integer or
                FilePropertyDataType.DateTime
                    => new FileInformationTextDataColumn(inType),
                _ => null
            };
            if (result is null)
                return null;

            result.IsReadOnly = att.IsReadOnly;
            result.CanUserReorder = att.IsReordable;
            result.CanUserResize = att.IsResizable;
            result.Header = att.DisplayText;

            var viewModel = inParentViewModel.GetColumn(inType);
            if (viewModel is not null)
            {
                BindingOperations.SetBinding(result, DataGridColumn.WidthProperty, 
                    new Binding(nameof(PathInformationColumnViewModel.Width))
                {
                    Source = viewModel,
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

                BindingOperations.SetBinding(result, DataGridColumn.VisibilityProperty,
                    new Binding(nameof(PathInformationColumnViewModel.Visibility))
                {
                    Source = viewModel,
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger= UpdateSourceTrigger.PropertyChanged
                });
            }

            return result;
        }

        public static BindingBase CreateBinding(FilePropertyItemType inType)
        {
            var att = inType.GetFilePropertyAttribute();

            var result = new Binding();
            result.Path = PathInformationColumnViewModel.GetPropertyPath(inType);
            result.Mode = att.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            result.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            if (att.ColumnValueConverter is not null)
            {
                result.Converter = new FilePropertyConverter(
                    (Activator.CreateInstance(att.ColumnValueConverter)
                        as RtFileExplorer.Model.ValueConverter.IValueConverter)!
                );
            }

            return result;
        }
    }    
}
