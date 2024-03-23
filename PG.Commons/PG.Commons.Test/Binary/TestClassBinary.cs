using PG.Commons.Binary;

namespace PG.Commons.Test.Binary;

public class TestClassBinary(byte[] bytes) : IBinary
{
    public byte[] Bytes { get; } = bytes;
    public int Size => Bytes.Length;
}