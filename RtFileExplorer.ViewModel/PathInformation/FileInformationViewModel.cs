using Reactive.Bindings;
using System.IO;

namespace RtFileExplorer.ViewModel.Wpf.PathInformation
{
    public class FileInformationViewModel : PathInformationViewModel
    {
        public FileInformationViewModel(string inFilepath)
            : base(inFilepath)
        {
            RegisterPropertyNotification(_fileSize, nameof(Size));

            UpdateFileInfo();
        }

        public override string Name
        {
            get => System.IO.Path.GetFileName(Path);
            set
            {

            }
        }
        public override long? Size => _fileSize.Value;

        public void Execute()
        {
            FileExecutor.Instance.Execute(Path);
        }

        private void UpdateFileInfo()
        {
            var info = new FileInfo(Path);
            UpdateInfo(info);

            _fileSize.Value = info.Length;
        }

        private ReactiveProperty<long> _fileSize = new ReactiveProperty<long>();
    }
}
