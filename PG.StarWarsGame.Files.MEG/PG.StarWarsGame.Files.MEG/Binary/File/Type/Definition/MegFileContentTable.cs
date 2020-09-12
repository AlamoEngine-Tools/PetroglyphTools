using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using PG.Commons.Binary;
using PG.Commons.Binary.File;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.MEG.Test")]

namespace PG.StarWarsGame.Files.MEG.Binary.File.Type.Definition
{
    internal class MegFileContentTable : IBinaryFile, ISizeable
    {
        [NotNull] private readonly List<MegFileContentTableRecord> m_megFileContentTableRecords;
        internal List<MegFileContentTableRecord> MegFileContentTableRecords => m_megFileContentTableRecords;

        public MegFileContentTable(List<MegFileContentTableRecord> megFileContentTableRecords)
        {
            m_megFileContentTableRecords = megFileContentTableRecords ?? new List<MegFileContentTableRecord>();
        }

        public byte[] ToBytes()
        {
            List<byte> b = new List<byte>();
            foreach (MegFileContentTableRecord megFileContentTableRecord in m_megFileContentTableRecords)
            {
                b.AddRange(megFileContentTableRecord.ToBytes());
            }

            return b.ToArray();
        }

        public int Size => m_megFileContentTableRecords.Aggregate(0,
            (current, megFileContentTableRecord) => current + megFileContentTableRecord.Size);

    }
}