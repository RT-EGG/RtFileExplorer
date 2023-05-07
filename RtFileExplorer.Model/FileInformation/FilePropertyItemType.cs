using RtFileExplorer.Model.ValueConverter;
using System.Reflection;
using System.Runtime.Serialization;

namespace RtFileExplorer.Model.FileInformation
{
    public enum FilePropertyItemType
    {
        [EnumMember(Value = "icon")]
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

        [EnumMember(Value = "name")]
        [FileProperty(
            DataType = FilePropertyDataType.String,             
            DisplayText = "名前",
            InitialVisibility = true,
            InitialWidth = 200.0,
            IsReadOnly = false,
            IsSupported = true
        )]
        Name,

        [EnumMember(Value = "size")]
        [FileProperty(
            DataType = FilePropertyDataType.Integer,
            DisplayText = "サイズ",
            InitialVisibility = true,
            IsReadOnly = true,
            IsSupported = true,
            ColumnValueConverter = typeof(FileSizeToStringConverter)
        )]
        Size,

        [EnumMember(Value = "creation_date")]
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

        [EnumMember(Value = "last_update_date")]
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

        [EnumMember(Value = "last_access_date")]
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

        [EnumMember(Value = "rating")]
        [FileProperty(
            DataType = FilePropertyDataType.Integer,
            DisplayText = "評価",
            InitialVisibility = true,
            InitialWidth = 84,
            IsReadOnly = false,
            IsReordable = true,
            IsResizable = false,
            IsSupported = true
        )]
        Rating,
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
