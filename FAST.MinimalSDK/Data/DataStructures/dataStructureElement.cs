namespace FAST.Services.Models.Data
{
    public class dataStructureElement<TMapping, TValidator, TPersistence, TExtension> 
                                                            where TMapping : IdataStructureFieldMapping, new()
                                                            where TValidator : IdataStructureFieldValidation, new()
                                                            where TPersistence : IdataStructurePersistence, new()
                                                            where TExtension : IdataStructureElementExtension, new()
    {
        /// <summary>
        /// Field name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// The expected datatype of the input data
        /// Default is string
        /// </summary>
        public string inputDatatype { get; set; } = "string";

        /// <summary>
        /// The actual, internal datatype
        /// default is string
        /// </summary>
        public string innerDatatype { get; set; } = "string";

        /// <summary>
        /// The default value
        /// </summary>
        public string defaultValue {  get;set; } = null;

        /// <summary>
        /// Indicates if the field is mandatory to have a value
        /// default false;
        /// </summary>
        public bool isMandatory { get; set; } = false;

        /// <summary>
        /// Value Mapping Strategy 
        /// </summary>
        public TMapping mapping {  get; set; } = default;

        /// <summary>
        /// Validation Strategy 
        /// </summary>
        public TValidator validation { get; set;} = default;

        /// <summary>
        /// Validation Strategy 
        /// </summary>
        public TPersistence persistence { get; set; } = default;

        /// <summary>
        /// Extension to the class.
        /// This is used to avoid to inherit this class.
        /// </summary>
        public TExtension ext { get; set; } = default; 

    }

}
