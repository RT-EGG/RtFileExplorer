using RtFileExplorer.Model.FileInformation;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace RtFileExplorer.ViewModel.Wpf.PathInformation
{
    public class PathInformationColumnViewModel : ViewModelBase<FileInformationColumn>
    {
        public PathInformationColumnViewModel(PathInformationListViewModel inParent, FilePropertyItemType inType)
        {
            Parent = inParent;
            Type = inType;
        }

        public DataGridLength Width
        {
            get => Model!.Width.Value;
            set => Model!.Width.Value = value.Value;
        }

        public Visibility Visibility
        {
            get => IsVisible ? Visibility.Visible : Visibility.Hidden;
            set => IsVisible = value == Visibility.Visible;
        }

        public bool IsVisible
        {
            get => Model!.IsVisible.Value;
            set => Model!.IsVisible.Value = value;
        }

        public ListSortDirection? Sorting
        {
            get => Model!.Sorting.Value switch
            {
                FileInformationColumn.SortDirection.Ascending => ListSortDirection.Ascending,
                FileInformationColumn.SortDirection.Descending => ListSortDirection.Descending,
                _ => null,
            };
            set => Model!.Sorting.Value = value switch
            {
                ListSortDirection.Ascending => FileInformationColumn.SortDirection.Ascending,
                ListSortDirection.Descending => FileInformationColumn.SortDirection.Descending,
                _ => null,
            };
        }

        public static string GetPropertyName(FilePropertyItemType inType)
            => inType switch {
                FilePropertyItemType.Icon => nameof(PathInformationViewModel.Icon),
                FilePropertyItemType.Name => nameof(PathInformationViewModel.Name),
                FilePropertyItemType.Size => nameof(PathInformationViewModel.Size),
                FilePropertyItemType.CreationDate => nameof(PathInformationViewModel.CreationTime),
                FilePropertyItemType.LastWriteDate => nameof(PathInformationViewModel.LastWriteTime),
                FilePropertyItemType.LastAccessDate => nameof(PathInformationViewModel.LastAccessTime),
                FilePropertyItemType.Rating => nameof(PathInformationViewModel.Rating),
                _ => throw new NotSupportedException()
            };

        public static FilePropertyItemType? ParsePropertyItemType(string inPropertyName)
            => inPropertyName switch {
                nameof(PathInformationViewModel.Icon) => FilePropertyItemType.Icon,
                nameof(PathInformationViewModel.Name) => FilePropertyItemType.Name,
                nameof(PathInformationViewModel.Size) => FilePropertyItemType.Size  ,
                nameof(PathInformationViewModel.CreationTime) => FilePropertyItemType.CreationDate,
                nameof(PathInformationViewModel.LastWriteTime) => FilePropertyItemType.LastWriteDate,
                nameof(PathInformationViewModel.LastAccessTime) => FilePropertyItemType.LastAccessDate,
                nameof(PathInformationViewModel.Rating) => FilePropertyItemType.Rating,
                _ => null
            };

        public static PropertyPath GetPropertyPath(FilePropertyItemType inType)
            => new PropertyPath(GetPropertyName(inType));

        protected override void BindModelProperties(FileInformationColumn inModel)
        {
            base.BindModelProperties(inModel);

            if (inModel is not null)
            {
                RegisterPropertyNotification(inModel.Width, nameof(Width));
                RegisterPropertyNotification(inModel.IsVisible, nameof(Visibility), nameof(IsVisible));
                RegisterPropertyNotification(inModel.Sorting, nameof(Sorting));
            }
        }

        public readonly FilePropertyItemType Type;
        private readonly PathInformationListViewModel Parent;
    }
}
