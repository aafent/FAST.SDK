using FAST.Core.Models;
using FAST.Logging;

namespace FAST.Core
{
    /// <summary>
    /// Error parser and helper for errors and exceptions
    /// </summary>
    public class errorParser
    {
        private elementaryMultiErrorContainer _errorContainer;

        /// <summary>
        /// The underlying error container, as single Error container 
        /// </summary>
        public IerrorCarrier singleError
        {
            get
            {
                if (_errorContainer == null ) _errorContainer = new();
                return _errorContainer;
            }
            set
            {
                if (_errorContainer == null) _errorContainer = new();
                copy(value, _errorContainer);
                add(_errorContainer);
            }
        }

        /// <summary>
        /// The underlying error container, as multi Error container 
        /// </summary>
        public ImultiErrorCarrier multiError
        {
            get
            {
                if (_errorContainer == null) _errorContainer = new();
                return _errorContainer;
            }
            set
            {
                if (_errorContainer == null) _errorContainer = new();
                copy(value, _errorContainer);
                add(_errorContainer);
            }
        }


        /// <summary>
        /// The error message
        /// </summary>
        public string message { get; set;  }

        /// <summary>
        /// The source of the error
        /// </summary>
        public string source { get; set; }

        /// <summary>
        /// The stack trace to the error
        /// </summary>
        public string stackTrace { get; set; }

        /// <summary>
        /// The trace separator
        /// </summary>
        public string traceSeparator = "\n[ {0} ]";

        [Obsolete("Use traceSeparator instead")]
        public string traceSeperator=> traceSeparator;

        /// <summary>
        /// The messages separator
        /// </summary>
        public string messageSeparator = "[ {0} ] ";

        [Obsolete("Use messageSeparator instead")]
        public string messageSeperator => messageSeparator;

        /// <summary>
        /// A collection with the errors
        /// </summary>
        public List<string> errors = new List<string>();

        /// <summary>
        /// The number of levels of the underlying error
        /// </summary>
        public int levels = 0;

        #region (+) Constructors 

        /// <summary>
        /// No arguments constructor, used to construct an error from scratch 
        /// </summary>
        public errorParser()
        {
            this.message = "";
            this.source = "";
            this.stackTrace = "";
            this.levels = 0;
        }

        /// <summary>
        /// Constructor with exception as argument
        /// </summary>
        /// <param name="ex"></param>
        public errorParser(Exception ex)
        {
            this.message = "";
            this.source = "";
            this.stackTrace = "";
            this.levels = 0;
            add(ex);
        }

        /// <summary>
        /// Constructor with a string message with the error text
        /// </summary>
        /// <param name="message"></param>
        public errorParser(string message)
        {
            this.stackTrace = "";
            this.levels = 0;
            add(message);
        }

        /// <summary>
        /// Constructor with an single error carrier as argument
        /// </summary>
        /// <param name="source"></param>
        public errorParser(IerrorCarrier source)
        {
            this.stackTrace = "";
            this.levels = 0;
            add(source);
        }

        /// <summary>
        /// Constructor with a multi error carrier as argument
        /// </summary>
        /// <param name="source"></param>
        public errorParser(ImultiErrorCarrier source)
        {
            this.stackTrace = "";
            this.levels = 0;
            add(source);
        }

        #endregion (+) Constructors 

        #region (+) add methods

        /// <summary>
        /// Add an exception to the underlying errors
        /// </summary>
        /// <param name="ex">The exception to add</param>
        /// <returns>Self</returns>
        public errorParser add(Exception ex)
        {
            for(;;)
            {
                if ( ex == null ) break;

                this.levels++;
                if (!string.IsNullOrEmpty(ex.Message))
                {
                    if (!string.IsNullOrEmpty(this.message)) this.message += string.Format(messageSeparator,this.levels);
                    errors.Add(ex.Message);
                    this.message += ex.Message;
                }


                if (!string.IsNullOrEmpty(ex.Source))
                {
                    if (!string.IsNullOrEmpty(this.source)) this.source += string.Format(", ");
                    this.source += ex.Source;
                }


                if (!string.IsNullOrEmpty(ex.StackTrace))
                {
                    if (!string.IsNullOrEmpty(this.stackTrace)) this.stackTrace += string.Format(traceSeparator, this.levels);
                    this.stackTrace += ex.StackTrace;
                }
        
                ex = ex.InnerException;

            }

            return this;

        }

        /// <summary>
        /// Add an error message to the underlying errors
        /// </summary>
        /// <param name="message">The message to add</param>
        /// <returns>Self</returns>
        public errorParser add(string message)
        {
            this.levels++;
            if (!string.IsNullOrEmpty(message))
            {
                errors.Add(message);
                if (!string.IsNullOrEmpty(this.message)) this.message += string.Format("\n[ L{0} ] ", this.levels);
                this.message += message;
            }

            string src = "App.Logic";
            if (!string.IsNullOrEmpty(src))
            {
                if (!string.IsNullOrEmpty(src)) this.source += string.Format(", ");
                this.source += src;
            }

            return this;
        }

        /// <summary>
        /// Add multi error carrier to the underlying errors
        /// </summary>
        /// <param name="source"></param>
        /// <returns>Self</returns>
        public errorParser add(IerrorCarrier source)
        {
            if ( source.hasError )
            { 
                string text=source.errorText;
                if ( !string.IsNullOrEmpty(source.extendedErrorText )) text+="\n"+source.extendedErrorText;
                add(text);
            }

            return this;
        }

        /// <summary>
        ///  Add single error carrier to the underlying errors
        /// </summary>
        /// <param name="source"></param>
        public errorParser add(ImultiErrorCarrier source)
        {
            if (source.hasError)
            {
                add((IerrorCarrier)source);
                foreach( var item in source.errors )
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        if ( item!=source.errorText) add(item);
                    }
                }
            }

            return this;
        }

        #endregion (+) add methods

        #region (+) other methods

        /// <summary>
        /// Log the error using the fast logger
        /// </summary>
        /// <param name="isFatal"></param>
        /// <returns>Self</returns>
        public errorParser logError(bool isFatal=false)
        {
            if (isFatal) fastLogger.fatal(this.ToString());
                    else fastLogger.error(this.ToString());
            return this;
        }

        /// <summary>
        /// Log the error as warning using the fast logger
        /// </summary>
        /// <returns>Self</returns>
        public errorParser logWarning()
        {
            fastLogger.error(this.ToString());
            return this;
        }

        /// <summary>
        /// Convert to a single error message
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return message;
        }

        /// <summary>
        /// Check if there is an error in an single error carrier
        /// </summary>
        /// <param name="errorCarrier">The error carrier</param>
        /// <returns>Boolean, true if has errors</returns>
        public static bool hasError(IerrorCarrier errorCarrier)
        {
            if (errorCarrier is ImultiErrorCarrier)
            {
                var carrier = ((ImultiErrorCarrier)errorCarrier);
                if (carrier != null)
                {
                    if (carrier.errors != null)
                    {
                        if (carrier.errors.Count > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(errorCarrier.errorText))
            {
                return true;
            }
            return false;
        }

        #endregion (+) other methods

        #region (+) copy methods

        /// <summary>
        /// Copy underlying errors to another error carrier destinations 
        /// </summary>
        /// <param name="destination">The destination</param>
        public void copyTo(IerrorCarrier destination)
        {
            destination.hasError = true;
            destination.errorText = this.message;
            destination.extendedErrorText = stackTrace;
        }

        /// <summary>
        /// Copy underlying errors to another multi error carrier destination
        /// </summary>
        /// <param name="destination">The destination</param>
        public void copyTo(ImultiErrorCarrier destination)
        {
            destination.hasError = true;
            destination.errorText = this.message;
            destination.extendedErrorText = stackTrace;
            destination.errors = this.errors;
        }

        #endregion (+) copy methods

        #region (+) static copy methods
        /// <summary>
        /// Copy errors from a source to a destination 
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="destination">the destination</param>
        public static void copy(IerrorCarrier source, IerrorCarrier destination)
        {
            destination.hasError = source.hasError;
            destination.errorText = source.errorText;
            destination.extendedErrorText = source.extendedErrorText;

            destination.hasError=hasError(destination); 
        }

        /// <summary>
        /// Copy errors from a source to a destination 
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="destination">the destination</param>
        public static void copy(ImultiErrorCarrier source, ImultiErrorCarrier destination)
        {
           copy(  ((IerrorCarrier)source), ((IerrorCarrier)destination) );
           destination.errors = source.errors;
           destination.hasError = hasError(destination);
        }

        /// <summary>
        /// Copy errors from a source to a destination 
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="destination">the destination</param>
        public static void copy(IerrorCarrier source, ImultiErrorCarrier destination)
        {
            copy(((IerrorCarrier)source), ((IerrorCarrier)destination));
            destination.errors=new();
            destination.hasError = hasError(destination);
        }

        /// <summary>
        /// Copy errors from a source to a destination 
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="destination">the destination</param>
        public static void copy(ImultiErrorCarrier source, IerrorCarrier destination)
        {
            copy(((IerrorCarrier)source), ((IerrorCarrier)destination));
            if ( string.IsNullOrEmpty(destination.errorText) && source.errors != null )
            {
                if ( source.errors.Count>0 )
                {
                    destination.errorText = source.errors[0];
                    if (source.errors.Count > 1) destination.extendedErrorText= source.errors[1];
                    destination.hasError=true;
                }
            }
            
        }

        /// <summary>
        /// Copy errors from a source to a destination 
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="destination">the destination</param>
        public static void copy(IerrorExtractor source, IerrorCarrier destination)
        {
            source.copyErrorsTo(destination);
            destination.hasError = hasError(destination);
        }

        #endregion (+) static copy methods

        #region (+) static methods

        /// <summary>
        /// Gets the most inner (deepest) exception of a given Exception object
        /// </summary>
        /// <param name="ex">Source Exception</param>
        /// <returns></returns>
        public static Exception getMostInner(Exception ex)
        {
            Exception actualInnerEx = ex;

            while (actualInnerEx != null)
            {
                actualInnerEx = actualInnerEx.InnerException;
                if (actualInnerEx != null) ex = actualInnerEx;
            }
            return ex;
        }

        /// <summary>
        /// Return Exception from IerrorCarrier
        /// </summary>
        /// <param name="source"></param>
        /// <returns>The new exception. eg: throw errorParser.error(result);</returns>
        public static Exception error(IerrorCarrier source)
        {
            return new Exception(new errorParser(source).ToString() );
        }

        /// <summary>
        /// Return Exception from ImultiErrorCarrier
        /// </summary>
        /// <param name="source"></param>
        /// <returns>The new exception. eg: throw errorParser.error(result);</returns>
        public static Exception error(ImultiErrorCarrier source)
        {
            return new Exception(new errorParser(source).ToString());
        }


        /// <summary>
        /// Place error to the destination instance
        /// </summary>
        /// <param name="message">the message to add</param>
        /// <param name="destination">the destination instance</param>
        public static void errorTo(string message, IerrorCarrier destination)
        {
            var error=new errorParser(message);
            error.copyTo(destination);
        }

        /// <summary>
        /// Place error to the destination instance
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="destination">the destination instance</param>
        public static void errorTo(Exception ex, IerrorCarrier destination)
        {
            var error = new errorParser(ex);
            error.copyTo(destination);
        }

        /// <summary>
        /// Add error to a multi error carrier destination
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="destination">the destination instance</param>
        public static void errorAddTo(Exception ex, ImultiErrorCarrier destination)
        {
            var error = new errorParser(destination);
            error.add(ex);
            error.copyTo(destination);
        }

        /// <summary>
        /// Add error to a multi error carrier destination
        /// </summary>
        /// <param name="message">the message text</param>
        /// <param name="destination">the destination</param>
        public static void errorAddTo(string message, ImultiErrorCarrier destination)
        {
            var error = new errorParser(destination);
            error.add(message);
            error.copyTo(destination);
        }

        #endregion (+) static methods
    }
}
