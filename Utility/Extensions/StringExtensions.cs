namespace Utility
{
    public static class StringExtensions
    {
        public static string UnifyNewLineMarker(this string inValue, string inNewLineMarker = "\n")
            => inNewLineMarker == "\n"
            ? inValue.Replace("\r\n", "\n").Replace("\r", "\n")
            : inValue.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", inNewLineMarker);
    }
}
