namespace FAST.Core
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
    [Serializable]
    public class synonymsAttribute : Attribute
    {
        public synonymsAttribute(params string[] columnNames)
        {
            this.columnNames = columnNames;
        }
        public string[] columnNames = null;
    }

}
