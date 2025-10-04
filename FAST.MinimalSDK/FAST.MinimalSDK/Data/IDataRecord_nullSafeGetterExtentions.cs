using System.Data;

namespace FAST.Data
{
    /// <summary>
    /// Extentions of IDataRecord.
    /// </summary>
    public static class IDataRecord_nullSafeGetterExtentions
    {
        /// <summary>
        /// Get Value or a Defult
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
        /// Get Value or a Defult
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
