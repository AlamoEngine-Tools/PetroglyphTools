using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using PG.Commons.Hashing;

namespace PG.StarWarsGame.Files.MTD.Data;

public interface IMegaTextureDirectory : IReadOnlyCollection<MegaTextureFileIndex>
{
    bool Contains(Crc32 hashedName);

    bool TryGetIndex(Crc32 hashedName, [MaybeNullWhen(false)] out MegaTextureFileIndex index);
}

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

    public bool Contains(Crc32 hashedName)
    {
        return _filesIndices.ContainsKey(hashedName);
    }

    public bool TryGetIndex(Crc32 hashedName, out MegaTextureFileIndex index)
    {
        return _filesIndices.TryGetValue(hashedName, out index);
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

public sealed class DuplicateFileIndexException(string message) : Exception(message);