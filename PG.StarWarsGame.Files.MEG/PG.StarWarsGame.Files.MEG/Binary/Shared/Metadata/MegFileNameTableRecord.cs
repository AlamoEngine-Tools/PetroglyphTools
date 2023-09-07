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

    internal string OriginalFileName { get; }

    public byte[] Bytes
    {
        get
        {
            var bytes = new byte[Size];
            BinaryPrimitives.WriteUInt16LittleEndian(bytes, _fileNameLength);

#if NETCOREAPP2_1_OR_GREATER
            var fileNameArea = bytes.AsSpan(sizeof(ushort));
            _encoding.GetBytes(FileName, fileNameArea);
#else
            MegFileConstants.MegContentFileNameEncoding.GetBytes(FileName, 0, FileName.Length, bytes, sizeof(ushort));
#endif
            return bytes;
        }
    }

    public int Size => sizeof(ushort) + _fileNameLength;

    public MegFileNameTableRecord(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException($"{nameof(fileName)} must not be null or empty");

        var encoding = MegFileConstants.MegContentFileNameEncoding;
        OriginalFileName = fileName;

        var charCount = StringUtilities.ValidateStringCharLengthUInt16(fileName);
        _fileNameLength = charCount;

        var byteCount = encoding.GetByteCountPG(charCount);
        FileName = encoding.EncodeString(fileName, byteCount);
    }
    
}
