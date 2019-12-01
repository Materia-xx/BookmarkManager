using BookmarkDtos;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookmarkDataProvider
{
    /// <summary>
    /// Provides additional functions for interacting with the Bookmarks table that are not provided in the base CRUD class
    /// </summary>
    public class BookmarksCRUD : DataProviderCRUDBase<BookmarkDto>
    {
        /// <summary>
        /// We keep a distinct set of tags in memory. I think it makes things faster, though not tested.
        /// </summary>
        public List<string> Tags { get; private set; }

        public BookmarksCRUD(LiteDatabase liteDatabase, string collectionName) : base(liteDatabase, collectionName)
        {
            UpdateTags();
        }

        public override CUDResult Insert(BookmarkDto insertingObject)
        {
            var opResult = new CUDResult();
            opResult = UpsertChecks(opResult, insertingObject);
            if (opResult.Errors.Any())
            {
                return opResult;
            }

            opResult = base.Insert(insertingObject);
            UpdateTags();
            return opResult;
        }

        public override CUDResult Update(BookmarkDto updatingObject)
        {
            var opResult = new CUDResult();
            opResult = UpsertChecks(opResult, updatingObject);
            if (opResult.Errors.Any())
            {
                return opResult;
            }

            opResult = base.Update(updatingObject);
            UpdateTags();
            return opResult;
        }

        public override CUDResult Delete(int deletingId)
        {
            var opResult = new CUDResult();
            opResult = base.Delete(deletingId);
            UpdateTags();
            return opResult;
        }

        public IEnumerable<string> TagsSearch(string searchString)
        {
            return Tags.Where(t => t.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) > -1);
        }

        public IEnumerable<BookmarkDto> BookmarksSearch(List<string> searchTerms, List<string> searchTags)
        {
            IEnumerable<BookmarkDto> result = Enumerable.Empty<BookmarkDto>();

            bool firstSearchDone = false;

            // Tag Search
            for (int i = 0; i < searchTags.Count; i++)
            {
                var searchTag = searchTags[i];
                if (!firstSearchDone)
                {
                    result = Find(b => b.Tags.Contains(searchTag));
                    firstSearchDone = true;
                }
                else
                {
                    result = result.Where(t => t.Tags.Contains(searchTag, StringComparer.OrdinalIgnoreCase));
                }
            }

            // Term Search
            for (int i = 0; i < searchTerms.Count; i++)
            {
                var searchTerm = searchTerms[i];
                if (!firstSearchDone)
                {
                    result = Find(b => b.Title.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) > -1);
                    firstSearchDone = true;
                }
                else
                {
                    result = result.Where(t => t.Title.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) > -1);
                }
            }

            return result;
        }

        /// <summary>
        /// returns the first bookmark with the given URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public BookmarkDto Get_ForUrl(string url)
        {
            var results = Find(s =>
                s.Url.Equals(url, StringComparison.Ordinal) // Note: case 'sensitive' compare so we allow renames to upper/lower case
            );
            return results.FirstOrDefault();
        }

        private CUDResult UpsertChecks(CUDResult opResult, BookmarkDto dto)
        {
            // If we are checking an insert operation
            if (dto.Id == 0)
            {
                if (Get_ForUrl(dto.Url) != null)
                {
                    opResult.Errors.Add($"A bookmark with this url already exists.");
                }
            }
            // If we are checking an update operation
            {
                // Look for bookmarks with this url that are not this record
                var dupeResults = Find(s =>
                    s.Id != dto.Id
                    && s.Url.Equals(dto.Url, StringComparison.Ordinal) // Note: case 'sensitive' compare so we allow renames to upper/lower case
                );

                if (dupeResults.Any())
                {
                    opResult.Errors.Add($"A bookmark with this url already exists.");
                    return opResult;
                }
            }

            return opResult;
        }

        private void UpdateTags()
        {
            Tags = base.GetAll().SelectMany(b => b.Tags).Distinct().OrderBy(t => t).ToList();
        }
    }
}
