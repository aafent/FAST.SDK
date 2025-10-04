using System.Runtime.Serialization;

namespace FAST.Core.Models
{

    [DataContract]
    public class rowsResponse<T> : serviceResponse, Irowset<T>
    {
        [DataMember]
        public List<T> data { get; set; }

    }

}