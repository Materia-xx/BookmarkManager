using BookmarkDataProvider;
using BookmarkDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookmarkManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HotKey activationHotkey;
        private static char[] charArraySpace = new char[] { ' ' };

        private List<string> previousSearchTerms = new List<string>();
        private List<string> previousSearchTags = new List<string>();
        private string previousTagsSearchTerm = null;
        private int previousTagSearchChosenTagsCount = 0;

        private List<string> chosenTags = new List<string>();
        private int selectedTagIndex = -1;
        private int selectedBookmarkIndex = -1;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
            //BookmarkTestData.InsertTestRecords(); // Uncomment to create test records

            // Get db tables/cache loaded up as fast as possible so UI is more snappy on the first db hit.
            DataProvider.DataStore.Value.Bookmarks.Value.TagsSearch("#pre-cache#");
            CenterWindowOnScreen();
            RegisterActivationHotkey();

            // If there is a URL on the clipboard, do a search for it. This makes it faster to edit an
            // already existing bookmark in the database.
            // re:(checkFormat=false) Searching for any url string isn't harmful, so there isn't any validation on 
            // if it contains http or something.
            var clipboardUrl = ClipboardUtil.GetUrlFromClipboard(checkFormat:false);
            if (!string.IsNullOrWhiteSpace(clipboardUrl))
            {
                SearchForBookmark(clipboardUrl);
            }

            txtSearch.Focus();
        }

        private void RegisterActivationHotkey()
        {
            activationHotkey = new HotKey(ModifierKeys.Control | ModifierKeys.Shift, Keys.X, this); // TODO: make this configurable and store the value in the db
            activationHotkey.HotKeyPressed += Activation_HotKeyPressed;
        }

        private void Activation_HotKeyPressed(HotKey obj)
        {
            ClearSearchAndInputs();
            this.Visibility = Visibility.Visible;
        }

        private void ClearSearchAndInputs()
        {
            txtSearch.Text = "";
            chosenTags.Clear();
            stackTagsChosen.Children.Clear();
            DoSearches(true);
            this.UpdateLayout();
        }

        private void CenterWindowOnScreen()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }

        #region Tag selection
        private void ShowTagHints(string searchTag, bool force)
        {
            // If we're already showing the results for this term then don't do it again
            // We're not checking the actual chosen tags one by one because they cannot be unselected currently
            if (!force)
            {
                if (searchTag == previousTagsSearchTerm && chosenTags.Count == previousTagSearchChosenTagsCount)
                {
                    return;
                }
            }

            selectedTagIndex = -1;
            previousTagsSearchTerm = searchTag;
            previousTagSearchChosenTagsCount = chosenTags.Count;
            if (!string.IsNullOrWhiteSpace(searchTag))
            {
                var foundTags = DataProvider.DataStore.Value.Bookmarks.Value.TagsSearch(searchTag);
                stackTagSelector.Children.Clear();
                double chosenTagsWidth = GetChosenTagsWidth();
                double remainingWidth = stackTagSelector.ActualWidth - chosenTagsWidth;
                foreach (var foundTag in foundTags)
                {
                    // Only add this hint tag if it is not already a chosen tag
                    if (!chosenTags.Contains(foundTag))
                    {
                        var tagPill = Pills.CreatePill(Pills.PillType.Unselected, foundTag);
                        
                        // Only show the tag if there is enough space to do it without overlapping with the chosen tags
                        stackTagSelector.Children.Add(tagPill);
                        double hintTagsWidth = GetHintTagsWidth();
                        if (hintTagsWidth > remainingWidth)
                        {
                            stackTagSelector.Children.Remove(tagPill);
                            break;
                        }
                    }
                }
            }
            else
            {
                stackTagSelector.Children.Clear();
            }
        }

        private double GetChosenTagsWidth()
        {
            stackTagsChosen.UpdateLayout();
            double measuredWidth = 0;
            foreach (var tag in stackTagsChosen.Children)
            {
                measuredWidth += (tag as Border).ActualWidth + 2; // 2 is the known margin on tags
            }
            return measuredWidth;
        }

        private double GetHintTagsWidth()
        {
            stackTagSelector.UpdateLayout();
            double measuredWidth = 0;
            foreach (var tag in stackTagSelector.Children)
            {
                measuredWidth += (tag as Border).ActualWidth + 2; // 2 is the known margin on tags
            }
            return measuredWidth;
        }

        private void StackTagSelector_SelectNextTag()
        {
            int tagCount = stackTagSelector.Children.Count;

            selectedTagIndex++;
            if (selectedTagIndex >= tagCount)
            {
                selectedTagIndex = -1;
            }

            StackResults_RenderTagSelectorStyles();
        }

        private void StackTagSelector_SelectPreviousTag()
        {
            int tagCount = stackTagSelector.Children.Count;

            selectedTagIndex--;
            if (selectedTagIndex < -1)
            {
                selectedTagIndex = tagCount - 1;
            }

            StackResults_RenderTagSelectorStyles();
        }

        private void StackResults_RenderTagSelectorStyles()
        {
            int tagCount = stackTagSelector.Children.Count;

            // Update borders to show the chosen one
            for (int tagIndex = 0; tagIndex < tagCount; tagIndex++)
            {
                var tagPill = stackTagSelector.Children[tagIndex] as Border;
                if (selectedTagIndex == -1 || selectedTagIndex != tagIndex)
                {
                    Pills.ModifyPillStyle(Pills.PillType.Unselected, tagPill);
                }
                else
                {
                    Pills.ModifyPillStyle(Pills.PillType.Selected, tagPill);
                }
            }
        }
        
        private void ChooseSelectedTag()
        {
            int tagCount = stackTagSelector.Children.Count;
            if (tagCount == 0)
            {
                return;
            }

            var selectedTagPill = stackTagSelector.Children[selectedTagIndex] as Border;
            var selectedTag = Pills.GetPillText(selectedTagPill);
            chosenTags.Add(selectedTag);
            var chosenTag = Pills.CreatePill(Pills.PillType.Chosen, selectedTag);
            stackTagsChosen.Children.Add(chosenTag);
            stackTagSelector.Children.Clear();
        }
        #endregion

        #region Bookmark Results

        private void AddBookmarkDtoAsSearchResult(BookmarkDto bookmark, int resultIndex)
        {
            if (bookmark != null)
            {
                // Create pill with embedded bookmark id
                var resultPill = Pills.CreatePill(Pills.PillType.Unselected, bookmark.Title);

                // Add the right click context menu
                // Tag stores the result index and the bookmark id
                // This is a different thing than the tags stored in bookmarks
                resultPill.Tag = new Tuple<int, int>(resultIndex, bookmark.Id);
                ContextMenu pillMenu = new ContextMenu();
                resultPill.ContextMenu = pillMenu;
                // Edit
                MenuItem editBookmarkCommand = new MenuItem();
                editBookmarkCommand.Header = "Edit";
                editBookmarkCommand.Click += EditBookmarkCommand_Click;
                pillMenu.Items.Add(editBookmarkCommand);
                // Delete
                MenuItem deleteBookmarkCommand = new MenuItem();
                deleteBookmarkCommand.Header = "Delete";
                deleteBookmarkCommand.Click += DeleteBookmarkCommand_Click;
                pillMenu.Items.Add(deleteBookmarkCommand);

                // Add a right mouse down event that highlights the item being right clicked on
                resultPill.MouseDown += ResultPill_MouseDown;

                stackResult.Children.Add(resultPill);
            }
        }

        private void DeleteBookmarkCommand_Click(object sender, RoutedEventArgs e)
        {
            MenuItem editBookmarkCommand = sender as MenuItem;
            ContextMenu pillMenu = editBookmarkCommand.Parent as ContextMenu;
            Border pillResult = pillMenu.PlacementTarget as Border;
            var bookmarkIds = Pills.GetBookmarkIdFromPill(pillResult);
            var bookmark = DataProvider.DataStore.Value.Bookmarks.Value.Get(bookmarkIds.Item2);

            var msgResponse = MessageBox.Show($"Delete '{bookmark.Title}'?", "Delete bookmark", MessageBoxButton.YesNo);
            if (msgResponse != MessageBoxResult.Yes)
            {
                return;
            }

            DataProvider.DataStore.Value.Bookmarks.Value.Delete(bookmarkIds.Item2);
            DoSearches(true);
            txtSearch.Focus();
        }

        private void ResultPill_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Border pillResult = sender as Border;
            var bookmarkIds = Pills.GetBookmarkIdFromPill(pillResult);
            selectedBookmarkIndex = bookmarkIds.Item1;
            StackResults_RenderBookmarkResultStyles();
        }

        private void EditBookmarkCommand_Click(object sender, RoutedEventArgs e)
        {
            MenuItem editBookmarkCommand = sender as MenuItem;
            ContextMenu pillMenu = editBookmarkCommand.Parent as ContextMenu;
            Border pillResult = pillMenu.PlacementTarget as Border;
            var bookmarkIds = Pills.GetBookmarkIdFromPill(pillResult);

            this.Visibility = Visibility.Collapsed;
            var bookmarkEditor = new EditBookmarkWindow();
            bookmarkEditor.ShowEditor(bookmarkIds.Item2);
            this.Visibility = Visibility.Visible;
            DoSearches(true);
            txtSearch.Focus();
        }

        private void BtnAddBookmark_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            var bookmarkEditor = new EditBookmarkWindow();
            bookmarkEditor.ShowEditor(null);
            this.Visibility = Visibility.Visible;
            DoSearches(true);
            txtSearch.Focus();
        }

        private void SearchForBookmark(string url)
        {
            var bookmark = DataProvider.DataStore.Value.Bookmarks.Value.Get_ForUrl(url);

            if (bookmark != null)
            {
                previousSearchTerms = new List<string>();
                previousSearchTags = new List<string>();
                selectedBookmarkIndex = -1;
                stackResult.Children.Clear();
                AddBookmarkDtoAsSearchResult(bookmark, 0);
            }
        }

        private void SearchForBookmarks(List<string> searchTerms, List<string> searchTags, bool force)
        {
            // If we are already showing the results for this term/tag combo then don't do it again
            if (!force)
            {
                if (searchTerms.Count == previousSearchTerms.Count && searchTags.Count == previousSearchTags.Count)
                {
                    bool allMatch = true;
                    for (int i = 0; i < searchTerms.Count; i++)
                    {
                        if (searchTerms[i] != previousSearchTerms[i])
                        {
                            allMatch = false;
                            break;
                        }
                    }

                    if (allMatch)
                    {
                        for (int i = 0; i < searchTags.Count; i++)
                        {
                            if (searchTags[i] != previousSearchTags[i])
                            {
                                allMatch = false;
                                break;
                            }
                        }
                    }

                    if (allMatch)
                    {
                        return;
                    }
                }
            }

            previousSearchTerms = searchTerms;
            previousSearchTags = searchTags;
            selectedBookmarkIndex = -1;
            stackResult.Children.Clear();
            var bookmarks = DataProvider.DataStore.Value.Bookmarks.Value.BookmarksSearch(searchTerms, searchTags).Take(10);
            // Show max of 10 results
            var visibleBookmarks = bookmarks.Take(10).ToList();
            for (int resultIndex = 0; resultIndex < visibleBookmarks.Count; resultIndex++)
            {
                var bookmark = visibleBookmarks[resultIndex];
                AddBookmarkDtoAsSearchResult(bookmark, resultIndex);
            }
        }

        private void StackResults_SelectNextBookmark()
        {
            int resultsCount = stackResult.Children.Count;
            if (resultsCount == 0)
            {
                return;
            }

            selectedBookmarkIndex++;
            if (selectedBookmarkIndex >= resultsCount)
            {
                selectedBookmarkIndex = -1;
            }
            StackResults_RenderBookmarkResultStyles();
        }

        private void StackResults_SelectPreviousBookmark()
        {
            int resultsCount = stackResult.Children.Count;
            if (resultsCount == 0)
            {
                return;
            }

            selectedBookmarkIndex--;
            if (selectedBookmarkIndex < 0)
            {
                selectedBookmarkIndex = resultsCount - 1;
            }
            StackResults_RenderBookmarkResultStyles();
        }

        private void StackResults_RenderBookmarkResultStyles()
        {
            int resultsCount = stackResult.Children.Count;

            // Update borders to show the chosen one
            for (int resultIndex = 0; resultIndex < resultsCount; resultIndex++)
            {
                var resultPill = stackResult.Children[resultIndex] as Border;
                if (selectedBookmarkIndex == -1 || selectedBookmarkIndex != resultIndex)
                {
                    Pills.ModifyPillStyle(Pills.PillType.Unselected, resultPill);
                }
                else
                {
                    Pills.ModifyPillStyle(Pills.PillType.Selected, resultPill);
                }
            }
        }

        private bool OpenBookmark(int resultIndex)
        {
            var resultPill = stackResult.Children[resultIndex] as Border;
            var bookmarkIds = Pills.GetBookmarkIdFromPill(resultPill);
            var bookmark = DataProvider.DataStore.Value.Bookmarks.Value.Get(bookmarkIds.Item2);
            if (!string.IsNullOrWhiteSpace(bookmark?.Url))
            {
                System.Diagnostics.Process.Start(bookmark.Url);
                return true;
            }
            return false;
        }
        #endregion

        private void DoSearches(bool force)
        {
            var allKeywords = txtSearch.Text.Split(charArraySpace, StringSplitOptions.RemoveEmptyEntries).ToList();

            var searchWord = allKeywords.Count > 0 ? allKeywords.Last() : string.Empty;

            // Show tag hints
            ShowTagHints(searchWord, force);

            // Show search results
            SearchForBookmarks(allKeywords, chosenTags, force);
        }

        private void TxtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            // Esc closes the search window
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                ClearSearchAndInputs();
                this.Visibility = Visibility.Hidden;
                return;
            }

            // Tab cycles through selectable tags
            if (e.Key == Key.Tab)
            {
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    StackTagSelector_SelectPreviousTag();
                }
                else
                {
                    StackTagSelector_SelectNextTag();
                }
                e.Handled = true;
                return;
            }

            // Down selects the next bookmark
            if (e.Key == Key.Down)
            {
                StackResults_SelectNextBookmark();
                e.Handled = true;
                return;
            }

            // Up selects the previous bookmark
            if (e.Key == Key.Up)
            {
                StackResults_SelectPreviousBookmark();
                e.Handled = true;
                return;
            }

            // Return opens up the bookmark and closes the window
            if (e.Key == Key.Return && selectedBookmarkIndex > -1)
            {
                if (OpenBookmark(selectedBookmarkIndex))
                {
                    e.Handled = true;
                    ClearSearchAndInputs();
                    this.Visibility = Visibility.Hidden;
                    return;
                }
                else
                {
                    MessageBox.Show("Failed to open bookmark.");
                }
            }

            if (e.Key == Key.Space && selectedTagIndex != -1)
            {
                var allKeywords = txtSearch.Text.Split(charArraySpace, StringSplitOptions.RemoveEmptyEntries).ToList();
                ChooseSelectedTag();

                // Delete tag from search and make sure the spacing and cursor is right
                if (allKeywords.Count > 0)
                {
                    allKeywords.RemoveAt(allKeywords.Count - 1);
                }
                string newText = string.Join(" ", allKeywords);
                if (allKeywords.Count > 0)
                {
                    newText += " ";
                }
                txtSearch.Text = newText;
                txtSearch.CaretIndex = newText.Length;
                e.Handled = true;

                // Show search results, which may be different now that we have a chosen tag
                SearchForBookmarks(allKeywords, chosenTags, false);
                return;
            }

            DoSearches(false);
        }
    }
}
