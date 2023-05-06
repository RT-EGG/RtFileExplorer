using Reactive.Bindings;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace RtFileExplorer.ViewModel.Wpf.PathInformation.Filter
{
    public partial class PathInformationFilterViewModel : ViewModelBase
    {
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        // Viewでの設計時DataContext設定のためにデフォルトコンストラクタが必要
        public PathInformationFilterViewModel()
#pragma warning restore CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        {
            _collectionViewSource = new CollectionViewSource();
        }

        public PathInformationFilterViewModel(CollectionViewSource inCollectionViewSource)
        {
            _collectionViewSource = inCollectionViewSource;
            _collectionViewSource.Filter += DoFilter;

            CloseFilterViewCommand = new CloseFilterViewCommandClass(this);

            RegisterPropertyNotification(_viewVisibility, nameof(ViewVisibility), nameof(ViewWidth));
            RegisterPropertyNotification(_viewWidth, nameof(ViewWidth));
            AddModelSubscription(_fileNameFilter.Subscribe(async value =>
            {
                FirePropertyChanged(nameof(FileNameFilter));
                await SetFileNameFilter(value);
            }));
        }

        public Visibility ViewVisibility
        {
            get => _viewVisibility.Value;
            set => _viewVisibility.Value = value;
        }

        public GridLength ViewWidth
        {
            get => new GridLength(ViewVisibility == Visibility.Visible ? Math.Max(_viewWidth.Value, MinViewWidth) : 0.0);
            set => _viewWidth.Value = value.Value;
        }

        public string FileNameFilter
        {
            get => _fileNameFilter.Value;
            set => _fileNameFilter.Value = value;
        }

        public ICommand CloseFilterViewCommand { get; }
        public event EventHandler? FilterFinished;

        private void DoFilter(object sender, FilterEventArgs e)
        {
            e.Accepted = false;
            if (e.Item is not PathInformationViewModel item)
                return;

            var fileNameFilter = FileNameFilter;
            if (string.IsNullOrWhiteSpace(fileNameFilter))
            {
                e.Accepted = true;
                return;
            }

            e.Accepted = item.Name.Contains(fileNameFilter);
        }

        private async Task SetFileNameFilter(string inFilterText)
        {
            if (_fileNameFilterSetCancellationToken is not null)
                _fileNameFilterSetCancellationToken.Cancel();

            _fileNameFilterSetCancellationToken = new CancellationTokenSource();
            try
            {
                await Task.Delay(1000, _fileNameFilterSetCancellationToken.Token);

                _fileNameFilterSetCancellationToken?.Dispose();
                _collectionViewSource.View.Refresh();
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                FilterFinished?.Invoke(this, EventArgs.Empty);
                _fileNameFilterSetCancellationToken = null;
            }
        }

        private const double MinViewWidth = 100.0;
        private readonly CollectionViewSource _collectionViewSource;

        private IReactiveProperty<Visibility> _viewVisibility = new ReactiveProperty<Visibility>(Visibility.Collapsed);
        private IReactiveProperty<double> _viewWidth = new ReactiveProperty<double>(200.0);
        private CancellationTokenSource? _fileNameFilterSetCancellationToken = null;
        private IReactiveProperty<string> _fileNameFilter = new ReactiveProperty<string>();
    }
}
