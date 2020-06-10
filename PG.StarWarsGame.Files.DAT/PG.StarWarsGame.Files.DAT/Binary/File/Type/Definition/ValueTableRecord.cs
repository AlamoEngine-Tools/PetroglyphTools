using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using PG.Commons.Binary;
using PG.Commons.Binary.File;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.DAT.Test")]

namespace PG.StarWarsGame.Files.DAT.Binary.File.Type.Definition
{
    public sealed class ValueTableRecord : IBinaryFile, ISizeable, IComparable<ValueTableRecord>
    {
        private string m_value;
        private static readonly Encoding ENCODING = Encoding.Unicode;

        public string Value
        {
            get => m_value;
            set => m_value = value.Replace("\0", string.Empty);
        }

        public ValueTableRecord(string value)
        {
            Value = value;
        }

        public ValueTableRecord(byte[] bytes, long index, long stringLength)
        {
            Debug.Assert(ENCODING != null, nameof(ENCODING) + " != null");
            char[] chars = ENCODING.GetChars(bytes, Convert.ToInt32(index), Convert.ToInt32(stringLength * 2));
            Value = new string(chars);
        }

        public byte[] ToBytes()
        {
            Debug.Assert(ENCODING != null, nameof(ENCODING) + " != null");
            return ENCODING.GetBytes(Value);
        }

        public int Size => ToBytes() == null ? 0 : ToBytes().Length;

        public int CompareTo(ValueTableRecord other)
        {
            throw new NotImplementedException();
        }
    }
}
