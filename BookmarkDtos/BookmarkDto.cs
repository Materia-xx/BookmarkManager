using System.Collections.Generic;

namespace BookmarkDtos
{
    public class BookmarkDto : IdBasedObject
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}
