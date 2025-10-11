using FAST.Config.YamlMini.Grammar;
using System.Text;


namespace FAST.Config.YamlMini
{
    public partial class YamlParser
    {
        private int position;

        public int Position
        {
            get { return position; }
            set { position = value; }
        }

        IParserInput<char> Input;

        public List<Tuple<int, string>> parsingErrors = new List<Tuple<int, string>>();
        private Stack<int> ErrorStack = new Stack<int>();

        public YamlParser() { }

        private void SetInput(IParserInput<char> input)
        {
            Input = input;
            position = 0;
        }

        private bool TerminalMatch(char terminal)
        {
            if (Input.HasInput(position))
            {
                char symbol = Input.GetInputSymbol(position);
                return terminal == symbol;
            }
            return false;
        }

        private bool TerminalMatch(char terminal, int pos)
        {
            if (Input.HasInput(pos))
            {
                char symbol = Input.GetInputSymbol(pos);
                return terminal == symbol;
            }
            return false;
        }

        private char MatchTerminal(char terminal, out bool success)
        {
            success = false;
            if (Input.HasInput(position))
            {
                char symbol = Input.GetInputSymbol(position);
                if (terminal == symbol)
                {
                    position++;
                    success = true;
                }
                return symbol;
            }
            return default(char);
        }

        private char MatchTerminalRange(char start, char end, out bool success)
        {
            success = false;
            if (Input.HasInput(position))
            {
                char symbol = Input.GetInputSymbol(position);
                if (start <= symbol && symbol <= end)
                {
                    position++;
                    success = true;
                }
                return symbol;
            }
            return default(char);
        }

        private char MatchTerminalSet(string terminalSet, bool isComplement, out bool success)
        {
            success = false;
            if (Input.HasInput(position))
            {
                char symbol = Input.GetInputSymbol(position);
                bool match = isComplement ? terminalSet.IndexOf(symbol) == -1 : terminalSet.IndexOf(symbol) > -1;
                if (match)
                {
                    position++;
                    success = true;
                }
                return symbol;
            }
            return default(char);
        }

        private string MatchTerminalString(string terminalString, out bool success)
        {
            int currrent_position = position;
            foreach (char terminal in terminalString)
            {
                MatchTerminal(terminal, out success);
                if (!success)
                {
                    position = currrent_position;
                    return null;
                }
            }
            success = true;
            return terminalString;
        }

        private int Error(string message)
        {
            parsingErrors.Add(new Tuple<int, string>(position, message));
            return parsingErrors.Count;
        }

        private void ClearError(int count)
        {
            parsingErrors.RemoveRange(count, parsingErrors.Count - count);
        }

        public string GetErrorMessages()
        {
            StringBuilder text = new StringBuilder();
            foreach (Tuple<int, string> msg in parsingErrors)
            {
                text.Append(Input.FormErrorMessage(msg.Item1, msg.Item2));
                text.AppendLine();
            }
            return text.ToString();
        }



    }
}
