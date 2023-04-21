using RtFileExplorer.Model.FileInformation;
using RtFileExplorer.View.Columns;
using RtFileExplorer.ViewModel.Wpf.Directory;
using RtFileExplorer.ViewModel.Wpf.PathInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            
            VerifyColumns(DataContext);
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
    }
}
