namespace FAST.Services.Models.Data
{
    /// <summary>
    /// Interface of data structure Validation Strategy
    /// </summary>
    public interface IdataStructureFieldValidation
    {
        /// <summary>
        /// Data Structure Engine will be used for validation
        /// </summary>
        public dataStructureFieldValidationEngine engine { get; set; }

        /// <summary>
        /// An argument (1) to the validation engine
        /// The usage of this field is depending on the validation engine
        /// Most of the cases is the main parameter, such us the name of the validation 
        /// or the expression
        /// </summary>
        public string arg1 { get; set; }

        /// <summary>
        /// An argument (2) to the validation engine
        /// The usage of this field is depending on the validation engine
        /// </summary>
        public string arg2 { get; set; }

        /// <summary>
        /// An argument (3) to the validation engine
        /// The usage of this field is depending on the validation engine
        /// </summary>
        public string arg3 { get; set; }

        /// <summary>
        /// The main method for the validation
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="value">The value</param>
        /// <returns>boolean, True is valid</returns>
        public bool isValid<T>(T value);

    }


}
