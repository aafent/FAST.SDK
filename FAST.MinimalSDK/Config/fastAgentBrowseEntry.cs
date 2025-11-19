using FAST.Core;

namespace FAST.Services.Models.Config
{
    /// <summary>
    /// Browse results of a fast Agent
    /// </summary>
    public class fastAgentBrowseEntry
    {

        /// <summary>
        /// Empty constructor
        /// </summary>
        public fastAgentBrowseEntry()
        {
        }


        /// <summary>
        /// Construct an entry based on one-line result
        /// </summary>
        /// <param name="entry"></param>
        public fastAgentBrowseEntry(string entry):this()
        {
            string[] parts = entry.Split(new[] { ' ' }, 2);
            rawType = parts[0].ToUpper();
            value = parts[1].Trim();
        }

        /// <summary>
        /// The raw value type
        /// </summary>
        protected string rawType {get; set; }

        /// <summary>
        /// The value of the entry
        /// </summary>
        public string value {get; set; }

        /// <summary>
        /// Check if the entry is the real full path of the requested for browse folder
        /// </summary>
        [toDo("to Rename to isBrowsingFolderRealFullPath")]
        public bool isBrowsingFolerRealFullPath 
        {
            get
            {
                return rawType=="C";
            }
        }

        /// <summary>
        /// Check if the entry is a subfolder
        /// </summary>
        public bool isSubfolder
        {
            get
            {
                return rawType == "D";
            }
        }

        /// <summary>
        /// Check if the entry is a any other file (not special eg. log files etc)
        /// </summary>
        public bool isOtherFile
        {
            get
            {
                return rawType == "F";
            }
        }

        /// <summary>
        /// Check if the entry is a any other file (not special eg. log files etc)
        /// </summary>
        public bool isLogFile
        {
            get
            {
                return rawType[0] == 'L';
            }
        }


        /// <summary>
        /// Check if the entry is a file
        /// </summary>
        public bool isFile
        {
            get
            {
                return (isOtherFile | isOtherFile);
            }
        }

    }
}
