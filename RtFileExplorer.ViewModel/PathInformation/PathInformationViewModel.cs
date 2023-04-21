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
            Path = inPath;

            RegisterPropertyNotification(_icon, nameof(Icon));
            RegisterPropertyNotification(_creationTime, nameof(CreationTime));
            RegisterPropertyNotification(_lastWriteTime, nameof(LastWriteTime));
            RegisterPropertyNotification(_lastAccessTime, nameof(LastAccessTime));
        }

        public BitmapSource? Icon => _icon.Value;
        public readonly string Path;
        public abstract string Name { set; get; }
        public virtual long? Size => null;
        public DateTime CreationTime => _creationTime.Value;
        public DateTime LastWriteTime => _lastWriteTime.Value;
        public DateTime LastAccessTime => _lastAccessTime.Value;

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
        private ReactiveProperty<DateTime> _creationTime = new ReactiveProperty<DateTime>();
        private ReactiveProperty<DateTime> _lastWriteTime = new ReactiveProperty<DateTime>();
        private ReactiveProperty<DateTime> _lastAccessTime = new ReactiveProperty<DateTime>();
    }
}
