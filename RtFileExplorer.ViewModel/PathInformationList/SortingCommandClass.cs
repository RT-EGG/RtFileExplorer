using RtFileExplorer.Model.FileInformation;
using RtFileExplorer.ViewModel.Wpf.PathInformation;
using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace RtFileExplorer.ViewModel.Wpf.PathInformationList
{
    public partial class PathInformationListViewModel
    {
        public class SortChangedEventArgs : EventArgs
        {
            public SortChangedEventArgs(FilePropertyItemType inColumn, ListSortDirection? inDirection)
            {
                Column = inColumn;
                Direction = inDirection;
            }

            public readonly FilePropertyItemType Column;
            public readonly ListSortDirection? Direction;
        }

        private void ChangeSort(string inPropertyName, ListSortDirection? inDirection)
        {
            _sortDescription = inDirection is null 
                ? null : new SortDescription(inPropertyName, inDirection.Value);

            ApplySort();
        }

        private void ApplySort()
        {
            _collectionViewSource.SortDescriptions.Clear();
            // ドライブ/フォルダ/ファイルでのソートを第一に設定する
            _collectionViewSource.SortDescriptions.Add(
                new SortDescription(
                    nameof(PathInformationViewModel.PathType),
                    _sortDescription is null 
                        ? ListSortDirection.Ascending : _sortDescription.Value.Direction)
            );
            if (_sortDescription is not null)
                _collectionViewSource.SortDescriptions.Add(_sortDescription.Value);
            CollectionView.Refresh();

            var sortItem = PathInformationColumnViewModel.ParsePropertyItemType(_sortDescription?.PropertyName ?? string.Empty);
            var direction = _sortDescription?.Direction;
            foreach (var column in _columns)
            {
                column.Sorting = column.Type == sortItem
                        ? direction : null;
            }

            TargetApplicationBinder.Instance?.Application?.UiDispatcher?.Invoke(() =>
            {
                View?.InvalidateVisual();
            });
        }

        class SortingCommandClass : ReactiveCommandBase<PathInformationListViewModel, DataGridSortingEventArgs>
        {
            public SortingCommandClass(PathInformationListViewModel inViewModel) 
                : base(inViewModel)
            {
                this.Subscribe(args =>
                    ViewModel.ChangeSort(args.Column.SortMemberPath, args.Column.SortDirection)
                );
            }
        }
    }
}
