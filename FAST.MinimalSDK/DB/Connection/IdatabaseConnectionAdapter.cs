using System.Data.Common;

namespace FAST.DB
{
    /// <summary>
    /// Defines the contract for a database connection adapter, which provides methods to create database connections and access dialect details.
    /// </summary>
    public interface IdatabaseConnectionAdapter
    {

        /// <summary>
        /// Gets or sets the connection string used to connect to the database.
        /// </summary>
        string connectionString { get; set; }

        /// <summary>
        /// Gets the dialect details of the database.
        /// </summary>
        IdbDialectDetails dialect { get; }

        /// <summary>
        /// Creates and returns a new database connection.
        /// </summary>
        /// <returns></returns>
        DbConnection newDataBaseConnection();
    }



}
