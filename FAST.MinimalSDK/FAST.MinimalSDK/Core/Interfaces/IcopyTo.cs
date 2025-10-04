namespace FAST.Core
{
    /// <summary>
    /// Interface for classes that are Cloneable and supporting copy method
    /// </summary>
    /// <typeparam name="T">The type of the destination object</typeparam>
    public interface IcopyTo<T> : ICloneable
    {
        /// <summary>
        /// Copy this contents or properties to a destination object
        /// </summary>
        /// <param name="dest">The destination object</param>
        /// <param name="doMerge">Optional, Merge with the destination values. Default is: true</param>
        void copyTo(T dest, bool doMerge=true);
    }
}