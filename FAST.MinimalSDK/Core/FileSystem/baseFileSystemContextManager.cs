
namespace FAST.Core.FileSystem
{

    /// <summary>
    /// Base class to manage file system contexts, providing methods to interact with different file system entities.
    /// </summary>
    public abstract class baseFileSystemContextManager
    {

        /// <summary>
        /// Gets the inner file system manager instance.
        /// </summary>
        protected abstract IfileSystemManager inner_FileSystemManager { get; }

        /// <summary>
        /// Gets the file system manager instance.
        /// </summary>
        public IfileSystemManager FS
        {
            get
            {
                if (inner_FileSystemManager == null) throw new Exception("File System Manager is missing");
                return inner_FileSystemManager;
            }
        }

        /// <summary>
        /// Retrieves the context for libraries.
        /// </summary>
        /// <param name="getAllLibraries">True to return all the libraries</param>
        /// <returns>Collection of library entities</returns>
        public abstract IEnumerable<IfileSystemEntityModel> contextForLibraries(bool getAllLibraries = false);

        /// <summary>
        /// Generates a search pattern based on the provided context, set of files, and path to browse.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="setOfFiles">Which set of files</param>
        /// <param name="pathToBrowse">The path to browse</param>
        /// <returns></returns>
        public abstract string contextSearchPattern(string context, string setOfFiles, string pathToBrowse);

        /// <summary>
        /// Maps the provided context to its corresponding home folder, or returns null if not found.
        /// </summary>
        /// <param name="context">the context</param>
        /// <returns></returns>
        protected abstract string? inner_contextToHomeFolderOrNull(string context);

        /// <summary>
        /// Checks if the provided context is valid.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns></returns>
        public bool isValidContext(string context)
        {
            return !string.IsNullOrEmpty(inner_contextToHomeFolderOrNull(context) );

        }

        /// <summary>
        /// Retrieves the home folder associated with the given context.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns></returns>
        public string contextHomeFolder(string context)
        {
            var folder= inner_contextToHomeFolderOrNull(context);
            if ( string.IsNullOrEmpty(folder) )
            {
                throw new Exception($"{context} does not have home folder.");
            }
            return folder;
        }
    }



    [Obsolete("Use baseFileSystemContextManager instead")]
    public abstract class baseFileSystemContexManager2 : baseFileSystemContextManager
    {

        [Obsolete("Use contextSearchPattern() instead")]
        public abstract string contexSearchPattern(string context, string setOfFiles, string pathToBrowse);

        [Obsolete("Use inner_contextToHomeFolderOrNull() instead")]
        protected abstract string? inner_contexToHomeFolderOrNull(string context);

        [Obsolete("use isValidContext() instead")]
        public bool isValidContex(string context)
        {
            return !string.IsNullOrEmpty(inner_contexToHomeFolderOrNull(context));

        }

        [Obsolete("use contextHomeFolder() instead")]
        public string contexHomeFolder(string context)
        {
            var folder = inner_contexToHomeFolderOrNull(context);
            if (string.IsNullOrEmpty(folder))
            {
                throw new Exception($"{context} does not have home folder.");
            }
            return folder;
        }
    }




}
