using Newtonsoft.Json;
using Reactive.Bindings;

namespace RtFileExplorer.Model.FileInformation.FileProperty
{
    public class FileTag : FileSharedProperty<FileTag.Json>
    {
        public FileTag(string inName = "")
        {
            Name.Value = inName;
        }


        public Json Export()
            => ExportTo(new Json());

        public static FileTag FromJson(Json inJson)
        {
            var result = new FileTag();
            result.ImportFrom(inJson);

            return result;
        }

        public readonly IReactiveProperty<string> Name = new ReactiveProperty<string>();

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
