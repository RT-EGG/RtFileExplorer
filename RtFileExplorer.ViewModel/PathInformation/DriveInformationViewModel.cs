using System;

namespace RtFileExplorer.ViewModel.Wpf.PathInformation
{
    public class DriveInformationViewModel : DirectoryInformationViewModel
    {
        public DriveInformationViewModel(string inPath)
            : base(inPath)
        {

        }

        public override string Name 
        {
            get => Path;
            set => throw new NotImplementedException(); 
        }
    }
}
