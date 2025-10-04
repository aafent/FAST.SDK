namespace FAST.Logging
{
    public interface IsupportLogging
    {
        bool isLoggerBind
        {
            get;
        }
        Ilogger logger
        {
            set;
        }
    }
}