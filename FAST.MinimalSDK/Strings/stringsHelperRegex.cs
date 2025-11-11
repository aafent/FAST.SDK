using System.Text.RegularExpressions;

namespace FAST.Strings
{
    /// <summary>
    /// helper with string functions using RegEx engine
    /// </summary>
    public static class stringsHelperRegex
    {
        /// <summary>
        ///     Count number of occurrences in string
        /// </summary>
        /// <param name="val">string containing text</param>
        /// <param name="stringToMatch">string or pattern find</param>
        /// <returns></returns>
        public static int countOccurrences(string value, string stringToMatch)
        {
            return Regex.Matches(value, stringToMatch, RegexOptions.IgnoreCase).Count;
        }

        /// <summary>
        /// An implementation of string split using regex expression
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="input"></param>
        /// <returns>string array</returns>
        public static string[] split(string expression, string input)
        {
            return Regex.Split(input, expression);
        }

        /// <summary>
        ///     Validate any matching patter
        ///     regexValues static class contains many out of the box patters
        ///     eg: matchingIP4Address, matchingEMail etc
        /// </summary>
        /// <param name="value">string value</param>
        /// <returns>true or false if matching the pattern if valid</returns>
        public static bool isMatching(string value, string pattern)
        {
            return Regex.Match(value, pattern).Success;
        }

        /// <summary>
        ///     Replace Line Feeds
        /// </summary>
        /// <param name="value">string to remove line feeds</param>
        /// <returns>System.string</returns>
        public static string removeLineFeeds(string value)
        {
            return Regex.Replace(value, @"^[\r\n]+|\.|[\r\n]+$", "");
        }



    }
}
