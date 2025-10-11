using FAST.Core;
using FAST.Core.Models;
using FAST.Logging;
using FAST.Services.Models;
using FAST.Strings;
using System.ComponentModel;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace FAST.API
{

    /// <summary>
    /// Abstract class, use by all  rest services as ancestor 
    /// </summary>
    public abstract class restService : IerrorCarrier, ImultiErrorCarrier
    {
        private HttpClient _client;
        private JsonSerializerOptions _serializerOptions;
        private HttpResponseMessage _response = null;
        private StringContent contentToSend=null;
        private bool needToRestoreHeaders;
        private HttpRequestHeaders currentRequestHeads;

        protected Uri baseUri = null;
        private string _endpoint = null;

        /// <summary>
        /// The URL to used as base URL to reach the method. 
        /// The full endpoint syntax is: [endpoint]/[controllerName]/[restTemplate]
        /// <example>https://service.example.com:55555/[controllerName]/[restTemplate]</example>
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
        /// Underlying HTTP Client
        /// </summary>
        public HttpClient underlyingHttpClient
        {
            get
            {
                return _client;
            }
        }

        /// <summary>
        /// A Stream that contains the result of the invocation 
        /// </summary>
        public MemoryStream resultStream { get; private set; }

        private string _controllerName = null;
        /// <summary>
        /// The URL to used as base URL to reach the method. 
        /// The full endpoint syntax is: [endpoint]/[controllerName]/[restTemplate]
        /// <example>account  :: [endpoint]/account/[restTemplate]</example>
        /// </summary>
        public string controllerName
        {
            get
            {
                return _controllerName;
            }
            set
            {
                _controllerName = value;
                buildInnerBaseUri();
            }
        }


        public string authTokenHeaderName { get; set; } = "fastToken";

        /// <summary>
        /// True to enable the Dump to log file before invoke, default is false
        /// </summary>
        public bool enableDump { get; set; } = false;

        /// <summary>
        /// True, to throw exceptions, False to hide. Default is false
        /// When are hidden, the property "errors" will contain the exactions, as well as the log file
        /// </summary>
        public bool throwExceptionsEnabled { get; set; } = false;

        /// <summary>
        /// True to disable the logging, default is: false
        /// </summary>
        public bool disableLogging { get; set; } = false;

        #region (+) Public properties to controll the invocation and the errors

        /// <summary>
        /// In case of faulty execution the error text
        /// </summary>
        public string errorText { get; set; }

        /// <summary>
        /// In case of faulty execution an optional extended error text
        /// </summary>
        public string extendedErrorText { get; set; }

        /// <summary>
        /// True if the invocation (execution) raised an error
        /// </summary>
        public bool hasError { get; set; }

        /// <summary>
        /// A list with all the errors raised 
        /// </summary>
        public List<string> errors { get; set; }

        /// <summary>
        /// The HttpStatusCode of the last invocation 
        /// </summary>
        public HttpStatusCode statusCode
        {
            get
            {
                if (_response == null) return HttpStatusCode.Unauthorized;
                return _response.StatusCode;
            }
        }

        /// <summary>
        /// Holds the IsSuccessStatusCode from the response. 
        /// </summary>
        public bool isSuccessStatusCode
        {
            get
            {
                if (_response == null) return false;  
                return _response.IsSuccessStatusCode;
            }
        }

        [Obsolete("Replace it with: isSuccessStatusCode")]
        public bool success => isSuccessStatusCode;

        #endregion (+) Public properties to controll the invocation and the errors

        /// <summary>
        /// Constructor with client handler
        /// </summary>
        /// <param name="handler">The handler</param>
        public restService(HttpClient httpClient)
        {
            commonConstructorCode(httpClient);
        }

        /// <summary>
        /// Constructor, using the default client Handler
        /// 
        /// </summary>
        public restService() 
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.UseDefaultCredentials = true;
            handler.ServerCertificateCustomValidationCallback +=
                            (sender, certificate, chain, errors) =>
                            {
                                return true;
                            };
            HttpClient client = new HttpClient(handler);
            commonConstructorCode(client);
        }

        /// <summary>
        /// Common code to all constructors
        /// </summary>
        /// <param name="httpClient"></param>
        private void commonConstructorCode(HttpClient httpClient)
        {
            _client = httpClient;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        #region (+) log virtual methods

        public virtual void logError(string errorText, Exception ex = null)
        {
            if (!disableLogging)
            {
                fastLogger.error(errorText, ex);
            }
        }
        public virtual void logInfo(string infoText)
        {
            if (!disableLogging)
            {
                fastLogger.info(infoText);
            }
        }

        public virtual void dump(string title, string content)
        {
            var maxLen = 31*1024;
            if (!disableLogging)
            {
                try
                {
                    stringsHelper.foreachChunk(content,maxLen, (msg) =>
                        { fastLogger.debug(title + "\t" + msg); } );
                }
                catch { }

            }
        }

        #endregion (+) log virtual methods

        #region (+) various public methods

        /// <summary>
        /// Clear the errors of a previous invocation 
        /// </summary>
        public void clearErrors()
        {
            hasError = false;
            errorText = String.Empty;
            extendedErrorText = null;
            if (errors != null) errors.Clear();
        }
        public Tuple<string, string> authToken(string value)
        {
            return new Tuple<string, string>(authTokenHeaderName, value);
        }


        public TResult model<TResult>(IpropertyModel<TResult> objectWithPropertyModel)
            where TResult : class, new()
        {
            if (objectWithPropertyModel == null) return null;
            if (objectWithPropertyModel.model == null) objectWithPropertyModel.model = new(); // (<) if model is not created, create it

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

        #endregion (+) various public methods


        /// <summary>
        /// Prepare the Underlining HTTP Client before invoke the web method
        /// </summary>
        /// <typeparam name="contentType"></typeparam>
        /// <param name="parameters">restParameters to use for the HTTP Method</param>
        /// <param name="verb">The requested HTTP Verb</param>
        /// <param name="uri">The Uri for the method</param>
        /// <param name="content">The content of the method's body</param>
        public void prepareHttpClient<contentType>(restParameters parameters, httpVerbs verb, Uri uri, contentType content)
                                                                                            where contentType : class, new()
        {
            this.currentRequestHeads = _client.DefaultRequestHeaders;// (<) keep a copy of the default headers
            this.needToRestoreHeaders = false;
            clearErrors();

            string json = null;
            contentToSend = null;

            if (verb != httpVerbs.get && content != null) // (<) for verbs with content
            {
                json = JsonSerializer.Serialize<contentType>(content, _serializerOptions);
                contentToSend = new StringContent(json, Encoding.UTF8, "application/json");
            }

            // (v) prepare headers
            foreach (var header in parameters.headers)
                if (_client.DefaultRequestHeaders.TryAddWithoutValidation(header.Item1, header.Item2))
                {
                    needToRestoreHeaders = true;
                }

            // (v) AuthenticationHeaderValue with bearer token
            if (parameters.hasAuthTokenValue)
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(parameters.authTokenSchema, parameters.authTokenValue);
            }


            // (v) invoke the method
            if (enableDump)
            {
                dump("VERB", verb.ToString().ToUpper());
                dump("URL", uri.ToString());
                foreach (var item in _client.DefaultRequestHeaders) dump($"HEADER:[{item.Key}", item.Value.ToString());
                if (contentToSend != null) dump("CONTENT", JsonSerializer.Serialize(contentToSend));
            }

            _response = null;

            return;
        }

        public async Task<TResult> doVerbAsync<TResult, contentType>(restParameters parameters, httpVerbs verb, Uri uri, contentType content)
            where TResult : class, new()
            where contentType : class, new()
        {
            TResult result = null;

            try 
            {
                prepareHttpClient(parameters, verb, uri, content);
            
                switch (verb)
                {
                    // (v) Verbs without content
                    case httpVerbs.get:
                        _response = await _client.GetAsync(uri);
                        break;
                    case httpVerbs.delete:
                        _response = await _client.DeleteAsync(uri);
                        break;

                    // (v) Verbs with content
                    case httpVerbs.post:
                        _response = await _client.PostAsync(uri, contentToSend);
                        break;
                    case httpVerbs.put:
                        _response = await _client.PutAsync(uri, contentToSend);
                        break;
                    case httpVerbs.patch:
                        _response = await _client.PatchAsync(uri, contentToSend);
                        break;


                    default:
                        throw new NotImplementedException("Verb is not implemented yet");
                }

                result = null;
                if (_response.IsSuccessStatusCode)
                {
                    if (parameters.responseAsStream)
                    {
                        this.resultStream = new MemoryStream();
                        var tempResults = await _response.Content.ReadAsStreamAsync();
                        await tempResults.CopyToAsync(this.resultStream);
                    }
                    else
                    if (parameters.responseIsJSON)
                    {
                        result = await _response.Content.ReadFromJsonAsync<TResult>(_serializerOptions);

                        if (enableDump) dump("RESULT", await _response.Content.ReadAsStringAsync());

                        if (result == null) result = new();

                        if (reflectionHelper.containsAnyOfTheInterfaces(typeof(TResult), nameof(ImultiErrorCarrier)))
                        {
                            if (((ImultiErrorCarrier)result).hasError)
                            {
                                errorParser.copy((ImultiErrorCarrier)result, this);
                            }
                        }
                        else
                        if (reflectionHelper.containsAnyOfTheInterfaces(typeof(TResult), nameof(IerrorCarrier)))
                        {
                            if (((IerrorCarrier)result).hasError)
                            {
                                errorParser.copy((IerrorCarrier)result, this);
                            }
                        }

                    }
                    else
                    {
                        string resultAsString = await _response.Content.ReadAsStringAsync();
                        if (enableDump) dump("RESULT", resultAsString);
                        result = resultAsString as TResult;
                    }
                }
                else // error
                {
                    string httpErrorText = ((int)_response.StatusCode).ToString() + " " + _response.StatusCode.ToString();
                    if (enableDump) dump("ERROR-STATUS", httpErrorText);
                    logError($"Error: {uri.AbsolutePath}, {httpErrorText}");
                }
            }
            catch (Exception ex)
            {
                var error = new errorParser(ex);

                if (enableDump) dump("ERROR", $"{ex}"); ;
                logError($"Error: {uri.AbsolutePath}, {error}", ex);
                error.copyTo(this);
                if (throwExceptionsEnabled) throw;
            }
            finally
            {
                // (v) restore the default headers as was before the call. 
                if (needToRestoreHeaders)
                {
                    _client.DefaultRequestHeaders.Clear();
                    foreach (var header in currentRequestHeads)
                    {
                        _client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
            }

            return result;
        }

        public async Task<TResult> doRawVerbAsync<TResult, contentType>(restParameters parameters, httpVerbs verb, Uri uri, contentType content)
            where contentType : class, new()
        {
            TResult result = default(TResult);
            try
            {
                prepareHttpClient(parameters, verb, uri, content);  
                switch (verb)
                {
                    // (v) Verbs without content
                    case httpVerbs.get:
                        _response = await _client.GetAsync(uri);
                        break;
                    case httpVerbs.delete:
                        _response = await _client.DeleteAsync(uri);
                        break;

                    // (v) Verbs with content
                    case httpVerbs.post:
                        _response = await _client.PostAsync(uri, contentToSend);
                        break;
                    case httpVerbs.put:
                        _response = await _client.PutAsync(uri, contentToSend);
                        break;
                    case httpVerbs.patch:
                        _response = await _client.PatchAsync(uri, contentToSend);
                        break;


                    default:
                        throw new NotImplementedException("Verb is not implemented yet");
                }

                result = default(TResult);
                if (_response.IsSuccessStatusCode)
                {
                    if (parameters.responseAsStream)
                    {
                        this.resultStream = new MemoryStream();
                        var tempResults = await _response.Content.ReadAsStreamAsync();
                        await tempResults.CopyToAsync(this.resultStream);
                    }
                    else
                    if (parameters.responseIsJSON)
                    {
                        result = await _response.Content.ReadFromJsonAsync<TResult>(_serializerOptions);

                        if (enableDump) dump("RESULT", await _response.Content.ReadAsStringAsync());

                        if (result == null)
                        {
                            result = reflectionHelper.getInstance<TResult>(); // hope to create 
                        }

                        if (reflectionHelper.containsAnyOfTheInterfaces(typeof(TResult), nameof(ImultiErrorCarrier)))
                        {
                            if (((ImultiErrorCarrier)result).hasError)
                            {
                                errorParser.copy((ImultiErrorCarrier)result, this);
                            }
                        }
                        else
                        if (reflectionHelper.containsAnyOfTheInterfaces(typeof(TResult), nameof(IerrorCarrier)))
                        {
                            if (((IerrorCarrier)result).hasError)
                            {
                                errorParser.copy((IerrorCarrier)result, this);
                            }
                        }

                    }
                    else
                    {
                        string resultAsString = await _response.Content.ReadAsStringAsync();
                        if (enableDump) dump("RESULT", resultAsString);

                        // (v) convert to Result Type
                        //result = resultAsString as TResult;
                        TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(TResult));
                        result = (TResult)typeConverter.ConvertFromString(resultAsString);
                    }
                }
                else // error
                {
                    string httpErrorText = ((int)_response.StatusCode).ToString() + " " + _response.StatusCode.ToString();
                    if (enableDump) dump("ERROR-STATUS", httpErrorText);
                    logError($"Error: {uri.AbsolutePath}, {httpErrorText}");
                }
            }
            catch (Exception ex)
            {
                var error = new errorParser(ex);

                if (enableDump) dump("ERROR", $"{ex}"); ;
                logError($"Error: {uri.AbsolutePath}, {error}", ex);
                error.copyTo(this);
                if (throwExceptionsEnabled) throw;
            }
            finally
            {
                // (v) restore the default headers as was before the call. 
                if (needToRestoreHeaders)
                {
                    _client.DefaultRequestHeaders.Clear();
                    foreach (var header in currentRequestHeads)
                    {
                        _client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
            }

            return result;
        }

        #region (+) doVerb type of methods, giving polymorphism to the basic group of doVerb methods

        public Task<TResult> doRawVerb<TResult, contentType>(restParameters parameters, httpVerbs verb, contentType content, string restTemplate, params string[] args)
                            where contentType : class, new()
        {
            //
            // resource: https://stackoverflow.com/questions/13086258/net-4-5-async-await-and-overloaded-methods
            //           https://stackoverflow.com/questions/3917249/the-type-arguments-for-method-cannot-be-inferred-from-the-usage
            //
            //string url = inetHelper.urlCombine(baseUri.AbsoluteUri, string.Format(restTemplate, args));
            //Uri uri = new Uri(url);
            var uri = restTemplateToUri(restTemplate, args);
            return doRawVerbAsync<TResult, contentType>(parameters, verb, uri, content);
        }
        public Task<TResult> doRawVerb<TResult>(restParameters parameters, httpVerbs verb, string restTemplate, params string[] args)
        {
            return doRawVerb<TResult, noModel>(parameters, verb, new noModel(), restTemplate, args);
        }

        public Task<TResult> doVerb<TResult, contentType>(restParameters parameters, httpVerbs verb, contentType content, string restTemplate, params string[] args)
            where TResult : class, new()
            where contentType : class, new()
        {
            //
            // resource: https://stackoverflow.com/questions/13086258/net-4-5-async-await-and-overloaded-methods
            //           https://stackoverflow.com/questions/3917249/the-type-arguments-for-method-cannot-be-inferred-from-the-usage
            //
            //string url;
            //try
            //{
            //    url = inetHelper.urlCombine(baseUri.AbsoluteUri, string.Format(restTemplate, args));
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("restAPI: Cannot build the request URL", ex);
            //}

            //Uri uri = new Uri(url);
            var uri = restTemplateToUri(restTemplate, args);
            return doVerbAsync<TResult, contentType>(parameters, verb, uri, content);
        }
        public Task<TResult> doVerb<TResult>(restParameters parameters, httpVerbs verb, string restTemplate, params string[] args)
                        where TResult : class, new()
        {
            return doVerb<TResult, noModel>(parameters, verb, new noModel(), restTemplate, args);
        }
        public Task<TResult> doGetVerb<TResult>(restParameters parameters, string restTemplate, params string[] args)
                        where TResult : class, new() 
        {
            return doVerb<TResult>(parameters, httpVerbs.get, restTemplate, args);
        }
        public Task<TResult> doDeleteVerb<TResult>(restParameters parameters, string restTemplate, params string[] args)
                where TResult : class, new()
        {
            return doVerb<TResult>(parameters, httpVerbs.delete, restTemplate, args);
        }

        #endregion (+) doVerb type of methods, giving polymorphism to the basic group of doVerb methods

        #region (+) Private and protected methods

        protected Uri restTemplateToUri(string restTemplate, params string[] args)
        {
            string url;
            try
            {
                Queue<string> arguments = new();
                foreach(var arg in args)
                {
                    arguments.Enqueue(WebUtility.UrlEncode(arg) );
                }
                url = inetHelper.urlCombine(baseUri.AbsoluteUri, string.Format(restTemplate, arguments.ToArray()));

                return new Uri(url);
            }
            catch (Exception ex)
            {
                throw new Exception($"restAPI: Cannot build the request URL: {restTemplate}", ex);
            }
        }

        private void buildInnerBaseUri()
        {
            string url = "http://localhost"; // (<) default

            if (!string.IsNullOrEmpty(_endpoint)) url = _endpoint; // (<) replace default with the endpoint

            if (!string.IsNullOrEmpty(_controllerName))
            {
                if (url.IndexOf("[controller]") >= 0) // (v) use the substitute strategy
                {
                    url = url.Replace("[controller]", _controllerName);
                }
                else // (v) use the combine strategy
                {
                    url = inetHelper.urlCombine(url, _controllerName); // (<) add the controller name
                }
            }

            baseUri = new(url);
        }

        // (v) this method will be used in the close future. Can replace the  xx_verb_xAnsyc() (eg:PutAsync) into doVerbAnsyc(), 
        private void codeToHold<TResult, contentType>(restParameters parameters, httpVerbs verb, Uri uri, contentType content)
        {
            string json = null;
            StringContent contentToSend = null;

            if (content == null) // (<) for verbs with content
            {
                switch (verb) // verbs with content only
                {
                    case httpVerbs.post:
                    case httpVerbs.put:
                    case httpVerbs.patch:
                        json = JsonSerializer.Serialize<contentType>(content, _serializerOptions);
                        contentToSend = new StringContent(json, Encoding.UTF8, "application/json");
                        break;
                }
            }



            //resource: https://stackoverflow.com/questions/35907642/custom-header-to-httpclient-request
            //
            var request = new HttpRequestMessage
            {
                RequestUri = uri,
                Method = restService.toHttpMethod(verb),
                Headers = {
                    { "X-Version", "1" }, // HERE IS HOW TO ADD HEADERS,
                    { HttpRequestHeader.Authorization.ToString(), "[your authorization token]" },
                    { HttpRequestHeader.ContentType.ToString(), "multipart/mixed" },//use this content type if you want to send more than one content type
                    },
                Content = new MultipartContent { // Just example of request sending multipart request
                    contentToSend
                    /*
                    new ObjectContent<[YOUR JSON OBJECT TYPE]>(
                        new [YOUR JSON OBJECT TYPE INSTANCE](...){...},
                        new JsonMediaTypeFormatter(),
                        "application/json"), // this will add 'Content-Type' header for the first part of request
                    */
                   /*
                    new ByteArrayContent([BINARY DATA]) {
                            Headers = { // this will add headers for the second part of request
                                { "Content-Type", "application/Executable" },
                            { "Content-Disposition", "form-data; filename=\"test.pdf\"" },
                        },
                    },*/
                }
            };
        }

        #endregion (+) Private and protected methods

        #region (+) Public Helper methods

        /// <summary>
        /// URL Encode a value
        /// </summary>
        /// <param name="value">The input value</param>
        /// <returns>The value URl Encoded</returns>
        public string Enc(string value) => WebUtility.UrlEncode(value);

        #endregion (+) Public Helper methods

        /*
         * (v) Static methods
         */

        /// <summary>
        /// Convert the input httpVerbs enumeration to HttpMethod 
        /// </summary>
        /// <param name="verb">The input httpVerbs</param>
        /// <returns>HttpMethod the equvelent of the input value</returns>
        public static HttpMethod toHttpMethod(httpVerbs verb)
        {
            switch (verb)
            {
                case httpVerbs.get:
                    return HttpMethod.Get;
                case httpVerbs.post:
                    return HttpMethod.Post;
                case httpVerbs.put:
                    return HttpMethod.Put;
                case httpVerbs.patch:
                    return HttpMethod.Patch;
                case httpVerbs.delete:
                    return HttpMethod.Delete;
                default:
                    throw new NotImplementedException("Verb is not implemented yet");
            }
        }

    }

}
