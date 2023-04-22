using System;
using System.Windows;

namespace Utility.Wpf
{
    public class Messages
    {
        public static void ShowErrorMessage(string inText)
        {
            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            if (assembly is null)
                throw new InvalidProgramException();

            MessageBox.Show(
                inText, assembly.GetName().Name,
                MessageBoxButton.OK, MessageBoxImage.Error
            );
        }
    }
}
