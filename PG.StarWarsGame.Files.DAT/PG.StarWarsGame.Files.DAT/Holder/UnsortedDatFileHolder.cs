using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PG.StarWarsGame.Files.DAT.Files;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.DAT.Test")]

namespace PG.StarWarsGame.Files.DAT.Holder
{
    public sealed class UnsortedDatFileHolder : ADatFileHolder<List<Tuple<string, string>>, UnsortedDatAlamoFileType>
    {
        public UnsortedDatFileHolder(string filePath, string fileName) : base(filePath, fileName)
        {
        }

        public override UnsortedDatAlamoFileType FileType { get; } = new UnsortedDatAlamoFileType();
        public override List<Tuple<string, string>> Content { get; set; }
    }
}
