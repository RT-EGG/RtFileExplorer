using System.Windows;
using System.Windows.Media;

namespace Utility.Wpf.Extensions
{
    public static class UIElementExtensions
    {
        public static T? FindAncestor<T>(this UIElement inElement) where T : UIElement
        {
            var element = inElement;

            while (element is not null)
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                if (element is T target)
                {
                    return target;
                }
            }
            return null;
        }
    }
}
