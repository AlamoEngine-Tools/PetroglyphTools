using PG.Commons.Data.Files;

namespace PG.StarWarsGame.Files.DAT.Files
{
    public abstract class ADatAlamoFileType : IAlamoFileType
    {
        private const FileType FILE_TYPE = FileType.Binary;
        private const string FILE_EXTENSION = "dat";
        public abstract bool IsSorted { get; }
        public FileType Type => FILE_TYPE;
        public string FileExtension => FILE_EXTENSION;
    }
}
