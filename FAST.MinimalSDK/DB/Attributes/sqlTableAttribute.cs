namespace FAST.DB
{

    /// <summary>
    /// Attribute to mark a class as table in the database  
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    [Serializable]
    public class sqlTableAttribute : Attribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="updateMethod"></param>
        public sqlTableAttribute(string name = "", updateMethods updateMethod = updateMethods.useUpdate)
        {
            this.name = name;
            this.updateMethod = updateMethod;
        }

        /// <summary>
        /// Name of the table in the database
        /// </summary>
        public string name = "";
        /// <summary>
        /// Update method to use
        /// </summary>
        public updateMethods updateMethod = updateMethods.useUpdate;

    }
}
