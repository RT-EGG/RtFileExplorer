using Newtonsoft.Json;
using Reactive.Bindings;
using RtFileExplorer.Model.FileInformation.FileProperty;
using RtFileExplorer.ViewModel.Wpf.PathInformation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
                _fileSystemWatcher?.Dispose();

                FirePropertyChanged(nameof(Directory), nameof(DirectoryPathes));

                Refresh();
            });

            Directory = "";

            OpenPathCommand = new OpenPathCommandClass(this);
            RefreshCommand = new RefreshCommandClass(this);
        }

        public string Directory
        {
            get => _directory.Value;
            set
            {
                if (Directory != value)
                {
                    ExportPropertiesFile();

                    _directory.Value = value;
                }
            }
        }

        public IEnumerable<string> DirectoryPathes
        {
            get => Directory.Replace('\\', '/').Split('/', StringSplitOptions.RemoveEmptyEntries);
            set => Directory = string.Join("/", value);
        }

        public ICommand OpenPathCommand { get; }
        public ICommand RefreshCommand { get; }
        internal FileSharedProperties SharedProperties { get; } = new FileSharedProperties();

        private FileSystemWatcher CreateNewWatcher(string inDirectoryPath)
        {
            var result = new FileSystemWatcher(inDirectoryPath, "");

            result.Changed += OnFileChanged;
            result.Created += OnFileCreated;
            result.Deleted += OnFileDeleted;
            result.Renamed += OnFileRenamed;
            result.Error += OnFileSystemWatcherError;

            result.EnableRaisingEvents = true;

            return result;
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Renamed)
                throw new InvalidProgramException();

            var oldPath = e.OldFullPath.EnsureFileSystemPath();
            if (TryGetFirst(item => item.Path == oldPath, out var item) && item is not null)
            {
                var newPath = e.FullPath.EnsureFileSystemPath();
                item.NotifyPathChanged(newPath);
            }
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Deleted)
                throw new InvalidProgramException();

            RemovePathInformation(e.FullPath.EnsureFileSystemPath());
            FirePropertyChanged(nameof(PathCountText));
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Created)
                throw new InvalidProgramException();

            var path = e.FullPath.EnsureFileSystemPath();
            if (System.IO.File.Exists(path))
            {
                AddPathInformation(new FileInformationViewModel(this, path));
            }
            else if (System.IO.Directory.Exists(e.FullPath))
            {
                AddPathInformation(new DirectoryInformationViewModel(path));
            }
            else
            {
                throw new InvalidProgramException();
            }
            FirePropertyChanged(nameof(PathCountText));
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
                throw new InvalidProgramException();

            var path = e.FullPath.EnsureFileSystemPath();
            if (TryGetFirst(item => item.Path == path, out var item) && item is not null)
            {
                item.UpdateInformation();
            }
        }

        private void OnFileSystemWatcherError(object sender, ErrorEventArgs e)
        {
            Debug.WriteLine(e.GetException().GetType().Name);
            Debug.WriteLine(e.GetException().Message);
        }

        private void ExportPropertiesFile()
        {
            var filepath = ExtraPropertiesFilepath;

            var items = new Dictionary<string, FileExtraProperties.Json>(
                Pathes.OfType<FileInformationViewModel>()
                .Select(path => new KeyValuePair<string, FileExtraProperties.Json?>(
                    path.Name, path.ExtraProperties?.Export()
                ))
                .Where(item => item.Value is not null)
                .Select(item => new KeyValuePair<string, FileExtraProperties.Json>(
                    item.Key, item.Value!
                ))
            );

            if (!items.Any())
            {
                if (File.Exists(filepath))
                    File.Delete(filepath);
                return;
            }

            var json = new FileExtraPropertiesList.Json()
            {
                SharedProperties = SharedProperties.Export(),
                Items = items
            };

            using (var writer = new StreamWriter(new FileStream(filepath, FileMode.Create, FileAccess.Write)))
            {
                writer.Write(
                    JsonConvert.SerializeObject(json, Formatting.Indented)
                );                
            }
        }

        private const string ExtraPropertiesFilename = "_props.json";
        private string ExtraPropertiesFilepath => $"{Directory}/{ExtraPropertiesFilename}";

        private ReactiveProperty<string> _directory = new ReactiveProperty<string>("");
        private FileSystemWatcher? _fileSystemWatcher = null;
    }
}
