namespace FAST.Data
{
    /// <summary>
    /// Data Mapping - Default Processor, 
    /// supports only build-ins and add-ons. 
    /// </summary>
    public class dataMappingDefaultProcessor : dataMappingProcessorAbstract
    {
        public override bool tryMapping(bool isIncoming, string identifier, object input, out object value)
        {
            value = input;
            return false;
        }

        public override bool tryMapping(bool isIncoming, string identifier, string args, object input, out object value)
        {
            value = input;
            return false;
        }

    }
}
