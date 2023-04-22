using Reactive.Bindings;
using RtFileExplorer.ViewModel.Wpf.PathInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Utility;

namespace RtFileExplorer.ViewModel.Wpf.Directory
{
    public partial class DirectoryViewModel : PathInformationListViewModel
    {
        public DirectoryViewModel()
        {
            _directory.Subscribe(d =>
            {
                FirePropertyChanged(nameof(Directory), nameof(DirectoryPathes));

                ClearPathes();
                if (string.IsNullOrEmpty(d))
                {
                    // PC表示（ドライブリスト）
                    System.IO.DriveInfo.GetDrives()
                        .Where(drive => drive.IsReady)
                        .ForEach(drive => AddPathInformation(new DriveInformationViewModel(drive.Name)));
                }
                else
                {
                    if (!System.IO.Directory.Exists(d))
                        return;

                    if (!d.EndsWith("\\"))
                        d += "\\";

                    System.IO.Directory.GetDirectories(d)
                        .Where(directory => System.IO.Directory.Exists(directory))
                        .ForEach(directory => AddPathInformation(new DirectoryInformationViewModel(directory)));
                    System.IO.Directory.GetFiles(d)
                        .Where(f => System.IO.File.Exists(f))
                        .ForEach(f => AddPathInformation(new FileInformationViewModel(f)));
                }
            });

            Directory = "";

            OpenPathCommand = new OpenPathCommandClass(this);
        }

        public string Directory
        {
            get => _directory.Value;
            set
            {
                if (Directory != value)
                {
                    _directory.Value = value;
                }
            }
        }

        public IEnumerable<string> DirectoryPathes
        {
            get => Directory.Replace('\\', '/').Split('/');
            set => Directory = string.Join("/", value);
        }

        public ICommand OpenPathCommand { get; }

        private ReactiveProperty<string> _directory = new ReactiveProperty<string>("");
    }
}
