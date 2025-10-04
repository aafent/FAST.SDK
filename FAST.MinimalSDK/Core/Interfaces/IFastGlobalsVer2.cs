using FAST.Core.Interfaces;

namespace FAST.Core
{
    /// <summary>
    /// Interface to the Version2 of the FASTGlobals class implementation
    /// </summary>
    public interface IFastGlobalsVer2 : IFastGlobals, IFastGlobalsHomeFolder
    {
        /// <summary>
        /// A Mapping between the Location and a Folder Relative Path
        /// </summary>
        /// <param name="location">The Location Enumeration</param>
        /// <returns>String, The Relative Real Path of a Folder</returns>
        string folderRelativePath(Enum location);
    }

}
