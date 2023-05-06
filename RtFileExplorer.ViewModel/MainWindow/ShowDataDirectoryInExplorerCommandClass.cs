using RtFileExplorer.ViewModel.Wpf.Application;
using System.Diagnostics;

namespace RtFileExplorer.ViewModel.Wpf.MainWindow
{
    public partial class MainWindowViewModel
    {
        class ShowDataDirectoryInExplorerCommandClass : CommandBase<MainWindowViewModel>
        {
            public ShowDataDirectoryInExplorerCommandClass(MainWindowViewModel inViewModel)
                : base(inViewModel)
            { }

            public override bool CanExecute(object? parameter)
                => true;

            public override void Execute(object? parameter)
            {
                Process.Start(new ProcessStartInfo()
                {
                    UseShellExecute = true,
                    FileName = ApplicationViewModel.ApplicationDataDirectoryPath
                });
            }
        }
    }
}
