using Newtonsoft.Json;

namespace RtFileExplorer.Model.FileInformation.FileProperty
{
    public class FileExtraPropertiesList
    {
        public FileSharedProperties SharedProperties
        { get; } = new FileSharedProperties();

        public Dictionary<string, FileExtraProperties> Items
        { get; } = new Dictionary<string, FileExtraProperties>();

        public FileExtraProperties CreateNewItem(string inFilename)
        {
            var result = new FileExtraProperties(SharedProperties);
            Items.Add(inFilename, result);

            return result;
        }

        public bool HasSomeData
        {
            get
            {
                if (!Items.Any())
                    return false;

                return false;
            }
        }

        public Json Export()
            => new Json
            {
                SharedProperties = SharedProperties.Export(),
                Items = new Dictionary<string, FileExtraProperties.Json>(
                        Items.Select(item => (item.Key, item.Value.Export()))
                            .Where(item => item.Item2 is not null)
                            .Select(item => new KeyValuePair<string, FileExtraProperties.Json>(
                                item.Key, item.Item2!
                            ))
                    ),
            };

        public void ImportFrom(Json inJson)
        {
            SharedProperties.ImportFrom(inJson.SharedProperties);

            Items.Clear();
            foreach (var (key, prop) in inJson.Items)
            {
                var item = CreateNewItem(key);
                item.ImportFrom(prop);
            }
        }

        public class Json
        {
            [JsonProperty("shared")]
            public FileSharedProperties.Json SharedProperties
            { get; set; } = new FileSharedProperties.Json();

            [JsonProperty("items")]
            public Dictionary<string, FileExtraProperties.Json> Items
            { get; set; } = new Dictionary<string, FileExtraProperties.Json>();
        }
    }
}
