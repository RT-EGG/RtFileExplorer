using System.Diagnostics;

namespace RtFileExplorer.ViewModel.Wpf.Application
{
    public partial class ApplicationViewModel
    {
        internal class ShowDataDirectoryInExplorerCommandClass : CommandBase<ApplicationViewModel>
        {
            public ShowDataDirectoryInExplorerCommandClass(ApplicationViewModel inViewModel) 
                : base(inViewModel)
            { }

            public override bool CanExecute(object? parameter)
                => true;

            public override void Execute(object? parameter)
            {
                Process.Start(new ProcessStartInfo()
                {
                    UseShellExecute = true,
                    FileName = ApplicationDataDirectoryPath
                });
            }
        }
    }
}
