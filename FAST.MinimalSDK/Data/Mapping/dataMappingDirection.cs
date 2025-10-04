namespace FAST.Data
{
    /// <summary>
    /// Enumeration with the Data Mapping Direction
    /// </summary>
    public enum dataMappingDirection : sbyte
    {
        /// <summary>
        /// No Mapping is needed
        /// </summary>
        none = 0,

        /// <summary>
        /// Map the incoming data only
        /// </summary>
        incoming = 10,

        /// <summary>
        /// Map the outgoing data only
        /// </summary>
        outgoing = 20,

        /// <summary>
        /// Map both
        /// </summary>
        both = 30
    }

}
