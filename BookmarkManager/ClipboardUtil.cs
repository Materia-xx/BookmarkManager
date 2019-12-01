using System.Windows;

namespace BookmarkManager
{
    public static class ClipboardUtil
    {
        private static char[] charArrayLinebreaks = new char[] { '\r', '\n' };

        public static string GetUrlFromClipboard(bool checkFormat)
        {
            if (Clipboard.ContainsText(TextDataFormat.Text))
            {
                string clip = Clipboard.GetText(TextDataFormat.Text);
                var lines = clip.Split(charArrayLinebreaks);

                // A url that is intended to be used by the program will only be 1 line
                if (lines.Length > 1)
                {
                    return null;
                }

                if (checkFormat)
                {
                    if (!clip.ToLower().StartsWith("http"))
                    {
                        return null;
                    }
                    if (!clip.Contains("://"))
                    {
                        return null;
                    }
                }
                return clip;
            }
            return null;
        }
    }
}
