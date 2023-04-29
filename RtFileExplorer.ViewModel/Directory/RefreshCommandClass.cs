using RtFileExplorer.ViewModel.Wpf.PathInformation;
using System.Linq;
using Utility;

namespace RtFileExplorer.ViewModel.Wpf.Directory
{
    public partial class DirectoryViewModel
    {
        private void Refresh()
        {
            ClearPathes();
            if (string.IsNullOrEmpty(Directory))
            {
                // PC表示（ドライブリスト）
                System.IO.DriveInfo.GetDrives()
                    .Where(drive => drive.IsReady)
                    .ForEach(drive => AddPathInformation(new DriveInformationViewModel(drive.Name)));
            }
            else
            {
                if (!System.IO.Directory.Exists(Directory))
                    return;

                System.IO.Directory.GetDirectories(Directory)
                    .Where(directory => System.IO.Directory.Exists(directory))
                    .ForEach(directory => AddPathInformation(new DirectoryInformationViewModel(directory.EnsureFileSystemPath())));
                System.IO.Directory.GetFiles(Directory)
                    .Where(f => System.IO.File.Exists(f))
                    .ForEach(f => AddPathInformation(new FileInformationViewModel(this, f.EnsureFileSystemPath())));

                _fileSystemWatcher = CreateNewWatcher(Directory);
            }
        }

        class RefreshCommandClass : CommandBase<DirectoryViewModel>
        {
            public RefreshCommandClass(DirectoryViewModel inViewModel) 
                : base(inViewModel)
            { }

            public override bool CanExecute(object? parameter)
                => true;

            public override void Execute(object? parameter)
                => ViewModel.Refresh();
        }
    }
}
