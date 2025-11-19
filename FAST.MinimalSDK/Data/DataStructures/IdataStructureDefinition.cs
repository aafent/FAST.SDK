
namespace FAST.Services.Models.Data
{
    /// <summary>
    /// The Data Structure Interface
    /// </summary>
    /// <typeparam name="TMapping">The type for Mapping</typeparam>
    /// <typeparam name="TValidator">The type for Validation</typeparam>
    /// <typeparam name="TPersistence">The type for Persistence</typeparam>
    public interface IdataStructureDefinition<TMapping, TValidator, TPersistence, TExtension>
                                                where TMapping : IdataStructureFieldMapping, new()
                                                where TValidator : IdataStructureFieldValidation, new()
                                                where TPersistence : IdataStructurePersistence, new()
                                                where TExtension : IdataStructureElementExtension, new()
    {
        /// <summary>
        /// The name of the data structure
        /// </summary>
        string name { get; set; }

        /// <summary>
        /// A description of the data structure
        /// </summary>
        string description { get; set; }
        
        /// <summary>
        /// A collection of data structure elements
        /// </summary>
        IEnumerable<dataStructureElement<TMapping, TValidator, TPersistence, TExtension>> definitions { get; set; }
    }
}