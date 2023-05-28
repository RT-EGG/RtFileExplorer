﻿using RtFileExplorer.ViewModel.Wpf.PathInformation;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
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
                var filepathes = (parameter as IEnumerable)!
                                .OfType<PathInformationViewModel>()
                                .Select(item => item.Path).ToArray();

                var stream = new MemoryStream(new byte[]
                {
                    (byte)(DragDropEffects.Copy | DragDropEffects.Link),
                    0, 0, 0
                });
                var data = new DataObject(DataFormats.FileDrop, filepathes);
                data.SetData("Preferred DropEffect", stream);

                Clipboard.SetDataObject(data);
            }
        }
    }
}
