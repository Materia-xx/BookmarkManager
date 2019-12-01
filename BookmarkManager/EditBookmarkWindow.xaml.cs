using BookmarkDataProvider;
using BookmarkDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BookmarkManager
{
    /// <summary>
    /// Interaction logic for EditBookmarkWindow.xaml
    /// </summary>
    public partial class EditBookmarkWindow : Window
    {
        public BookmarkDto bookmark { get; set; }

        private static char[] charArraySpace = new char[] { ' ' };
        private List<string> tagHints = new List<string>();

        public EditBookmarkWindow()
        {
            InitializeComponent();
        }

        public void ShowEditor(int? bookmarkIdToEdit)
        {
            if (bookmarkIdToEdit == null)
            {
                bookmark = new BookmarkDto();

                // If we are creating a new entry then check the clipboard, there might be 
                // something copied there that we can auto-paste into the url field.
                var clipboardUrl = ClipboardUtil.GetUrlFromClipboard(checkFormat:true);
                if (!string.IsNullOrWhiteSpace(clipboardUrl))
                {
                    bookmark.Url = clipboardUrl;
                }
            }
            else
            {
                bookmark = DataProvider.DataStore.Value.Bookmarks.Value.Get(bookmarkIdToEdit.Value);
            }

            RenderBookmark();
            this.ShowDialog();
        }

        private void RenderBookmark()
        {
            txtTitle.Text = bookmark.Title;
            txtUrl.Text = bookmark.Url;

            stackTags.Children.Clear();
            foreach (var tag in bookmark.Tags)
            {
                RoutedEventHandler click = (clickSender, clickE) =>
                {
                    bookmark.Tags.Remove(tag);
                    RenderBookmark();
                };
                var tagPill = Pills.CreatePill(Pills.PillType.Chosen, tag, "X", click);
                stackTags.Children.Add(tagPill);
            }

            // If the title is missing then put the focus there
            if (string.IsNullOrWhiteSpace(bookmark.Title))
            {
                txtTitle.Focus();
            }
            // otherwise put it on the tags searcher
            else
            {
                txtTagSearch.Focus();
            }
        }

        private void RenderTagHints()
        {
            stackTagHints.Children.Clear();
            foreach (var tagHint in tagHints)
            {
                RoutedEventHandler click = (clickSender, clickE) =>
                {
                    AddTagToBookmark(tagHint);
                    tagHints.Remove(tagHint);
                    RenderBookmark();
                    RenderTagHints();
                };
                var tagPill = Pills.CreatePill(Pills.PillType.Unselected, tagHint, "+", click);
                stackTagHints.Children.Add(tagPill);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
            CenterWindowOnScreen();
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

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(bookmark.Title) || string.IsNullOrWhiteSpace(bookmark.Url))
            {
                MessageBox.Show("Title and URL are both required.");
                return;
            }

            CUDResult result;
            if (bookmark.Id == 0)
            {
                result = DataProvider.DataStore.Value.Bookmarks.Value.Insert(bookmark);
            }
            else
            {
                result = DataProvider.DataStore.Value.Bookmarks.Value.Update(bookmark);
            }

            if (result.Errors.Any())
            {
                MessageBox.Show(result.ErrorsCombined);
            }
            else
            {
                this.Close();
            }
        }

        private void TxtTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            bookmark.Title = txtTitle.Text;
        }

        private void TxtUrl_TextChanged(object sender, TextChangedEventArgs e)
        {
            bookmark.Url = txtUrl.Text;
        }

        private void BtnAddNewTag_Click(object sender, RoutedEventArgs e)
        {
            AddTagToBookmark(txtTagSearch.Text);
        }

        private void AddTagToBookmark(string tag)
        {
            if (!bookmark.Tags.Contains(tag))
            {
                bookmark.Tags.Add(tag);
                RenderBookmark();
            }
        }

        private void TxtTagSearch_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var allKeywords = txtTagSearch.Text.Split(charArraySpace, StringSplitOptions.RemoveEmptyEntries).ToList();
            var searchWord = allKeywords.Count > 0 ? allKeywords.Last() : string.Empty;

            // Show tag hints
            if (!string.IsNullOrWhiteSpace(searchWord))
            {
                var foundTags = DataProvider.DataStore.Value.Bookmarks.Value.TagsSearch(searchWord);
                tagHints.Clear();

                foreach (var foundTag in foundTags)
                {
                    tagHints.Add(foundTag);
                }
            }
            RenderTagHints();
        }
    }
}
