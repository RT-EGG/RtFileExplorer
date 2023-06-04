using RtFileExplorer.ViewModel.Wpf;
using RtFileExplorer.ViewModel.Wpf.Utils;
using System;

namespace RtFileExplorer.View.Dialogs
{
    internal class Dialogs : ITargetApplicationDialogs
    {
        public IDisposable ShowProgressDialog(ProgressViewModel inViewModel)
        {
            var dialog = new ProgressDialog();
            dialog.DataContext = inViewModel;

            dialog.Show();
            return new WindowCloser(dialog);
        }
    }
}
