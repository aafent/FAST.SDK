namespace FAST.Core
{
    /// <summary>
    /// Extension methods for <see cref="FastKeyValuePair{TKey, TValue}"/>
    /// </summary>
    public static class fastKeyValuePair_Extensions
    {

        /// <summary>
        /// Converts a <see cref="IDictionary{TKey, TValue}"/> to an<see cref="IEnumerable{T}"/> of <see cref="FastKeyValuePair{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public static IEnumerable<FastKeyValuePair<TKey, TValue>> ToFastKeyValuePairs<TKey, TValue>(this IDictionary<TKey, TValue> pairs)
        {
            return pairs.Select(p => new FastKeyValuePair<TKey, TValue>(p.Key, p.Value));
        }

        /// <summary>
        /// Converts an <see cref="IEnumerable{T}"/> of <see cref="FastKeyValuePair{TKey, TValue}"/> to a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<FastKeyValuePair<TKey, TValue>> pairs)
        {
            return pairs.ToDictionary(x => x.Key, x => x.Value);
        }

    }

}
