
namespace FAST.Services.Models
{
    /// <summary>
    /// Interface to define the model property of the response. 
    /// </summary>
    /// <typeparam name="modelType"></typeparam>
    public interface IpropertyModel<modelType>
    {
        modelType model { get; set; }
    }

}