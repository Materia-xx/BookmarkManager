using BookmarkDtos;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookmarkDataProvider
{
    /// <summary>
    /// Provides additional functions for interacting with the schema table that are not provided in the base CRUD class
    /// </summary>
    public class SchemaRU : DataProviderCRUDBase<SchemaDto>
    {
        public SchemaRU(LiteDatabase liteDatabase, string collectionName) : base(liteDatabase, collectionName)
        {
        }

        public void CheckVersion(int programVersion)
        {
            int currentSchemaVersion = GetCurrentVersion();
            if (currentSchemaVersion == programVersion)
            {
                // Program matches schema
                return;
            }

            if (programVersion < currentSchemaVersion)
            {
                // The program is an old version, the db has been upgraded by another program
                // The user of this program needs to update to the latest program version before
                // running it again in order to avoid potential db corruption.
                // TODO: Throwing for now, though something more elegant could be put in place.
                throw new Exception("Schema is higher than program, please update to the newest program version.");
            }

            // Now we need to upgrade. 1 version at a time.
            for (int updateToVersion = currentSchemaVersion; updateToVersion <= currentSchemaVersion; updateToVersion++)
            {
                switch (updateToVersion)
                {
                    case 2:
                        UpgradeToVersion_2();
                        break;
                }
            }
        }

        private void UpgradeToVersion_2()
        {
            SetCurrentVersion(2);

            // There isn't a version 2 schema yet, this is the place to put upgrade code when needed.
        }

        private void SetCurrentVersion(int newSchemaVersion)
        {
            var allRecords = base.GetAll();
            if (!allRecords.Any())
            {
                var chosenRecord = new SchemaDto()
                {
                    Version = newSchemaVersion
                };
                base.Insert(chosenRecord);
            }
            else
            {
                var chosenRecord = allRecords.Last();
                chosenRecord.Version = newSchemaVersion;
                base.Update(chosenRecord);
            }
        }

        private int GetCurrentVersion()
        {
            var allRecords = base.GetAll();
            if (!allRecords.Any())
            {
                SetCurrentVersion(1);
            }
            return base.GetAll().Last().Version;
        }
    }
}
