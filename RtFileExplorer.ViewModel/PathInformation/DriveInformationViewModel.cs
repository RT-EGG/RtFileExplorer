using System;

namespace RtFileExplorer.ViewModel.Wpf.PathInformation
{
    public class DriveInformationViewModel : DirectoryInformationViewModel
    {
        public DriveInformationViewModel(string inPath)
            : base(inPath)
        {

        }

        public override bool IsNameChangeable => false;

        protected override string GetFileName(string inPath) => inPath;
        protected override bool ChangeName(string inName)
            => throw new InvalidProgramException();
    }
}
