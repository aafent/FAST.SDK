using FAST.Strings;
using FAST.Types;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Reflection;
using System.Xml.Serialization;

namespace FAST.Core
{
    /// <summary>
    /// A container for named variables. 
    /// This class implements IvariablesContainer and InamedVariablesContainer
    /// It is one of the most used classes in FAST
    /// </summary>
    [XmlRoot("variables")]
    public class variablesContainer : IvariablesContainer, InamedVariablesContainer
    {

        /// <summary>
        /// The opening quote for string variables
        /// </summary>
        protected virtual string openQuote { get; } = "'";

        /// <summary>
        /// The closing quote for string variables
        /// </summary>
        protected virtual string closeQuote { get; } = "'";

        /// <summary>
        /// The date time format to use when setting or getting DateTime variables
        /// </summary>
        protected virtual string dateTimeFormat { get; } = "yyyy-MM-ddTHH:mm:ss";


        /// <summary>
        /// The internal list of variables
        /// </summary>
        [XmlArray("vars"), XmlArrayItem("pair")]
        public List<stringsPair> variables = new List<stringsPair>();

        /// <summary>
        /// Indicates if the last set variable was of a quotable type (string, datetime)
        /// </summary>
        // (v) add 13/1/2020
        [XmlIgnore]
        public bool wasQuotableType { get; set; } = false;
        /*
         * public bool wasQuotableType { get; private set; } = false;
         * Cannot de-serialize type 'FAST.Core.variablesBuffer' because it contains property 'wasQuotableType' which has no public setter.
         * */

        #region (+) Constructors 

        /// <summary>
        /// Constructor without arguments
        /// </summary>
        public variablesContainer()
        {
        }

        // (v) 22 nov 2019
        /// <summary>
        /// Construct by copying variables from another container
        /// </summary>
        /// <param name="copyFrom"></param>
        public variablesContainer(variablesContainer copyFrom) : this()
        {
            copyFrom.copyVariablesTo(this);
        }

        /// <summary>
        /// Construct by copying variables from a container 
        /// </summary>
        /// <param name="vars"></param>
        public variablesContainer(InamedVariablesContainer vars)
        {
            var all = vars.getVariableNames();
            foreach( var vname in all)
            {
                this.setAny(vname, vars.getAsObject(typeof(object), vname, false) );
            }
        }
        
        /// <summary>
        /// Construct by using a key value pair of strings as name and value 
        /// </summary>
        /// <param name="variables">Enumerable key value pair of strings</param>
        public variablesContainer(IEnumerable<FastKeyValuePair<string, string>> variables)
        {
            foreach( var item in variables)
            {
                this.set(item.Key,item.Value);
            }
        }

        #endregion (+) Constructors 


        /// <summary>
        /// Reset the container by removing all variables
        /// </summary>
        public virtual void reset()
        {
            variables.Clear();
        }

        /// <summary>
        /// Clear all variable values
        /// </summary>
        public virtual void clearVariables()
        {
            foreach (var variable in variables)
            {
                variable.right = "";
            }
        }

        /// <summary>
        /// Preprocess the variable name before using it
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        protected virtual string preprocessVariableName(string variable)
        {
            wasQuotableType = false;
            return variable.Trim();
        }

        /// <summary>
        /// Postprocess after setting a variable
        /// </summary>
        /// <param name="variable"></param>
        protected virtual void postprocessSet(string variable)
        {
        }


        #region (+) set methods


        // DATATYPES
        /// <summary>
        /// Set multiple parameters from a DbParameterCollection
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="setBySourceColumn"></param>
        public void set(DbParameterCollection parameters, bool setBySourceColumn)
        {
            foreach (var parameter in parameters)
            {
                set((DbParameter)parameter, setBySourceColumn);
            }
        }

        /// <summary>
        /// Set a single parameter from a DbParameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="setBySourceColumn"></param>
        /// <exception cref="Exception"></exception>
        public virtual void set(DbParameter parameter, bool setBySourceColumn)
        {
            string variableName = setBySourceColumn ? parameter.SourceColumn : parameter.ParameterName;

            // (M) DATATYPES POINT
            switch (parameter.DbType)
            {
                case System.Data.DbType.String:
                    if (parameter.Value is System.DBNull)
                    {
                        // TODO: i dont like this cast. Must be review
                        set(variableName, (string)string.Empty);  
                    }
                    else
                    {
                        if (((string)parameter.Value) == "null")
                        {
                            set(variableName, (string)string.Empty);
                        }
                        else
                        {
                            set(variableName, (string)parameter.Value);
                        }
                    }
                    return;

                case System.Data.DbType.DateTime:
                    if (parameter.Value is System.DBNull)
                    {
                        // TODO: i dont like this cast. Must be review
                        set(variableName, (string)string.Empty);  
                    }
                    else
                    {
                        if (parameter.Value == null)
                        {
                            set(variableName, (string)string.Empty);
                        }
                        else
                        {
                            set(variableName, (DateTime)parameter.Value);
                        }
                    }
                    return;

                case System.Data.DbType.Boolean:
                    set(variableName, (Boolean)parameter.Value);
                    return;

                case System.Data.DbType.Decimal:
                    set(variableName, parameter.Value == null ? (Decimal?)parameter.Value : (Decimal)parameter.Value);
                    return;

                case System.Data.DbType.Double:
                    set(variableName, parameter.Value == null ? (Double?)parameter.Value : (Double)parameter.Value);
                    return;

                case System.Data.DbType.Int16:
                    if (parameter.Value is byte)
                    {
                        set(variableName, (byte)parameter.Value);
                    }
                    else
                    {
                        set(variableName, (Int16)parameter.Value);
                    }
                    return;

                case System.Data.DbType.Int32:
                    if (parameter.Value is System.DBNull) 
                    {
                        setNull(variableName);
                        //set(variableName, 0); // TODO: i dont like this cast. Must be review
                    }
                    else
                    { 
                        set(variableName, (int)parameter.Value);
                    }
                   return;

                case System.Data.DbType.Int64:
                   set(variableName, (Int64)parameter.Value);
                   return;


                default:
                   throw new Exception(string.Format("set(DbParameter) cannot set type:{0}", parameter.DbType.ToString() ));

            }

        }

        /// <summary>
        /// Set multiple variables from a data record
        /// </summary>
        /// <param name="source"></param>
        /// <param name="fieldSelection"></param>
        public virtual void set(IDataRecord source, Func<string, bool> fieldSelection=null)
        {
            for( int inx=0 ; inx<source.FieldCount; inx++ )
            {
                string vname = source.GetName(inx);
                if ( fieldSelection == null )
                {
                    this.setAny(vname, source[inx]);
                } else
                {
                    if ( fieldSelection(vname) ) this.setAny(vname, source[inx]);
                }
            }
        }



        // (M) DATATYPES POINT
        /// <summary>
        /// Set a variable to null
        /// </summary>
        /// <param name="variable"></param>
        public virtual void setNull(string variable)
        {
            set(variable, (string)null);
            wasQuotableType = false;
        }

        /// <summary>
        /// Set a variable to a string value
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        public virtual void set(string variable, string value)
        {
            string inputVariable = variable;
            variable = preprocessVariableName(variable);
            if (!string.IsNullOrEmpty(value)) { value=value.Replace("'", "`"); }
            if (variables.Any( v => (v.left == variable) ) )
            {
                variables.First(v => (v.left == variable)  ).right = value;
            }
            else
            {
                variables.Add(new stringsPair() { left = variable, right = value });
            }
            wasQuotableType = true;
            postprocessSet(inputVariable);
        }

        /// <summary>
        /// Set a variable to a DateTime value
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        public virtual void set(string variable, DateTime value)
        {
            value=DateTime.SpecifyKind(value, DateTimeKind.Local);
            set(variable, value.ToLocalTime().ToString(this.dateTimeFormat));
            wasQuotableType = true;
        }

        /// <summary>
        /// Set a variable to a boolean value
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        public virtual void set(string variable, bool value)
        {
            if (value) { set(variable, 1); } else { set(variable, 0); }
            wasQuotableType = false;
        }

        /// <summary>
        /// Set a variable to a byte value
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        public virtual void set(string variable, decimal value)
        {
            set(variable, value.ToString(CultureInfo.InvariantCulture));
            wasQuotableType = false;
        }

        /// <summary>
        /// Set a variable to a nullable decimal value
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        public virtual void set(string variable, decimal? value)
        {
            if (value == null)
            {
                set(variable, "null");
            }
            else
            {
                set(variable, ((decimal)value).ToString(CultureInfo.InvariantCulture));
            }
            wasQuotableType = false;
        }
        // (!) int = Int32
        /// <summary>
        /// Set a variable to an int value
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        public virtual void set(string variable, int value)
        {
            set(variable, value.ToString(CultureInfo.InvariantCulture));
            wasQuotableType = false;
        }
        // (!) long = Int64
        /// <summary>
        /// Set a variable to a long value
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        public virtual void set(string variable, long value)
        {
            set(variable, value.ToString(CultureInfo.InvariantCulture));
            wasQuotableType = false;
        }

        /// <summary>
        /// Set a variable to a nullable int value
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        public virtual void set(string variable, Int16 value)
        {
            set(variable, value.ToString(CultureInfo.InvariantCulture));
            wasQuotableType = false;
        }

        /// <summary>
        /// Set a variable to a nullable int value
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        public virtual void set(string variable, double? value)
        {
            if (value == null)
            {
                set(variable, "null");
            }
            else
            {
                set(variable, ((double)value).ToString(CultureInfo.InvariantCulture));
            }
            wasQuotableType = false;
        }

        /// <summary>
        /// Set a variable to a double value
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        public virtual void set(string variable, double value)
        {
            set(variable, value.ToString(CultureInfo.InvariantCulture));
            wasQuotableType = false;
        }

        /// <summary>
        /// Set a variable to a byte value
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        /// <exception cref="Exception"></exception>
        public virtual void setAny(string variable, object value)
        {
            // (M) DATATYPES POINT
            if (value == null) { this.set(variable, "null"); }
            else if (value is string) { this.set(variable, (string)value); }
            else if (value is int) { this.set(variable, (int)value); }
            else if (value is sbyte) { this.set(variable, (sbyte)value); }
            else if (value is Int16) { this.set(variable, (Int16)value); }
            else if (value is Int32) { this.set(variable, (Int32)value); }
            else if (value is Int64) { this.set(variable, (Int64)value); }
            else if (value is long) { this.set(variable, (long)value); }
            else if (value is double) { this.set(variable, (double)value); }
            else if (value is Byte) { this.set(variable, (Byte)value); }
            else if (value is Decimal) { this.set(variable, (Decimal)value); }
            else if (value is DateTime) { this.set(variable, (DateTime)value); }
            else if (value is Boolean) { this.set(variable, (Boolean)value); }
            else if (value is Enum) { this.set(variable, value.ToString()); }
            else if (value.GetType().IsArray)
            {  
                // (v) 2024-12-15
                IEnumerable items = (IEnumerable)value;
                this.setAny(variable, items.Cast<object>().FirstOrDefault());
            }
            else if (value.GetType().IsGenericType) //value.GetType().IsArray ||  //2018-01-31
            {
                IEnumerable items = (IEnumerable)value;
                this.setAny(variable, items.Cast<object>().FirstOrDefault());
            }
            else if (value.GetType().IsClass) //2018-01-31
            {
                this.setFromObjects(setFromObjectsPolicy.dbData, value);
            }
            else
            {
                throw new Exception(string.Format("Cannot set value for variable: {0} of type: {1}. Found in: variablesBuffer::mySetAny()", variable, value.GetType().ToString()));
            }

            //if (canQuote)
            //{
            //    if (this.variablesNeedQuotes == null) this.variablesNeedQuotes = new();
            //    if (!this.variablesNeedQuotes.Contains(variable)) this.variablesNeedQuotes.Add(variable);
            //}

        }


        /// <summary>
        /// Set the value only once. 
        /// Do not set if the variable exists
        /// </summary>
        /// <param name="variable">The variable name</param>
        /// <param name="value">The value</param>
        public void setAnyOnce(string variable, object value)
        {
            if (! isVariable(variable) ) setAny(variable, value);
        }

        /// <summary>
        /// Set multiple values from the input objects properties 
        /// </summary>
        /// <param name="policy">The setting policy</param>
        /// <param name="args">The object to extract from the values</param>
        public void setFromObjects(setFromObjectsPolicy policy, params object[] args )
        {
            foreach (var objectToCopyValues in args)
            {

                if (objectToCopyValues is dynamicObject)
                {
                    dynamicObject dobj = (dynamicObject)objectToCopyValues;
                    foreach (var field in dobj.getFieldNames())
                    {
                        this.setAny(field, dobj[field]);
                    }
                    return; // (<) no further processing required for dynamic objects
                }
                else if (objectToCopyValues.GetType().IsArray)
                {
                    IEnumerable items = (IEnumerable)objectToCopyValues;
                    this.setFromObjects(policy, items.Cast<object>().FirstOrDefault());
                    return;  // (<) no further processing required
                }
                else if (objectToCopyValues.GetType().IsGenericType)
                {
                    IEnumerable items = (IEnumerable)objectToCopyValues;
                    this.setFromObjects(policy, items.Cast<object>().FirstOrDefault());
                    return; // (<) no further processing required
                }

                // (v) ELSE any other object

                PropertyInfo[] properties = objectToCopyValues.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo p in properties)
                {
                    if (!p.CanRead) { continue; }  // if not readable then cannot check it's value

                    MethodInfo mget = p.GetGetMethod(false);
                    if (mget == null) { continue; } // Get methods have to be public

                    object value = p.GetValue(objectToCopyValues, null);
                    if ((policy.ignoreNulls) && (value == null )) { continue; }

                    this.setAny(p.Name, value);
                }


                FieldInfo[] fields = objectToCopyValues.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo f in fields)
                {
                    object value=f.GetValue(objectToCopyValues);
                    if ((policy.ignoreNulls) && (value == null)) { continue; } 
                    this.setAny(f.Name, value);
                }


            }
        }

        #endregion (+) set methods

        #region (+) get numeric
        // (M) DATATYPES POINT
        public virtual Byte getByte(string variable)
        {
            string stringValue = getString(variable);
            if (string.IsNullOrEmpty(stringValue)) { return 0; }
            Byte value;
            if (!Byte.TryParse(getString(variable), NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                throw new Exception(string.Format("Variable: {0} does not contain a valid Byte value. Current value is: {1}", variable, stringValue));
            }
            else
            {
                return value;
            }
        }
        public virtual Int16 getInt16(string variable)
        {
            string stringValue = getString(variable);
            if (string.IsNullOrEmpty(stringValue)) { return 0; }
            Int16 value;
            if (!Int16.TryParse(getString(variable), NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                throw new Exception(string.Format("Variable: {0} does not contain a valid Int16 value. Current value is: {1}", variable, stringValue));
            }
            else
            {
                return value;
            }
        }
        public virtual int getInt(string variable)
        {
            string stringValue = getString(variable);
            if (string.IsNullOrEmpty(stringValue)) { return 0; }
            int value;
            if (!int.TryParse(getString(variable), NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                throw new Exception(string.Format("Variable: {0} does not contain a valid int value. Current value is: {1}", variable, stringValue));
            }
            else
            {
                return value;
            }

        }
        public virtual long getLong(string variable)
        {
            string stringValue = getString(variable);
            if (string.IsNullOrEmpty(stringValue)) { return 0; }
            long value;
            if (!long.TryParse(getString(variable), NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                throw new Exception(string.Format("Variable: {0} does not contain a valid int value. Current value is: {1}", variable, stringValue));
            }
            else
            {
                return value;
            }

        }
        public virtual decimal getDecimal(string variable)
        {
            string stringValue = getString(variable);
            if (string.IsNullOrEmpty(stringValue)) { return 0M; }
            decimal value;
            if (!Decimal.TryParse(getString(variable), NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                throw new Exception(string.Format("Variable: {0} does not contain a valid decimal value. Current value is: {1}", variable, stringValue));
            }
            else
            {
                return value;
            }

        }
        public virtual double getDouble(string variable)
        {
            string stringValue = getString(variable);
            if (string.IsNullOrEmpty(stringValue)) { return 0; }
            double value;
            if (!double.TryParse(getString(variable), NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                throw new Exception(string.Format("Variable: {0} does not contain a valid decimal value. Current value is: {1}", variable, stringValue));
            }
            else
            {
                return value;
            }

        }
        #endregion (+) get numeric

        #region (+) get other types 

        /// <summary>
        /// Get a variable as a string
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public virtual string getString(string variable)
        {
            variable = preprocessVariableName(variable);
            if (variables.Any(v => v.left == variable))
            {
                return variables.First(v => v.left == variable).right;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get a variable as a nullable DateTime
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public virtual DateTime? getDateTime(string variable, DateTimeKind kind = DateTimeKind.Local)
        {
            var sValue = this.getString(variable);
            if (string.IsNullOrEmpty(sValue) || sValue=="null" )
            {
                return null;
            }
            else
            {
                DateTime value = Convert.ToDateTime(sValue);
                value = DateTime.SpecifyKind(value, kind);
                return value;
            }
        }

        /// <summary>
        /// Get a variable as a boolean
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public virtual bool getBoolean(string variable)
        {
            string stringValue = getString(variable);
            if (string.IsNullOrEmpty(stringValue)) { return false; }
            bool value;
            if (!bool.TryParse(stringValue, out value))
            {
                if (stringValue == "0") 
                { 
                    stringValue = Boolean.FalseString; 
                } 
                else 
                { 
                    stringValue = Boolean.TrueString; 
                }
                if (!bool.TryParse(stringValue, out value))
                {
                    throw new Exception(string.Format("Variable: {0} does not contain a valid bool value. Current value is: {1}", variable, stringValue));
                }
            }

            return value;

        }
        #endregion (+) get other types

        /// <summary>
        /// Get a variable as an object of the specified type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="variable"></param>
        /// <param name="nullable"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public virtual object getAsObject(Type type, string variable, bool nullable=false )
        {
            // (M) DATATYPES POINT
            if (reflectionHelper.isNullable(type)) 
            {
                type=type.GetGenericArguments()[0];
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Object:
                    return (object)getString(variable);
                case TypeCode.Byte:
                    return getByte(variable);
                case TypeCode.Int16:
                    return getInt(variable);
                case TypeCode.SByte:
                    return getByte(variable);
                case TypeCode.Int32:
                    return getInt(variable);
                case TypeCode.Int64:
                    return getLong(variable);
                case TypeCode.Decimal:
                    return getDecimal(variable);
                case TypeCode.Double:
                    return getDouble(variable);
                case TypeCode.String:
                    string stringValue = getString(variable);
                    if (stringValue.ToUpper() == "NULL") { stringValue = null; }
                    return stringValue;
                case TypeCode.DateTime:

                    return nullable ? getDateTime(variable) : (DateTime)getDateTime(variable);
                case TypeCode.Boolean:
                    return getBoolean(variable);
                default:
                    throw new Exception(string.Format("getObjec<T>() does not support type: {0} yet.", type.ToString()));
            }
        }

        /// <summary>
        /// Check if the input string contains any variable pattern {varname}
        /// </summary>
        /// <param name="stringToTest"></param>
        /// <returns></returns>
        public bool hasAnyVariable(string stringToTest)
        {
            int p1 = stringToTest.IndexOf('{');
            if (p1 < 0) { return false; }
            p1 = stringToTest.IndexOf('}', p1);
            if (p1 < 0) { return false; }
            return true;
        }

        /// <summary>
        /// Check if the input string is a variable name
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public bool isVariable(string variable)
        {
            variable = preprocessVariableName(variable);
            return variables.Any(v => v.left == variable);
        }


        /// <summary>
        /// Copy the inner variables to the input buffer
        /// </summary>
        /// <param name="otherVariables">The input variables buffer</param>
        public void copyVariablesTo(variablesContainer otherVariables)
        {
            foreach (var variable in variables)
            {
                otherVariables.set(variable.left, variable.right);
            }
        }

        /// <summary>
        /// Copy variables from variables container into the buffer
        /// </summary>
        /// <param name="source">Source named variables container</param>
        public void copyVariablesFrom(InamedVariablesContainer source)
        {
            var names=source.getVariableNames();
            foreach (var name in names)
            {
                var value=source.getAsObject(typeof(string), name, false);
                this.set(name, value.ToString() );
            }
        }


        /// <summary>
        /// Get the internal list of string pairs
        /// </summary>
        /// <returns></returns>
        public List<stringsPair> stringsPairList()
        {
            return variables;
        }

        /// <summary>
        /// Get a list of keys from a variable that contains a delimited list of keys
        /// </summary>
        /// <param name="keyListVariableName"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public List<string> keyList(string keyListVariableName, string delimiter)
        {
            List<string> keys = new List<string>();
            var keyList = getString(keyListVariableName);

            if (!string.IsNullOrEmpty(keyList))
            {
                keys = keyList.Split(delimiter.ToArray() ).ToList();
            }

            return keys;
        }

        /// <summary>
        /// Convert the variables container to a json object
        /// </summary>
        /// <returns></returns>
        public json toJson()
        {
            return json.toJson(this);
        }

        private IEnumerable<string> inner_getVariableNames()
        {
            foreach ( var variable in variables) yield return variable.left;
        }

        /// <summary>
        /// Get all variable names
        /// </summary>
        /// <returns>String array with the names</returns>
        public string[] getVariableNames()
        {
            return inner_getVariableNames().DefaultIfEmpty().ToArray();
        }
    }
}
