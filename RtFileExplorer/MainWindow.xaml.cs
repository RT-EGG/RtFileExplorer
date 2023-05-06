using RtFileExplorer.ViewModel.Wpf.Application;
using RtFileExplorer.ViewModel.Wpf.MainWindow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RtFileExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.Loaded += (_, _) => TargetApplication.Instance.AddFrameworkElement(this);
                this.Unloaded += (_, _) =>
                {
                    TargetApplication.Instance.RemoveFrameworkElement(this);
                    if (DataContext is IDisposable d)
                        d.Dispose();
                };

                this.ContentRendered += MainWindow_ContentRendered;
            }
        }

        private void MainWindow_ContentRendered(object? sender, EventArgs e)
        {
            this.ContentRendered -= MainWindow_ContentRendered;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (DataContext is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
