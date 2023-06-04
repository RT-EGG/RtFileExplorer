using Reactive.Bindings;
using System;
using System.Windows;
using System.Windows.Input;

namespace RtFileExplorer.ViewModel.Wpf.Utils
{
    public class ProgressViewModel : ViewModelBase
    {
        public ProgressViewModel()
        {
            CancelCommand = new CancelCommandClass(this, null);
        }

        public ProgressViewModel(Action? inOnCancel)
        {
            CancelCommand = new CancelCommandClass(this, inOnCancel);

            RegisterPropertyNotification(_title, nameof(Title));
            RegisterPropertyNotification(_description, nameof(Descrition));
            RegisterPropertyNotification(_progress, nameof(Progress));
            RegisterPropertyNotification(_subDescription, nameof(SubDescription), nameof(SubDescriptionVisibility));
            RegisterPropertyNotification(_subProgress, nameof(SubProgress), nameof(SubProgressVisibility));
        }

        public string Title
        {
            get => _title.Value;
            set => _title.Value = value;
        }

        public string Descrition
        {
            get => _description.Value;
            set => _description.Value = value;
        }

        public int Progress
        {
            get => _progress.Value;
            set => _progress.Value = value;
        }

        public string? SubDescription
        {
            get => _subDescription.Value;
            set => _subDescription.Value = value;
        }

        public int? SubProgress
        {
            get => _subProgress.Value;
            set => _subProgress.Value = value;
        }

        public bool Cancelable
        {
            get => CancelCommand.CanExecute(null);
        }

        public ICommand CancelCommand { get; }

        public Visibility SubDescriptionVisibility
            => SubDescription is null ? Visibility.Collapsed : Visibility.Visible;
        public Visibility SubProgressVisibility
            => SubProgress is null ? Visibility.Collapsed : Visibility.Visible;
        public Visibility CancelVisibility
            => Cancelable ? Visibility.Visible : Visibility.Collapsed;

        private IReactiveProperty<string> _title = new ReactiveProperty<string>();
        private IReactiveProperty<string> _description = new ReactiveProperty<string>();
        private IReactiveProperty<int> _progress = new ReactiveProperty<int>();
        private IReactiveProperty<string?> _subDescription = new ReactiveProperty<string?>();
        private IReactiveProperty<int?> _subProgress = new ReactiveProperty<int?>();

        private class CancelCommandClass : CommandBase<ProgressViewModel>
        {
            public CancelCommandClass(ProgressViewModel inViewModel, Action? inOnCancel)
                : base(inViewModel)
            { 
                OnCancel = inOnCancel;
            }

            public override bool CanExecute(object? parameter)
                => OnCancel is not null;

            public override void Execute(object? parameter)
                => OnCancel?.Invoke();

            private readonly Action? OnCancel;
        }

    }
}
