namespace RtFileExplorer.Model.ValueConverter
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object inSource)
            => ((DateTime)inSource).ToString("yyyy/MM/dd HH:mm:ss");
    }
}
