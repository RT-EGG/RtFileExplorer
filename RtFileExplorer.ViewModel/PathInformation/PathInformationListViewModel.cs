using Newtonsoft.Json;
using RtFileExplorer.Model.FileInformation;
using RtFileExplorer.ViewModel.Wpf.Application;
using RtFileExplorer.ViewModel.Wpf.PathInformation.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Utility;

namespace RtFileExplorer.ViewModel.Wpf.PathInformation
{
    public partial class PathInformationListViewModel : ViewModelBase
    {
        public PathInformationListViewModel()
        {
            SortingCommand = new SortingCommandClass(this);
            OpenFilterViewCommand = new OpenFilterViewCommandClass(this);
            ColumnPropertyChagedCommand = new ColumnPropertyChangedCommandClass(this);
            SwitchColumnVisibilityCommand = new SwitchColumnVisibilityCommandClass(this);
            FilterViewModel = new PathInformationFilterViewModel(_collectionViewSource);

            _collectionViewSource.Source = _pathes;
            _collectionViewSource.IsLiveSortingRequested = true;
            _collectionViewSource.IsLiveFilteringRequested = true;

            BindingOperations.EnableCollectionSynchronization(_pathes, new object());

            // create columns
            foreach (var (i, type) in Enum.GetValues<FilePropertyItemType>().Indexed())
            {
                var column = new PathInformationColumnViewModel(this, type);
                var model = new FileInformationColumn(type);

                column.BindModel(model);
                column.DisplayIndex = i;

                _columns.Add(column);
            }

            var sorting = (
                Name: nameof(PathInformationViewModel.Name), 
                Direction: ListSortDirection.Ascending
            );
            if (File.Exists(LatestColumnSaveFilepath))
            {
                var latest = (JsonFileInformationColumnSaveData?)null;
                using (var reader = new StreamReader(new FileStream(LatestColumnSaveFilepath, FileMode.Open, FileAccess.Read)))
                    latest = JsonConvert.DeserializeObject<JsonFileInformationColumnSaveData>(reader.ReadToEnd());

                if (latest is not null)
                {
                    var columnCount = _columns.Count();
                    foreach (var column in _columns)
                    {
                        if (!latest.Items.TryGetFirst(item => item.Type == column.Type, out var j))
                            continue;

                        var json = j!;
                        column.Width = json.Width;
                        column.IsVisible = json.IsVisible;

                        if (json.DisplayIndex.InRange(0, columnCount - 1))
                            column.DisplayIndex = json.DisplayIndex;
                        else
                            Logger.Instance.PushLog(new Logger.LogObject
                            {
                                Level = Logger.LogLevel.Error,
                                Message = $"カラムインデックスが範囲外です. ({json.DisplayIndex} : {columnCount})"
                            });
                    }

                    if (latest.Sorting is not null)
                    {
                        sorting.Name = latest.Sorting.PropertyName;
                        sorting.Direction = latest.Sorting.Direction switch
                        {
                            SortDirection.Ascending => ListSortDirection.Ascending,
                            SortDirection.Descending => ListSortDirection.Descending,
                            _ => throw new InvalidOperationException()
                        };
                    }
                }
            }

            ChangeSort(sorting.Name, sorting.Direction);
        }

        public ICommand SortingCommand { get; }
        public ICommand OpenFilterViewCommand { get; }
        public ICommand ColumnPropertyChagedCommand { get; }
        public ICommand SwitchColumnVisibilityCommand { get; }
        public PathInformationFilterViewModel FilterViewModel { get; }

        public void AddPathInformation(PathInformationViewModel inValue)
            => _pathes.Add(inValue);

        public void RemovePathInformation(string inPath)
            => _pathes.RemoveAll(item => item.Path == inPath);

        public void ClearPathes()
            => _pathes.Clear();

        public IEnumerable<PathInformationViewModel> Pathes => _pathes;

        public string PathCountText
        {
            get
            {
                var fileCount = Pathes.Count(path => path is FileInformationViewModel);
                var directoryCount = Pathes.Count(path => path is DirectoryInformationViewModel);

                var result = "";
                if (fileCount > 0)
                    result += $"{fileCount} 個のファイル ";
                if (directoryCount > 0)
                    result += $"{directoryCount} 個のフォルダ";

                return result;
            }
        }

        public bool TryGetFirst(Predicate<PathInformationViewModel> inPredication, out PathInformationViewModel? outPath)
        {
            outPath = _pathes.FirstOrDefault(item => inPredication(item));
            return outPath != null;
        }

        public PathInformationColumnViewModel? GetColumn(FilePropertyItemType inType)
            => _columns.FirstOrDefault(c => c.Type == inType);

        public ICollectionView CollectionView => _collectionViewSource.View;

        private readonly string LatestColumnSaveFilepath = $"{ApplicationViewModel.ApplicationDataDirectoryPath}/latest_columns.json";

        private Utility.ReactiveCollection<PathInformationViewModel> _pathes = new Utility.ReactiveCollection<PathInformationViewModel>();
        private CollectionViewSource _collectionViewSource = new CollectionViewSource();
        private IList<PathInformationColumnViewModel> _columns = new List<PathInformationColumnViewModel>();
        private SortDescription? _sortDescription = null;
    }
}
