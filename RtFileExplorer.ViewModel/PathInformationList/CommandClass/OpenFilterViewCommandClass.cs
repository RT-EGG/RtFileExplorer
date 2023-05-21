using System.Windows;

namespace RtFileExplorer.ViewModel.Wpf.PathInformationList
{
    public partial class PathInformationListViewModel
    {
        class OpenFilterViewCommandClass : CommandBase<PathInformationListViewModel>
        {
            public OpenFilterViewCommandClass(PathInformationListViewModel inViewModel)
                : base(inViewModel)
            { }

            public override bool CanExecute(object? parameter)
                => ViewModel.FilterViewModel.ViewVisibility != Visibility.Visible;

            public override void Execute(object? parameter)
                => ViewModel.FilterViewModel.ViewVisibility = Visibility.Visible;
        }
    }
}
