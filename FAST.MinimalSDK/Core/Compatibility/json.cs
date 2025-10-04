using FAST.Strings;
#if !NETCOREAPP
using System.Web.Script.Serialization;
#else
#if NET6_0_OR_GREATER
#else
    using Newtonsoft.Json;
#endif
#endif

#if NETCOREAPP 
#else
#endif

#if NET6_0_OR_GREATER 
#else
#endif


namespace FAST.Core
{
    /// <summary>
    /// Wrapper to the JSON Serializer, using the FAST internal setup
    /// As it is ancient code, use the .NET Json serializer
    /// Original code (signatures) was the .NET Framework 1.0 SP1 back to December 2002
    /// </summary>
    public class json
    {
        public json()
        {
        }

        private string buffer = "";
        public object objectToSerialize
        {
            set
            {
                javaScriptSerializer serializer=javaScriptSerializerINSTANCE();
                buffer = serializer.Serialize(value);
            }
        }
        public string alreadySerializedObject
        {
            set
            {
                this.buffer = value;
            }
        }

        public override string ToString()
        {
            return buffer;
        }
        public T toObject<T>()
        {
            var serializer = javaScriptSerializerINSTANCE();
            return serializer.Deserialize<T>(buffer);
        }
        public object toAnyObject()
        {
            var serializer = javaScriptSerializerINSTANCE();
            return serializer.DeserializeObject(buffer);
        }
        public void toFile( string filePath )
        {
            converters.stringToFile(filePath, ToString() );
        }

        public static json fromString(string jsonString)
        {
            json jsonObject = new json();
            jsonObject.alreadySerializedObject = jsonString;
            return jsonObject;
        }
        public static json fromObjectCache(objectCache cache)
        {
            return fromString(cache.read());
        }
        public static json fromFile(string filePath, string encodingName="" )
        {
            return fromString(converters.fileToString(filePath, false, encodingName));
        }
        public static json toJson(object objectToSerialize)
        {
            json jsonObject = new json();
            jsonObject.objectToSerialize = objectToSerialize;
            return jsonObject;
        }
        public static T convert<T>( object anyObject )
        {
            return toJson(anyObject).toObject<T>();
        }


        public static javaScriptSerializer javaScriptSerializerINSTANCE()
        {
#if NETCOREAPP
            bool enableConverters = false;
#else
            bool enableConverters = true; // it was method argument
#endif


            javaScriptSerializer serializer = new javaScriptSerializer();
            if (enableConverters)
            {
#if NETCOREAPP
                throw new NotImplementedException("enableConverters=true / Is not supported for .NET Core and .NET6 or newer");
#else
                serializer.RegisterConverters(new JavaScriptConverter[] { new ExpandoJSONConverter() });
#endif
            }
            return serializer;
        }

  

    }


}
