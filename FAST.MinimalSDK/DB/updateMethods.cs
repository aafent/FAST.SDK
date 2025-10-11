namespace FAST.DB
{
    /// <summary>
    /// Defines the methods available for updating records in the database.
    /// </summary>
    public enum updateMethods 
    {

        /// <summary>
        /// Use the SQL UPDATE command to modify existing records.
        /// </summary>
        useUpdate = 0,

        /// <summary>
        /// Use the SQL INSERT command to add new records.
        /// </summary>
        useDeleteAndInsert = 2,

        /// <summary>
        /// Use the SQL DELETE command to remove records by group key, followed by INSERT commands to add new records.
        /// </summary>
        useDeleteGroupAndInserts = 3,

        /// <summary>
        /// No updates will be performed on the records.
        /// </summary>
        noUpdates = 4 
            
    }


}
