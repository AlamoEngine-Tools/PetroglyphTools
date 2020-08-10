using PG.Commons.Data.Files;

namespace PG.StarWarsGame.Files.MEG.Files
{
    /// <inheritdoc />
    public sealed class MegAlamoFileType : IAlamoFileType
    {
        private const FileType FILE_TYPE = FileType.Binary;
        private const string FILE_EXTENSION = "meg";

        /// <inheritdoc />
        public FileType Type => FILE_TYPE;

        /// <inheritdoc />
        public string FileExtension => FILE_EXTENSION;
    }
}