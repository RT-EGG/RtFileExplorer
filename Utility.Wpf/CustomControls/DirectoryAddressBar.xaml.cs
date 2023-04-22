using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Utility.Wpf.CustomControls
{
    public partial class DirectoryAddressBar : UserControl
    {
        public DirectoryAddressBar()
        {
            InitializeComponent();

            CommandManager.RegisterClassCommandBinding(typeof(DirectoryAddressBar), new CommandBinding(
                OnInputEnterTextBoxCommand,
                OnInputEnterTextBox,
                (_, e) => e.CanExecute = true
            ));
            CommandManager.RegisterClassCommandBinding(typeof(DirectoryAddressBar), new CommandBinding(
                OnInputEscapeTextBoxCommand,
                OnInputEscapeTextBox,
                (_, e) => e.CanExecute = true
            ));

            SetImage("Utility.Wpf.Resources.Back.png", ButtonImage_Back);
            SetImage("Utility.Wpf.Resources.Next.png", ButtonImage_Next);
        }

        public IEnumerable<string> Items
        {
            get => (IEnumerable<string>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, new List<string>(value.Where(item => !string.IsNullOrEmpty(item))));
        }

        public void Undo()
        {
            if (!Undoable)
                throw new InvalidProgramException();

            var path = UndoPathes.Pop();
            // null文字列は"PC"ディレクトリ（ドライブリスト）とする
            if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
            {
                Messages.ShowErrorMessage($"\"{path}\"は見つかりません。\n削除されたか名前が変更された可能性があります。");
                UndoPathes.Clear();
                return;
            }

            RedoPathes.Push(string.Join('/', Items.Where(item => !string.IsNullOrEmpty(item))));

            IsUndoing = true;
            try
            {
                Items = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            }
            finally
            {
                IsUndoing = false;
            }
        }

        public void Redo()
        {
            if (!Redoable)
                throw new InvalidProgramException();

            var path = RedoPathes.Pop();
            // null文字列は"PC"ディレクトリ（ドライブリスト）とする
            if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
            {
                Messages.ShowErrorMessage($"\"{path}\"は見つかりません。\n削除されたか名前が変更された可能性があります。");
                RedoPathes.Clear();
                return;
            }

            Items = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            switch (e.Property.Name)
            {
                case nameof(Items):
                    if (!(e.NewValue is IList<string> newValue))
                        break;

                    if (LastItems is not null)
                    {
                        if (!newValue.SequenceEqual(LastItems))
                        {
                            if (!IsUndoing)
                            {
                                if (LastItems.Any())
                                    UndoPathes.Push(string.Join('/', LastItems));
                                RedoPathes.Clear();
                            }

                            SetupButtons();
                        }
                    }
                    else
                    {
                        SetupButtons();
                    }

                    LastItems = new List<string>(newValue);

                    Button_Back.IsEnabled = Undoable;
                    Button_Next.IsEnabled = Redoable;
                    break;
            };
        }

        private void SetImage(string inPath, Image inImage)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = assembly.GetManifestResourceStream(inPath);
            bitmap.EndInit();

            inImage.Source = bitmap;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            StackPanel.Visibility = Visibility.Hidden;
            TextBox.Text = string.Join('\\', Items);

            TextBox.SelectAll();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            StackPanel.Visibility = Visibility.Visible;
            TextBox.Text = string.Empty;
        }

        private void OnInputEnterTextBox(object sender, ExecutedRoutedEventArgs inArgs)
        {
            var newDirectory = TextBox.Text.Replace('/', '\\');
            // null文字列は"PC"ディレクトリ（ドライブリスト）とする
            if (!string.IsNullOrEmpty(newDirectory) && !Directory.Exists(newDirectory))
            {
                Messages.ShowErrorMessage($"\"{newDirectory}\"は見つかりません。");
            }
            else
            {
                Items = newDirectory.Split('\\', options: StringSplitOptions.RemoveEmptyEntries);
            }


            DummyFocasable.Focus();
        }

        private void OnInputEscapeTextBox(object sender, ExecutedRoutedEventArgs inArgs)
        {
            DummyFocasable.Focus();
        }

        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            if (Undoable)
                Undo();
        }

        private void Button_Next_Click(object sender, RoutedEventArgs e)
        {
            if (Redoable)
                Redo();
        }

        private void SetupButtons()
        {
            StackPanel.Children.Clear();

            var items = new List<string>(Items);
            string path = "";
            foreach (var (i, item) in items.Indexed())
            {
                path += (i > 0) ? "/" + item : item;

                StackPanel.Children.Add(new DirectoryButton(path)
                {
                    Content = item,
                    OnDirectoryClick = i == items.Count - 1
                        ? null
                        : url => Items = url.Replace('\\', '/').Split('/', options: StringSplitOptions.RemoveEmptyEntries)
                });

                if (i < items.Count - 1)
                {
                    StackPanel.Children.Add(new SplitterButton(path)
                    {
                        OnDirectorySelect = url => Items = url.Replace('\\', '/').Split('/', options: StringSplitOptions.RemoveEmptyEntries)
                    });
                }
            }
        }

        private bool Undoable => UndoPathes.Any();
        private bool Redoable => RedoPathes.Any();
        private bool IsUndoing { get; set; } = false;
        private List<string>? LastItems { get; set; } = null;
        private readonly Stack<string> UndoPathes = new Stack<string>();
        private readonly Stack<string> RedoPathes = new Stack<string>();

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            "Items",
            typeof(IEnumerable<string>),
            typeof(DirectoryAddressBar),
            new PropertyMetadata(null)
        );

        public static RoutedCommand OnInputEnterTextBoxCommand = new RoutedCommand(nameof(OnInputEnterTextBoxCommand), typeof(DirectoryAddressBar));
        public static RoutedCommand OnInputEscapeTextBoxCommand = new RoutedCommand(nameof(OnInputEscapeTextBoxCommand), typeof(DirectoryAddressBar));

        private class DirectoryButton : Button
        {
            public DirectoryButton(string inDirectory)
            {
                Directory = inDirectory;

                Background = Brushes.Transparent;
                BorderBrush = Brushes.Transparent;
                Click += (sender, e) => OnDirectoryClick?.Invoke(Directory);
            }

            public Action<string>? OnDirectoryClick;

            private readonly string Directory;
        }

        private class SplitterButton : Button
        {
            public SplitterButton(string inDirectory)
            {
                Directory = inDirectory.EndsWith('\\')
                    ? inDirectory : $"{inDirectory}\\";

                Content = ">";
                Background = Brushes.Transparent;
                BorderBrush = Brushes.Transparent;

                Click += (sender, e) =>
                {
                    var menu = new ContextMenu();

                    foreach (var dir in System.IO.Directory.EnumerateDirectories(Directory).Select(d => System.IO.Path.GetFileName(d)))
                    {
                        var item = new MenuItem()
                        {
                            Header = dir,
                        };
                        item.Click += (sender, e) 
                            => OnDirectorySelect?.Invoke($"{Directory}/{((MenuItem)sender).Header}");

                        menu.Items.Add(item);
                    }

                    if (menu.Items.Count > 0)
                    {
                        menu.IsOpen = true;
                    }
                };
            }

            public Action<string>? OnDirectorySelect;

            private readonly string Directory;
        }
    }
}
