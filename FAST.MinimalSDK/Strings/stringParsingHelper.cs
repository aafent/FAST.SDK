using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace FAST.Strings
{
    /// <summary>
    /// string methods oriented to parsing process
    /// </summary>
    public static class stringParsingHelper
    {
        public const int noWordFound = (-1);
        public const int unclosedQuoteFound = (-2);


        public static int nextWord(string input, string extraValidCharacters, int startingPosition, bool bypassFirstWhite, ref string wordFound)
        {

            // is equiv to: 
            //bool foundInQuotes = false;
            //return nextWord(input, extraValidCharacters, startingPosition, bypassFirstWhite, null, ref wordFound, ref foundInQuotes);




            int End_Of_Word = (-1);
            string alphabetToUse = stringValue.latinCapitalAlphabet26Letters + stringValue.numericOnlyDigits + extraValidCharacters;
            wordFound = "";
            End_Of_Word = (-1);

            if (bypassFirstWhite)
            {
                for (int p2 = startingPosition; p2 <= (input.Length - 1); p2++)
                {
                    if (Convert.ToInt32(input[p2]) <= 32) { continue; }
                    startingPosition = p2;
                    break;
                }
            }
            for (int p2 = startingPosition; p2 <= (input.Length - 1); p2++)
            {
                if (alphabetToUse.IndexOf(input.Substring(p2, 1).ToUpper()) >= 0)
                {
                    wordFound += input.Substring(p2, 1);
                    End_Of_Word = p2;
                    continue; // next loop
                }
                break; // first non litteral character found
            }
            return End_Of_Word;
        }

        // (v) added 13 nov 2019
        public static int nextWord(string input, string extraValidCharacters, int startingPosition, bool bypassFirstWhite, stringsPair quotes, ref string wordFound, ref bool foundInQuotes)
        {
            int End_Of_Word = stringParsingHelper.noWordFound; // was -1
            string alphabetToUse = stringValue.latinCapitalAlphabet26Letters + stringValue.numericOnlyDigits + extraValidCharacters;
            wordFound = "";
            foundInQuotes = false; 
            End_Of_Word = stringParsingHelper.noWordFound; //was -1
            bool useQuotes = !(quotes == null);

            // (v) skip all characters less than the ASCII value 32 aka the non-printable characters
            if (bypassFirstWhite)
            {
                for (int p2 = startingPosition; p2 <= (input.Length - 1); p2++)
                {
                    if (Convert.ToInt32(input[p2]) <= 32) { continue; }
                    startingPosition = p2;
                    break;
                }
            }

            if (useQuotes && startingPosition<input.Length)
            {
                if (input.Substring(startingPosition, quotes.left.Length) == quotes.left)
                {
                    for (int p2 = startingPosition+quotes.left.Length; p2 <= (input.Length - 1); p2++)
                    {
                        if (input.Substring(p2, quotes.right.Length) == quotes.right)
                        {
                            End_Of_Word = p2 + quotes.right.Length;
                            foundInQuotes = true;
                            return End_Of_Word;
                        }
                        else
                        {
                            wordFound += input.Substring(p2, 1);
                            End_Of_Word = p2;
                            continue; // next loop
                        }
                    }
                    // if the flow reach this point, means that an unclosed quote found
                    foundInQuotes = true;
                    return stringParsingHelper.unclosedQuoteFound;
                }
            }


            // (v) continues by searching for the first non acceptable character
            for (int p2 = startingPosition; p2 <= (input.Length - 1); p2++)
            {
                if (alphabetToUse.IndexOf(input.Substring(p2, 1).ToUpper()) >= 0)
                {
                    wordFound += input.Substring(p2, 1);
                    End_Of_Word = p2;
                    continue; // next loop
                }
                break; // first non litteral character found
            }
            return End_Of_Word;
        }


        public static string keepLetters(string input, string extraValidCharacters)
        {
            string alphabetToUse = stringValue.latinCapitalAlphabet26Letters + extraValidCharacters;
            string output = "";
            foreach (char c1 in input)
            {
                if (alphabetToUse.IndexOf(c1) >= 0) { output += c1; }
            }
            return output;
        }
        public static string keepNumbers(string input, string extraValidCharacters)
        {
            string output = "";
            string alphabetToUse = extraValidCharacters + stringValue.numericOnlyDigits;
            foreach (char c1 in input)
            {
                if (alphabetToUse.IndexOf(c1) >= 0) { output += c1; }
            }
            return output;
        }

        public static IEnumerable<string> subStrings(string input, string startToken, string endToken)
        {
            Regex r = new Regex(Regex.Escape(startToken) + "(.*?)" + Regex.Escape(endToken));
            MatchCollection matches = r.Matches(input);
            foreach (Match match in matches)
                yield return match.Groups[1].Value;
        }
        public static string subStringsFirstOrNull(string input, string startToken, string endToken)
        { 
            // (i) added: 22/7/2021
            var result=subStrings(input,startToken, endToken).ToArray();
            if ( result == null ) return null;
            if ( result.Length < 1 ) return null;
            return result[0];
        }
        public static string subStringsFirstOrEmpty(string input, string startToken, string endToken)
        {
            // (i) added: 25/8/2021
            var result = subStringsFirstOrNull(input, startToken, endToken);
            return result==null ? string.Empty: result;
        }


        public static bool trySplitBetweenTokens(string input, string beginToken, string endToken, out string beforeString, out string betweenWithoutSurroundingTokens, out string betweenString, out string afterString)
        {
            beforeString = string.Empty;
            betweenString = string.Empty;
            afterString = string.Empty;
            betweenWithoutSurroundingTokens = string.Empty; 

            var p1 = input.IndexOf(beginToken);
            var p2 = 0;
            if (p1 >= 0) p2 = input.IndexOf(endToken, p1);
            if ((p1 >= 0) && (p2 >= 0))
            {
                beforeString = input.Substring(0, p1);
                betweenString = input.Substring(p1, p2 - p1 + endToken.Length);
                betweenWithoutSurroundingTokens = input.Substring(p1 + beginToken.Length, p2 - p1 - beginToken.Length );
                afterString = input.Substring(p2 + endToken.Length);

                return true;
            }
            return false;
        }

        public static bool trySplitBetweenTokens(string input, string beginToken, string endToken, out string beforeString, out string betweenString, out string afterString)
        {
            string betweenWithoutSurroundingTokens;
            return trySplitBetweenTokens(input, beginToken, endToken, out beforeString, out betweenWithoutSurroundingTokens, out betweenString, out afterString);
 
            //beforeString = string.Empty;
            //betweenString = string.Empty;
            //afterString = string.Empty;

            //var p1 = input.IndexOf(beginToken);
            //var p2 = 0;
            //if (p1 >= 0) p2 = input.IndexOf(endToken, p1);
            //if ((p1 >= 0) && (p2 >= 0))
            //{
            //    beforeString = input.Substring(0, p1);
            //    betweenString = input.Substring(p1, p2 - p1 + endToken.Length );
            //    afterString = input.Substring(p2 + endToken.Length );
            //    return true;
            //}
            //return false;
        }

        public static bool tryInsertBetweenTokens(string input, string beginToken, string endToken, string textToInsert, out string newText )
        {
            string left, right, textFound, textFoundNoWithoutSurroundingTokens;
            if (stringParsingHelper.trySplitBetweenTokens(input, beginToken, endToken, out left, out textFoundNoWithoutSurroundingTokens, out textFound, out right))
            {
                newText = left + beginToken + textFoundNoWithoutSurroundingTokens + textToInsert + endToken + right;
                return true;
            }
            else
            {
                newText = input;
                return false;
            }
        }

        public static bool isEmpty(string value)
        {
            if (value == null) { value = ""; }
            value = value.Replace("\n", "");
            value = value.Trim();
            return String.IsNullOrEmpty(value);
        }
        public static bool isNotEmpty(string value)
        {
            return (!isEmpty(value));
        }

        public static bool tryOffsetValue(string input, out bool isAbsolute, out short multiplier, out float value)
        {
            isAbsolute = true;
            multiplier = 0;
            value = 0F;

            if (isEmpty(input)) { return false; }
            input = keepNumbers(input, "+-.");
            if (isEmpty(input)) { return false; }


            switch (input.Substring(0, 1))
            {
                case "+":
                    multiplier = +1;
                    input = input.Substring(1);
                    isAbsolute = false;
                    break;
                case "-":
                    multiplier = -1;
                    input = input.Substring(1);
                    isAbsolute = false;
                    break;
                default:
                    multiplier = 0;
                    isAbsolute = true;
                    break;
            }

            return float.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out value  );
        }

        // (v) added: 10 nov 2019
        private static IEnumerable<char> readNext(string str, int currentPosition, int count)
        {
            for (var i = 0; i < count; i++)
            {
                if (currentPosition + i >= str.Length)
                {
                    yield break;
                }
                else
                {
                    yield return str[currentPosition + i];
                }
            }
        }
        public static IEnumerable<string> quotedSplit(string s, string delim, string quoteCharacter)
        {
            char quote = quoteCharacter[0];


            var sb = new StringBuilder(s.Length);
            var counter = 0;
            while (counter < s.Length)
            {
                // if starts with delimiter if so read ahead to see if matches
                if (delim[0] == s[counter] &&
                    delim.SequenceEqual(readNext(s, counter, delim.Length)))
                {
                    yield return sb.ToString();
                    sb.Clear();
                    counter = counter + delim.Length; // Move the counter past the delimiter 
                }
                // if we hit a quote read until we hit another quote or end of string
                else if (s[counter] == quote)
                {
                    sb.Append(s[counter++]);
                    while (counter < s.Length && s[counter] != quote)
                    {
                        sb.Append(s[counter++]);
                    }
                    // if not end of string then we hit a quote add the quote
                    if (counter < s.Length)
                    {
                        sb.Append(s[counter++]);
                    }
                }
                else
                {
                    sb.Append(s[counter++]);
                }
            }

            if (sb.Length > 0)
            {
                yield return sb.ToString();
            }
        }

       // (v) added: 22 nov 2019
        public static string removeWhiteSpace(string text)
        {
            var lines = text.Split('\n');
            return string.Join("\n", lines.Select(str => str.Trim()));
        }

        // (v) added: 28 aug 2022
        /// <summary>
        /// keep only one white space between words. 
        /// Method uses regexpressions and maybe is slow
        /// </summary>
        /// <param name="text">the multi-spaced text</param>
        /// <returns>the single spaced text</returns>
        public static string removeMoreThanOneWhiteSpace(string text)
        {
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            return  regex.Replace(text, " ");
        }

        //; (v) added: 24/6/2021

        /// <summary>
        /// Remove quotes, if found, around the string.
        /// </summary>
        /// <param name="text">Text with quotes or without quotes</param>
        /// <returns>Text without quotes</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <see langword="null"/></exception>
        public static string removeQuotesIfAny(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            // Check if there are quotes at both ends and have at least two characters
            if (text.Length > 1 && text[0] == '"' && text[text.Length - 1] == '"')
            {
                // Remove quotes at both ends
                return text.Substring(1, text.Length - 2);
            }

            // If no quotes were found, the text is just returned
            return text;
        }

        /// <summary>
        /// Split a string into a list of strings using a specified character.<br/>
        /// Everything inside quotes are ignored.
        /// </summary>
        /// <param name="input">A string to split</param>
        /// <param name="toSplitAt">The character to use to split with</param>
        /// <returns>A List of strings that was delimited by the <paramref name="toSplitAt"/> character</returns>
        public static List<string> splitStringWithCharNotInsideQuotes(string input, char toSplitAt)
        {
            List<string> elements = new List<string>();

            int lastSplitLocation = 0;
            bool insideQuote = false;

            char[] characters = input.ToCharArray();

            for (int i = 0; i < characters.Length; i++)
            {
                char character = characters[i];
                if (character == '\"')
                    insideQuote = !insideQuote;

                // Only split if we are not inside quotes
                if (character == toSplitAt && !insideQuote)
                {
                    // We need to split
                    int length = i - lastSplitLocation;
                    elements.Add(input.Substring(lastSplitLocation, length));

                    // Update last split location
                    // + 1 so that we do not include the character used to split with next time
                    lastSplitLocation = i + 1;
                }
            }

            // Add the last part
            elements.Add(input.Substring(lastSplitLocation, input.Length - lastSplitLocation));

            return elements;
        }

        // (v) added: 27/8/2023

        /// <summary>
        ///     Remove Characters from string
        /// </summary>
        /// <param name="input">string to remove characters</param>
        /// <param name="chars">array of chars</param>
        /// <returns>System.string</returns>
        public static string removeChars(string input, params char[] chars)
        {
            var sb = new StringBuilder(input.Length);
            foreach (char c in input.Where(c => !chars.Contains(c)))
            {
                sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>
        ///     Replace specified characters with an empty string.
        /// </summary>
        /// <param name="input">the string</param>
        /// <param name="chars">list of characters to replace from the string</param>
        /// <example>
        ///     string s = "Friends";
        ///     s = s.Replace('F', 'r','i','s');  //s becomes 'end;
        /// </example>
        /// <returns>System.string</returns>
        public static string replace(string input, params char[] chars)
        {
            return chars.Aggregate(input, (current, c) => current.Replace(c.ToString(CultureInfo.InvariantCulture), ""));
        }

        /// <summary>
        ///     Appends String quotes for type CSV data
        /// </summary>
        /// <param name="value">val</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string parseStringToAddToCsv(string value)
        {
            if (string.IsNullOrEmpty(value)) value = string.Empty;
            return '"' + value.Replace("\"", "\"\"") + '"';
        }

        /// <summary>
        ///     Removes the first part of the string, if no match found return original string
        /// </summary>
        /// <param name="value">string to remove prefix</param>
        /// <param name="prefix">prefix</param>
        /// <param name="ignoreCase">Indicates whether the compare should ignore case</param>
        /// <returns>trimmed string with no prefix or original string</returns>
        public static string removePrefix(string value, string prefix, bool ignoreCase = true)
        {
            if (!string.IsNullOrEmpty(value) && (ignoreCase ? stringsHelper.startsWithIgnoreCase(value, prefix) : value.StartsWith(prefix)))
            {
                return value.Substring(prefix.Length, value.Length - prefix.Length);
            }
            return value;
        }

        /// <summary>
        ///     Removes the end part of the string, if no match found return original string
        /// </summary>
        /// <param name="value">string to remove suffix</param>
        /// <param name="suffix">suffix</param>
        /// <param name="ignoreCase">Indicates whether the compare should ignore case</param>
        /// <returns>trimmed string with no suffix or original string</returns>
        public static string removeSuffix(string value, string suffix, bool ignoreCase = true)
        {
            if (!string.IsNullOrEmpty(value) && (ignoreCase ? stringsHelper.endsWithIgnoreCase(value, suffix) : value.EndsWith(suffix)))
            {
                return value.Substring(0, value.Length - suffix.Length);
            }
            return null;
        }

        /// <summary>
        ///     Appends the suffix to the end of the string if the string does not already end in the suffix.
        /// </summary>
        /// <param name="value">string to append suffix</param>
        /// <param name="suffix">suffix</param>
        /// <param name="ignoreCase">Indicates whether the compare should ignore case</param>
        /// <returns></returns>
        public static string appendSuffixIfMissing(string value, string suffix, bool ignoreCase = true)
        {
            if (string.IsNullOrEmpty(value) || (ignoreCase ? stringsHelper.endsWithIgnoreCase(value, suffix) : value.EndsWith(suffix)))
            {
                return value;
            }
            return value + suffix;
        }

        /// <summary>
        ///     Appends the prefix to the start of the string if the string does not already start with prefix.
        /// </summary>
        /// <param name="value">string to append prefix</param>
        /// <param name="prefix">prefix</param>
        /// <param name="ignoreCase">Indicates whether the compare should ignore case</param>
        /// <returns></returns>
        public static string appendPrefixIfMissing(string value, string prefix, bool ignoreCase = true)
        {
            if (string.IsNullOrEmpty(value) || (ignoreCase ? stringsHelper.startsWithIgnoreCase(value, prefix) : value.StartsWith(prefix)))
            {
                return value;
            }
            return prefix + value;
        }

        /// <summary>
        ///     ToTextElements
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<string> toTextElements(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("val");
            }
            TextElementEnumerator elementEnumerator = StringInfo.GetTextElementEnumerator(value);
            while (elementEnumerator.MoveNext())
            {
                string textElement = elementEnumerator.GetTextElement();
                yield return textElement;
            }
        }


        /// <summary>
        /// Check if a delimited tag list, contains specific tag
        /// The default list is the comma seperated list.
        /// </summary>
        /// <param name="allTags">The comma seperated tag list</param>
        /// <param name="tagToCheck">The tag to check</param>
        /// <param name="delimiter">Optional,the delimiter. Default is comma</param>
        /// <returns>True if exists</returns>
        public static bool containsTag(string allTags, string tagToCheck, string delimiter = ",")
        {
            if (string.IsNullOrEmpty(allTags)) return false;
            if (string.IsNullOrEmpty(tagToCheck)) return false;
            if (string.IsNullOrEmpty(delimiter)) delimiter=",";
            allTags = delimiter + allTags.Trim().ToUpper() + delimiter;
            tagToCheck = delimiter + tagToCheck.Trim().ToUpper() + delimiter;
            return allTags.Contains(tagToCheck);
        }

        /// <summary>
        /// To array of lines. Each item is one line
        /// </summary>
        /// <param name="input">The input string</param>
        /// <returns>String array, one line per item</returns>
        public static string[] toLinesArray(string input)
        {
            return input.Split(new[] { '\r', '\n' });
        }

    }



}
