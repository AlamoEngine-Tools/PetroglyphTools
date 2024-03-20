// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.Commons.Binary;
using PG.Commons.Binary.File;

namespace PG.StarWarsGame.Files.DAT.Binary.Metadata;

internal sealed class DatBinaryFile : BinaryBase, IBinaryFile
{
    internal DatHeader Header { get; }
    internal IndexTable IndexTable { get; }
    internal ValueTable ValueTable { get; }
    internal KeyTable KeyTable { get; }

    public int RecordNumber => (int)Header.RecordCount;
    
    public DatBinaryFile(DatHeader header, IndexTable indexTable, ValueTable valueTable, KeyTable keyTable)
    {
        Header = header;
        IndexTable = indexTable;
        ValueTable = valueTable;
        KeyTable = keyTable;
    }

    protected override int GetSizeCore()
    {
        return Header.Size + IndexTable.Size + ValueTable.Size + KeyTable.Size;
    }

    protected override byte[] ToBytesCore()
    {
        var b = new List<byte>();
        b.AddRange(Header.Bytes);
        b.AddRange(IndexTable.Bytes);
        b.AddRange(ValueTable.Bytes);
        b.AddRange(KeyTable.Bytes);
        return b.ToArray();
    }
}