// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections;
using System.Collections.Generic;
using PG.Commons.Hashing;

namespace PG.StarWarsGame.Files.MTD.Data;

internal class MegaTextureDirectory : IMegaTextureDirectory
{
    private readonly Dictionary<Crc32, MegaTextureFileIndex> _filesIndices;

    public int Count => _filesIndices.Count;

    public MegaTextureDirectory(IEnumerable<MegaTextureFileIndex> indices)
    {
        _filesIndices = new Dictionary<Crc32, MegaTextureFileIndex>();

        foreach (var fileIndex in indices)
        {
            if (_filesIndices.ContainsKey(fileIndex.Crc32))
                throw new DuplicateFileIndexException("MTD files must not have entries with the same name CRC32 value.");
            _filesIndices[fileIndex.Crc32] = fileIndex;
        }
    }

    public bool Contains(Crc32 crc32)
    {
        return _filesIndices.ContainsKey(crc32);
    }

    public bool TryGetEntry(Crc32 crc32, out MegaTextureFileIndex entry)
    {
        return _filesIndices.TryGetValue(crc32, out entry);
    }

    public IEnumerator<MegaTextureFileIndex> GetEnumerator()
    {
        return _filesIndices.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}