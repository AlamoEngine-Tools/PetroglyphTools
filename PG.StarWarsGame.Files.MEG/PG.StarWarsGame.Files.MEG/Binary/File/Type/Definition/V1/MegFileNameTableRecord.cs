using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using PG.Commons.Binary;
using PG.Commons.Binary.File;
using PG.Commons.Util;

namespace PG.StarWarsGame.Files.MEG.Binary.File.Type.Definition.V1
{
    internal class MegFileNameTableRecord : IBinaryFile, ISizeable, IComparable
    {
        private readonly ushort m_fileNameLength;
        [NotNull] private readonly string m_fileName;
        private readonly Encoding m_fileNameEncoding = Encoding.ASCII;

        public MegFileNameTableRecord(string fileName)
        {
            if (!StringUtility.HasText(fileName))
                throw new ArgumentNullException($"{nameof(fileName)} may never be null.");
            string fn = fileName.ToUpper().Replace("\0", string.Empty);
            int l = fn.Length;
            try
            {
                m_fileNameLength = Convert.ToUInt16(l);
                m_fileName = fn;
            }
            catch (OverflowException)
            {
                throw new OverflowException(
                    $"The filename {fn} is too long to be inserted into a meg file. It may not exceed {ushort.MaxValue} characters.");
            }
        }

        public byte[] ToBytes()
        {
            List<byte> b = new List<byte>();
            b.AddRange(BitConverter.GetBytes(m_fileNameLength));
            b.AddRange(m_fileNameEncoding.GetBytes(m_fileName));
            return b.ToArray();
        }

        public int Size => ToBytes().Length;

        internal string FileName => m_fileName;
        
        #region Auto-Generated IComparable Implementation

        sealed class MegFileNameTableRecordComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                if (x == null || y == null)
                {
                    return 0;
                }

                if (!(x is MegFileNameTableRecord a) || !(y is MegFileNameTableRecord b))
                {
                    return 0;
                }

                if (ChecksumUtility.GetChecksum(a.FileName) < ChecksumUtility.GetChecksum(b.FileName))
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

            MegFileNameTableRecord b = obj as MegFileNameTableRecord;
            if (b != null && ChecksumUtility.GetChecksum(this.FileName) > ChecksumUtility.GetChecksum(b.FileName))
            {
                return 1;
            }

            if (b != null && ChecksumUtility.GetChecksum(this.FileName) < ChecksumUtility.GetChecksum(b.FileName))
            {
                return -1;
            }

            return 0;
        }

        #endregion
        
    }
}