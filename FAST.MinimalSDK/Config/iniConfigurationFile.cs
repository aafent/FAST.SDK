using FAST.Core;
using FAST.Strings;

namespace FAST.Config
{
    /// <summary>
    /// A class for reading values by section and key from a standard ".ini" initialization file.
    /// </summary>
    /// <remarks>
    /// Section and key names are not case-sensitive. Values are loaded into a hash table for fast access.
    /// Use <see cref="getAllValues"/> to read multiple values that share the same section and key.
    /// Sections in the initialization file must have the following form:
    /// <code>
    ///     ; comment line
    ///     [section]
    ///     key=value
    /// </code>
    /// </remarks>
    public class iniConfigurationFile : IvariablesContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="iniConfigurationFile"/> class.
        /// </summary>
        /// <param name="file">The initialization file path.</param>
        /// <param name="commentDelimiter">The comment delimiter string (default value is ";").
        /// </param>
        public iniConfigurationFile(string file, string lineCommentCharacter = stringValue.semicolon )
        {
            this.lineCommentCharacter = lineCommentCharacter;
            iniFilePath = file;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="iniConfigurationFile"/> class.
        /// </summary>
        public iniConfigurationFile()
        {
            lineCommentCharacter = stringValue.semicolon;
        }

        /// <summary>
        /// The comment delimiter string (default value is ";").
        /// </summary>
        public string lineCommentCharacter { get; set; }

        private string _iniFilePath = null;

        /// <summary>
        /// The initialization file path.
        /// </summary>
        public string iniFilePath
        {
            get
            {
                return _iniFilePath;
            }
            set
            {
                parseFile(value,true);
            }
        }

        // "[section]key"   -> "value1"
        // "[section]key~2" -> "value2"
        // "[section]key~3" -> "value3"
        private Dictionary<string, string> dictionary = new Dictionary<string, string>();

        private bool tryGetValue(string section, string key, out string value)
        {
            string key2;
            if (section.StartsWith("["))
                key2 = String.Format("{0}{1}", section, key);
            else
                key2 = String.Format("[{0}]{1}", section, key);

            return dictionary.TryGetValue(key2.ToLower(), out value);
        }

        /// <summary>
        /// Gets a string value by section and key.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value.</returns>
        /// <seealso cref="getAllValues"/>
        public string getValue(string section, string key, string defaultValue = "")
        {
            string value;
            if (!tryGetValue(section, key, out value)) return defaultValue;

            return value;
        }

        /// <summary>
        /// Gets a string value by section and key.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <returns>The value.</returns>
        /// <seealso cref="getValue"/>
        public string this[string section, string key]
        {
            get
            {
                return getValue(section, key);
            }
        }

        /// <summary>
        /// Gets an integer value by section and key.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="minValue">Optional minimum value to be enforced.</param>
        /// <param name="maxValue">Optional maximum value to be enforced.</param>
        /// <returns>The value.</returns>
        public int getInteger(string section, string key, int defaultValue = 0, 
            int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            string stringValue;
            if (!tryGetValue(section, key, out stringValue))
                return defaultValue;

            int value;
            if (!int.TryParse(stringValue, out value))
            {
                double dvalue;
                if (!double.TryParse(stringValue, out dvalue))
                    return defaultValue;
                value = (int)dvalue;
            }

            if (value < minValue)
                value = minValue;
            if (value > maxValue)
                value = maxValue;
            return value;
        }

        /// <summary>
        /// Gets a double floating-point value by section and key.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="minValue">Optional minimum value to be enforced.</param>
        /// <param name="maxValue">Optional maximum value to be enforced.</param>
        /// <returns>The value.</returns>
        public double getDouble(string section, string key, double defaultValue = 0, 
            double minValue = double.MinValue, double maxValue = double.MaxValue)
        {
            string stringValue;
            if (!tryGetValue(section, key, out stringValue))
                return defaultValue;

            double value;
            if (!double.TryParse(stringValue, out value))
                return defaultValue;

            if (value < minValue)
                value = minValue;
            if (value > maxValue)
                value = maxValue;
            return value;
        }

        /// <summary>
        /// Gets a boolean value by section and key.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value.</returns>
        public bool getBoolean(string section, string key, bool defaultValue = false)
        {
            string stringValue;
            if (!tryGetValue(section, key, out stringValue))
                return defaultValue;

            return (stringValue != "0" && !stringValue.StartsWith("f", true, null));
        }

        /// <summary>
        /// Gets an array of string values by section and key.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <returns>The array of values, or null if none found.</returns>
        /// <seealso cref="getValue"/>
        public string[] getAllValues(string section, string key)
        {
            string key2, key3, value;
            if (section.StartsWith("["))
                key2 = String.Format("{0}{1}", section, key).ToLower();
            else
                key2 = String.Format("[{0}]{1}", section, key).ToLower();

            if (!dictionary.TryGetValue(key2, out value))
                return null;

            List<string> values = new List<string>();
            values.Add(value);
            int index = 1;
            while (true)
            {
                key3 = String.Format("{0}~{1}", key2, ++index);
                if (!dictionary.TryGetValue(key3, out value))
                    break;
                values.Add(value);
            }

            return values.ToArray();
        }

        public List<string> getSectionAsAttributes(string section, bool includeEnabled=true, bool includeDisabled=false)
        {
            List<string> attributes = new List<string>();
            foreach (var key in dictionary.Keys.Where(k => k.StartsWith("[" + section + "]")))
            {
                bool value = FAST.Strings.converters.toBoolean(dictionary[key],false);
                int p1 = key.IndexOf(']');
                var attribute = key.Substring(p1+1);
                if (value && includeEnabled)
                {
                    attributes.Add(attribute);
                }
                else if ( (!value) && includeDisabled )
                {
                    attributes.Add(attribute);
                }             
            }
            return attributes;
        }

        public List<stringsPair> getSectionAsVariables(string section)
        {
            List<stringsPair> variables = new List<stringsPair>();
            foreach (var key in dictionary.Keys.Where(k => k.StartsWith("[" + section + "]")))
            {
                int p1 = key.IndexOf(']');
                var variable = key.Substring(p1+1);
                variables.Add(new stringsPair(variable,dictionary[key] ));
            }
            return variables;
        }



        public void parseFile(string iniFilePath, bool resetInternalFilePath)
        {
            dictionary.Clear();
            if (resetInternalFilePath)
                _iniFilePath = iniFilePath;

            using (StreamReader reader = new StreamReader(iniFilePath))
            {
                parseStream(reader);
                reader.Close();
            }
        }

        public void parseText(string text)
        {
            dictionary.Clear();
            using (StringReader reader = new StringReader(text))
            {
                parseStream(reader);
                reader.Close();
            } 
        }

        public void parseStream(TextReader reader)
        {
            dictionary.Clear();
            string line, section = "";
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.Length == 0)
                    continue;  // empty line
                if (!String.IsNullOrEmpty(lineCommentCharacter) && line.StartsWith(lineCommentCharacter))
                    continue;  // comment

                if (line.StartsWith("[") && line.Contains("]"))  // [section]
                {
                    int index = line.IndexOf(']');
                    section = line.Substring(1, index - 1).Trim();
                    continue;
                }

                if (line.Contains("="))  // key=value
                {
                    int index = line.IndexOf('=');
                    string key = line.Substring(0, index).Trim();
                    string val = line.Substring(index + 1).Trim();
                    string key2 = String.Format("[{0}]{1}", section, key).ToLower();

                    if (val.StartsWith("\"") && val.EndsWith("\""))  // strip quotes
                        val = val.Substring(1, val.Length - 2);

                    if (dictionary.ContainsKey(key2))  // multiple values can share the same key
                    {
                        index = 1;
                        string key3;
                        while (true)
                        {
                            key3 = String.Format("{0}~{1}", key2, ++index);
                            if (!dictionary.ContainsKey(key3))
                            {
                                dictionary.Add(key3, val);
                                break;
                            }
                        }
                    }
                    else
                    {
                        dictionary.Add(key2, val);
                    }
                }
            }
        }

        public void setAny(string variable, object value)
        {
            throw new NotImplementedException();
        }

        public object getAsObject(Type type, string variable, bool nullable)
        {
            if (type != typeof(string))
            {
                throw new Exception("Only string type is supported");
            }

            string section;
            dotNotationToSectionAndKey(variable, out section, out variable);
            return this[section, variable];
        }

        public bool isVariable(string variable)
        {
            string section;
            dotNotationToSectionAndKey(variable, out section, out variable);
            return dictionary.ContainsKey(string.Format("[{0}]{1}", section, variable));
        }

        public bool isVariable(string section, string variable)
        {
            return dictionary.ContainsKey(string.Format("[{0}]{1}", section.ToLower(), variable.ToLower() ));
        }

        private void dotNotationToSectionAndKey(string dotNotationVariable, out string section, out string key)
        {
            section = "";
            key = "";
            var tokens = dotNotationVariable.Split('.');
            if (tokens.Length == 0)
            {
                new ArgumentException("wrong input argument of form secion.variable]");
            }
            else if (tokens.Length == 1)
            {
                section = "defaults";
                key = tokens[0];
            }
            else
            {
                section = tokens[0];
                key = tokens[1];
            }

            if (!string.IsNullOrEmpty(section)) section = section.ToLower();
            if (!string.IsNullOrEmpty(key)) key = key.ToLower();

            return;
        }

    }


    

   

}
