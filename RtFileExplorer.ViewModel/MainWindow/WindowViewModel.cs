using RtFileExplorer.ViewModel.Wpf.Directory;
using System.Windows.Input;
using Utility;

namespace RtFileExplorer.ViewModel.Wpf.MainWindow
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            ShowDataDirectoryInExplorerCommand = new ShowDataDirectoryInExplorerCommandClass(this);

            Directories.Add(new DirectoryViewModel());
        }

        public ICommand ShowDataDirectoryInExplorerCommand { get; }

        public ReactiveCollection<DirectoryViewModel> Directories
        { get; } = new ReactiveCollection<DirectoryViewModel>();
    }
}
