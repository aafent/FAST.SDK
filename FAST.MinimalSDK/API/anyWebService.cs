using FAST.Core;
using FAST.Core.Models;
using FAST.Logging;
using FAST.Services.Models;

namespace FAST.API
{
    /// <summary>
    /// Base class for any web service
    /// </summary>
    public abstract class anyWebService : IerrorCarrier, ImultiErrorCarrier
    {
        /// <summary>
        /// The Basic Uri of the web service
        /// </summary>
        protected Uri baseUri = null!;

        private string _endpoint = null!;

        /// <summary>
        /// The endpoint of the web service
        /// </summary>
        public string endpoint
        {
            get
            {
                return _endpoint;
            }
            set
            {
                _endpoint = value;
                buildInnerBaseUri();
            }
        }

        /// <summary>
        /// The last error text
        /// </summary>
        public string errorText { get; set; }

        /// <summary>
        /// Extended error text, when available
        /// </summary>
        public string extendedErrorText { get; set; }

        /// <summary>
        /// True if an error has occurred
        /// </summary>
        public bool hasError { get; set; }

        /// <summary>
        /// A list of errors that occurred
        /// </summary>
        public List<string> errors { get; set; }


        /// <summary>
        /// True when Success and no error has occurred
        /// </summary>
        public abstract bool success { get; }

        /// <summary>
        /// Build the inner base Uri
        /// </summary>
        protected abstract void buildInnerBaseUri();

        /// <summary>
        /// Log an error. This is virtual method that can be overridden to implement custom logging
        /// </summary>
        /// <param name="errorText"></param>
        /// <param name="ex"></param>
        public virtual void logError(string errorText, Exception ex = null)
        {
            fastLogger.error(errorText, ex);
        }

        /// <summary>
        /// Log an info. This is virtual method that can be overridden to implement custom logging
        /// </summary>
        /// <param name="infoText"></param>
        public virtual void logInfo(string infoText)
        {
            fastLogger.info(infoText);
        }

        /// <summary>
        /// Dump a title and content. Using debug logging. 
        /// This is virtual method that can be overridden to implement custom logging
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        public virtual void dump(string title, string content)
        {
            fastLogger.debug(title + "\t" + content);
        }


        /// <summary>
        /// Clear all errors
        /// </summary>
        public void clearErrors()
        {
            hasError = false;
            errorText = String.Empty;
            extendedErrorText = null;
            if (errors != null) errors.Clear();
        }


        /// <summary>
        /// Helper to get the model from an object that contains a model property.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="objectWithPropertyModel"></param>
        /// <returns></returns>
        public TResult model<TResult>(IpropertyModel<TResult> objectWithPropertyModel)
                                                 where TResult : class, new()
        {
            if (objectWithPropertyModel == null) return null;
            if (objectWithPropertyModel.model == null) objectWithPropertyModel.model = new(); // (<) if model isnot created, create it

            // if the input object it is an error container and the return class (the model) is an error container, if there is 
            // an error in the input object and there is not an error on the model, copy the error to the model
            //
            if (
                reflectionHelper.containsAnyOfTheInterfaces(typeof(TResult), nameof(IerrorCarrier), nameof(ImultiErrorCarrier))       // (<) the model
                &&
                reflectionHelper.containsAnyOfTheInterfaces(objectWithPropertyModel.GetType(), nameof(IerrorCarrier), nameof(ImultiErrorCarrier)) // (<) the container object (contains the model)
               )
            {
                if (
                    !((IerrorCarrier)objectWithPropertyModel.model).hasError &       // (<) the model is without error
                    ((IerrorCarrier)objectWithPropertyModel).hasError                 //     and the container is with error
                    )
                {
                    errorParser.copy(objectWithPropertyModel as IerrorCarrier, objectWithPropertyModel.model as IerrorCarrier);
                }
            }

            // (v) return the model
            return objectWithPropertyModel.model;
        }


    }

}
