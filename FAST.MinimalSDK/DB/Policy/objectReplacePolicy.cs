namespace FAST.DB
{

    /// <summary>
    /// Object replace policy used by SQL Channel and SQL Commands
    /// </summary>
    public class objectReplacePolicy
    {
        private int _maxDepth = 5;

        /// <summary>
        /// Max Depth of replacement 
        /// </summary>
        public int maxDepth
        {
            get
            {
                return _maxDepth;
            }
            set
            {
                _maxDepth = value;
                if (_maxDepth < 1) { _maxDepth = 0; }
            }
        }

        /// <summary>
        /// Check if a max depth is declared 
        /// </summary>
        public bool hasMaxDepth
        {
            get
            {
                return !(_maxDepth < 1);
            }
        }

        /// <summary>
        /// Represents and return the FAST Standard object replace policy use by default
        /// </summary>
        public static objectReplacePolicy standard
        {
            get
            {
                return new objectReplacePolicy();
            }
        }
    }

}
