using System;
using JetBrains.Annotations;

namespace PG.Commons.Util
{
    public static class ChecksumUtility
    {
        [NotNull] private static readonly ulong[] LOOKUP_TABLE = new ulong[256];

        static ChecksumUtility()
        {
            InitTable();
        }

        private static void InitTable()
        {
            for (int i = 0; i < 256; i++)
            {
                uint crc = (uint)i;
                for (int j = 0; j < 8; j++)
                {
                    crc = ((crc & 1) != 0) ? (crc >> 1) ^ 0xEDB88320 : (crc >> 1);
                }
                LOOKUP_TABLE[i] = crc & 0xFFFFFFFF;
            }
        }

        public static uint GetChecksum([NotNull] string s)
        {
            return ComputeCrc32(s);
        }

        private static uint ComputeCrc32([NotNull] string s)
        {
            ulong crc = 0xFFFFFFFF;
            for (int j = 0; j < s.Length; j++)
            {
                crc = ((crc >> 8) & 0x00FFFFFF) ^ LOOKUP_TABLE[(crc ^ (s)[j]) & 0xFF];
            }
            return Convert.ToUInt32(crc ^ 0xFFFFFFFF);
        }

        public static byte[] GetChecksumAsBytes([NotNull] string s)
        {
            return BitConverter.GetBytes(ComputeCrc32(s));
        }
    }
}
