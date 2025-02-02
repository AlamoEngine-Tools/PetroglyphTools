using System;
using PG.StarWarsGame.Files.Binary;

namespace PG.StarWarsGame.Files.Test.Binary;

public class TestBinary(byte[] bytes) : BinaryBase
{
    public override void GetBytes(Span<byte> span) => bytes.CopyTo(span);

    protected override int GetSizeCore()
    {
        return bytes.Length;
    }
}