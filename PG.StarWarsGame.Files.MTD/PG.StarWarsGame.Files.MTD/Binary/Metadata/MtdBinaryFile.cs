// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.Binary.File;

namespace PG.StarWarsGame.Files.MTD.Binary.Metadata;

internal class MtdBinaryFile : BinaryBase, IBinaryFile
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

    protected override int GetSizeCore()
    {
        return Header.Size + Items.Size;
    }

    protected override byte[] ToBytesCore()
    {
        var b = new List<byte>();
        b.AddRange(Header.Bytes);
        b.AddRange(Items.Bytes);
        return b.ToArray();
    }
}