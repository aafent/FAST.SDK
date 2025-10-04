using System.Text;
using System.Security.Cryptography;
using FAST.Core;

#if NETCOREAPP
#else
using System.Security.AccessControl;
#endif


namespace FAST.Files
{
    public class fileHelpers
    {

#if NETCOREAPP 
        public void setFileSecurity(string fileName, string account, FileSystemRights rights, AccessControlType controlType)
        {
            return;
        }
        public void setFullAccess(string fileName)
        {
            return;
        }
#else
        public void setFileSecurity(string fileName, string account, FileSystemRights rights, AccessControlType controlType)
        {
            FileSecurity fileSecurity = File.GetAccessControl(fileName);
            fileSecurity.AddAccessRule(new FileSystemAccessRule(account, rights, controlType));
            File.SetAccessControl(fileName, fileSecurity);
        }
        public void setFullAccess(string fileName)
        {
            this.setFileSecurity(fileName, "Everyone", System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow);
        }
#endif


        /// <summary>
        /// Copy any Stream to a file
        /// </summary>
        /// <param name="anyStream">Any stream</param>
        /// <param name="filePath">The destination file path</param>
        public static void streamToFile(Stream anyStream, string filePath)
        {
            using (FileStream file = new FileStream(filePath, FileMode.Create, System.IO.FileAccess.Write))
            {
                byte[] bytes = new byte[anyStream.Length];
                anyStream.Read(bytes, 0, (int)anyStream.Length);
                file.Write(bytes, 0, bytes.Length);
                anyStream.Close();
            }
        }


        [Obsolete("use streamToFile() instand of memoryStreamToFile()")]
        public static void memoryStreamToFile(MemoryStream memoryStream, string filePath)
        {
            using (FileStream file = new FileStream(filePath, FileMode.Create, System.IO.FileAccess.Write)) 
            {
                byte[] bytes = new byte[memoryStream.Length];
                memoryStream.Read(bytes, 0, (int)memoryStream.Length);
                file.Write(bytes, 0, bytes.Length);
                memoryStream.Close();
            }
        }


        public static void fileToMemoryStream(string filePath, MemoryStream memoryStream)
        {
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read)) {
               byte[] bytes = new byte[file.Length];
               file.Read(bytes, 0, (int)file.Length);
               memoryStream.Write(bytes, 0, (int)file.Length);
            }
        }

        /// <summary>
        /// Copies the contents of input to output. Doesn't close either stream.
        /// </summary>
        public static void copyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[16*1024];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0) { output.Write(buffer, 0, read); }
        }

        public static void createEmptyFile(string filePath )
        {
            var file = File.Create(filePath);
            file.Flush(true);
            file.Close();
        }

        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the text file's endianness fails.
        /// </summary>
        /// <param name="filename">The text file to analyze.</param>
        /// <returns>The detected encoding.</returns>
        public static Encoding getEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
#if !NET6_0_OR_GREATER
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
#endif
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.ASCII;
        }

        /// <summary>
        /// Backwards seek to a begin of line. From the current point of the file (current seek), moves backward to the begin of the 
        /// line indicating the 'numberOfLine' argument. 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="numberOfLine"></param>
        /// <param name="currentOffset"></param>
        /// <returns>The new file position (seek)</returns>
        public static long backwardSeekToBeginOfLine(FileStream stream, int numberOfLine, out int linesCanRead)
        {
            int backLineNumber = 0;
            long currentOffset = stream.Seek(0, SeekOrigin.Current);

            if (stream.CanRead) while (stream.ReadByte() == -1) currentOffset = stream.Seek(-1, SeekOrigin.Current); // locate the first readable location (in case of EOF)
            currentOffset = stream.Seek(-1, SeekOrigin.Current); // unread the last read byte

            int currentByte=0;
            while (stream.CanRead && currentByte >= 0 && currentOffset > 2) // locate for character 13
            {            
                currentByte = stream.ReadByte();
                if (currentByte == System.Environment.NewLine[0]) 
                {
                    if (stream.ReadByte() == System.Environment.NewLine[1]) // LF   
                    {
                        backLineNumber++;
                        if (backLineNumber == numberOfLine)
                        {
                            linesCanRead = backLineNumber;
                            return currentOffset;
                        }
                    }
                    currentOffset = stream.Seek(-1, SeekOrigin.Current); //goto back one
                }
                currentOffset = stream.Seek(-2, SeekOrigin.Current); // go back 2 characters and be ready for next read. 
            }
            if (backLineNumber > 0) 
            { 
                linesCanRead = backLineNumber;
                return 1; 
            }

            linesCanRead = 0;
            return -1;
        }

        //
        // Summary:
        //     Moves a specified file to a new location, providing the option to specify a new
        //     file name.
        //
        // Parameters:
        //   sourceFileName:
        //     The name of the file to move. Can include a relative or absolute path.
        //
        //   destFileName:
        //     The new path and name for the file.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     The destination file already exists.-or- sourceFileName was not found.
        //
        //   T:System.ArgumentNullException:
        //     sourceFileName or destFileName is null.
        //
        //   T:System.ArgumentException:
        //     sourceFileName or destFileName is a zero-length string, contains only white space,
        //     or contains invalid characters as defined in System.IO.Path.InvalidPathChars.
        //
        //   T:System.UnauthorizedAccessException:
        //     The caller does not have the required permission.
        //
        //   T:System.IO.PathTooLongException:
        //     The specified path, file name, or both exceed the system-defined maximum length.
        //     For example, on Windows-based platforms, paths must be less than 248 characters,
        //     and file names must be less than 260 characters.
        //
        //   T:System.IO.DirectoryNotFoundException:
        //     The path specified in sourceFileName or destFileName is invalid, (for example,
        //     it is on an unmapped drive).
        //
        //   T:System.NotSupportedException:
        //     sourceFileName or destFileName is in an invalid format.
        public static void move(string sourceFileName, string destFileName, bool overrideIfExists=false)
        {
            if (File.Exists(destFileName))
            {
                if (overrideIfExists)
                {
                    File.Delete(destFileName);
                }
            }

            File.Move(sourceFileName, destFileName);
        }

        public static bool tryToRemoveStartingPath(string inputPath, string startingPath, out string endingPath)
        {
            endingPath = null;
            inputPath = Path.GetFullPath(inputPath);
            startingPath = Path.GetFullPath(startingPath);

            if (string.IsNullOrEmpty(inputPath)) return false; // no path to workwith
            if (string.IsNullOrEmpty(startingPath)) // return the inputPath, as the starting does not exists
            {
                endingPath = inputPath;
                return true;
            }

            // (v) make both arrays
            var input = inputPath.Split(Path.DirectorySeparatorChar);
            var start = startingPath.Split(Path.DirectorySeparatorChar);

            if (start.Length > input.Length) return false; // (<) the starting path was more folders than the input

            // (v) check the input array if all parts exists into start
            for (int inx = 0; inx < start.Length; inx++)
            {
                if (!start[inx].Equals(input[inx], StringComparison.InvariantCultureIgnoreCase))
                {
                    return false; // (<) not all parts are equal, so the startingPath isn't the starting path of the input path
                }
            }
            // (v) now, the initial parts of the input path are equal to the starting path
            //          the only action now is to remove the starting path
            if (start.Length == input.Length)
            {
                endingPath = String.Empty;
                return true;
            }

            // (v) build the ending path
            endingPath = string.Join(Path.DirectorySeparatorChar, input, start.Length, (input.Length - start.Length));
            return true;
        }

        public static string trimPathSeperators(string inputPath)
        {
            if (string.IsNullOrEmpty(inputPath)) return inputPath;
            if (inputPath.StartsWith(Path.DirectorySeparatorChar)) inputPath = inputPath.Substring(1);
            if (inputPath.EndsWith(Path.DirectorySeparatorChar)) inputPath = inputPath.Substring(0, inputPath.Length - 1);

            return inputPath;
        }


        public static string md5(string filePath, bool standardFormat = true)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    if (standardFormat)
                        return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                    else
                        return BitConverter.ToString(md5.ComputeHash(stream));

                }
            }
        }

        public static string md5(Stream inputStream, bool standardFormat = true)
        {
            using (var md5 = MD5.Create())
            {
                if (standardFormat)
                    return BitConverter.ToString(md5.ComputeHash(inputStream)).Replace("-", "").ToLower();
                else
                    return BitConverter.ToString(md5.ComputeHash(inputStream));
            }
        }


        [toDo("To implement a threadSafe mechanism")]
        private class thredSafeWrite
        {
            private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();

            public void WriteToFileThreadSafe(string text, string path)
            {
                // Set Status to Locked
                _readWriteLock.EnterWriteLock();
                try
                {
                    // Append text to the file
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine(text);
                        sw.Close();
                    }
                }
                finally
                {
                    // Release lock
                    _readWriteLock.ExitWriteLock();
                }
            }
        }
    }




#if NETCOREAPP 
    [Flags]
    public enum FileSystemRights
    {
        ReadData = 1,
        ListDirectory = 1,
        WriteData = 2,
        CreateFiles = 2,
        AppendData = 4,
        CreateDirectories = 4,
        ReadExtendedAttributes = 8,
        WriteExtendedAttributes = 16,
        ExecuteFile = 32,
        Traverse = 32,
        DeleteSubdirectoriesAndFiles = 64,
        ReadAttributes = 128,
        WriteAttributes = 256,
        Write = 278,
        Delete = 65536,
        ReadPermissions = 131072,
        Read = 131209,
        ReadAndExecute = 131241,
        Modify = 197055,
        ChangePermissions = 262144,
        TakeOwnership = 524288,
        Synchronize = 1048576,
        FullControl = 2032127
    }
    public enum AccessControlType
    {
        Allow = 0,
        Deny = 1
    }
#endif




}
