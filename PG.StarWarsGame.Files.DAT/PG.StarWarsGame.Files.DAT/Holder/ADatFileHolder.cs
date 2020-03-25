using PG.Commons.Data.Files;
using PG.Commons.Data.Holder;

namespace PG.StarWarsGame.Files.DAT.Holder
{
    public abstract class ADatFileHolder<TContent, TAlamoFileType> : IFileHolder<TContent, TAlamoFileType> where TAlamoFileType : IAlamoFileType, new()
    {
        protected ADatFileHolder(string filePath, string fileName)
        {
            FilePath = filePath;
            FileName = fileName;
        }

        public string FilePath { get; }
        public string FileName { get; }

        public abstract TAlamoFileType FileType { get; }
        public abstract TContent Content { get; set; }
    }
}
