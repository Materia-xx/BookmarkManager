using LiteDB;
using System;

namespace BookmarkDataProvider
{
    /// <summary>
    /// A data provider that interacts with the Bookmark data store database
    /// </summary>
    public class BookmarkDataProviderDataStore
    {
        public static int CurrentProgramVersion = 1;
        public Lazy<SchemaRU> Schema { get; private set; }

        private Lazy<BookmarksCRUD> bookmarks = null;
        public Lazy<BookmarksCRUD> Bookmarks
        {
            get
            {
                Schema.Value.CheckVersion(CurrentProgramVersion);
                return bookmarks;
            }
        }

        private Lazy<ConfigRU> config = null;
        public Lazy<ConfigRU> Config
        {
            get
            {
                Schema.Value.CheckVersion(CurrentProgramVersion);
                return config;
            }
        }

        public BookmarkDataProviderDataStore(LiteDatabase dataStoreDatabase)
        {
            Schema = new Lazy<SchemaRU>(() =>
            {
                return new SchemaRU(dataStoreDatabase, "Schema");
            });

            bookmarks = new Lazy<BookmarksCRUD>(() =>
            {
                return new BookmarksCRUD(dataStoreDatabase, "Bookmarks");
            });

            config = new Lazy<ConfigRU>(() =>
            {
                return new ConfigRU(dataStoreDatabase, "Config");
            });
        }
    }
}
