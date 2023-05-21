using RtFileExplorer.ViewModel.Wpf.PathInformation;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace RtFileExplorer.ViewModel.Wpf.PathInformationList
{
    public partial class PathInformationListViewModel
    {
        class CopyPathCommandClass : CommandBase<PathInformationListViewModel>
        {
            public CopyPathCommandClass(PathInformationListViewModel inViewModel) 
                : base(inViewModel)
            { }

            public override bool CanExecute(object? parameter)
                => parameter is IEnumerable items
                && items.OfType<object>().All(item => item is PathInformationViewModel);

            public override void Execute(object? parameter)
            {
                var collection = new StringCollection();
                collection.AddRange(
                    (parameter as IEnumerable)!
                        .OfType<PathInformationViewModel>()
                        .Select(path => path.Path).ToArray()
                );

                Clipboard.SetFileDropList(collection);
            }
        }
    }
}
