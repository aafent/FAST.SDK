using System.Data.Common;

namespace FAST.DB
{
    /// <summary>
    /// Enumeration with the Supported Special SQL Commands
    /// </summary>
    public enum specialSQLCommands : sbyte 
    {
        /// <summary>
        /// Command to retrieve the current date and time from the database server
        /// </summary>
        retrieveDateTime = 0 
    }


    /// <summary>
    /// Interface for DB Dialect Details
    /// </summary>
    public interface IdbDialectDetails
    {
        /// <summary>
        /// String representing a NULL value in SQL statements
        /// </summary>
        string nullValue { get; }

        /// <summary>
        /// String representing the DateTime format in SQL statements
        /// </summary>
        string dateTimeFormat { get; }

        /// <summary>
        /// String representing the opening quote character for identifiers in SQL statements
        /// </summary>
        string openQuote { get; }

        /// <summary>
        /// String representing the closing quote character for identifiers in SQL statements
        /// </summary>
        string closeQuote { get; }

        /// <summary>
        /// Indicates whether the dialect is used over an ODBC connection
        /// </summary>
        bool isOverOdbcConnection {  get; set; }

        /// <summary>
        /// Filters error text to remove or modify sensitive information
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        string filterErrorText(string text);

        /// <summary>
        /// Returns the SQL syntax for a given special command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        string sqlSyntax(specialSQLCommands command);

        /// <summary>
        /// Creates and returns a new database parameter object
        /// </summary>
        /// <returns></returns>
        DbParameter newParameter();
    }

}
