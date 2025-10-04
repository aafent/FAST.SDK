using FAST.Core;
using FAST.Strings;

namespace FAST.Data
{
    public class filterItem : IdataFilterItem
    {
        public variablesContainer variables { get; set; } = new();
        public List<string> quotable { get; set; } = new List<string>();
        public string syntax { get; set; }

        public override string ToString()
        {
            stringReplacer replace = new stringReplacer(this.variables);
            replace.addQuotesToValue = true;
            replace.variablesNeedQuotes = this.quotable;
            replace.setTemplate(this.syntax);
            return replace.replace(1);
        }

    }
}
