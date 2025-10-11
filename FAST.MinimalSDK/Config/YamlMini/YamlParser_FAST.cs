using FAST.Core.Models;
using System.Text;

namespace FAST.Config.YamlMini
{
    public partial class YamlParser : ImultiErrorCarrier
    {
        public List<string> errors 
        {
            get
            {
                return parsingErrors.Select(e=>$"{e.Item1}: {e.Item2}" ).ToList();
            }
            set
            {
                clearAllErrors();
                value.ForEach(e=>Error(e));
            }
        }
        public string errorText
        {
            get
            {
                return hasError ? parsingErrors.First().Item2 : string.Empty;
            }
            set
            {
                // do nothing
            }
            
        }
        public string extendedErrorText
        {
            get
            {
                
                int last = parsingErrors.Count;
                if (last < 1 ) return string.Empty;

                string text = string.Empty;
                if ( last >= 3) last=3;
                for ( int inx=1; inx<=last; inx++)
                {
                    if ( ! string.IsNullOrEmpty(text)) text+="\n";
                    text += parsingErrors[inx].Item2 ?? string.Empty;
                }
                return text;
            }
            set
            {
                // do noting
            }

        }
        public bool hasError 
        { 
            get
            {
                return (parsingErrors.Count > 0);
            }
            set
            {
                // do nothing
            }
        }

        public string getErrorMessages()
        {
            StringBuilder text = new StringBuilder();
            foreach (Tuple<int, string> msg in parsingErrors)
            {
                text.Append(Input.FormErrorMessage(msg.Item1, msg.Item2));
                text.AppendLine();
            }
            return text.ToString();
        }

        public void clearAllErrors()
        {
            ClearError(this.parsingErrors.Count);
        }
    }

}
