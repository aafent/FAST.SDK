using System.Text.RegularExpressions;

namespace FAST.Strings
{

    public class regexValues
    {
        // (v) added: 1 dec 2019
        public const string splitOnCommaKeepDoubleQuotes=
                             @"(?x)   "   +
                            ",          " +   // Split on comma
                            "(?=        " +   // Followed by
                            "  (?:      " +   // Start a non-capture group
                            "    [^\"]* " +   // 0 or more non-quote characters
                            "    \"     " +   // 1 quote
                            "    [^\"]* " +   // 0 or more non-quote characters
                            "    \"     " +   // 1 quote
                            "  )*       " +   // 0 or more repetition of non-capture group (multiple of 2 quotes will be even)
                            "  [^\"]*   " +   // Finally 0 or more non-quotes
                            "  $        " +   // Till the end  (This is necessary, else every comma will satisfy the condition)
                            ")          ";     // End look-ahead

             
        public const string splitOnSpaceKeepDoubleQuotes = "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";

        /// <summary>
        /// The first capturing group is all option. It allows the URL to begin with "http://", "https://", or neither of them. I have a question mark after the s to allow URL's that have http or https. In order to make this entire group optional, I just added a question mark to the end of it.
        /// Next is the domain name: one or more numbers, letters, dots, or hypens followed by another dot then two to six letters or dots. The following section is the optional files and directories. Inside the group, we want to match any number of forward slashes, letters, numbers, underscores, spaces, dots, or hyphens. Then we say that this group can be matched as many times as we want. Pretty much this allows multiple directories to be matched along with a file at the end. I have used the star instead of the question mark because the star says zero or more, not zero or one. If a question mark was to be used there, only one file/directory would be able to be matched.
        /// Then a trailing slash is matched, but it can be optional. Finally we end with the end of the line.
        /// Source: https://code.tutsplus.com/tutorials/8-regular-expressions-you-should-know--net-6149
        /// </summary>
        public const string matchingIP4Address = @"/^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/";  // src: https://code.tutsplus.com/tutorials/8-regular-expressions-you-should-know--net-6149

        /// <summary>
        /// This regex is almost like taking the ending part of the above regex, slapping it between "http://" and some file structure at the end. It sounds a lot simpler than it really is. To start off, we search for the beginning of the line with the caret.
        /// The first capturing group is all option. It allows the URL to begin with "http://", "https://", or neither of them. I have a question mark after the s to allow URL's that have http or https. In order to make this entire group optional, I just added a question mark to the end of it.
        /// Next is the domain name: one or more numbers, letters, dots, or hypens followed by another dot then two to six letters or dots. The following section is the optional files and directories. Inside the group, we want to match any number of forward slashes, letters, numbers, underscores, spaces, dots, or hyphens. Then we say that this group can be matched as many times as we want. Pretty much this allows multiple directories to be matched along with a file at the end. I have used the star instead of the question mark because the star says zero or more, not zero or one. If a question mark was to be used there, only one file/directory would be able to be matched.
        /// Then a trailing slash is matched, but it can be optional. Finally we end with the end of the line.
        /// String that matches: https://net.tutsplus.com/about
        /// String that doesn't match: http://google.com/some/file!.html (contains an exclamation point)
        /// </summary>
        public const string matchingURL = @"/^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$/";  // src: https://code.tutsplus.com/tutorials/8-regular-expressions-you-should-know--net-6149
        
        /// <summary>
        /// We begin by telling the parser to find the beginning of the string (^). Inside the first group, we match one or more lowercase letters, numbers, underscores, dots, or hyphens. I have escaped the dot because a non-escaped dot means any character. Directly after that, there must be an at sign. Next is the domain name which must be: one or more lowercase letters, numbers, underscores, dots, or hyphens. Then another (escaped) dot, with the extension being two to six letters or dots. I have 2 to 6 because of the country specific TLD's (.ny.us or .co.uk). Finally, we want the end of the string ($).
        /// Source: https://code.tutsplus.com/tutorials/8-regular-expressions-you-should-know--net-6149
        /// String that matches: john@doe.com
        /// String that doesn't match: john@doe.something (TLD is too long)
        /// </summary>
        public const string matchingEMail = @"/^([a-z0-9_\.-]+)@([\da-z\.-]+)\.([a-z\.]{2,6})$/";  // src: https://code.tutsplus.com/tutorials/8-regular-expressions-you-should-know--net-6149


        /// <summary>
        /// We begin by telling the parser to find the beginning of the string (^). Next, a number sign is optional because it is followed a question mark. The question mark tells the parser that the preceding character — in this case a number sign — is optional, but to be "greedy" and capture it if it's there. Next, inside the first group (first group of parentheses), we can have two different situations. The first is any lowercase letter between a and f or a number six times. The vertical bar tells us that we can also have three lowercase letters between a and f or numbers instead. Finally, we want the end of the string ($).
        /// The reason that I put the six character before is that parser will capture a hex value like #ffffff. If I had reversed it so that the three characters came first, the parser would only pick up #fff and not the other three f's.
        /// </summary>
        public const string matchingHex = @"/^#?([a-f0-9]{6}|[a-f0-9]{3})$/";  // eg: #a3c113  src: https://code.tutsplus.com/tutorials/8-regular-expressions-you-should-know--net-6149
        
        /// <summary>
        /// First comes the tag's name. It must be one or more letters long. This is the first capture group, it comes in handy when we have to grab the closing tag. The next thing are the tag's attributes. This is any character but a greater than sign (>). Since this is optional, but I want to match more than one character, the star is used. The plus sign makes up the attribute and value, and the star says as many attributes as you want.
        /// Next comes the third non-capture group. Inside, it will contain either a greater than sign, some content, and a closing tag; or some spaces, a forward slash, and a greater than sign. The first option looks for a greater than sign followed by any number of characters, and the closing tag. \1 is used which represents the content that was captured in the first capturing group. In this case it was the tag's name. Now, if that couldn't be matched we want to look for a self closing tag (like an img, br, or hr tag). This needs to have one or more spaces followed by "/>".
        /// The regex is ended with the end of the line.
        /// Source: https://code.tutsplus.com/tutorials/8-regular-expressions-you-should-know--net-6149
        /// </summary>
        public const string matchingHTMLTag = @"/^<([a-z]+)([^<]+)*(?:>(.*)<\/\1>|\s+\/>)$/"; 

        /// <summary>
        /// Description:
        ///  You will be using this regex if you ever have to work with mod_rewrite and pretty URL's. We begin by telling the parser to find the beginning of the string (^), followed by one or more (the plus sign) letters, numbers, or hyphens. Finally, we want the end of the string ($).
        ///  String that matches: my-title-here
        ///  String that doesn't match: my_title_here (contains underscores)
        /// </summary>
        public const string matchingSlug = @"/^[a-z0-9-]+$/";


        // (v) added 22/6/2021
        public const string lowerCase = "(?=.*[a-z])";
        public const string upperCase = "(?=.*[A-Z])";
        public const string digits = "(?=.*\\d)";
        public const string symbols = "(?=.*[~`!@#$%^&*()-_=+{[}|]:\";'<>?,./\\])"; // Wrong!
        public const string noWhiteSpace = "[^\r\n\t\f ]"; // Dodgy!
        public const string passwordPolicy1 = "^" + lowerCase + upperCase + digits + symbols + noWhiteSpace + "{8,16}$";


        // (v) added 2/8/2023

        /// <summary>
        /// collection of email matching patterns
        /// </summary>
        public static readonly string[] matchingEmailPatterns = {
            matchingEMail,
            "^[a-zA-Z][\\w\\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\\w\\.-]*[a-zA-Z0-9]\\.[a-zA-Z][a-zA-Z\\.]*[a-zA-Z]$"
        };


        /// <summary>
        /// collection of IPv4 matching patterns
        /// </summary>
        public static readonly string[] matchingIPv4Patterns = {
            matchingIP4Address,
            @"(?:^|\s)([a-z]{3,6}(?=://))?(://)?((?:25[0-5]|2[0-4]\d|[01]?\d\d?)\.(?:25[0-5]|2[0-4]\d|[01]?\d\d?)\.(?:25[0-5]|2[0-4]\d|[01]?\d\d?)\.(?:25[0-5]|2[0-4]\d|[01]?\d\d?))(?::(\d{2,5}))?(?:\s|$)"
        };

        [Obsolete("Use the method: stringsHelperRegex.split()")]
        public static string[] split(string expression, string input)
        {
            return Regex.Split(input, expression);
        }

    }
}
