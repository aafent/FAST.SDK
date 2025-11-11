using System.Data;

namespace FAST.Data
{
    /// <summary>
    /// Extensions of IDataRecord.
    /// </summary>
    public static class IDataRecord_nullSafeGetterExtensions
    {
        /// <summary>
        /// Get Value or a Default
        /// </summary>
        /// <param name="row">The IDataRecord</param>
        /// <param name="fieldName">The field name</param>
        /// <returns></returns>
        public static T getValueOrDefault<T>(this IDataRecord row, string fieldName)
        {
            int ordinal = row.GetOrdinal(fieldName);
            return row.getValueOrDefault<T>(ordinal);
        }

        /// <summary>
        /// Get Value or a Default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">The IDataRecord</param>
        /// <param name="ordinal">The ordinal number of the field</param>
        /// <returns></returns>
        public static T getValueOrDefault<T>(this IDataRecord row, int ordinal)
        {
            return (T)(row.IsDBNull(ordinal) ? default(T) : row.GetValue(ordinal));
        }
    }
}
