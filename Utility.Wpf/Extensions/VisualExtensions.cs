using System.Windows.Media;

namespace Utility.Wpf.Extensions
{
    public static class VisualExtensions
    {
        public static T? GetVisualChild<T>(this Visual inParent) where T : Visual
        {
            var child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(inParent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(inParent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }

                if (child != null)
                {
                    break;
                }
            }

            return child;
        }
    }
}
