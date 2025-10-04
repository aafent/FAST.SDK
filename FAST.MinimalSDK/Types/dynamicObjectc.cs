using FAST.Core;
using System.Dynamic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;

namespace FAST.Types
{
    /// <summary>
    /// Object for Dynamic data types manipulation
    /// </summary>
    public class dynamicObject : IcopyTo<IvariablesContainer>, ICloneable
    {
        private dynamic _obj;

        private PropertyInfo[] propertiesOfData;
        private FieldInfo[] fieldsOfData;

        private List<string> propertyNamesOfData;
        private List<string> fieldNamesOfData;

        private IDictionary<string, object> expandoDict;

        private IEnumerable<string> needQuotes;

        /// <summary>
        /// Properties and Fields indexer. To get the value of an object
        /// </summary>
        /// <param name="fieldName">The name of the field</param>
        /// <returns>Object, the field's value</returns>
        public object this[string fieldName]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(fieldName))
                {
                    throw new ArgumentOutOfRangeException("fieldName");
                }

                return getValue(fieldName);
            }
            set
            {
                if (string.IsNullOrWhiteSpace(fieldName))
                {
                    throw new ArgumentOutOfRangeException("fieldName");
                }
                setValue(fieldName, value);
            }
        }

        /// <summary>
        /// The value of the underlying dynamic object
        /// </summary>
        public dynamic underlyingObject
        {
            get => _obj;
        }

        /// <summary>
        /// Enumerable with field names that base on their datatypes, 
        /// they need quotes in value representation (eg. string).
        /// List is created once, on the first reference.
        /// The list if never set, or set to null, calculated using the datatype of the value,
        /// If set, the set collection of values is used.
        /// </summary>>
        public IEnumerable<string> fieldsNeedQuotes
        {
            // (v) if privet is null, return the list based on the value's datatype
            get
            {
                if (needQuotes == null)
                {
                    needQuotes = getFieldsNeedQuotesFromDatatypes();
                }

                return needQuotes;
            }

            // (v) override the list and use the given.
            set
            {
                needQuotes = value;
            }
        }

        /// <summary>
        /// True if the underlying object is Expando object
        /// </summary>
        public bool isExpando
        {
            get;
            private set;
        } = false;


        /// <summary>
        /// Constructor with the underlying dynamic object
        /// The dynamic object can by ExpandoObject or any other type of Dynamic Object
        /// </summary>
        /// <param name="obj"></param>
        public dynamicObject(dynamic obj)
        {
            _obj = obj;
            isExpando = _obj is ExpandoObject;
            if (isExpando)
            {
                expandoDict = (IDictionary<string, object>)_obj;
                return;
            }

            propertiesOfData = _obj.GetType().GetProperties();
            propertyNamesOfData = propertiesOfData.Select((PropertyInfo p) => p.Name).ToList();

            fieldsOfData = _obj.GetType().GetFields();
            fieldNamesOfData = fieldsOfData.Select((FieldInfo p) => p.Name).ToList();
        }

        /// <summary>
        /// Check if a field exists
        /// </summary>
        /// <param name="fieldName">The field name</param>
        /// <returns>boolean, true if the field exists</returns>
        public bool isField(string fieldName)
        {
            if (isExpando)
            {
                return expandoDict.ContainsKey(fieldName);
            }

            return propertyNamesOfData.Contains(fieldName) || fieldNamesOfData.Contains(fieldName); ;
        }


        /// <summary>
        /// Get the value of the underlying data
        /// </summary>
        /// <param name="fieldName">Field or property name</param>
        /// <returns>Object, the value of the field</returns>
        public object getValue(string fieldName)
        {
            if (isExpando)
            {
                return expandoDict[fieldName];
            }
            else
            {
                PropertyInfo propertyInfo = _obj.GetType().GetProperty(fieldName);
                if (propertyInfo == null)
                {
                    FieldInfo fieldInfo = _obj.GetType().GetField(fieldName);
                    if (fieldInfo == null)
                    {
                        throw new ArgumentException($"Field or property {fieldName} does not exist.");
                    }
                    return fieldInfo.GetValue(_obj);
                }
                return propertyInfo.GetValue(_obj);
            }
        }

        /// <summary>
        /// Set a value to the underlying data
        /// </summary>
        /// <param name="fieldName">The field or property name</param>
        /// <param name="value">The value to set</param>
        /// <exception cref="ArgumentException"></exception>
        public void setValue(string fieldName, object value)
        {
            if (isExpando)
            {
                expandoDict[fieldName] = value;
            }
            else
            {
                PropertyInfo propertyInfo = _obj.GetType().GetProperty(fieldName);
                if (propertyInfo == null)
                {
                    FieldInfo fieldInfo = _obj.GetType().GetField(fieldName);
                    if (fieldInfo == null)
                    {
                        throw new ArgumentException($"Field or property {fieldName} does not exist.");
                    }
                    fieldInfo.SetValue(_obj, value);
                }
                else
                {
                    propertyInfo.SetValue(_obj, value);
                }
            }
        }

        /// <summary>
        /// Get all field names names
        /// </summary>
        /// <returns>String array with the names</returns>
        public IEnumerable<string> getFieldNames()
        {
            if (isExpando)
            {
                return expandoDict.Keys;
            }

            return propertyNamesOfData;
        }

        /// <summary>
        /// Get a List with field names that base on their datatypes, 
        /// they need quotes in value representation (eg. string).
        /// </summary>
        /// <returns>A List with field names</returns>
        private IEnumerable<string> getFieldsNeedQuotesFromDatatypes()
        {
            List<string> list = new List<string>();
            foreach (string fieldName in getFieldNames())
            {
                var value = getValue(fieldName);
                if (value == null) { continue; }
                Type type = value.GetType();
                if ((type == typeof(string)) | (type == typeof(DateTime)) | (type == typeof(DateTime?)))
                {
                    list.Add(fieldName);
                }
            }

            return list;
        }

        /// <summary>
        /// Check if a field is quotable
        /// </summary>
        /// <param name="fieldName">The field name</param>
        /// <returns>Boolean, true if it is quotable</returns>
        public bool isQuotable(string fieldName)
        {
            return fieldsNeedQuotes.Contains(fieldName);
        }

        /// <summary>
        /// Copy underlying field values to a variables class by merging with the existing values.
        /// </summary>
        /// <param name="variables">The variables object to receive the field values</param>
        public void copyTo(IvariablesContainer variables, bool doMerge = true)
        {
            if (!doMerge)
            {
                throw new NotSupportedException("Copy to variables without merge, is an unsupported feature");
            }

            foreach (string fieldName in getFieldNames())
            {
                variables.setAny(fieldName, getValue(fieldName));
            }
        }


        /// <summary>
        /// Unsupported method
        /// </summary>
        public object Clone()
        {
            throw new NotSupportedException("Cannot clone(). Cloning is an unsupported feature");
        }



        /// <summary>
        /// Create a dynamically a data type using Reflection.Emit
        /// </summary>
        /// <param name="fieldsAndValues">A dictionary with field names as key, A Tuple of the Type and Field Value as value.</param>
        /// <param name="nameingPrefix">A name prefix to the inner assembly name, module name and objectType name. Default is 'my' (myAssembly, myModule, myObject)</param>
        /// <returns>Type</returns>
        public static Type createObjectTypeUsingEmit(Dictionary<string, Tuple<Type, object>> fieldsAndValues, string nameingPrefix = "my")
        {
            string assemblyName = nameingPrefix + "Assembly";
            string name = nameingPrefix + "Module";
            string name2 = nameingPrefix + "Object";
            TypeBuilder typeBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run).DefineDynamicModule(name).DefineType(name2, TypeAttributes.Public);
            foreach (KeyValuePair<string, Tuple<Type, object>> fieldsAndValue in fieldsAndValues)
            {
                typeBuilder.DefineField(fieldsAndValue.Key, fieldsAndValue.Value.Item1, FieldAttributes.Public);
            }

            return typeBuilder.CreateType();
        }


        /// <summary>
        /// Create a dynamically an instance of a dynamic object using Reflection.Emit
        /// </summary>
        /// <param name="setValues">True if the values will be set to the instance of the class</param>
        /// <param name="fieldsAndValues">A dictionary with field names as key, A Tuple of the Type and Field Value as value.</param>
        /// <param name="nameingPrefix">A name prefix to the inner assembly name, module name and objectType name. Default is 'my' (myAssembly, myModule, myObject)</param>
        /// <returns>Type</returns>
        public static object createObjectUsingEmit(bool setValues, Dictionary<string, Tuple<Type, object>> fieldsAndValues, string nameingPrefix = "my")
        {
            object obj = Activator.CreateInstance(createObjectTypeUsingEmit(fieldsAndValues, nameingPrefix));
            if (setValues)
            {
                foreach (KeyValuePair<string, Tuple<Type, object>> fieldsAndValue in fieldsAndValues)
                {
                    obj.GetType().GetField(fieldsAndValue.Key).SetValue(obj, fieldsAndValue.Value.Item2);
                }
            }

            return obj;
        }


        /// <summary>
        /// Cast a type in a string value 
        /// </summary>
        /// <param name="type">The string containing the requested type. Only ordinary, simple types are supported</param>
        /// <param name="value">the value to be casted</param>
        /// <param name="defaultValue">Optional, the value to used if the fields value is null.</param>
        /// <returns>Object, the casted value</returns>
        /// <exception cref="NotImplementedException">Unsupported data type</exception>
        public static object castStringToType(string type, object value, object defaultValue=null)
        {
            if (value==null) return defaultValue;

            switch (type.ToLower())
            {
                case "string":
                    return value.ToString();

                case "int16":
                    Int16.TryParse(value.ToString(), out Int16 tmpi16);
                    return tmpi16;

                case "int64":
                    Int64.TryParse(value.ToString(), out Int64 tmpi64);
                    return tmpi64;

                case "int":
                    int.TryParse(value.ToString(), out int tmpi);
                    return tmpi;

                case "decimal":
                    decimal.TryParse(value.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal tmpd);
                    return tmpd;
                case "datetime":
                    DateTime.TryParse(value.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt);
                    return dt;
                default:
                    throw new NotImplementedException($"Not handled type {type}");
            }
        }


        /// <summary>
        /// Convert a Dictionary of string,object (field name, value) to ExpandoObject
        /// </summary>
        /// <param name="dictionary">The input dictionary</param>
        /// <returns>ExpandoObject</returns>
        public static ExpandoObject toExpandoObject(Dictionary<string, object> dictionary)
        {
            if (dictionary == null) return new ExpandoObject();
            var expando = new ExpandoObject();
            var expandoDict = (IDictionary<string, object>)expando;
            foreach (var kvp in dictionary)
            {
                expandoDict[kvp.Key] = kvp.Value;
            }
            return expando;
        }

        /// <summary>
        /// Convert a Dictionary of string,object (field name, value) to dynamicObject
        /// </summary>
        /// <param name="dictionary">The input dictionary</param>
        /// <returns>The dynamicObject</returns>
        public static dynamicObject toDynamicObject(Dictionary<string, object> dictionary) 
            => new dynamicObject(toExpandoObject(dictionary));


    }

}
