using FAST.Core;
using System.Xml.Serialization;

namespace FAST.Logging
{

    /// <summary>
    /// The configuration model for FAST logging mechanism
    /// </summary>
    public class loggerConfigurableParametersBase
    {
        [XmlAttribute]
        public string defaultWarningSource = "(!)";

        [XmlAttribute]
        public string defaultErrorSource = "(E)";

        [XmlAttribute]
        public string defaultFatalSource = "(F)";

        [XmlAttribute]
        public bool ignoreEmptyMessages = true;

        [XmlAttribute]
        public string defaultMessageFormat = "{l} {d}: {s}\t:{m}";  //d,s,m,e,l

        [XmlAttribute]
        public string defaultDatestampFormat = "dd/MM/yyyy HH:mm:ss.fff";

        [XmlArray("abilities")]
        [XmlArrayItem("ability")]
        public List<string> abilities = new List<string>();

    }

    /// <summary>
    /// The configuration for FAST logging mechanism
    /// </summary>
    public class loggerConfiguration : loggerConfigurableParametersBase
    {
        //
        // (>) Slots Map: [1=user1] [2=user2] [3=user3] [4=fast telemetry]
        //
        private const int _numberOfUserSlots = 3;
        private const int _numberOfSpecialSlots = 1;
        private const int numberOfSlots = (_numberOfUserSlots + _numberOfSpecialSlots); //aka 4

        /// <summary>
        /// Number of special slot for Fast Telemetry Service
        /// </summary>
        public const int slotOfFastTelemetry = (_numberOfUserSlots + 1);

        private IloggerAdapter[] adapterSlots = genericsHelper.initializeArrayWithDefault<IloggerAdapter>(numberOfSlots);
        private bool[] isSlotBinded = genericsHelper.initializeArrayWithDefault<bool>(numberOfSlots);
        private bool _lockConfiguration = false;

        #region (+) variables & properties for console redirection
        private StreamWriter writerForRedirectedConsole = null;
        private FileStream outputStreamForRedirectedConsole = null;
        private TextWriter oldErrorForRedirectedConsole = null;
        private TextWriter oldOutputForRedirectedConsole = null;
        #endregion (+) variables & properties  for console redirection


        /// <summary>
        /// Constructor without parameters
        /// </summary>
        public loggerConfiguration()
        {
        }

        /// <summary>
        /// Constructor with parameters model
        /// </summary>
        /// <param name="parameter">The parameters model</param>
        public loggerConfiguration(loggerConfigurableParametersBase parameter):this()
        {
            this.setParameters(parameter);
        }

        /// <summary>
        /// True (default) to enable debug
        /// </summary>
        public bool enableDebug = true;

        /// <summary>
        /// True (default) to enable trace
        /// </summary>
        public bool enableTrace = true;

        /// <summary>
        /// True if there is a console redirection to a file
        /// </summary>
        public bool hasConsoleRedirectionToFile
        {
            get
            {
                return !(outputStreamForRedirectedConsole == null);
            }
        }

        /// <summary>
        /// If true and if a console redirection is action, will write in both console and the file
        /// </summary>
        public bool logOnBothFileAndConsole { get; private set; } = false;


        /// <summary>
        /// Dictionary indeing levels and logging engines
        /// </summary>
        public Dictionary<levels, List<loggerEngines>> loggerRoutes = new Dictionary<levels, List<loggerEngines>>();
        
        /// <summary>
        /// Get an adapter based on the slot
        /// </summary>
        /// <param name="slot">Slot number</param>
        /// <param name="requestingLogger">Optional, The requesting Ilogger instance</param>
        /// <returns>IloggerAdapter, the adapter</returns>
        public IloggerAdapter slot(sbyte slot, Ilogger requestingLogger=null)
        {
            if (slot < 1 || slot > numberOfSlots) throw new ArgumentException($"Slot must be between 1 and {numberOfSlots}");
            IloggerAdapter adapter = adapterSlots[slot - 1];
            if (!isSlotBinded[slot - 1])
            {
                if (requestingLogger==null) throw new Exception($"Adapter of slot: {slot} cannot be binded with the logger. Specify the requestingLogger at least once. ");
                adapter.bind(requestingLogger);
                isSlotBinded[slot - 1] = true;
            }
            return adapter;
        }

        /// <summary>
        /// True if the configuration is locked
        /// </summary>
        public bool isConfigurationLocked
        {
            get
            {
                return _lockConfiguration;
            }
        }

        /// <summary>
        /// Lock the configuration, do not permit further changes
        /// </summary>
        /// <returns>The configurator itself</returns>
        public loggerConfiguration lockConfiguration()
        {
            if (isConfigurationLocked) { return this; }

            if (loggerRoutes.Count == 0)
            {
                addRoute(loggerEngines.console);
            }

            _lockConfiguration = true;

            return this;
        }

        /// <summary>
        /// Add/enable a routing output for specific level
        /// </summary>
        /// <param name="level">The level of logging</param>
        /// <param name="engine">The output engine</param>
        /// <returns>The configurator itself</returns>
        public loggerConfiguration addRoute(levels level, loggerEngines engine)
        {
            if (isConfigurationLocked) { return this; }
            if (! loggerRoutes.ContainsKey(level))
            {
                loggerRoutes.Add(level, new List<loggerEngines>() );
            }

            loggerRoutes[level].Add(engine);
            return this;
        }

        /// <summary>
        /// Add/enable a new routing for logging output
        /// </summary>
        /// <param name="engine">The output engine</param>
        /// <returns>The configurator itself</returns>
        public loggerConfiguration addRoute(loggerEngines engine)
        {
            if (isConfigurationLocked) { return this; }

            var levels = enumHelper.toList<levels>();
            foreach (var level in  levels )
            {
                addRoute(level, engine);
            } 

            return this;
 
        }

        /// <summary>
        /// Bind and adapter to the logger by creating additional routing output handler
        /// There are 3 slots for adapters
        /// </summary>
        /// <param name="slot">The slot number 1 to 3</param>
        /// <param name="adapter">The adapter, instance of a class implementing the IloggerAdapter interface</param>
        /// <returns>The configurator itself</returns>
        public loggerConfiguration bindAdapter(sbyte slot, IloggerAdapter adapter)
        {
            switch (slot)
            {
                case 1:
                    adapterSlots[0] = adapter;
                    adapterSlots[0].init();
                    //isSlotBinded[0]=true;

                    break;
                case 2:
                    adapterSlots[1] = adapter;
                    adapterSlots[1].init();
                    //isSlotBinded[1] = true;
                    break;
                case 3:
                    adapterSlots[2] = adapter;
                    adapterSlots[2].init();
                    //isSlotBinded[2] = true;
                    break;

                case slotOfFastTelemetry:
                    adapterSlots[slotOfFastTelemetry - 1] = adapter;
                    adapterSlots[slotOfFastTelemetry -1 ].init();
                    //isSlotBinded[slotOfFastTelemetry - 1] = true;
                    break;


                default:
                    throw new ArgumentException(string.Format("Invalid slot number: {0}. Supported slots are: 1,2 and 3", slot));
            }

            return this;
        }

        /// <summary>
        /// Set confiuration parameters based on model
        /// </summary>
        /// <param name="par">Configuration model</param>
        /// <returns>The configurator itself</returns>
        public loggerConfiguration setParameters(loggerConfigurableParametersBase par)
        {
            this.defaultWarningSource   =   par.defaultWarningSource;
            this.defaultErrorSource     =   par.defaultErrorSource;
            this.defaultFatalSource     =   par.defaultFatalSource;
            this.ignoreEmptyMessages    =   par.ignoreEmptyMessages;
            this.defaultMessageFormat   =   par.defaultMessageFormat;  //d,s,m,e,l
            this.defaultDatestampFormat =   par.defaultDatestampFormat;

            foreach (var ability in abilities)
            {
                switch (ability)
                {
                    case "debug":
                        this.enableDebug=true;
                        break;

                    case "trace":
                        this.enableTrace=true;
                        break;

                    case "logging.console":
                        this.addRoute(loggerEngines.console);
                        break;

                    case "logging.slot1":
                        this.addRoute(loggerEngines.otherSlot1);
                        break;
                    case "logging.slot2":
                        this.addRoute(loggerEngines.otherSlot2);
                        break;
                    case "logging.slot3":
                        this.addRoute(loggerEngines.otherSlot3);
                        break;

                }

            }
            return this;
        }


        /// <summary>
        /// Redirect the Console output & Error to a file. 
        ///   The will created if does not exists, otherwise will append new lines.
        /// </summary>
        /// <param name="folderPath">Folder Path</param>
        /// <param name="fileName">File name</param>
        /// <param name="writeInBothFileAndConsole">If true will write in both file and console each logged line</param>
        /// <returns>The configurator itself</returns>
        public loggerConfiguration redirectConsoleToFile(string folderPath, string fileName, bool writeInBothFileAndConsole=false)
        {
            this.logOnBothFileAndConsole= writeInBothFileAndConsole;
            string filePath = Path.Combine(folderPath, fileName);
            oldOutputForRedirectedConsole = Console.Out;
            oldErrorForRedirectedConsole = Console.Error;
            outputStreamForRedirectedConsole = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            writerForRedirectedConsole = new StreamWriter(outputStreamForRedirectedConsole);
            Console.SetOut(writerForRedirectedConsole);
            Console.SetError(writerForRedirectedConsole);
            return this;
        }

        /// <summary>
        /// Restores the console to file (output, errors) redirection
        /// </summary>
        public void restoreConsoleRedirection()
        {
            Console.SetOut(oldOutputForRedirectedConsole);
            Console.SetError(oldErrorForRedirectedConsole);
            writerForRedirectedConsole.Close();
            outputStreamForRedirectedConsole.Close();
        }

        /// <summary>
        /// Write a line to the original console when it is redirected to a file
        /// </summary>
        /// <param name="line"></param>
        public void writeLineToOriginalConsole(string line)
        {
            if (oldErrorForRedirectedConsole==null) return;
            oldErrorForRedirectedConsole.WriteLine(line);
        }

    }
}
