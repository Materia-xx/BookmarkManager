using LiteDB;
using System;

namespace BookmarkDataProvider
{
    /// <summary>
    /// A data provider that interacts with the Bookmark data store database
    /// </summary>
    public class BookmarkDataProviderDataStore
    {
        public Lazy<BookmarksCRUD> Bookmarks { get; private set; }

        public BookmarkDataProviderDataStore(LiteDatabase dataStoreDatabase)
        {
            Bookmarks = new Lazy<BookmarksCRUD>(() =>
            {
                return new BookmarksCRUD(dataStoreDatabase, "Bookmarks");
            });
        }
    }
}
