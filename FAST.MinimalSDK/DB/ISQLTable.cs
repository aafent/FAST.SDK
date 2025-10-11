using FAST.Data;

namespace FAST.DB
{
    /// <summary>
    /// SQL Table Interface, used to define standard SQL operations for a given channel type.
    /// </summary>
    /// <typeparam name="TChannel"></typeparam>
    public interface ISQLTable<TChannel> where TChannel : class
    {
        /// <summary>
        /// Initialize the SQL Table, this method is called once per application lifecycle.
        /// </summary>
        void initialize();

        /// <summary>
        /// Prepare the SQL Command for the given channel, command type, and optional data filter.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="command"></param>
        /// <param name="dataFilter"></param>
        void prepareSQLCommand(TChannel channel, sqlTableCommands command, IdataFilterItem dataFilter);

        /// <summary>
        /// Select all data for the given channel.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        bool existsByKey(TChannel channel);

        /// <summary>
        /// Select all data for the given channel.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        bool deleteDataByGroupKey(TChannel channel);

        /// <summary>
        /// Select all data for the given channel.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        bool deleteDataByKey(TChannel channel);

        /// <summary>
        /// Select all data for the given channel.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        bool insertData(TChannel channel);

        /// <summary>
        /// Select all data for the given channel.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        bool updateData(TChannel channel);
    }
}
