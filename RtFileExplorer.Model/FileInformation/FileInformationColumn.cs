using Reactive.Bindings;

namespace RtFileExplorer.Model.FileInformation
{
    public class FileInformationColumn
    {
        public FileInformationColumn(FilePropertyItemType inType)
        {
            var att = inType.GetFilePropertyAttribute();

            Width = new ReactiveProperty<double>(att.InitialWidth);
            IsVisible = new ReactiveProperty<bool>(att.InitialVisibility);
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

        public readonly IReactiveProperty<double> Width;
        public readonly IReactiveProperty<bool> IsVisible;
    }
}
