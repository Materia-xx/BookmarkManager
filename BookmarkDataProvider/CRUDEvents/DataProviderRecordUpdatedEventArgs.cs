using System;

namespace BookmarkDataProvider.CRUDEvents
{
    public class DataProviderRecordUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// The Dto as it was before the update
        /// </summary>
        public object DtoBefore { get; set; }

        /// <summary>
        /// The Dto as it was after the update
        /// </summary>
        public object DtoAfter { get; set; }
    }
}
