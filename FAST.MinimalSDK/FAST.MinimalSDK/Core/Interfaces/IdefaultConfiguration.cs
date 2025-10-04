namespace FAST.Core
{
    /// <summary>
    /// Interface indicates that a class supports default configuration management properties
    /// </summary>
    public interface IdefaultConfiguration
    {
        /// <summary>
        /// The default configuration file name
        /// </summary>
        string defaultConfigFileName { get; }
    }
}
