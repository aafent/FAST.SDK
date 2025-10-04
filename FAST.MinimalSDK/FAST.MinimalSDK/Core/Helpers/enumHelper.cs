using System.ComponentModel;
using System.Reflection;

namespace FAST.Core
{
    /// <summary>
    /// The enumeration helper (Enumerable, Enum)
    /// </summary>
    public class enumHelper
    {
        /// <summary>
        /// Convert an Enumeration to list
        /// </summary>
        /// <typeparam name="T">The Enumeration</typeparam>
        /// <returns>A list of type T</returns>
        public static List<T> toList<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        public static IEnumerable<T> repeat<T>(IEnumerable<T> source, int times)
        {
            source = source.ToArray();
            return Enumerable.Range(0, times).SelectMany(_ => source);
        }

        /// <summary>
        /// Return a random item for an IEnumerable T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns>T.</returns>
        public static T pickRandom<T>(IEnumerable<T> source)
        {
            return pickRandom(source,1).Single();
        }

        /// <summary>
        /// Return a random item for an IEnumerable T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="count">The count.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public static IEnumerable<T> pickRandom<T>(IEnumerable<T> source, int count)
        {
            return shuffle(source).Take(count);
        }

        /// <summary>
        /// Return source ordered by a new Guid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public static IEnumerable<T> shuffle<T>(IEnumerable<T> source)
        {
            return source.OrderBy(x => Guid.NewGuid());
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerationValue">The enumeration value.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="ArgumentException">EnumerationValue must be of Enum type - enumerationValue</exception>
        /// <exception cref="System.ArgumentException">EnumerationValue must be of Enum type - enumerationValue</exception>
        public static string getDescription<T>(T enumerationValue) where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }

        /// <summary>
        /// Gets localized description
        /// </summary>
        /// <param name="value">The enum.</param>
        /// <returns>System.String.</returns>
        public static string getLocalizedDescription(Enum value)
        {
            if (value == null)
                return null;

            string description = value.ToString();

            FieldInfo fieldInfo = value.GetType().GetField(description);
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Any())
                return attributes[0].Description;

            return description;
        }

        /// <summary>
        /// Extension method to return an enum value of type T for the given string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        /// <exception cref="Exception">T must be an Enumeration type.</exception>
        public static T toEnum<T>(string value) where T : struct, Enum
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new Exception("T must be an Enumeration type.");
            }

            return Enum.TryParse(value, out T val) ? val : default;
        }

        /// <summary>
        /// Extension method to return an enum value of type T for the given int.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        /// <exception cref="Exception">T must be an Enumeration type.</exception>
        public static T toEnum<T>(int value)
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new Exception("T must be an Enumeration type.");
            }

            return (T)Enum.ToObject(enumType, value);
        }

        /// <summary>
        /// Empty enumerable if it is null
        /// </summary>
        /// <typeparam name="T">The input type</typeparam>
        /// <param name="source">The input source</param>
        /// <returns>Enumeration of the input Type. The source of empty</returns>
        public static IEnumerable<T> emptyIfNull<T>(IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }
    }
}
