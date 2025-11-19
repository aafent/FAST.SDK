namespace FAST.Services.Models.Data
{
    /// <summary>
    /// Enumeration with the available Validation Engines for the Data Structure Field
    /// </summary>
    public enum dataStructureFieldValidationEngine : sbyte
    {
        /// <summary>
        /// No Validation 
        /// </summary>
        none = 0,

        /// <summary>
        /// Valid if match a regular expression
        /// </summary>
        regExMatch = 10,

        /// <summary>
        /// Valid if does not match a regular expression
        /// </summary>
        regExNotMatch = 12,

        /// <summary>
        /// validation via FAST Validator Service
        /// </summary>
        validatorService = 50,

        /// <summary>
        /// validation via Decision Engine
        /// </summary>
        decisionEngine = 60,

        /// <summary>
        /// validation via other service
        /// </summary>
        other = 90,
    }
}
