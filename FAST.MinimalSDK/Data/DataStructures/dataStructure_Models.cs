using FAST.Data;

namespace FAST.Services.Models.Data
{

    /// <summary>
    /// A model class of IdataStructureElementExtension
    /// </summary>
    public class dataStructureElementExtensionModel : IdataStructureElementExtension
    {
    }


    /// <summary>
    /// A model class of IdataStructurePersistence
    /// </summary>
    public class dataStructurePersistenceModel : IdataStructurePersistence
    {
        public dataStructurePersistenceEngine engine { get; set; } = dataStructurePersistenceEngine.none;
        public string source { get; set; } = null;
        public string entityName { get; set; } = null;
        public string fieldName { get; set; } = null;
        public string key1Name { get; set; } = null;
        public string key1Value { get; set; } = null;
        public string key2Name { get; set; } = null;
        public string key2Value { get; set; } = null;
        public string key3Name { get; set; } = null;
        public string key3Value { get; set; } = null;
    }

    /// <summary>
    /// A model class of IdataStructureFieldMapping
    /// </summary>
    public class dataStructureDataMappingModel : IdataStructureFieldMapping
    {
        public dataMappingDirection direction { get; set; } = dataMappingDirection.none;
        public string arg1 { get; set; } = null;
    }


    /// <summary>
    /// A model class of IdataStructureFieldValidation
    /// </summary>
    public class dataStructureDataFieldValidationModel : IdataStructureFieldValidation
    {
        public dataStructureFieldValidationEngine engine { get; set; } = dataStructureFieldValidationEngine.none;
        public string arg1 { get; set; } = null;
        public string arg2 { get; set; } = null;
        public string arg3 { get; set; } = null;

        public bool isValid<T>(T value)
        {
            throw new NotImplementedException();
        }
    }


}
