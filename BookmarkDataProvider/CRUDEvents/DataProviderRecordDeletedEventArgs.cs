using System;

namespace BookmarkDataProvider.CRUDEvents
{
    public class DataProviderRecordDeletedEventArgs : EventArgs
    {
        /// <summary>
        /// The Dto as it was before being deleted
        /// </summary>
        public object DtoBefore { get; set; }
    }
}
