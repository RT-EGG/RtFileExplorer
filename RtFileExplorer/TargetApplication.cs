using RtFileExplorer.View.Dialogs;
using RtFileExplorer.ViewModel.Wpf;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace RtFileExplorer
{
    internal class TargetApplication : ITargetApplication
    {
        public TargetApplication(Application inApplication)
        {
            _application = inApplication;
            _application.ShutdownMode = ShutdownMode.OnLastWindowClose;
        }

        public Dispatcher UiDispatcher => Application.Current.Dispatcher;
        public IEnumerable<FrameworkElement> FrameworkElements => _frameworkElements;
        public ITargetApplicationDialogs Dialogs { get; } = new Dialogs();

        public void QuitApplication()
            => _application.Shutdown(0);

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

        public static void InitializeApplication(Application inApplication)
        {
            _instance = new TargetApplication(inApplication);
            TargetApplicationBinder.InitializeForApplication(Instance);
        }
        public static TargetApplication Instance => _instance!;
        private static TargetApplication? _instance = null;

        private readonly Application _application;
        private List<FrameworkElement> _frameworkElements = new List<FrameworkElement>();
    }
}
