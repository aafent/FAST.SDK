namespace FAST.Core.Models
{
    /// <summary>
    /// implements a consumer normalization method
    /// </summary>
    public interface IconsumerNormalization
    {
        /// <summary>
        /// Consumer normalization method
        /// </summary>
        /// <param name="ext"></param>
        void consumerNormalization(Func<int> ext = null);
    }
}
