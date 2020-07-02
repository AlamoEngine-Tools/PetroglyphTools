using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using PG.Commons.Binary.File;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.MEG.Test")]
namespace PG.StarWarsGame.Files.MEG.Binary.File.Type.Definition
{
    internal class MegFile : IBinaryFile
    {
        [NotNull] private readonly MegHeader m_header;
        [NotNull] private readonly MegFileNameTable m_fileNameTable;
        [NotNull] private readonly MegFileContentTable m_fileContentTable;

        public MegFile(MegHeader header, MegFileNameTable fileNameTable, MegFileContentTable fileContentTable)
        {
            m_header = header ?? throw new ArgumentNullException(nameof(header));
            m_fileNameTable = fileNameTable ?? throw new ArgumentNullException(nameof(fileNameTable));
            m_fileContentTable = fileContentTable ?? throw new ArgumentNullException(nameof(fileContentTable));
        }

        public byte[] ToBytes()
        {
            List<byte> b = new List<byte>();
            b.AddRange(m_header.ToBytes());
            b.AddRange(m_fileNameTable.ToBytes());
            b.AddRange(m_fileContentTable.ToBytes());
            return b.ToArray();
        }
    }
}