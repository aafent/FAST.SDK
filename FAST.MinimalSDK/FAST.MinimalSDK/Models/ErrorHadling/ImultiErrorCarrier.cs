namespace FAST.Core.Models
{
    public interface ImultiErrorCarrier : IerrorCarrier
    {
        List<string> errors { get; set; }
    }
}
