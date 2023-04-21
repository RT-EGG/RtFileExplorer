using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Utility;

namespace RtFileExplorer.ViewModel.Wpf
{
    public interface IViewModel : INotifyPropertyChanged
    { }

    public abstract class ViewModelBase : IViewModel, IDisposable
    {
        public ViewModelBase()
        {
            if (TargetApplicationBinder.Instance == null)
                return;

            TargetApplicationBinder.Instance.Application!.OnBeforeFrameworkElementRemove += element =>
            {
                if ((_view is not null) && (_view == element))
                    _view = null;
            };
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected FrameworkElement? View
        {
            get
            {
                if (_view == null)
                    _view = TargetApplicationBinder.Instance?.FindViewFor<FrameworkElement>(this);
                return _view;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    ModelSubscriptions.DisposeItems();
                    ModelSubscriptions.Clear();
                }

                _isDisposed = true;
            }
        }

        protected void AddModelSubscription(IDisposable inSubscription)
            => ModelSubscriptions.Add(inSubscription);
        protected void RegisterPropertyNotification<T>(IReadOnlyReactiveProperty<T> inProperty, params string[] inPropertyNames)
            => AddModelSubscription(inProperty.ObserveOnUIDispatcher().Subscribe(_ =>
                inPropertyNames.ForEach(name => FirePropertyChanged(name))
            ));
        protected void ClearModelSubscriptions()
        {
            ModelSubscriptions.DisposeItems();
            ModelSubscriptions.Clear();
        }

        protected void FirePropertyChanged(params string[] inPropertyNames)
            => inPropertyNames.ForEach(name => FirePropertyChanged(name));

        protected void FirePropertyChanged(string inPropertyName)
            => FirePropertyChanged(new PropertyChangedEventArgs(inPropertyName));

        protected void FirePropertyChanged(PropertyChangedEventArgs inArgs)
        {
            if (TargetApplicationBinder.Instance?.Application != null)
            {
                TargetApplicationBinder.Instance.Application.UiDispatcher.BeginInvoke(() => PropertyChanged?.Invoke(this, inArgs));
            }
        }

        private FrameworkElement? _view;
        private List<IDisposable> ModelSubscriptions = new List<IDisposable>();
        private bool _isDisposed;
    }

    public abstract class ViewModelBase<M> : ViewModelBase where M : class
    {
        public void BindModel(M? inModel)
        {
            ClearModelSubscriptions();

            _model.Value = inModel;
            if (Model != null)
                BindModelProperties(Model);
        }

        public IDisposable RegisterAfterModelChanged(Action<M?> onChange)
            => _model.Subscribe(m => onChange(m));

        protected M? Model => _model.Value;
        private ReactiveProperty<M?> _model = new ReactiveProperty<M?>();
        public bool ModelBinded => Model != null;

        protected virtual void BindModelProperties(M inModel)
        { }
    }
}
