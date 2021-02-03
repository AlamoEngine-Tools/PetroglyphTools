// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using PG.Commons.Binary;
using PG.Commons.Binary.File;

namespace PG.StarWarsGame.Files.MEG.Binary.File.Type.Definition.V1
{
    internal class MegFileNameTable : IBinaryFile, ISizeable, IEnumerable<MegFileNameTableRecord>
    {
        public MegFileNameTable(List<MegFileNameTableRecord> megFileNameTableRecords)
        {
            m_megFileNameTableRecords = megFileNameTableRecords ?? new List<MegFileNameTableRecord>();
        }

        [NotNull] private readonly List<MegFileNameTableRecord> m_megFileNameTableRecords;

        public byte[] ToBytes()
        {
            List<byte> b = new List<byte>();
            foreach (MegFileNameTableRecord megFileNameTableRecord in this)
            {
                b.AddRange(megFileNameTableRecord.ToBytes());
            }

            return b.ToArray();
        }

        public int Size => ToBytes().Length;

        public IEnumerator<MegFileNameTableRecord> GetEnumerator()
        {
            return m_megFileNameTableRecords.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) m_megFileNameTableRecords).GetEnumerator();
        }

        public MegFileNameTableRecord this[int i] => m_megFileNameTableRecords[i];
    }
}
