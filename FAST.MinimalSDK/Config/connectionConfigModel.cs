using FAST.Config;
using FAST.Core;

namespace FAST.Services.Models
{
    /// <summary>
    /// Connections configuration model
    /// </summary>
    public class connectionConfigModel : elementaryModel
    {
        /// <summary>
        /// Local variables, available to all connections
        /// </summary>
        public List<FastKeyValuePair<string,string>> variables { get;set; }

        /// <summary>
        /// Connection sources
        /// </summary>
        public List<connectionConfigItem> sources {  get;set; }
    }
}
