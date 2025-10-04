namespace FAST.Core
{
    /// <summary>
    /// Elementary interface for variables containers
    /// Variable names are unknown to the container
    /// </summary>
    public interface IvariablesContainer
    {
        /// <summary>
        /// Set a variable value
        /// </summary>
        /// <param name="variable">Variable name</param>
        /// <param name="value">Variable value</param>
        void setAny(string variable, object value);

        /// <summary>
        /// Get a variable value as object
        /// </summary>
        /// <param name="type">The variable's type</param>
        /// <param name="variable">The variable name</param>
        /// <param name="nullable">True if the type is nullable</param>
        /// <returns>The value</returns>
        object getAsObject(Type type, string variable, bool nullable);

        /// <summary>
        /// Check if a variable exists
        /// </summary>
        /// <param name="variable">The variable name</param>
        /// <returns>True if the variable exists</returns>
        bool isVariable(string variable);
    }


    /// <summary>
    /// Elementary interface for variables containers.
    /// The container can report the variable names in the container
    /// </summary>
    public interface InamedVariablesContainer : IvariablesContainer
    {
        /// <summary>
        /// Get all the variable names in the container
        /// </summary>
        /// <returns>String array</returns>
        string[] getVariableNames();
    }

}
