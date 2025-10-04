using FAST.Authentication;

namespace FAST.Authentication
{
    /// <summary>
    /// FAST Credentials, elementary fields
    /// </summary>
    public class fastCredentialsElementary 
    {
        /// <summary>
        /// Optional, Pass phrase. Default is empty space.
        /// This is a client/application specific value. 
        /// </summary>
        public string passPhrase { get; set; } = string.Empty;

        /// <summary>
        /// User name. Callers user identification unique id/name
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// Optional, password for the user name
        /// </summary>
        public string userPassword { get; set; } = string.Empty;

        /// <summary>
        /// Unique identification of the tenant/company etc.
        /// </summary>
        public string tenantID { get; set; } = null;


        /// <summary>
        /// Constructor with no arguments
        /// </summary>
        public fastCredentialsElementary()
        {
        }


        /// <summary>
        /// Constructor with the elementary fields as arguments
        /// </summary>
        /// <param name="passPhrase">Application specific pass phrase</param>
        /// <param name="userName">The user name</param>
        /// <param name="userPassword">Optional, the password for the username</param>
        public fastCredentialsElementary(string passPhrase, string userName, string userPassword="")
        {
            this.passPhrase=passPhrase;
            this.userName=userName;
            this.userPassword=userPassword;
        }


        /// <summary>
        /// Constructor with a token as argument
        /// </summary>
        /// <param name="passPhrase">Application specific pass phrase</param>
        /// <param name="token">A valid token</param>
        public fastCredentialsElementary(string passPhrase, string token)
        {
            sessionToken session;
            if (!sessionToken.tryGetValidSession(token, out session)) { throw new UnauthorizedAccessException(); }

            this.passPhrase = passPhrase;
            this.userName = session.userName;
            this.userPassword = session.userPassword;
        }

    }



}
