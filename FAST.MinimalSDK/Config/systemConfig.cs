namespace FAST.Config
{
    /// <summary>
    /// systemConfig class makes it easy to get values from AppSettings.
    /// </summary>
    public class systemConfig
    {
        private static Dictionary<string, object> cache = new Dictionary<string, object>();
        private bool supportCache = true;

        public bool noCache
        {
            get
            {
                return supportCache;
            }
            set
            {
                supportCache = value;
                if (!supportCache) clearCache();
            }
        }
        public void clearCache()
        {
            cache.Clear();
        }

        public systemConfig()
        {
            this.loadKeyValueMethod= loadKeyValue;
        }
        public systemConfig(loadKeyValueDelegate method)
        {
            this.loadKeyValueMethod = method;
        }

        public delegate object loadKeyValueDelegate(string key);
        public loadKeyValueDelegate loadKeyValueMethod;

        // (v) default method
        private object loadKeyValue(string key)
        {
#if ( NET40 )
            return System.Configuration.ConfigurationSettings.AppSettings[key];
#elif (NETCOREAPP)
            throw new PlatformNotSupportedException("Not supported by this platform. Try to use loadKeyValueMethod delegation");
#else
            return ConfigurationManager.AppSettings[key];  
#endif
        }
        private object keyValue(string key)
        {
            if (cache.ContainsKey(key))
            {
                return cache[key];
            }
            else
            {
                cache.Add(key, loadKeyValueMethod(key));
                return cache[key];
            }
        }


        public string toString(string key, string defaultValue)
        {
            if ( keyValue(key) == null)
                return defaultValue;
            else
                return ( keyValue(key).ToString() );
        }

        public int toInt32(string key, int defaultValue)
        {
            if ( keyValue(key) == null)
                return defaultValue;
            else
                return (Int32.Parse(keyValue(key).ToString()));
        }

        public double toDouble(string key, double defaultValue)
        {
            if ( keyValue(key) == null)
                return defaultValue;
            else
                return (Double.Parse(keyValue(key).ToString()));
        }

        public bool toBoolean(string key, bool defaultValue)
        {
            var value = keyValue(key);
            if (value == null)
            {
                return defaultValue;
            }
            else
            {
                bool result;
                string stringValue = value.ToString().ToLower();
                if (!bool.TryParse(stringValue , out result))
                {
                    return Strings.converters.toBoolean(stringValue , defaultValue);
                }
                return result;
           }
        }



        public static string getString(string key, string defaultValue)
        {
            return new systemConfig().toString(key, defaultValue);
        }
        public static int getInt32(string key, int defaultValue)
        {
            return new systemConfig().toInt32(key, defaultValue);
        }
        public static bool toDouble(string key, bool defaultValue)
        {
            return new systemConfig().toBoolean(key, defaultValue);
        }

    }

}
