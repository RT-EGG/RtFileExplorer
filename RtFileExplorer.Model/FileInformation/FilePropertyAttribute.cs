using RtFileExplorer.Model.ValueConverter;

namespace RtFileExplorer.Model.FileInformation
{
    public class FilePropertyAttribute : Attribute
    {
        public bool IsSupported { get; set; } = false;
        public bool IsReadOnly { get; set; } = true;
        public string DisplayText { get; set; } = String.Empty;
        public FilePropertyDataType DataType { get; set; }
        public bool IsReordable { get; set; } = true;
        public bool IsResizable { get; set; } = true;
        public double InitialWidth { get; set; } = 100.0;
        public bool InitialVisibility { get; set; } = false;

        public Type? ColumnValueConverter { get; set; } = null;
    }
}
