using PG.StarWarsGame.Files.Binary;

namespace PG.StarWarsGame.Files.Test.Binary;

public class TestBinary(byte[] bytes) : BinaryBase
{
    protected override int GetSizeCore()
    {
        return bytes.Length;
    }

    protected override byte[] ToBytesCore()
    {
        return bytes;
    }
}