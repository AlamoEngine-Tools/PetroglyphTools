// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using JetBrains.Annotations;
using System;

namespace PG.Commons.Util
{
    /// <summary>
    /// A utility class to handle Petroglyph's CRC32 checksum implementation used throughout the Alamo Engine
    /// </summary>
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
                uint crc = (uint) i;
                for (int j = 0; j < 8; j++)
                {
                    crc = ((crc & 1) != 0) ? (crc >> 1) ^ 0xEDB88320 : (crc >> 1);
                }

                LOOKUP_TABLE[i] = crc & 0xFFFFFFFF;
            }
        }

        /// <summary>
        /// Computes the CRC32 checksum for a given not nullable <c>string</c> as <code>uint</code>.
        /// </summary>
        /// <param name="s">Not-nullable input.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Convenience method to directly get a CRC32 checksum as byte array.
        /// Analogue to calling <code>BitConverter.GetBytes(ChecksumUtility.ComputeCrc32(s))</code>
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] GetChecksumAsBytes([NotNull] string s)
        {
            return BitConverter.GetBytes(ComputeCrc32(s));
        }
    }
}
