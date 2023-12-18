// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Buffers.Binary;
using PG.Commons.Binary;

using PG.StarWarsGame.Files.MEG.Utilities;

#if NETSTANDARD2_1_OR_GREATER || NET
using System;
#endif

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
            MegFileConstants.MegDataEntryPathEncoding.GetBytes(FileName, fileNameArea);
#else
            MegFileConstants.MegDataEntryPathEncoding.GetBytes(FileName, 0, FileName.Length, bytes, sizeof(ushort));
#endif
            return bytes;
        }
    }

    public int Size => sizeof(ushort) + _fileNameLength;

    public MegFileNameTableRecord(string filePath)
    {
        Commons.Utilities.ThrowHelper.ThrowIfNullOrWhiteSpace(filePath);

        var encoding = MegFileConstants.MegDataEntryPathEncoding;
        OriginalFilePath = filePath;

        // Encoding the string as ASCII has the potential of creating PG/Windows
        // illegal file names due to the replacement character '?'. 
        // At this stage we don't check for sanity in order to read .MEG files created by other tools, such as Mike's MEG Editor.
        FileName = MegFilePathUtilities.EncodeMegFilePath(filePath, encoding);
        _fileNameLength = MegFilePathUtilities.ValidateFilePathCharacterLength(FileName);
    }

    internal static int GetRecordSize(string filePath)
    {
        return sizeof(ushort) + filePath.Length;
    }
}