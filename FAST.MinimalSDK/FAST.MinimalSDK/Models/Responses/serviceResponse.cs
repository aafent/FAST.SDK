using System.Runtime.Serialization;

namespace FAST.Core.Models
{
    [DataContract]
    public class serviceResponse : IserviceResponse
    {
        public serviceResponse()
        {
            this.errorText = string.Empty;
            this.extendedErrorText = string.Empty;
            this.hasError = false;
            this.dataNotFound = false;
        }

        [DataMember]
        public string errorText { get; set; }

        [DataMember]
        public string extendedErrorText { get; set; }

        [DataMember]
        public bool hasError { get; set; }

        [DataMember]
        public bool dataNotFound { get; set; }


        public string tag { get; set; }

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

        public virtual void setError(string errorText)
        {
            serviceResponse.setUnderlyingError(this, errorText);
        }
        public virtual T returnError<T>(string errorText) where T : serviceResponse
        {
            this.setError(errorText);
            return (T)this;
        }

        public static void setUnderlyingError(IserviceResponse response, string errorText)
        {
            response.extendedErrorText = string.Empty;
            response.dataNotFound = false;
            response.errorText = errorText;
            response.hasError = true;
        }


        public static serviceResponse success()
        {
            serviceResponse success = new serviceResponse();
            success.errorText = string.Empty;
            success.extendedErrorText = string.Empty;
            success.hasError = false;
            success.dataNotFound = false;
            return success;
        }
        public static serviceResponse generalError(string errorText)
        {
            serviceResponse problem = new serviceResponse();
            serviceResponse.setUnderlyingError(problem, errorText);
            return problem;
        }

        private static string exceptionToText(Exception ex)
        {
            string text = "General Exception.";
            if (ex != null)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                {
                    text = ex.Message;
                }

                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        text += "\n";
                    }

                    text += ex.InnerException.Message;
                }
            }
            return text;
        }


        public static void exceptionToMultiError(Exception ex, ImultiErrorCarrier response)
        {
            string text = exceptionToText(ex);

            if (response.errors == null) { response.errors = new List<string>(); }
            response.errors.Add(text);
            response.extendedErrorText = string.Empty;
            response.errorText = text;
            response.hasError = true;

            //serviceResponse problem = new serviceResponse();
            //serviceResponse.setUnderlyingError(problem, text);
            //return problem;

            return;
        }
        public static void exceptionToError(Exception ex, serviceResponse response)
        {
            string text= exceptionToText(ex);

            response.extendedErrorText = string.Empty;
            response.errorText = text;
            response.hasError = true;

            return;
        }

    }
}
