using FAST.Core;

namespace FAST.Authentication
{

    public class sessionID : ISessionID
    {
        public string id { get; set; }

        public bool hasSession( IFastGlobals globals )
        {
                try
                {
                    var dummy = getSession(globals);
                }
                catch
                {
                    return false;
                }

                return true;
        }

        public sessionID()
        {
            this.id = string.Empty;
        }
        public sessionID(string id)
        {
            this.id = id;
        }

        public ISession getSession(IFastGlobals globals)
        {
            string sessionFilePath = Path.Combine(globals.folderPath(fastLocations.sessions), string.Format("{0}.session", id));
            if (!File.Exists(sessionFilePath))
            {
                throw new Exception("No active session found");
            }

            ISession session = new serializer<ISession>(globals).deserialize(sessionFilePath, "::activeSession[1]");
            string sessionIDFilePath = Path.Combine(globals.folderPath(fastLocations.sessions), string.Format("{0}.sessionID", session.user.id));

            if (!File.Exists(sessionIDFilePath))
            {
                throw new Exception("No user information found");
            }
            return session;
        }
    }

}