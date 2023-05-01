using Newtonsoft.Json;
using Utility;

namespace RtFileExplorer.Model.FileInformation.FileProperty
{
    public class FileSharedProperties
    {
        public Utility.ReactiveCollection<FileAuthor> Authors { get; } = new Utility.ReactiveCollection<FileAuthor>();
        public Utility.ReactiveCollection<FileTag> Tags { get; } = new Utility.ReactiveCollection<FileTag>();

        public void Initialize()
        {
            Authors.Clear();
            Tags.Clear();
        }

        public Json Export()
            => new Json
            {
                Authors = Authors.Select(item => item.Export()).ToArray(),
                Tags = Tags.Select(item => item.Export()).ToArray(),
            };

        public void ImportFrom(Json inJson)
        {
            Initialize();

            inJson.Authors
                .Select(author => FileAuthor.FromJson(author))
                .ForEach(author => Authors.Add(author));

            inJson.Tags
                .Select(tag => FileTag.FromJson(tag))
                .ForEach(tag => Tags.Add(tag));
        }

        public class Json
        {
            [JsonProperty("autors")]
            public FileAuthor.Json[] Authors
            { get; set; } = new FileAuthor.Json[0];
            
            [JsonProperty("tags")]
            public FileTag.Json[] Tags
            { get; set; } = new FileTag.Json[0];
        }
    }
}
