using FAST.Core.Interfaces;

namespace FAST.Core
{
    /// <summary>
    /// FAST File Locations 
    /// </summary>
    public enum fastLocations { home, config, example, sessions, temporary };

    /// <summary>
    /// FAST Files 
    /// </summary>
    public enum fastFiles { connections, license };

    /// <summary>
    /// FAST File Versions
    /// </summary>
    public enum fastFileVersions { official, backup }


    /// <summary>
    /// Interface for a FASTGlobals class. 
    /// Indicates that the class is the FASTGlobals class
    /// </summary>
    public interface IFastGlobals
    {
        /// <summary>
        /// A mapping between Locations and Real Folder Paths
        /// </summary>
        /// <param name="location">The Location Enumeration</param>
        /// <returns>String, a real Folder Path</returns>
        string folderPath(Enum location);

        /// <summary>
        /// A mapping between Locations, Versioned Files and Real File Paths
        /// </summary>
        /// <param name="location">The location enumeration</param>
        /// <param name="version">The version enumeration</param>
        /// <param name="fileName">The File Name</param>
        /// <param name="fileNameSuffix">A file suffix, will be added to the file name after the version specific file name part</param>
        /// <returns>string, A real file path</returns>
        string filePath(Enum location, Enum version, string fileName, string fileNameSuffix);

        /// <summary>
        /// A mapping between Versioned Files and Real File Paths
        /// </summary>
        /// <param name="file">The File enumeration</param>
        /// <param name="version">The Version enumeration</param>
        /// <returns>string, A real file path</returns>
        string filePath(Enum file, Enum version);

        /// <summary>
        /// Get Only, Defines the default file version
        /// </summary>
        Enum defaultFileVersion { get; }
    }

    /// <summary>
    /// Interface for classes that supporting biding of FASTGlobal from another class that supporting them
    /// </summary>
    public interface IFastGlobalsUsingOnlySupport
    {
        void bindFastGlobalsFrom(IFastGlobals globals);
    }


    /// <summary>
    /// Interface for classes that exposing FASTServices for biding by another classes 
    /// </summary>
    public interface IFastGlobalsSupport : IFastGlobalsUsingOnlySupport
    {
        /// <summary>
        /// Expose the FASTGlobals to a destination object 
        /// </summary>
        /// <param name="destination"></param>
        void bindFastGlobalsTo(IFastGlobalsSupport destination);
    }



    /// <summary>
    /// Build-in FASTGlobals class with virtual methods.
    /// </summary>
    public class defaultFASTGlobals : IFastGlobals
    {
        /// <summary>
        /// Map Location to a Real Folder Path
        /// </summary>
        /// <param name="location">The Build-In FASTLocation Enumeration</param>
        /// <returns>String, the Real Folder Path</returns>
        public virtual string folderPath(fastLocations location)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Map Location to a Real Folder Path
        /// </summary>
        /// <param name="location">The Location Enumeration</param>
        /// <returns>String, the Real Folder Path</returns>
        public virtual string folderPath(Enum location)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// A mapping between Locations, Versioned Files and Real File Paths
        /// Method is Virtual and can be overridden 
        /// </summary>
        /// <param name="location">The Location Enumeration</param>
        /// <param name="version">The Version Enumeration</param>
        /// <param name="fileName">The File Name</param>
        /// <param name="fileNameSuffix">A file suffix, will be added to the file name after the version specific file name part</param>
        /// <returns>String, The Real File Path</returns>
        public virtual string filePath(Enum location, Enum version, string fileName, string fileNameSuffix)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// A mapping between Versioned Files and Real File Paths
        /// Method is Virtual and can be overridden
        /// </summary>
        /// <param name="file">The File Enumeration</param>
        /// <param name="version">The Version Enumeration</param>
        /// <returns>String, The Real File Path</returns>
        public virtual string filePath(Enum file, Enum version)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Get Only, Defines the default file version
        /// Property's getter is Virtual and can be overridden 
        /// </summary>
        public virtual Enum defaultFileVersion
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Returns a VariablesBuffer containing the Locations as variables.
        /// The variable name is the item in the locations enumeration, the value is the folder path.
        /// </summary>
        /// <param name="locationsEnumeration">The Locations Enumeration</param>
        /// <param name="variablePrefix">Optional, A prefix to the variable names.</param>
        /// <param name="variables">Optional, a variablesBuffer to populate the variables</param>
        /// <returns>variablesBuffer with the locations as variables</returns>
        public variablesContainer folderPathToVariables(Type locationsEnumeration, string variablePrefix="", variablesContainer variables=null )
        {
            if ( variables == null ) variables = new variablesContainer();
            var items = Enum.GetValues(locationsEnumeration );
            var names=  Enum.GetNames(locationsEnumeration  );
            for ( int inx=0; inx< items.Length; inx++ )
            {
                string path = folderPath((Enum)items.GetValue(inx)  );
                variables.set(variablePrefix+names[inx], path);
            }
            return variables;
        }

        /// <summary>
        /// Returns a VariablesBuffer containing the Files Enumeration as variables.
        /// The variable name is the item in the locations enumeration, the value is the file path.
        /// </summary>
        /// <param name="filesEnumeration">The Files Enumeration</param>
        /// <param name="version">The file Version Enumeration</param>
        /// <param name="variablePrefix">Optional, A prefix to the variable names.</param>
        /// <param name="variables">Optional, a variablesBuffer to populate the variables</param>
        /// <returns>variablesBuffer with the locations as variables</returns>
        public variablesContainer filePathToVariables(Type filesEnumeration, Enum version, string variablePrefix="", variablesContainer variables=null )
        {
            if ( variables == null ) variables = new variablesContainer();
            var items = Enum.GetValues(filesEnumeration );
            var names=  Enum.GetNames(filesEnumeration  );
            for ( int inx=0; inx< items.Length; inx++ )
            {
                string path = filePath((Enum)items.GetValue(inx), version  );
                variables.set(variablePrefix+names[inx], path);
            }
            return variables;
        }


        private static defaultFASTGlobals _instance;
        /// <summary>
        /// Get a static instance of the defaultFASTGlobals
        /// </summary>
        public static defaultFASTGlobals instance
        {
            get
            {
                if (defaultFASTGlobals._instance == null) { defaultFASTGlobals._instance = new defaultFASTGlobals(); }
                return _instance;
            }
        }
    }



    /// <summary>
    /// The abstract version of the defaultFASTGlobals class
    /// </summary>
    public abstract class defaultFASTGlobalsAbstract : defaultFASTGlobals, IFastGlobalsHomeFolder
    {
        public override abstract string folderPath(fastLocations location);

        public override abstract string folderPath(Enum location);

        public override abstract string filePath(Enum location, Enum version, string fileName, string fileNameSuffix);

        public override abstract string filePath(Enum file, Enum version);

        /// <summary>
        /// Get Only, Defines the default file version
        /// Property's getter is Virtual and can be overridden 
        /// Default value is fastFileVersions.official
        /// </summary>
        public new virtual Enum defaultFileVersion
        {
            get
            {
               return fastFileVersions.official;
            }
        }

        /// <summary>
        /// The home folder of the application
        /// </summary>
        public abstract string homeFolder { get; }
        
    }

    /// <summary>
    /// The abstract version of the defaultFASTGlobals (Ver2) class
    /// </summary>
    public abstract class defaultFASTGlobalsVer2Abstract : defaultFASTGlobalsAbstract, IFastGlobalsVer2
    {
        /// <summary>
        /// A Mapping between the Location and a Folder Relative Path
        /// </summary>
        /// <param name="location">The Location Enumeration</param>
        /// <returns>String, The Relative Real Path of a Folder</returns>
        public abstract string folderRelativePath(Enum location);
    }


}
