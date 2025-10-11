using System.Data.Common;

namespace FAST.DB
{
    public class csBuilder
    {
        private string _connectionString;
        private DbConnectionStringBuilder _builder;

        public bool fixQuoteCurly = true;
        public bool useODBCRules = false;

        public string connectionString
        {
            set
            {
                _connectionString = value;
                _builder = new DbConnectionStringBuilder(useODBCRules);
                _builder.ConnectionString = _connectionString;
            }

            get
            {
                if (_builder == null) { return null; }
                string cs = _builder.ConnectionString;
                if (fixQuoteCurly) { cs = cs.Replace("\"{", "{").Replace("}\"", "}"); }
                return cs;
            }
        }
        public DbConnectionStringBuilder builder
        {
            get
            {
                return _builder;
            }
        }

        public object this[string variable]   
        {
            set
            {
                _builder[variable] = value;
            }
            get
            {
                return _builder[variable];
            }
            
        }

        public csBuilder()
        {
        }
        public csBuilder(string connectionString)
        {
            this.connectionString = connectionString;
        }

    }
}
