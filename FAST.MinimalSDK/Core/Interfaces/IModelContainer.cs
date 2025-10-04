namespace FAST.Core
{
    /// <summary>
    /// Interface that declares a model property of a class.
    /// </summary>
    /// <typeparam name="modelType">The type of the model property</typeparam>
    public interface IModelContainer<modelType>
    {
        /// <summary>
        /// The model property
        /// </summary>
        modelType model { get; set; }
    }
}
