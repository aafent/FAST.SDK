using FAST.Data;

namespace FAST.Services.Models.Data
{
    /// <summary>
    /// Interface of data structure for Data Mapping Strategy
    /// </summary>
    public interface IdataStructureFieldMapping
    {
        /// <summary>
        /// The direction of the mapping
        /// </summary>
        public dataMappingDirection direction { get; set; }

        /// <summary>
        /// An argument (1) to the mapping
        /// The usage of this field is depending on the mapping engine
        /// </summary>
        public string arg1 { get; set; }

    }




}
