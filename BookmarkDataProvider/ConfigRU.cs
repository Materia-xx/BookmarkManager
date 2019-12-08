using BookmarkDtos;
using LiteDB;
using System.Linq;

namespace BookmarkDataProvider
{
    /// <summary>
    /// Provides additional functions for interacting with the schema table that are not provided in the base CRUD class
    /// </summary>
    public class ConfigRU : DataProviderCRUDBase<ConfigDto>
    {
        public ConfigRU(LiteDatabase liteDatabase, string collectionName) : base(liteDatabase, collectionName)
        {
        }

        public ConfigDto GetConfig()
        {
            var allRecords = base.GetAll();
            if (!allRecords.Any())
            {
                var newConfig = new ConfigDto();
                base.Insert(newConfig);
            }

            return allRecords.Last();
        }
    }
}
