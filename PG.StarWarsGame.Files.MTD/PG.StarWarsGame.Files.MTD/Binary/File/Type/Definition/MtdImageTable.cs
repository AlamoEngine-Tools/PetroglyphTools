// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using PG.Commons.Binary;
using PG.Commons.Binary.File;

namespace PG.StarWarsGame.Files.MTD.Binary.File.Type.Definition
{
    internal sealed class MtdImageTable : IBinaryFile, ISizeable, IEnumerable<MtdImageTableRecord>
    {
        private readonly List<MtdImageTableRecord> m_mtdImageTableRecords;
        public MtdImageTable([NotNull] IEnumerable<MtdImageTableRecord> mtdImageTableRecords)
        {
            IList<MtdImageTableRecord> imageTableRecords = mtdImageTableRecords.ToList();
            if (!imageTableRecords.Any())
            {
                throw new ArgumentException(
                    $"The provided list of {nameof(MtdImageTableRecord)} must never be empty.",
                    nameof(mtdImageTableRecords));
            }

            m_mtdImageTableRecords = new List<MtdImageTableRecord>(imageTableRecords);
        }

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            foreach (MtdImageTableRecord mtdImageTableRecord in this)
            {
                bytes.AddRange(mtdImageTableRecord.ToBytes());
            }

            return bytes.ToArray();
        }

        public int Size => m_mtdImageTableRecords.Sum(r => r.Size);

        public IEnumerator<MtdImageTableRecord> GetEnumerator()
        {
            return m_mtdImageTableRecords.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) m_mtdImageTableRecords).GetEnumerator();
        }

        public MtdImageTableRecord this[int i] => m_mtdImageTableRecords[i];
        public int Count => m_mtdImageTableRecords.Count;
    }
}
