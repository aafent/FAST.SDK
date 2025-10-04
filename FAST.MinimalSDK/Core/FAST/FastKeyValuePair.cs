using System.Text;

namespace FAST.Core
{

    /// <summary>
    /// Based on .NET's KeyValuePair.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class FastKeyValuePair<TKey, TValue>
    {
        #region Public Constructors

        public FastKeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        #endregion Public Constructors

        #region Public Properties

        public TKey Key { get; private set; }

        public TValue Value { get; private set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Provides a string representation ([key, value]) of the key value pair.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder s = new();
            s.Append('[');
            if (Key != null)
            {
                s.Append(Key.ToString());
            }
            s.Append(", ");
            if (Value != null)
            {
                s.Append(Value.ToString());
            }
            s.Append(']');
            return s.ToString();
        }

        #endregion Public Methods

        public static implicit operator FastKeyValuePair<TKey, TValue>(KeyValuePair<TKey, TValue> t) => new(t.Key, t.Value);

        public static implicit operator KeyValuePair<TKey, TValue>(FastKeyValuePair<TKey, TValue> t) => new(t.Key, t.Value);

    }
}