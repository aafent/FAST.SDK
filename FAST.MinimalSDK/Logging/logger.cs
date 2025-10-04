using FAST.Strings;

namespace FAST.Logging
{
    /// <summary>
    /// defaultMessageFormat: is "{l} {d}: {s}\t:{m}"  
    ///                             {d} is the date, defaultDatestampFormat is "dd/MM/yyyy HH:mm:ss.fff"
    ///                             {s} is the source
    ///                             {m} is the message text
    ///                             {e} is the extra text
    ///                             {l} is the severity level
    /// </summary>
    public class logger : Ilogger
    {
        private loggerConfiguration _config = null;
        private bool _lockConfiguration = false; 
        public loggerConfiguration config
        {
            get
            {
                if ( _config == null ) _config=new loggerConfiguration();
                return _config;
            }
            set
            {
                if (!_lockConfiguration)
                {
                    _config = value;
                }
            }
        }

        public bool isConfigurationLocked
        {
            get
            {
                return _lockConfiguration;
            }
        }


        /// <summary>
        /// Constructor without parameters
        /// </summary>
        public logger()
        {
        }

        /// <summary>
        /// Constructor with one logging engine pre-routed
        /// </summary>
        /// <param name="logEngine">The logging engine</param>
        public logger(loggerEngines logEngine)
        {
            this.config.addRoute(logEngine);
        }

        /// <summary>
        /// Constructor with more than one logging engine pre-routed
        /// </summary>
        /// <param name="logEngine">Array of logging engines</param>
        public logger(loggerEngines[] logEngines)
        {
            foreach ( var logEngine in logEngines) 
            {
                this.config.addRoute(logEngine);
            }
        }


        /// <summary>
        /// Do not permit further logging configuration changes
        /// </summary>
        public void lockConfiguration()
        {
            config.lockConfiguration();
            _lockConfiguration = true;
        }

        /// <summary>
        /// Raw write logging details
        /// </summary>
        /// <param name="level">The level</param>
        /// <param name="source">The source</param>
        /// <param name="message">Empty string (exception must provided) or the message that will be combined with an exception (if will provided)</param>
        /// <param name="extra">Extra information</param>
        /// <param name="carryingException">null (message must provided) or/and an exception</param>
        public void write(levels level, string source, string message, string extra, Exception carryingException)
        {
            bool exceptionIsProvided = !(carryingException == null); 


            if (string.IsNullOrEmpty(message) & (! exceptionIsProvided)) return;

            if (string.IsNullOrEmpty(message)) message = string.Empty;

            if (exceptionIsProvided)
            {
                if (carryingException == null) return;


                // TODO a better CODE HERE
                if (!string.IsNullOrEmpty(message)) message+=stringValue.space;
                message +=carryingException.Message??string.Empty;
                if ( !string.IsNullOrEmpty(carryingException.StackTrace) )
                { 
                    if ( !string.IsNullOrEmpty(message) ) message+="\n";
                }


                if (string.IsNullOrEmpty(message)) return;
            }

            string inputMessage = message;

            string dateText = DateTime.Now.ToString(config.defaultDatestampFormat);
            string format = config.defaultMessageFormat;
            string severityMessage = "[!]";

            switch (level)
            {
                case levels.debug:
                    if (!config.enableDebug) return;
                    severityMessage = "[D]";
                    break;

                case levels.trace:
                    if (!config.enableTrace) return;
                    severityMessage = "[T]";
                    break;

                case levels.error:
                    severityMessage = "[E]";
                    break;


                case levels.fatal:
                    severityMessage = "[F]";
                    break;
                case levels.info:
                    severityMessage = "[I]";
                    break;
                case levels.warning:
                    severityMessage = "[W]";
                    break;
                default:
                    severityMessage = "[!]";
                    break;
            }


            format = format.Replace("{d}", "{0}");
            format = format.Replace("{s}", "{1}");
            format = format.Replace("{m}", "{2}");
            format = format.Replace("{e}", "{3}");
            format = format.Replace("{l}", "{4}");

            message = string.Format(format, dateText, source, message, extra, severityMessage);

            if (!config.loggerRoutes.ContainsKey(level)) { return; }
            IloggerAdapter slot;
            foreach (var engine in config.loggerRoutes[level])
            {
                switch (engine)
                {
                    case loggerEngines.doNotLog:
                        break;

                    case loggerEngines.console:
                        Console.WriteLine(message);
                        if (config.logOnBothFileAndConsole & config.hasConsoleRedirectionToFile)
                        {
                            config.writeLineToOriginalConsole(message);
                        }
                        break;

                    case loggerEngines.diagnostics:
                        diagnosticsWrite(level, source, inputMessage, extra, carryingException, message);
                        break;

                    case loggerEngines.fastTelemetry:
                        slot = config.slot(loggerConfiguration.slotOfFastTelemetry,this);
                        if (slot != null) slot.write(level, source, inputMessage, extra, carryingException, message);
                        break;

                    case loggerEngines.otherSlot1:
                        slot = config.slot(1,this);
                        if (slot != null) slot.write(level, source, inputMessage, extra, carryingException, message);
                        break;
                    case loggerEngines.otherSlot2:
                        slot = config.slot(2,this);
                        if (slot != null) slot.write(level, source, inputMessage, extra, carryingException, message);
                        break;
                    case loggerEngines.otherSlot3:
                        slot = config.slot(3, this);
                        if (slot != null) slot.write(level, source, inputMessage, extra, carryingException, message);
                        break;
                        
                    default:
                        Console.WriteLine(string.Format("NEXT {0} IS FOR THE UNSUPPORTED ENGINE: {1}", level.ToString(), engine.ToString()));
                        Console.WriteLine(message);
                        break;
                }
            }
            

        }

        public void debug(string message, string extra=null)
        {
            if (string.IsNullOrEmpty(extra)) extra = string.Empty;
            write(levels.debug, "DEBUG:", message, extra, null);
        }

        public void trace(string message, string extra=null)
        {
            if (string.IsNullOrEmpty(extra)) extra = string.Empty;
            write(levels.debug, "TRACE:", message, extra, null);
        }

        public void info(string message, string source = null, string extra = null)
        {
            if (string.IsNullOrEmpty(source))source = string.Empty;
            if (string.IsNullOrEmpty(extra)) extra = string.Empty;
            write(levels.info, source, message, extra, null);
        }

        public void warning(string message, string source=null, string extra=null)
        {
            if (string.IsNullOrEmpty(source)) source = config.defaultWarningSource;
            if (string.IsNullOrEmpty(extra)) extra = string.Empty;

            write(levels.warning, source, message, extra, null);
        }

        public void error(string message, string source=null, string extra=null)
        {
            if (string.IsNullOrEmpty(source)) source = config.defaultErrorSource;
            if (string.IsNullOrEmpty(extra)) extra = string.Empty;
            write(levels.error, source, message, extra, null);
        }
        public void error(string message, Exception carryingException, string source=null, string extra=null)
        {
            if (string.IsNullOrEmpty(source)) source = config.defaultErrorSource;
            if (string.IsNullOrEmpty(extra)) extra = string.Empty;
            write(levels.error, source, message, extra, carryingException);
        }
        public void error(Exception carryingException, string source=null, string extra=null)
        {
            if (string.IsNullOrEmpty(source)) source = config.defaultErrorSource;
            if (string.IsNullOrEmpty(extra)) extra = string.Empty;

            write(levels.error, source, null, extra, carryingException);
        }


        public void fatal(string message, string source=null, string extra=null)
        {
            if (string.IsNullOrEmpty(source)) source = config.defaultErrorSource;
            if (string.IsNullOrEmpty(extra)) extra = string.Empty;
            write(levels.fatal, source, message, extra, null);
        }
        public void fatal(string message, Exception carryingException, string source=null, string extra=null)
        {
            if (string.IsNullOrEmpty(source)) source = config.defaultErrorSource;
            if (string.IsNullOrEmpty(extra)) extra = string.Empty;
            write(levels.fatal, source, message, extra, carryingException);
        }
        public void fatal(Exception carryingException, string source=null, string extra=null)
        {
            if (string.IsNullOrEmpty(source)) source = config.defaultErrorSource;
            if (string.IsNullOrEmpty(extra)) extra = string.Empty;

            write(levels.fatal, source, null, extra, carryingException);
        }


        private void diagnosticsWrite(levels level, string source, string message, string extra, Exception carryingException, string formattedMessage)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

    }
}
