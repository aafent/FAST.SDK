
namespace FAST.Core
{
    /// <summary>
    /// Enumeration of the FAST Components
    /// </summary>
    public enum fastComponent
    {
        /// <summary>
        /// For any FAST compliant service, but not member of FAST collection
        /// </summary>
        None = 0,


        /// <summary>
        /// The Core library
        /// </summary>
        Core=1,

        /// <summary>
        /// The basic FAST Services
        /// </summary>
        Services=2,
        
        /// <summary>
        /// The Pandora Service (currently member of the Services)
        /// </summary>
        Pandora=20,

        /// <summary>
        /// The Validator Service
        /// </summary>
        Validator=21,

        /// <summary>
        /// The Validator Service
        /// </summary>
        Agent = 22

    }
}
