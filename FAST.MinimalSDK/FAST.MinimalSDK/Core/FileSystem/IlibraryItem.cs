namespace FAST.Core.FileSystem
{
    /// <summary>
    /// Interface for classes that are supporting library items
    /// </summary>
    public interface IlibraryItem
    {
        /// <summary>
        /// The Library name
        /// </summary>
        string library { get; }

        /// <summary>
        /// The name of the Item in the Library
        /// </summary>
        string name { get; }
    }
}
