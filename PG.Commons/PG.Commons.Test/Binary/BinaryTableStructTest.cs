using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PG.Commons.Test.Binary;

[TestClass]
public class BinaryTableStructTest : BinaryTableTest<TestStructBinary>
{
    protected override TestStructBinary CreateFile(uint index, uint seed)
    {
        var random = new Random();
        var size = (seed + 1) * 2;
        var bytes = new byte[size];
        random.NextBytes(bytes);
        return new TestStructBinary(bytes);
    }
}