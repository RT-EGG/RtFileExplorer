using System.Collections.Specialized;

namespace Utility
{
    public static class ObservableCollectionSubscription_
    {
        public static IDisposable Subscribe(this INotifyCollectionChanged inDistoributor, Action<NotifyCollectionChangedEventArgs> onNext)
            => new ObservableCollectionSubscription(inDistoributor, onNext);
    }

    internal class ObservableCollectionSubscription : IDisposable
    {
        public ObservableCollectionSubscription(INotifyCollectionChanged inDistoributor, Action<NotifyCollectionChangedEventArgs> onNext)
        {
            _distributor = inDistoributor;
            _distributor.CollectionChanged += _distributor_CollectionChanged;

            _onNext = onNext;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void _distributor_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
            => _onNext(e);

        private readonly INotifyCollectionChanged _distributor;
        private readonly Action<NotifyCollectionChangedEventArgs> _onNext;
        private bool _isDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                    _distributor.CollectionChanged -= _distributor_CollectionChanged;

                _isDisposed = true;
            }
        }
    }
}
