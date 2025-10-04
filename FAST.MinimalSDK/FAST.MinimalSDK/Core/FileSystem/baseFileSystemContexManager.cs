
namespace FAST.Core.FileSystem
{
    public abstract class baseFileSystemContexManager
    {
        protected abstract IfileSystemManager inner_FileSystemManager { get; }

        public IfileSystemManager FS
        {
            get
            {
                if (inner_FileSystemManager == null) throw new Exception("File System Manager is missing");
                return inner_FileSystemManager;
            }
        }

        public abstract IEnumerable<IfileSystemEntityModel> contextForLibraries(bool getAllLibraries = false);
        public abstract string contexSearchPattern(string contex, string setOfFiles, string pathToBrowse);
        protected abstract string? inner_contexToHomeFolderOrNull(string contex);

        public bool isValidContex(string contex)
        {
            return !string.IsNullOrEmpty(inner_contexToHomeFolderOrNull(contex) );

        }
        public string contexHomeFolder(string contex)
        {
            var folder= inner_contexToHomeFolderOrNull(contex);
            if ( string.IsNullOrEmpty(folder) )
            {
                throw new Exception($"{contex} does not have home folder.");
            }
            return folder;
        }
    }


  
}
