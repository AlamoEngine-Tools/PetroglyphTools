using System;
using System.Linq;
using PG.StarWarsGame.Files.Binary;

namespace PG.StarWarsGame.Files.Test.Binary;

public class TestBinaryFile(params IBinary[] binaries) : BinaryFile
{
    public override void GetBytes(Span<byte> bytes)
    {
        var offset = 0;
        foreach (var item in binaries)
        {
            item.GetBytes(bytes.Slice(offset));
            offset += item.Size;
        }
    }

    protected override int GetSizeCore() => binaries.Sum(x => x.Size);
}