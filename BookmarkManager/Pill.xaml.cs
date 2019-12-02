using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BookmarkManager
{
    /// <summary>
    /// Interaction logic for Pill.xaml
    /// </summary>
    public partial class Pill : UserControl
    {
        public const string PillStyle_Unselected = "UnselectedPill";
        public const string PillStyle_Selected = "SelectedPill";
        public const string PillStyle_Chosen = "ChosenPill";

        public Pill()
        {
            InitializeComponent();
            PillButtonLabel.PreviewMouseLeftButtonUp += (sender, args) => RaiseClickEvent();
        }

        #region Pill Styles
        public Brush PillBorderBrush
        {
            get { return (Brush)GetValue(PillBorderBrushProperty); }
            set { SetValue(PillBorderBrushProperty, value); }
        }
        public static readonly DependencyProperty PillBorderBrushProperty =
            DependencyProperty.Register("PillBorderBrush", typeof(Brush), typeof(Pill),
                new PropertyMetadata(Brushes.Black));

        public Brush PillBackgroundBrush
        {
            get { return (Brush)GetValue(PillBackgroundBrushProperty); }
            set { SetValue(PillBackgroundBrushProperty, value); }
        }
        public static readonly DependencyProperty PillBackgroundBrushProperty =
            DependencyProperty.Register("PillBackgroundBrush", typeof(Brush), typeof(Pill),
                new PropertyMetadata(Brushes.Gray));

        public Thickness PillBorderThickness
        {
            get { return (Thickness)GetValue(PillBorderThicknessProperty); }
            set { SetValue(PillBorderThicknessProperty, value); }
        }
        public static readonly DependencyProperty PillBorderThicknessProperty =
            DependencyProperty.Register("PillBorderThickness", typeof(Thickness), typeof(Pill),
                new PropertyMetadata(new Thickness(1)));

        public Brush PillForegroundBrush
        {
            get { return (Brush)GetValue(PillForegroundBrushProperty); }
            set { SetValue(PillForegroundBrushProperty, value); }
        }
        public static readonly DependencyProperty PillForegroundBrushProperty =
            DependencyProperty.Register("PillForegroundBrush", typeof(Brush), typeof(Pill),
                new PropertyMetadata(Brushes.Black));
        #endregion

        #region Pill Button Styles
        public Brush PillButtonBorderBrush
        {
            get { return (Brush)GetValue(PillButtonBorderBrushProperty); }
            set { SetValue(PillButtonBorderBrushProperty, value); }
        }
        public static readonly DependencyProperty PillButtonBorderBrushProperty =
            DependencyProperty.Register("PillButtonBorderBrush", typeof(Brush), typeof(Pill),
                new PropertyMetadata(Brushes.Black));

        public Brush PillButtonBorderBrushMouseOver
        {
            get { return (Brush)GetValue(PillButtonBorderBrushMouseOverProperty); }
            set { SetValue(PillButtonBorderBrushMouseOverProperty, value); }
        }
        public static readonly DependencyProperty PillButtonBorderBrushMouseOverProperty =
            DependencyProperty.Register("PillButtonBorderBrushMouseOver", typeof(Brush), typeof(Pill),
                new PropertyMetadata(Brushes.Black));

        public Brush PillButtonBackgroundBrush
        {
            get { return (Brush)GetValue(PillButtonBackgroundBrushProperty); }
            set { SetValue(PillButtonBackgroundBrushProperty, value); }
        }
        public static readonly DependencyProperty PillButtonBackgroundBrushProperty =
            DependencyProperty.Register("PillButtonBackgroundBrush", typeof(Brush), typeof(Pill),
                new PropertyMetadata(Brushes.Gray));

        public Brush PillButtonBackgroundBrushMouseOver
        {
            get { return (Brush)GetValue(PillBackgroundBrushMouseOverProperty); }
            set { SetValue(PillBackgroundBrushMouseOverProperty, value); }
        }
        public static readonly DependencyProperty PillBackgroundBrushMouseOverProperty =
            DependencyProperty.Register("PillButtonBackgroundBrushMouseOver", typeof(Brush), typeof(Pill),
                new PropertyMetadata(Brushes.Gray));

        #endregion

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Pill));
        private void RaiseClickEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(Pill.ClickEvent);
            RaiseEvent(newEventArgs);
        }

        public void ModifyStyle(string pillStyle)
        {
            var styleResource = this.FindResource(pillStyle) as Style;
            if (styleResource != null)
            {
                this.Style = styleResource;
            }
        }

        public string GetPillText()
        {
            return PillLabel.Content.ToString();
        }

        public static Pill CreatePill(string pillStyle, string text, string buttonText = null, RoutedEventHandler clickEvent = null)
        {
            var pill = new Pill();
            pill.PillLabel.Content = text;

            if (!string.IsNullOrWhiteSpace(buttonText) && clickEvent != null)
            {
                pill.PillButtonLabel.Text = buttonText;
                pill.Click += clickEvent;
            }
            else
            {
                pill.PillButtonBorder.Visibility = Visibility.Collapsed;
            }

            pill.ModifyStyle(pillStyle);
            return pill;
        }

        public Tuple<int, int> GetBookmarkIdFromPill()
        {
            if (Tag != null)
            {
                try
                {
                    var bookmarkIds = Tag as Tuple<int, int>;
                    return bookmarkIds;
                }
                catch
                {
                    // Ignore
                }
            }
            return null;
        }
    }
}
