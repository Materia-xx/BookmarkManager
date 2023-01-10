using System.Collections.Generic;

namespace BookmarkDtos
{
    public class ConfigDto : IdBasedObject
    {
        public bool CloseSearchFormOnLostFocus { get; set; } = true;
        public bool CloseSearchFormOnSearch { get; set; } = true;
    }
}
