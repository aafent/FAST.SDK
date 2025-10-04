using System.Runtime.Serialization;
using FAST.Core.Models;

namespace FAST.Core
{
    /// <summary>
    /// A Service Response containing a List of strings field with name: data
    /// </summary>
    [DataContract]
    public class stringCollectionResponse : serviceResponse
    {

        /// <summary>
        /// Constructor without arguments
        /// </summary>
        public stringCollectionResponse():base()
        {
        }

        /// <summary>
        /// The Collection (List) of strings 
        /// </summary>
        [DataMember]
        public List<string> data { get; set; }
    }

}