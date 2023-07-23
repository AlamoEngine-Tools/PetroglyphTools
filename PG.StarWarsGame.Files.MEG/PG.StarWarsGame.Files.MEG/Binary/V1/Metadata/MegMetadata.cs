// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.Commons.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Shared.Metadata;

namespace PG.StarWarsGame.Files.MEG.Binary.V1.Metadata;

/// <summary>
/// Meg archive representation WITHOUT content data.
/// </summary>
internal class MegMetadata : BinaryBase, IMegFileMetadata
{
    public int FileNumber => (int) Header.NumFiles;

    internal MegHeader Header { get; }

    internal MegFileNameTable FileNameTable { get; }

    public MegFileTable FileTable { get; }

    IFileNameTable IMegFileMetadata.FileNameTable => FileNameTable;

    IFileTable IMegFileMetadata.FileTable => FileTable;

    public MegMetadata(MegHeader header, MegFileNameTable fileNameTable, MegFileTable fileContentTable)
    {
        Header = header;
        FileNameTable = fileNameTable ?? throw new ArgumentNullException(nameof(fileNameTable));
        FileTable = fileContentTable ?? throw new ArgumentNullException(nameof(fileContentTable));
    }

    protected override int GetSizeCore()
    {
        return Header.Size + FileTable.Size + FileTable.Size;
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
