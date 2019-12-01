namespace BookmarkDataProvider
{
    public static class CUDDepthTracking
    {
        /// <summary>
        /// Keeps track of the depth of Create, Update and Delete operations.
        /// It is possible to subscribe to Create/Update/Delete events and in-turn
        /// do additional CUD operations, which in-turn may chain into more, etc. 
        /// As protection against stack overflows caused by incorrectly coded
        /// routines, missing protections, etc, the depth of all CUD operations are tracked through this variable.
        /// The program will return an error through the <see cref="CUDResult"/> when the configured limit is exceeded.
        /// </summary>
        public static int OperationDepth { get; set; } = 0;

        /// <summary>
        /// The max operation depth allowed before a Create, Update or Delete operation will return an error.
        /// </summary>
        private static int OperationMaxDepth { get; set; } = 10;

        public static bool ExceedsMaxOperationDepth(CUDResult opResult)
        {
            if (OperationDepth > OperationMaxDepth)
            {
                opResult.Errors.Add($"Create, Update or Delete operations have exceeded the max depth of {OperationMaxDepth}.");
                return true;
            }
            return false;
        }
    }
}
