
namespace FAST.API
{

    /// <summary>
    /// Parameters for REST API Calls
    /// </summary>
    public class restParameters
    {
        private List<Tuple<string, string>> _headers = new();

        /// <summary>
        /// Headers that will be added to the RESTCall 
        /// </summary>
        public List<Tuple<string, string>> headers
        {
            get
            {
                return _headers;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Headers cannot be null");
                }
                _headers = value;
            }

        }

        /// <summary>
        /// Get the response as a stream. Caller should read the restService.resultStream
        /// default is no (false) and has priority against the responseIsJSON parameter
        /// </summary>
        public bool responseAsStream { get; set; } = false;

        /// <summary>
        /// Declares that the response is JSON Format. 
        /// Default is true
        /// </summary>
        public bool responseIsJSON { get; set; } = true;

        /// <summary>
        /// No arguments constructor
        /// </summary>
        public restParameters()
        {
        }

        /// <summary>
        /// Constructor with one header as argument
        /// </summary>
        /// <param name="headerName">Header name</param>
        /// <param name="value">Header value</param>
        public restParameters(string headerName, string value) : this()
        {
            addHeader(headerName, value);
        }

        /// <summary>
        /// Constructor with a Header Tuple of two strings as argument
        /// </summary>
        /// <param name="header">The header to be added</param>
        public restParameters(Tuple<string, string> header) : this(header.Item1, header.Item2)
        {
        }

        /// <summary>
        /// Value to used with Authorization  Token. To set the value use method: addAuthorizationUsingBearerToken()
        /// </summary>
        public string authTokenValue { get; private set; } = null;

        /// <summary>
        /// Schema to used with Authorization header
        /// </summary>
        public string authTokenSchema {  get; set; } = "Bearer";

        /// <summary>
        /// Has authorization value, true if has. 
        /// </summary>
        public bool hasAuthTokenValue
        {
            get
            {
                return string.IsNullOrEmpty(authTokenValue);
            }
        }

        /// <summary>
        /// Add header by name once
        /// </summary>
        /// <param name="headerName">Header Name</param>
        /// <param name="value">Header value</param>
        public void addHeader(string headerName, string value)
        {
            if (!_headers.Any(h=>h.Item1 ==headerName) )
            {
                _headers.Add(new Tuple<string, string>(headerName, value));
            }
        }

        /// <summary>
        /// Declare that Authorization Using bearer token will used
        /// Equiv. to: HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        /// </summary>
        /// <param name="tokenValue">Token value</param>
        /// <param name="tokenSchema">Optional, Token Schema</param>
        public void addAuthorizationUsingBearerToken(string tokenValue, string tokenSchema= "Bearer")
        {
            this.authTokenValue = authTokenValue;
            this.authTokenSchema = tokenSchema;
        }
        

    }


}
