using FAST.Core;
using FAST.Core.Models;
using FAST.Data;
using System.Data;
using System.Data.Common;

namespace FAST.DB
{
    /// <summary>
    /// Delegate for preparing a SQL command before execution
    /// </summary>
    public enum sqlChannelConnectionPolicy 
    {
        /// <summary>
        /// The connection is always opened and closed automatically for each operation
        /// </summary>
        automatic = 0 
    }

    /// <summary>
    /// Delegate for preparing a SQL command before execution
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="reader"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public delegate bool fetchRow(object obj, DbDataReader reader, int row);


    /// <summary>
    /// Interface for a basic/elementary SQL Channel
    /// </summary>
    public interface IsqlChannelElementary
    {
        /// <summary>
        /// The SQL command that caused the last error, if any
        /// </summary>
        string errorSQLCommand { get; }

        /// <summary>
        /// The error text of the last error, if any
        /// </summary>
        string errorText { get; set; }

        /// <summary>
        /// Extended error text, if any
        /// </summary>
        string extendedErrorText { get; set; }

        /// <summary>
        /// Indicates whether the channel has encountered an error
        /// </summary>
        bool hasError { get; set; }

        /// <summary>
        /// Indicates whether the connection is currently open
        /// </summary>
        bool isConnectionOpen { get; }

        /// <summary>
        /// The last error text, if any
        /// </summary>
        string lastErrorText { get; }

        /// <summary>
        /// The last SQL command executed
        /// </summary>
        string lastSQLCommand { get; }

        /// <summary>
        /// The current transaction level (0 if no transaction is active)
        /// </summary>
        int transactionLevel { get; }

        /// <summary>
        /// Begin a new transaction
        /// </summary>
        void beginTransaction();

        /// <summary>
        /// Clone the current channel to a new channel instance
        /// </summary>
        /// <returns></returns>
        IsqlChannelElementary cloneToNewChannel();

        /// <summary>
        /// Close the current connection
        /// </summary>
        void close();

        /// <summary>
        /// Asynchronously close the current connection
        /// </summary>
        /// <returns></returns>
        Task closeAsync();

        /// <summary>
        /// Commit the current transaction
        /// </summary>
        void commiTransaction();

        /// <summary>
        /// Copy the error information from another error carrier object
        /// </summary>
        /// <param name="givingObject"></param>
        void copyErrorFrom(IerrorCarrier givingObject);

        /// <summary>
        /// Copy the error information to another error carrier object
        /// </summary>
        /// <param name="receiverObject"></param>
        void copyErrorTo(IerrorCarrier receiverObject);

        /// <summary>
        /// End the current transaction
        /// </summary>
        /// <returns></returns>
        bool endTransaction();

        /// <summary>
        /// Execute the current SQL command and return a data reader
        /// </summary>
        /// <returns></returns>
        DbDataReader executeAndGetReader();

        /// <summary>
        /// Execute the current SQL command as a non-query (INSERT, UPDATE, DELETE)
        /// </summary>
        /// <returns></returns>
        bool executeNonQuery();

        /// <summary>
        /// Asynchronously execute the current SQL command as a non-query (INSERT, UPDATE, DELETE)
        /// </summary>
        /// <returns></returns>
        Task<bool> executeNonQueryAsync();

        /// <summary>
        /// Execute the current SQL command and process each row using the provided fetchRowMethod delegate
        /// </summary>
        /// <param name="fetchRowMethod"></param>
        /// <returns></returns>
        int executeReader(fetchRow fetchRowMethod);

        /// <summary>
        /// Execute the current SQL command and process each row using the provided fetchRowMethod delegate, populating the provided object
        /// </summary>
        /// <param name="objToFetchRow"></param>
        /// <param name="fetchRowMethod"></param>
        /// <returns></returns>
        int executeReader(object objToFetchRow, fetchRow fetchRowMethod);

        /// <summary>
        /// Execute the current SQL command and process each row using the provided fetchRowWithHelper delegate, populating the provided object
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="projector"></param>
        /// <param name="hub"></param>
        /// <returns></returns>
        Task executeReaderAsyncViaHub<TData>(Func<IDataRecord, TData> projector, IproducerConsumerHub<TData> hub);

        /// <summary>
        /// Execute the current SQL command and process each row using the provided fetchRowMethod delegate, populating the provided object
        /// </summary>
        /// <param name="objToFetchRow"></param>
        /// <param name="fetchRowMethod"></param>
        /// <returns></returns>
        int executeReaderUsingHelper(object objToFetchRow, fetchRow fetchRowMethod);

        /// <summary>
        /// Execute a SQL command and check if any rows exist
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="policy"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        bool exists(string sql, setFromObjectsPolicy policy, object input = null);

        /// <summary>
        /// Finalize the reader and release associated resources
        /// </summary>
        void finalizeReader();

        /// <summary>
        /// Fetch the first row of a SQL query and return it as a dictionary
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        Dictionary<string, object> firstRowToDictionary(string sql);

        /// <summary>
        /// Get the dialect details for the current SQL channel
        /// </summary>
        /// <returns></returns>
        IdbDialectDetails getDialect();

        /// <summary>
        /// Get the current list of parameters for the SQL command
        /// </summary>
        /// <returns></returns>
        List<DbParameter> getParameters();

        /// <summary>
        /// Get the current SQL command text
        /// </summary>
        /// <returns></returns>
        DbParameter newParameter();

        /// <summary>
        /// Create a new list of parameters for the SQL command
        /// </summary>
        /// <returns></returns>
        List<DbParameter> newParameters();

        /// <summary>
        /// Open the database connection
        /// </summary>
        void open();

        /// <summary>
        /// Asynchronously open the database connection
        /// </summary>
        /// <returns></returns>
        Task openAsync();

        /// <summary>
        /// Populate an object with data from the current row of a data reader
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectToPopulate"></param>
        /// <returns></returns>
        bool replace(DbDataReader reader, object objectToPopulate);

        /// <summary>
        /// Reset the error state of the channel
        /// </summary>
        void resetErrors();

        /// <summary>
        /// Rollback the last transaction
        /// </summary>
        void rollbackLastTransaction();

        /// <summary>
        /// Rollback the current transaction
        /// </summary>
        void rollbackTransaction();

        /// <summary>
        /// Run a SQL command with the given policy, input object, and output objects
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="policy"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        bool run(string sql, setFromObjectsPolicy policy, object input, params object[] output);

        /// <summary>
        /// Set the error state of the channel with the given exception
        /// </summary>
        /// <param name="ex"></param>
        void setError(Exception ex);

        /// <summary>
        /// Set the error state of the channel with the given error text
        /// </summary>
        /// <param name="errorText"></param>
        void setError(string errorText);

        /// <summary>
        /// Set the current SQL command text
        /// </summary>
        /// <param name="parameters"></param>
        void setParameters(List<DbParameter> parameters);

        /// <summary>
        /// Set the current SQL command text
        /// </summary>
        /// <param name="command"></param>
        void setTemplate(specialSQLCommands command);

        /// <summary>
        /// Set the current SQL command text
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="columnName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        object valueOf(DbDataReader reader, string columnName, object defaultValue = null);

        /// <summary>
        /// Set the current SQL command text
        /// </summary>
        /// <param name="values"></param>
        /// <param name="columnName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        object valueOf(Dictionary<string, object> values, string columnName, object defaultValue = null);
    }
}