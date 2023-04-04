// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using JetBrains.Annotations;
using PG.Commons.Binary.File;
using System;
using System.Collections.Generic;

namespace PG.StarWarsGame.Files.MTD.Binary.File.Type.Definition
{
    internal sealed class MtdFile : IBinaryFile
    {
        private readonly MtdHeader m_header;
        private readonly MtdImageTable m_imageTable;

        public MtdFile([NotNull] IEnumerable<MtdImageTableRecord> imageTableRecords) : this(
            new MtdImageTable(imageTableRecords))
        {
        }

        public MtdFile([NotNull] MtdImageTable imageTable)
        {
            m_imageTable = imageTable;
            m_header = new MtdHeader(Convert.ToUInt32(imageTable.Count));
        }

        public MtdFile([NotNull] MtdHeader header, [NotNull] MtdImageTable imageTable)
        {
            m_header = header;
            m_imageTable = imageTable;
        }

        public byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddRange(m_header.ToBytes());
            bytes.AddRange(m_imageTable.ToBytes());
            return bytes.ToArray();
        }
    }
}
