// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.Commons.Binary;

namespace PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;

/// <summary>
/// Meg archive representation WITHOUT content data.
/// </summary>
internal class MegMetadata : BinaryBase, IMegFileMetadata
{
    public IMegHeader Header { get; }

    public BinaryTable<MegFileNameTableRecord> FileNameTable { get; }

    public MegFileTable FileTable { get; }

    IMegFileTable IMegFileMetadata.FileTable => FileTable;

    public MegMetadata(MegHeader header, BinaryTable<MegFileNameTableRecord> fileNameTable, MegFileTable fileTable)
    {
        if (fileNameTable == null)
            throw new ArgumentNullException(nameof(fileNameTable));
        if (fileTable == null)
            throw new ArgumentNullException(nameof(fileTable));
        if (fileNameTable.Count != fileTable.Count)
            throw new ArgumentException("The FileNameTable and FileTable have do not have the same number of entries.");
        if (fileNameTable.Count != header.NumFileNames)
            throw new ArgumentException("MEG Header and tables do not have the same number of entries.");
        Header = header;
        FileNameTable = fileNameTable;
        FileTable = fileTable;
    }

    protected override int GetSizeCore()
    {
        return Header.Size + FileNameTable.Size + FileTable.Size;
    }

    protected override byte[] ToBytesCore()
    {
        var bytes = new List<byte>(Size);
        bytes.AddRange(Header.Bytes);
        bytes.AddRange(FileNameTable.Bytes);
        bytes.AddRange(FileTable.Bytes);
        return bytes.ToArray();
    }
}
