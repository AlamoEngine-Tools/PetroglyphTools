using System;
using System.Collections.Generic;
using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Holder
{
    public sealed class UnsortedDatFileHolder : ADatFileHolder<List<Tuple<string,string>>, UnsortedDatAlamoFileType>
    {
        public UnsortedDatFileHolder(string filePath, string fileName) : base(filePath, fileName)
        {
        }
        public override UnsortedDatAlamoFileType FileType { get; } = new UnsortedDatAlamoFileType();
        public override List<Tuple<string, string>> Content { get; set; }
    }
}
