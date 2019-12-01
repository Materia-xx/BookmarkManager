using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookmarkData
{
    public class BookmarkTestData : IBookmarkData
    {
        private List<string> testTags = new List<string>()
        {
            "Docs", "Cars", "Azure", "GitHub", "Graphics", "Board Games", "Build", "Release"
        };

        public IEnumerable<string> TagsSearch(string searchString)
        {
            return testTags.Where(t => t.Contains(searchString));
        }
    }
}
