namespace FAST.Services.Models.Data
{
    /// <summary>
    /// Interface of data structure for Data Persistence Strategy
    /// </summary>
    public interface IdataStructurePersistence
    {
        /// <summary>
        /// The engine and the logic of the persistence 
        /// default is none
        /// </summary>
        public dataStructurePersistenceEngine engine { get; set; } 

        /// <summary>
        /// The connection name used by the entity.
        /// For SQL type of entries, is the connection to the data source
        /// </summary>
        public string source { get; set; }


        /// <summary>
        /// The name of the Entity. Can be the Table Name, The SP name
        /// or any other name the specific engine requires.
        /// </summary>
        public string entityName {  get; set; }

        /// <summary>
        /// The name of the field in the Entity. 
        /// Can be the Column Name, SP Argument (without the @) 
        /// or any other name the specific engine requires.
        /// </summary>
        public string fieldName { get; set; }

        /// <summary>
        /// The name of the Key1. It is engine specific,
        /// for SQL type of engines is the SQL key column
        /// </summary>
        public string key1Name { get; set; }

        /// <summary>
        /// The value of the Key1. It is engine specific,
        /// </summary>
        public string key1Value { get; set; }

        /// <summary>
        /// The name of the Key2. It is engine specific,
        /// for SQL type of engines is the SQL key column
        /// </summary>
        public string key2Name { get; set; }

        /// <summary>
        /// The value of the Key2. It is engine specific,
        /// </summary>
        public string key2Value { get; set; }


        /// <summary>
        /// The name of the Key3. It is engine specific,
        /// for SQL type of engines is the SQL key column
        /// </summary>
        public string key3Name { get; set; }

        /// <summary>
        /// The value of the Key3. It is engine specific,
        /// </summary>
        public string key3Value { get; set; }

    }


}
