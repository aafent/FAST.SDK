using FAST.Core.Models;
using System.Text.Json.Serialization;

namespace FAST.Services.Models.Data
{

    public class columnDescriptor  : IconsumerNormalization
    {
        public int id {  get;set; }
        public string name { get; set; }
        public string typeName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]

        public string label { get; set; } =null;


        [NonSerialized] 
        public Type type;


        public void consumerNormalization(Func<int> ext=null)
        {
            this.type=Type.GetType(this.typeName);

        }
    }
}
