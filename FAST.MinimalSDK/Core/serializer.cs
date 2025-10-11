#if NET6_0_OR_GREATER // (<) copy from FAST_LOCALS.cs
#define FAST_MINNET6
#elif NETCOREAPP
#define FAST_NEWTONSOFT
#else
#define FAST_MAX478
#endif


using System.Xml.Serialization;
#if FAST_NEWTONSOFT 
using Newtonsoft.Json; 
#elif FAST_MAX478 
using System.Web.Script.Serialization;
#elif  FAST_MINNET6
#endif





namespace FAST.Core
{
    /// <summary>
    /// Base serializer class to handle XML and JSON serialization and deserialization
    /// </summary>
    public class baseSerializer
    {
        public bool newtonsoftJson = false;

        public Object deserialize(Type objectType, string xmlFilePath, string contexDescription)
        {
            Object objectToReturn;
            if (!File.Exists(xmlFilePath)) { throw new Exception("XML file needed for deserialization cannot be found. File path is: " + xmlFilePath); }

            XmlSerializer mySerializer = new XmlSerializer(objectType);
            using (FileStream XMLfileToRead = new FileStream(xmlFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                try
                {
                    objectToReturn = mySerializer.Deserialize(XMLfileToRead) ?? new object(); // 2022-04-28
                }
                catch (Exception ex)
                {
                    throw new Exception("Cannot deserialize. " + contexDescription, ex);
                }
                finally
                {
                    XMLfileToRead.Close();
                }
            }
            return objectToReturn;
        }
        public void serialize(Object objectToSerialize, Type objectType, string xmlFilePath, string contexDescription)
        {
            XmlSerializer serializer = new XmlSerializer(objectType);

            using (StreamWriter xmlFileToWrite = new StreamWriter(xmlFilePath))
            {
                try
                {
                    serializer.Serialize(xmlFileToWrite, objectToSerialize);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Cannot serialize {0}\n\n{1} ",contexDescription,ex),ex);
                }
                finally
                {
                    xmlFileToWrite.Close();
                }
            }
        }

        public void serialize(Object objectToSerialize, Type objectType, TextWriter writer, string contexDescription)
        {
            XmlSerializer serializer = new XmlSerializer(objectType);
            try
            {
                serializer.Serialize(writer, objectToSerialize);
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot serialize: " + contexDescription, ex);
            }
            return;
        }
        public Object deserialize(Type objectType, TextReader reader, string contexDescription)
        {
            XmlSerializer serializer = new XmlSerializer(objectType);
            try
            {
                return serializer.Deserialize(reader) ?? emptyObject.instance();
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot deserialize." + contexDescription, ex);
            }
        }

        public T jsonDeserialize<T>(string json)
        {
            if (newtonsoftJson)
            {
#if FAST_NEWTONSOFT 
                return (T)JsonConvert.DeserializeObject(json);
#else
                throw new NotImplementedException();
#endif
            }
            else
            {
                return (T)new jsonSerializer().deserialize<T>(json);
                // for .NET47x was: return new JavaScriptSerializer().Deserialize<T>(json); logic xfered to jsonSerializer class
            }
        }

        public string jsonSerialize<T>(T objectToSerialize)
        {
            if (newtonsoftJson)
            {
#if FAST_NEWTONSOFT 
                return JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented);
#else
                throw new NotImplementedException();          
#endif
            }
            else
            {
                return new jsonSerializer().serialize(objectToSerialize);
                // was return JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented);
                // was return new JavaScriptSerializer().Serialize(objectToSerialize);
            }
        }


        public static T convert<T>(object obj)
        {
            baseSerializer ser = new baseSerializer();
            using (StringWriter writer = new StringWriter())
            {
                ser.serialize(obj, obj.GetType(), writer, "convert");
                using (StringReader reader = new StringReader(writer.ToString()))
                {
                    return (T)ser.deserialize(typeof(T), reader, "convert");
                }
            }

            
        }
    }



    public class serializer<T> : baseSerializer, IFastGlobalsSupport
    {
        private IFastGlobals fastGlobals;

        public serializer(IFastGlobals globals)
        {
            bindFastGlobalsFrom(globals);
        }

        public void serialize(T objectToSerialize, Enum file, string contextDescription)
        {
            serialize(objectToSerialize, file, fastGlobals.defaultFileVersion, contextDescription);
        }
        public void serialize(T objectToSerialize, Enum file, Enum version, string contextDescription)
        {
            this.serialize(objectToSerialize, objectToSerialize.GetType(), fastGlobals.filePath(file, version), contextDescription);
        }
        public void serialize(T objectToSerialize, Enum location, string fileName, string contextDescription)
        {
            this.serialize(objectToSerialize, objectToSerialize.GetType(), fastGlobals.filePath(location, fastGlobals.defaultFileVersion, fileName, ""), contextDescription);
        }
        public void serialize(T objectToSerialize, string fullPathName, string contextDescription)
        {
            this.serialize(objectToSerialize, objectToSerialize.GetType(), fullPathName, contextDescription);
        }

        public T deserialize(Enum file, string contextDescription)
        {
            return deserialize(file, fastGlobals.defaultFileVersion, contextDescription);
        }
        public T deserialize(Enum file, Enum version, string contextDescription)
        {
            Type typeParameterType = typeof(T);
            return (T)this.deserialize(typeParameterType, fastGlobals.filePath(file, version), contextDescription);
        }
        public T deserialize(Enum location, string fileName, string contextDescription)
        {
            Type typeParameterType = typeof(T);
            return (T)this.deserialize(typeParameterType, fastGlobals.filePath(location,fastGlobals.defaultFileVersion, fileName, ""), contextDescription);
        }
        public T deserialize(string fullpathName, string contextDescription)
        {
            Type typeParameterType = typeof(T);
            return (T)this.deserialize(typeParameterType, fullpathName, contextDescription);
        }

        public T jsonDeserialize(string json)
        {
                return  new jsonSerializer().deserialize<T>(json);
                // was return (T)JsonConvert.DeserializeObject(json);
                // was return new JavaScriptSerializer().Deserialize<T>(json);
        }
        public string jsonSerialize(T objectToSerialize)
        {
            return new jsonSerializer().serialize(objectToSerialize);
             
            // was: return JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented);
            // was: return new JavaScriptSerializer().Serialize(objectToSerialize);
            
        }

        public void bindFastGlobalsFrom(IFastGlobals globals)
        {
            this.fastGlobals = globals;
        }

        public void bindFastGlobalsTo(IFastGlobalsSupport otherObject)
        {
            otherObject.bindFastGlobalsFrom(this.fastGlobals);
        }

        public static void serializeToFile(T obj, string filePath, string contextDescription)
        {
            new baseSerializer().serialize(obj, typeof(T), filePath, contextDescription);
        }

        public static T deserializeFromFile(string filePath, string contextDescription)
        {
            return (T)new baseSerializer().deserialize(typeof(T), filePath, contextDescription);
        }


    }


    /* TODO: 
     * SERIALIZE:
            using (var fileStream = new FileStream(@"C:\VentusRepository\Temporary\lala.task", FileMode.Create))
            using(StreamReader reader = new StreamReader(fileStream)) 
            {
                DataContractSerializer serializer = new DataContractSerializer(task.GetType());
                serializer.WriteObject(fileStream, task);
                fileStream.Position = 0;
                reader.ReadToEnd();
            }

     * 
     * 
     * DESERIALIZE:
    public static object Deserialize(string xml, Type toType) {
        using(Stream stream = new MemoryStream()) {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
            stream.Write(data, 0, data.Length);
            stream.Position = 0;
            DataContractSerializer deserializer = new DataContractSerializer(toType);
            return deserializer.ReadObject(stream);
        }
    */

}
