namespace FAST.Services.Models
{
    /// <summary>
    /// FAST Automatic Configuration 
    /// </summary>
    public class fastConfigModel
    {
        public fastServicesLocation location { get; set; }

        /// <summary>
        /// inner class for location
        /// </summary>
        public class fastServicesLocation
        {
            /// <summary>
            /// The Main endpoint
            /// <example>https://gbcs.eu/FASTServices/api-v1001</example>
            /// </summary>
            public string endpoint { get; set; }

            /// <summary>
            /// An alternative to the main (optional)
            /// <example>https://spddev1.relationalfs.com/FASTServices/api-v1001</example>
            /// </summary>
            public string alternativeEndpoint { get; set; } 

            /// <summary>
            /// If true, will configure both endpoints, otherwise onle the main.
            /// </summary>
            public bool configureBothEndpoints {  get; set; }


            /// <summary>
            /// The endpoint to the agent or null
            /// <example>https://gbcs.eu/FASTAGENT/api-v1001</example>
            /// </summary>
            public string agentEndpoint { get; set; } = null;
        }     

    }
}
