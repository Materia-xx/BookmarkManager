using LiteDB;
using System;
using System.IO;

namespace BookmarkDataProvider
{
    public static class DataProvider
    {
        public static string ProgramExeFolder
        {
            get
            {
                return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            }
        }

        internal static LiteDatabase DataStoreDatabase
        {
            get
            {
                if (dataStoreDatabase == null)
                {
                    var dataStoreDbPath = Path.Combine(ProgramExeFolder, "Bookmark.db");
                    dataStoreDatabase = new LiteDatabase(dataStoreDbPath);
                }
                return dataStoreDatabase;
            }
        }
        private static LiteDatabase dataStoreDatabase;

        public static Lazy<BookmarkDataProviderDataStore> DataStore = new Lazy<BookmarkDataProviderDataStore>(() =>
        {
            return new BookmarkDataProviderDataStore(DataStoreDatabase);
        });
    }
}
