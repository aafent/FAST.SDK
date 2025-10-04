namespace FAST.Core
{
    /// <summary>
    /// Defines the policy for setting values from objects.
    /// </summary>
    public class setFromObjectsPolicy
    {
        public bool ignoreNulls = true;
        public bool ignoreEmptyStrings = true;
        public bool ignoreZeroNumerics = true;

        public static setFromObjectsPolicy dbData
        {
            get
            {
                return new setFromObjectsPolicy();
            }
        }
    }
}