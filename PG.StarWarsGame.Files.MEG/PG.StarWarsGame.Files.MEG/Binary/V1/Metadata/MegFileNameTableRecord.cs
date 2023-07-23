// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for detailsem.Collections;

using System;
using System.Collections.Generic;
using System.Text;
using PG.Commons.Binary;
using PG.Commons.Utilities;

namespace PG.StarWarsGame.Files.MEG.Binary.V1.Metadata;

internal readonly struct MegFileNameTableRecord : IBinary
{
    private readonly Encoding _encoding;
    private readonly ushort _fileNameLength;

    internal string FileName { get; }

    public byte[] Bytes
    {
        get
        {
            var b = new List<byte>();
            b.AddRange(BitConverter.GetBytes(_fileNameLength));
            b.AddRange(_encoding.GetBytes(FileName));
            return b.ToArray();
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
        FileName = fileName;
        _fileNameLength = FileNameUtilities.ValidateFileNameByteSizeUInt16(fileName, _encoding);
    }
}
