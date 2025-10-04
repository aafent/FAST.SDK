using FAST.Core.Models;

namespace FAST.Core.FileSystem
{
    public interface IfileManagmentAdapter : IerrorCarrier, ImultiErrorCarrier
    {
        IEnumerable<IfileSystemEntityModel> browseFolder(string folderPath, string browsePattern);
        void createFolder(string folderPath, string newFolderName);
        void renameFileOrFolder(string itemPathToRename, string newName);
        void deleteFileOrFolder(string itemPathToDelete);

        Task copyFileToMemoryStreamAsync(string filePathToDownload, MemoryStream memoryStream);
        FileStream getFileStream(string destinationFolderPath, string untrustedFileName);
    }
}