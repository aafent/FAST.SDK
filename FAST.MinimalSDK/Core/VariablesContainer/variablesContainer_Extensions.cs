namespace FAST.Core
{
    /// <summary>
    /// Extension methods for the InamedVariablesContainer interface
    /// </summary>
    public static class variablesContainer_Extensions
    {
        /// <summary>
        /// Extension method to create an object of type T from a json representation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vars"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T createFromJson<T>(this T vars, json json) where T : InamedVariablesContainer
        {
            return json.toObject<T>();
        }

        /// <summary>
        /// Return variable value as string, but if it is empty return the default value
        /// </summary>
        /// <param name="value">The variablesBuffer object</param>
        /// <param name="variable">The name of the variable</param>
        /// <param name="defaultValue">Optional, the default value. Default is null</param>
        /// <returns>String,the variable's value or the default value</returns>
        public static string getStringOrDefault(this variablesValue value, string variable, string defaultValue = null)
        {
            var val = value.getString(variable);
            if (string.IsNullOrWhiteSpace(val)) return defaultValue;
            return val;
        }

    }
}
