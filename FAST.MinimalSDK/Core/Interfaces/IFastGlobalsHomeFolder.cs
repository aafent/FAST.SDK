namespace FAST.Core.Interfaces
{
    /// <summary>
    /// Interface to declare the FASTGlobals Home Folder filed
    /// </summary>
    public interface IFastGlobalsHomeFolder
    {
        /// <summary>
        /// The home folder of the application
        /// </summary>
        string homeFolder { get; }
    }
}
