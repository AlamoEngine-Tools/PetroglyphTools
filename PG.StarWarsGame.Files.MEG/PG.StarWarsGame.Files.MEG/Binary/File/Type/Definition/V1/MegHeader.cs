using System;
using System.Collections.Generic;
using PG.Commons.Binary;
using PG.Commons.Binary.File;

namespace PG.StarWarsGame.Files.MEG.Binary.File.Type.Definition.V1
{
    internal class MegHeader : IBinaryFile, ISizeable
    {
        private readonly uint m_numFileNames;
        private readonly uint m_numFiles;

        internal MegHeader(uint numFileNames, uint numFiles)
        {
            m_numFileNames = numFileNames;
            m_numFiles = numFiles;
        }


        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(m_numFileNames));
            bytes.AddRange(BitConverter.GetBytes(m_numFiles));
            return bytes.ToArray();
        }

        public int Size => sizeof(uint) * 2;

        internal uint NumFileNames => m_numFileNames;

        internal uint NumFiles => m_numFiles;
    }
}