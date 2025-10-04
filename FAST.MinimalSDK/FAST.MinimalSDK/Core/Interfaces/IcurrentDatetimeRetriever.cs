namespace FAST.Core
{
    /// <summary>
    /// Interface to declare that a class containing the current date and time fields. 
    /// </summary>
    public interface IcurrentDatetimeRetriever
    {
        /// <summary>
        /// The current Time
        /// </summary>
        DateTime now { get; }

        /// <summary>
        /// The current date
        /// </summary>
        DateTime today { get; }
    }
}
