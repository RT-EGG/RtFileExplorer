using RtFileExplorer.ViewModel.Wpf.PathInformation;
using System.Collections;
using System.IO;
using System.Linq;
using System.Windows;

namespace RtFileExplorer.ViewModel.Wpf.PathInformationList
{
    public partial class PathInformationListViewModel
    {
        class CutPathCommandClass : CommandBase<PathInformationListViewModel>
        {
            public CutPathCommandClass(PathInformationListViewModel inViewModel) 
                : base(inViewModel)
            { }

            public override bool CanExecute(object? parameter)
                => parameter is IEnumerable items
                && items.OfType<object>().All(item => item is PathInformationViewModel);

            public override void Execute(object? parameter)
            {
                var pathes = (parameter as IEnumerable)!
                        .OfType<PathInformationViewModel>()
                        .Select(path => path.Path).ToArray();

                var data = new DataObject(DataFormats.FileDrop, pathes);
                data.SetData("Preferred DropEffect",
                    new MemoryStream(new byte[] {
                        (byte)DragDropEffects.Move, 0, 0, 0
                    })
                );

                Clipboard.SetDataObject(data);
            }
        }
    }
}
