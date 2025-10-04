namespace FAST.Core.Models
{

    public interface IerrorCarrier : IerrorQuestion
    {
        string errorText { get; set; }
        string extendedErrorText { get; set; }
    }
}
