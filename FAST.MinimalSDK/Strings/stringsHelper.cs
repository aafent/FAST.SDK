using System.Globalization;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using FAST.Core;

namespace FAST.Strings
{

    /// <summary>
    /// helper with string functions
    /// </summary>
    public class stringsHelper
    {
        [Obsolete("use: stringValue.latinCapitalAlphabet26Letters")]
        public static string latinAlphabet // future: remove, as moved to stringValue
        {
            get
            {
                return stringValue.latinCapitalAlphabet26Letters;
            }
        }

        [Obsolete("use: stringValue.numericOnlyDigits")]
        public static string numericDigits // future: remove, as moved to stringValue
        {
            get
            {
                return  stringValue.numericOnlyDigits;
            }
        }

        [Obsolete("use: stringValue.whiteSpace")]
        public static string whiteSpace  // future: remove, as moved to stringValue
        { 
            get 
            { 
                return " "; 
            } 
        }

        [Obsolete("use: stringValue.doubleWhiteSpace")]
        public static string doubleWhiteSpace // future: remove, as moved to stringValue
        {
            get
            {
                return whiteSpace+whiteSpace;
            }
        }

        /// <summary>
        ///     Reverse string
        /// </summary>
        /// <param name="input">string to reverse</param>
        /// <returns>System.string</returns>
        public static string reverse(string input)
        {
            if ( string.IsNullOrEmpty(input) ) return input;
            char[] arr = input.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }


        /// <summary>
        ///     Gets empty String if passed value of type object is of type Null/Nothing
        /// </summary>
        /// <param name="input">val</param>
        /// <returns>System.String</returns>
        /// <remarks></remarks>
        public static string mapNull(object input)
        {
            if (input == null)
            {
                return String.Empty;
            }
            else
            {
                return mapNull(input.ToString());
            }
        }
        /// <summary>
        ///     Gets empty String if passed string value  is of type Null/Nothing
        /// </summary>
        /// <param name="input">val</param>
        /// <returns>System.String</returns>
        /// <remarks></remarks>
        public static string mapNull(string input) { return input == null ? string.Empty : input; }
        /// <summary>
        ///  Checks if a string is null or empty and returns String if not Empty else returns the emptyValue
        /// </summary>
        /// <param name="input">the input string</param>
        /// <param name="emptyValue">value to return if input is empty or null</param>
        /// <returns>string</returns>
        public static string mapNullOrEmpty(string input, string emptyValue="")
        {
            if (input == null) { return emptyValue; }
            input = input.Trim();
            if (string.IsNullOrEmpty(input)) { return emptyValue; }
            return input;
        }

        public static string zip(string value)
        {
            //Transform string into byte[]  
            byte[] byteArray = new byte[value.Length];
            int indexBA = 0;
            foreach (char item in value.ToCharArray())
            {
                byteArray[indexBA++] = (byte)item;
            }

            return zip(byteArray);
        }
        public static string zip(byte[] byteArray)
        {
            //Prepare for compress
            using ( System.IO.MemoryStream ms = new System.IO.MemoryStream() )
            using (System.IO.Compression.GZipStream sw = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress))
            {
                //Compress
                sw.Write(byteArray, 0, byteArray.Length);
                //Close, DO NOT FLUSH cause bytes will go missing...
                sw.Close();

                //Transform byte[] zip data to string
                byteArray = ms.ToArray();
                System.Text.StringBuilder sB = new System.Text.StringBuilder(byteArray.Length);
                foreach (byte item in byteArray)
                {
                    sB.Append((char)item);
                }
                ms.Close();
                sw.Dispose();
                ms.Dispose();
                return sB.ToString();
            }
        }

        public static string unZip(string value)
        {
            //Transform string into byte[]
            byte[] byteArray = new byte[value.Length];
            int indexBA = 0;
            foreach (char item in value.ToCharArray())
            {
                byteArray[indexBA++] = (byte)item;
            }
            return unZip(byteArray);
        }
        public static string unZip(byte[] bytes) 
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream()) 
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress)) 
                {
                    streamHelper.streamToStream(gs,mso, false);
                    // (^>)replace: fileHelpers.copyStream(gs, mso);
                }
                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }


        // (v) unZip() replaced on 27 nov 2019 due a bug related with the input string's length
/*
        public static string unZip(string value)
        {
            //Transform string into byte[]
            byte[] byteArray = new byte[value.Length];
            int indexBA = 0;
            foreach (char item in value.ToCharArray())
            {
                byteArray[indexBA++] = (byte)item;
            }

            //Prepare for decompress
            using( System.IO.MemoryStream ms = new System.IO.MemoryStream(byteArray) )
            using (System.IO.Compression.GZipStream sr = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress))
            {
                //Reset variable to collect uncompressed result
                byteArray = new byte[byteArray.Length];

                //Decompress
                int rByte = sr.Read(byteArray, 0, byteArray.Length);

                //Transform byte[] unzip data to string
                System.Text.StringBuilder sB = new System.Text.StringBuilder(rByte);
                //Read the number of bytes GZipStream red and do not a for each bytes in
                //resultByteArray;
                for (int i = 0; i < rByte; i++)
                {
                    sB.Append((char)byteArray[i]);
                }
                sr.Close();
                ms.Close();
                sr.Dispose();
                ms.Dispose();
                return sB.ToString();
            }
        }
*/
        public static string doubleQuote(string value)
        {
            return @"""" + value.Trim('"') + @"""";
        }

        public static string toBooleanString(string value, bool defaultValue )
        {
            value = value.Trim().ToUpper();
            switch (value)
            {
                case "T":
                case "YES":
                case "TRUE":
                case "ON":
                    value = "TRUE";
                    break;

                case "F":
                case "NO":
                case "FALSE":
                case "OFF":
                    value = "FALSE";
                    break;
                default:
                    value = defaultValue.ToString().ToUpper();
                    break;
            }
            return value;

        }

        public static string charMap(string charSetFrom, string charSetTo, string stringToMap)
        {
            if (string.IsNullOrEmpty(stringToMap)) { return stringToMap; }
            string result = "";

            for (int index = 0; index < stringToMap.Length; index++)
            {
                char c = stringToMap[index];
                int p1 = charSetFrom.IndexOf(c);
                if (p1 >= 0) { c = charSetTo[p1]; }
                result += c;
            }
            return result;
        }

        public static string toGreeklish(string greek)
        {
            string greekAB = "ΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩαβγδεζηθικλμνξοπρσςτυφχψωάέόίήύώϊ";
            string latinAB = "ABGDEZH8IKLMNJOPRSTYFXCWabgdezh8iklmnjoprsstufxcwaeoihuwi";
            return charMap(greekAB, latinAB, greek);
        }

        public static string toGreekUpperWithoutAccesnts(string greek)
        {
            string greekAB      = "ΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩαβγδεζηθικλμνξοπρσςτυφχψωάέόίήύώϊ";
            string greekUpper   = "ΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΣΤΥΦΧΨΩΑΕΟΙΗΥΩΙ";
            return charMap(greekAB, greekUpper, greek);
            
        }

        public static string toGreekCommonPhonetic(string greek, bool returnAsGreeklish )
        {
            if (string.IsNullOrEmpty(greek)) { return greek; }

            greek = toGreekUpperWithoutAccesnts(greek);
            greek = greek.Replace("ΑΙ","ε");
            greek = greek.Replace("ΟΙ", "ι");
            greek = greek.Replace("ΥΙ", "ι");
            greek = greek.Replace("ΕΙ", "ι");
            greek = greek.Replace("ΕΥ", "Β");
            greek = greek.Replace("ΓΓ", "ΓΚ");
            greek = greek.Replace("ΛΛ", "Λ");
            greek = greek.Replace("ΝΝ", "Ν");
            greek = greek.Replace("ΚΚ", "Κ");
            greek = greek.Replace("ΜΜ", "Μ");
            greek = greek.Replace("ΣΣ", "Σ");
            greek = greek.Replace("ΒΒ", "Β");

            string greekAB =    "ΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩαβγδεζηθικλμνξοπρσςτυφχψωάέόίήύώϊ";
            string phoneticAB = "ΑΒΓΔΕΖΙΘΙΚΛΜΝΞΟΠΡΣΤΙΦΧΨΟΑΒΓΔΕΖΙΘΙΚΛΜΝΞΟΠΡΣΣΤΙΦΧΨΟΑΕΟΙΙΙΟΙ";
            greek = charMap(greekAB, phoneticAB, greek);

            if (returnAsGreeklish)
            {
                return toGreeklish(greek);
            }
            else
            {
                return greek;
            }
        }

        public static int toInt(string value, int valueForNullOrEmpty = 0)
        {
            if (string.IsNullOrEmpty(value)) { value = ""; }
            value = value.Trim();
            if (string.IsNullOrEmpty(value))
            {
                return valueForNullOrEmpty;
            }
            int intValue;
            if (!int.TryParse(value, out intValue)) { return valueForNullOrEmpty; }
            return intValue;
        }

        public static string addTo(string input, string delimiter, string whatToAdd, params object[] formatArgs )
        {
            if (string.IsNullOrEmpty(input)) 
            { 
                input = string.Empty; 
            }
            else
            {
                input += delimiter;
            }
            if (formatArgs == null || formatArgs.Length==0 )
                input += whatToAdd;
            else
                input += string.Format(whatToAdd, formatArgs);

            return input;
        }
        public static string addLine(string input, string whatToAdd, params object[] formatArgs)
        {
            return addTo(input, Environment.NewLine, whatToAdd, formatArgs);
        }


        public static int indexOfNth(string input, string value, int nth = 1)
        {
            if (nth <= 0) throw new ArgumentException("Can not find the zeroth index of substring in string. Must start with 1");
            int offset = input.IndexOf(value);
            for (int i = 1; i < nth; i++)
            {
                if (offset == -1) return -1;
                offset = input.IndexOf(value, offset + 1);
            }
            return offset;
        }

        /// <summary>
        /// Check if a string start's with any string item of a collection
        /// </summary>
        /// <param name="input">The string to check</param>
        /// <param name="list">The collection of string items</param>
        /// <returns>True, if any collection item if the start of the input string</returns>
        public bool startWithAnyOf(string input, IEnumerable<string> list)
        {
            if (input == null)
            {
                return false;
            }

            return list.Any(f => input.StartsWith(f));
        }


        /// <summary>
        /// Replace multiple old values with a new value
        /// </summary>
        /// <param name="input">The string to perform the replaces</param>
        /// <param name="oldValues">A collection of the old values</param>
        /// <param name="newValue">The new value</param>
        /// <returns>string, the replaced string</returns>
        public  string replaceMultiple(string input, string[] oldValues, string newValue)
        {
            foreach (string value in oldValues)
            {
                input = input.Replace(value, newValue);
            }
            return input;
        }



        public static string beautifyXML(string xml)
        {
            string result = "";

            using (MemoryStream mStream = new MemoryStream()) 
            using (XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode))
            {
                XmlDocument document = new XmlDocument();

                try
                {
                    // Load the XmlDocument with the XML.
                    document.LoadXml(xml);

                    writer.Formatting = Formatting.Indented;

                    // Write the XML into a formatting XmlTextWriter
                    document.WriteContentTo(writer);
                    writer.Flush();
                    mStream.Flush();

                    // Have to rewind the MemoryStream in order to read
                    // its contents.
                    mStream.Position = 0;

                    // Read MemoryStream contents into a StreamReader.
                    StreamReader sReader = new StreamReader(mStream);

                    // Extract the text from the StreamReader.
                    string formattedXml = sReader.ReadToEnd();

                    result = formattedXml;
                }
                catch (XmlException ex)
                {
                    throw new Exception("Cannot beautify XML. Check the inner exception for further details",ex);
                }

                mStream.Close();
                writer.Close();
            }

            return result;
        }

        public static string loremText()
        {
            return @"
Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Sit amet est placerat in egestas erat. Malesuada fames ac turpis egestas integer eget aliquet nibh. Sit amet nisl purus in mollis. Tristique et egestas quis ipsum suspendisse ultrices gravida dictum fusce. Morbi enim nunc faucibus a pellentesque sit amet porttitor eget. Proin sed libero enim sed faucibus turpis in eu mi. Neque viverra justo nec ultrices dui sapien eget. Faucibus ornare suspendisse sed nisi lacus. Ullamcorper morbi tincidunt ornare massa eget egestas.
Laoreet non curabitur gravida arcu. Libero justo laoreet sit amet cursus sit. In ante metus dictum at. Ullamcorper morbi tincidunt ornare massa eget egestas purus. Mauris cursus mattis molestie a iaculis at erat. Cursus eget nunc scelerisque viverra mauris in. Varius quam quisque id diam. Sed tempus urna et pharetra pharetra massa massa ultricies. Netus et malesuada fames ac turpis egestas. Condimentum vitae sapien pellentesque habitant.
Eget duis at tellus at. Nunc vel risus commodo viverra maecenas accumsan lacus. Dapibus ultrices in iaculis nunc sed. Libero id faucibus nisl tincidunt eget. Sed felis eget velit aliquet sagittis id consectetur purus. Et leo duis ut diam. Diam maecenas sed enim ut sem. Gravida arcu ac tortor dignissim convallis. Ornare suspendisse sed nisi lacus. Magnis dis parturient montes nascetur ridiculus. Faucibus interdum posuere lorem ipsum dolor sit amet consectetur. Dui sapien eget mi proin sed. Porttitor massa id neque aliquam vestibulum. Adipiscing elit duis tristique sollicitudin nibh sit. Lobortis scelerisque fermentum dui faucibus. Vulputate odio ut enim blandit volutpat maecenas volutpat blandit aliquam. Egestas dui id ornare arcu odio ut sem. Eget felis eget nunc lobortis mattis aliquam faucibus purus. Suscipit tellus mauris a diam maecenas.

Nulla aliquet porttitor lacus luctus accumsan tortor posuere. Platea dictumst quisque sagittis purus sit amet volutpat consequat mauris. Fames ac turpis egestas maecenas. Aenean vel elit scelerisque mauris. Tristique senectus et netus et malesuada fames ac turpis egestas. Diam maecenas sed enim ut. Lacus luctus accumsan tortor posuere ac ut consequat semper. Interdum consectetur libero id faucibus nisl tincidunt eget nullam. Accumsan sit amet nulla facilisi morbi tempus iaculis urna. Enim blandit volutpat maecenas volutpat blandit aliquam etiam erat. Consequat interdum varius sit amet mattis vulputate. Non tellus orci ac auctor augue. Ac orci phasellus egestas tellus.
Tincidunt tortor aliquam nulla facilisi cras fermentum. Nulla facilisi morbi tempus iaculis urna id. Nunc mattis enim ut tellus elementum sagittis vitae et. Purus faucibus ornare suspendisse sed nisi lacus sed viverra. Condimentum lacinia quis vel eros donec ac odio tempor orci. Amet mauris commodo quis imperdiet massa tincidunt. Feugiat vivamus at augue eget arcu dictum. Eu tincidunt tortor aliquam nulla facilisi cras fermentum. Phasellus faucibus scelerisque eleifend donec. Vestibulum rhoncus est pellentesque elit. Gravida dictum fusce ut placerat orci nulla pellentesque dignissim enim. Maecenas ultricies mi eget mauris. Est sit amet facilisis magna etiam tempor orci eu. Cras tincidunt lobortis feugiat vivamus. Iaculis nunc sed augue lacus viverra vitae congue eu. Pulvinar etiam non quam lacus suspendisse faucibus interdum posuere lorem. Dignissim sodales ut eu sem integer vitae. Et molestie ac feugiat sed lectus vestibulum. Mus mauris vitae ultricies leo integer malesuada nunc vel.
";
        }

        /// <summary>
        /// this is to emulate what's evailable in PHP
        /// </summary>
        public static string repeatString(string text, int count)
        {
            var result = new StringBuilder(text.Length * count);
            for (int tms = 0; tms < count; tms++) result.Append(text);
            return result.ToString();
        }

        // (v) 27/8/2023 addtions

        /// <summary>
        ///     Checks if date with dateFormat is parse-able to System.DateTime format returns boolean value if true else false
        /// </summary>
        /// <param name="value">String date</param>
        /// <param name="dateFormat">date format example dd/MM/yyyy HH:mm:ss</param>
        /// <returns>boolean True False if is valid System.DateTime</returns>
        public static bool isDateTime(string value, string dateFormat)
        {
            // ReSharper disable once RedundantAssignment
            DateTime dateVal = default(DateTime);
            return DateTime.TryParseExact(value, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out dateVal);
        }

        /// <summary>
        ///     Returns an enumerable collection of the specified type containing the substrings in this instance that are
        ///     delimited by elements of a specified Char array
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="separator">
        ///     An array of Unicode characters that delimit the substrings in this instance, an empty array containing no
        ///     delimiters, or null.
        /// </param>
        /// <typeparam name="T">
        ///     The type of the element to return in the collection, this type must implement IConvertible.
        /// </typeparam>
        /// <returns>
        ///     An enumerable collection whose elements contain the substrings in this instance that are delimited by one or more
        ///     characters in separator.
        /// </returns>
        public static IEnumerable<T> splitTo<T>(string value, params char[] separator) where T : IConvertible
        {
            return value.Split(separator, StringSplitOptions.None).Select(s => (T)Convert.ChangeType(s, typeof(T)));
        }

        /// <summary>
        ///     Returns an enumerable collection of the specified type containing the substrings in this instance that are
        ///     delimited by elements of a specified Char array
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="options">StringSplitOptions <see cref="StringSplitOptions" /></param>
        /// <param name="separator">
        ///     An array of Unicode characters that delimit the substrings in this instance, an empty array containing no
        ///     delimiters, or null.
        /// </param>
        /// <typeparam name="T">
        ///     The type of the element to return in the collection, this type must implement IConvertible.
        /// </typeparam>
        /// <returns>
        ///     An enumerable collection whose elements contain the substrings in this instance that are delimited by one or more
        ///     characters in separator.
        /// </returns>
        public static IEnumerable<T> splitTo<T>(string value, StringSplitOptions options, params char[] separator)
            where T : IConvertible
        {
            return value.Split(separator, options).Select(s => (T)Convert.ChangeType(s, typeof(T)));
        }

        /// <summary>
        ///     Converts string to its Enum type
        ///     Checks of string is a member of type T enum before converting
        ///     if fails returns default enum
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="value"> The string representation of the enumeration name or underlying value to convert</param>
        /// <param name="defaultValue"></param>
        /// <returns>Enum object</returns>
        /// <remarks>
        ///     <exception cref="ArgumentException">
        ///         enumType is not an System.Enum.-or- value is either an empty string ("") or
        ///         only contains white space.-or- value is a name, but not one of the named constants defined for the enumeration
        ///     </exception>
        /// </remarks>
        public static T toEnum<T>(string value, T defaultValue = default(T)) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("Type T Must of type System.Enum");
            }

            T result;
            bool isParsed = Enum.TryParse(value, true, out result);
            return isParsed ? result : defaultValue;
        }


        /// <summary>
        ///  Function checks if a string is a valid int32 value
        /// </summary>
        /// <param name="value">val</param>
        /// <returns>Boolean True if is Integer else False</returns>
        public static bool isInteger(string value)
        {
            // Variable to collect the Return value of the TryParse method.

            // Define variable to collect out parameter of the TryParse method. If the conversion fails, the out parameter is zero.
            int retNum;

            // The TryParse method converts a string in a specified style and culture-specific format to its double-precision floating point number equivalent.
            // The TryParse method does not generate an exception if the conversion fails. If the conversion passes, True is returned. If it does not, False is returned.
            bool isNum = Int32.TryParse(value, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        /// <summary>
        ///     Read in a sequence of words from standard input and capitalize each
        ///     one (make first letter uppercase; make rest lowercase).
        /// </summary>
        /// <param name="input">string</param>
        /// <returns>Word with capitalization</returns>
        public static string capitalize(string input)
        {
            if (input.Length == 0)
            {
                return input;
            }
            return input.Substring(0, 1).ToUpper() + input.Substring(1).ToLower();
        }

        /// <summary>
        ///     Gets first character in string
        /// </summary>
        /// <param name="value">val</param>
        /// <returns>System.string</returns>
        public static string firstCharacter(string value)
        {
            return (!string.IsNullOrEmpty(value))
                ? (value.Length >= 1)
                    ? value.Substring(0, 1)
                    : value
                : null;
        }

        /// <summary>
        ///     Gets last character in string
        /// </summary>
        /// <param name="value">val</param>
        /// <returns>System.string</returns>
        public static string lastCharacter(string value)
        {
            return (!string.IsNullOrEmpty(value))
                ? (value.Length >= 1)
                    ? value.Substring(value.Length - 1, 1)
                    : value
                : null;
        }

        /// <summary>
        ///     Check a String ends with another string ignoring the case.
        /// </summary>
        /// <param name="value">string</param>
        /// <param name="suffix">suffix</param>
        /// <returns>true or false</returns>
        public static bool endsWithIgnoreCase(string value, string suffix)
        {
            if (value == null)
            {
                throw new ArgumentNullException("val", "val parameter is null");
            }
            if (suffix == null)
            {
                throw new ArgumentNullException("suffix", "suffix parameter is null");
            }
            if (value.Length < suffix.Length)
            {
                return false;
            }
            return value.EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        ///     Check a String starts with another string ignoring the case.
        /// </summary>
        /// <param name="value">string</param>
        /// <param name="prefix">prefix</param>
        /// <returns>true or false</returns>
        public static bool startsWithIgnoreCase(string value, string prefix)
        {
            if (value == null)
            {
                throw new ArgumentNullException("val", "val parameter is null");
            }
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix", "prefix parameter is null");
            }
            if (value.Length < prefix.Length)
            {
                return false;
            }
            return value.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase);
        }


        /// <summary>
        ///     Extracts the left part of the input string limited with the length parameter
        /// </summary>
        /// <param name="value">The input string to take the left part from</param>
        /// <param name="length">The total number characters to take from the input string</param>
        /// <returns>The substring starting at startIndex 0 until length</returns>
        /// <exception cref="System.ArgumentNullException">input is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Length is smaller than zero or higher than the length of input</exception>
        public static string left(string value, int length)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }
            if (length < 0 || length > value.Length)
            {
                throw new ArgumentOutOfRangeException("length",
                    "length cannot be higher than total string length or less than 0");
            }
            return value.Substring(0, length);
        }

        /// <summary>
        ///     Extracts the right part of the input string limited with the length parameter
        /// </summary>
        /// <param name="value">The input string to take the right part from</param>
        /// <param name="length">The total number characters to take from the input string</param>
        /// <returns>The substring taken from the input string</returns>
        /// <exception cref="System.ArgumentNullException">input is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Length is smaller than zero or higher than the length of input</exception>
        public static string right(string value, int length)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }
            if (length < 0 || length > value.Length)
            {
                throw new ArgumentOutOfRangeException("length",
                    "length cannot be higher than total string length or less than 0");
            }
            return value.Substring(value.Length - length);
        }


        /// <summary>
        ///     Truncate String and append (optional) a value at the end (eg "...")
        /// </summary>
        /// <param name="input">String to be truncated</param>
        /// <param name="maxLength">number of chars to truncate</param>
        /// <param name="valueToAppendIfTruncate">if truncate append this value (eg "...")</param>
        /// <returns>string</returns>
        /// <remarks></remarks>
        public static string truncate(string input, int maxLength, string valueToAppendIfTruncate = null)
        {
            if (String.IsNullOrEmpty(input) || maxLength <= 0)
            {
                return String.Empty;
            }
            if (input.Length > maxLength)
            {
                if (string.IsNullOrEmpty(valueToAppendIfTruncate))
                    return input.Substring(0, maxLength) + valueToAppendIfTruncate;
                else
                    return input.Substring(0, maxLength) + "...";
            }
            return input;
        }

        /// <summary>
        ///     Check if a string does not start with prefix
        /// </summary>
        /// <param name="value">string to evaluate</param>
        /// <param name="prefix">prefix</param>
        /// <returns>true if string does not match prefix else false, null values will always evaluate to false</returns>
        public static bool doesNotStartWith(string value, string prefix)
        {
            return value == null || prefix == null ||
                   !value.StartsWith(prefix, StringComparison.InvariantCulture);
        }

        /// <summary>
        ///     Check if a string does not end with prefix
        /// </summary>
        /// <param name="value">string to evaluate</param>
        /// <param name="suffix">suffix</param>
        /// <returns>true if string does not match prefix else false, null values will always evaluate to false</returns>
        public static bool doesNotEndWith(string value, string suffix)
        {
            return value == null || suffix == null ||
                   !value.EndsWith(suffix, StringComparison.InvariantCulture);
        }

        // (v) additions 11/9/2023


        /// <summary>
        /// Gets the last.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="tailLength">Length of the tail.</param>
        /// <returns>System.String.</returns>
        public static string getLast(string source, int tailLength)
        {
            if (tailLength >= source.Length)
                return source;
            return source.Substring(source.Length - tailLength);
        }

        /// <summary>
        /// Remove all HTML tags from a string
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public static string stripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        /// <summary>
        /// Takes a substring between two anchor strings (or the end of the string if that anchor is null)
        /// </summary>
        /// <param name="this">a string</param>
        /// <param name="from">an optional string to search after</param>
        /// <param name="until">an optional string to search before</param>
        /// <param name="comparison">an optional comparison for the search</param>
        /// <returns>a substring based on the search</returns>
        /// <exception cref="ArgumentException">from: Failed to find an instance of the first anchor</exception>
        public static string substringBetween(string @this, string from = null, string until = null,
                                              StringComparison comparison = StringComparison.InvariantCulture)
        {
            var fromLength = (from ?? string.Empty).Length;
            var startIndex = !string.IsNullOrEmpty(from) ? @this.IndexOf(from, comparison) + fromLength : 0;

            if (startIndex < fromLength)
            {
                throw new ArgumentException("from: Failed to find an instance of the first anchor");
            }

            var endIndex = !string.IsNullOrEmpty(until) ? @this.IndexOf(until, startIndex, comparison) : @this.Length;

            if (endIndex < 0)
            {
                endIndex = @this.Length;
            }

            var subString = @this.Substring(startIndex, endIndex - startIndex);
            return subString;
        }

        /// <summary>
        /// Truncate a string after maxLength characters.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns>A string truncates after maxLength characters. If the string length is less the maxLength, return the string</returns>
        public static string truncateString(string str, int maxLength)
        {
            return new string(str.Take(maxLength).ToArray());
        }

        /// <summary>
        /// Truncate a string after maxLength characters.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <param name="addFullStop">if set to <c>true</c> [add full stop].</param>
        /// <returns>A string truncates after maxLength characters. If the string length is less the maxLength, return the string</returns>
        public static string truncateString(string str, int maxLength, bool addFullStop = false)
        {
            string rtn = "";

            if (!string.IsNullOrEmpty(str))
            {
                if (str.Length > maxLength)
                {
                    rtn = new string(str.ToCharArray(0, maxLength));
                    if (addFullStop)
                    {
                        rtn += "...";
                    }
                }
                else
                {
                    rtn = str;
                }
            }

            return rtn;
        }

        /// <summary>
        /// Returns a random string with random alphanumeric characters
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="length">String length</param>
        /// <returns>A string with random alphanumeric characters</returns>
        public static string randomString(string str, int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Check if a string is Palindrome (left to right same as right to left)
        /// </summary>
        /// <param name="input">input string to check</param>
        /// <returns>Boolean, true if it is palidrome string</returns>
        public bool isPalindrome(string input)
        {
            return input.SequenceEqual(input.Reverse());
        }

        /// <summary>
        /// Split a string into chunks 
        /// </summary>
        /// <param name="str">The input (big) string</param>
        /// <param name="chunkSize">The size of each chunk</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static IEnumerable<string> splitIntoChunks(string str, int chunkSize)
        {
            if (string.IsNullOrEmpty(str))
            {
                yield break; // Or throw an exception if you prefer
            }

            if (chunkSize <= 0)
            {
                throw new ArgumentOutOfRangeException("Chunk size must be greater than zero.");
            }

            for (int i = 0; i < str.Length; i += chunkSize)
            {
                int currentChunkSize = Math.Min(chunkSize, str.Length - i); // Handle last chunk
                yield return str.Substring(i, currentChunkSize);
            }
        }

        /// <summary>
        /// Foreach Chunk of chunkSize to the anonyomus function codeToDo
        /// </summary>
        /// <param name="str">The input (big) string</param>
        /// <param name="chunkSize">The size of each chunk</param>
        /// <param name="codeToDo">The anonynous function with one string argument as input where is the chunk</param>\
        /// <example>stringsHelper.foreachChunks(content,maxLen, (msg) =>{ fastLogger.debug(title + "\t" + msg); } );</example>
        public static void foreachChunk(string str, int chunkSize, Action<string> codeToDo )
        {
            var allChuncks = splitIntoChunks(str,chunkSize);
            foreach(var value in allChuncks)
            {
                codeToDo(value);
            }
            return;
        }

    }




    //public class fcStrings
    //{
    //    private string _getListBuffer = "";
    //    private string _setListBuffer = "";

    //    public string nextToken(string delimiter)
    //    {
    //        return nextToken(ref _getListBuffer, delimiter);
    //    }
    //    public string token(string stringList, string delimiter, int numberOfToken)
    //    {
    //        string tokenToReturn = "";
    //        for (int interation = 1; interation <= numberOfToken; interation++)
    //        {
    //            tokenToReturn = nextToken(ref stringList, delimiter);
    //        }
    //        return tokenToReturn;
    //    }
    //    public void setStringList(string stringList)
    //    {
    //        _getListBuffer = stringList;
    //        return;
    //    }
    //    public void reset()
    //    {
    //        _setListBuffer = "";
    //        return;
    //    }
    //    public string add(string stringToAddToList, string delimiter)
    //    {
    //        if (_setListBuffer != "") { _setListBuffer += delimiter; }
    //        _setListBuffer += stringToAddToList;

    //        return _setListBuffer;
    //    }
    //    // (v) todo: to rename or unify both methods
    //    public string getStringList()
    //    {
    //        return _setListBuffer;
    //    }
    //    public string getRemainingStringList()
    //    {
    //        return _getListBuffer;
    //    }
    //    // (^) 
    //    public bool eol
    //    {
    //        get
    //        {
    //            if (_getListBuffer == "") { return true; }
    //            else { return false; }
    //        }
    //    }
    //    public bool containsInList(string stringList, string delimiter, string valueToCheck)
    //    {
    //        stringList = delimiter + stringList.ToLower().Trim() + delimiter;
    //        valueToCheck = delimiter + valueToCheck.ToLower().Trim() + delimiter;
    //        return stringList.Contains(valueToCheck);
    //    }

    //    public string join(string[] tokenToJoin, string delimeter)
    //    {
    //        string jointTokens = "";
    //        for (int item = 0; item <= tokenToJoin.GetUpperBound(0); item++)
    //        {
    //            if (jointTokens != "") { jointTokens += delimeter; }
    //            jointTokens += tokenToJoin[item];
    //        }
    //        return jointTokens;
    //    }

    //    public string uriToString(string url, string fileNameToStore = "")
    //    {
    //        return uriToString(new Uri(url), fileNameToStore);
    //    }
    //    public string uriToString(Uri uri, string fileNameToStore = "")
    //    {
    //        if (string.IsNullOrEmpty(fileNameToStore))
    //        {
    //            fileNameToStore = System.IO.Path.GetTempFileName();
    //        }
    //        new WebClient().DownloadFile(uri, fileNameToStore);
    //        return new fcStrings().fileToString(fileNameToStore, false);
    //    }

    //    public static IEnumerable<string> subStrings(string input, string startToken, string endToken)
    //    {
    //        Regex r = new Regex(Regex.Escape(startToken) + "(.*?)" + Regex.Escape(endToken));
    //        MatchCollection matches = r.Matches(input);
    //        foreach (Match match in matches)
    //            yield return match.Groups[1].Value;
    //    }

    //}


}
