using System.CodeDom;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using FAST.Logging;
using FAST.Services.API;

namespace FAST.Core
{
    /// <summary>
    /// Helper for internet,TCP,UTP methods,  there are different versions for .NET and .NETCORE 
    /// </summary>
    public static partial class inetHelper
    {

        /// <summary>
        /// Check if a URL exists
        /// </summary>
        /// <param name="url">The Url to check</param>
        /// <returns>Boolean</returns>
        public static bool urlExists(string url)
        {
            try
            {
                WebRequest req = WebRequest.Create(url);
                //req.Credentials = new System.Net.NetworkCredential("rt-net\\aafent", "***");
                WebResponse res = req.GetResponse();
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Check if a string contains a URL/URI and optionaly if it is valid
        /// </summary>
        /// <param name="url">The string with the URI/URL</param>
        /// <param name="isValidOnlyWhenExists">True if has to check the existance of it</param>
        /// <returns>boolean, true if it is valid</returns>
        public static bool isValidURL(string url, bool isValidOnlyWhenExists)
        {
            Uri uriResult;
            bool isValid = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (isValid && isValidOnlyWhenExists)
            {
                isValid = urlExists(url);
            }

            return isValid;
        }


        /// <summary>
        /// Http to string
        /// </summary>
        /// <param name="uri">The requested URI</param>
        /// <param name="client">Optional, the http client to use</param>
        /// <returns>String with the http content or null</returns>
        public static async Task<string> httpToStringAsync(Uri uri, HttpClient client = null)
        {
            if (client == null ) client=new();
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode(); // Throws an exception for bad status codes (4xx, 5xx)
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                fastLogger.error($"Error downloading from {uri}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Http to string
        /// Synchronous version (use with caution in UI or performance-critical code)
        /// </summary>
        /// <param name="uri">The requested URI</param>
        /// <param name="client">Optional, the http client to use</param>
        /// <returns>String with the http content or null</returns>
        public static string httpToString(Uri uri, HttpClient client = null)
        {
            if (client == null) client = new();
            try
            {
                HttpResponseMessage response = client.GetAsync(uri).Result; // Blocking call
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result; // Blocking call
            }
            catch (HttpRequestException ex)
            {
                fastLogger.error($"Error downloading from {uri}: {ex.Message}");
                return null;
            }

            // (v) replace of obsolete version
            //public static string httpToString(Uri uri)
            //{
            //    WebClient client = new WebClient();
            //    return client.DownloadString(uri);
            //}

        }



        public static List<IPAddress> findAllIPs()
        {
            List<IPAddress> ips = new List<IPAddress>();
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();

                if (properties.DnsAddresses.Count > 0)
                    foreach (IPAddress ipAddress in properties.DnsAddresses)
                    {
                        ips.Add(ipAddress);
                    }
            }
            return ips;
        }


        /// <summary>
        /// Combines the url base and the relative url into one, consolidating the '/' between them
        /// </summary>
        /// <param name="urlBase">Base url that will be combined</param>
        /// <param name="relativeUrl">The relative path to combine</param>
        /// <returns>The merged url</returns>
        public static string urlCombine(string baseUrl, string relativeUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));

            if (string.IsNullOrWhiteSpace(relativeUrl))
                return baseUrl;

            baseUrl = baseUrl.TrimEnd('/');
            relativeUrl = relativeUrl.TrimStart('/');

            return $"{baseUrl}/{relativeUrl}";
        }

        /// <summary>
        /// Combines the url base and the array of relatives urls into one, consolidating the '/' between them
        /// </summary>
        /// <param name="urlBase">Base url that will be combined</param>
        /// <param name="relativeUrl">The array of relative paths to combine</param>
        /// <returns>The merged url</returns>
        public static string urlCombine(string baseUrl, params string[] relativePaths)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));

            if (relativePaths.Length == 0)
                return baseUrl;

            var currentUrl = urlCombine(baseUrl, relativePaths[0]);

            return urlCombine(currentUrl, relativePaths.Skip(1).ToArray());
        }


        /// Gets the base url from a list with endpoints sperated by semicolon (;)
        /// </summary>
        /// <param name="endpoint">The input list of endpoints</param>
        /// <returns>The base url</returns>
        public static string getBaseUrl(string endpoints)
        {
            var urls = endpoints.Split(";");
            var baseUrl = urls.FirstOrDefault(g => g.StartsWith("https://")) ??
                            urls.FirstOrDefault(g => g.StartsWith("http://"));

            var p1 = baseUrl.IndexOf(apiConstants.routeForInvokation);
            if (p1 >= 0)
            {
                baseUrl = baseUrl.Substring(0, p1);
            } else
            {
                p1 = baseUrl.IndexOf("/api");
                if (p1 >= 0)
                {
                    baseUrl = baseUrl.Substring(0, p1);
                } 
                else
                {
                    Uri uri = new Uri(baseUrl);
                    baseUrl = $"{uri.Scheme}://{uri.Host}";

                    // Include port if it's not the default (80 for http, 443 for https)
                    if ((uri.Scheme == Uri.UriSchemeHttp && uri.Port != 80) ||
                        (uri.Scheme == Uri.UriSchemeHttps && uri.Port != 443))
                    {
                        baseUrl += $":{uri.Port}";
                    }
                }
            }

            return baseUrl;
        } 



        /// <summary>
        /// Remove the ending (if exists) api version from a url
        /// </summary>
        /// <param name="url">The input url</param>
        /// <param name="apiVersion">optional the API version to remove. If not specified the default is '/api-v' from the end of the url.</param>
        /// <returns>The url without the api version</returns>
        public static string removeEndingApiVersion(string url, string apiVersion = null)
        {
            // (v) probable inputs
            // https://spddev.inner.relational.gr/FASTServices/
            // https://spddev.inner.relational.gr/FASTServices
            // https://spddev.inner.relational.gr/FASTServices/api-v1001
            // https://spddev.inner.relational.gr/FASTServices/api-v1001/

            if (string.IsNullOrEmpty(url)) return url;

            // (v) url
            url = url.Trim(); // trim spaces
            if (url.EndsWith('/')) url = url.Substring(0, url.Length - 1); // remove the trailing '/'

            // (v) apiVersion
            if (string.IsNullOrEmpty(apiVersion)) apiVersion = string.Empty;
            apiVersion = apiVersion.Trim();
            if (apiVersion.EndsWith('/')) apiVersion = apiVersion.Substring(0, apiVersion.Length - 1); // remove the trailing '/'
            if (apiVersion.StartsWith('/')) apiVersion = apiVersion.Substring(1); // remove the leading '/'

            // (v) final processing
            if (string.IsNullOrEmpty(apiVersion))
            {
                int p1 = url.LastIndexOf('/');
                if (p1 >= 0)
                {
                    apiVersion = url.Substring(p1 + 1);
                    if (apiVersion.StartsWith("api-v"))
                    {
                        url = url.Substring(0, p1 - 1);
                    }
                }
            }
            else
            {
                if (url.EndsWith(apiVersion)) url = url.Substring(0, (url.Length - apiVersion.Length)); // remove the apiVersion
                url = url.Trim();
                if (url.EndsWith('/')) url = url.Substring(0, url.Length - 1); // remove the trailing '/'
            }

            return url;
        }


        /// <summary>
        /// Return the endpoint of a URL
        /// </summary>
        /// <example>The URL: https://example.com/api/v1/resource?id=123 will return: /api/v1/resource</example>
        /// <param name="url"></param>
        /// <returns>string, the endpoint</returns>
        public static string getWebServiceEndpoint(string url)
        {
            // Parse the URL using Uri
            var uri = new Uri(url);

            // Extract the path and remove any query or fragment
            string endpoint = uri.AbsolutePath;

            return endpoint;
        }

        /// <summary>
        /// Remove the last part of an input Url
        /// <example>
        /// The URL: https://example.com/api/v1 will return: https://example.com/api
        /// </example>
        /// </summary>
        /// <param name="url">The input Url</param>
        /// <param name="expectAValidUri">True if the input is a well formated Uri. The return will reconstructed. False if is a part of Url (like endpoint)</param>
        /// <returns>stirng, the remaining Url</returns>
        public static string removeLastPartOfUrl(string url, bool expectAValidUri)
        {
            string path=null;
            Uri uri=null;
            if (expectAValidUri)
            {
                uri = new Uri(url);  // (<) Parse the URL, error is it is not well-formated
                path = uri.AbsolutePath; // (<) Extract the path
            } else
            {
                path=url; // (<) trust and use the input as part of an Uri/Url, eg. an endpoint
            }

            // Remove the last segment of the path
            string[] segments = path.TrimEnd('/').Split('/');
            if (segments.Length > 1)
            {
                path = string.Join("/", segments, 0, segments.Length - 1);
            }

            if (expectAValidUri) // (<) Reconstruct the URL with the updated path
            {
                return $"{uri.Scheme}://{uri.Host}{path}";
            } else
            {
                return path;
            }
            
            
        }



        /// <summary>
        /// Remove the first part of an input Url
        /// <example>
        /// The URL: https://example.com/api/v1 will return: https://example.com/api
        /// </example>
        /// </summary>
        /// <param name="url">The input Url</param>
        /// <param name="expectAValidUri">True if the input is a well formated Uri. The return will reconstructed. False if is a part of Url (like endpoint)</param>
        /// <returns>stirng, the remaining Url</returns>
        public static string removeFirstPartOfUrl(string url, bool expectAValidUri)
        {
            string path = null;
            Uri uri = null;
            if (expectAValidUri)
            {
                uri = new Uri(url);  // (<) Parse the URL, error is it is not well-formated
                path = uri.AbsolutePath; // (<) Extract the path
            }
            else
            {
                path = url; // (<) trust and use the input as part of an Uri/Url, eg. an endpoint
            }

            // Remove the last segment of the path
            string[] segments = path.TrimStart('/').Split('/');
            if (segments.Length > 2)
            {
                path = string.Join("/", segments, 1, segments.Length - 1 );
            }
           

            if (expectAValidUri) // (<) Reconstruct the URL with the updated path
            {
                return $"{uri.Scheme}://{uri.Host}{path}";
            }
            else
            {
                return path;
            }


        }



        /// <summary>
        ///     Convert url query string to IDictionary value key pair
        /// </summary>
        /// <param name="queryString">query string value</param>
        /// <returns>IDictionary value key pair</returns>
        public static IDictionary<string, string> queryStringToDictionary(string queryString)
        {
            if (string.IsNullOrWhiteSpace(queryString))
            {
                return null;
            }
            if (!queryString.Contains("?"))
            {
                return null;
            }
            string query = queryString.Replace("?", "");
            if (!query.Contains("="))
            {
                return null;
            }
            return query.Split('&').Select(p => p.Split('=')).ToDictionary(
                key => key[0].ToLower().Trim(), value => value[1]);
        }



        // (v) additions 11/9/2023

        /// <summary>
        /// Extract a domain name from a full URL
        /// </summary>
        /// <param name="Url">Full URL</param>
        /// <returns>Domain</returns>
        public static string extractDomainNameFromURL(string Url)
        {
            string rtn = "";
            if (!string.IsNullOrEmpty(Url))
            {
                rtn = System.Text.RegularExpressions.Regex.Replace(
                            Url,
                            @"^([a-zA-Z]+:\/\/)?([^\/]+)\/.*?$",
                            "$2"
                        );
            }

            return rtn;
        }

        /// <summary>
        /// Gets a number from a IPv4
        /// </summary>
        /// <param name="Ip">IPv4 to convert in a number</param>
        /// <returns>System.Decimal.</returns>
        public static decimal ip4ToNumber(string Ip)
        {
            decimal ipnum = 0;

            // verifiy the IP is an IPv4
            // IPv6 has :
            if (Ip.IndexOf(":") < 0)
            {
                string[] split = Ip.Split('.');
                ipnum = Convert.ToInt64(split[0]) * (256 * 256 * 256) + Convert.ToInt64(split[1]) * (256 * 256) +
                        Convert.ToInt64(split[2]) * 256 + Convert.ToInt64(split[3]);
            }

            return ipnum;
        }

        /// <summary>
        /// Checks the ip valid.
        /// </summary>
        /// <param name="strIP">The string ip.</param>
        /// <returns>System.String.</returns>
        public static string checkIPValid(string strIP)
        {
            //IPAddress result = null;
            //return !String.IsNullOrEmpty(strIP) && IPAddress.TryParse(strIP, out result);
            IPAddress address;
            if (IPAddress.TryParse(strIP, out address))
            {
                switch (address.AddressFamily)
                {
                    case System.Net.Sockets.AddressFamily.InterNetwork:
                        // we have IPv4
                        return "ipv4";
                    //break;
                    case System.Net.Sockets.AddressFamily.InterNetworkV6:
                        // we have IPv6
                        return "ipv6";
                    //break;
                    default:
                        // umm... yeah... I'm going to need to take your red packet and...
                        return null;
                        //break;
                }
            }
            return null;
        }


        /// <summary>
        /// UDP Broadcast a string value to a Port
        /// </summary>
        /// <param name="port"></param>
        /// <param name="message">The message to broad cast</param>
        public static void udpBroadcast(int port, string message)
        {
            using (UdpClient udpClient = new UdpClient())
            {
                // Enable broadcasting
                udpClient.EnableBroadcast = true;
                // Convert the message to bytes
                byte[] data = Encoding.UTF8.GetBytes(message);

                // Set the broadcast address (255.255.255.255 for broadcasting to all)
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, port);

                // Send the message
                udpClient.Send(data, data.Length, endPoint);
                fastLogger.info($"Broadcasted to port {port}: {message} ");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port">Port to listen</param>
        /// <param name="incomingMessage">The lamda method to handle the incoming message. If returns true, the listening loop will stop</param>
        /// <param name="listenForDuration">The duration to listen, or, null value for ever loop</param>
        public static void udpListen(int port, Func<string, bool> incomingMessage, TimeSpan? listenForDuration=null)
        {
            bool workForEver = listenForDuration == null;
            TimeSpan duration = listenForDuration.Value;

            using (UdpClient udpClient = new UdpClient(port))
            {
                // Set a timeout for the listener (optional, in case no data arrives)
                udpClient.Client.ReceiveTimeout = 1000; // 1-second timeout to allow checking the time

                fastLogger.info(workForEver? $"Listening for broadcasts on port {port} for {duration.TotalMinutes} minutes..." : $"Listening for on UDP port {port}..."); 
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
                DateTime endTime = DateTime.Now.Add(duration);
                while (true)
                {
                    if (!workForEver) 
                    {
                        if ( !(DateTime.Now < endTime) ) break;
                    }

                    try
                    {
                        // Receive data
                        byte[] receivedData = udpClient.Receive(ref endPoint);
                        string receivedMessage = Encoding.UTF8.GetString(receivedData);

                        fastLogger.info($"Received from {endPoint.Address}: {receivedMessage}");
                        if (incomingMessage!=null)
                        { 
                            if ( incomingMessage(receivedMessage) ) 
                            {
                                fastLogger.info($"Last received message requests to stop the listening");
                                break;
                            }
                        }

                    }
                    catch (SocketException ex)
                    {
                        if (ex.SocketErrorCode == SocketError.TimedOut)
                        {
                            // Timeout occurred, continue waiting for data
                            continue;
                        }

                        fastLogger.error($"Socket error: {ex.Message}");
                        break;
                    }
                }
                fastLogger.info("Stopped listening (time limit reached).");
            }
        }

        /// <summary>
        /// Download a file and save it at a specific location
        /// </summary>
        /// <param name="httpClient">The input HttpClient</param>
        /// <param name="downloadUrl">The full path to download</param>
        /// <param name="destinationFilePath">The full file path to save the downloaded file</param>
        /// <param name="hideTheExceptions">True if hide the exceptions</param>
        /// <returns>Boolean,True, if succeed, False if an error when the exceptions are hidden</returns>
       public static async Task<bool> downloadFileAndSave(HttpClient httpClient, string downloadUrl, string destinationFilePath, bool hideTheExceptions)
        {
            if (httpClient == null ) throw new ArgumentNullException("Specify HttpClient");
            try
            {
                using (var response = await httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode(); // Throw if not a successful status code

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await contentStream.CopyToAsync(memoryStream);
                            memoryStream.Position = 0; // Reset position to start reading

                            // Save memory stream to a file
                            using (var fileStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write))
                            {
                                await memoryStream.CopyToAsync(fileStream);
                            }

                            return true; // Download and save successful
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                fastLogger.error($"HTTP Request Error: {ex.Message}");
                if (!hideTheExceptions) throw;
                return false;
            }
            catch (Exception ex)
            {
                fastLogger.error($"An error occurred: {ex.Message}");
                if (!hideTheExceptions) throw;
                return false;
            }
        }

        /// <summary>
        /// Download a file and return it as MemoryStream
        /// </summary>
        /// <param name="httpClient">The input HttpClient</param>
        /// <param name="downloadUrl">The full path to download</param>
        /// <param name="hideTheExceptions">True if hide the exceptions</param>
        /// <returns>MemoryStream,if succeed, null if an error when the exceptions are hidden</returns>
        public static async Task<MemoryStream> downloadFileToMemoryStreamAsync(HttpClient httpClient, string downloadUrl, bool hideTheExceptions)
        {
            if (httpClient == null) throw new ArgumentNullException("Specify HttpClient");
            try
            {
                using (var response = await httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    {
                        var memoryStream = new MemoryStream();
                        await contentStream.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;
                        return memoryStream;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                fastLogger.error($"HTTP Request Error: {ex.Message}");
                if (!hideTheExceptions) throw;
                return null;
            }
            catch (Exception ex)
            {
                fastLogger.error($"An error occurred: {ex.Message}");
                if (!hideTheExceptions) throw;
                return null;
            }
        }

        /// <summary>
        /// Download a file and return it as MemoryStream
        /// </summary>
        /// <param name="httpClient">The input HttpClient</param>
        /// <param name="downloadUrl">The full path to download</param>
        /// <param name="hideTheExceptions">True if hide the exceptions</param>
        /// <returns>MemoryStream,if succeed, null if an error when the exceptions are hidden</returns>
        public static MemoryStream downloadFileToMemoryStream(HttpClient httpClient, string downloadUrl, bool hideTheExceptions)
        {
            if (httpClient == null) throw new ArgumentNullException("Specify HttpClient");
            try
            {
                using (var response = httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead).Result)
                {
                    response.EnsureSuccessStatusCode();

                    using (var contentStream = response.Content.ReadAsStream())
                    {
                        var memoryStream = new MemoryStream();
                        contentStream.CopyTo(memoryStream);
                        memoryStream.Position = 0;
                        return memoryStream;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                fastLogger.error($"HTTP Request Error: {ex.Message}");
                if (!hideTheExceptions) throw;
                return null;
            }
            catch (Exception ex)
            {
                fastLogger.error($"An error occurred: {ex.Message}");
                if (!hideTheExceptions) throw;
                return null;
            }
        }


    }
}






