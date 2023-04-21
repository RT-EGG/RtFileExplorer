using RtFileExplorer.ViewModel.Wpf.PathInformation;
using System;
using System.Collections;
using System.Linq;

namespace RtFileExplorer.ViewModel.Wpf.Directory
{
    public partial class DirectoryViewModel
    {
        internal class OpenPathCommandClass : CommandBase<DirectoryViewModel>
        {
            public OpenPathCommandClass(DirectoryViewModel inViewModel)
                : base(inViewModel)
            { }

            public override bool CanExecute(object? parameter)
                => parameter is IEnumerable items
                && (
                    items.OfType<object>().All(item => item is FileInformationViewModel)
                    || items.OfType<object>().All(item => item is DirectoryInformationViewModel)
                );

            public override void Execute(object? parameter)
            {
                if (!(parameter is IEnumerable items))
                    throw new InvalidProgramException();

                if (items.OfType<object>().All(item => item is FileInformationViewModel))
                {
                    foreach (FileInformationViewModel file in items)
                    {
                        file.Execute();
                    }
                }
                else if (items.OfType<object>().All(item => item is DirectoryInformationViewModel))
                {
                    var directory = items.OfType<DirectoryInformationViewModel>().First();
                    ViewModel.Directory = directory.Path;
                }
                else
                {
                    throw new InvalidProgramException();
                }
            }
        }
    }
}
