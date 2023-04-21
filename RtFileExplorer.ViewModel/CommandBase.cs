using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RtFileExplorer.ViewModel.Wpf
{
    public abstract class CommandBase<VM> : ICommand where VM : IViewModel
    {
        public CommandBase(VM inViewModel)
        {
            ViewModel = inViewModel;
        }

        protected readonly VM ViewModel;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public abstract bool CanExecute(object? parameter);
        public abstract void Execute(object? parameter);
    }

    public class SimpleCommand<VM> : CommandBase<VM> where VM : IViewModel
    {
        public delegate bool CanExecuteFunc(object? inParameter, VM inViewModel);
        public delegate void ExecuteEvent(object? inParameter, VM inViewModel);

        public SimpleCommand(VM inViewModel, CanExecuteFunc inCanExecute, ExecuteEvent inExecute)
            : base(inViewModel)
        {
            _canExecute = inCanExecute;
            _execute = inExecute;
        }

        public override bool CanExecute(object? parameter)
            => _canExecute(parameter, ViewModel);
        public override void Execute(object? parameter)
            => _execute(parameter, ViewModel);

        private readonly CanExecuteFunc _canExecute;
        private readonly ExecuteEvent _execute;
    }

    public abstract class AsyncCommandBase<VM> : ICommand where VM : IViewModel
    {
        public AsyncCommandBase(VM inViewModel)
        {
            ViewModel = inViewModel;
        }

        protected readonly VM ViewModel;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public abstract bool CanExecute(object? parameter);

        public async void Execute(object? parameter)
        {
            FireCanExecuteChanged();

            await ExecuteAsync(parameter);

            FireCanExecuteChanged();
        }

        protected virtual async Task ExecuteAsync(object? parameter)
            => await Task.CompletedTask;

        private void FireCanExecuteChanged()
            => CommandManager.InvalidateRequerySuggested();
    }

    public class SimpleAsyncCommand<VM> : AsyncCommandBase<VM> where VM : IViewModel
    {
        public delegate bool CanExecuteFunc(object? inParameter, VM inViewModel);
        public delegate Task ExecuteEvent(object? inParameter, VM inViewModel);

        public SimpleAsyncCommand(VM inViewModel, CanExecuteFunc inCanExecute, ExecuteEvent inExecute)
            : base(inViewModel)
        {
            _canExecute = inCanExecute;
            _execute = inExecute;
        }

        public override bool CanExecute(object? parameter)
            => _canExecute(parameter, ViewModel);
        protected override async Task ExecuteAsync(object? parameter)
            => await _execute(parameter, ViewModel);

        private readonly CanExecuteFunc _canExecute;
        private readonly ExecuteEvent _execute;
    }

    public class ReactiveCommandBase<VM, Args> : Reactive.Bindings.ReactiveCommand<Args> where VM : IViewModel
    {
        public ReactiveCommandBase(VM inViewModel)
        {
            ViewModel = inViewModel;
        }

        protected readonly VM ViewModel;
    }
}
