using Newtonsoft.Json;
using RtFileExplorer.Model.FileInformation.FileProperty;
using RtFileExplorer.ViewModel.Wpf.PathInformation;
using System.IO;
using System.Linq;
using Utility;

namespace RtFileExplorer.ViewModel.Wpf.Directory
{
    public partial class DirectoryViewModel
    {
        private void Refresh()
        {
            ClearPathes();

            var propFilepath = ExtraPropertiesFilepath;
            var exProps = new FileExtraPropertiesList.Json();
            if (File.Exists(propFilepath))
            {
                using (var reader = new StreamReader(new FileStream(propFilepath, FileMode.Open, FileAccess.Read)))
                {
                    exProps = JsonConvert.DeserializeObject<FileExtraPropertiesList.Json>(reader.ReadToEnd());
                }
            }

            SharedProperties.Initialize();
            SharedProperties.ImportFrom(exProps!.SharedProperties);

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
                    .Where(f => File.Exists(f))
                    .ForEach(f => AddPathInformation(new FileInformationViewModel(this, f.EnsureFileSystemPath())));

                _fileSystemWatcher = CreateNewWatcher(Directory);
            }

            foreach (var file in Pathes.OfType<FileInformationViewModel>())
            {
                if (exProps.Items.TryGetValue(file.Name, out var props))
                {
                    file.ExtraProperties.ImportFrom(props);
                }
            }

            FirePropertyChanged(nameof(PathCountText));
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
