// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using PG.Commons.Binary;
using PG.Commons.Binary.File;

namespace PG.StarWarsGame.Files.MEG.Binary.File.Type.Definition.V1
{
    internal class MegFileContentTableRecord : IBinaryFile, ISizeable, IComparable
    {
        private readonly uint m_crc32;
        private readonly uint m_fileNameTableIndex;

        public MegFileContentTableRecord(uint crc32, uint fileTableRecordIndex, uint fileSizeInBytes,
            uint fileStartOffsetInBytes, uint fileNameTableIndex)
        {
            m_crc32 = crc32;
            FileTableRecordIndex = fileTableRecordIndex;
            FileSizeInBytes = fileSizeInBytes;
            FileStartOffsetInBytes = fileStartOffsetInBytes;
            m_fileNameTableIndex = fileNameTableIndex;
        }

        internal uint FileTableRecordIndex { get; set; }

        internal uint FileStartOffsetInBytes { get; set; }

        internal uint FileSizeInBytes { get; }

        public byte[] ToBytes()
        {
            List<byte> b = new List<byte>();
            b.AddRange(BitConverter.GetBytes(m_crc32));
            b.AddRange(BitConverter.GetBytes(FileTableRecordIndex));
            b.AddRange(BitConverter.GetBytes(FileSizeInBytes));
            b.AddRange(BitConverter.GetBytes(FileStartOffsetInBytes));
            b.AddRange(BitConverter.GetBytes(m_fileNameTableIndex));
            return b.ToArray();
        }

        public int Size => sizeof(uint) * 5;

        #region Auto-Generated IComparable Implementation

        private sealed class MegFileContentTableRecordComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                if (x == null || y == null)
                {
                    return 0;
                }

                if (!(x is MegFileContentTableRecord a) || !(y is MegFileContentTableRecord b))
                {
                    return 0;
                }

                if (a.m_crc32 < b.m_crc32)
                {
                    return -1;
                }

                return 1;
            }
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 0;
            }

            MegFileContentTableRecord b = obj as MegFileContentTableRecord;
            if (b != null && m_crc32 > b.m_crc32)
            {
                return 1;
            }

            if (b != null && m_crc32 < b.m_crc32)
            {
                return -1;
            }

            return 0;
        }

        #endregion
    }
}
