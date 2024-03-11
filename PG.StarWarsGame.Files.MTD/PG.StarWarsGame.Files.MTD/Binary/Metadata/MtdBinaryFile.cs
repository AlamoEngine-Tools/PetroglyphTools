using System.Collections;
using System.Collections.Generic;
using PG.Commons.Binary;

namespace PG.StarWarsGame.Files.MTD.Binary.Metadata;

internal class MtdBinaryFile : BinaryBase, IBinaryTable<BinaryImageTableRecord>
{
    public int Count { get; }

    public BinaryImageTableRecord this[int index] => throw new System.NotImplementedException();

    protected override int GetSizeCore()
    {
        throw new System.NotImplementedException();
    }

    protected override byte[] ToBytesCore()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator<BinaryImageTableRecord> GetEnumerator()
    {
        throw new System.NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}