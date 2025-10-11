using FAST.Strings;

namespace FAST.API
{
    /// <summary>
    /// Helper to decode sensitive configuration values
    /// </summary>
    public static class sensitiveValue
    {
        /// <summary>
        /// Decode a connection string that is encrypted by the FAST Services
        /// </summary>
        /// <param name="sensitiveValue">The encrypted value</param>
        /// <returns>string, the connection string</returns>
        public static string connectionString(string sensitiveValue)
        {
            return converters.configValueToSensitiveValue("$connectionString$", sensitiveValue);
        }

        [Obsolete("use connectionString() with one argument only")]
        public static string connectionString(string readersPassword, string sensitiveValue)
        {
            return connectionString(sensitiveValue);
        }
    }
}
