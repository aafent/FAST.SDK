using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace FAST.API
{
    //https://stackoverflow.com/questions/40014047/add-client-certificate-to-net-core-httpclient

    /// <summary>
    /// Collection with http handlers profiles
    /// </summary>
    public static class httpClientAndHandlersSetup
    {
        /// <summary>
        /// Default used by the fast services for server to server interaction.
        /// </summary>
        /// <returns>HttpClientHandler</returns>
        public static HttpClientHandler defaultHttpHandler()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.UseDefaultCredentials = true;
            handler.ServerCertificateCustomValidationCallback +=
                            (sender, certificate, chain, errors) =>
                            {
                                return true;
                            };
            return handler;
        }

        /// <summary>
        /// Windows Authedication, similar to the default, but wihout to set a ServerCertificateCustomValidationCallback
        /// </summary>
        /// <returns>HttpClientHandler</returns>
        public static HttpClientHandler windowsAuthentication()
        {
            var handler = new HttpClientHandler();
            handler.UseDefaultCredentials = true; // This might work in trusted environments

            return handler;
        }

        /// <summary>
        /// Network credentials handler
        /// <example>
        ///     client invocation will be like:
        ///     using (var client = new HttpClient(handler)) {
        ///        client.DefaultRequestHeaders.Authorization =        
        ///        new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        ///      }
        /// </example>
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static HttpClientHandler networkCredentials(string username, string password)
        {
            var handler = new HttpClientHandler();
            var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
            handler.Credentials = new NetworkCredential(username, password);

            return handler;
        }

        /// <summary>
        /// Token-Based Authentication
        /// Setup a HttpClient to be used with Bearer Token
        /// </summary>
        /// <param name="inputClient">The client to setup</param>
        /// <param name="bearerToken">The token</param>
        /// <returns>The setup client</returns>
        public static HttpClient clientWithTokenBasedAuthentication(HttpClient inputClient, string bearerToken)
        {
            inputClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            return inputClient;
        }

        /// <summary>
        /// Basic Authentication
        /// Setup a HttpClient to be used with Basic Authentication
        /// </summary>
        /// <param name="inputClient">The client to setup</param>
        /// <param name="username">The user name</param>
        /// <param name="password">The Password</param>
        /// <returns>HttpClient, The http client to work with</returns>
        public static HttpClient clientWithBasicAuthentication(HttpClient inputClient, string username, string password)
        {
            var authString = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
            inputClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);
            return inputClient;
        }


        /// <summary>
        /// Client Certificates
        /// Setup a HttpClient to be used with Client Certificates
        /// <example>
        /// ...
        /// var handler = new HttpClientHandler();
        /// handler.ClientCertificates.Add(new X509Certificate2("your_certificate.pfx", "your_password"));
        /// HttpClient client=new(handler);
        /// client=clientWithClientCertificates(client, username, password )
        /// ...
        /// </example>
        /// </summary>
        /// <param name="inputClient">The client to setup</param>
        /// <param name="username">The user name</param>
        /// <param name="password">The Password</param>
        /// <returns>HttpClient, The http client to work with</returns>
        public static HttpClient clientWithClientCertificates(HttpClient inputClient, string username, string password)
        {
            var authString = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
            inputClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);
            return inputClient;
        }

    }


}
