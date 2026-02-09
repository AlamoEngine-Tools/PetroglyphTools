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

    // While this technically is a path, not just a file name,
    // we keep the original wording of the spec for the binary data types.
    internal string FileName { get; }

    internal string OriginalFileName { get; }

    public int Size => sizeof(ushort) + _fileNameLength;

    public byte[] Bytes
    {
        get
        {
            var bytes = new byte[Size];
            GetBytes(bytes);
            return bytes;
        }
    }
    
    public void GetBytes(Span<byte> bytes)
    {
        BinaryPrimitives.WriteUInt16LittleEndian(bytes, _fileNameLength);
        MegFileConstants.MegDataEntryPathEncoding.GetBytes(FileName.AsSpan(), bytes.Slice(sizeof(ushort)));
    }

    public MegFileNameTableRecord(string fileName, string originalFileName)
    {
        ThrowHelper.ThrowIfNullOrEmpty(originalFileName);
        ThrowHelper.ThrowIfNullOrWhiteSpace(fileName);
        StringUtilities.ValidateIsAsciiOnly(fileName.AsSpan());
        _fileNameLength = MegPathUtilities.ValidateEntryFileNameLength(fileName);

        OriginalFileName = originalFileName;
        FileName = fileName;
    }

    internal static int GetRecordSize(string fileName)
    {
        return sizeof(ushort) + MegPathUtilities.ValidateEntryFileNameLength(fileName);
    }
}