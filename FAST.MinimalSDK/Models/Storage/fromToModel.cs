using FAST.Core;

namespace FAST.Services.Models
{

    public class fromToModel<T>: IniceToInstantiateOnConstructor where T: new()
    { 
        public T from { get; set; }
        public T to { get; set; }
    }
}
