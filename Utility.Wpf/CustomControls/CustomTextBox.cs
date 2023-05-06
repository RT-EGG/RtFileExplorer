using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Utility.Wpf.CustomControls
{
    public class CustomTextBox : TextBox
    {
        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public Brush PlaceholderForeground
        {
            get => (Brush)GetValue(PlaceholderForegroundProperty);
            set => SetValue(PlaceholderForegroundProperty, value);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Loaded += new RoutedEventHandler((sender, e) =>
            {
                _placeholderAdorner = new PlaceholderAdorner(this)
                {
                    Text = Placeholder,
                    Foreground = PlaceholderForeground,
                };
                AdornerLayer.GetAdornerLayer(this).Add(_placeholderAdorner);
            });
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            RefreshPlaceholder();
        }

        private void RefreshPlaceholder()
        {
            if (_placeholderAdorner is null)
                return;

            if (string.IsNullOrEmpty(Text) && !string.IsNullOrEmpty(Placeholder))
            {
                _placeholderAdorner.Text = Placeholder;
                _placeholderAdorner.Visibility = Visibility.Visible;
                _placeholderAdorner.SetFont(
                    new Thickness(3, 0, 0, 0),
                    FontSize, FontStyle, FontWeight
                );
            }
            else
            {
                _placeholderAdorner.Visibility = Visibility.Collapsed;
            }

        }

        private void RefreshPlaceholderForeground()
        {
            if (_placeholderAdorner is not null)
                _placeholderAdorner.Foreground = Foreground;
        }

        private PlaceholderAdorner? _placeholderAdorner;

        private static void OnPlaceholderChanged(DependencyObject inSender, DependencyPropertyChangedEventArgs inArgs)
        {
            if (inSender is not CustomTextBox sender)
                throw new ArgumentException("", nameof(inSender));

            sender.RefreshPlaceholder();
        }

        private static void OnPlaceholderForegroundChanged(DependencyObject inSender, DependencyPropertyChangedEventArgs inArgs)
        {
            if (inSender is not CustomTextBox sender)
                throw new ArgumentException("", nameof(inSender));

            sender.RefreshPlaceholderForeground();
        }

        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(
                "Placeholder", typeof(string), typeof(CustomTextBox), new PropertyMetadata("", OnPlaceholderChanged)
            );
        public static readonly DependencyProperty PlaceholderForegroundProperty = DependencyProperty.Register(
                "PlaceholderBrush", typeof(Brush), typeof(CustomTextBox),
                new PropertyMetadata(new SolidColorBrush(Colors.LightGray), OnPlaceholderForegroundChanged)
            );

        private class PlaceholderAdorner : Adorner
        {
            public PlaceholderAdorner(UIElement adornedElement)
                : base(adornedElement)
            {
                _placeholder = new TextBlock
                {
                    IsHitTestVisible = false
                };

                _visualChildren = new VisualCollection(this) {
                    _placeholder
                };
            }

            public string Text
            {
                get => _placeholder.Text;
                set => _placeholder.Text = value;
            }

            public Brush Foreground
            {
                get => _placeholder.Foreground;
                set => _placeholder.Foreground = value;
            }

            public void SetFont(Thickness inMargin, double inFontSize, FontStyle inFontStyle, FontWeight inFontWeight)
            {
                _placeholder.Margin = inMargin;
                _placeholder.FontSize = inFontSize;
                _placeholder.FontStyle = inFontStyle;
                _placeholder.FontWeight = inFontWeight;
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                if (AdornedElement is FrameworkElement element)
                {
                    _placeholder.Arrange(new Rect(
                        0, 0,
                        element.ActualWidth,
                        element.ActualHeight
                    ));
                }

                return base.ArrangeOverride(finalSize);
            }

            protected override int VisualChildrenCount => _visualChildren.Count;
            protected override Visual GetVisualChild(int index) => _visualChildren[index];

            private readonly TextBlock _placeholder;
            private readonly VisualCollection _visualChildren;
        }
    }
}
