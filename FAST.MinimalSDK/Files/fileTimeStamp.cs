namespace FAST.Files
{
    public class fileTimeStamp
    {
        private bool useSpecificCurrentTime = false;
        private DateTime _specificCurrentTime;

        public void setOnceSpecificCurrentDateTime(DateTime specificCurretDateTime)
        {
            useSpecificCurrentTime = true;
            _specificCurrentTime = specificCurretDateTime;
        }
        public void timestamps(out string dateStamp, out string datetimeStamp)
        {
            DateTime currentTime;
            if (useSpecificCurrentTime)
            {
                currentTime = _specificCurrentTime;
                useSpecificCurrentTime = false;
            }
            else
            {
                currentTime = DateTime.UtcNow;
            }
            dateStamp = string.Format("{0}{1}{2}", currentTime.Year, currentTime.Month.ToString().PadLeft(2, '0'), currentTime.Day.ToString().PadLeft(2, '0'));
            datetimeStamp = string.Format("{0}-{1}{2}{3}-{4}", dateStamp, currentTime.Hour.ToString().PadLeft(2, '0'),
                                                                            currentTime.Minute.ToString().PadLeft(2, '0'),
                                                                            currentTime.Second.ToString().PadLeft(2, '0'),
                                                                            currentTime.Millisecond.ToString().PadLeft(3, '0'));
            return;
        }
        public string getTimeSequenceFileName(string inFolderPath, string fileNameSuffix = "", bool createDateFolder = true, bool allocateFileName = true, bool addDateFolderToThePath = true)
        {
            string dateFolderName;
            string timeFileName;
            timestamps(out dateFolderName, out timeFileName);


            var dateFolder = new DirectoryInfo(inFolderPath);
            if (createDateFolder)
            {
                if (!dateFolder.Exists) { dateFolder.Create(); }
                dateFolder.CreateSubdirectory(dateFolderName);
            }

            if (string.IsNullOrEmpty(fileNameSuffix)) { fileNameSuffix = ""; }

            string fileName = "";
            int sequence = 0;
            while (true)
            {
                fileName = string.Format("{0}-{1}{2}", timeFileName, sequence.ToString().PadLeft(3, '0'), fileNameSuffix);
                if (allocateFileName)
                {
                    try
                    {
                        if (addDateFolderToThePath)
                        {
                            using (FileStream file = new FileStream(Path.Combine(inFolderPath, Path.Combine(dateFolderName, fileName)), FileMode.CreateNew)) { }
                        }
                        else
                        {
                            using (FileStream file = new FileStream(Path.Combine(inFolderPath, fileName), FileMode.CreateNew)) { }
                        }
                    }
                    catch (IOException)
                    {
                        sequence++;
                        continue; // reloop
                    }
                }
                break; // jump out of the next file sequence loopS
            }

            if (addDateFolderToThePath)
            {
                return Path.Combine(inFolderPath, Path.Combine(dateFolderName, fileName));
            }
            else
            {
                return Path.Combine(inFolderPath, fileName);
            }
        }

        public static string getInMemoryTempFileName()
        {
            string temporaryFile = System.IO.Path.GetTempFileName();
            File.SetAttributes(temporaryFile, File.GetAttributes(temporaryFile) | FileAttributes.Temporary); // (<) Set the temporary attribute, meaning the file will live in memory and will not be written to disk 
            return temporaryFile;
        }
        public static string getStampAsID()
        {
            string stamp = string.Empty;
            string dateStamp = string.Empty;
            fileTimeStamp fileStamp = new fileTimeStamp();
            fileStamp.timestamps(out dateStamp, out stamp);
            return stamp;
        }
        public static string getStampAsID(string preallocateInFolderPath,string fileNameSuffix, bool createDateFolder, bool addDateFolderToThePath)
        {
            string stamp = string.Empty;
            string dateStamp = string.Empty;
            fileTimeStamp fileStamp = new fileTimeStamp();
            return fileStamp.getTimeSequenceFileName(preallocateInFolderPath,fileNameSuffix, createDateFolder, addDateFolderToThePath);
        }


    }




}
