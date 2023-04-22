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
                if (!System.IO.Directory.Exists(d))
                    return;

                if (!d.EndsWith("\\"))
                    d += "\\";

                System.IO.Directory.GetDirectories(d)
                    .Where(d => System.IO.Directory.Exists(d))
                    .ForEach(d => AddDirectory(d));
                System.IO.Directory.GetFiles(d)
                    .Where(f => System.IO.File.Exists(f))
                    .ForEach(f => AddFile(f));
            });

            Directory = @"Z:\hoge\cmc";

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
