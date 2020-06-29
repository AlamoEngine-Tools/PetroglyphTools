using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using PG.Commons.Binary;
using PG.Commons.Binary.File;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.DAT.Test")]

namespace PG.StarWarsGame.Files.DAT.Binary.File.Type.Definition
{
    public sealed class ValueTableRecord : IBinaryFile, ISizeable, IEquatable<ValueTableRecord>,
        IComparable<ValueTableRecord>
    {
        private static readonly Encoding ENCODING = Encoding.Unicode;

        public ValueTableRecord(string value)
        {
            Value = value.Replace("\0", string.Empty);
        }

        public ValueTableRecord(byte[] bytes, long index, long stringLength)
        {
            Debug.Assert(ENCODING != null, nameof(ENCODING) + " != null");
            char[] chars = ENCODING.GetChars(bytes, Convert.ToInt32(index), Convert.ToInt32(stringLength * 2));
            Value = new string(chars);
        }

        public string Value { get; }

        public byte[] ToBytes()
        {
            Debug.Assert(ENCODING != null, nameof(ENCODING) + " != null");
            return ENCODING.GetBytes(Value);
        }

        public int CompareTo(ValueTableRecord other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return string.Compare(Value, other.Value, StringComparison.Ordinal);
        }

        public bool Equals(ValueTableRecord other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return CompareTo(other) == 0;
        }

        public int Size => ToBytes() == null ? 0 : ToBytes().Length;

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is ValueTableRecord other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value != null ? Value.GetHashCode() : 0;
        }

        public static bool operator ==(ValueTableRecord left, ValueTableRecord right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ValueTableRecord left, ValueTableRecord right)
        {
            return !Equals(left, right);
        }

        public static bool operator <(ValueTableRecord left, ValueTableRecord right)
        {
            return Comparer<ValueTableRecord>.Default.Compare(left, right) < 0;
        }

        public static bool operator >(ValueTableRecord left, ValueTableRecord right)
        {
            return Comparer<ValueTableRecord>.Default.Compare(left, right) > 0;
        }

        public static bool operator <=(ValueTableRecord left, ValueTableRecord right)
        {
            return Comparer<ValueTableRecord>.Default.Compare(left, right) <= 0;
        }

        public static bool operator >=(ValueTableRecord left, ValueTableRecord right)
        {
            return Comparer<ValueTableRecord>.Default.Compare(left, right) >= 0;
        }
    }
}