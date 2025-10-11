using System.Data;

namespace FAST.DB
{

    /// <summary>
    /// Enumeration with the Supported Column Behaviors
    /// </summary>
    public enum columnBehavior 
    {
        /// <summary>
        /// No special behavior
        /// </summary>
        nop = 0 
    } // , trackDBValueChange = 1, ... 2,4,8,16,32,64,128,256,512,1024,2048


    /// <summary>
    /// Attribute to define a property/field as a column in a SQL table
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
    [Serializable]
    public class sqlColumnAttribute : Attribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="isKey"></param>
        /// <param name="isUpdateable"></param>
        /// <param name="name"></param>
        /// <param name="isGroupKey"></param>
        /// <param name="direction"></param>
        /// <param name="size"></param>
        /// <param name="behavior"></param>
        public sqlColumnAttribute(bool isKey = false,   bool isUpdateable = true, string name = "", bool isGroupKey = false, 
                                                        ParameterDirection direction = ParameterDirection.Input, int size = 0,
                                                        columnBehavior behavior = columnBehavior.nop
                                )
        {
            this.isKey = isKey;
            this.isGroupKey = isGroupKey;
            this.isUpdateable = isUpdateable;
            this.direction = direction;
            this.name = name;
            this.size = size;
            this.behavior = behavior;
        }

        /// <summary>
        /// Is part of the primary key
        /// </summary>
        public bool isKey = false;

        /// <summary>
        /// Is part of a group key (multiple fields). Group keys are used in conjunction with isKey to define composite primary keys.
        /// </summary>
        public bool isGroupKey = false;

        /// <summary>
        /// Indicates if this group key is part of the delete criteria in SQL DELETE statements.
        /// </summary>
        public bool isGroupKeyPartOfDelete = true;

        /// <summary>
        /// Indicates if the column is updateable in SQL UPDATE statements.
        /// </summary>
        public bool isUpdateable = true;

        /// <summary>
        /// The behavior of the column
        /// </summary>
        public columnBehavior behavior = columnBehavior.nop;

        /// <summary>
        /// The size of the column (for string types, etc.)
        /// </summary>
        public int size = 0;

        /// <summary>
        /// The direction of the parameter (Input, Output, InputOutput, ReturnValue)
        /// </summary>
        public ParameterDirection direction = ParameterDirection.Input;

        /// <summary>
        /// The name of the column in the database. If not specified, the property/field name will be used.
        /// </summary>
        public string name = "";
    }
}
