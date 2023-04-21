using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Utility
{
    public interface IReadOnlyReactiveCollection<T> : IEnumerable<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
    {

    }

    public interface IReactiveCollection<T> : ICollection<T>, IList<T>, ICollection, IList, IReadOnlyReactiveCollection<T>
    {

    }

    //public class ReactiveCollection<T> : Reactive.Bindings.ReactiveCollection<T>, IReactiveCollection<T>
    public class ReactiveCollection<T> : ObservableCollection<T>, IReactiveCollection<T>
    {
        public ReactiveCollection()
            : base()
        {
        }
    }
}
