// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using JetBrains.Annotations;
using PG.Commons.Binary;
using PG.Commons.Binary.File;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PG.StarWarsGame.Files.MEG.Binary.File.Type.Definition.V1
{
    internal class MegFileContentTable : IBinaryFile, ISizeable, IEnumerable<MegFileContentTableRecord>
    {
        public MegFileContentTable(List<MegFileContentTableRecord> megFileContentTableRecords)
        {
            m_megFileContentTableRecords = megFileContentTableRecords ?? new List<MegFileContentTableRecord>();
        }

        [NotNull] private readonly List<MegFileContentTableRecord> m_megFileContentTableRecords;

        public byte[] ToBytes()
        {
            List<byte> b = new();
            foreach (MegFileContentTableRecord megFileContentTableRecord in this)
            {
                b.AddRange(megFileContentTableRecord.ToBytes());
            }
            return b.ToArray();
        }

        public int Size => m_megFileContentTableRecords.Aggregate(0,
            (current, megFileContentTableRecord) => current + megFileContentTableRecord.Size);

        public IEnumerator<MegFileContentTableRecord> GetEnumerator()
        {
            return m_megFileContentTableRecords.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) m_megFileContentTableRecords).GetEnumerator();
        }

        public MegFileContentTableRecord this[int i] => m_megFileContentTableRecords[i];
    }
}
