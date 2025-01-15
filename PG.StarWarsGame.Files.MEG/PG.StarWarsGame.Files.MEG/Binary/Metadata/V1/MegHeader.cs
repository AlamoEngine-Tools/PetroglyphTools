// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Buffers.Binary;

namespace PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;

internal readonly struct MegHeader : IMegHeader
{
    internal static readonly int SizeValue = sizeof(uint) + sizeof(uint);

    internal uint NumFileNames { get; }

    internal uint NumFiles { get; }

    public int FileNumber => (int)NumFileNames;

    public byte[] Bytes
    {
        get
        {
            var bytes = new byte[Size];
            GetBytes(bytes);
            return bytes;
        }
    }

    public int Size => SizeValue;

    internal MegHeader(uint numFileNames, uint numFiles)
    {
        if (numFileNames != numFiles)
            throw new ArgumentException("Number of files must match number of file names.", nameof(numFiles));
        if (numFileNames > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(numFileNames), ".MEG archives with more files than Int32.MaxValue are not supported.");
        NumFileNames = numFileNames;
        NumFiles = numFiles;
    }

    public void GetBytes(Span<byte> bytes)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(bytes, NumFileNames);
        var fileNumArea = bytes.Slice(sizeof(uint));
        BinaryPrimitives.WriteUInt32LittleEndian(fileNumArea, NumFiles);
    }
}
