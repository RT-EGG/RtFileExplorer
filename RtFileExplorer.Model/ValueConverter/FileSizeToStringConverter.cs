namespace RtFileExplorer.Model.ValueConverter
{
    public class FileSizeToStringConverter : IValueConverter
    {
        public object Convert(object inSource)
            => inSource is long source ? GetFileSizeText(source) : "";

        public static string GetFileSizeText(long inFileSize)
        {
            int unit = 1024;
            float size = inFileSize;
            int index = 0;

            while (size >= unit)
            {
                size /= unit;
                index++;
            }

            return $"{size.ToString("#,##0.##")}{_suffix[index]}";
        }

        private static readonly string[] _suffix = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
    }
}
