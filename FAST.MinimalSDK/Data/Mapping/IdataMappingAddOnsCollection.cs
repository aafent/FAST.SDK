namespace FAST.Data
{
    /// <summary>
    /// Interface for Data Mapping AddOns Collections
    /// </summary>
    public interface IdataMappingAddOnsCollection
    {
        /// <summary>
        /// Method to add an AddOn to the collection
        /// </summary>
        /// <param name="name"></param>
        /// <param name="addOnMethod"></param>
        public void add(string name, Func<string, object, object> addOnMethod);

        /// <summary>
        /// The AddOns collection
        /// </summary>
        public Dictionary<string, Func<string, object, object>> addOns { get; }
    }
}
