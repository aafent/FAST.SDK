#if !NETCOREAPP
using System.Web.Script.Serialization;
#else
#if NET6_0_OR_GREATER
using System.Text.Json;
#else
    using Newtonsoft.Json;
#endif
#endif


namespace FAST.Core
{
    /// <summary>
    /// Wrapper to the JSON Serializer, using the FAST internal setup and compatibility 
    /// As it is ancient code, use the .NET Json serializer
    /// Original code (signatures) was the .NET Framework 1.0 SP1 back to December 2002
    /// </summary>
    public class jsonSerializer
    {
#if NETCOREAPP
#else
       private JavaScriptSerializer serializer = new JavaScriptSerializer();
#endif



        public string serialize(object obj)
        {
#if NETCOREAPP 
#if NET6_0_OR_GREATER
            return JsonSerializer.Serialize(obj,obj.GetType()); 
            //return JsonSerializer.Serialize(obj);
#else
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
#endif
#else
            return serializer.Serialize(obj);
#endif
        }


        public string serialize(object obj, int maxJsonLength )
        {
#if NETCOREAPP
            return serialize(obj);
#else
            return new JavaScriptSerializer() { MaxJsonLength = maxJsonLength }.Serialize(obj);
#endif
        }

        public T deserialize<T>(string input)
        {
#if NETCOREAPP 

            var obj =JsonSerializer.Deserialize<T>(input);
            if ( obj == null )
            {
                throw new NullReferenceException($"Json to deserialize::1:: returns null value");
            }
            return (T)obj;
#if NET6_0_OR_GREATER
#else
            return (T)JsonConvert.DeserializeObject(input);
#endif
#else
            return serializer.Deserialize<T>(input);
#endif
        }
        public T deserialize<T>(string input, int maxJsonLength)
        {
#if NETCOREAPP 
            return deserialize<T>(input);
#else
            return new JavaScriptSerializer() { MaxJsonLength = maxJsonLength }.Deserialize<T>(input);
#endif
        }

        public object deserialize(string input, Type targetType)
        {
#if NETCOREAPP
#if NET6_0_OR_GREATER
            var obj = JsonSerializer.Deserialize(input, targetType);
            if (obj == null)
            {
                throw new NullReferenceException($"Json to deserialize::2:: returns null value");
            }
            return obj;
#else
            return JsonConvert.DeserializeAnonymousType(input, targetType);
#endif
#else
            return new JavaScriptSerializer().Deserialize(input, targetType);
#endif
        }

        public object deserializeObject(string input)
        {
#if NETCOREAPP
#if NET6_0_OR_GREATER
            throw new NotImplementedException("DeserializeObject() is not supported for .NET6");
#else
            return (object)JsonConvert.DeserializeObject(input);
#endif

#else
            return serializer.DeserializeObject(input);
#endif
        }

#if NETCOREAPP 
        public void registerConverters(IEnumerable<object> converters)
        {
            throw new NotImplementedException("RegisterConverters() is not supported for .NET Core");
        }
#else
        public void RegisterConverters(IEnumerable<JavaScriptConverter> converters)
        {
            serializer.RegisterConverters(new JavaScriptConverter[] { new ExpandoJSONConverter() });
        }
#endif
    }









    public class javaScriptSerializer : jsonSerializer //(<) wrapper class for compatibility 
    {
        public string Serialize(object obj)
        {
            return serialize(obj);
        }
        public string Serialize(object obj, int maxJsonLength)
        {
            return serialize(obj, maxJsonLength);
        }
        public T Deserialize<T>(string input)
        {
            return deserialize<T>(input);
        }
        public T Deserialize<T>(string input, int maxJsonLength)
        {
            return deserialize<T>(input, maxJsonLength);
        }
        public object Deserialize(string input, Type targetType)
        {
            return deserialize(input, targetType);
        }
        public object DeserializeObject(string input)
        {
            return deserializeObject(input);
        }

#if NETCOREAPP
        public void RegisterConverters(IEnumerable<object> converters)
        {
            registerConverters(converters);
            return;
        }
#endif
    }














#if NETCOREAPP 
    public class ExpandoJSONConverter
    {
    }
#else
    public class ExpandoJSONConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {         
            var result = new Dictionary<string, object>();
            var dictionary = obj as IDictionary<string, object>;
            foreach (var item in dictionary)
                result.Add(item.Key, item.Value);
            return result;
        }
        public override IEnumerable<Type> SupportedTypes
        {
            get 
            { 
                  return new ReadOnlyCollection<Type>(new Type[] { typeof(System.Dynamic.ExpandoObject) });
            }
        }
    }

#endif

}
