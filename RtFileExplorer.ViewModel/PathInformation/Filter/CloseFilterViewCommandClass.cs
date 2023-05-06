using System.Windows;

namespace RtFileExplorer.ViewModel.Wpf.PathInformation.Filter
{
    public partial class PathInformationFilterViewModel
    {
        class CloseFilterViewCommandClass : CommandBase<PathInformationFilterViewModel>
        {
            public CloseFilterViewCommandClass(PathInformationFilterViewModel inViewModel) 
                : base(inViewModel)
            { }

            public override bool CanExecute(object? parameter)
                => ViewModel.ViewVisibility != Visibility.Collapsed;

            public override void Execute(object? parameter)
                => ViewModel.ViewVisibility = Visibility.Collapsed;
        }
    }
}
