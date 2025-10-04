using FAST.Core.Models;
using System.Runtime.CompilerServices;

namespace FAST.Logging
{

    /// <summary>
    /// Static FAST Logger
    /// </summary>
    public static class fastLogger //: Ilogger
    {
        private static Ilogger _logger = null;
        private static loggerInLineNotation commands=null; 

        /// <summary>
        /// Inner logger, initially is the default and can change using the bindLogger() method.
        /// </summary>
        public static Ilogger log
        {
            get
            {
                if ( _logger == null )
                {
                    _logger = new logger();
                    commands = new(_logger);
                }
                return _logger;
            }
        }

        /// <summary>
        /// Static Property giving access to the Logger Commands without logging method (error,fatal,info etc)
        /// </summary>
        public static loggerInLineNotation L
        {
            get
            {
                initIfNotInit();
                return commands;
            }
        }

        /// <summary>
        /// Bind a new logger with the static fast logger (initially the default).
        /// </summary>
        /// <param name="logger"></param>
        /// <returns>Ilogger, previous binder logger</returns>
        public static Ilogger bindLogger(Ilogger logger)
        {
            var oldLogger = _logger;
            if (logger!=null) _logger=logger;
            commands = new(_logger);
            return oldLogger;
        }

        #region (+) same methods as the Ilogger interface, but statics with return type of loggerInLineNotation

        private static void initIfNotInit()
        {
            // (v) do not change this line
            var forceInit = log;  // (<) force the getter
        }

        public static loggerInLineNotation debug(string message, string extra = null)
        {
            initIfNotInit();
            commands.reset();
            commands.debug(message,extra);
            return commands;
        }
        public static loggerInLineNotation error(Exception carryingException, string source = null, string extra = null)
        {
            initIfNotInit();
            commands.reset();
            commands.error(carryingException, source , extra );
            return commands;
        }
        public static loggerInLineNotation error(IerrorCarrier objectWithError, string source = null, string extra = null)
        {
            initIfNotInit();
            commands.reset();
            commands.error(objectWithError.errorText, source, (extra??"")+(objectWithError.extendedErrorText??"")  );
            return commands;
        }
        public static loggerInLineNotation error(string message, string source = null, string extra = null)
        {
            initIfNotInit();
            commands.reset();
            commands.error(message, source, extra);
            return commands;
        }
        public static loggerInLineNotation error(string message, Exception carryingException, string source = null, string extra = null)
        {
            initIfNotInit();
            commands.reset();
            commands.error(message,carryingException,source,extra );
            return commands;
        }
        public static loggerInLineNotation fatal(Exception carryingException, string source = null, string extra = null)
        {
            initIfNotInit();
            commands.reset();
            commands.fatal(carryingException, source, extra);
            return commands;
        }
        public static loggerInLineNotation fatal(string message, string source = null, string extra = null)
        {
            initIfNotInit();
            commands.reset();
            commands.fatal(message,source,extra);
            return commands;
        }
        public static loggerInLineNotation fatal(string message, Exception carryingException, string source = null, string extra = null)
        {
            initIfNotInit();
            commands.reset();
            commands.fatal(message,carryingException, source,extra);
            return commands;
        }
        public static loggerInLineNotation info(string message, string source = null, string extra = null)
        {
            initIfNotInit();
            commands.reset();
            commands.info(message, source , extra);
            return commands;
        }
        public static loggerInLineNotation trace(string message, string extra = null)
        {
            initIfNotInit();
            commands.reset();
            commands.trace(message, extra );
            return commands;
        }
        public static loggerInLineNotation warning(string message, string source = null, string extra = null)
        {
            initIfNotInit();
            commands.reset();
            commands.warning(message, source , extra );
            return commands;
        }

        #endregion (+) same methods as the Ilogger interface, but statics




        /// <summary>
        /// Change the inner fast logger as far as the class is active
        ///   Use it in combination with using()
        ///   <example>using(new fastLogger.swapLogger(myLogger) { ...CODE HERE... } )</example>
        /// </summary>
        public class swapLogger : IDisposable
        {
            private Ilogger oldLogger;
            private Action codeAtEnd =null;


            /// <summary>
            /// Constructor with the new logger that will replace the existing
            /// </summary>
            /// <param name="newLogger"></param>
            public swapLogger(Ilogger newLogger)
            {
                this.oldLogger=fastLogger.bindLogger(newLogger);
            }


            /// <summary>
            /// Anonymous delegation to invoked at the end of the scope of the swapLogger instance
            /// <example>
            /// using(new fastLogger.swapLogger(myLogger).atEnd( ()=>myLogger.config.restoreConsoleRedirection() ))
            /// </example>
            /// </summary>
            /// <param name="action"></param>
            /// <returns></returns>
            public swapLogger atEnd(Action action)
            {
                this.codeAtEnd= action;
                return this;
            }

            /// <summary>
            /// Dispose method
            /// </summary>
            public void Dispose()
            {
                if ( this.codeAtEnd!=null ) codeAtEnd();
                fastLogger.bindLogger(this.oldLogger);
            }
        }

    }
} 