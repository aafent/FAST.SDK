
namespace FAST.DB
{
    /// <summary>
    /// Data Retrieval Policy used by the SQL Classes
    /// </summary>
    public class dataRetrievalPolicy
    {
        private int _maxRows = 1;

        /// <summary>
        /// Max rows to retrieve 
        /// </summary>
        public int maxRows
        {
            get
            {
                return maxRows;
            }
            set
            {
                if (value < 1)
                {
                    _maxRows = 0;
                }
                else
                {
                    _maxRows = value;
                }
            }
        }

        /// <summary>
        /// Check if just one (single) row is declared 
        /// </summary>
        public bool singleRow
        {
            get
            {
                return (_maxRows == 1);
            }
            set
            {
                if (value)
                {
                    _maxRows = 1;
                }
                else
                {
                    _maxRows = 0;
                }
            }
        }



        [Obsolete("Replace it with:singleRowRetrieval")]
        public static dataRetrievalPolicy singleRowRetrival => singleRowRetrieval;

        public static dataRetrievalPolicy singleRowRetrieval
        {
            get
            {
                return new dataRetrievalPolicy() { _maxRows = 1 };
            }
        }

        [Obsolete("Replace it with:singleRowRetrieval")]
        public static dataRetrievalPolicy multiRowsRetrival => multiRowsRetrieval;


        /// <summary>
        /// Returns the standard policy, as multi row data retrieval 
        /// </summary>
        public static dataRetrievalPolicy multiRowsRetrieval
        {
            get
            {
                return new dataRetrievalPolicy() { maxRows = 0 };
            }
        }
    }

}
