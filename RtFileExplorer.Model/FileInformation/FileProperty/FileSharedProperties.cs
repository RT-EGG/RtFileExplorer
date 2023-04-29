namespace RtFileExplorer.Model.FileInformation.FileProperty
{
    public class FileSharedProperties
    {
        public readonly Utility.IReactiveCollection<FileAuthor> Authors = new Utility.ReactiveCollection<FileAuthor>();
        public readonly Utility.IReactiveCollection<FileTag> Tags = new Utility.ReactiveCollection<FileTag>();
    }
}
