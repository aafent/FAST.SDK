using FAST.Core;
using FAST.Core.Models;
using FAST.Strings;

namespace FAST.Config
{
    /// <summary>
    /// Application Maintenance basic class
    /// Only for FAST.Core set of namespaces. 
    /// This does not support FAST.Services.
    /// </summary>
    public abstract class applicationMaintenanceBase
    {
        protected readonly string repairToken = "marika";
        protected errorParser errors = new();

        protected Queue<string> directoriesToCreate = new Queue<string>();
        protected Queue<Tuple<string, string>> directoriesToEmpty = new Queue<Tuple<string, string>>();
        protected Queue<Type> classesToReset = new Queue<Type>();
        protected Dictionary<fastComponent,string> fastComponentMinimumVersion = new();

        /// <summary>
        /// More Actions to run during maintenance procedure.
        /// <example> maint.moreActionsForMaintenance = ()=>{___your_code_here___}</example>
        /// </summary>
        public Action moreActionsForMaintenance = null;

        /// <summary>
        /// Check if the FAST Services are configured and can be used by the maintenance flow
        /// </summary>
        public virtual bool isFastServicesConfigured { get; } = false;


        /// <summary>
        /// Check if the repair token is valid
        /// </summary>
        /// <param name="repairTokenToCheck">The repair token to check</param>
        /// <returns>true if it is valid</returns>
        public bool isRepairTokenValid(string repairTokenToCheck)
        {
            bool isOk = repairTokenToCheck == repairToken;
            if (! isOk )
            {
                isOk=converters.configValueToSensitiveValue("fixit", repairTokenToCheck) == repairToken;
            } 
            return isOk;
        }
        [Obsolete("Replace with: isRepairTokenValid() same arguments")]
        public bool isRepareTokenValid(string repairTokenToCheck) => isRepairTokenValid(repairTokenToCheck);


        /// <summary>
        /// Main method to invoke system repair
        /// </summary>
        public abstract void repairSystem();

        /// <summary>
        /// Add a directory to empty it
        /// </summary>
        /// <param name="folderPath">The full folder path</param>
        /// <param name="filePattern">The file pattern</param>
        protected void addDirectoryToEmpty(string folderPath, string filePattern)
        {
            directoriesToEmpty.Enqueue(new Tuple<string, string>(folderPath, filePattern));
        }


        /// <summary>
        /// Add Folder (directory) to create if it is missing
        /// </summary>
        /// <param name="folderPath">The full folder path</param>
        protected void addDirectoryToCreate(string folderPath)
        {
            directoriesToCreate.Enqueue(folderPath);
        }

        /// <summary>
        /// Add locations as directories to create
        /// </summary>
        /// <typeparam name="folderLocations"></typeparam>
        /// <param name="fastGlobals">The FAST Globals</param>
        /// <param name="excludeList">Optional, a List with locations to exclude</param>
        public void addDirectoriesToCreate<folderLocations>(IFastGlobals fastGlobals,
                                                            List<folderLocations> excludeList = null)
            where folderLocations : System.Enum
        {
            //var folders = Enum.GetValues(typeof(folderLocations)).Cast<folderLocations>();
            var folders = genericsHelper.EnumToEnumerable<folderLocations>();
            foreach (var location in folders)
            {
                if (excludeList != null) if (excludeList.Contains(location)) continue;
                this.addDirectoryToCreate(fastGlobals.folderPath(location));
            }
        }

        /// <summary>
        /// Add resettable classes
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="Exception"></exception>
        protected void addClassToReset(Type type)
        {

            if (reflectionHelper.containsAnyOfTheInterfaces(type, nameof(Iresettable)))
            {
                classesToReset.Enqueue(type);
            }
            else
            {
                throw new Exception($"Class: {type} does not implements the interfaces Iresettable");
            }
        }

        /// <summary>
        /// Add minimum required version for FAST Components 
        /// </summary>
        /// <param name="component"></param>
        /// <param name="version"></param>
        protected void addFastComponentMinimumVersion(fastComponent component, string version)
        {
            if (fastComponentMinimumVersion.ContainsKey(component))
            {
                fastComponentMinimumVersion[component] = version;
            }
            else
            {
                fastComponentMinimumVersion.Add(component,version);
            }
        }
        [Obsolete("Replace with: addFastComponentMinimumVersion() same arguments")]
        protected void addFastComponentMinimunVersion(fastComponent component, string version) => addFastComponentMinimumVersion(component,version);    


        /// <summary>
        /// Reset all the requested classes
        /// </summary>
        protected void resetClasses()
        {
            foreach (var classType in classesToReset)
            {
                var obj = Activator.CreateInstance(classType);
                if (obj is Iresettable) ((Iresettable)obj).reset();
                obj = null;
            }
        }

        /// <summary>
        /// Force a garbage collection
        /// </summary>
        protected void forceGC()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /*
        /// <summary>
        /// Check for the minimum FAST Component Versions
        /// </summary>
        protected void checkForMinimumFASTComponentVersions()
        {
            return;
        }
        */


        /// <summary>
        /// Get an enumeration with errors
        /// </summary>
        /// <returns></returns>
        public ImultiErrorCarrier getErrors()
        {
            return errors.multiError;
        }
    }


}
