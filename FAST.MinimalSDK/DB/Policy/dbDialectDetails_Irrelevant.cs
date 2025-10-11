using System.Data.Common;

namespace FAST.DB
{
    /// <summary>
    /// Dialect details. Provides formats and special manipulation for a specific SQL flavor
    /// </summary>
    public class irrelevantDialectDetails : IdbDialectDetails
    {
        /// <summary>
        /// The opening quot
        /// </summary>
        public string openQuote
        {
            get { return "'"; }
        }

        /// <summary>
        /// The closing quote 
        /// </summary>
        public string closeQuote
        {
            get { return "'"; }
        }

        /// <summary>
        /// The value of the NULL values
        /// </summary>
        public string nullValue
        {
            get { return "NULL"; }
        }

        /// <summary>
        /// The format for the date time values
        /// </summary>
        public string dateTimeFormat
        {
            get { return "yyyy-MM-ddTHH:mm:ss"; }
        }

        /// <summary>
        /// Check if is running over an ODBC connection
        /// </summary>
        public bool isOverOdbcConnection { get; set; } = false;

        /// <summary>
        /// Filter and normalized error messages
        /// </summary>
        /// <param name="text">The message text</param>
        /// <returns>The normalized (filtered) error text</returns>
        public string filterErrorText(string text)
        {
            return text;
        }


        /// <summary>
        /// Return an instance to the appropriate parameters object
        /// </summary>
        /// <returns>DbParameter, the new instance</returns>
        public DbParameter newParameter()
        {
            throw new NotSupportedException();
            //return default(DbParameter);
        }

        /// <summary>
        /// The SQL syntax of special SQL command
        /// </summary>
        /// <param name="command">The SQL command</param>
        /// <returns>String,The SQL source </returns>
        public string sqlSyntax(specialSQLCommands command)
        {
            switch (command)
            {
                case specialSQLCommands.retrieveDateTime:
                    return @"SELECT SYSDATETIME() as sysdt";
            }
            throw new NotImplementedException(command.ToString());
        }
    }

}
