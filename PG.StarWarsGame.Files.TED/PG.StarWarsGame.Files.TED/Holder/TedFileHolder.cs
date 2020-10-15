using PG.Commons.Data.Holder;
using PG.StarWarsGame.Files.TED.Files;
using Validation;

namespace PG.StarWarsGame.Files.TED.Holder
{
    public sealed class TedFileHolder : IFileHolder<TedMapModel, TedAlamoFileType>
    {
        public TedFileHolder(string filePath, string fileName, TedMapModel map)
        {
            Requires.NotNull(filePath, nameof(filePath));
            Requires.NotNull(fileName, nameof(fileName));
            FilePath = filePath;
            FileName = fileName;
        }
        public string FilePath { get; }
        public string FileName { get; }
        public TedAlamoFileType FileType { get; } = new TedAlamoFileType();
        public TedMapModel Content { get; set; }
    }
}
