using System.Collections.Generic;
using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Holder
{
    public sealed class SortedDatFileHolder : ADatFileHolder<Dictionary<string,string>,SortedDatAlamoFileType>
    {
        public SortedDatFileHolder(string filePath, string fileName) : base(filePath, fileName)
        {

        }

        public override SortedDatAlamoFileType FileType { get; } = new SortedDatAlamoFileType();
        public override Dictionary<string, string> Content { get; set; }
    }
}
