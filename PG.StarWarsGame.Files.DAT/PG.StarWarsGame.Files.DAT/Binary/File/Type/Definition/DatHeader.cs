// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Binary;
using PG.Commons.Binary.File;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.DAT.Test")]

namespace PG.StarWarsGame.Files.DAT.Binary.File.Type.Definition
{
    internal sealed class DatHeader : IBinaryFile, ISizeable, IEquatable<DatHeader>
    {
        public uint KeyCount { get; }

        public DatHeader(uint keyCount)
        {
            KeyCount = keyCount;
        }

        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(KeyCount);
        }

        public int Size { get; } = sizeof(uint);

        #region Auto-Generated IEquatable<T> Implementation

        public bool Equals(DatHeader other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return KeyCount == other.KeyCount && Size == other.Size;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is DatHeader other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(KeyCount, Size);
        }

        public static bool operator ==(DatHeader left, DatHeader right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DatHeader left, DatHeader right)
        {
            return !Equals(left, right);
        }

        #endregion Auto-Generated IEquatable<T> Implementation
    }
}
