namespace FAST.Services.Models.Config
{
    /// <summary>
    /// FAST Agent Capabilities
    /// </summary>
    public class fastAgentCapabilities 
    {
        /// <summary>
        /// Has Access to the internet
        /// </summary>
        public bool internetAccess {  get; set; }

        /// <summary>
        /// Can upload files
        /// </summary>
        public bool upload {  get; set; }

        /// <summary>
        /// Can browse folder 
        /// </summary>
        public bool browse {  get; set; }

    }
}
