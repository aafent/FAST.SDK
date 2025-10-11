using FAST.Core;
using FAST.Core.Models;
using FAST.Logging;

namespace FAST.Services.Models
{


    /// <summary>
    /// Response model, the elementary (minimum) returning model. All other models are inherited from this
    /// </summary>
    public class elementaryModel : IelementaryModel, IdataFoundQuestion, ImultiErrorCarrier, IerrorCarrier 
    {
        public bool dataNotFound { get; set; }

        public string tag { get; set; }

        public bool hasError { get; set; }
        public List<string> errors { get; set; }
        public string errorText { get; set; }
        public string extendedErrorText { get; set; }
    }

    /// <summary>
    /// A class that containing an elementary model as well as methods to work with it. 
    /// </summary>
    public class elementaryModelClass : elementaryModel, IserviceResponse
    {

        public virtual void copyFrom(IserviceResponse source)
        {
            this.errorText = source.errorText;
            this.extendedErrorText = source.extendedErrorText;
            this.hasError = source.hasError;
            this.dataNotFound = source.dataNotFound;
        }
        public virtual void copyFrom<T>(errorsContainer<T> source) where T : IErrorItem
        {
            if (!source.hasError) { return; }

            this.errorText = string.Empty;
            this.extendedErrorText = string.Empty;
            this.hasError = true;
            this.dataNotFound = true;

            int cnt = 0;
            foreach (var item in source.items.Where(i => i.type == errorTypes.error))
            {
                cnt++;
                if (cnt == 1)
                {
                    this.errorText = item.description;
                }
                else
                {
                    if (cnt == 2) { this.errorText += "\n(+)"; }
                    if (!string.IsNullOrEmpty(this.extendedErrorText)) { this.extendedErrorText += "\n"; }
                    this.extendedErrorText += item.description;
                }
            }

        }


        public void processErrorsBeforeReturn(params IerrorCarrier[] items)
        {
            this.hasError=false;
            foreach (var item in items)
            {
                if (!item.hasError) continue;

                if (this.errors.Count == 0)
                {
                    this.hasError = true;
                    this.errorText = item.errorText;
                    this.extendedErrorText = item.extendedErrorText;
                }
                this.errors.Add(item.errorText);
            }
            if ( !string.IsNullOrEmpty(this.errorText ) ) this.hasError=true;
        }
    }


    /// <summary>
    /// Extensions on elementaryModelClass
    /// </summary>
    public static class elementaryModelClass_extensions
    {

        /// <summary>
        /// If an error log it as error or fatal
        /// </summary>
        /// <param name="asFatal">True to log it as fatal, default is false (aka error)</param>
        public static void logIfError(this elementaryModelClass model, bool asFatal=false)
        {
            if (model.hasError)
            {
                if (asFatal)
                {
                    string msg = new errorParser(model).ToString();
                    fastLogger.fatal(msg);
                }
                else fastLogger.error(model);
            }
        }

        /// <summary>
        /// If an error log it as error and throw an exception
        /// <param name="asFatal">True to log it as fatal, default is false (aka error)</param>
        /// </summary>
        public static void throwIfError(this elementaryModelClass model, bool asFatal = false)
        {
            if (model.hasError)
            {
                string msg = new errorParser(model).logError(asFatal).ToString();
                throw new Exception(msg);
            }
        }
    }


}

