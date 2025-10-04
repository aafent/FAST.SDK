namespace FAST.Logging
{
    public interface Ilogger
    {
        void debug(string message, string extra = null);
        void error(Exception carryingException, string source = null, string extra = null);
        void error(string message, string source = null, string extra = null);
        void error(string message, Exception carryingException, string source = null, string extra = null);
        void fatal(Exception carryingException, string source = null, string extra = null);
        void fatal(string message, string source = null, string extra = null);
        void fatal(string message, Exception carryingException, string source = null, string extra = null);
        void info(string message, string source = null, string extra = null);
        void trace(string message, string extra = null);
        void warning(string message, string source = null, string extra = null);
    }
}