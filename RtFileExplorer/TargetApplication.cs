using RtFileExplorer.ViewModel.Wpf;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace RtFileExplorer
{
    internal class TargetApplication : ITargetApplication
    {
        public TargetApplication(Window inMainWindow)
        {
            _mainWindow = inMainWindow;
        }

        public Dispatcher UiDispatcher => Application.Current.Dispatcher;
        public IEnumerable<FrameworkElement> FrameworkElements => _frameworkElements;

        public void QuitApplication()
            => _mainWindow.Close();

        public void AddFrameworkElement(FrameworkElement inElement)
        {
            _frameworkElements.Add(inElement);
            OnAfterFrameworkElementAdd?.Invoke(inElement);
        }
        public void RemoveFrameworkElement(FrameworkElement inElement)
        {
            OnBeforeFrameworkElementRemove?.Invoke(inElement);
            _frameworkElements?.Remove(inElement);
        }

        public event FrameworkElementNotifyAction? OnAfterFrameworkElementAdd;
        public event FrameworkElementNotifyAction? OnBeforeFrameworkElementRemove;

        private Window _mainWindow;

        public static void InitializeApplication(Window inMainWindow)
        {
            _instance = new TargetApplication(inMainWindow);
        }
        public static TargetApplication Instance => _instance!;
        private static TargetApplication? _instance = null;

        private List<FrameworkElement> _frameworkElements = new List<FrameworkElement>();
    }
}
