using RtFileExplorer.Model.FileInformation;
using System;
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

        public static string GetPropertyName(FilePropertyItemType inType)
            => inType switch {
                FilePropertyItemType.Icon => nameof(PathInformationViewModel.Icon),
                FilePropertyItemType.Name => nameof(PathInformationViewModel.Name),
                FilePropertyItemType.Size => nameof(FileInformationViewModel.Size),
                FilePropertyItemType.CreationDate => nameof(PathInformationViewModel.CreationTime),
                FilePropertyItemType.LastWriteDate => nameof(PathInformationViewModel.LastWriteTime),
                FilePropertyItemType.LastAccessDate => nameof(PathInformationViewModel.LastAccessTime),
                FilePropertyItemType.Rating => nameof(PathInformationViewModel.Rating),
                _ => throw new NotSupportedException()
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
            }
        }

        public readonly FilePropertyItemType Type;
        private readonly PathInformationListViewModel Parent;
    }
}
