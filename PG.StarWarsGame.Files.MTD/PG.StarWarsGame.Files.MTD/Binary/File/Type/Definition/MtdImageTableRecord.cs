// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Text;
using PG.Commons.Binary;
using PG.Commons.Binary.File;
using PG.Commons.Util;
using PG.StarWarsGame.Files.MTD.Commons.Exceptions;

namespace PG.StarWarsGame.Files.MTD.Binary.File.Type.Definition
{
    internal sealed class MtdImageTableRecord : IBinaryFile, ISizeable
    {
        internal const int BINARY_NAME_LENGTH = 64;
        private readonly string m_name;
        private readonly uint m_xPosition;
        private readonly uint m_yPosition;
        private readonly uint m_xExtend;
        private readonly uint m_yExtend;
        private readonly bool m_alpha;
        private readonly IEnumerable<byte> m_binaryName;

        public MtdImageTableRecord([NotNull] string name, uint xPosition, uint yPosition, uint xExtend, uint yExtend,
            bool alpha)
        {
            if (!StringUtility.HasText(name))
            {
                throw new ArgumentException($"{nameof(name)} may never be null, empty or whitespace.");
            }

            m_name = name.Trim().ToUpper();
            m_xPosition = xPosition;
            m_yPosition = yPosition;
            m_xExtend = xExtend;
            m_yExtend = yExtend;
            m_alpha = alpha;
            m_binaryName = CreateBinaryName();
        }

        private IEnumerable<byte> CreateBinaryName()
        {
            byte[] bytes = new byte[BINARY_NAME_LENGTH];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = 0;
            }

            byte[] unpaddedName = Encoding.ASCII.GetBytes(m_name);
            if (unpaddedName.Length > BINARY_NAME_LENGTH)
            {
                throw new InvalidIconNameException(
                    $"An MTD-element's name may only be {BINARY_NAME_LENGTH} bytes long. The element \'{m_name}\' is {unpaddedName.Length} bytes long and exceeds the limit by {unpaddedName.Length - BINARY_NAME_LENGTH}.");
            }

            for (int i = 0; i < unpaddedName.Length && i < BINARY_NAME_LENGTH; i++)
            {
                bytes[i] = unpaddedName[i];
            }

            return bytes;
        }

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(m_binaryName);
            bytes.AddRange(BitConverter.GetBytes(m_xPosition));
            bytes.AddRange(BitConverter.GetBytes(m_yPosition));
            bytes.AddRange(BitConverter.GetBytes(m_xExtend));
            bytes.AddRange(BitConverter.GetBytes(m_yExtend));
            bytes.AddRange(BitConverter.GetBytes(m_alpha));
            return bytes.ToArray();
        }

        public int Size => sizeof(byte) * BINARY_NAME_LENGTH + sizeof(uint) + sizeof(uint) + sizeof(uint) +
                           sizeof(uint) + sizeof(bool);
    }
}
