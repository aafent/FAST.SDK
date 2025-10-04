namespace FAST.Core.FileSystem
{
    /// <summary>
    /// Library items main class
    /// </summary>
    public class libraryItem : IlibraryItem
    {
        /// <summary>
        ///  The Library name
        /// </summary>
        public string library {get; set; }

        /// <summary>
        /// The name of the Item in the Library
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// A Friendly name (user friendly) 
        /// </summary>
        public string friendlyName { get => Path.GetFileNameWithoutExtension(name); }

        /// <summary>
        /// Contractor without arguments
        /// </summary>
        public libraryItem()
        {
        }

        /// <summary>
        /// Constructor with a Library Item as arguments
        /// </summary>
        /// <param name="library">The Library name</param>
        /// <param name="name">The Item name in the Library</param>
        /// <exception cref="ArgumentNullException"></exception>
        public libraryItem(string library, string name):this()
        {
            this.library = library ?? throw new ArgumentNullException(nameof(library));
            this.name = name ?? throw new ArgumentNullException(nameof(name));
        }   

        /// <summary>
        /// Convert item to Tuple
        /// </summary>
        /// <returns>Tuple of two strings, the Library name and the Item name</returns>
        public Tuple<string,string> toTuple() => new Tuple<string,string>(this.library, this.name);
        public FastKeyValuePair<string,string> toKeyValuePair() => new FastKeyValuePair<string, string>(this.library, this.name);

        public override string ToString() 
        {
            return $"{library}/${name}";
        }

    }
}
