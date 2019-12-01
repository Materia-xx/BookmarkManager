using System;

namespace BookmarkDataProvider.CRUDEvents
{
    public class DataProviderRecordCreatedEventArgs : EventArgs
    {
        /// <summary>
        /// The new Dto that was created
        /// </summary>
        public object CreatedDto { get; set; }
    }
}
