using FAST.Strings;
using FAST.Types;
using static FAST.Core.reflectionHelper;
using System.Reflection;

namespace FAST.Core
{

    /// <summary>
    /// A collection of key-value pairs with fast access by key.
    /// Is also an IvariablesContainer for easy integration with other FAST components.
    /// </summary>
    /// <typeparam name="KType">Key type</typeparam>
    /// <typeparam name="VType">Value type</typeparam>
    public class keyValuePairCollection<KType,VType> : IvariablesContainer
    {
        private List<FastKeyValuePair<KType, VType>> innerValues = null;

        /// <summary>
        /// Constructor without arguments
        /// </summary>
        /// <param name="keyValuesPair"></param>
        public keyValuePairCollection( IEnumerable<FastKeyValuePair<KType,VType>>  keyValuesPair )
        {
            this.innerValues=keyValuesPair.ToList();
        }

        /// <summary>
        /// Constructor with tuples as arguments
        /// </summary>
        /// <param name="tuples"></param>
        public keyValuePairCollection( IEnumerable<Tuple<KType,VType>> tuples)
        {
            var query = from t in tuples
                        select t;
            innerValues = query.ToList().Select(t => new FastKeyValuePair<KType, VType>(t.Item1, t.Item2))
                                        .ToList();
        }

        /// <summary>
        /// Constructor with stringsPair as arguments
        /// </summary>
        /// <param name="stringPairs"></param>
        public keyValuePairCollection(IEnumerable<stringsPair> stringPairs)
        {
            var query = from t in stringPairs
                        select t;
            innerValues = query.ToList().Select(t => new FastKeyValuePair<KType, VType>(typeConverter.convertObject<KType>(t.left), typeConverter.convertObject<VType>(t.right)))
                                        .ToList();
        }

        /// <summary>
        /// Constructor without arguments
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public VType this[KType key]
        {
            get
            {
                return getValue(key);
            }
            set
            {
                setValue(key, value);
            }
        }

        /// <summary>
        /// Get a value by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public VType getValue(KType key)
        {
            return innerValues.FirstOrDefault(i=>i.Key.Equals(key) ).Value;
        }

        /// <summary>
        /// Set a value by key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void setValue(KType key, VType value)
        {
            if ( innerValues.Any(i=>i.Key.Equals(key )))
            {
                innerValues.RemoveAll(i=>i.Key.Equals(key));
                innerValues.Add( new FastKeyValuePair<KType, VType>(key,value));
            }
        }

        /// <summary>
        /// Set a value by key or add the key-value pair if the key does not exist
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void setOrAddValue(KType key, VType value)
        {
            if (innerValues.Any(i => i.Key.Equals(key)))
            {
                setValue(key,value);
                return;
            }
            else
            {
                innerValues.Add(new FastKeyValuePair<KType, VType>(key,value) );
            }
        }

        /// <summary>
        /// Copy values to an object properties/fields with the same names as keys in the collection
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="dest"></param>
        /// <param name="commaListFieldNamesExceptions"></param>
        /// <exception cref="NotSupportedException"></exception>
        public void copyValuesTo<U>(U dest, string commaListFieldNamesExceptions=null)
        {
            if ( string.IsNullOrEmpty(commaListFieldNamesExceptions )) commaListFieldNamesExceptions=string.Empty;
            if (! string.IsNullOrEmpty(commaListFieldNamesExceptions))
            {
                commaListFieldNamesExceptions="," + commaListFieldNamesExceptions + ",";
            }

            var setDST = getFieldsAndProperties(typeof(U), propertiesChangeability.writable, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);

            foreach (MemberInfo member in setDST)
            {
                if (commaListFieldNamesExceptions.Contains("," + member.Name + ",")) continue; // it is exception

                object value = null;
                KType key = typeConverter.convertObject<KType>(member.Name);
                
                if ( ! innerValues.Any(i=>i.Key.Equals(key) )) continue; // go to next     
                value = getValue(key);

                // (v) assign the value
                switch (member.MemberType)
                {
                    case MemberTypes.Property:
                        PropertyInfo property = (PropertyInfo)member;
                        property.SetValue(dest, value, null);
                        break;

                    case MemberTypes.Field:
                        FieldInfo field = (FieldInfo)member;
                        field.SetValue(dest, value);
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }
        }

        /// <summary>
        /// Clear all key-value pairs
        /// </summary>
        public void clear()
        {
            innerValues.Clear();
        }

        /// <summary>
        /// Get all key-value pairs as List
        /// </summary>
        /// <returns></returns>
        public List<FastKeyValuePair<KType, VType>> toList()
        {
            return innerValues;
        }

        /// <summary>
        /// Create a Dictionary having as key the left value and as value the right value
        /// </summary>
        /// <returns></returns>
        public Dictionary<KType, VType> toDictionary()
        {
            return innerValues.ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Get all key-value pairs as collection of stringsPair
        /// </summary>
        /// <returns></returns>
        public IEnumerable<stringsPair> toStringPairCollection()
        {
            var query = from t in innerValues
                        select t;
            return query.ToList().Select(t => new stringsPair(
                                                    typeConverter.convertObject<string>(t.Key), 
                                                    typeConverter.convertObject<string>(t.Value)
                                                ) )
                                        .ToList();
        }

        /// <summary>
        /// Get all key-value pairs as collection of Tuples
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tuple<KType, VType>> toTuplesCollection()
        {
            var query = from t in innerValues
                        select t;
            return query.ToList().Select(t => new Tuple<KType, VType>(t.Key,t.Value)
                                                )
                                        .ToList();
        }

        /// <summary>
        /// Get a variable value as object
        /// </summary>
        /// <param name="type"></param>
        /// <param name="variable"></param>
        /// <param name="nullable"></param>
        /// <returns></returns>
        public object getAsObject(Type type, string variable, bool nullable)
        {
            KType key = typeConverter.convertObject<KType>(variable);
            VType value = getValue(key);
            return Convert.ChangeType(value,type);
        }

        /// <summary>
        /// Check if a variable exists
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public bool isVariable(string variable)
        {
            KType key = typeConverter.convertObject<KType>(variable);
            return innerValues.Any(i=>i.Key.Equals(key) );
        }

        /// <summary>
        /// Set a variable value
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        public void setAny(string variable, object value)
        {
            KType key = typeConverter.convertObject<KType>(variable);
            VType typedValue = typeConverter.convertObject<VType>(value);
            setValue(key, typedValue );
        }

    }
}
