using RtFileExplorer.Model.ValueConverter;
using System.Reflection;

namespace RtFileExplorer.Model.FileInformation
{
    public enum FilePropertyItemType
    {
        [FileProperty(
            DataType = FilePropertyDataType.Icon,
            DisplayText = "",
            InitialVisibility = true,
            InitialWidth = 16.0,
            IsReadOnly = true,
            IsReordable = false,
            IsResizable = false,
            IsSupported = true
        )]
        Icon,

        [FileProperty(
            DataType = FilePropertyDataType.String,             
            DisplayText = "名前",
            InitialVisibility = true,
            InitialWidth = 200.0,
            IsReadOnly = false,
            IsSupported = true
        )]
        Name,

        [FileProperty(
            DataType = FilePropertyDataType.Integer,
            DisplayText = "サイズ",
            InitialVisibility = true,
            IsReadOnly = true,
            IsSupported = true,
            ColumnValueConverter = typeof(FileSizeToStringConverter)
        )]
        Size,

        [FileProperty(
            DataType = FilePropertyDataType.DateTime,
            DisplayText = "作成日時",
            InitialVisibility = true,
            IsReadOnly = true,
            IsReordable = true,
            IsResizable = true,
            IsSupported = true,
            ColumnValueConverter = typeof(DateTimeToStringConverter)
        )]
        CreationDate,

        [FileProperty(
            DataType = FilePropertyDataType.DateTime,
            DisplayText = "更新日時",
            InitialVisibility = true,
            IsReadOnly = true,
            IsReordable = true,
            IsResizable = true,
            IsSupported = true,
            ColumnValueConverter = typeof(DateTimeToStringConverter)
        )]
        LastWriteDate,

        [FileProperty(
            DataType = FilePropertyDataType.DateTime,
            DisplayText = "アクセス日時",
            InitialVisibility = true,
            IsReadOnly = true,
            IsReordable = true,
            IsResizable = true,
            IsSupported = true,
            ColumnValueConverter = typeof(DateTimeToStringConverter)
        )]
        LastAccessDate,
    }

    public static class FilePropertyItemTypeAttributeExtractor
    {
        public static bool GetIsSupported(this FilePropertyItemType inValue)
            => inValue.GetFilePropertyAttribute().IsSupported;

        public static bool GetIsReadOnly(this FilePropertyItemType inValue)
            => inValue.GetFilePropertyAttribute().IsReadOnly;

        public static FilePropertyDataType GetDataType(this FilePropertyItemType inValue)
            => inValue.GetFilePropertyAttribute().DataType;


        public static FilePropertyAttribute GetFilePropertyAttribute(this FilePropertyItemType inValue)
        {
            FieldInfo field = inValue.GetType().GetField(inValue.ToString())!;
            if (field.GetCustomAttribute<FilePropertyAttribute>() is FilePropertyAttribute attribute)
                return attribute;

            throw new NotImplementedException();
        }
    }
}
