// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.Binary.File;

namespace PG.StarWarsGame.Files.DAT.Binary.Metadata;

internal sealed class DatBinaryFile(
    DatHeader header,
    BinaryTable<IndexTableRecord> indexTable,
    BinaryTable<ValueTableRecord> valueTable,
    BinaryTable<KeyTableRecord> keyTable)
    : BinaryBase, IBinaryFile
{
    internal DatHeader Header { get; } = header;
    internal BinaryTable<IndexTableRecord> IndexTable { get; } = indexTable ?? throw new ArgumentNullException(nameof(indexTable));
    internal BinaryTable<ValueTableRecord> ValueTable { get; } = valueTable ?? throw new ArgumentNullException(nameof(valueTable));
    internal BinaryTable<KeyTableRecord> KeyTable { get; } = keyTable ?? throw new ArgumentNullException(nameof(keyTable));

    public int RecordNumber => (int)Header.RecordCount;

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