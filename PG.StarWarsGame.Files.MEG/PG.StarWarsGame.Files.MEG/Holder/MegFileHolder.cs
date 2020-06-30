using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using PG.Commons.Data.Holder;
using PG.StarWarsGame.Files.MEG.Files;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.MEG.Test")]

namespace PG.StarWarsGame.Files.MEG.Holder
{
    public sealed class MegFileHolder : IFileHolder<List<string>, MegAlamoFileType>
    {
        public MegFileHolder([NotNull] string filePath, [NotNull] string fileName)
        {
            FilePath = filePath ?? throw new ArgumentNullException($"Argument {nameof(filePath)} may not be null.");
            FileName = fileName ?? throw new ArgumentNullException($"Argument {nameof(fileName)} may not be null.");
        }

        public string FilePath { get; }
        public string FileName { get; }
        public MegAlamoFileType FileType { get; } = new MegAlamoFileType();
        public List<string> Content { get; set; } = new List<string>();
    }
}