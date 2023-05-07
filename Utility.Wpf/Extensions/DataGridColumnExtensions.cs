using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Utility.Wpf.Extensions
{
    public static class DataGridColumnExtensions
    {
        public static DataGridColumnHeader? GetHeader(this DataGridColumn inColumn)
        {
            switch (inColumn.Header)
            {
                case DataGridColumnHeader header:
                    return header;

                case FrameworkElement element:
                    if (element.Parent is DataGridColumnHeader eHeader)
                        return eHeader;
                    break;
            }

            return null;
        }
    }
}
