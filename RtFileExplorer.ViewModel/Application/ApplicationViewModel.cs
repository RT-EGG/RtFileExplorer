using System;
using System.IO;

namespace RtFileExplorer.ViewModel.Wpf.Application
{
    using ApplicationModel = Model.Application.Application;
    public partial class ApplicationViewModel : ViewModelBase<ApplicationModel>
    {
        public ApplicationViewModel()
        {
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

        public static string ApplicationDataDirectoryPath
            => Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RT-EGG", "RtFileExplorer");

        private IDisposable? _loggerThread;
    }
}
