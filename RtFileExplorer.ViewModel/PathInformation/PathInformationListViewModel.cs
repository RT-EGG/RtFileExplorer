﻿using RtFileExplorer.Model.FileInformation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Utility;

namespace RtFileExplorer.ViewModel.Wpf.PathInformation
{
    public class PathInformationListViewModel : ViewModelBase
    {
        public PathInformationListViewModel()
        {
            _collectionViewSource.Source = _pathes;
            _collectionViewSource.IsLiveSortingRequested = true;
            _collectionViewSource.IsLiveFilteringRequested = true;

            BindingOperations.EnableCollectionSynchronization(_pathes, new object());

            // create columns
            foreach (FilePropertyItemType type in Enum.GetValues(typeof(FilePropertyItemType)))
            {
                var column = new PathInformationColumnViewModel(this, type);
                var model = new FileInformationColumn(type);

                column.BindModel(model);

                _columns.Add(column);
            }
        }

        public void AddPathInformation(PathInformationViewModel inValue)
            => _pathes.Add(inValue);

        public void RemovePathInformation(string inPath)
            => _pathes.RemoveAll(item => item.Path == inPath);

        public void ClearPathes()
            => _pathes.Clear();

        public bool TryGetFirst(Predicate<PathInformationViewModel> inPredication, out PathInformationViewModel? outPath)
        {
            outPath = _pathes.FirstOrDefault(item => inPredication(item));
            return outPath != null;
        }

        public PathInformationColumnViewModel? GetColumn(FilePropertyItemType inType)
            => _columns.FirstOrDefault(c => c.Type == inType);

        public ICollectionView CollectionView => _collectionViewSource.View;

        private Utility.ReactiveCollection<PathInformationViewModel> _pathes = new Utility.ReactiveCollection<PathInformationViewModel>();
        private CollectionViewSource _collectionViewSource = new CollectionViewSource();
        private IList<PathInformationColumnViewModel> _columns = new List<PathInformationColumnViewModel>();
    }
}
