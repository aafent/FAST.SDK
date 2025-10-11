using FAST.Core.Models;

namespace FAST.Core.FileSystem
{
    /// <summary>
    /// Interface for file management operations such as browsing, creating, renaming, deleting, and copying files and folders.
    /// </summary>
    public interface IfileManagmentAdapter : IerrorCarrier, ImultiErrorCarrier
    {
        /// <summary>
        /// Browse a folder and return its contents based on the specified pattern.
        /// </summary>
        /// <param name="folderPath">Path to the folder</param>
        /// <param name="browsePattern">Files pattern to browse</param>
        /// <returns></returns>
        IEnumerable<IfileSystemEntityModel> browseFolder(string folderPath, string browsePattern);

        /// <summary>
        /// Create a new folder in the specified path.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="newFolderName"></param>
        void createFolder(string folderPath, string newFolderName);

        /// <summary>
        /// Rename a file or folder to a new name.
        /// </summary>
        /// <param name="itemPathToRename"></param>
        /// <param name="newName"></param>
        void renameFileOrFolder(string itemPathToRename, string newName);

        /// <summary>
        /// Delete a file or folder at the specified path.
        /// </summary>
        /// <param name="itemPathToDelete"></param>
        void deleteFileOrFolder(string itemPathToDelete);

        /// <summary>
        /// Copy a file from the source path to the destination path.
        /// </summary>
        /// <param name="filePathToDownload"></param>
        /// <param name="memoryStream"></param>
        /// <returns></returns>
        Task copyFileToMemoryStreamAsync(string filePathToDownload, MemoryStream memoryStream);

        /// <summary>
        /// Get a file stream for the specified destination folder and untrusted file name.
        /// </summary>
        /// <param name="destinationFolderPath"></param>
        /// <param name="untrustedFileName"></param>
        /// <returns></returns>
        FileStream getFileStream(string destinationFolderPath, string untrustedFileName);
    }
}