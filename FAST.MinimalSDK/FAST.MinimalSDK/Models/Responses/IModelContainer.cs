namespace FAST.Core.Models
{
    internal interface IModelContainer<modelType>
    {
        modelType model { get; set; }
    }
}
