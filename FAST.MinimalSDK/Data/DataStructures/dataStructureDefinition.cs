namespace FAST.Services.Models.Data
{
    /// <summary>
    /// The definition of a data structure
    /// </summary>
    /// <typeparam name="TMapping">The type for mapping</typeparam>
    /// <typeparam name="TValidator">The type of validator</typeparam>
    /// <typeparam name="TPersistence">The type for persistence</typeparam>
    public class dataStructureDefinition<TMapping, TValidator, TPersistence, TExtension> : IdataStructureDefinition<TMapping, TValidator, TPersistence, TExtension> 
                                        where TMapping : IdataStructureFieldMapping, new()
                                        where TValidator : IdataStructureFieldValidation, new()
                                        where TPersistence : IdataStructurePersistence, new()
                                        where TExtension : IdataStructureElementExtension, new()
    {
        /// <summary>
        /// The name of the data structure
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// The description of a data structure
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Collection of the columns definition 
        /// </summary>
        public IEnumerable<dataStructureElement<TMapping, TValidator, TPersistence, TExtension>> definitions { get; set; }
    }
}
