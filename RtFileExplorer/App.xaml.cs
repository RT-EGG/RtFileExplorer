using RtFileExplorer.ViewModel.Wpf;
using RtFileExplorer.ViewModel.Wpf.Application;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RtFileExplorer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            _viewModel = new ApplicationViewModel();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += (sender, args) 
                => WriteExceptionLog((args.ExceptionObject as Exception)!);
            DispatcherUnhandledException += (sender, args)
                => WriteExceptionLog(args.Exception);
            TaskScheduler.UnobservedTaskException += (sender, args)
                => WriteExceptionLog(args.Exception);

            TargetApplication.InitializeApplication(this);
        }

        private void WriteExceptionLog(Exception e)
        {
            Logger.Instance.PushLog(e);
            Logger.Instance.WriteSoon();
        }

        private ApplicationViewModel _viewModel;
    }
}
