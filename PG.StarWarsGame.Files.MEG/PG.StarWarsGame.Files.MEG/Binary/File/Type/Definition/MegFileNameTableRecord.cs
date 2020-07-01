using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using PG.Commons.Binary;
using PG.Commons.Binary.File;
using PG.Commons.Util;

namespace PG.StarWarsGame.Files.MEG.Binary.File.Type.Definition
{
    internal class MegFileNameTableRecord : IBinaryFile, ISizeable
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
    }
}