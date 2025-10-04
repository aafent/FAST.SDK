using FAST.Strings;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Reflection;

namespace FAST.Core
{
    /// <summary>
    /// Helping class to get values from an IvariablesContainer easy
    /// Class can be customized by inheritance and virtual elements
    /// </summary>
    public class variablesValue : IvariablesContainer
    {
        private IvariablesContainer variables;

        protected virtual string dateTimeFormat { get; } = "yyyy-MM-ddTHH:mm:ss";


        /// <summary>
        /// Check if the last used variable was quotable 
        /// Quotable types are the values need quotes like strings,dates etc
        /// </summary>
        public bool wasQuotableType { get; protected set; } = false;

        /// <summary>
        /// Contractor taking variables from a container
        /// </summary>
        /// <param name="variables">The variables container</param>
        public variablesValue(IvariablesContainer variables)
        {
            this.wasQuotableType = false;
            this.variables = variables;
        }

        /// <summary>
        /// Construct by using a key value pair of strings as name and value 
        /// </summary>
        /// <param name="variables">Enumerable key value pair of strings</param>
        public variablesValue(IEnumerable<FastKeyValuePair<string, string>> variables)
        {
            this.variables = new variablesContainer(variables);
        }

        /// <summary>
        /// Construct copying variablesAndAttributes object
        /// </summary>
        /// <param name="varAndAttr"></param>
        public variablesValue(variablesAndAttributes varAndAttr)
        {
            varAndAttr.copyTo(this);
        }

        protected virtual string preprocessVariableName(string variable)
        {
            this.wasQuotableType = false;
            return variable.Trim();
        }
        protected virtual void postprocessSet(string variable)
        {
        }


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
        public virtual string getString(string variable)
        {
            variable = preprocessVariableName(variable);
            var value = variables.getAsObject(typeof(string), variable, true);
            if (value == null) return null;
            else return value.ToString();
        }

        public virtual DateTime? getDateTime(string variable, DateTimeKind kind = DateTimeKind.Local)
        {
            var sValue = this.getString(variable);
            if (string.IsNullOrEmpty(sValue) || sValue == "null")
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

        public object getAsObject(Type type, string variable, bool nullable)
        {
            return variables.getAsObject(type, variable, nullable);
        }
        #endregion (+) get other types

        #region (+) set methods


        // DATATYPES
        public void set(DbParameterCollection parameters, bool setBySourceColumn)
        {
            foreach (var parameter in parameters)
            {
                set((DbParameter)parameter, setBySourceColumn);
            }
        }
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
                    throw new Exception(string.Format("set(DbParameter) cannot set type:{0}", parameter.DbType.ToString()));

            }

        }
        public virtual void set(IDataRecord source, Func<string, bool> fieldSelection = null)
        {
            for (int inx = 0; inx < source.FieldCount; inx++)
            {
                string vname = source.GetName(inx);
                if (fieldSelection == null)
                {
                    this.setAny(vname, source[inx]);
                }
                else
                {
                    if (fieldSelection(vname)) this.setAny(vname, source[inx]);
                }
            }
        }



        // (M) DATATYPES POINT
        public virtual void setNull(string variable)
        {
            set(variable, (string)null);
            wasQuotableType = false;
        }
        public virtual void set(string variable, string value)
        {
            string inputVariable = variable;
            variable = preprocessVariableName(variable);
            if (!string.IsNullOrEmpty(value)) { value = value.Replace("'", "`"); }
            variables.setAny(variable, value);
            wasQuotableType = true;
            postprocessSet(inputVariable);
        }
        public virtual void set(string variable, DateTime value)
        {
            value = DateTime.SpecifyKind(value, DateTimeKind.Local);
            set(variable, value.ToLocalTime().ToString(this.dateTimeFormat));
            wasQuotableType = true;
        }
        public virtual void set(string variable, bool value)
        {
            if (value) { set(variable, 1); } else { set(variable, 0); }
            wasQuotableType = false;
        }
        public virtual void set(string variable, decimal value)
        {
            set(variable, value.ToString(CultureInfo.InvariantCulture));
            wasQuotableType = false;
        }
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
        public virtual void set(string variable, int value)
        {
            set(variable, value.ToString(CultureInfo.InvariantCulture));
            wasQuotableType = false;
        }
        // (!) long = Int64
        public virtual void set(string variable, long value)
        {
            set(variable, value.ToString(CultureInfo.InvariantCulture));
            wasQuotableType = false;
        }
        public virtual void set(string variable, Int16 value)
        {
            set(variable, value.ToString(CultureInfo.InvariantCulture));
            wasQuotableType = false;
        }

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
        public virtual void set(string variable, double value)
        {
            set(variable, value.ToString(CultureInfo.InvariantCulture));
            wasQuotableType = false;
        }

        public virtual void setAny(string variable, object value)
        {
            // (M) DATATYPES POINT
            if (value == null) { set(variable, "null"); }
            else if (value is string) { set(variable, (string)value); }
            else if (value is int) { set(variable, (int)value); }
            else if (value is sbyte) { set(variable, (sbyte)value); }
            else if (value is Int16) { set(variable, (Int16)value); }
            else if (value is Int32) { set(variable, (Int32)value); }
            else if (value is Int64) { set(variable, (Int64)value); }
            else if (value is long) { set(variable, (long)value); }
            else if (value is double) { set(variable, (double)value); }
            else if (value is Byte) { set(variable, (Byte)value); }
            else if (value is Decimal) { set(variable, (Decimal)value); }
            else if (value is DateTime) { set(variable, (DateTime)value); }
            else if (value is Boolean) { set(variable, (Boolean)value); }
            else if (value is Enum) { set(variable, value.ToString()); }
            else if (value.GetType().IsGenericType) //value.GetType().IsArray ||  //2018-01-31
            {
                IEnumerable items = (IEnumerable)value;
                setAny(variable, items.Cast<object>().FirstOrDefault());
            }
            else if (value.GetType().IsClass) //2018-01-31
            {
                this.setFromObjects(setFromObjectsPolicy.dbData, value);
            }
            else
            {
                throw new Exception(string.Format("Cannot set value for variable: {0} of type: {1}. Found in: variablesBuffer::setAny()", variable, value.GetType().ToString()));
            }
        }

        /// <summary>
        /// Set the value only once. 
        /// Do not set if the variable exists
        /// </summary>
        /// <param name="variable">The variable name</param>
        /// <param name="value">The value</param>
        public void setAnyOnce(string variable, object value)
        {
            if (!isVariable(variable)) setAny(variable, value);
        }


        /// <summary>
        /// Set values from the input as arguments objects, by following the specified policy.
        /// </summary>
        /// <param name="policy">The policy</param>
        /// <param name="args">The array object objects</param>
        public void setFromObjects(setFromObjectsPolicy policy, params object[] args)
        {
            foreach (var objectToCopyValues in args)
            {
                PropertyInfo[] properties = objectToCopyValues.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo p in properties)
                {
                    if (!p.CanRead) { continue; }  // if not readable then cannot check it's value

                    MethodInfo mget = p.GetGetMethod(false);
                    if (mget == null) { continue; } // Get methods have to be public

                    object value = p.GetValue(objectToCopyValues, null);
                    if ((policy.ignoreNulls) && (value == null)) { continue; }

                    this.setAny(p.Name, value);
                }

                FieldInfo[] fields = objectToCopyValues.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo f in fields)
                {
                    object value = f.GetValue(objectToCopyValues);
                    if ((policy.ignoreNulls) && (value == null)) { continue; }
                    this.setAny(f.Name, value);
                }
            }
        }

        #endregion (+) set methods

        /// <summary>
        /// Check if a specific name is a variable
        /// </summary>
        /// <param name="variable">The name to check</param>
        /// <returns>Boolean, true if the name is a variable</returns>
        public bool isVariable(string variable)
        {
            variable = preprocessVariableName(variable);
            return variables.isVariable(variable);

        }
    }
}
