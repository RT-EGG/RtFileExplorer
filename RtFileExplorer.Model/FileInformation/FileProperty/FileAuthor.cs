using Newtonsoft.Json;
using Reactive.Bindings;

namespace RtFileExplorer.Model.FileInformation.FileProperty
{
    public class FileAuthor : FileSharedProperty<FileAuthor.Json>
    {
        public FileAuthor(string inName = "")
        {
            Name.Value = inName;
        }

        public Json Export()
            => ExportTo(new Json());

        public static FileAuthor FromJson(Json inJson)
        {
            var result = new FileAuthor();
            result.ImportFrom(inJson);

            return result;
        }

        public IReactiveProperty<string> Name { get; } = new ReactiveProperty<string>();

        protected override Json ExportTo(Json inJson)
        {
            base.ExportTo(inJson);
            inJson.Name = Name.Value;

            return inJson;
        }

        protected override void ImportFrom(Json inJson)
        {
            base.ImportFrom(inJson);
            Name.Value = inJson.Name;
        }

        public new class Json : FileSharedProperty.Json
        {
            [JsonProperty("name")]
            public string Name
            { get; set; } = string.Empty;
        }
    }
}
