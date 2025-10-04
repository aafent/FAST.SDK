using FAST.DB;
using System.Xml.Serialization;

namespace FAST.Config
{

    /// <summary>
    /// Connection configuration Item
    /// </summary>
    public class connectionConfigItem
    {
        /// <summary>
        /// The working environment 
        /// </summary>
        [XmlAttribute]
        public string environment { get; set; }

        /// <summary>
        /// The name of the connection item
        /// </summary>
        [XmlAttribute]
        public string name { get; set; }

        /// <summary>
        /// The Role of the connection
        /// </summary>
        [XmlAttribute]
        public string role { get; set; }

        /// <summary>
        /// An ADO provider. Default is the ODBC
        /// </summary>
        [XmlAttribute]
        public adoProviderTypes provider { get; set; } = adoProviderTypes.odbc;

        /// <summary>
        /// A provider's override
        /// </summary>
        public string providerOverride {  get;set; } = null;

        /// <summary>
        /// The connection string or the URL
        /// </summary>
        public string cs { get; set; }

        /// <summary>
        /// Comments of the connection
        /// </summary>
        public string comments { get; set; }

        /// <summary>
        /// Tags, comma seperated list.
        /// </summary>
        [XmlAttribute]
        public string tag { get; set; }


        /// <summary>
        /// Return the provider of a connectionConfigItem
        /// </summary>
        /// <param name="item">The item to extract the provider</param>
        /// <param name="toUpper">Optional, if true (default) the provider will returnd in uppercase</param>
        /// <returns>String, the provider</returns>
        public static string getProvider(connectionConfigItem item, bool toUpper=true)
        {
            if (toUpper)
            {
                if (!string.IsNullOrWhiteSpace(item.providerOverride)) return item.providerOverride.ToUpper();
                return item.provider.ToString().ToUpper();
            }
            if (!string.IsNullOrWhiteSpace(item.providerOverride)) return item.providerOverride;
            return item.provider.ToString();
        }
    }
}
