namespace FAST.DB
{

    /// <summary>
    /// Ado providers and few other types
    /// </summary>
    public enum adoProviderTypes:sbyte
    {
        /// <summary>
        /// ODBC Generic Provider
        /// </summary>
        odbc = 10,

        /// <summary>
        /// OLEDB Generic Provider
        /// </summary>
        oledb = 11,

        /// <summary>
        /// Mockup provider for testing without database
        /// </summary>
        mockup = 12,

        /// <summary>
        /// URL provider for REST/HTTP/HTTPS services
        /// </summary>
        url = 13,

        /// <summary>
        /// SQL Server Provider by Microsoft (TSQL)
        /// </summary>
        sql = 20,

        /// <summary>
        /// Oracle Provider by Oracle (PL/SQL)
        /// </summary>
        oracle = 21,

        /// <summary>
        /// Sybase Provider by SAP (TSQL)
        /// </summary>
        ase = 22,

        /// <summary>
        /// MySQL Provider by Oracle (SQL)
        /// </summary>
        mySQL = 23,

        /// <summary>
        /// PostgreSQL Provider by PostgreSQL Global Development Group (SQL)
        /// </summary>
        postgreSQL = 24,

        /// <summary>
        /// SQLite Provider by SQLite.org (SQL)
        /// </summary>
        sqLite = 25,

        /// <summary>
        /// DB2 Provider by IBM (SQL)
        /// </summary>
        db2 = 26,

        /// <summary>
        /// Special provider (non-ado) for services
        /// </summary>
        [Obsolete("Use url instead.")]
        uri = 100
    }

}
