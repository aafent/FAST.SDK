using System.Text.RegularExpressions;

namespace FAST.Strings
{
    public class tokenizer
    {
        public enum tokenType : sbyte { text = 0, beginDelimeter = 1, endDelimeter=2, textBetweenDelimeters=3 }
        public class tokenEntry
        {
            public tokenType type;
            public string token;
        }

        private string defaultRegEx = @"(\x29|\x28|%more%|\x2b|\x2d|\x2a|\x2f)";
        public List<string> tokenDelimeters = new List<string>();
        public List<string> results { private set; get; }
        public tokenizer()
        {
            results = new List<string>();
        }
        public tokenizer(string expression)
            : this()
        {
            setExpression(expression);
        }
        public tokenizer(List<string> tokens, string expression)
            : this()
        {
            this.results = tokens;
            setExpression(expression);
        }
        public void setExpression(string expression)
        {
            results.Clear();
            string moreDelimeters = String.Join("|", tokenDelimeters.ToArray());
            if (String.IsNullOrEmpty(moreDelimeters))
            {
                moreDelimeters = "|";
            }
            else
            {
                moreDelimeters = "|" + moreDelimeters + "|";
            }
            string regExpr = defaultRegEx.Replace("|%more%|", moreDelimeters);
            Regex regex = new Regex(regExpr, RegexOptions.IgnoreCase);
            string[] rawTokins = regex.Split(expression);
            for (int x = 0; x < rawTokins.Length; x++)
            {
                string currentTokin = rawTokins[x].Trim();
                if (currentTokin == null || currentTokin == String.Empty) continue; //workaround: sometimes regex will bring back empty entries, skip these

                results.Add(currentTokin);
            }
        }


        public static Queue<tokenEntry> queueTokens(string input, string beginDelimeter, string endDelimeter, char escapeChar, bool removeEscapeCharacters)
        {
            Queue<tokenEntry> tokens = new Queue<tokenEntry>();

            string nextToken = "";
            int nextPosition = 0;
            int tempPosition = 0;
            int startingPosition = 0;
            string searchingDelimeter = "";
            tokenType searchForTokenType = tokenType.beginDelimeter;
            while (startingPosition >= 0)
            {
                searchingDelimeter = searchForTokenType == tokenType.beginDelimeter ? beginDelimeter : endDelimeter;

                // (v) check for delimiter
                tempPosition = startingPosition;
                while (true)
                {
                    nextPosition = input.IndexOf(searchingDelimeter, tempPosition);
                    if (nextPosition < 0) { break; } // reach the end of the input
                    if ( nextPosition>0 && input[nextPosition - 1] == escapeChar)
                    {
                        tempPosition = nextPosition + 1;
                        if (tempPosition > input.Length) // reach at the end of the input and the delimiter is escaped
                        {
                            nextPosition = -1;
                            break;
                        }
                        continue; // reloop to locate the next unescaped position
                    }
                    break; // just exist from the infinity loop
                }

                if (nextPosition < 0) // (<) no starting token found
                {
                    nextToken = input.Substring(startingPosition);
                    tokens.Enqueue(new tokenEntry() { type = tokenType.text, token = removeEscapeCharacters?removeEscapeCharacter(nextToken,escapeChar):nextToken });
                    startingPosition = -1;
                    break; // no reason for farther loop
                }
                else
                {
                    nextToken = input.Substring(startingPosition, nextPosition - startingPosition);
                    if (searchForTokenType == tokenType.endDelimeter )
                    {
                        tokens.Enqueue(new tokenEntry() { type = tokenType.textBetweenDelimeters, token = removeEscapeCharacters?removeEscapeCharacter(nextToken, escapeChar):nextToken });
                    }
                    else
                    {
                        tokens.Enqueue(new tokenEntry() { type = tokenType.text, token = removeEscapeCharacters?removeEscapeCharacter(nextToken, escapeChar):nextToken });
                    }
                    nextToken = input.Substring(nextPosition, searchingDelimeter.Length);
                    tokens.Enqueue(new tokenEntry() { type = searchForTokenType, token = nextToken });
                    startingPosition = nextPosition + searchingDelimeter.Length; // use this position as the next for ending-or-starting delimeter check
                    searchForTokenType = searchForTokenType == tokenType.beginDelimeter ? tokenType.endDelimeter : tokenType.beginDelimeter;
                }

            } // while loop

            return tokens;
        }

        public static string removeEscapeCharacter( string input, char escapeChar)
        {
            string output = "";
            string delimeter = escapeChar.ToString();
            string[] split = { delimeter + delimeter };

            foreach( var part in input.Split( split, StringSplitOptions.None ) )
            {
                if (!string.IsNullOrEmpty(output))
                {
                    output += delimeter;
                }
                output+= part.Replace(delimeter, "");
            }
            return output;
        }

        public static string nextToken(ref string stringList, string delimiter)
        {
            string token = "";
            int p1 = 0;
            p1 = stringList.IndexOf(delimiter);
            if (p1 >= 0)
            {
                token = stringList.Substring(0, p1);
                stringList = stringList.Substring(p1 + delimiter.Length);
            }
            else
            {
                token = stringList;
                stringList = "";
            }
            return token;
        }

        public static string findThenSubstring(ref string input, bool modifyInput, string searchFor, string startToken, string endToken)
        {
            string value = "";
            string copy = input;

            int p1 = copy.IndexOf(searchFor);
            if (p1 < 0)
            {
                if (modifyInput) { input = string.Empty; }
                return string.Empty;
            }
            copy = copy.Substring(p1 + searchFor.Length);

            p1 = copy.IndexOf(startToken);
            if (p1 < 0)
            {
                if (modifyInput) { input = string.Empty; }
                return string.Empty;
            }
            copy = copy.Substring(p1 + startToken.Length);


            p1 = copy.IndexOf(endToken);
            if (p1 < 0)
            {
                if (modifyInput) { input = string.Empty; }
                return string.Empty;
            }

            value = copy.Substring(0, p1);
            copy = copy.Substring(p1 + endToken.Length);



            if (modifyInput) { input = copy; }
            return value;
        }

        public static IEnumerable<string> splitTokenized(string delimeter, string input)
        {
            List<string> result = new();
            if (input.IndexOf(delimeter) > 0)
            {
                var partSet = input.Split(delimeter);
                int inx = 0;
                foreach (var token in partSet)
                {
                    inx++;
                    if (token == null) continue;
                    if (token == String.Empty) continue;
                    if (token.Trim() == String.Empty) continue;

                    result.Add(token);
                    if (inx > 1) result.Add(delimeter);
                }
            }
            return result;
        }

        public static IEnumerable<string> tokenizeLikeMacroCommands(string input)
        {
            List<string> tokens = new List<string>();

            // (v) tokenize by spaces and commas but keep the double quotes
            //     see: https://stackoverflow.com/questions/4780728/regex-split-string-preserving-quotes/4780801#4780801
            //
            var set1 = Regex.Split(input, "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*)[ ,](?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            foreach (var part in set1)
            {
                if (part == null) continue;
                if (part == String.Empty) continue;
                if (part.Trim() == String.Empty) continue;
                if (part.StartsWith("\""))
                {
                    tokens.Add(part);
                    continue;
                }

                string[] leftParenthesis;
                string[] rightParenthesis;
                if (part.IndexOf("(") > 0)
                {
                    leftParenthesis = tokenizer.splitTokenized("(", part).ToArray();
                    foreach (var item in leftParenthesis)
                    {
                        rightParenthesis = tokenizer.splitTokenized(")", item).ToArray();
                        if (rightParenthesis.Length > 0)
                            tokens.AddRange(rightParenthesis);
                        else
                            tokens.Add(item);
                    }
                    continue;
                }
                if (part.IndexOf(")") > 0)
                {
                    rightParenthesis = tokenizer.splitTokenized(")", part).ToArray();
                    tokens.AddRange(rightParenthesis);
                    continue;
                }
                tokens.Add(part);
            }

            return tokens.ToArray();
        }
    }
}
