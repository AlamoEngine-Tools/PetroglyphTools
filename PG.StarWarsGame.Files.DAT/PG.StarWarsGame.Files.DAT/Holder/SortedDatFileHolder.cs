using System.Collections.Generic;
using PG.Commons.Data.Holder;
using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Holder
{
    public sealed class SortedDatFileHolder : IFileHolder<Dictionary<string,string>,SortedDatAlamoFileType>
    {
        public SortedDatFileHolder(string filePath, string fileName)
        {
            FilePath = filePath;
            FileName = fileName;
        }
        public string FilePath { get; }
        public string FileName { get; }
        public SortedDatAlamoFileType FileType { get; } = new SortedDatAlamoFileType();
        public Dictionary<string, string> Content { get; set; }
    }
}
