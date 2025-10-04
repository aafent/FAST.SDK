namespace FAST.Core
{

    /// <summary>
    /// interface indicates the support a result status 
    /// </summary>
    public interface IResultStatus
    {
        /// <summary>
        /// True if had a successful execution
        /// </summary>
        bool success { get; set; }

        /// <summary>
        /// Result message
        /// </summary>
        string message { get; set; }
    }


    /// <summary>
    /// A class for the result status of a response
    /// </summary>
    public class resultStatus : IResultStatus
    {
        /// <summary>
        /// if true, the execution was successful 
        /// </summary>
        public bool success { get; set; }

        /// <summary>
        /// A result message (can also be an error message)
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// The data element of the result
        /// </summary>
        public object data { get; set; }

        /// <summary>
        /// true if the execution is succeed
        /// </summary>
        public bool isSucceed
        {
            get
            {
                return success;
            }
        }

        /// <summary>
        /// True if the execution is not succeed 
        /// </summary>
        public bool isFailed
        {
            get
            {
                return !isSucceed;
            }
        }

        /// <summary>
        /// Constructor without arguments 
        /// </summary>
        public resultStatus()
        {
            success = true;
            message = string.Empty;
        }

        /// <summary>
        /// Constructor with an exception as argument
        /// The status will marked as Faild, the message will contain the error text of the exception
        /// </summary>
        /// <param name="ex">The exception</param>
        public resultStatus(Exception ex)
        {
            success = false;
            message = ex.Message;
        }

        /// <summary>
        /// Return an instance to a faild result status.
        /// </summary>
        /// <param name="message">Optional, an error message</param>
        /// <returns>resultStatus, unsuccessfull (faild) status</returns>
        public static resultStatus error(string message = "")
        {
            if (string.IsNullOrEmpty(message)) { message = "General Error"; }
            return new resultStatus() { message = message, success = false };
        }

        /// <summary>
        /// Return an instance to a succeded result status.
        /// </summary>
        /// <returns></returns>
        public static resultStatus succeed()
        {
            return new resultStatus() { message=string.Empty, success=true };
        }

    }

}
