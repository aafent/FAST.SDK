namespace FAST.Core.Models
{
    public interface IErrorItem
    {
        errorTypes type { get; set; }
        string description { get; set; }
    }
}
