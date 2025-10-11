using FAST.Core;

namespace FAST.DB
{
    public class dataCacheElementSQLCommands : IdataCacheElement 
    {
        public string sqlSelectAllCommand = "";
        public string sqlUpdateCommand = "";
        public string sqlInsertCommand = "";
        public string sqlSelectAllByKeyCommand = "";
        public string sqlSelectAllByGroupKeyCommand = "";
        public string sqlExistsByKeyCommand = "";
        public string sqlDeleteByKeyCommand = "";
        public string sqlDeleteByGroupKeyCommand = "";

        public string dbObjectName = string.Empty;
        public bool dbObjectNameIsOverwritten = false;
        public updateMethods updateMethod = updateMethods.useUpdate;

        public Dictionary<string, bool> needQuoteDict = new Dictionary<string, bool>();


        public void copyTo(IdataCacheElement destination )
        {
            dataCacheElementSQLCommands dst = (dataCacheElementSQLCommands)destination;
            dst.sqlSelectAllCommand = sqlSelectAllCommand;
            dst.sqlUpdateCommand = sqlUpdateCommand;
            dst.sqlInsertCommand = sqlInsertCommand;
            dst.sqlSelectAllByKeyCommand = sqlSelectAllByKeyCommand;
            dst.sqlSelectAllByGroupKeyCommand = sqlSelectAllByGroupKeyCommand;
            dst.sqlExistsByKeyCommand = sqlExistsByKeyCommand;
            dst.sqlDeleteByKeyCommand = sqlDeleteByKeyCommand;
            dst.sqlDeleteByGroupKeyCommand = sqlDeleteByGroupKeyCommand;

            dst.dbObjectName = dbObjectName;
            dst.dbObjectNameIsOverwritten = dbObjectNameIsOverwritten;
            dst.updateMethod = updateMethod;

            dst.needQuoteDict = needQuoteDict.ToDictionary(k => k.Key, v => v.Value);
        }
    }
}
