namespace MyTrainingV1231AngularDemo.Storage
{
    public class TempFileInfo
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public byte[] File { get; set; }

        public TempFileInfo()
        {
        }

        public TempFileInfo(byte[] file)
        {
            File = file;
        }

        public TempFileInfo(string fileName, string fileType, byte[] file)
        {
            FileName = fileName;
            FileType = fileType;
            File = file;
        }
    }
}