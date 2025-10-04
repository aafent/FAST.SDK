using System.Reflection;
using System.Xml.Serialization;
using FAST.Core;

namespace FAST.Strings
{
    public class stringReplacer : variablesContainer
    {

        #region (+) Constructors

        /// <summary>
        /// No arguments constructor
        /// </summary>
        public stringReplacer() : base()
        {
        }

        /// <summary>
        /// Constructor taking variables from another container
        /// add 13/1/2020
        /// </summary>
        /// <param name="copyFrom"></param>
        public stringReplacer(variablesContainer copyFrom) : this()
        {
            copyFrom.copyVariablesTo(this);
        }

        /// <summary>
        /// Constructor taking variables from another string replacer
        /// add 13/1/2020
        /// </summary>
        /// <param name="copyFrom"></param>
        public stringReplacer(stringReplacer copyFrom) : this()
        {
            copyFrom.copyVariablesTo(this);
        }

        /// <summary>
        /// Construct by taking the string template
        /// </summary>
        /// <param name="template"></param>
        public stringReplacer(string template) :base()
        {
            setTemplate(template);
        }

        /// <summary>
        /// Construct by copying variables from a container 
        /// </summary>
        /// <param name="vars"></param>
        public stringReplacer(InamedVariablesContainer vars) : base(vars)
        {
        }

        /// <summary>
        /// Construct by using a key value pair of strings as name and value 
        /// </summary>
        /// <param name="variables">Enumerable key value pair of strings</param>
        public stringReplacer(IEnumerable<FastKeyValuePair<string, string>> variables) : base(variables)
        {
        }

        #endregion (+) Constructors

        [XmlArray("variablesNeedQuotes"), XmlArrayItem("var")]
        public List<string> variablesNeedQuotes = null;

        [XmlAttribute]
        public bool addQuotesToValue = false;

        [XmlIgnore]
        public List<string> buffers = new List<string>();
        protected string template = "";
        private Dictionary<string, string> namedTemplates = new Dictionary<string, string>();



        private bool _enableVariableUsed = false;
        [XmlIgnore]
        public bool enableVariableUsed
        {
            get
            {
                return _enableVariableUsed;
            }
            set
            {
                _enableVariableUsed = value;
            }
        }

        private List<string> _variablesUsed = null;
        [XmlIgnore]
        public List<string> variablesUsed
        {
            get
            {
                if (_variablesUsed == null) _variablesUsed = new List<string>();
                return _variablesUsed;
            }
            private set
            {
                if (_variablesUsed == null) _variablesUsed = new List<string>();
                _variablesUsed = value;
            }
        }




        public override void reset()
        {
            base.reset();
            buffers.Clear();
        }
        public void clearBuffers()
        {
            buffers.Clear();
        }
        public void clearNamedTemplates()
        {
            namedTemplates.Clear();
        }

        public string replace(string input)
        {
            bool needQuote;

            if ( _enableVariableUsed)
            {
                _variablesUsed = new List<string>();

                foreach (var variable in variables)
                {
                    needQuote = false;
                    if (variablesNeedQuotes != null)
                    {
                        if (variablesNeedQuotes.Contains(variable.left)) { needQuote = true; }
                    }
                    string variableMarkup = "{" + variable.left + "}";
                    if (this.addQuotesToValue && needQuote)
                    {
                        if (variable.right.ToUpper() != "NULL")
                        {
                            if (input.IndexOf(variableMarkup) >= 0)
                            {
                                input = input.Replace(variableMarkup, this.openQuote + variable.right + this.closeQuote);
                                if (!_variablesUsed.Contains(variable.left)) _variablesUsed.Add(variable.left);
                            }
                        }
                        else
                        {
                            if (input.IndexOf(variableMarkup) >= 0)
                            {
                                input = input.Replace(variableMarkup, variable.right);
                                if (!_variablesUsed.Contains(variable.left)) _variablesUsed.Add(variable.left);
                            }
                        }
                    }
                    else
                    {
                        if (input.IndexOf(variableMarkup) >= 0)
                        {
                            input = input.Replace(variableMarkup, variable.right);
                            if (!_variablesUsed.Contains(variable.left)) _variablesUsed.Add(variable.left);
                        }
                    }
                }
            }
            else
            {

                // (^v) same code but does not recording the _variablesUsed. The code is duplicated for performance reasons.
                foreach (var variable in variables)
                {
                    needQuote = false;
                    if (variablesNeedQuotes != null)
                    {
                        if (variablesNeedQuotes.Contains(variable.left)) { needQuote = true; }
                    }
                    string variableMarkup = "{" + variable.left + "}";
                    if (this.addQuotesToValue && needQuote)
                    {
                        if (variable.right.ToUpper() != "NULL")
                        {
                            input = input.Replace(variableMarkup, this.openQuote + variable.right + this.closeQuote);
                        }
                        else
                        {
                            input = input.Replace(variableMarkup, variable.right);
                        }
                    }
                    else
                    {
                        input = input.Replace(variableMarkup, variable.right);
                    }
                }
            }

            
            
            return input;
        }
        public string replace()
        {
            return replace(template);
        }
        public string replace(int numberOfIterations )
        {
            string output = template;
            for ( int tms = 1; tms <= numberOfIterations ; tms ++ )
            {
                output = replace(output);
            }
            return output;
        }

        // (M) DATATYPES POINT
        public void replace<T>(T objectToPopulate)
        {
            bool setTypedValue=true;
            bool isNullable=true;

            PropertyInfo[] properties = objectToPopulate.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance );
            foreach (PropertyInfo p in properties)
            {
                if (!p.CanWrite) { continue; }  // if not readable then cannot check it's value

                MethodInfo mset = p.GetSetMethod(false);
                if (mset == null) { continue; } // Get methods have to be public

                isNullable = reflectionHelper.isNullable(p.PropertyType);
                

                if (isNullable) 
                {
                    if (string.IsNullOrEmpty(this.getString(p.Name)))
                    {
                        p.SetValue(objectToPopulate, null, null);
                        setTypedValue = false;
                    }
                    else
                    {
                        setTypedValue = true;
                    }
                }
                else
                {
                    setTypedValue = true;
                }

                if (setTypedValue)
                {
                    string typeName = reflectionHelper.getTypeName(p.PropertyType);
                    switch (typeName)
                    {
                        case "Byte":
                            p.SetValue(objectToPopulate, this.getByte(p.Name), null);
                            break;

                        case "SByte":
                            p.SetValue(objectToPopulate, (sbyte)this.getByte(p.Name), null);
                            break;

                        case "Int":
                        case "Int32":
                            p.SetValue(objectToPopulate, this.getInt(p.Name), null);
                            break;
                        case "Int64":
                            p.SetValue(objectToPopulate, this.getLong(p.Name), null);
                            break;
                        case "short":
                        case "Int16":
                            p.SetValue(objectToPopulate, this.getInt16(p.Name), null);
                            break;
                        case "Decimal":
                            p.SetValue(objectToPopulate, this.getDecimal(p.Name), null);
                            break;
                        case "Double":
                            p.SetValue(objectToPopulate, this.getDouble(p.Name), null);
                            break;

                        case "String":
                            p.SetValue(objectToPopulate, this.getString(p.Name), null);
                            break;
                        case "DateTime":
                            p.SetValue(objectToPopulate, this.getDateTime(p.Name), null);
                            break;
                        case "Boolean":
                            p.SetValue(objectToPopulate, this.getBoolean(p.Name), null);
                            break;
                        default:
                            if (p.PropertyType.IsEnum)
                            {
                                Enum.Parse(p.PropertyType, this.getString(p.Name), false);
                                break;
                            }

                            throw new Exception(string.Format("Cannot find the matching type for:{0} variable:{1} in class stringReplacer::replace<T>()", typeName, p.Name));

                    }
                }

                
            }


            FieldInfo[] fields = objectToPopulate.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo f in fields)
            {
                f.SetValue(objectToPopulate, this.getString(f.Name));
            }
        }

        public string reformat(string text)
        {
            string output = "";
            var tokens = tokenizer.queueTokens(text, "%", "%", '\\', false);
            foreach (var token in tokens)
            {
                switch (token.type)
                {
                    case tokenizer.tokenType.beginDelimeter:
                    case tokenizer.tokenType.endDelimeter:
                        continue; // by pass delimiters
                    case tokenizer.tokenType.text:
                        output += token.token;
                        break;
                    case tokenizer.tokenType.textBetweenDelimeters:
                        string value = "";
                        var pair = converters.toStringsPair(token.token, ":");
                        int size = 0;
                        if (!int.TryParse(pair.right, out size)) { size = 0; }
                        if ( this.isVariable(pair.left))
                        {
                            value = this.getString(pair.left);
                        }
                        if (size != 0)
                        {
                            value = string.Format("{0," + (-1 * size).ToString() + "}", value).Substring(0, Math.Abs(size));
                        }
                        output += value;
                        break;
                    default:
                        break;
                }
            }
            return output;
        }
        public string reformat()
        {
            return reformat(template);
        }

        public void setTemplate(string template, bool triggerTemplateChangedEvent=true)
        {
            this.template = template;
            if (triggerTemplateChangedEvent) { templateChanged(); }
        }
        public void addTemplate(string templateName, string template, bool activate=false)
        {
            if (!namedTemplates.ContainsKey(templateName))
            {
                namedTemplates.Add(templateName, template);
            }
            else
            {
                namedTemplates[templateName] = template;
            }
            if (activate) { setTemplate(template); }
        }
        public void addTemplate(Enum templateName, string template, bool activate = false)
        {
            addTemplate(templateName.ToString(), template, activate);
        }
        public void addTemplate(int templateNumber, string template, bool activate = false)
        {
            addTemplate(templateNumber.ToString(), template,activate);
        }

        public void activateTemplate(string templateName)
        {
            if (namedTemplates.ContainsKey(templateName))
            {
                setTemplate( namedTemplates[templateName]);
            }
        }
        public void activateTemplate(Enum templateName)
        {
            activateTemplate(templateName.ToString());
        }
        public void activateTemplate(int templateNumber)
        {
            activateTemplate(templateNumber.ToString());
        }

        public bool isTemplate(string templateName)
        {
            return namedTemplates.Keys.Contains(templateName);
        }
        public bool isTemplate(Enum templateName)
        {
            return isTemplate(templateName.ToString());
        }
        public bool isTemplate(int templateNumber)
        {
            return isTemplate(templateNumber.ToString());
        }

        public virtual void declareQuotable(string variable)
        {
            if ( !variablesNeedQuotes.Contains(variable) ) variablesNeedQuotes.Add(variable);
        }
        public virtual void undeclareQuotable(string variable)
        {
            if (variablesNeedQuotes.Contains(variable)) variablesNeedQuotes.Remove(variable);
        }

        public virtual void set(string variable, string value, bool delcareAsQuotable)
        {
            base.set(variable, value);
            if ( delcareAsQuotable ) declareQuotable(variable);
        }


        public void addToBuffer(string input)
        {
            buffers.Add(input);
        }
        public void addToBuffer()
        {
            addToBuffer(replace());
        }

        public string substitute(string commandsSyntax)
        {
            return substitute(commandsSyntax, template);
        }
        public string substitute(string commandsSyntax, string input)
        {
            input = replace(input);
            var commands = commandsSyntax.Split( new string[1]{"//"}, StringSplitOptions.RemoveEmptyEntries  );
            foreach (var nextCommand in commands)
            {
                var args = nextCommand.Split(new string[1] { "," }, StringSplitOptions.RemoveEmptyEntries);
                switch (args[0].ToUpper() )
                {
                    case "UPPERGREEK":
                        input = stringsHelper.toGreekUpperWithoutAccesnts(input);
                        break;

                    case "UPPER":
                        input = input.ToUpper();
                        break;

                    case "LOWER":
                        input = input.ToLower();
                        break;

                    case "TRIM":
                        input = input.Trim();
                        break;

                    case "NOSPACE":
                        input= input.Replace(' ', '_');
                        break;
                        
                    case "NOSLASH":
                        input = input.Replace('/', '-');
                        break;

                    case "TOLATIN":
                        input = stringsHelper.toGreeklish(input);
                        break;

                    default:
                        break;
                }

            }
            return input;
        }

        protected virtual void templateChanged()
        {
        }

    }
}
