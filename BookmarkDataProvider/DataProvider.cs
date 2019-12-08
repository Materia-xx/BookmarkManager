using LiteDB;
using System;
using System.IO;

namespace BookmarkDataProvider
{
    public static class DataProvider
    {
        private static string BookmarkDbPath
        {
            get
            {
                var dbFolder = System.Configuration.ConfigurationManager.AppSettings["DBFolder"];
                if (string.IsNullOrWhiteSpace(dbFolder))
                {
                    dbFolder = "|ApplicationData|";
                }
                switch (dbFolder)
                {
                    case "|ApplicationData|":
                        string userFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                        dbFolder = Path.Combine(userFolderPath, "BookmarkManager");
                        break;
                    case "|NextToExe|":
                        dbFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                        break;
                }

                return Path.Combine(dbFolder, "Bookmark.db");
            }
        }

        private static LiteDatabase DataStoreDatabase
        {
            get
            {
                if (dataStoreDatabase == null)
                {
                    var dataStoreDbPath = BookmarkDbPath;

                    var dbFolder = Path.GetDirectoryName(dataStoreDbPath);
                    if (!Directory.Exists(dbFolder))
                    {
                        Directory.CreateDirectory(dbFolder);
                    }

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
