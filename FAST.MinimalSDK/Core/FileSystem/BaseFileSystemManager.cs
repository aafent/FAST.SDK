using FAST.Core.Models;
using FAST.Files;
using System.Text;

namespace FAST.Core.FileSystem
{
    public abstract class BaseFileSystemManager : IfileSystemManager
    {
        public Encoding defaultEncoding = Encoding.UTF8;
        protected abstract string rootPath { get; }

        #region (+) abstract methods
        public abstract IEnumerable<IfileSystemEntityModel> getFilesList(string homePath, string pathToBrowse, string searchPattern);
        public abstract IEnumerable<IfileSystemEntityModel> getFilesTree(string homePath, string pathToBrowse, string searchPattern);

        public abstract fsCommandStatus createFolder(string destinationFolder, string newFolderName);
        public abstract fsCommandStatus deleteFolder(string destinationFolder, string folderName);
        public abstract fsCommandStatus deleteFile(string destinationFolder, string fileName);
        public abstract fsCommandStatus renameFile(string pathToFile, string newName);
        public abstract fsCommandStatus renameFolder(string pathToFolder, string newName);

        public abstract fsCommandStatus writeToTextFile(string destinationFolder, string fileName, string fileContent, bool overrideIfExists);
        public abstract fsCommandStatus readTextFromFile(string destinationFolder, string fileName, out string fileContent);

        public abstract bool folderExists(string pathToFolder);
        public abstract bool fileExists(string pathToFile);

        #endregion (+) abstract methods


        #region (+) virtual methods
        public virtual string pathCombine(params string[] paths)
        {
            return Path.Combine(paths);
        }

        public virtual string applicationPathToRealPath(string applicationPath)
        {
            return applicationPath;
        }

        public virtual fsCommandStatus deleteFileOrFolder(string destinationFolder, string pathToNameForDelete) //eg: FileSystem, folder1\folder2\file.txt
        {
            string itemToDelete = pathCombine(destinationFolder, pathToNameForDelete);
            if (fileExists(itemToDelete))
            {
                return deleteFile(destinationFolder, pathToNameForDelete);
            }
            else
            if (folderExists(itemToDelete))
            {
                return deleteFolder(destinationFolder, pathToNameForDelete);
            }
            return fsCommandStatus.notFound;
        }

        #endregion (+) virtual methods


        #region (+) other FS methods
        public fsCommandStatus renameFileOrFolder(string pathToFilerOrFolder, string newName)
        {
            if (fileExists(pathToFilerOrFolder))
            {
                return renameFile(pathToFilerOrFolder, newName);
            }
            else
            if (folderExists(pathToFilerOrFolder))
            {
                return renameFolder(pathToFilerOrFolder, newName);
            }

            return fsCommandStatus.notFound;
        }

        public async Task copyFileToMemoryStreamAsync(string filePath, MemoryStream memoryStream)
        {
            string realFullPath = applicationPathToRealPath(filePath);

            using (var stream = new FileStream(realFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                await stream.CopyToAsync(memoryStream);
            }
        }

        public FileStream getFileStream(string pathToFile, FileMode fileMode = FileMode.Create)
        {
            string realPathToFile = applicationPathToRealPath(pathToFile);
            var stream = new FileStream(realPathToFile, fileMode);
            return stream;
        }

        public long readTextFilePartial(string pathToTextFile, StringBuilder textContent, long pointToFetchFrom, int maxLines, bool useAppendLine = true, bool readBackward = false)
        {
            long newPoint = 0;

            if (pointToFetchFrom < 0) { pointToFetchFrom = 0; }
            string fullPathToTextFile = pathToTextFile = applicationPathToRealPath(pathToTextFile);
            var fileInfo = new FileInfo(fullPathToTextFile);

            using (FileStream fs = new FileStream(fullPathToTextFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                string line = null;
                int cnt = 0;
                StreamReader sr = null;

                if (!readBackward) // fetch next starting from point: pointToFetchFrom
                {
                    newPoint = fs.Seek(pointToFetchFrom, SeekOrigin.Begin);
                    using (sr = new StreamReader(fs))
                    {
                        cnt = 1;
                        while (cnt <= maxLines && sr.Peek() >= 0)
                        {
                            line = sr.ReadLine();
                            newPoint += line.Length + Environment.NewLine.Length;
                            if (useAppendLine)
                            {
                                textContent.AppendLine(line);
                            }
                            else
                            {
                                textContent.Append(line);
                            }
                            cnt++;
                        }
                        sr.Close();
                    }
                }
                else
                {
                    long readTo;
                    if (pointToFetchFrom == 0) pointToFetchFrom = fileInfo.Length;

                    newPoint = fs.Seek(pointToFetchFrom, SeekOrigin.Begin);
                    int linesToRead = 0;
                    newPoint = fileHelpers.backwardSeekToBeginOfLine(fs, maxLines, out linesToRead);
                    readTo = newPoint - 1;
                    if (readTo < 2) readTo = 1;

                    using (sr = new StreamReader(fs))
                    {
                        cnt = 1;
                        while (cnt <= linesToRead && sr.Peek() >= 0)
                        {
                            line = sr.ReadLine();
                            newPoint += line.Length + Environment.NewLine.Length;
                            if (useAppendLine)
                            {
                                textContent.AppendLine(line);
                            }
                            else
                            {
                                textContent.Append(line);
                            }
                            cnt++;
                        }
                        sr.Close();
                    }
                }
                fs.Close();
            }

            return newPoint;
        }
        #endregion (+) other FS methods

        #region (+) other class methods
        public bool isOk(fsCommandStatus statusCode, IerrorCarrier error)
        {
            if (statusCode == fsCommandStatus.success)
            {
                error.hasError = false;
                return true;
            }

            error.hasError = true;
            switch (statusCode)
            {
                case fsCommandStatus.alreadyExists:
                    error.errorText = "Item (File/Folder/Item/Library/Contex) already exists";
                    break;
                case fsCommandStatus.notFound:
                    error.errorText = "Requested item not found";
                    break;
                case fsCommandStatus.destinationDoesNotExists:
                    error.errorText = "Destination does not exists";
                    break;
                case fsCommandStatus.emptyArgument:
                    error.errorText = "One or more arguments are empty or null";
                    break;
                case fsCommandStatus.rootFileAccessProhibited:
                    error.errorText = "File access subsystem with root path as argument it is prohibited.";
                    break;
                default:
                    error.errorText = $"Error of type: {statusCode}.";
                    break;
            }

            return false;
        }


        #endregion (+) other class methods
    }

}
