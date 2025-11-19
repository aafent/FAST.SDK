using FAST.Core;
using FAST.Core.Models;
using FAST.Logging;

namespace FAST.Services.Models.Data
{
    /// <summary>
    /// The base class for the data structures processor
    /// </summary>
    public abstract class dataStructureProcessorBase : ImultiErrorCarrier
    {
        /// <summary>
        /// The error container for the Processor.
        /// Is able to contain more than one errors
        /// </summary>
        public List<string> errors { get; set; } = null;

        /// <summary>
        /// The main error, other errors in the errors list
        /// </summary>
        public string errorText { get; set; } = null;

        /// <summary>
        /// Extended error text
        /// </summary>
        public string extendedErrorText { get; set; } = null;

        /// <summary>
        /// True if the error container contains an error
        /// </summary>
        public bool hasError { get; set; }

        /// <summary>
        /// Add error to the internal errors objects
        /// </summary>
        /// <param name="ex">Exception to get the error</param>
        /// <param name="extraMessage">Extra message to the exception</param>
        public void addError(Exception ex, string extraMessage)
        {
            var err = new errorParser(ex);
            this.addError(err.message);
        }

        /// <summary>
        /// Add error to the internal errors objects
        /// </summary>
        /// <param name="message">The error message</param>
        public void addError(string message)
        {
            if (errors == null) errors = new();
            if (errors == null) errors = new();
            errors.Add(message);
            errorText = $"{errorText.Count()} errors found.";
            fastLogger.error(message);
            this.hasError = true;
        }

        /// <summary>
        /// Add multiple error to the internal errors objects
        /// </summary>
        /// <param name="errors">A collection with the errors to add</param>
        public void addErrors(IEnumerable<string> errors)
        {
            foreach (var error in errors)
            {
                this.addError(error);
            }
        }


    }

}
