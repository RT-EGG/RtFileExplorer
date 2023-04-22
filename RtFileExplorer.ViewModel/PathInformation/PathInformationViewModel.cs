using Reactive.Bindings;
using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace RtFileExplorer.ViewModel.Wpf.PathInformation
{
    public abstract class PathInformationViewModel : ViewModelBase
    {
        public PathInformationViewModel(string inPath)
        {
            RegisterPropertyNotification(_icon, nameof(Icon));
            RegisterPropertyNotification(_filepath, nameof(Path), nameof(Name));
            RegisterPropertyNotification(_creationTime, nameof(CreationTime));
            RegisterPropertyNotification(_lastWriteTime, nameof(LastWriteTime));
            RegisterPropertyNotification(_lastAccessTime, nameof(LastAccessTime));

            Path = inPath;
        }

        public BitmapSource? Icon => _icon.Value;
        public string Path
        {
            get => _filepath.Value;
            private set
            {
                if (IsNameChangeable && System.IO.Path.GetDirectoryName(value) is null)
                    throw new ArgumentException("Path must contains directory.");

                if (Path != value)
                {
                    _filepath.Value = value.Replace('\\', '/');
                }
            }
        }

        public string Name 
        {
            get => GetFileName(Path);
            set {
                if (Name == value)
                    return;

                if (ChangeName(value))
                {
                    _filepath.Value =
                        System.IO.Path.GetDirectoryName(_filepath.Value)!.TrimEnd('/')
                        + $"/{value}";
                }
            }
        }

        public virtual bool IsNameChangeable => true;

        public virtual long? Size => null;
        public DateTime CreationTime => _creationTime.Value;
        public DateTime LastWriteTime => _lastWriteTime.Value;
        public DateTime LastAccessTime => _lastAccessTime.Value;

        protected virtual string GetFileName(string inPath) => System.IO.Path.GetFileName(Path);
        protected abstract bool ChangeName(string inName);

        protected void SetFilepath(string inFilepath)
        {
            Path = inFilepath;
        }

        protected void UpdateInfo(FileSystemInfo inSource)
        {
            BackgroundTaskQueue.Instance.AddTask(new BackgroundTask
            {
                Method = async () =>
                {
                    _icon.Value = await FileIcons.Instance.GetAssociatedIcon(Path, FileIcons.IconSize.Small);
                },
                Async = true,
            });

            _creationTime.Value = inSource.CreationTime;
            _lastWriteTime.Value = inSource.LastWriteTime;
            _lastAccessTime.Value = inSource.LastAccessTime;
        }

        private ReactiveProperty<BitmapSource?> _icon = new ReactiveProperty<BitmapSource?>();
        private ReactiveProperty<string> _filepath = new ReactiveProperty<string>();
        private ReactiveProperty<DateTime> _creationTime = new ReactiveProperty<DateTime>();
        private ReactiveProperty<DateTime> _lastWriteTime = new ReactiveProperty<DateTime>();
        private ReactiveProperty<DateTime> _lastAccessTime = new ReactiveProperty<DateTime>();
    }
}
