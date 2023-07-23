// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Binary;
using PG.Commons.Binary.File;
using System;

namespace PG.StarWarsGame.Files.MTD.Binary.File.Type.Definition;

internal sealed class MtdHeader : IBinaryFile, ISizeable
{
    private readonly uint m_recordCount;

    public MtdHeader(uint recordCount = 0)
    {
        m_recordCount = recordCount;
    }

    public byte[] ToBytes()
    {
        return BitConverter.GetBytes(m_recordCount);
    }

    public int Size => sizeof(uint);
}