using System.Runtime.Serialization;
using FAST.Core.Models;

namespace FAST.Core
{
    /// <summary>
    /// A Service Response containing a result a string value
    /// </summary>
    [DataContract]
    public class stringResponse : rowResponse<string>
    {

        /// <summary>
        /// Constructor without arguments
        /// </summary>
        public stringResponse()
            : base()
        {
        }

    }

}