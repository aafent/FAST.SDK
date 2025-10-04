namespace FAST.Logging
{
    /// <summary>
    /// Interface to implement a logger slot for the FAST logger
    /// </summary>
    public interface IloggerAdapter
    {
        void write(levels level, string source, string message, string extra, Exception curringException, string formatedMessage);
        void init();
        void bind(Ilogger logger);
    }

}
