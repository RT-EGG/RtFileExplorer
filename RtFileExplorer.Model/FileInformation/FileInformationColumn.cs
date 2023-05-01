using Reactive.Bindings;

namespace RtFileExplorer.Model.FileInformation
{
    public class FileInformationColumn
    {
        public FileInformationColumn(FilePropertyItemType inType)
        {
            PropertyItemType = inType;
            var att = inType.GetFilePropertyAttribute();

            Width = new ReactiveProperty<double>(att.InitialWidth);
            IsVisible = new ReactiveProperty<bool>(att.InitialVisibility);
            Sorting = new ReactiveProperty<SortDirection?>(initialValue: null);
        }

        public JsonFileInformationColumn Export()
            => new JsonFileInformationColumn
            {
                Width = Width.Value,
                IsVisible = IsVisible.Value
            };

        public void Import(JsonFileInformationColumn inValue)
        {
            Width.Value = inValue.Width;
            IsVisible.Value = inValue.IsVisible;
        }

        public readonly FilePropertyItemType PropertyItemType;
        public IReactiveProperty<double> Width { get; }
        public IReactiveProperty<bool> IsVisible { get; }
        public IReactiveProperty<SortDirection?> Sorting { get; }

        public enum SortDirection
        {
            Ascending,
            Descending
        }
    }
}
