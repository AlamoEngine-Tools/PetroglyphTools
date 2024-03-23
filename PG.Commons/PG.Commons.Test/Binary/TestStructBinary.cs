using PG.Commons.Binary;

namespace PG.Commons.Test.Binary;

public readonly struct TestStructBinary(byte[] bytes) : IBinary
{
    public byte[] Bytes { get; } = bytes;
    public int Size => Bytes.Length;
}