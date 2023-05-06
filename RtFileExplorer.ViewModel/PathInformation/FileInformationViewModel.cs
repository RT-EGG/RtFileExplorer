using Reactive.Bindings;
using RtFileExplorer.Model.FileInformation.FileProperty;
using RtFileExplorer.ViewModel.Wpf.Directory;
using System;
using System.IO;
using Utility.Wpf;

namespace RtFileExplorer.ViewModel.Wpf.PathInformation
{
    public partial class FileInformationViewModel : PathInformationViewModel
    {
        public FileInformationViewModel(DirectoryViewModel inParent, string inFilepath)
            : base(inFilepath)
        {
            Parent = inParent;
            _extraProperties = new FileExtraProperties(Parent.SharedProperties);

            RegisterPropertyNotification(_fileSize, nameof(Size));
            RegisterPropertyNotification(_extraProperties.Rating, nameof(Rating));

            UpdateInformation();
        }

        public void Execute()
        {
            FileExecutor.Instance.Execute(Path);
        }

        public override PathType PathType => PathType.File;

        public override bool GetIsSupported(string inPropertyName)
        {
            return inPropertyName switch
            {
                nameof(Size) or
                nameof(Rating)
                    => true,

                _ => base.GetIsSupported(inPropertyName)
            };
        }

        public override long? Size => _fileSize.Value;
        public override uint? Rating 
        { 
            get => _extraProperties.Rating.Value;
            set => _extraProperties.Rating.Value = value; 
        }

        public FileExtraProperties ExtraProperties => _extraProperties;

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

        private readonly DirectoryViewModel Parent;
        private ReactiveProperty<long> _fileSize = new ReactiveProperty<long>();
        private FileExtraProperties _extraProperties;
    }
}
