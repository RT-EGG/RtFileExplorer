﻿using Reactive.Bindings;
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

                    System.IO.Directory.GetDirectories(d)
                        .Where(directory => System.IO.Directory.Exists(directory))
                        .ForEach(directory => AddPathInformation(new DirectoryInformationViewModel(directory.EnsureFileSystemPath())));
                    System.IO.Directory.GetFiles(d)
                        .Where(f => System.IO.File.Exists(f))
                        .ForEach(f => AddPathInformation(new FileInformationViewModel(f.EnsureFileSystemPath())));

                    _fileSystemWatcher = CreateNewWatcher(d);
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
            get => Directory.Replace('\\', '/').Split('/', StringSplitOptions.RemoveEmptyEntries);
            set => Directory = string.Join("/", value);
        }

        public ICommand OpenPathCommand { get; }

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
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Created)
                throw new InvalidProgramException();

            var path = e.FullPath.EnsureFileSystemPath();
            if (System.IO.File.Exists(path))
            {
                AddPathInformation(new FileInformationViewModel(path));
            }
            else if (System.IO.Directory.Exists(e.FullPath))
            {
                AddPathInformation(new DirectoryInformationViewModel(path));
            }
            else
            {
                throw new InvalidProgramException();
            }
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

        private ReactiveProperty<string> _directory = new ReactiveProperty<string>("");
        private FileSystemWatcher? _fileSystemWatcher = null;
    }
}
