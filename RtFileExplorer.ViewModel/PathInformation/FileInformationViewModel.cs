using Reactive.Bindings;
using System;
using System.IO;
using Utility.Wpf;

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

        public override long? Size => _fileSize.Value;

        public void Execute()
        {
            FileExecutor.Instance.Execute(Path);
        }

        protected override bool ChangeName(string inName)
        {
            if (File.Exists(inName))
            {
                Messages.ShowErrorMessage($"\"{inName}\"は既に存在します。");
                return false;
            }

            try
            {
                File.Move(Path, inName);

            }
            catch (Exception e)
            {
                Messages.ShowErrorMessage($"Error occued, {e.GetType()}.{Environment.NewLine}{e.Message}");
            }

            return true;    
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
