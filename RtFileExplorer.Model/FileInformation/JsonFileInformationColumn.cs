using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RtFileExplorer.Model.FileInformation
{
    public class JsonFileInformationColumn
    {
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public FilePropertyItemType Type
        { get; set; }

        [JsonProperty("width")]
        public double Width
        { get; set; }

        [JsonProperty("is_visible")]
        public bool IsVisible
        { get; set; }

        [JsonProperty("display_index")]
        public int DisplayIndex
        { get; set; } = -1;
    }

    public class JsonFileInformationColumnSorting
    {
        [JsonProperty("name")]
        public string PropertyName
        { get; set; } = string.Empty;

        [JsonProperty("direction")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SortDirection Direction
        { get; set; }
    }

    public class JsonFileInformationColumnSaveData
    {
        [JsonProperty("sorting", NullValueHandling = NullValueHandling.Ignore)]
        public JsonFileInformationColumnSorting? Sorting
        { get; set; } = null;

        [JsonProperty("items")]
        public List<JsonFileInformationColumn> Items
        { get; set; } = new List<JsonFileInformationColumn>();
    }
}
