using System.Windows.Input;

namespace RtFileExplorer.ViewModel.Wpf.MainWindow
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            ShowDataDirectoryInExplorerCommand = new ShowDataDirectoryInExplorerCommandClass(this);
        }

        public ICommand ShowDataDirectoryInExplorerCommand { get; }
    }
}
