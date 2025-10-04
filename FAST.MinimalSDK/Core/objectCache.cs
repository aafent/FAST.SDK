using System.Text;

namespace FAST.Core
{
    public class objectCache
    {
        public class cacheContainer
        {
            public DateTime created;
            public string name;
            public string json;
            public string stringTimeToLive;
        }

        private cacheContainer cache = new cacheContainer();
        private string cacheFilePath
        {
            get
            {
                return Path.Combine(Path.GetTempPath(), string.Format("{0}.cache", this.cache.name));
            }
        }

        private int maxJsonLength
        {
            get
            {
                return (500 * (1024 * 1024 / 2)); // =500MB of unicode characters
            }
        }

        public objectCache(string name)
        {
            this.timeToLive = new TimeSpan(0, 5, 0);
            this.cache.name = name;
            if (exists) { read(); }
        }
        public objectCache(string name, string json)
        {
            this.timeToLive = new TimeSpan(0, 5, 0);
            this.cache.name = name;
            setObject(json);
        }
        public objectCache(string name, object objectToCache)
        {
            this.timeToLive = new TimeSpan(0, 5, 0);
            this.cache.name = name;
            setObject(objectToCache);
        }

        public bool exists
        {
            get
            {
                return new FileInfo(cacheFilePath).Exists;
            }
        }
        public bool expired
        {
            get
            {
                if (!exists) { return true; }
                DateTime now = DateTime.UtcNow;
                var timeDiff = now.Subtract(cache.created);
                if (timeDiff > this.timeToLive)
                {
                    return true;
                }
                return false;
            }
        }
        public TimeSpan timeToLive
        {
            set
            {
                cache.stringTimeToLive = value.ToString();
            }
            get
            {
                return TimeSpan.Parse(cache.stringTimeToLive);
            }
        }

        public void setObject(string json)
        {
            cache.created = DateTime.UtcNow;
            this.cache.json = json;
        }
        public void setObject(object objectToCache)
        {
            cache.created = DateTime.UtcNow;
            this.cache.json = new javaScriptSerializer().Serialize(objectToCache, maxJsonLength);
            //it was: this.cache.json = new JavaScriptSerializer() { MaxJsonLength = maxJsonLength }.Serialize(objectToCache);
        }

        private string fileToString(string fileName, bool useAppendLine, string encodingName = "")
        {
            Encoding fileEncoding;
            if (String.IsNullOrEmpty(encodingName))
            {
                fileEncoding = Encoding.Default;
            }
            else
            {
                fileEncoding = Encoding.GetEncoding(encodingName);
            }
            StringBuilder toReturn = new StringBuilder();
            string line = "";
            using (StreamReader sr = new StreamReader(fileName, fileEncoding))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    if (useAppendLine)
                    {
                        toReturn.AppendLine(line);
                    }
                    else
                    {
                        toReturn.Append(line);
                    }
                }
            }
            return toReturn.ToString();
        }

        public void keep()
        {
            if (string.IsNullOrEmpty(cache.json))
            {
                throw new Exception(string.Format("Cannot keep() non populated object", cache.name));
            }
            
            string serializedCache = new javaScriptSerializer().Serialize(cache,maxJsonLength );
            //it was: string serializedCache = new JavaScriptSerializer() { MaxJsonLength = maxJsonLength }.Serialize(cache);
            FAST.Strings.converters.stringToFile(cacheFilePath, serializedCache);
        }
        public void forget()
        {
            var file = new FileInfo(cacheFilePath);
            if (file.Exists) { file.Delete(); }
            cache.json = string.Empty;
        }
        public string read()
        {
            string serializedJson = fileToString(cacheFilePath, false);
            cache = new javaScriptSerializer().Deserialize<cacheContainer>(serializedJson,maxJsonLength);
            //it was: cache = new JavaScriptSerializer() { MaxJsonLength = maxJsonLength }.Deserialize<cacheContainer>(serializedJson);
            return cache.json;
        }
        
        public T toObject<T>()
        {
            //it was: var serializer = new JavaScriptSerializer() { MaxJsonLength = maxJsonLength };
            var serializer = new javaScriptSerializer();

            if (string.IsNullOrEmpty(cache.json)) { this.read(); }
            if (string.IsNullOrEmpty(cache.json)) { throw new Exception(string.Format("No data found in cache memory: {0}", this.cache.name)); }
            return serializer.Deserialize<T>(cache.json, maxJsonLength);
        }


    }
}