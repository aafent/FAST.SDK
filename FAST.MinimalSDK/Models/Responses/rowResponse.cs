using System.Runtime.Serialization;

namespace FAST.Core.Models
{

    [DataContract]
    public class rowResponse<T> : serviceResponse, IRow<T>
    {
        [DataMember]
        public T data { get; set; }

    }





}