using PG.Commons.Data.Files;

namespace PG.StarWarsGame.Files.MEG.Files.V1
{
    /// <summary>
    /// Minimal file info for the <a href="https://modtools.petrolution.net/docs/MegFileFormat">v1 <code>*.MEG</code> archive file type</a>.
    /// </summary>
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