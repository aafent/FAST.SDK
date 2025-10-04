namespace FAST.Logging
{
    /// <summary>
    /// Logger adapter, to log at a file.
    /// </summary>
    public class loggerToFile : IloggerAdapter
    {
        /// <summary>
        /// Full path to the logfile. 
        /// If the logfile contains the character dash '-' at the file name part and the keepDaillyFiles is true,
        /// then the character - will be replaces by the current date of the format yyyyMMdd.
        /// </summary>
        public string logFilePath
        {
            get;
            private set;
        }

        /// <summary>
        /// Enable to keep dailly files. See logFilePath property for more information.
        /// </summary>
        public bool keepDaillyFiles
        {
            get;
            set;
        } = false;

        #region (+) Interface implemetation 

        public void bind(Ilogger logger)
        {
        }

        public void init()
        {
            string fname = Path.GetFileNameWithoutExtension(logFilePath);
            if (keepDaillyFiles && fname.IndexOf('-') >=0  )
            {
                var fnameWithDay = fname.Replace("-", DateTime.Now.ToString("yyyyMMdd") );
                logFilePath = logFilePath.Replace(fname,fnameWithDay);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath) );
        }

        public void write(levels level, string source, string message, string extra, Exception curringException, string formatedMessage)
        {
            using ( TextWriter textWriter = new StreamWriter(logFilePath, true) )
            {
                textWriter.WriteLine(formatedMessage);
                textWriter.Flush();
                textWriter.Close();
            }
        }

        #endregion (+) Interface implemetation 
    }
}