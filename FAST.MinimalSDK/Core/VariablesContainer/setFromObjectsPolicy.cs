namespace FAST.Core
{
    /// <summary>
    /// Defines the policy for setting values from objects.
    /// </summary>
    public class setFromObjectsPolicy
    {
        /// <summary>
        /// If true, null values will be ignored when setting values from objects.
        /// </summary>
        public bool ignoreNulls = true;

        /// <summary>
        /// If true, empty strings will be ignored when setting values from objects.
        /// </summary>
        public bool ignoreEmptyStrings = true;

        /// <summary>
        /// If true, zero numeric values will be ignored when setting values from objects.
        /// </summary>
        public bool ignoreZeroNumerics = true;


        /// <summary>
        /// Creates a new instance of setFromObjectsPolicy with default settings.
        /// </summary>
        public static setFromObjectsPolicy dbData
        {
            get
            {
                return new setFromObjectsPolicy();
            }
        }
    }
}