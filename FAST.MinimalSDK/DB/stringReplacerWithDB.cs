using FAST.Core;
using FAST.Strings;
using System.Collections.Specialized;

namespace FAST.DB
{
    /// <summary>
    /// This is the stringReplacer (manager) class
    /// </summary>
    public class stringReplacerWithDB : stringReplacer 
    {
        protected IdbDialectDetails dialect = new irrelevantDialectDetails();

        protected override string openQuote { get => dialect.openQuote; }
        protected override string closeQuote { get => dialect.closeQuote; }
        protected override string dateTimeFormat { get => dialect.dateTimeFormat; }

        #region (+) Constructors

        public stringReplacerWithDB() : base()
        {
        }

        // (v) 22 nov 2019
        public stringReplacerWithDB(variablesContainer copyFrom) : base(copyFrom)
        {
        }

        [Obsolete("This constructor will be removed")]
        public stringReplacerWithDB(NameValueCollection nvc)
        {
            reset();
            foreach (var key in nvc.AllKeys)
            {
                if (key == "__VIEWSTATEGENERATOR" || key == "__VIEWSTATE") { continue; } // by pass ASP.NET's form special fields
                this.set(key, nvc[key]);
            }
        }




        /// <summary>
        /// Construct by copying variables from a container 
        /// </summary>
        /// <param name="vars"></param>
        public stringReplacerWithDB(InamedVariablesContainer vars) : base(vars)
        {

        }

        /// <summary>
        /// Construct by using a key value pair of strings as name and value 
        /// </summary>
        /// <param name="variables">Enumerable key value pair of strings</param>
        public stringReplacerWithDB(IEnumerable<FastKeyValuePair<string, string>> variables) : base(variables)
        {
        }

        #endregion (+) Constructors

        /// <summary>
        /// Set the dialect details to be used by this buffer
        /// </summary>
        /// <param name="dialect"></param>
        public void setDialect(IdbDialectDetails dialect)
        {
            this.dialect = dialect;
        }

        #region (+) set methods

        /// <summary>
        /// Set multiple values from a dbHelper object
        /// </summary>
        /// <param name="dbHelper"></param>
        public virtual void set(ISQLHelperElementary dbHelper)
        {
            set(dbHelper, listTypes.exclude, new List<string>());
        }

        // DATATYPES
        /// <summary>
        /// Set multiple values from a dbHelper object
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="listType"></param>
        /// <param name="columnNames"></param>
        /// <exception cref="Exception"></exception>
        public virtual void set(ISQLHelperElementary dbHelper, listTypes listType, List<string> columnNames)
        {
            foreach (var columnName in dbHelper.col.Keys)
            {
                object nullObject = new object();
                var value = dbHelper.getAny(columnName, nullObject);
                if (value == nullObject)
                {
                    set(columnName, string.Empty);
                }
                else
                {
                    // DATATYPES POINT
                    try
                    {
                        var @switch = new Dictionary<Type, Action>
                    {
                        { typeof(string), () => set(columnName, (string)value ) },
                        { typeof(DateTime), () => set(columnName, ((DateTime)value) ) }, //.ToUniversalTime()
                        { typeof(bool), () => set(columnName, (bool)value ) },
                        { typeof(decimal), () => set(columnName, (decimal)value ) },
                        //{ typeof(int), () => set(columnName, (int)value ) }, 
                        { typeof(Int16), () => set(columnName, (Int16)value ) },
                        { typeof(System.Int32), () => set(columnName, (Int32)value ) },
                        { typeof(Int64), () => set(columnName, (Int64)value ) },
                        { typeof(Byte), () => set(columnName, (Byte)value ) }
                    };
                        @switch[dbHelper.getColumnType(columnName)]();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("stringReplacerWithDB::set() cannot find type: " + dbHelper.getColumnType(columnName).ToString(), ex);
                    }
                }
            }
        }

        #endregion (+) set methods


        /// <summary>
        /// Copy the inner variables to the input buffer
        /// </summary>
        /// <param name="otherVariables">The input variables buffer</param>
        public void copyVariablesTo(variablesContainer otherVariables)
        {
            foreach (var variable in variables)
            {
                otherVariables.set(variable.left, variable.right);
            }
        }
    }

}
