#undef MSEXT_LOGGING

using FAST.Core.Models;

#if MSEXT_LOGGING
using Microsoft.Extensions.Logging;
#endif

namespace FAST.Logging
{
    public static class logHelper
    {

        // https://learn.microsoft.com/en-us/javascript/api/@microsoft/signalr/loglevel?view=signalr-js-latest
        // Trace = 0, Debug = 1, Information = 2, Warning = 3, Error = 4, Critical = 5, and None = 6.

        public static levels errorTypeToLoggingLevel(errorTypes type)
        {
            switch (type)
            {
                case errorTypes.error:
                    return levels.error;
                case errorTypes.information:
                    return levels.info;
                case errorTypes.warning:
                    return levels.warning;
                default:
                    return levels.debug;
            }
        }
        public static errorTypes  loggingLevelToErrorType(levels level)
        {
            switch (level)
            {
                case levels.debug:
                    return errorTypes.information;
                case levels.error:
                    return errorTypes.error;
                case levels.fatal:
                    return errorTypes.error;
                case levels.info:
                    return errorTypes.information;
                case levels.trace:
                    return errorTypes.information;
                case levels.warning:
                    return errorTypes.warning;
                default:
                    return errorTypes.information;
            }
        }

#if MSEXT_LOGGING
        public static levels msLevelToFastLevel(LogLevel level)
        {
            switch(level)
            {
                case LogLevel.Trace:
                    return levels.trace;
                case LogLevel.Debug:
                    return levels.debug;
                case LogLevel.Information:
                    return levels.info;
                case LogLevel.Warning:
                    return levels.warning;
                case LogLevel.Error:
                    return levels.error;
                case LogLevel.Critical:
                    return levels.fatal;
                case LogLevel.None:
                    return levels.none;
                default:
                    throw new Exception($"Cannot map level: {level}");
            }
        }
        public static LogLevel fastLevelToMSLevel(levels level)
        {
            switch (level)
            {
                case levels.trace:
                    return LogLevel.Trace;
                case levels.debug:
                    return LogLevel.Debug;
                case levels.info:
                    return LogLevel.Information;
                case levels.warning:
                    return LogLevel.Warning;
                case levels.error:
                    return LogLevel.Error;
                case levels.fatal:
                    return LogLevel.Critical;
                case levels.none:
                    return LogLevel.None;
                default:
                    throw new Exception($"Cannot map level: {level}");
            }
        }
#endif
    }
}
