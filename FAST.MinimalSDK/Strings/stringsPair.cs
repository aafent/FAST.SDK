using FAST.Core;
using System.Collections;
using System.Text;
using System.Xml.Serialization;

namespace FAST.Strings
{
    /// <summary>
    /// Pair of strings. Similar to Tuple<string,string>
    /// but Tuple is not serializable by default, stringsPair it is, Tuple is more generic, stringsPair is always strings
    /// </summary>
    [XmlRootAttribute("pair")]
    public class stringsPair
    {
        /// <summary>
        /// Constructor without arguments
        /// </summary>
        public stringsPair()
        {
        }

        /// <summary>
        /// Constructor with two arguments 
        /// </summary>
        /// <param name="left">The left value</param>
        /// <param name="right">The right value</param>
        public stringsPair(string left, string right)
        {
            this.left = left;
            this.right = right;
        }

        /// <summary>
        /// Constructor with stringsPair as argument
        /// </summary>
        /// <param name="otherPairToCopy">stringPair to copy</param>
        public stringsPair(stringsPair otherPairToCopy)
        {
            this.left = otherPairToCopy.left;
            this.right = otherPairToCopy.right;
        }

        /// <summary>
        /// Constructor with a strings tuple as argument
        /// </summary>
        /// <param name="tuple">The strings Tuple</param>
        public stringsPair(Tuple<string,string> tuple)
        {
            this.left = tuple.Item1;
            this.right = tuple.Item2;
        }

        public stringsPair(FastKeyValuePair<string,string> keyPair)
        {
            this.left = keyPair.Key;
            this.right = keyPair.Value;
        }
        /// <summary>
        /// Constructor with KeyValuePair as argument
        /// </summary>
        /// <param name="keyPair"></param>
        public stringsPair(KeyValuePair<string, string> keyPair)
        {
            this.left = keyPair.Key;
            this.right = keyPair.Value;
        }



        // (v) for compatibility with Tuple()
        [XmlIgnore]
        public string Item1
        {
            get
            {
                return left;
            }
            set
            {
                left = value;
            }
        }
        [XmlIgnore]
        public string Item2
        {
            get
            {
                return right;
            }
            set
            {
                right = value;
            }
        }

        //private FastKeyValuePair<string, string> buffer = new FastKeyValuePair<string, string>();
        [XmlAttribute("L")]
        public string left = "";
        [XmlAttribute("R")]
        public string right = "";

        /// <summary>
        /// Override method ToString()
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            StringBuilder s = new();
            s.Append('[');
            s.Append(string.IsNullOrEmpty(left)?string.Empty:left);
            s.Append(", ");
            s.Append(string.IsNullOrEmpty(right) ? string.Empty : right);
            s.Append(']');
            return s.ToString();
        }

        /// <summary>
        /// Create a Dictionary having as key the left value and as value the right value
        /// </summary>
        /// <param name="stringsPairList">Input collection</param>
        /// <returns>The Dictionary</returns>
        public static Dictionary<string, string> toDictionaryBasedOnLeft(IEnumerable<stringsPair> stringsPairCollection)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            foreach (var item in stringsPairCollection)
            {
                if ( ! dictionary.ContainsKey( item.left ) ) { dictionary.Add( item.left, item.right ); }
            }
            return dictionary;
        }

        /// <summary>
        /// Convert stringsPairList to Tuple of strings;
        /// </summary>
        /// <param name="stringsPairList"></param>
        /// <returns></returns>
        public static Tuple<string, string> toTuple(stringsPair stringsPairList)
        {
            return new Tuple<string, string>(stringsPairList.left, stringsPairList.right);
        }

        /// <summary>
        /// Convert to equal in size string arrays to enumerable collection
        /// </summary>
        /// <param name="left">Array of left values</param>
        /// <param name="right">Array of right values</param>
        /// <returns>IEnumerable, the enumerable collection</returns>
        /// <exception cref="Exception"></exception>
        public static IEnumerable<stringsPair> createEnumerable(string[] left, string[] right)
        {
            if (left.Length != right.Length)
            {
                throw new Exception(string.Format("createList() left (#:{0}) and right (#:{1}) arrays must have equal number of items",left.Length,right.Length));
            }

            for (int item = 0; item < left.Length; item++)
            {
                yield return new stringsPair() { left = left[item], right = right[item] };
            }
        }



    }






}
