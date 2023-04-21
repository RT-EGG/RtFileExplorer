using System.Text.Json.Serialization;

namespace RtFileExplorer.Model.FileInformation
{
    public class JsonFileInformationColumn
    {
        [JsonPropertyName("width")]
        public double Width
        { get; set; }

        [JsonPropertyName("is_visible")]
        public bool IsVisible
        { get; set; }
    }
}
