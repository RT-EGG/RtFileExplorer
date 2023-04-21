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

        public override string Name 
        {
            get => System.IO.Path.GetFileName(Path);
            set 
            {

            } 
        }

        private void UpdateDirectoryInfo()
        {
            var info = new DirectoryInfo(Path);
            UpdateInfo(info);
        }
    }
}
