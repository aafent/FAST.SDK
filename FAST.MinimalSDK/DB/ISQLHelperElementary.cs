namespace FAST.DB
{
    /// <summary>
    /// This is an elementary interface for SQL helpers
    /// </summary>
    public interface ISQLHelperElementary
    {

        /// <summary>
        /// Get a dictionary of column names and their ordinal positions
        /// </summary>
        public Dictionary<string, int> col { get; }

        /// <summary>
        /// Get the type of a column by its name
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        public Type getColumnType(string colName);



        /// <summary>
        /// Get a string value from the current row
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="nullValue"></param>
        /// <returns></returns>
        public object getAny(string colName, object nullValue);
    }


}
