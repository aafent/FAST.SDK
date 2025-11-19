namespace FAST.Services.Models.Data
{
    /// <summary>
    /// Enumeration with the existing Persistence Engines
    /// </summary>
    public enum dataStructurePersistenceEngine : sbyte
    {
        /// <summary>
        /// No Persistence engine and strategy 
        /// </summary>
        none = 0,

        /// <summary>
        /// The persistence will performed by an hard coded logic
        /// most probably by an addIns
        /// </summary>
        hardCoded = 11,

        /// <summary>
        /// SQL Update will be used
        /// </summary>
        update = 10,

        /// <summary>
        /// SQL Insert will be used
        /// </summary>
        insert = 12,

        /// <summary>
        /// SQL Update or Insert will be used
        /// </summary>
        updateInsert = 15,

        /// <summary>
        /// SQL Delete and the insert will be used
        /// </summary>
        deleteInsert = 18,

        /// <summary>
        /// The persistence will performed by a stored procedure
        /// </summary>
        storedProcedure = 20,

        /// <summary>
        /// Application Specific
        /// </summary>
        appSpecific01 = 101,
        appSpecific02 = 102,
        appSpecific03 = 103,
        appSpecific04 = 104,
        appSpecific05 = 105,
        appSpecific06 = 106,
        appSpecific07 = 107,
        appSpecific08 = 108,
        appSpecific09 = 109,
        appSpecific10 = 110,
        appSpecific11 = 111,
        appSpecific12 = 112,
        appSpecific13 = 113,
        appSpecific14 = 114,
        appSpecific15 = 115,
        appSpecific16 = 116,
        appSpecific17 = 117,
        appSpecific18 = 118,
        appSpecific19 = 119,
        appSpecific20 = 120


    }
}
