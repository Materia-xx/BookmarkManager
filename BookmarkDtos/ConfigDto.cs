using System.Collections.Generic;

namespace BookmarkDtos
{
    public class ConfigDto : IdBasedObject
    {
        public bool HotkeyShift { get; set; } = true;
        public bool HotkeyCtrl { get; set; } = true;
        public bool HotkeyAlt { get; set; }
        public string HotkeyKey { get; set; } = "X";

        public bool HideSearchFormOnLostFocus { get; set; } = true;
        public bool HideSearchFormOnSearch { get; set; } = true;
    }
}
