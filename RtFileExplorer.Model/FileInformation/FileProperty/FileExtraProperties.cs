using Newtonsoft.Json;
using Reactive.Bindings;
using System.Collections.Specialized;
using Utility;

namespace RtFileExplorer.Model.FileInformation.FileProperty
{
    public class FileExtraProperties
    {
        public FileExtraProperties(FileSharedProperties inSharedProperties)
        {
            FileSharedProperties = inSharedProperties;

            FileSharedProperties.Authors.Subscribe(args =>
            {
                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Remove:
                        args.OldItems?
                            .OfType<FileAuthor>()
                            .ForEach(author => Authors.Remove(author));
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        Authors.Clear();
                        break;
                }
            });

            FileSharedProperties.Tags.Subscribe(args =>
            {
                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Remove:
                        args.OldItems?
                            .OfType<FileTag>()
                            .ForEach(tag => Tags.Remove(tag));
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        Tags.Clear();
                        break;
                }
            });
        }

        public Utility.ReactiveCollection<FileAuthor> Authors { get; } = new Utility.ReactiveCollection<FileAuthor>();
        public IReactiveProperty<uint?> Rating { get; } = new ReactiveProperty<uint?>();
        public Utility.ReactiveCollection<FileTag> Tags { get; } = new Utility.ReactiveCollection<FileTag>();

        public Json? Export()
        {
            if (Rating.Value is not null
                || Authors.Any()
                || Tags.Any())
            {
                return new Json
                {
                    Rating = Rating.Value,
                    Authors = Authors.Any() ? Authors.Select(item => item.ID).ToArray() : null,
                    Tags = Tags.Any() ? Tags.Select(item => item.ID).ToArray() : null,
                };
            }

            return null;
        }

        public void ImportFrom(Json inJson)
        {
            Rating.Value = inJson.Rating;

            Authors.Clear();
            if (inJson.Authors is not null)
            {
                foreach (var id in inJson.Authors)
                {
                    var author = FileSharedProperties.Authors.FirstOrDefault(item => item.ID == id);
                    if (author is not null)
                        Authors.Add(author);
                }
            }

            Tags.Clear();
            if (inJson.Tags is not null)
            {
                foreach (var id in inJson.Tags)
                {
                    var tag = FileSharedProperties.Tags.FirstOrDefault(item => item.ID == id);
                    if (tag is not null)
                        Tags.Add(tag);
                }
            }
        }

        private readonly FileSharedProperties FileSharedProperties;

        public class Json
        {
            [JsonProperty("authors", NullValueHandling = NullValueHandling.Ignore)]
            public Guid[]? Authors
            { get; set; } = null;

            [JsonProperty("rating", NullValueHandling = NullValueHandling.Ignore)]
            public uint? Rating
            { get; set; } = null;

            [JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
            public Guid[]? Tags
            { get; set; } = null;
        }
    }
}
