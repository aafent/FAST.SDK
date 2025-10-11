using FAST.Core;

namespace FAST.Config
{
    public class configGate : IFastGlobalsUsingOnlySupport
    {
        private string configHome=null; 
        private string exampleHome=null;
        private Enum configLocation=null;
        private Enum examplesLocation=null;
        private string lastUsedName = null;
        private string configFilePath
        {
            get
            {
                return Path.Combine(configHome,lastUsedName+".config");
            }
        }
        private string exampleFilePath
        {
            get
            {
                return Path.Combine(exampleHome, lastUsedName + ".config");
            }
        }


        public bool supportExamples
        {
            get
            {
                return string.IsNullOrEmpty(exampleHome);
            }
        }

        public string name
        {
            get
            {
                return lastUsedName;
            }
            set
            {
                lastUsedName=value;
            }
        }
        
        public configGate(IFastGlobals globals, Enum configLocation, Enum examplesLocation)
        {
            this.configLocation=configLocation;
            this.examplesLocation=examplesLocation;
            bindFastGlobalsFrom(globals);
        }
        public configGate(IFastGlobals globals, Enum configLocation) : this(globals, configLocation, null )
        {
        }
        public configGate(IFastGlobals globals) : this(globals,fastLocations.config, fastLocations.example)
        {
        }
        public configGate(string pathToConfigFolder, string pathToExamplesFolder)
        {
            this.configHome=pathToConfigFolder;
            this.exampleHome=pathToExamplesFolder;
        }
        public configGate(string pathToConfigFolder) : this(pathToConfigFolder,null)
        {

        }


        public void bindFastGlobalsFrom(IFastGlobals globals)
        {
            if (configLocation != null) configHome=globals.folderPath(configLocation);
            else throw new ArgumentNullException("Instance created using a wrong constructor (without FastGlobals support)");
            
            if (examplesLocation != null) exampleHome = globals.folderPath(examplesLocation);
        }

        public void save(string name, string whatToSave)
        {
            this.lastUsedName=name;
            Strings.converters.stringToFile(configFilePath,whatToSave);
            return;
        }
        public void save(IdefaultConfiguration config)
        {
            this.lastUsedName= config.defaultConfigFileName;
            new baseSerializer().serialize(config, config.GetType(), configFilePath, "::configGate->"+lastUsedName);
            return;
        }


        public string load(string name)
        {
            this.lastUsedName=name;
            return load();
        }
        public string load()
        {
            return Strings.converters.fileToString(configFilePath, false);
        }
        public T load<T>(IdefaultConfiguration objectToGetNameFrom)
        {
            this.lastUsedName = objectToGetNameFrom.defaultConfigFileName;
            return (T)load<T>();
        }
        public T load<T>(string name)
        {
            this.lastUsedName = name;
            return (T)load<T>();
        }
        public T load<T>()
        {
            return (T)new baseSerializer().deserialize(typeof(T), configFilePath, "::configGate(load)->" + lastUsedName);
        }

        public bool exists(string name)
        {
            this.lastUsedName=name;
            return exists();
        }
        public bool exists()
        {
            return File.Exists(Path.Combine(configFilePath, lastUsedName));
        }



        public void example(string name, string whatToSave)
        {
            if ( !supportExamples ) throw new Exception("This configGate does not support examples");
            this.lastUsedName=name;
            Strings.converters.stringToFile(exampleFilePath, whatToSave);
        }
        public void example(string name, Iexample config)
        {
            this.lastUsedName=name;
            config.example(exampleFilePath);
        }
        public void example(IdefaultConfiguration config)
        {
            this.lastUsedName=config.defaultConfigFileName;
            if ( config is Iexample )
            {
                example(lastUsedName, (Iexample)config);
            } else
            {
                new baseSerializer().serialize(config, config.GetType(), exampleFilePath, "::configGate:example->" + lastUsedName);
            }
        }

    }
}
