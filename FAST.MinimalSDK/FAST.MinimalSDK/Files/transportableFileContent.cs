namespace FAST.Files
{
    //[DataContract]
    public class transportableFileContent
    {
        //[DataMember]
        public string tag_string = "";

       // [DataMember]
        public byte[] content = new byte[0];

        private string _filePath = "";
      
        //[DataMember]
        public string filePath
        {
            get { return _filePath; }
        }

        //[DataMember]
        public bool encryptionOnPersistent {  get;set; } = false;

        public transportableFileContent()
        {
            return;
        }
        public transportableFileContent(string filePath)
        {
            setFilePath(Path.GetDirectoryName(filePath), Path.GetFileName(filePath) );
            return;
        }

        public virtual void checkInFile( ) // Release File that is checked out by same user.
        {
            throw new NotImplementedException();
        }
        public virtual void checkOutFile() // Check out the file if available
        {
            throw new NotImplementedException();
        }
        public virtual bool isCheckedOut() // Check availability of the file
        {
            throw new NotImplementedException();
        }
        public virtual bool isCheckedOutByOtherUser(out string lockerUserName)
        {
            throw new NotImplementedException();
        }

        public string getTimeSequenceFileName(string inFolderPath, string fileNameSuffix="", bool createDateFolder=true, bool allocateFileName=true)
        {
            DateTime currentTime = DateTime.UtcNow;
            string dateFolderName = string.Format("{0}{1}{2}", currentTime.Year, currentTime.Month.ToString().PadLeft(2, '0'), currentTime.Day.ToString().PadLeft(2, '0'));
            string timeFileName = string.Format("{0}-{1}{2}{3}-{4}", dateFolderName, currentTime.Hour.ToString().PadLeft(2, '0'),
                                                                                     currentTime.Minute.ToString().PadLeft(2, '0'),
                                                                                     currentTime.Second.ToString().PadLeft(2, '0'),
                                                                                     currentTime.Millisecond.ToString().PadLeft(3, '0'));
            var dateFolder = new DirectoryInfo(inFolderPath);
            if (createDateFolder)
            {
                if (!dateFolder.Exists) { dateFolder.Create(); }
                dateFolder.CreateSubdirectory(dateFolderName); 
            }

            if (string.IsNullOrEmpty(fileNameSuffix)) { fileNameSuffix=""; }

            string fileName = "";
            int sequence = 0;
            while (true)
            {
                fileName = string.Format("{0}-{1}{2}", timeFileName, sequence.ToString().PadLeft(3, '0'), fileNameSuffix);
                if (allocateFileName)
                {
                    try
                    {
                        using (FileStream file = new FileStream(Path.Combine(inFolderPath, Path.Combine(dateFolderName, fileName)), FileMode.CreateNew)) { }
                    }
                    catch (IOException)
                    {
                        sequence++;
                        continue; // reloop
                    }
                }
                break; // jump out of the next file sequence loopS
            }

            return Path.Combine(inFolderPath, Path.Combine(dateFolderName, fileName));
        }
        public void setFilePath(string fileDirectoryPath, string fileName )
        {
            setInternalFilePath(fileDirectoryPath, fileName);

            return;
        }

        protected virtual void setInternalFilePath(string fileDirectoryPath, string fileName)
        {
            _filePath = Path.Combine(fileDirectoryPath, fileName);
            return;
        }

        // (v) file to/from content methods

        public void loadContent( string filePath, bool checkOutifPossible )
        {
            if (checkOutifPossible) { checkOutFile(); }

            using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                content = new byte[fs.Length];
                fs.Read(content, 0, (int)fs.Length );
                fs.Close();
            }

            if (encryptionOnPersistent) 
            {
                var cryptography = new aesCryptography();
                content=cryptography.decrypt(content);
            }

            return;
        }

        public void saveContentTo(string fileName )
        {
            int contentSize = new int();
            //contentSize = content.GetUpperBound(0) + 1 ; // +1 because array starts from 0
            contentSize = content.Length;
            if (contentSize < 1)
            {
                throw new Exception(string.Format("Request to write a zero size file {0} is not permited",Path.GetFileName(fileName) ));
            }

            aesCryptography cryptography=null;
            if (encryptionOnPersistent) cryptography = new aesCryptography();

            string tempFileName = fileName + "_$temporary$"; // used to wirte the internal content
            string oldFileName = fileName + "_$old$"; // used to keep the prev. version
            new FileInfo(tempFileName).Delete();
            using (FileStream fs = new FileStream(tempFileName, FileMode.Create, FileAccess.Write)) 
            {
                fs.Write(encryptionOnPersistent ? cryptography.encrypt(content):content, 0, contentSize);
                fs.Close();
            }
            try
            {
                new FileInfo(oldFileName).Delete();
                if (new FileInfo(fileName).Exists)
                {
                    new FileInfo(fileName).MoveTo(oldFileName);
                }
                new FileInfo(tempFileName).MoveTo(fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error versioning file: {0}\n{1}", fileName,ex.Message), ex);
            }

            return;
        }
        public string saveToTemp()
        {
            string fileName = Path.GetTempFileName();
            saveContentTo(fileName);
            return fileName;
        }
    }
}






