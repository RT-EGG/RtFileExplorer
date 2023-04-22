using System;
using System.IO;
using Utility.Wpf;

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
            if (System.IO.Directory.Exists(inPath))
            {
                Messages.ShowErrorMessage($"\"{inPath}\"は既に存在します。");
                return false;
            }

            try
            {
                System.IO.Directory.Move(Path, inPath);

            }
            catch (Exception e)
            {
                Messages.ShowErrorMessage($"Error occued, {e.GetType()}.{Environment.NewLine}{e.Message}");
                return false;
            }

            return true;
        }

        private void UpdateDirectoryInfo()
        {
            var info = new DirectoryInfo(Path);
            UpdateInfo(info);
        }
    }
}
