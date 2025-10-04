using System.Xml.Serialization;

namespace FAST.Logging
{
    [Serializable]
    [XmlRoot("exceptionContainer")]
    public class exceptionContainer
{
    [XmlAttribute]
    public DateTime timestampUTC { get; set; }
    public string message { get; set; }
    public string stackTrace { get; set; }


    [XmlAttribute]
    public string source { get; set; }


    public exceptionContainer()
    {
        this.timestampUTC = DateTime.UtcNow;
    }

    public exceptionContainer(string message) : this()
    {
        this.message = message;
    }

    public exceptionContainer(System.Exception ex) : this(ex.Message)
    {
        this.stackTrace = ex.StackTrace;
        this.source = ex.Source;
    }

    public override string ToString()
    {
        return this.message;
    }

    public Exception toException()
    {
        Exception ex = new Exception(this.message);
        ex.Source = source;
        return ex;
    }
}

}
