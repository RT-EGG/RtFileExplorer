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
        }

        public override long? Size => _fileSize.Value;

        public void Execute()
        {
            FileExecutor.Instance.Execute(Path);
        }

        protected override bool ChangePath(string inPath)
        {
            if (File.Exists(inPath))
            {
                Messages.ShowErrorMessage($"\"{inPath}\"は既に存在します。");
                return false;
            }

            try
            {
                File.Move(Path, inPath);

            }
            catch (Exception e)
            {
                Messages.ShowErrorMessage($"Error occued, {e.GetType()}.{Environment.NewLine}{e.Message}");
                return false;
            }

            return true;    
        }

        protected override void UpdateInformationCore()
        {
            var info = new FileInfo(Path);
            UpdateCommonInformations(info);

            _fileSize.Value = info.Length;
        }

        private ReactiveProperty<long> _fileSize = new ReactiveProperty<long>();
    }
}
