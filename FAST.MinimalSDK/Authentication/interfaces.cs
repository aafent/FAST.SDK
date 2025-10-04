using FAST.Core;

namespace FAST.Authentication
{

    public interface IsessionToken
    {
        bool isValid { get; }
        string token { get; set; }
        string userName { get; }
        string userPassword { get; }
        dynamic data { get; set; }

        void setUser(string name, string password);
    }



    //
    // (v) FUTURE USE
    //

    public interface ISessionID
    {
        string id { get; set; }
        bool hasSession(IFastGlobals globals);
        ISession getSession( IFastGlobals globals );
    }

    public interface ISession
    {
        string id { get; set; }
        dynamic user { get; set; }
        DateTime loginTime { get; set; }
        ISessionID getSessionID<T>() where T : ISessionID;
        void forget(IFastGlobals globals);
    }

    public interface IUser
    {
        string id { get; set; }
        bool hasActiveSession { get; }
        ISession activeSession { get; }
    }

}