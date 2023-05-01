using RtFileExplorer.Model.FileInformation;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace RtFileExplorer.ViewModel.Wpf.PathInformation
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
            _customSortComparer = null;

            ApplySort();
        }

        private void ApplySort()
        {
            _collectionViewSource.SortDescriptions.Clear();
            if (_collectionViewSource.View is ListCollectionView view && _customSortComparer is not null)
                view.CustomSort = _customSortComparer;
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
