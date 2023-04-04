// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Binary;
using PG.Commons.Binary.File;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.DAT.Test")]

namespace PG.StarWarsGame.Files.DAT.Binary.File.Type.Definition
{
    internal sealed class IndexTableRecord : IBinaryFile, ISizeable, IEquatable<IndexTableRecord>
    {
        public uint Crc32 { get; }
        public uint KeyLength { get; }
        public uint ValueLength { get; }

        public IndexTableRecord(uint crc32, uint keyLength, uint valueLength)
        {
            Crc32 = crc32;
            KeyLength = keyLength;
            ValueLength = valueLength;
        }

        public byte[] ToBytes()
        {
            List<byte> b = new();
            b.AddRange(BitConverter.GetBytes(Crc32));
            b.AddRange(BitConverter.GetBytes(ValueLength));
            b.AddRange(BitConverter.GetBytes(KeyLength));
            return b.ToArray();
        }

        public int Size { get; } = sizeof(uint) * 3;

        #region Auto-Generated IEquatable<T> Implementation

        public bool Equals(IndexTableRecord other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Crc32 == other.Crc32 && KeyLength == other.KeyLength;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is IndexTableRecord other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Crc32, KeyLength);
        }

        public static bool operator ==(IndexTableRecord left, IndexTableRecord right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(IndexTableRecord left, IndexTableRecord right)
        {
            return !Equals(left, right);
        }

        #endregion Auto-Generated IEquatable<T> Implementation
    }
}
