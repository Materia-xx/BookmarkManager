using BookmarkDtos;
using System.Collections.Generic;

namespace BookmarkDataProvider
{
    public static class BookmarkTestData
    {
        public static void InsertTestRecords()
        {
            var bookMarksTable = DataProvider.DataStore.Value.Bookmarks.Value;

            bookMarksTable.Insert(
                new BookmarkDto()
                {
                    Title = "Center window on screen",
                    Url = "https://stackoverflow.com/questions/4019831/how-do-you-center-your-main-window-in-wpf",
                    Tags = new List<string>() { "TestRecord", "WPF" }
                });

            bookMarksTable.Insert(
                new BookmarkDto()
                {
                    Title = "Set window transparency",
                    Url = "https://stackoverflow.com/questions/21461017/wpf-window-with-transparent-background-containing-opaque-controls",
                    Tags = new List<string>() { "TestRecord", "WPF" }
                });

            bookMarksTable.Insert(
                new BookmarkDto()
                {
                    Title = "Bookmark managers on GitHub",
                    Url = "https://github.com/search?l=C%23&q=bookmark+manager&type=Repositories",
                    Tags = new List<string>() { "TestRecord", "GitHub", "BookmarkManager" }
                });

            bookMarksTable.Insert(
                new BookmarkDto()
                {
                    Title = "Azure Friday - Channel9",
                    Url = "https://channel9.msdn.com/Shows/Azure-Friday",
                    Tags = new List<string>() { "TestRecord", "Azure" }
                });

            bookMarksTable.Insert(
                new BookmarkDto()
                {
                    Title = "Pictures of cars",
                    Url = "https://www.bing.com/images/search?q=cars&FORM=HDRSC2",
                    Tags = new List<string>() { "TestRecord", "Cars", "Graphics" }
                });

            bookMarksTable.Insert(
                new BookmarkDto()
                {
                    Title = "LiteDb docs",
                    Url = "https://www.litedb.org/",
                    Tags = new List<string>() { "TestRecord", "LiteDB", "Docs" }
                });

            bookMarksTable.Insert(
                new BookmarkDto()
                {
                    Title = "Metroid Playthroughs",
                    Url = "https://www.youtube.com/user/ShadowMario3/playlists",
                    Tags = new List<string>() { "TestRecord", "VideoGame", "Metroid" }
                });

            bookMarksTable.Insert(
                new BookmarkDto()
                {
                    Title = "Zoë Johnston - Always",
                    Url = "https://www.youtube.com/watch?v=ZALrv7eMWbs",
                    Tags = new List<string>() { "TestRecord", "Above&Beyond" }
                });

            bookMarksTable.Insert(
                new BookmarkDto()
                {
                    Title = "Cassiopeia Project Quantum Electrodynamics",
                    Url = "https://www.youtube.com/watch?v=KZ67q4pv0HI",
                    Tags = new List<string>() { "TestRecord", "ParticlePhysics" }
                });

            bookMarksTable.Insert(
                new BookmarkDto()
                {
                    Title = "FortressCraft Evolved! (Steam)",
                    Url = "https://store.steampowered.com/app/254200/FortressCraft_Evolved/",
                    Tags = new List<string>() { "TestRecord", "VideoGame", "FortressCraft" }
                });

            bookMarksTable.Insert(
                new BookmarkDto()
                {
                    Title = "Hello World from scratch on a 6502",
                    Url = "https://www.youtube.com/watch?v=LnzuMJLZRdU&t=721s",
                    Tags = new List<string>() { "TestRecord", "C64", "Breadboard" }
                });
        }
    }
}
