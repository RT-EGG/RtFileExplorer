using System.IO;

namespace RtFileExplorer.ViewModel.Wpf.PathInformation
{
    public class DirectoryInformationViewModel : PathInformationViewModel
    {
        public DirectoryInformationViewModel(string inPath) 
            : base(inPath)
        {
            UpdateDirectoryInfo();
        }

        protected override bool ChangePath(string inPath)
        {
            return false;
        }

        private void UpdateDirectoryInfo()
        {
            var info = new DirectoryInfo(Path);
            UpdateInfo(info);
        }
    }
}
