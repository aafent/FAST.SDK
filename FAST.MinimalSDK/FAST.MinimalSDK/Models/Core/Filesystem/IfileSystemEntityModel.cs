namespace FAST.Core.FileSystem
{
    public interface IfileSystemEntityModel
    {
        string comments { get; set; }
        string contex { get; set; }
        string CurrentPath { get; set; }
        string DownloadPath { get; set; }
        string Filename { get; set; }
        string FileType { get; set; }
        DateTime InitialCreation { get; set; }
        string ParentPath { get; set; }
        long Size { get; set; }
        string title { get; set; }
    }
}