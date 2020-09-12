using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PG.Commons.Binary;
using PG.Commons.Binary.File;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.MEG.Test")]

namespace PG.StarWarsGame.Files.MEG.Binary.File.Type.Definition
{
    internal class MegFileContentTableRecord : IBinaryFile, ISizeable, IComparable
    {
        private readonly uint m_crc32;
        private uint m_fileTableRecordIndex;
        private readonly uint m_fileSizeInBytes;
        private uint m_fileStartOffsetInBytes;
        private readonly uint m_fileNameTableIndex;

        public MegFileContentTableRecord(uint crc32, uint fileTableRecordIndex, uint fileSizeInBytes,
            uint fileStartOffsetInBytes, uint fileNameTableIndex)
        {
            m_crc32 = crc32;
            FileTableRecordIndex = fileTableRecordIndex;
            m_fileSizeInBytes = fileSizeInBytes;
            FileStartOffsetInBytes = fileStartOffsetInBytes;
            m_fileNameTableIndex = fileNameTableIndex;
        }

        public byte[] ToBytes()
        {
            List<byte> b = new List<byte>();
            b.AddRange(BitConverter.GetBytes(m_crc32));
            b.AddRange(BitConverter.GetBytes(m_fileTableRecordIndex));
            b.AddRange(BitConverter.GetBytes(m_fileSizeInBytes));
            b.AddRange(BitConverter.GetBytes(m_fileStartOffsetInBytes));
            b.AddRange(BitConverter.GetBytes(m_fileNameTableIndex));
            return b.ToArray();
        }

        public int Size => sizeof(uint) * 5;

        internal uint FileTableRecordIndex
        {
            get => m_fileTableRecordIndex;
            set => m_fileTableRecordIndex = value;
        }

        internal uint FileStartOffsetInBytes
        {
            get => m_fileStartOffsetInBytes;
            set => m_fileStartOffsetInBytes = value;
        }

        internal uint FileSizeInBytes => m_fileSizeInBytes;

        #region Auto-Generated IComparable Implementation

        sealed class MegFileContentTableRecordComparer : IComparer
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