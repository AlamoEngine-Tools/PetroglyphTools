// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Buffers.Binary;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MEG.Utilities;
using AnakinRaW.CommonUtilities;
using PG.StarWarsGame.Files.Binary;
#if NETSTANDARD2_0
using AnakinRaW.CommonUtilities.Extensions;
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
            var bytesSpan = bytes.AsSpan();
            
            BinaryPrimitives.WriteUInt16LittleEndian(bytesSpan, _fileNameLength);
            MegFileConstants.MegDataEntryPathEncoding.GetBytes(FileName.AsSpan(), bytesSpan.Slice(sizeof(ushort)));

            return bytes;
        }
    }

    public int Size => sizeof(ushort) + _fileNameLength;

    public MegFileNameTableRecord(string filePath, string originalFilePath)
    {
        ThrowHelper.ThrowIfNullOrEmpty(originalFilePath);
        ThrowHelper.ThrowIfNullOrWhiteSpace(filePath);
        StringUtilities.ValidateIsAsciiOnly(filePath.AsSpan());
        _fileNameLength = MegFilePathUtilities.ValidateFilePathCharacterLength(filePath);

        OriginalFilePath = originalFilePath;
        FileName = filePath;
    }

    internal static int GetRecordSize(string filePath)
    {
        return sizeof(ushort) + MegFilePathUtilities.ValidateFilePathCharacterLength(filePath);
    }
}