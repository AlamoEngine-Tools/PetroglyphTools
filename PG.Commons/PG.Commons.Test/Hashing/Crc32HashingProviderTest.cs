using System;
using System.IO;
using System.Threading.Tasks;
using PG.Commons.Hashing;
using Xunit;

namespace PG.Commons.Test.Hashing;

public class Crc32HashingProviderTest
{
    [Fact]
    public void HashData_NullArgs_Throws()
    {
        var checksumService = new Crc32HashingProvider();
        Assert.Throws<ArgumentNullException>(() => checksumService.HashData((Stream) null!, new Span<byte>(new byte[4])));
    }

    [Theory]
    [InlineData(new byte[] { }, 0u)]
    [InlineData(new byte[] { 0, 1, 2, 3 }, 2344191507u)]
    [InlineData(new byte[] { 1, 2, 3 }, 1438416925u)]
    [InlineData(new byte[] { 1, 2, 3, 4 }, 3057449933u)]
    public unsafe void HashData(byte[] inputData, uint expected)
    {
        Span<byte> destination = stackalloc byte[20];
        var bytesWritten = new Crc32HashingProvider().HashData(inputData, destination);

        Assert.Equal(sizeof(Crc32), bytesWritten);
        var expectedBytes = BitConverter.GetBytes(expected);
        Assert.Equal(expectedBytes, destination.Slice(0, bytesWritten).ToArray());


        bytesWritten = new Crc32HashingProvider().HashData(new MemoryStream(inputData), destination);
        Assert.Equal(sizeof(Crc32), bytesWritten);
        expectedBytes = BitConverter.GetBytes(expected);
        Assert.Equal(expectedBytes, destination.Slice(0, bytesWritten).ToArray());
    }

    [Theory]
    [InlineData(new byte[] { }, 0u)]
    [InlineData(new byte[] { 0, 1, 2, 3 }, 2344191507u)]
    [InlineData(new byte[] { 1, 2, 3 }, 1438416925u)]
    [InlineData(new byte[] { 1, 2, 3, 4 }, 3057449933u)]
    public async Task HashDataAsync(byte[] inputData, uint expected)
    {
        var destination = new byte[20];
        var bytesWritten = await new Crc32HashingProvider().HashDataAsync(new MemoryStream(inputData), destination);

        Assert.Equal(4, bytesWritten);
        var expectedBytes = BitConverter.GetBytes(expected);
        Assert.Equal(expectedBytes, destination.AsSpan().Slice(0, bytesWritten).ToArray());
    }
}