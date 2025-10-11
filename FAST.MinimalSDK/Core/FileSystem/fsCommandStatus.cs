namespace FAST.Core.FileSystem
{
    /// <summary>
    /// Status codes for file system commands.
    /// </summary>
    public enum fsCommandStatus : sbyte
    {
        /// <summary>
        /// The operation completed successfully.
        /// </summary>
        success = 0,

        /// <summary>
        /// A Not Found error occurred.
        /// </summary>
        notFound = -2,

        /// <summary>
        /// The item already exists.
        /// </summary>
        alreadyExists = -3,

        /// <summary>
        /// Access to the root file is prohibited.
        /// </summary>
        rootFileAccessProhibited = -4,

        /// <summary>
        /// An argument was empty or null.
        /// </summary>
        emptyArgument = -5,

        /// <summary>
        /// An destination item does not exists, error occurred.
        /// </summary>
        destinationDoesNotExists = -10,

        /// <summary>
        /// The item is not of the expected type (file or folder).
        /// </summary>
        expectedFile = -11,

        /// <summary>
        /// The item is not of the expected type (file or folder).
        /// </summary>
        expectedFolder = -12
    }
}
