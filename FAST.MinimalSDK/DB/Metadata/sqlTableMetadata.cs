namespace FAST.DB
{
    /// <summary>
    /// Update methods for sqlTableMetadata
    /// </summary>
    public struct sqlTableMetadata
    {
        /// <summary>
        /// Columns that need to be quoted when used in SQL statements
        /// </summary>
        public string[] needQuote;

        /// <summary>
        /// Database object name (table or view)
        /// </summary>
        public string dbObjectName;

        /// <summary>
        /// Indicates if dbObjectName is overwritten in the attribute
        /// </summary>
        public bool dbObjectNameIsOverwritten;

        /// <summary>
        /// Update method to use
        /// </summary>
        public updateMethods updateMethod;

        /// <summary>
        /// SQL command to select all rows
        /// </summary>
        public string sqlSelectAllCommand;

        /// <summary>
        /// SQL command to select all rows by key
        /// </summary>
        public string sqlSelectAllByKeyCommand;

        /// <summary>
        /// SQL command to select all rows by group key
        /// </summary>
        public string sqlSelectAllByGroupKeyCommand;

        /// <summary>
        /// SQL command to check if a row exists by key
        /// </summary>
        public string sqlExistsByKeyCommand;

        /// <summary>
        /// SQL command to check if a row exists by group key
        /// </summary>
        public string sqlUpdateCommand;

        /// <summary>
        /// SQL command to insert a row
        /// </summary>
        public string sqlInsertCommand;

        /// <summary>
        /// SQL command to delete a row by key
        /// </summary>
        public string sqlDeleteByKeyCommand;

        /// <summary>
        /// SQL command to delete rows by group key
        /// </summary>
        public string sqlDeleteByGroupKeyCommand;

    }
}
