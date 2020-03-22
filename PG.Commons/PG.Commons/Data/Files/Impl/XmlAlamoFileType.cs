namespace PG.Commons.Data.Files.Impl
{
    public class XmlAlamoFileType : IAlamoFileType
    {
        private const FileType FILE_TYPE = FileType.Text;
        private const string FILE_EXTENSION = "xml";

        public FileType Type => FILE_TYPE;
        public string FileExtension => FILE_EXTENSION;
    }
}
