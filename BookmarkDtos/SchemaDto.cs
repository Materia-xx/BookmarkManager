using System.Collections.Generic;

namespace BookmarkDtos
{
    public class SchemaDto : IdBasedObject
    {
        /// <summary>
        /// Represents the "schema" version of the DTOs recorded in the database.
        /// Used by the program to update older versions of the db when found
        /// </summary>
        public int Version { get; set; }
    }
}
