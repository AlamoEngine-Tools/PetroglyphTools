// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Buffers.Binary;
using PG.Commons.Binary;
using PG.Commons.Utilities;

namespace PG.StarWarsGame.Files.MEG.Binary.Metadata;

internal readonly struct MegFileNameTableRecord : IBinary
{
    private readonly ushort _fileNameLength;

    internal string FileName { get; }

    internal string OriginalFilePath { get; }

    public byte[] Bytes
    {
        get
        {
            var bytes = new byte[Size];
            BinaryPrimitives.WriteUInt16LittleEndian(bytes, _fileNameLength);

#if NETSTANDARD2_1_OR_GREATER
            var fileNameArea = bytes.AsSpan(sizeof(ushort));
            MegFileConstants.MegContentFileNameEncoding.GetBytes(FileName, fileNameArea);
#else
            MegFileConstants.MegContentFileNameEncoding.GetBytes(FileName, 0, FileName.Length, bytes, sizeof(ushort));
#endif
            return bytes;
        }
    }

    public int Size => sizeof(ushort) + _fileNameLength;

    public MegFileNameTableRecord(string filePath)
    {
        Commons.Utilities.ThrowHelper.ThrowIfNullOrWhiteSpace(filePath);

        var encoding = MegFileConstants.MegContentFileNameEncoding;
        OriginalFilePath = filePath;

        var charCount = StringUtilities.ValidateStringCharLengthUInt16(filePath);
        _fileNameLength = charCount;

        var byteCount = encoding.GetByteCountPG(charCount);

        // Encoding the string as ASCII has the potential of creating PG/Windows
        // illegal file names due to the replacement character '?'. 
        // At this stage we don't check for sanity in order to read .MEG files created by other tools, such as Mike's MEG Editor.
        FileName = encoding.EncodeString(filePath, byteCount);
    }

    internal static int GetRecordSize(string filePath)
    {
        return sizeof(ushort) + filePath.Length;
    }
}
