namespace FAST.Core
{

    /// <summary>
    /// Indicates that a class supports example serialization or example data output
    /// </summary>
    public interface Iexample
    {

        /// <summary>
        /// Method with a file path to store the example
        /// </summary>
        /// <param name="filePath">Full file path</param>
        void example(string filePath);
    }
}