using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BookmarkManager
{
    public static class Pills
    {
        public enum PillType
        {
            /// <summary>
            /// Displayed, but not selected
            /// </summary>
            Unselected,
            /// <summary>
            /// Selected but not chosen
            /// </summary>
            Selected,
            Chosen
        }

        private class PillStyle
        {
            private static CornerRadius pillCornerRadius = new CornerRadius(10);
            private static Thickness pillMargin = new Thickness(2);
            private static FontFamily pillFontFamily = new FontFamily("Consolas");

            public Thickness BorderThickness { get; set; }
            public Brush BorderBrush { get; set; }
            public Brush BackgroundBrush { get; set; }

            public void ModifyStyle(Border pill)
            {
                pill.Margin = pillMargin;
                pill.BorderBrush = BorderBrush;
                pill.BorderThickness = BorderThickness;
                pill.CornerRadius = pillCornerRadius;
                pill.Background = BackgroundBrush;

                StackPanel childStack = pill.Child as StackPanel;
                foreach (var child in childStack.Children)
                {
                    if (child is Label)
                    {
                        (child as Label).FontFamily = pillFontFamily;
                    }
                    else if (child is Border)
                    {
                        var buttonBorder = child as Border;
                        buttonBorder.BorderThickness = BorderThickness;
                        buttonBorder.CornerRadius = pillCornerRadius;
                        buttonBorder.BorderBrush = BorderBrush;

                        var button = buttonBorder.Child as Button;
                        button.BorderThickness = new Thickness(0);
                        button.Background = BackgroundBrush;
                    }
                }
            }
        }

        private static Dictionary<PillType, PillStyle> PillStyles = new Dictionary<PillType, PillStyle>()
        {
            [PillType.Unselected] = new PillStyle()
            {
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.DarkGray,
                BackgroundBrush = Brushes.Gray
            },

            [PillType.Selected] = new PillStyle()
            {
                BorderThickness = new Thickness(2),
                BorderBrush = Brushes.Gray,
                BackgroundBrush = Brushes.LightGray
            },

            [PillType.Chosen] = new PillStyle()
            {
                BorderThickness = new Thickness(2),
                BorderBrush = Brushes.DarkGreen,
                BackgroundBrush = Brushes.Green
            }
        };

        public static string GetPillText(Border pill)
        {
            StackPanel childStack = pill.Child as StackPanel;
            foreach (var child in childStack.Children)
            {
                if (child is Label)
                {
                    return (child as Label).Content.ToString();
                }
            }
            return null;
        }

        public static Border CreatePill(PillType pillType, string text, string buttonText = null, RoutedEventHandler clickEvent = null)
        {
            var pillBorder = new Border();
            pillBorder.Width = double.NaN;

            var stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            pillBorder.Child = stack;

            var lblTag = new Label();
            lblTag.Width = double.NaN;
            lblTag.Content = text;
            lblTag.VerticalAlignment = VerticalAlignment.Center;
            stack.Children.Add(lblTag);

            if (!string.IsNullOrWhiteSpace(buttonText))
            {
                // TODO: Instead store this style in a resources xaml and just apply it here if possible. Same with the other pill styles.
                var buttonBorder = new Border();
                buttonBorder.Height = 25;
                buttonBorder.Width = 25;
                var button = new Button();
                button.Content = buttonText;
                button.Width = 15;
                button.Height = 15;
                button.Margin = new Thickness(0, 0, 0, 2);
                button.HorizontalAlignment = HorizontalAlignment.Center;
                button.VerticalAlignment = VerticalAlignment.Center;

                if (clickEvent != null)
                {
                    button.Click += clickEvent;
                }

                // TODO: update the hover style to look better
                buttonBorder.Child = button;
                stack.Children.Add(buttonBorder);
            }

            PillStyles[pillType].ModifyStyle(pillBorder);
            return pillBorder;
        }

        public static Tuple<int, int> GetBookmarkIdFromPill(Border pill)
        {
            if (pill?.Tag != null)
            {
                try
                {
                    var bookmarkIds = pill.Tag as Tuple<int, int>;
                    return bookmarkIds;
                }
                catch
                {
                    // Ignore
                }
            }
            return null;
        }

        public static void ModifyPillStyle(PillType targetPillType, Border pill)
        {
            PillStyles[targetPillType].ModifyStyle(pill);
        }
    }
}
