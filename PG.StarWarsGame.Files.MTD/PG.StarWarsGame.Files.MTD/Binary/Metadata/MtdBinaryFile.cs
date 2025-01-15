// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.Binary;

namespace PG.StarWarsGame.Files.MTD.Binary.Metadata;

internal class MtdBinaryFile : BinaryFile
{
    public MtdBinaryFile(MtdHeader header, IBinaryTable<MtdBinaryFileInfo> items)
    {
        if (items is null)
            throw new ArgumentNullException(nameof(items));
        if (items.Count != header.Count)
            throw new ArgumentException("Header index count and item count to not match.");

        Header = header;
        Items = items;
    }

    public MtdHeader Header { get; }

    public IBinaryTable<MtdBinaryFileInfo> Items { get; }

    public override void GetBytes(Span<byte> bytes)
    {
        Header.GetBytes(bytes);
        var itemsBytes = bytes.Slice(Header.Size);
        Items.GetBytes(itemsBytes);
    }

    protected override int GetSizeCore()
    {
        return Header.Size + Items.Size;
    }
}