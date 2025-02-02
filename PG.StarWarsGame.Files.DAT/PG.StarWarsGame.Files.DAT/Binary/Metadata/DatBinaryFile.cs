// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.Binary;

namespace PG.StarWarsGame.Files.DAT.Binary.Metadata;

internal sealed class DatBinaryFile(
    DatHeader header,
    BinaryTable<IndexTableRecord> indexTable,
    BinaryTable<ValueTableRecord> valueTable,
    BinaryTable<KeyTableRecord> keyTable)
    : BinaryFile
{
    internal DatHeader Header { get; } = header;
    internal BinaryTable<IndexTableRecord> IndexTable { get; } = indexTable ?? throw new ArgumentNullException(nameof(indexTable));
    internal BinaryTable<ValueTableRecord> ValueTable { get; } = valueTable ?? throw new ArgumentNullException(nameof(valueTable));
    internal BinaryTable<KeyTableRecord> KeyTable { get; } = keyTable ?? throw new ArgumentNullException(nameof(keyTable));

    public int RecordNumber => (int)Header.RecordCount;

    public override void GetBytes(Span<byte> bytes)
    {
        Header.GetBytes(bytes);
        IndexTable.GetBytes(bytes.Slice(Header.Size));
        ValueTable.GetBytes(bytes.Slice(Header.Size + IndexTable.Size));
        KeyTable.GetBytes(bytes.Slice(Header.Size + IndexTable.Size + ValueTable.Size));
    }

    protected override int GetSizeCore()
    {
        return Header.Size + IndexTable.Size + ValueTable.Size + KeyTable.Size;
    }
}