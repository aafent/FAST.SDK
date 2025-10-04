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
    }
}
