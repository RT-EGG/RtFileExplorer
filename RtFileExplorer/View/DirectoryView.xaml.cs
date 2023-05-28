using RtFileExplorer.Model.FileInformation;
using RtFileExplorer.View.Components;
using RtFileExplorer.View.Components.Columns;
using RtFileExplorer.ViewModel.Wpf.Directory;
using RtFileExplorer.ViewModel.Wpf.PathInformation;
using RtFileExplorer.ViewModel.Wpf.PathInformationList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Utility.Wpf.Extensions;

namespace RtFileExplorer.View
{
    /// <summary>
    /// DirectoryView.xaml の相互作用ロジック
    /// </summary>
    public partial class DirectoryView : UserControl
    {
        public DirectoryView()
        {
            InitializeComponent();
            
            foreach (var item in ContextMenu.Items.OfType<MenuItem>())
            {
                if (item is ColumnVisibilityMenuItem)
                    _columnVisibilityMenuItems.Add(item);
                else
                    _nonColumnVisibilityMenuItems.Add(item);
            }

            VerifyColumns(DataContext);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var column in DataGrid.Columns)
            {
                var header = column.GetHeader();
                if (header is not null)
                {
                    header.PreviewMouseUp += ColumnHeader_PreviewMouseUp;
                }
            }
        }

        private void ColumnHeader_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!(DataContext is PathInformationListViewModel viewModel))
                return;

            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    if (viewModel.ColumnPropertyChagedCommand.CanExecute(e))
                        viewModel.ColumnPropertyChagedCommand.Execute(e);
                    break;
            }
        }

        private void DataGrid_ColumnHeaderDragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (!(DataContext is PathInformationListViewModel viewModel))
                return;

            if (viewModel.ColumnPropertyChagedCommand.CanExecute(e))
                viewModel.ColumnPropertyChagedCommand.Execute(e);
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
            => VerifyColumns(e.NewValue);

        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is DataGridCell cell))
                return;
            if (!(cell.DataContext is PathInformationViewModel file))
                return;

            if (!(DataContext is DirectoryViewModel directory))
                return;

            var parameter = new object[] { file };
            if (directory.OpenPathCommand.CanExecute(parameter))
                directory.OpenPathCommand.Execute(parameter);
            return;
        }

        private void VerifyColumns(object inNewContext)
        {
            if (DataGrid is null)
                return;

            DataGrid.Columns.Clear();
            if (!(inNewContext is DirectoryViewModel context))
                return;

            foreach (FilePropertyItemType type in Enum.GetValues(typeof(FilePropertyItemType)))
            {
                var column = FileInformationDataColumn.CreateColumnFor(context, type);

                if (column is not null)
                    DataGrid.Columns.Add(column);
            }
        }

        private void DataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement)e.OriginalSource;

            var cell = element.FindAncestor<DataGridCell>();
            if (cell is null)
                return;

            if (e.ClickCount > 1)
            {
                e.Handled = true;

                if (!(cell.DataContext is PathInformationViewModel rowViewModel))
                    return;

                if (!(DataContext is DirectoryViewModel viewModel))
                    return;

                var parameter = new object[] { rowViewModel };
                if (viewModel.OpenPathCommand.CanExecute(parameter))
                    viewModel.OpenPathCommand.Execute(parameter);
            }
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var row = DataGrid.SelectedItem;
            if (!(row is PathInformationViewModel rowViewModel))
                return;

            if (!(DataContext is DirectoryViewModel viewModel))
                return;

            e.Handled = true;
            switch (e.Key)
            {
                case Key.Enter:
                    if (DataGrid.GetIsEditing())
                    {
                        DataGrid.CommitEdit();
                    }
                    else
                    {
                        var parameter = new object[] { rowViewModel };
                        if (viewModel.OpenPathCommand.CanExecute(parameter))
                            viewModel.OpenPathCommand.Execute(parameter);
                    }
                    break;

                default:
                    e.Handled = false;
                    break;
            }
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            switch (e.EditAction)
            {
                case DataGridEditAction.Commit:
                    if (e.EditingElement is TextBox textBox)
                    {
                        var expression = textBox.GetBindingExpression(TextBox.TextProperty);
                        if (expression is not null)
                            expression.UpdateSource();
                    }

                    break;
            }
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var row = DataGrid.SelectedItem;
            if (!(row is PathInformationViewModel rowViewModel))
                return;

            if (!(DataContext is DirectoryViewModel viewModel))
                return;

            bool isCtrlDown = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;

            e.Handled = true;
            switch (e.Key)
            {
                case Key.F2:
                    if (rowViewModel.IsNameChangeable)
                    {
                        var column = DataGrid.Columns
                                    .Where(c => c is IFileInformationDataColumn)
                                    .FirstOrDefault(c =>
                                        (c as IFileInformationDataColumn)!.ItemType == FilePropertyItemType.Name
                                    );
                        if (column is null)
                            break;

                        DataGrid.CurrentCell = new DataGridCellInfo(row, column);
                        DataGrid.BeginEdit();
                    }
                    break;

                case Key.F5:
                    if (viewModel.RefreshCommand.CanExecute(null))
                        viewModel.RefreshCommand.Execute(null);
                    break;

                case Key.F when isCtrlDown:
                    if (viewModel.OpenFilterViewCommand.CanExecute(null))
                        viewModel.OpenFilterViewCommand.Execute(null);
                    break;

                default:
                    e.Handled = false;
                    break;
            }
        }

        private void DataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var visibleCount = 0;
            if (e.OriginalSource is DependencyObject source)
            {
                // refered
                // https://stackoverflow.com/questions/59423764/context-menu-for-specific-dynamic-column-for-datagrid-wpf
                while (source is not null
                    && source is not DataGridCell
                    && source is not DataGridColumnHeader
                    && source is not DataGridRow)
                {
                    source = (source is Visual || source is Visual3D)
                        ? VisualTreeHelper.GetParent(source)
                        : LogicalTreeHelper.GetParent(source);
                }

                if (source is DataGridColumnHeader)
                {
                    _columnVisibilityMenuItems.ForEach(item => item.Visibility = Visibility.Visible);
                    _nonColumnVisibilityMenuItems.ForEach(item => item.Visibility = Visibility.Collapsed);
                    visibleCount = _columnVisibilityMenuItems.Count;
                }
                else
                {
                    _columnVisibilityMenuItems.ForEach(item => item.Visibility = Visibility.Collapsed);
                    _nonColumnVisibilityMenuItems.ForEach(item =>
                    {
                        if (item.Command is not null)
                        {
                            item.Visibility = item.Command.CanExecute(DataGrid.SelectedItems)
                                    ? Visibility.Visible : Visibility.Collapsed;
                        }
                        else
                        {
                            item.Visibility = Visibility.Visible;
                        }
                    });
                    visibleCount = _nonColumnVisibilityMenuItems.Count;
                }
            }

            e.Handled = visibleCount == 0;
        }

        private List<MenuItem> _columnVisibilityMenuItems = new List<MenuItem>();
        private List<MenuItem> _nonColumnVisibilityMenuItems = new List<MenuItem>();
    }
}
