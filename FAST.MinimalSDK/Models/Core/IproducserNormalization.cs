namespace FAST.Core.Models
{
    /// <summary>
    /// Implement a producer normalization method
    /// </summary>
    public interface IproducerNormalization
    {
        /// <summary>
        /// Producer normalization method
        /// </summary>
        /// <param name="ext"></param>
        void producerNormalization(Func<int> ext = null);
    }
}
