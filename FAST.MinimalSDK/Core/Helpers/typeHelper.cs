namespace FAST.Core
{
    public static class typeHelper
    {
        private static readonly HashSet<Type> simpleTypes = new HashSet<Type>
        {
            typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int),
            typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double),
            typeof(decimal), typeof(bool), typeof(string), typeof(char), typeof(DateTime),
            typeof(Guid), typeof(TimeSpan)
        };

        /// <summary>
        /// Check if a type is a simple typoe
        /// </summary>
        /// <param name="type">The input type</param>
        /// <returns>True if it is</returns>
        public static bool isSimpleType(Type type)
        {
            // Check if the type is a simple type directly
            if (simpleTypes.Contains(type))
            {
                return true;
            }

            // Check if the type is an enum
            if (type.IsEnum)
            {
                return true;
            }

            // Check if the type is a nullable type and its underlying type is simple
            Type underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                return isSimpleType(underlyingType);
            }

            // Check if the type is a value type (structs are value types but not simple by default)
            if (type.IsValueType)
            {
                return true;
            }

            // All other types are considered complex
            return false;
        }


        /// <summary>
        /// Check if type is in a collection of types
        /// </summary>
        /// <param name="typeToCheck">The type to check for it</param>
        /// <param name="type">The collection of types (one item)</param>
        /// <returns>True if found in the collection</returns>
        public static bool isAnyOf(Type typeToCheck, Type type)
        {
            return typeToCheck == type;
        }
        /// <summary>
        /// Check if type is in a collection of types
        /// </summary>
        /// <param name="typeToCheck">The type to check for it</param>
        /// <param name="type1">The collection of types (1st item)</param>
        /// <param name="type2">The collection of types (2nd item)</param>
        /// <returns>True if found in the collection</returns>
        public static bool isAnyOf(Type typeToCheck, Type type1, Type type2)
        {
            if (typeToCheck == type1) return true;
            if (typeToCheck == type2) return true;
            return false;
        }
        /// <summary>
        /// Check if type is in a collection of types
        /// </summary>
        /// <param name="typeToCheck">The type to check for it</param>
        /// <param name="types">The collection of types (many items)</param>
        /// <returns>True if found in the collection</returns>
        public static bool isAnyOf(Type typeToCheck, params Type[] types)
        {
            foreach(var itemToCheck in types)
            {
                if ( typeToCheck == itemToCheck) return true;
            }
            return false;
        }

        /// <summary>
        /// Check if the type is Numeric
        /// </summary>
        /// <param name="typeToCheck">Type to Check</param>
        /// <returns>bool, true is type is numeric</returns>
        public static bool isNumeric(Type typeToCheck)
        {
            return isAnyOf(typeToCheck, typeof(int), typeof(Int16), typeof(Int32), typeof(Int64),
                                        typeof(sbyte), typeof(Byte), typeof(long), typeof(double), 
                                        typeof(Decimal)
                                        );
        }

        /// <summary>
        /// Check if the type contains a Date
        /// </summary>
        /// <param name="typeToCheck">Type to Check</param>
        /// <returns>bool, true is type contains date</returns>
        public static bool isDate(Type typeToCheck)
        {
            return isAnyOf(typeToCheck, typeof(DateTime), typeof(DateOnly), typeof(DateTime?), typeof(DateOnly?) );
        }

        /// <summary>
        /// Check if the type contains a Time
        /// </summary>
        /// <param name="typeToCheck">Type to Check</param>
        /// <returns>bool, true is type contains Time</returns>
        public static bool isTime(Type typeToCheck)
        {
            return isAnyOf(typeToCheck, typeof(DateTime), typeof(DateTime?), typeof(TimeOnly), typeof(TimeOnly?));
        }

        /// <summary>
        /// Get the type of the input or if it is null get Object as type
        /// </summary>
        /// <param name="input">The input instance or null</param>
        /// <returns>Type, the type of the input</returns>
        public static Type getTypeOrDefault(object input)
        {
            if (input == null)
            {
                return typeof(object);
            }
            else
            {
                return input.GetType();
            }

        }

    }
}