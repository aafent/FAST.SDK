namespace FAST.Core
{

    /// <summary>
    /// Interface for classing that supporting copy of the internal cache element to a destination class
    /// </summary>
    public interface IdataCacheElement
    {
        /// <summary>
        /// Copy the inner data cache element to the destination object
        /// </summary>
        /// <param name="destination">The destination object</param>
        void copyTo(IdataCacheElement destination);
    }
}
