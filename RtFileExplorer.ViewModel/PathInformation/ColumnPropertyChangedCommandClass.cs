using Newtonsoft.Json;
using RtFileExplorer.Model.FileInformation;
using RtFileExplorer.ViewModel.Wpf.Application;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RtFileExplorer.ViewModel.Wpf.PathInformation
{
    public partial class PathInformationListViewModel
    {
        private void ExportLatestColumnStyles()
        {
            var json = new JsonFileInformationColumnSaveData();
            foreach (var column in _columns)
            {
                json.Items.Add(new JsonFileInformationColumn
                {
                    Type = column.Type,
                    Width = column.Width.Value,
                    IsVisible = column.IsVisible,
                    DisplayIndex = column.DisplayIndex,
                });
            }

            if (_sortDescription is not null)
            {
                json.Sorting = new JsonFileInformationColumnSorting
                {
                    PropertyName = _sortDescription.Value.PropertyName,
                    Direction = _sortDescription.Value.Direction switch
                    {
                        ListSortDirection.Ascending => SortDirection.Ascending,
                        ListSortDirection.Descending => SortDirection.Descending,
                        _ => throw new InvalidProgramException()
                    },
                };
            }

            var serialized = JsonConvert.SerializeObject(json, Formatting.Indented);

            ApplicationViewModel.EnsureApplicationDataDirectory();
            using (var writer = new StreamWriter(new FileStream(LatestColumnSaveFilepath, FileMode.OpenOrCreate, FileAccess.Write)))
            {
                writer.Write(serialized);
            }
        }

        class ColumnPropertyChangedCommandClass : AsyncCommandBase<PathInformationListViewModel>
        {
            public ColumnPropertyChangedCommandClass(PathInformationListViewModel inViewModel) 
                : base(inViewModel)
            { }

            public override bool CanExecute(object? parameter)
                => true;

            protected override async Task ExecuteAsync(object? parameter)
            {
                _cancellationToken?.Cancel();

                try
                {
                    _cancellationToken = new CancellationTokenSource();

                    await Task.Delay(500, _cancellationToken.Token);
                    ViewModel.ExportLatestColumnStyles();
                    _cancellationToken?.Dispose();

                }
                catch (OperationCanceledException)
                {

                }
                finally
                {
                    _cancellationToken = null;
                }
            }

            private CancellationTokenSource? _cancellationToken = null;
        }
    }
}
