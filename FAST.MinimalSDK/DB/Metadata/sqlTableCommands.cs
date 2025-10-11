namespace FAST.DB
{
    /// <summary>
    /// SQL Table Commands
    /// </summary>
    public enum sqlTableCommands : sbyte
    {
        /// <summary>
        /// Update
        /// </summary>
        update = 1,

        /// <summary>
        /// Insert
        /// </summary>
        insert = 2,

        /// <summary>
        /// Select All By Key
        /// </summary>
        selectAllByKey = 3,

        /// <summary>
        /// Select All By Group Key
        /// </summary>
        selectAllByGroupKey = 4,

        /// <summary>
        /// Exists By Key
        /// </summary>
        existsByKey = 5,

        /// <summary>
        /// Delete By Key
        /// </summary>
        deleteByKey = 6,

        /// <summary>
        /// Delete By Group Key
        /// </summary>
        deleteByGroupKey = 7,

        /// <summary>
        /// Select All
        /// </summary>
        selectAll = 8,

        /// <summary>
        /// Pre Execute
        /// </summary>
        preExecute = 9,

        /// <summary>
        /// Post Execute
        /// </summary>
        postExecute = 10
    }
}
