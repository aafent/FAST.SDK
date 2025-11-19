using FAST.Core.FileSystem;

namespace FAST.Services.Models
{
    [Obsolete]
    public class FileManagerModel : fileManagerModel {}


    public class fileManagerModel : IfileSystemEntityModel
    {
        public fileManagerModel()
        {
        }
        public fileManagerModel(IfileSystemEntityModel model)
        {
            this.comments= model.comments;
            this.contex =model.contex;
            this.CurrentPath= model.CurrentPath;
            this.DownloadPath= model.DownloadPath;
            this.Filename= model.Filename;
            this.FileType= model.FileType;
            this.InitialCreation= model.InitialCreation;
            this.ParentPath= model.ParentPath;
            this.Size= model.Size;
            this.title= model.title;
        }


        public string contex { get; set; } = "";
        /// <summary>
        /// The title of the file. The folder name if it is folder, the file name without the extention if it is file.
        /// It is only for houman reading not for any process.
        /// // eg: MyFile
        /// </summary>
        public string title { get; set; } = "";
        /// <summary>
        /// the file name with the extension
        /// eg: myFile.txt
        /// </summary>
        public string Filename { get; set; } = "";

        /// <summary>
        /// eg: folder1\folder2
        /// </summary>
        public string CurrentPath { get; set; } = "";

        /// <summary>
        /// folder1
        /// </summary>
        public string ParentPath { get; set; } = "";


        public string FileType { get; set; } = "";
        public string DownloadPath { get; set; } = "";
        public DateTime InitialCreation { get; set; }
        public long Size { get; set; }
        public string comments { get; set; }


        public IfileSystemEntityModel toModel() => this;

    }
}
