using PG.Commons.Data.Files;

namespace PG.StarWarsGame.Files.TED.Files
{
    /// <inheritdoc />
    public sealed class TedAlamoFileType : IAlamoFileType
    {
        /// <inheritdoc />
        public FileType Type => FileType.Chunk;
        /// <inheritdoc />
        public string FileExtension => "ted";
    }
}
