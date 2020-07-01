using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using PG.Commons.Binary;
using PG.Commons.Binary.File;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.MEGs.Test")]

namespace PG.StarWarsGame.Files.MEG.Binary.File.Type.Definition
{
    internal class MegFileNameTable : IBinaryFile, ISizeable
    {
        [NotNull] private readonly List<MegFileNameTableRecord> m_megFileNameTableRecords;

        public MegFileNameTable(List<MegFileNameTableRecord> megFileNameTableRecords)
        {
            m_megFileNameTableRecords = megFileNameTableRecords ?? new List<MegFileNameTableRecord>();
        }

        public byte[] ToBytes()
        {
            List<byte> b = new List<byte>();
            foreach (MegFileNameTableRecord megFileNameTableRecord in m_megFileNameTableRecords)
            {
                b.AddRange(megFileNameTableRecord.ToBytes());
            }

            return b.ToArray();
        }

        public int Size => ToBytes().Length;
    }
}