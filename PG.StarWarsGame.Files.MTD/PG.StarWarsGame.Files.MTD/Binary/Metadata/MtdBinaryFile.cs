// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.Commons.Binary;
using PG.Commons.Binary.File;

namespace PG.StarWarsGame.Files.MTD.Binary.Metadata;

internal class MtdBinaryFile(MtdHeader header, IBinaryTable<MtdBinaryFileInfo> items) : BinaryBase, IBinaryFile
{
    public MtdHeader Header { get; } = header;
    public IBinaryTable<MtdBinaryFileInfo> Items { get; } = items ?? throw new ArgumentNullException(nameof(items));

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