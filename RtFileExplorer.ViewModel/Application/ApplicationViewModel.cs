using System;
using System.IO;
using System.Windows.Input;

namespace RtFileExplorer.ViewModel.Wpf.Application
{
    using ApplicationModel = Model.Application.Application;
    public partial class ApplicationViewModel : ViewModelBase<ApplicationModel>
    {
        public ApplicationViewModel()
        {
            ShowDataDirectoryInExplorerCommand = new ShowDataDirectoryInExplorerCommandClass(this);

            BackgroundTaskQueue.Instance.Start();

            _loggerThread = Logger.Instance.StartWriteThread();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_loggerThread is not null)
            {
                _loggerThread.Dispose();
                _loggerThread = null;
            }
        }

        public ICommand ShowDataDirectoryInExplorerCommand { get; }

        public static string ApplicationDataDirectoryPath
            => Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RT-EGG", "RtFileExplorer");

        private IDisposable? _loggerThread;
    }
}
