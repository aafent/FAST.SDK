namespace FAST.Core
{
    /// <summary>
    /// Attribute to define synonyms for a property or field in a class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
    [Serializable]
    public class synonymsAttribute : Attribute
    {

        /// <summary>
        /// Constructor to initialize the synonyms attribute with an array of column names.
        /// </summary>
        /// <param name="columnNames"></param>
        public synonymsAttribute(params string[] columnNames)
        {
            this.columnNames = columnNames;
        }

        /// <summary>
        /// Array of synonyms for the property or field.
        /// </summary>
        public string[] columnNames = null;
    }

}
