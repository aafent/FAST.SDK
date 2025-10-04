namespace FAST.Core.Models
{
    public class elementaryErrorContainer : IerrorCarrier
    {
        public bool hasError { get; set; }
        public string errorText { get; set; }
        public string extendedErrorText { get; set; }
    }

    public class elementaryMultiErrorContainer : elementaryErrorContainer, ImultiErrorCarrier
    {
        public List<string> errors { get; set; }
    }
}
