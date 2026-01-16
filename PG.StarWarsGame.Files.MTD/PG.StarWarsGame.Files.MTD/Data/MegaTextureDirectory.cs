// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections;
using System.Collections.Generic;
using AnakinRaW.CommonUtilities.Collections;
using PG.Commons.Hashing;

namespace PG.StarWarsGame.Files.MTD.Data;

internal class MegaTextureDirectory : IMegaTextureDirectory
{
    private readonly FrugalValueListDictionary<Crc32, MegaTextureFileIndex> _filesIndices;

    public int Count => _filesIndices.ValueCount;

    public MegaTextureDirectory(IEnumerable<MegaTextureFileIndex> indices)
    {
        _filesIndices = new FrugalValueListDictionary<Crc32, MegaTextureFileIndex>();
        foreach (var fileIndex in indices) 
            _filesIndices.Add(fileIndex.Crc32, fileIndex);
    }

    public bool Contains(Crc32 crc32)
    {
        return _filesIndices.ContainsKey(crc32);
    }

    public MegaTextureFileIndex LastEntryWithCrc(Crc32 crc)
    {
        return _filesIndices.GetLastValue(crc);
    }

    public bool TryGetEntry(Crc32 crc32, out MegaTextureFileIndex entry)
    {
        return _filesIndices.TryGetLastValue(crc32, out entry!);
    }

    public ImmutableFrugalList<MegaTextureFileIndex> EntriesWithCrc(Crc32 crc)
    {
        _filesIndices.TryGetValues(crc, out var list);
        return list;
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