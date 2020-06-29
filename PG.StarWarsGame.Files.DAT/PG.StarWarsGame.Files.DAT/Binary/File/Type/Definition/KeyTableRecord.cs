using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using PG.Commons.Binary;
using PG.Commons.Binary.File;
using PG.Commons.Util;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.DAT.Test")]

namespace PG.StarWarsGame.Files.DAT.Binary.File.Type.Definition
{
    public sealed class KeyTableRecord : IBinaryFile, ISizeable, IComparable<KeyTableRecord>, IEquatable<KeyTableRecord>
    {
        private static readonly Encoding ENCODING = Encoding.ASCII;

        public KeyTableRecord(string key)
        {
            Key = key.Replace("\0", string.Empty);
        }

        public KeyTableRecord(byte[] bytes, long index, long stringLength)
        {
            Debug.Assert(ENCODING != null, nameof(ENCODING) + " != null");
            char[] chars = ENCODING.GetChars(bytes, Convert.ToInt32(index), Convert.ToInt32(stringLength));
            Key = new string(chars);
        }


        public string Key { get; }

        public byte[] ToBytes()
        {
            Debug.Assert(ENCODING != null, nameof(ENCODING) + " != null");
            return ENCODING.GetBytes(Key);
        }

        public int CompareTo(KeyTableRecord other)
        {
            if (other == null) return 0;
            if (ChecksumUtility.GetChecksum(Key) > ChecksumUtility.GetChecksum(other.Key))
            {
                return 1;
            }
            if (ChecksumUtility.GetChecksum(Key) < ChecksumUtility.GetChecksum(other.Key))
            {
                return -1;
            }
            return 0;
        }

        public bool Equals(KeyTableRecord other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is KeyTableRecord other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int) ChecksumUtility.GetChecksum(Key);
        }

        public static bool operator <(KeyTableRecord left, KeyTableRecord right)
        {
            return left.CompareTo(right) == -1;
        }
        
        public static bool operator <=(KeyTableRecord left, KeyTableRecord right)
        {
            return left.CompareTo(right) == -1 || left.CompareTo(right) == 0;
        }
        
        public static bool operator >(KeyTableRecord left, KeyTableRecord right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator >=(KeyTableRecord left, KeyTableRecord right)
        {
            return left.CompareTo(right) == 1 || left.CompareTo(right) == 0;
        }

        public static bool operator ==(KeyTableRecord left, KeyTableRecord right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(KeyTableRecord left, KeyTableRecord right)
        {
            return !Equals(left, right);
        }

        public int Size => ToBytes() == null ? 0 : ToBytes().Length;
    }
}