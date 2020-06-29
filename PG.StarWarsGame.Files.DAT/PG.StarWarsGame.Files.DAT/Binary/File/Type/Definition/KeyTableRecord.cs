using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using PG.Commons.Binary;
using PG.Commons.Binary.File;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.DAT.Test")]

namespace PG.StarWarsGame.Files.DAT.Binary.File.Type.Definition
{
    public sealed class KeyTableRecord : IBinaryFile, ISizeable, IComparable<KeyTableRecord>
    {
        private string m_key;
        private static readonly Encoding ENCODING = Encoding.ASCII;


        public string Key
        {
            get => m_key;
            set
            {
                if (value != null)
                {
                    m_key = value.Replace("\0", string.Empty);
                }
            }
        }

        public KeyTableRecord(string key)
        {
            Key = key;
        }

        public KeyTableRecord(byte[] bytes, long index, long stringLength)
        {
            Debug.Assert(ENCODING != null, nameof(ENCODING) + " != null");
            char[] chars = ENCODING.GetChars(bytes, Convert.ToInt32(index), Convert.ToInt32(stringLength));
            Key = new string(chars);
        }

        public byte[] ToBytes()
        {
            Debug.Assert(ENCODING != null, nameof(ENCODING) + " != null");
            return ENCODING.GetBytes(Key);
        }

        public int Size => ToBytes() == null ? 0 : ToBytes().Length;

        public int CompareTo(KeyTableRecord other)
        {
            if (other == null)
            {
                return 0;
            }

            return !(other is KeyTableRecord r) ? 0 : string.Compare(Key, r.Key, StringComparison.Ordinal);
        }
    }
}
