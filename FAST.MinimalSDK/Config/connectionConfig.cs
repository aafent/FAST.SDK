using FAST.Core;
using FAST.DB;
using FAST.Strings;
using System.Data.Common;
using System.Xml.Serialization;

namespace FAST.Config
{

    /// <summary>
    /// Connection Configuration Managing class, able to handle multiple connection strings for different environments
    /// </summary>
    [XmlRoot("connectionConfig")]
    public class connectionConfig : stringReplacer, IdefaultConfiguration, Iexample
    {

        /// <summary>
        /// List of connection items
        /// </summary>
        [XmlArray("sources"), XmlArrayItem("connection")]
        public List<connectionConfigItem> sources { get; set; } = new List<connectionConfigItem>();

        /// <summary>
        /// Error text of the last operation
        /// </summary>
        [XmlIgnore]
        public string errorText
        {
            private set;
            get;
        }

        /// <summary>
        /// The last found connection item after a tryToGetConnectionString call
        /// </summary>
        [XmlIgnore]
        public connectionConfigItem found
        {
            private set;
            get;
        }

        /// <summary>
        /// The default configuration file name
        /// </summary>
        [XmlIgnore]
        public string defaultConfigFileName => "connections";

        /// <summary>
        /// Try to get a connection string for a given environment and name
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="name"></param>
        /// <param name="cs"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public bool tryToGetConnectionString(string environment, string name, out string cs, out adoProviderTypes provider)
        {
            found = null;
            errorText = "General error";
            cs = string.Empty;
            provider = adoProviderTypes.odbc;

            if (string.IsNullOrEmpty(environment))
            {
                if (this.isVariable("environment")) environment = this.getString("environment");
                if (string.IsNullOrEmpty(environment))
                {
                    errorText=string.Format("To retrieve the connection string, you need to specify both the environment (is missing) and the name (it is:{0})", name);
                    return false;
                }
            }


            if (!sources.Any(i => i.environment == environment && i.name == name))
            {
                errorText = string.Format("Connection:{1} for Environment:{0} not found", environment, name);
                return false;
            }
            found = sources.First(i => i.environment == environment && i.name == name);

            this.set("name", name);
            this.set("environment", name);
            this.set("comments", found.comments);
            this.set("tag", found.tag);
            this.set("role", found.role);
            this.setTemplate(found.cs);

            cs = this.replace();
            provider = found.provider;

            return true;
        }

        /// <summary>
        /// Example method to create a sample connectionConfig XML file
        /// </summary>
        /// <param name="filePath"></param>
        public void example(string filePath)
        {
            connectionConfig con = new connectionConfig();
            con.set("host1", "prod.example.com");
            con.set("host2", "db.example.com");
            con.sources.Add(new connectionConfigItem() { name="con1", environment="PROD", cs="Data Source=myASEserver;Port=5000;Database=myDataBase;Uid=myUsername;Pwd=myPassword;", comments="1st app. connection" });
            con.sources.Add(new connectionConfigItem() { name="con1", environment="UAT", cs="Data Source=XYZ;host={host1},", comments="1st app. connection" });
            con.sources.Add(new connectionConfigItem() { name="con1", environment="DEV", cs="<ado_connection_string>", comments="1st app. connection" });
            con.sources.Add(new connectionConfigItem() { name="con2", environment="PROD", cs="<ado_connection_string>", comments="2nd app. connection" });
            con.sources.Add(new connectionConfigItem() { name="con2", environment="UAT", cs="<ado_connection_string>", comments="2nd app. connection" });
            con.sources.Add(new connectionConfigItem() { name="con2", environment="DEV", cs="<ado_connection_string>", comments="2nd app. connection" });

            new baseSerializer().serialize(con, typeof(connectionConfig),filePath, "connectionConfig:example");
        }

        /// <summary>
        /// Get a DbProviderFactory for a given environment and name
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public DbProviderFactory getDbProviderFactory(string environment, string name)
        {
            adoProviderTypes adoProvider;
            string cs;
            if ( tryToGetConnectionString(environment, name, out cs, out adoProvider) )
            {
                if ( ! string.IsNullOrEmpty(found.providerOverride) )
                {
                    DbProviderFactory factory = DbProviderFactories.GetFactory(found.providerOverride);
                    return factory;
                }
                return getDbProviderFactoryByAdoType(adoProvider);
            }
            throw new NotSupportedException($"Cannot find source entry for name: {name} of the environment: {environment}.");
        }

        /// <summary>
        /// Get a DbProviderFactory for the last found connection item
        /// </summary>
        /// <returns></returns>
        public DbProviderFactory getDbProviderFactory()
        {
            return getDbProviderFactory(found.environment,found.name);
        }


        /// <summary>
        /// Get a DbProviderFactory for a given ADO provider type
        /// </summary>
        /// <param name="adoProvider"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static DbProviderFactory getDbProviderFactoryByAdoType(adoProviderTypes adoProvider)
        {
            //
            //https://docs.telerik.com/reporting/knowledge-base/how-to-register-db-provider-factory-in-net-core-project
            //https://csharp.hotexamples.com/examples/System.Data.Odbc/OdbcFactory/-/php-odbcfactory-class-examples.html
            //
            string provider = adoProvider switch
            {
                adoProviderTypes.sql    => "System.Data.SqlClient",     // System.Data.SqlClient.SqlClientFactory
                adoProviderTypes.oledb  => "System.Data.OleDb",
                adoProviderTypes.sqLite => "System.Data.SQLite",        // System.Data.SQLite.SQLiteFactory
                adoProviderTypes.oracle => "System.Data.OracleClient",  // Oracle.ManagedDataAccess.Client.OracleClientFactory
                adoProviderTypes.mySQL  => "MySql.Data.MySqlClient",    // MySql.Data.MySqlClient.MySqlClientFactory
                adoProviderTypes.postgreSQL=> "Npgsql",                 // Npgsql.NpgsqlFactory
                //Teradata.Client.Provider
                //FirebirdSql.Data.FirebirdClient
                //NuoDb.Data.Client
                //Sap.Data.Hana
                _ => throw new NotSupportedException($"ADO Provider: {adoProvider} is not supported yet.")
            }; 
            DbProviderFactory factory = DbProviderFactories.GetFactory(provider);
            return factory;
        }
    }
}
