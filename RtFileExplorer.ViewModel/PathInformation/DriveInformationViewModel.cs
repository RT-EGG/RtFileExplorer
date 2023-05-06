using System;

namespace RtFileExplorer.ViewModel.Wpf.PathInformation
{
    public class DriveInformationViewModel : DirectoryInformationViewModel
    {
        public DriveInformationViewModel(string inPath)
            : base(inPath)
        {

        }

        public override PathType PathType => PathType.Drive;
        public override bool IsNameChangeable => false;

        protected override string GetFileName(string inPath) => inPath;
        protected override bool ChangePath(string inName)
            => throw new InvalidProgramException();
    }
}
