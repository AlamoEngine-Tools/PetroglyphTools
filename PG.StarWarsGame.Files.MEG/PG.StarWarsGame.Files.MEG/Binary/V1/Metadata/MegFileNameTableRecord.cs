// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for detailsem.Collections;

using System;
using System.Text;
using PG.Commons.Binary;
using PG.Commons.Utilities;

namespace PG.StarWarsGame.Files.MEG.Binary.V1.Metadata;

internal readonly struct MegFileNameTableRecord : IBinary
{
    private readonly Encoding _encoding;
    private readonly ushort _fileNameLength;

    internal string FileName { get; }

    internal string OriginalFileName { get; }

    public byte[] Bytes
    {
        get
        {

#if NETCOREAPP2_1_OR_GREATER
            var bytes = new byte[Size];
            BitConverter.TryWriteBytes(bytes, _fileNameLength);
            var fileNameArea = bytes.AsSpan(sizeof(ushort));
            _encoding.GetBytes(FileName, fileNameArea);
            return bytes;
#else
            var bytes = new byte[Size];
            Buffer.BlockCopy(BitConverter.GetBytes(_fileNameLength), 0, bytes, 0, sizeof(ushort));
            Buffer.BlockCopy(_encoding.GetBytes(FileName), 0, bytes, sizeof(ushort) * 1, _fileNameLength);
            return bytes;
#endif
        }
    }

    public int Size => sizeof(ushort) + _fileNameLength;

    public MegFileNameTableRecord(string fileName, Encoding encoding)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException($"{nameof(fileName)} must not be null or empty");

        if (!encoding.IsSingleByte)
            throw new NotSupportedException(".MEG files are required to use a single-byte encoding for string.");

        _encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        OriginalFileName = fileName;
        _fileNameLength = StringUtilities.ValidateFileNameByteSizeUInt16(fileName, _encoding);
        FileName = StringUtilities.EncodeString(fileName, _fileNameLength, encoding);
    }
}
