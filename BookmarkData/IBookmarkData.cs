using System;
using System.Collections.Generic;

namespace BookmarkData
{
    public interface IBookmarkData
    {
        IEnumerable<string> TagsSearch(string searchString);
    }
}
