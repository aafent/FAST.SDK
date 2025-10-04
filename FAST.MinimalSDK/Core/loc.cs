namespace FAST.Core
{
    public class loc
    {
        private static global::System.Resources.ResourceManager RM;
        private static string rm_NameSpace = "Resources";
        private static string rm_Object = "";
        private static string rm_Assembly = "App_GlobalResources";
        private static string rm_localityID = "en";
        private static string rm_application_localityID = "en";
        public static string resourceNameSpace 
        {
            set
            {
                rm_NameSpace = value;
                resetResources();
            }
        }
        public static string resourceObject 
        {
            set
            {
                rm_Object = value;
                resetResources();
            }
        }
        public static string resourceAssembly
        {
            set
            {
                rm_Assembly = value;
                resetResources();
            }
        }
        public static string localityID
        {
            get
            {
                return rm_localityID;
            }
            set
            {
                rm_localityID = value;
            }
        }
        public static string applicationLocalityID
        {
            get
            {
                return rm_application_localityID;
            }
            set
            {
                rm_application_localityID = value;
            }
        }
        private static void resetResources()
        {
            if ( string.IsNullOrEmpty(rm_NameSpace) || string.IsNullOrEmpty(rm_Object) || string.IsNullOrEmpty(rm_Assembly) ) { return; }
            RM = new global::System.Resources.ResourceManager(rm_NameSpace + "." + rm_Object, global::System.Reflection.Assembly.Load(rm_Assembly));
        }
        public static string t(string name)
        {
            try
            {
                return RM.GetString(name);
            }
            catch
            {
                return ":" + name + ":";
            }
            
        }
        public static string t(string name, string text)
        {
            if (string.IsNullOrEmpty(name))
            {
                return text;
            }
            if (rm_localityID == rm_application_localityID)
            {
                return text;
            }
            else
            {
                return t(name);
            }
        }
        public static string t(int numericID, string text)
        {
            return t(numericID.ToString(), text);
        }


        public loc(string resourceObject)
        {
            loc.resourceObject = resourceObject;
        }
        public string this[string name]
        {
            get
            {
                return RM.GetString(name);
            }
        }

        public static DateTime utcToLocaltime(DateTime utcDateTime)
        {
            DateTime runtimeKnowsThisIsUtc = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            return runtimeKnowsThisIsUtc.ToLocalTime();
        }

    }
}
