using FAST.Strings;
using FAST.Core;
using FAST.Core.Models;

namespace FAST.Authentication
{
    /// <summary>
    /// FAST Session Token, handling class.
    /// </summary>
    public class sessionToken : IsessionToken
    {
        /// <summary>
        /// private class to encode in the token information
        /// </summary>
        private class dataRecord
        {
            /// <summary>
            /// The userName
            /// </summary>
            public string UN {  get;set; } =""; // userName
            /// <summary>
            /// The user password
            /// </summary>
            public string UP { get;set; } =""; // userPassword
            /// <summary>
            /// The Real User name
            /// </summary>
            public string UR {  get;set; } =""; // User's Real Name (impersonate)

            /// <summary>
            /// Tenant ID 
            /// </summary>
            public string TE {  get; set; } = "";

            /// <summary>
            /// Any data assigned to the session
            /// </summary>
            public dynamic DT { get;set; } =null;
        }

        private const string _defaultTenant = "";
        private const string _defaultPassPhrase= "susami";
        private string _passPhrase = _defaultPassPhrase;



        /// <summary>
        /// The Application Specific Pass-Phrase 
        /// </summary>
        public string passPhrase
        {
            set
            {
                _passPhrase = value;
            }
        }

        [Obsolete("Rename the property to: passPhrase")]
        public string passPhase
        {
            set
            {
                _passPhrase = value;
            }
        }

        /// <summary>
        /// Constructor without arguments
        /// </summary>
        public sessionToken()
        {
        }

        /// <summary>
        /// Constructor with the Token as argument
        /// </summary>
        /// <param name="token"></param>
        public sessionToken(string token) 
        {
            this.token = token;
        }

        /// <summary>
        /// Constructor with the pass-phrase and the token as argument
        /// </summary>
        /// <param name="passPharse"></param>
        /// <param name="token"></param>
        public sessionToken(string passPhrase, string token)
        {
            this._passPhrase = passPhrase;
            this.token = token;
        }

        /// <summary>
        /// True if the Token is valid
        /// </summary>
        public bool isValid
        {
            get 
            {
                try
                {
                    var json = new json();
                    json.alreadySerializedObject = stringCipher.decrypt(token, _passPhrase);
                    var record = json.toObject<dataRecord>();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        private string _token = string.Empty;

        /// <summary>
        /// The Underlining Token
        /// </summary>
        public string token
        {
            get
            {
                return _token;
            }
            set
            {
                _token=value;
            }
        }

        /// <summary>
        /// The user name of the Token
        /// </summary>
        public string userName
        {
            get 
            {
                var json = new json();
                json.alreadySerializedObject = stringCipher.decrypt(token, _passPhrase);
                var record = json.toObject<dataRecord>();
                return record.UN;
            }
        }

        /// <summary>
        /// The optional user password for the token
        /// </summary>
        public string userPassword
        {
            get 
            {
                var json = new json();
                json.alreadySerializedObject = stringCipher.decrypt(token, _passPhrase);
                var record = json.toObject<dataRecord>();
                return record.UP;            
            }
        }

        /// <summary>
        /// The Tenant ID
        /// </summary>
        public string tenantID
        {
            get
            {
                var json = new json();
                json.alreadySerializedObject = stringCipher.decrypt(token, _passPhrase);
                var record = json.toObject<dataRecord>();
                return record.TE;
            }
        }

        /// <summary>
        /// The real user name, in case of impersonation 
        /// </summary>
        public string realUserName
        {
            get
            {
                var json = new json();
                json.alreadySerializedObject = stringCipher.decrypt(token, _passPhrase);
                var record = json.toObject<dataRecord>();
                return record.UR;
            }
        }

        /// <summary>
        /// True if the Token is impersonating enabled
        /// </summary>
        public bool impersonating
        {
            get
            {
                return !string.IsNullOrEmpty(realUserName);
            }
        }
        

        /// <summary>
        /// Get elementary credentials class
        /// </summary>
        public fastCredentialsElementary credentials
        {
            get
            {
                var passPhrase = _defaultPassPhrase == _passPhrase ? string.Empty : _passPhrase;
                var cred= new fastCredentialsElementary(passPhrase, this.userName, this.userPassword);
                cred.tenantID = _defaultTenant;
                return  cred;
            }
        }



        /// <summary>
        /// The Application specific data are encoded to the session token
        /// </summary>
        public dynamic data
        {
            get
            {
                var json = new json();
                json.alreadySerializedObject = stringCipher.decrypt( token, _passPhrase);
                var record=json.toObject<dataRecord>();
                return record.DT;
            }
            set
            {
                var json = new json();
                json.alreadySerializedObject = stringCipher.decrypt( token, _passPhrase);
                var record = json.toObject<dataRecord>();
                record.DT = value; 
                json.objectToSerialize = record;
                token = stringCipher.encrypt(json.ToString(), _passPhrase);
            }
        }



        /// <summary>
        /// Set the user name and password in the token
        /// </summary>
        /// <param name="name">The user name</param>
        /// <param name="password">Optional, the password</param>
        public void setUser(string name, string password)
        {
            inner_setUser(_defaultTenant, name, password);
        }

        /// <summary>
        /// Impersonate the current user
        /// </summary>
        /// <param name="impersonateUserName">The new user name</param>
        public void impersonate(string impersonateUserName)
        {   
            inner_setUser(this.userName,this.userPassword, impersonateUserName);
        }

        /// <summary>
        /// Stop the impersonation 
        /// </summary>
        public void deImpersonate()
        {
            if ( !impersonating ) return;
            inner_setUser(this.realUserName, this.userPassword);
        }

        private void inner_setUser(string name, string password, string impersonateUserName=null)
        {
            dataRecord record = new();
            if  ( string.IsNullOrEmpty(impersonateUserName))
            {
                record.UN = name;
                record.UP = password;
            }
            else
            {
                record.UN = impersonateUserName;
                record.UP = password;
                record.UR = name;
            }
            var json = new json();
            json.objectToSerialize = record;
            token = stringCipher.encrypt(json.ToString(), _passPhrase);
        }


        /// <summary>
        /// Check if a token is valid
        /// </summary>
        /// <param name="token">The token</param>
        /// <param name="passPhrase">Optional, the pass phrase</param>
        /// <returns></returns>
        public static bool isValidToken(string token, string passPhrase=null)
        {
            try
            {
                var auth = new sessionToken(token);
                if (!string.IsNullOrEmpty(passPhrase)) { auth.passPhrase = passPhrase; }
                return auth.isValid;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Return an invalid Token error
        /// </summary>
        public static void invalidTokenErrorTo(IserviceResponse instanceToSet)
        {
            instanceToSet.hasError = true;
            instanceToSet.errorText = "Invalid access token";
        }

        /// <summary>
        /// Return an invalid Token error
        /// </summary>
        public static void invalidTokenErrorTo(IserviceResponse instanceToSet, string errortext)
        {
            invalidTokenErrorTo(instanceToSet);
            instanceToSet.errorText = errortext;
        }

        /// <summary>
        /// Return an invalid Token error
        /// </summary>
        public static void invalidTokenErrorTo(IerrorCarrier instanceToSet)
        {
            instanceToSet.hasError = true;
            instanceToSet.errorText = "Invalid access token";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token">The token</param>
        /// <param name="session">Out, The session</param>
        /// <param name="passPhrase">Optional, set pass-phrase</param>
        /// <returns>bool, true if is a valid token</returns>
        public static bool tryGetValidSession(string token, out sessionToken session, string passPhrase = null)
        {
            session = null;
            try
            {
                session = new sessionToken(token);
                if (!string.IsNullOrEmpty(passPhrase)) { session.passPhrase = passPhrase; }
                return session.isValid;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Return an unauthorized error
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public static T unauthorized<T>(T result) where T : IerrorCarrier
        {
            sessionToken.invalidTokenErrorTo(result);
            return result;
        }
    }
    
}
