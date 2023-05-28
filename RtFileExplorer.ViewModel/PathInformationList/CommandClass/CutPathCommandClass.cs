﻿using RtFileExplorer.ViewModel.Wpf.PathInformation;
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
                && items.OfType<object>().Any()
                && items.OfType<object>().All(item => (item is PathInformationViewModel) && (item is not DriveInformationViewModel));

            public override void Execute(object? parameter)
            {
                var filepathes = (parameter as IEnumerable)!
                                .OfType<PathInformationViewModel>()
                                .Select(item => item.Path).ToArray();

                var stream = new MemoryStream(new byte[]
                {
                    (byte)DragDropEffects.Move,
                    0, 0, 0
                });
                var data = new DataObject(DataFormats.FileDrop, filepathes);
                data.SetData("Preferred DropEffect", stream);

                Clipboard.SetDataObject(data);
            }
        }
    }
}
