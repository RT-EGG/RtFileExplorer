using System;
using System.Windows;

namespace RtFileExplorer.View.Dialogs
{
    internal class WindowCloser : IDisposable
    {
        public WindowCloser(Window inWindow)
        {
            _window = inWindow;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _window.Close();
                }

                _isDisposed = true;
            }
        }

        void IDisposable.Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private bool _isDisposed;
        private readonly Window _window;
    }
}
