namespace FAST.DB
{

    /// <summary>
    /// Ado providers and few other types
    /// </summary>
    public enum adoProviderTypes:sbyte
    {   
        odbc = 10,
        oledb = 11,
        mockup= 12,
        url = 13,
        sql = 20,
        oracle=21,
        ase=22,
        mySQL=23,
        postgreSQL=24,
        sqLite=25,
        db2=26,

        /// <summary>
        /// Special provider (non-ado) for services
        /// </summary>
        [Obsolete("Use url instead.")]
        uri = 100
    }

}
