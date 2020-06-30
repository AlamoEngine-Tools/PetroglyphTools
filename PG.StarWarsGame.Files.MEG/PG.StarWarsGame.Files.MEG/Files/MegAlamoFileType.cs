using PG.Commons.Data.Files;

namespace PG.StarWarsGame.Files.MEG.Files
{
    public sealed class MegAlamoFileType : IAlamoFileType
    {
        private const FileType FILE_TYPE = FileType.Binary;
        private const string FILE_EXTENSION = "meg";
        public FileType Type => FILE_TYPE;
        public string FileExtension => FILE_EXTENSION;
    }
}