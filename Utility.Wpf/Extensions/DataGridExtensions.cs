using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Utility.Wpf.Extensions
{
    public static class DataGridExtensions
    {
        public static DataGridCell? GetCell(this DataGrid inDataGrid, DataGridCellInfo inCellInfo)
        {
            if (!inCellInfo.IsValid)
                return null;

            DataGridRow row = (DataGridRow)inDataGrid.ItemContainerGenerator.ContainerFromItem(inCellInfo.Item);
            if (row == null)
                return null;

            int columnIndex = inDataGrid.Columns.IndexOf(inCellInfo.Column);
            if (columnIndex < 0)
                return null;

            DataGridCellsPresenter? presenter = row.GetVisualChild<DataGridCellsPresenter>();
            if (presenter is null)
                return null;

            return (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
        }

        public static bool GetIsEditing(this DataGrid inDataGrid)
        {
            var currentCell = inDataGrid.GetCell(inDataGrid.CurrentCell);
            if (currentCell is null)
                return false;

            return currentCell.IsEditing;
        }
    }
}
