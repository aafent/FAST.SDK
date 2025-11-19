namespace FAST.Services.Models
{
    public class uploadResults
    {
        public bool uploaded { get; set; }
        public string? fileName { get; set; }
        public string? storedFileName { get; set; }
        public int errorCode { get; set; }
    }
}
