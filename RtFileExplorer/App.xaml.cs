﻿using RtFileExplorer.ViewModel.Wpf.Application;
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

            TargetApplication.InitializeApplication(this);
        }

        private ApplicationViewModel _viewModel;
    }
}
