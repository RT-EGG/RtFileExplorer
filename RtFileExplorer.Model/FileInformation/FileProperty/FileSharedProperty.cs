using Newtonsoft.Json;

namespace RtFileExplorer.Model.FileInformation.FileProperty
{
    public abstract class FileSharedProperty
    {
        public class Json
        {
            [JsonProperty("id")]
            public Guid ID
            { get; set; }
        }
    }

    public abstract class FileSharedProperty<J> : FileSharedProperty where J : FileSharedProperty.Json
    {
        public FileSharedProperty()
            : this(Guid.NewGuid())
        { }

        protected FileSharedProperty(Guid inGuid)
        {
            ID = inGuid;
        }

        protected virtual J ExportTo(J inJson)
        {
            inJson.ID = ID;
            return inJson;
        }

        protected virtual void ImportFrom(J inJson)
        {
            ID = inJson.ID;
        }

        public Guid ID
        { get; private set; }
    }
}
