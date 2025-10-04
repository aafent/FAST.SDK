using FAST.Core;

namespace FAST.Logging
{
    /// <summary>
    /// Interface with commands inline notation for fastLogger static class
    /// </summary>
    public interface IloggerInlineNotation : IloggerInlineNotation2ndLevel, IloggerInlineNotation3ndLevel
    {
        IloggerInlineNotation2ndLevel debug(string message, string extra = null);

        IloggerInlineNotation2ndLevel error(Exception carryingException, string source = null, string extra = null);

        IloggerInlineNotation2ndLevel error(string message, string source = null, string extra = null);

        IloggerInlineNotation2ndLevel error(string message, Exception carryingException, string source = null, string extra = null);

        IloggerInlineNotation2ndLevel fatal(Exception carryingException, string source = null, string extra = null);

        IloggerInlineNotation2ndLevel fatal(string message, string source = null, string extra = null);

        IloggerInlineNotation2ndLevel fatal(string message, Exception carryingException, string source = null, string extra = null);

        IloggerInlineNotation2ndLevel info(string message, string source = null, string extra = null);

        IloggerInlineNotation2ndLevel trace(string message, string extra = null);

        IloggerInlineNotation2ndLevel warning(string message, string source = null, string extra = null);

        IloggerInlineNotation guid(string guidToUse = null );

        string getGuid();


    }

    /// <summary>
    /// Interface with commands inline notation for fastLogger static class
    /// </summary>
    public interface IloggerInlineNotation2ndLevel : IloggerInlineNotation3ndLevel
    {
        void throwException();

        IloggerInlineNotation2ndLevel error();
        IloggerInlineNotation2ndLevel warning();
        IloggerInlineNotation2ndLevel fatal();
    }

    public interface IloggerInlineNotation3ndLevel
    {
        IloggerInlineNotation2ndLevel source(string source);
        IloggerInlineNotation2ndLevel message(string message);
        IloggerInlineNotation2ndLevel extra(string extra);
    }


    /// <summary>
    /// Interface with commands inline notation for fastLogger static class
    /// </summary>
    public class loggerInLineNotation : IloggerInlineNotation
    {
        public loggerInLineNotation(Ilogger log)
        {
            this.log = log;
            reset();
        }

        private readonly Ilogger log;

        #region (+) ressetable internal variables 
        private string text=null;
        private string extraText=null;
        private Exception carryingException =null;
        private string src=null;
        private string messageGUID =null;
        #endregion (+) ressetable internal variables 


        #region (+) standard logging methods (similar to Ilogger but with return value)

        public IloggerInlineNotation2ndLevel debug(string message, string extra = null)
        {
            this.text=message;
            this.extraText=extra;
            log.debug(message,extra);
            return this;
        }

        public IloggerInlineNotation2ndLevel error(Exception carryingException, string source = null, string extra = null)
        {
            this.carryingException=carryingException;
            this.src=source;
            this.extraText = extra;
            log.error(carryingException, source, extra);
            return this;
        }

        public IloggerInlineNotation2ndLevel error(string message, string source = null, string extra = null)
        {
            this.text = message;
            this.src = source;
            this.extraText = extra;
            log.error(message, source, extra);
            return this;
        }

        public IloggerInlineNotation2ndLevel error(string message, Exception carryingException, string source = null, string extra = null)
        {
            this.text = message;
            this.carryingException=carryingException;
            this.src = source;
            this.extraText = extra;
            log.error(message, carryingException, source, extra);
            return this;
        }

        public IloggerInlineNotation2ndLevel fatal(Exception carryingException, string source = null, string extra = null)
        {
            this.carryingException = carryingException;
            this.src = source;
            this.extraText = extra;
            log.fatal(carryingException, source, extra);
            return this;
        }

        public IloggerInlineNotation2ndLevel fatal(string message, string source = null, string extra = null)
        {
            this.text = message;
            this.src = source;
            this.extraText = extra;
            log.fatal(message, source, extra);
            return this;
        }

        public IloggerInlineNotation2ndLevel fatal(string message, Exception carryingException, string source = null, string extra = null)
        {
            this.text = message;
            this.carryingException = carryingException;
            this.src = source;
            this.extraText = extra;
            log.fatal(message, carryingException, source, extra);
            return this;
        }

        public IloggerInlineNotation2ndLevel info(string message, string source = null, string extra = null)
        {
            this.text = message;
            this.src = source;
            this.extraText = extra;
            log.info(message, source, extra);
            return this;
        }

        public IloggerInlineNotation2ndLevel trace(string message, string extra = null)
        {
            this.text = message;
            this.extraText = extra;
            log.trace(message, extra);
            return this;
        }

        public IloggerInlineNotation2ndLevel warning(string message, string source = null, string extra = null)
        {
            this.text = message;
            this.src = source;
            this.extraText = extra;
            log.warning(message, source, extra);
            return this;
        }

        #endregion (+) standard logging methods (similar to Ilogger but with return value)

        private string getMessage()
        {
            if (string.IsNullOrWhiteSpace(src)) src = "N/A";

            string msg = this.text;
            if (string.IsNullOrWhiteSpace(msg))
            {
                msg = extraText;
                if (string.IsNullOrWhiteSpace(msg))
                {
                    msg = $"Generic Error with source indicator : {this.src} ";
                }
            }
            else
            {
                msg = this.text;
                if (string.IsNullOrWhiteSpace(this.extraText)) msg += $".{this.extraText}.";
                msg += $"[{this.src}]";
            }

            if (!string.IsNullOrWhiteSpace(this.messageGUID))
            {
                msg+=$" [{this.messageGUID}]";
            }

            return msg;
        }



        /// <summary>
        /// Throw exception with the last logged message;
        /// </summary>
        public void throwException()
        {
            Exception ex;
            var msg=getMessage();
            if (this.carryingException == null)
            {
                ex = new(msg, this.carryingException);
            }
            else
            {
                ex =new Exception(msg);
            }

            throw ex;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>string with the message</returns>
        public override string ToString()
        {
            string msg=getMessage();
            if (this.carryingException != null)
            {
                var err = new errorParser(this.carryingException);
                if (string.IsNullOrWhiteSpace(msg) ) msg="";
                msg+=err.ToString();
            } 
            return msg;
        }

        /// <summary>
        /// Log error with previous defined messages
        /// </summary>
        public IloggerInlineNotation2ndLevel error()
        {
            if (this.carryingException != null )
            {
                if (string.IsNullOrWhiteSpace(this.text))
                    this.error(this.carryingException, this.src, this.extraText);
                else
                    this.error(this.text,this.carryingException,this.src,this.extraText);
            }
            else
            {
                    this.error(this.text, this.src, this.extraText);
            }
            return this;
        }

        /// <summary>
        /// Log fatal error with previous defined messages
        /// </summary>
        public IloggerInlineNotation2ndLevel fatal()
        {
            if (this.carryingException != null)
            {
                if (string.IsNullOrWhiteSpace(this.text))
                    this.fatal(this.carryingException, this.src, this.extraText);
                else
                    this.fatal(this.text, this.carryingException, this.src, this.extraText);
            }
            else
            {
                this.fatal(this.text, this.src, this.extraText);
            }
            return this;
        }

        /// <summary>
        /// Log warning with previous defined messages
        /// </summary>
        public IloggerInlineNotation2ndLevel warning()
        {
            this.warning(this.text, this.src, this.extraText);
            return this;
        }

        /// <summary>
        /// Set the source property of the log entry
        /// </summary>
        /// <param name="source">The value for the source property</param>
        public IloggerInlineNotation2ndLevel source(string source)
        {
            this.src=source;
            return this;
        }

        /// <summary>
        /// Set the message property of the log entry
        /// </summary>
        /// <param name="message">The value for the message property</param>
        public IloggerInlineNotation2ndLevel message(string message)
        {
            this.text=message;
            return this;
        }

        /// <summary>
        /// Set the extra property of the log entry
        /// </summary>
        /// <param name="extra">The value for the extra property</param>
        public IloggerInlineNotation2ndLevel extra(string extra)
        {
            this.extraText=extra;
            return this;
        }

        /// <summary>
        /// Set or Create a message Guid
        /// </summary>
        /// <param name="guidToUse">Optional, The Guid to use, if omitted a new Guid will created</param>
        public IloggerInlineNotation guid(string guidToUse = null)
        {
            if (string.IsNullOrWhiteSpace(guidToUse))
            {
                this.messageGUID=new Guid().ToString();
            }
            else
            {
                this.messageGUID = guidToUse;
            }
            return this;
        }

        /// <summary>
        /// Get or Create and Get a Guid for current message;
        /// </summary>
        /// <returns>The Guid as a string</returns>
        public string getGuid()
        {
            guid(this.messageGUID);
            return this.messageGUID;
        }

        /// <summary>
        /// Reset the inner values for the log entry. 
        /// The static fastLogger methods are calling this reset before any logging method. 
        /// </summary>
        public void reset()
        {
            this.text = null;
            this.extraText = null;
            this.carryingException = null;
            this.src = null;
            this.messageGUID = null;
        }

    }

     

}
