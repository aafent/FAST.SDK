namespace FAST.Printing
{
    public interface IPrinter
    {
        string printerName {get;}
        bool printToFile { get; }
        string filePath { get; }
        string fileName { get; }
        string fileExtension { get; }
        int copies { get; }
        void setTemporaryFileName(string fileName);
        string getTemporaryFileName();
    }
}
