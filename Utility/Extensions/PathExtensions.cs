namespace Utility
{
    public static class PathExtensions
    {
        public static string EnsureFileSystemPath(this string inPath)
            => string.Join(PathSplitter,
                inPath.Replace('\\', PathSplitter)
                .Split(PathSplitter, StringSplitOptions.RemoveEmptyEntries)
            );

        public const char PathSplitter = '/';
    }
}
