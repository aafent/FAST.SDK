using System.Reflection;
using System.Linq.Expressions;
using System.Data;
using System.Globalization;
using FAST.Logging;

namespace FAST.Core
{

    /// <summary>
    /// Reflection Helper
    /// </summary>
    public class reflectionHelper
    {
        private static List<Type> _simpleTypes = null;

        /// <summary>
        /// A collection with the simple types
        /// </summary>
        public static List<Type> simpleTypes
        {
            get
            {
                if (_simpleTypes == null)
                {
                    _simpleTypes = new List<Type>();
                    _simpleTypes.Add(typeof(Boolean));
                    _simpleTypes.Add(typeof(Byte));
                    _simpleTypes.Add(typeof(SByte));
                    _simpleTypes.Add(typeof(Int16));
                    _simpleTypes.Add(typeof(UInt16));
                    _simpleTypes.Add(typeof(Int32));
                    _simpleTypes.Add(typeof(UInt32));
                    _simpleTypes.Add(typeof(Boolean));
                    _simpleTypes.Add(typeof(Int64));
                    _simpleTypes.Add(typeof(UInt64));
                    _simpleTypes.Add(typeof(IntPtr));
                    _simpleTypes.Add(typeof(UIntPtr));
                    _simpleTypes.Add(typeof(Char));
                    _simpleTypes.Add(typeof(Double));
                    _simpleTypes.Add(typeof(Single));
                    _simpleTypes.Add(typeof(String));
                }
                return _simpleTypes;
            }
        }

        //public enum propertiesChangability { any = 0, readable = 1, writable = 2 }

        /// <summary>
        /// Changeability of properties
        /// </summary>
        public enum propertiesChangeability { any = 0, readable = 1, writable = 2 }


        #region (+) Property(ies), Field(s) & Members

        /// <summary>
        /// Get all public properties of a type
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>Array of properties</returns>
        public static PropertyInfo[] getPublicProperties(Type type )
        {
            return getProperties(type, propertiesChangeability.any,
                                            BindingFlags.FlattenHierarchy
                                        |   BindingFlags.Public
                                        |   BindingFlags.Instance);
        }

        /// <summary>
        /// Get properties of a type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="changeability"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static PropertyInfo[] getProperties(Type type, propertiesChangeability changeability, BindingFlags bindingFlags)
        {
            if (type.IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();

                var considered = new List<Type>();
                var queue = new Queue<Type>();
                considered.Add(type);
                queue.Enqueue(type);
                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface)) continue;

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetProperties( bindingFlags );

                    var newPropertyInfos = typeProperties
                        .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            switch (changeability)
            {
                case propertiesChangeability.any:
                    return type.GetProperties(bindingFlags);

                case propertiesChangeability.readable:
                    return type.GetProperties(bindingFlags).Where(p => p.CanRead).ToArray();

                case propertiesChangeability.writable:
                    return type.GetProperties(bindingFlags).Where(p => p.CanRead).ToArray();

                default:
                    throw new NotImplementedException();
            }

        }

        /// <summary>
        /// Get fields and properties of a type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="changeability"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static MemberInfo[] getFieldsAndProperties(Type type, propertiesChangeability changeability, BindingFlags bindingFlags)
        {
            MemberInfo[] fields = type.GetFields(bindingFlags);
            MemberInfo[] prop = getProperties(type, changeability, bindingFlags);
            MemberInfo[] union = fields.ToList().Union( prop.ToList() ).ToArray();
            return union ;
        }

        [Obsolete("Change getFieldsAndProperites() with getFieldsAndProperties()")]
        public static MemberInfo[] getFieldsAndProperites(Type type, propertiesChangeability changeability, BindingFlags bindingFlags) => getFieldsAndProperties(type, changeability, bindingFlags);
        

        public static string getMemberName<T>(Expression<Func<T>> memberExpression)
        {
            MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
            return expressionBody.Member.Name;
        }

        /// <summary>
        /// Get value from an instance of a given member
        /// </summary>
        /// <param name="memberInfo">the member (property or field)</param>
        /// <param name="forObject">the instance to the object to get the value</param>
        /// <returns>the value</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static object getMemberValue(MemberInfo memberInfo, object forObject)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).GetValue(forObject);
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).GetValue(forObject);
                default:
                    throw new NotImplementedException();
            }
        }

        [Obsolete("Replace the method getValue() with the getMemberValue()")]
        public static object getValue(MemberInfo memberInfo, object forObject) => 
            getMemberValue(memberInfo, forObject);
        

        /// <summary>
        /// Sets the value of a property or field (by name) on the given object instance.
        /// Returns the old value.
        /// </summary>
        /// <param name="instance">The object instance to modify.</param>
        /// <param name="name">The name of the property or field.</param>
        /// <param name="value">The new value to set.</param>
        /// <returns>The old value of the property or field.</returns>
        public static object setMemberValue(object instance, string name, object value)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));

            Type type = instance.GetType();

            // Try property first
            PropertyInfo prop = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (prop != null && prop.CanWrite)
            {
                object oldValue = prop.GetValue(instance);
                prop.SetValue(instance, value);
                return oldValue;
            }

            // Try field
            FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                object oldValue = field.GetValue(instance);
                field.SetValue(instance, value);
                return oldValue;
            }

            throw new ArgumentException($"No writable property or field named '{name}' found on type '{type.FullName}'.");
        }


        /// <summary>
        /// Sets the value of a property on an object using its string name.
        /// Handles basic type conversions for common types (strings, numbers, bools, enums).
        /// </summary>
        /// <typeparam name="TObject">The type of the object whose property is being set.</typeparam>
        /// <param name="obj">The instance of the object to modify.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The new value to assign to the property.</param>
        /// <param name="culture">Optional: The culture to use for type conversions. Defaults to InvariantCulture.</param>
        /// <returns>True if the property was successfully set, false otherwise.</returns>
        public static bool trySetPropertyValue<TObject>(TObject obj, string propertyName, object value, IFormatProvider culture = null) 
            where TObject : class
        {
            if (obj == null)
            {
                fastLogger.debug("Error: Object cannot be null.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                fastLogger.debug("Error: Property name cannot be null or empty.");
                return false;
            }

            // Use InvariantCulture for consistent conversions by default
            culture ??= CultureInfo.InvariantCulture;

            Type objType = typeof(TObject);
            PropertyInfo propertyInfo = objType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

            if (propertyInfo == null)
            {
                fastLogger.debug($"Error: Property '{propertyName}' not found on type '{objType.Name}'.");
                return false;
            }

            if (!propertyInfo.CanWrite)
            {
                fastLogger.debug($"Error: Property '{propertyName}' on type '{objType.Name}' does not have a public setter.");
                return false;
            }

            // Get the type of the property
            Type targetPropertyType = propertyInfo.PropertyType;

            // Handle Nullable types: get the underlying type if it's nullable
            // e.g., if targetPropertyType is Nullable<int>, get int.
            Type underlyingType = Nullable.GetUnderlyingType(targetPropertyType);
            if (underlyingType != null)
            {
                targetPropertyType = underlyingType;
            }

            object convertedValue = null;
            bool conversionSuccess = true;

            try
            {
                // If the incoming value is null or an empty string, set it to null for reference types and nullable value types
                if (value == null || (value is string s && string.IsNullOrEmpty(s)))
                {
                    if (targetPropertyType.IsValueType && underlyingType == null) // Non-nullable value type
                    {
                        // Cannot assign null to a non-nullable value type (e.g., int, bool)
                        fastLogger.debug($"Error: Cannot assign null/empty string to non-nullable value type property '{propertyName}' ({propertyInfo.PropertyType.Name}).");
                        return false;
                    }
                    convertedValue = null; // Assign null to reference types or nullable value types
                }
                // Handle specific conversions for Enums
                else if (targetPropertyType.IsEnum)
                {
                    if (value is string enumString)
                    {
                        convertedValue = Enum.Parse(targetPropertyType, enumString, true); // true for ignoreCase
                    }
                    else if (value.GetType() == typeof(int))
                    {
                        convertedValue = Enum.ToObject(targetPropertyType, value);
                    }
                    else
                    {
                        fastLogger.debug($"Error: Cannot convert value of type '{value.GetType().Name}' to enum type '{targetPropertyType.Name}'.");
                        conversionSuccess = false;
                    }
                }
                // General type conversion using Convert.ChangeType
                else
                {
                    // Convert the value to the property's type
                    convertedValue = Convert.ChangeType(value, targetPropertyType, culture);
                }
            }
            catch (InvalidCastException ex)
            {
                fastLogger.debug($"Error: Cannot cast value of type '{value?.GetType().Name ?? "null"}' to property type '{propertyInfo.PropertyType.Name}' for property '{propertyName}'. Details: {ex.Message}");
                conversionSuccess = false;
            }
            catch (FormatException ex)
            {
                fastLogger.debug($"Error: Invalid format for converting '{value}' to property type '{propertyInfo.PropertyType.Name}' for property '{propertyName}'. Details: {ex.Message}");
                conversionSuccess = false;
            }
            catch (Exception ex) // Catch any other conversion errors
            {
                fastLogger.debug($"Error during conversion for property '{propertyName}': {ex.Message}");
                conversionSuccess = false;
            }

            if (!conversionSuccess)
            {
                return false;
            }

            try
            {
                // Set the property value
                propertyInfo.SetValue(obj, convertedValue, null);
                return true;
            }
            catch (TargetInvocationException ex)
            {
                // This catches exceptions thrown by the property's setter itself
                fastLogger.debug($"Error: Property setter for '{propertyName}' threw an exception. Inner Exception: {ex.InnerException?.Message ?? ex.Message}");
                return false;
            }
            catch (ArgumentException ex)
            {
                // This might catch type mismatch if ChangeType didn't cover it fully, or other argument issues
                fastLogger.debug($"Error: Argument mismatch when setting property '{propertyName}'. Details: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                fastLogger.debug($"Error setting property '{propertyName}': {ex.Message}");
                return false;
            }
        }

        #endregion (+) Property(ies), Field(s) & Members


        #region (+) Copy Values
        /// <summary>
        /// Copy all values of members with same name
        /// </summary>
        /// <typeparam name="T">Type of Source</typeparam>
        /// <typeparam name="U">Type of Destination</typeparam>
        /// <param name="source">Source</param>
        /// <param name="dest">Destination</param>
        public static void copyValues<T, U>(T source, U dest)
        {
            copyValues(source, dest, false);
        }

        /// <summary>
        /// Copy all values of members with same name, if the source is non empty
        /// </summary>
        /// <typeparam name="T">Type of Source</typeparam>
        /// <typeparam name="U">Type of Destination</typeparam>
        /// <param name="source">Source</param>
        /// <param name="dest">Destination</param>
        public static void copyNonEmptyValues<T, U>(T source, U dest)
        {
            copyValues(source, dest, true);
        }
        private static void copyValues<T, U>(T source, U dest, bool nonEmptyOnly)
        {
            var setSRC = getFieldsAndProperties(typeof(T), propertiesChangeability.readable, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
            var setDST = getFieldsAndProperties(typeof(U), propertiesChangeability.writable, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);

            foreach (MemberInfo member in setDST)
            {
                MemberInfo sourceMember = null;
                object value = null;

                // (v) get the source member
                switch (member.MemberType)
                {
                    case MemberTypes.Property:
                        PropertyInfo property = (PropertyInfo)member;
                        // todo: to make a type match, not only name
                        //sourceMember = setSRC.First((p) => p.Name == property.Name && property.PropertyType.IsAssignableFrom(p.GetType())); 
                        sourceMember = setSRC.FirstOrDefault((p) => p.Name == property.Name);
                        break;

                    case MemberTypes.Field:
                        FieldInfo field = (FieldInfo)member;
                        // todo: to make a type match, not only name
                        //sourceMember = setSRC.First(f => f.Name == field.Name && field.FieldType.IsAssignableFrom(f.GetType()));
                        sourceMember = setSRC.FirstOrDefault(f => f.Name == field.Name);
                        break;

                    default:
                        throw new NotSupportedException();
                }
                if (sourceMember == null) { continue; }

                // (v) get the value
                switch (sourceMember.MemberType)
                {
                    case MemberTypes.Property:
                        PropertyInfo property = (PropertyInfo)sourceMember;
                        value = property.GetValue(source, null);
                        break;

                    case MemberTypes.Field:
                        FieldInfo field = (FieldInfo)sourceMember;
                        value = field.GetValue(source);
                        break;

                    default:
                        throw new NotSupportedException();
                }


                if (nonEmptyOnly)
                {
                    if (value == null) continue;
                    if (value is string)
                    {
                        if (string.IsNullOrEmpty(value.ToString())) continue;
                    }
                }


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
        /// Copy all values of members with same name, and return the destination
        /// is used to create dot notated expressions.
        /// </summary>
        /// <typeparam name="T">Type of Source</typeparam>
        /// <typeparam name="U">Type of Destination</typeparam>
        /// <param name="source">Source</param>
        /// <param name="dest">Destination</param>
        /// <returns>the destination</returns>
        public static U copyAndReturnValues<T, U>(T source, U dest)
        {
            copyValues(source, dest,false);
            return dest;
        }

        #endregion (+) Copy Values

        #region (+) Replace values

        /// <summary>
        /// Replace empty strings with a given value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="replacement"></param>
        /// <param name="applyTrim"></param>
        public static void replaceEmptyStrings<T>(List<T> list, string replacement, bool applyTrim=false)
        {
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo p in properties)
            {
                // Only work with strings
                if (p.PropertyType != typeof(string)) { continue; }

                // If not writable then cannot null it; if not readable then cannot check it's value
                if (!p.CanWrite || !p.CanRead) { continue; }

                MethodInfo mget = p.GetGetMethod(false);
                MethodInfo mset = p.GetSetMethod(false);

                // Get and set methods have to be public
                if (mget == null) { continue; }
                if (mset == null) { continue; }

                foreach (T item in list)
                {
                    var value = (string)p.GetValue(item, null);
                    if (applyTrim) { if (value != null) { value = value.Trim(); } }
                    if (string.IsNullOrEmpty(value))
                    {
                        p.SetValue(item, replacement, null);
                    }
                }
            }


            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo f in fields)
            {
                // Only work with strings
                if (f.FieldType != typeof(string)) { continue; }

                foreach (T item in list)
                {
                    var value = (string)f.GetValue(item);
                    if (applyTrim) { if (value != null) { value = value.Trim(); } }
                    if (string.IsNullOrEmpty(value))
                    {
                        f.SetValue(item, replacement);
                    }
                }

            }

        }


        /// <summary>
        /// Replace empty strings with a given value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instanceToReplace"></param>
        /// <param name="replacement"></param>
        /// <param name="applyTrim"></param>
        public static void replaceEmptyStrings<T>(T instanceToReplace, string replacement, bool applyTrim = false)
        {
            List<T> list = new List<T>();
            list.Add(instanceToReplace);
            replaceEmptyStrings<T>(list, replacement,applyTrim);
            return;
        }


        /// <summary>
        /// Replace a string with another string in all string properties and fields
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instanceToReplace"></param>
        /// <param name="search"></param>
        /// <param name="replacement"></param>
        public static void replaceString<T>(T instanceToReplace, string search, string replacement)
        {
            List<T> list = new List<T>();
            list.Add(instanceToReplace);
            replaceString<T>(list, search, replacement);
            return;
        }

        /// <summary>
        /// Replace a string with another string in all string properties and fields
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="search"></param>
        /// <param name="replacement"></param>
        public static void replaceString<T>(List<T> list, string search, string replacement)
        {
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo p in properties)
            {
                // Only work with strings
                if (p.PropertyType != typeof(string)) { continue; }

                // If not writable then cannot null it; if not readable then cannot check it's value
                if (!p.CanWrite || !p.CanRead) { continue; }

                MethodInfo mget = p.GetGetMethod(false);
                MethodInfo mset = p.GetSetMethod(false);

                // Get and set methods have to be public
                if (mget == null) { continue; }
                if (mset == null) { continue; }

                foreach (T item in list)
                {
                    string value = (string)p.GetValue(item, null);
                    value = value.Replace(search, replacement);
                    p.SetValue(item, value, null);
                }
            }


            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo f in fields)
            {
                // Only work with strings
                if (f.FieldType != typeof(string)) { continue; }

                foreach (T item in list)
                {
                    string value = (string)f.GetValue(item);
                    value = value.Replace(search, replacement);
                    f.SetValue(item, value);
                }

            }

        }

        #endregion (+) Replace values

        #region (+) is and check methods

        /// <summary>
        /// Check if a type is a simple type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool isSimpleType(Type type)
        {
            return simpleTypes.Contains(type);

        }

        /// <summary>
        /// Check if a property is a collection
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool isCollection(PropertyInfo property)
        {
            return property.PropertyType.GetInterface(typeof(IEnumerable<>).FullName) != null;
        }

        /// <summary>
        /// Check if a field is a collection
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static bool isCollection(FieldInfo field)
        {
            return field.FieldType.GetInterface(typeof(IEnumerable<>).FullName) != null;
        }

        /// <summary>
        /// Check if a type is a collection
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool isCollection(Type type)
        {
            return type.GetInterface(typeof(IEnumerable<>).FullName) != null;
        }

        /// <summary>
        /// Check if a type is nullable
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool isNullable(Type type)
        {
            if (Nullable.GetUnderlyingType(type) != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if the input type contains any of the given interfaces
        /// </summary>
        /// <param name="typeToCheck">Type to check</param>
        /// <param name="interfaces">Collection with interfaces to check</param>
        /// <returns>true or false</returns>
        public static bool containsAnyOfTheInterfaces(Type typeToCheck, params string[] interfaces)
        {
            foreach (var item in interfaces)
            {
                if (typeToCheck.GetInterface(item) != null) return true;
            }
            return false;
        }

        /// <summary>
        /// Check if a input type has a specific ancestor 
        /// </summary>
        /// <param name="typeToExamine"></param>
        /// <param name="lookupForAncenstor"></param>
        /// <returns>true or false</returns>
        public static bool isInheritedFrom(Type typeToExamine, Type lookupForAncestor)
        {
            var baseType = typeToExamine.BaseType;
            if (baseType == null) return false;

            if (baseType.IsGenericType
                    && baseType.GetGenericTypeDefinition() == lookupForAncestor)
                return true;

            return isInheritedFrom(baseType, lookupForAncestor);
        }


        /// <summary>
        /// Check if the given class contains any of the base type
        /// Usefull when we have a type in hand and wish to know if something is true of any of its base types. 
        /// </summary>
        /// <param name="type">The type to examine</param>
        /// <param name="predicate">Predicate to filter the types</param>
        /// <returns>true or false</returns>
        public static bool containsAnyBaseType(Type type, Func<Type, bool> predicate)
        {
            return getBaseTypes(type).Any(predicate);
        }

        /// <summary>
        /// Check if the given class contains any of the base type
        /// Usefull when we have a type in hand and wish to know if something is true of any of its base types. 
        /// </summary>
        /// <param name="type">the type to examine</param>
        /// <returns>true or false</returns>
        public static bool containsAnyBaseType(Type type)
        {
            return getBaseTypes(type).Any();
        }


        /// <summary>
        /// Check if a particular type is a particular generic
        /// </summary>
        /// <param name="type">type to check</param>
        /// <param name="generic">the generic type</param>
        /// <returns></returns>
        public static bool isParticularGeneric(Type type, Type generic)
        {
            if (generic == null) return false;
            return type.IsGenericType && type.GetGenericTypeDefinition() == generic;
        }


        #endregion (+) is and check methods

        #region (+) Attributes

        /// <summary>
        ///     Retrieves all Members decorated with the specific attribute
        ///     members are both attributes and fields
        ///     ex:  var fields = fieldsAndProperiteWithAttribute<MockupAttribute>(request);
        /// </summary>
        /// <typeparam name="TAttribute">The requested attribute</typeparam>
        /// <param name="model">The input model to retrieve the members </param>
        /// <returns>Array with the members found declared with the specific attribute</returns>
        public static MemberInfo[] fieldsAndProperiteWithAttribute<TAttribute>(object model)
                                                        where TAttribute : Attribute
        {
            List<MemberInfo> result = new();

            var items = reflectionHelper.getFieldsAndProperties(model.GetType(),
                                        reflectionHelper.propertiesChangeability.readable,
                                          BindingFlags.FlattenHierarchy
                                        | BindingFlags.Public
                                        | BindingFlags.Instance);
            foreach (var item in items)
            {
                object[] attribObject = item.GetCustomAttributes(typeof(TAttribute), false);
                TAttribute attrib = null;
                if (attribObject.Any()) { attrib = (TAttribute)attribObject.First(); }
                if (attrib == null) { continue; }

                result.Add(item);
            }
            return result.ToArray();
        }
    
        /// <summary>
        /// Returns an instance to the attribute of a member (field or property)
        /// </summary>
        /// <typeparam name="TAttribute">The attribute</typeparam>
        /// <param name="field">the requested member </param>
        /// <returns>The instance of the attribute</returns>
        public static TAttribute attributeInstance<TAttribute>(MemberInfo field) 
                                                                        where TAttribute : Attribute
        {
            object[] attribObject = field.GetCustomAttributes(typeof(TAttribute), false);
            TAttribute attrib = null;
            if (attribObject.Any()) { attrib = (TAttribute)attribObject.First(); }

            return attrib;
        }

        /// <summary>
        /// Get a value from an attribute of a type
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="type"></param>
        /// <param name="valueSelector"></param>
        /// <returns></returns>
        public static TValue getAttributeValue<TAttribute, TValue>(
            Type type,
            Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            var att = type.GetCustomAttributes(
                typeof(TAttribute), true
            ).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return valueSelector(att);
            }
            return default(TValue);
        }


        #endregion (+) Attributes

        #region (+) instances and types

        /// <summary>
        /// Get the underlying type of a member
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Type underlyingType(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    throw new ArgumentException("Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo");
            }
        }

        /// <summary>
        /// Get the name of a type, if it is nullable then return the underlying type name
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string getTypeName(Type type)
        {
            var nullableType = Nullable.GetUnderlyingType(type);

            bool isNullableType = nullableType != null;

            if (isNullableType)
                return nullableType.Name;
            else
                return type.Name;
        }

        /// <summary>
        /// Get base types
        /// </summary>
        /// <param name="type">The type to examine</param>
        /// <returns>A collection with the base types</returns>
        public static IEnumerable<Type> getBaseTypes(Type type)
        {
            Type t = type;
            while (true)
            {
                t = t.BaseType;
                if (t == null) break;
                yield return t;
            }
        }


        /// <summary>
        /// Get an instance of an object full qualified name
        /// </summary>
        /// <param name="fullyQualifiedName"></param>
        /// <returns>returns an instance of the type object</returns>
        public static object getInstance(string fullyQualifiedName)
        {
            Type t = Type.GetType(fullyQualifiedName);
            return Activator.CreateInstance(t);
        }

        /// <summary>
        /// Get an instance of an object full qualified name
        /// </summary>
        /// <typeparam name="T">The object`s type</typeparam>
        /// <param name="fullyQualifiedName"></param>
        /// <returns>returns an instance of the type T</returns>
        public static T getInstance<T>(string fullyQualifiedName)
        {
            Type t = Type.GetType(fullyQualifiedName);
            return (T)Activator.CreateInstance(t);
        }

        /// <summary>
        /// Get an instance of an object full qualified name
        /// </summary>
        /// <typeparam name="T">The object`s type</typeparam>
        /// <returns></returns>
        public static T getInstance<T>()
        {
            Type t = Type.GetType(nameof(T));
            return (T)Activator.CreateInstance(t);
        }


        /// <summary>
        /// Check if a type is numeric type
        /// </summary>
        /// <param name="type">the type to check</param>
        /// <returns>True if it is numeric type</returns>
        public static bool isNumericType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Check if an object represents a numeric type
        /// </summary>
        /// <param name="type">the type to check</param>
        /// <returns>True if it is numeric type</returns>
        public static bool isNumericType(object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        #endregion (+) instances and types


        /// <summary>
        /// Create a datatable from a class
        /// </summary>
        /// <param name="dataClass"></param>
        /// <returns></returns>
        public static DataTable toDataTable(Type dataClass)
        {
            DataTable return_Datatable = new DataTable();
            foreach (PropertyInfo info in dataClass.GetProperties())
            {
                return_Datatable.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }
            foreach (var info in dataClass.GetFields())
            {
                return_Datatable.Columns.Add(new DataColumn(info.Name, info.FieldType));
            }
            return return_Datatable;
        }


        /// <summary>
        /// Get the time of build(link) of the source code
        /// Example: var linkTimeLocal = GetLinkerTime(Assembly.GetExecutingAssembly());
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static DateTime getLinkerTime(Assembly assembly, TimeZoneInfo target = null)
        {
            //var linkTimeLocal = GetLinkerTime(Assembly.GetExecutingAssembly());

            var filePath = assembly.Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;

            var buffer = new byte[2048];

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                stream.Read(buffer, 0, 2048);

            var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

            var tz = target ?? TimeZoneInfo.Local;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

            return localTime;
        }

        /// <summary>
        /// Return Func<> of a given method
        /// eg:  getFunc<bool>(myobj,theMethod) returns a Func<bool> type
        /// </summary>
        /// <typeparam name="FType">Type type of Func</typeparam>
        /// <param name="obj">instance of an object</param>
        /// <param name="methodInfo">the method info class</param>
        /// <returns>a Func reference</returns>
        public static Func<FType> getFunc<FType>(object obj, MethodInfo methodInfo)
        {
            var xRef = Expression.Constant(obj);
            var callRef = Expression.Call(xRef, methodInfo);
            var lambda = (Expression<Func<FType>>)Expression.Lambda(callRef);

            return lambda.Compile();
        }


    }
}
