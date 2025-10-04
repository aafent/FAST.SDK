using FAST.Strings;
using FAST.Types;
using static FAST.Core.reflectionHelper;
using System.Reflection;

namespace FAST.Core
{
    public class keyValuePairCollection<KType,VType> : IvariablesContainer
    {
        private List<FastKeyValuePair<KType, VType>> innerValues = null;

        public keyValuePairCollection( IEnumerable<FastKeyValuePair<KType,VType>>  keyValuesPair )
        {
            this.innerValues=keyValuesPair.ToList();
        }
        public keyValuePairCollection( IEnumerable<Tuple<KType,VType>> tuples)
        {
            var query = from t in tuples
                        select t;
            innerValues = query.ToList().Select(t => new FastKeyValuePair<KType, VType>(t.Item1, t.Item2))
                                        .ToList();
        }
        public keyValuePairCollection(IEnumerable<stringsPair> stringPairs)
        {
            var query = from t in stringPairs
                        select t;
            innerValues = query.ToList().Select(t => new FastKeyValuePair<KType, VType>(typeConverter.convertObject<KType>(t.left), typeConverter.convertObject<VType>(t.right)))
                                        .ToList();
        }


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


        public VType getValue(KType key)
        {
            return innerValues.FirstOrDefault(i=>i.Key.Equals(key) ).Value;
        }

        public void setValue(KType key, VType value)
        {
            if ( innerValues.Any(i=>i.Key.Equals(key )))
            {
                innerValues.RemoveAll(i=>i.Key.Equals(key));
                innerValues.Add( new FastKeyValuePair<KType, VType>(key,value));
            }
        }

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

        public void clear()
        {
            innerValues.Clear();
        }


        public List<FastKeyValuePair<KType, VType>> toList()
        {
            return innerValues;
        }
        public Dictionary<KType, VType> toDictionary()
        {
            return innerValues.ToDictionary(x => x.Key, x => x.Value);
        }
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
        public IEnumerable<Tuple<KType, VType>> toTuplesCollection()
        {
            var query = from t in innerValues
                        select t;
            return query.ToList().Select(t => new Tuple<KType, VType>(t.Key,t.Value)
                                                )
                                        .ToList();
        }

        public object getAsObject(Type type, string variable, bool nullable)
        {
            KType key = typeConverter.convertObject<KType>(variable);
            VType value = getValue(key);
            return Convert.ChangeType(value,type);
        }
        public bool isVariable(string variable)
        {
            KType key = typeConverter.convertObject<KType>(variable);
            return innerValues.Any(i=>i.Key.Equals(key) );
        }
        public void setAny(string variable, object value)
        {
            KType key = typeConverter.convertObject<KType>(variable);
            VType typedValue = typeConverter.convertObject<VType>(value);
            setValue(key, typedValue );
        }

    }
}
