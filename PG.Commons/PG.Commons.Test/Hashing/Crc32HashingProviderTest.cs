using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Hashing;

namespace PG.Commons.Test.Hashing;

[TestClass]
public class Crc32HashingProviderTest
{
    [TestMethod]
    public void Test_GetChecksum_Throws()
    {
        var checksumService = new Crc32HashingProvider();
        Assert.ThrowsException<ArgumentNullException>(() => checksumService.HashData((Stream) null!, new Span<byte>(new byte[4])));
    }


    [TestMethod]
    [DataRow(new byte[] { }, 0u)]
    [DataRow(new byte[] { 0, 1, 2, 3 }, 2344191507u)]
    [DataRow(new byte[] { 1, 2, 3 }, 1438416925u)]
    [DataRow(new byte[] { 1, 2, 3, 4 }, 3057449933u)]
    public unsafe void Test_HashData(byte[] inputData, uint expected)
    {
        Span<byte> destination = stackalloc byte[20];
        var bytesWritten = new Crc32HashingProvider().HashData(inputData, destination);

        Assert.AreEqual(sizeof(Crc32), bytesWritten);
        var expectedBytes = BitConverter.GetBytes(expected);
        CollectionAssert.AreEqual(expectedBytes, destination.Slice(0, bytesWritten).ToArray());


        bytesWritten = new Crc32HashingProvider().HashData(new MemoryStream(inputData), destination);
        Assert.AreEqual(sizeof(Crc32), bytesWritten);
        expectedBytes = BitConverter.GetBytes(expected);
        CollectionAssert.AreEqual(expectedBytes, destination.Slice(0, bytesWritten).ToArray());
    }

    [TestMethod]
    [DataRow(new byte[] { }, 0u)]
    [DataRow(new byte[] { 0, 1, 2, 3 }, 2344191507u)]
    [DataRow(new byte[] { 1, 2, 3 }, 1438416925u)]
    [DataRow(new byte[] { 1, 2, 3, 4 }, 3057449933u)]
    public async Task Test_HashDataAsync(byte[] inputData, uint expected)
    {
        var destination = new byte[20];
        var bytesWritten = await new Crc32HashingProvider().HashDataAsync(new MemoryStream(inputData), destination);

        Assert.AreEqual(4, bytesWritten);
        var expectedBytes = BitConverter.GetBytes(expected);
        CollectionAssert.AreEqual(expectedBytes, destination.AsSpan().Slice(0, bytesWritten).ToArray());
    }
}