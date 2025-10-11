using FAST.Core.Models;
using System.Text;

namespace FAST.Core.FileSystem
{

    /// <summary>
    /// Interface for managing file system operations.
    /// </summary>
    public interface IfileSystemManager
    {
        string applicationPathToRealPath(string applicationPath);
        Task copyFileToMemoryStreamAsync(string filePath, MemoryStream memoryStream);
        fsCommandStatus createFolder(string destinationFolder, string newFolderName);
        fsCommandStatus deleteFile(string destinationFolder, string fileName);
        fsCommandStatus deleteFileOrFolder(string destinationFolder, string pathToNameForDelete);
        fsCommandStatus deleteFolder(string destinationFolder, string folderName);
        bool fileExists(string pathToFile);
        bool folderExists(string pathToFolder);
        IEnumerable<IfileSystemEntityModel> getFilesList(string homePath, string pathToBrowse, string searchPattern);
        IEnumerable<IfileSystemEntityModel> getFilesTree(string homePath, string pathToBrowse, string searchPattern);

        FileStream getFileStream(string pathToFile, FileMode fileMode = FileMode.Create);
        bool isOk(fsCommandStatus statusCode, IerrorCarrier error);
        string pathCombine(params string[] paths);
        long readTextFilePartial(string pathToTextFile, StringBuilder textContent, long pointToFetchFrom, int maxLines, bool useAppendLine = true, bool readBackward = false);
        fsCommandStatus readTextFromFile(string destinationFolder, string fileName, out string fileContent);
        fsCommandStatus renameFile(string pathToFile, string newName);
        fsCommandStatus renameFileOrFolder(string pathToFilerOrFolder, string newName);
        fsCommandStatus renameFolder(string pathToFolder, string newName);
        fsCommandStatus writeToTextFile(string destinationFolder, string fileName, string fileContent, bool overrideIfExists);
    }
}