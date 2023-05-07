namespace Utility
{
    public static class NumericExtensions
    {
        public static bool InRange(this int inValue, int inMin, int inMax)
            => inMin <= inValue && inValue <= inMax;
    }
}
