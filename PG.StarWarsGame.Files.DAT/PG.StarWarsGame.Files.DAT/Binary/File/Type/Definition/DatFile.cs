// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using JetBrains.Annotations;
using PG.Commons.Binary.File;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.DAT.Test")]

namespace PG.StarWarsGame.Files.DAT.Binary.File.Type.Definition
{
    internal sealed class DatFile : IBinaryFile, IEquatable<DatFile>
    {
        [NotNull] private readonly DatHeader m_header;
        [NotNull] private readonly IndexTable m_indexTable;
        [NotNull] private readonly ValueTable m_valueTable;
        [NotNull] private readonly KeyTable m_keyTable;

        public DatFile([NotNull] DatHeader header, [NotNull] IndexTable indexTable, [NotNull] ValueTable valueTable,
            [NotNull] KeyTable keyTable)
        {
            m_header = header;
            m_indexTable = indexTable;
            m_valueTable = valueTable;
            m_keyTable = keyTable;
        }

        public uint KeyValuePairCount => m_header.KeyCount;
        public List<ValueTableRecord> Values => m_valueTable.ValueTableRecords;
        public List<KeyTableRecord> Keys => m_keyTable.KeyTableRecords;

        public byte[] ToBytes()
        {
            List<byte> b = new List<byte>();
            b.AddRange(m_header.ToBytes());
            b.AddRange(m_indexTable.ToBytes());
            b.AddRange(m_valueTable.ToBytes());
            b.AddRange(m_keyTable.ToBytes());
            return b.ToArray();
        }

        #region Auto-Generated IEquatable<DatFile> Implementation

        public bool Equals(DatFile other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return m_header.Equals(other.m_header) && m_indexTable.Equals(other.m_indexTable);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is DatFile other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(m_header, m_indexTable, m_valueTable, m_keyTable);
        }

        public static bool operator ==(DatFile left, DatFile right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DatFile left, DatFile right)
        {
            return !Equals(left, right);
        }

        #endregion Auto-Generated IEquatable<DatFile> Implementation
    }
}
