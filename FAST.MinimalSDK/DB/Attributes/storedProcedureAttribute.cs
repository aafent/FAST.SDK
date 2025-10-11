namespace FAST.DB
{
    /// <summary>
    /// Indicates that a class represents a stored procedure in the database.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    [Serializable]
    public class storedProcedureAttribute : Attribute
    {

        /// <summary>
        /// Constructor for the storedProcedureAttribute.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="useFEArgument"></param>
        /// <param name="argumentFEValue"></param>
        public storedProcedureAttribute(string name = "", bool useFEArgument = true, string argumentFEValue = "SQL")
        {
            this.name = name;
            this.useFEArgument = useFEArgument;
            this.argumentFEValue = argumentFEValue;
        }

        /// <summary>
        /// The name of the stored procedure in the database.
        /// </summary>
        public string name = "";

        /// <summary>
        /// Indicates whether to use the FE argument. Defaults to true.
        /// Argument FE is the first argument in SPd, varchar(3) or more, and indicates the type of front-end call.
        /// </summary>
        public bool useFEArgument = true;

        /// <summary>
        /// The value to be used for the FE argument. Defaults to "SQL".
        /// </summary>
        public string argumentFEValue = "SQL";

    }
}
