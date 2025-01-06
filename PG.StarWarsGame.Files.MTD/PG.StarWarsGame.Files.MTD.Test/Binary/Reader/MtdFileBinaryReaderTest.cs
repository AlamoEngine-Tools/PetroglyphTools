using System;
using System.Collections.Generic;
using System.IO;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MTD.Binary;
using Xunit;

namespace PG.StarWarsGame.Files.MTD.Test.Binary.Reader;

public class MtdFileBinaryReaderTest : CommonMtdTestBase
{
    private readonly MdtFileReader _binaryReader;

    public MtdFileBinaryReaderTest()
    {
        _binaryReader = new MdtFileReader(ServiceProvider);
    }

    [Fact]
    public void Test__BuildMegHeader_NullStream_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _binaryReader.ReadBinary(null!));
    }

    [Theory]
    [MemberData(nameof(MtdTestData.InvalidMtdData), MemberType = typeof(MtdTestData))]
    public void Test__BuildMegHeader_BinaryCorrupted(byte[] data)
    {
        var dataStream = new MemoryStream(data);
        Assert.Throws<BinaryCorruptedException>(() => _binaryReader.ReadBinary(dataStream));
    }

    [Theory]
    [MemberData(nameof(MtdTestData.ValidMtdData), MemberType = typeof(MtdTestData))]
    public void Test__BuildMegHeader_Correct(byte[] data, IList<MtdEntryInformationContainer> files)
    {
        var dataStream = new MemoryStream(data);
        var mtd = _binaryReader.ReadBinary(dataStream);

        Assert.Equal(files.Count, (int)mtd.Header.Count);
        Assert.Equal(files.Count, mtd.Items.Count);

        for (var i = 0; i < files.Count; i++)
        {
            var expected = files[i];
            var actual = mtd.Items[i];
            expected.AsserEquals(actual);
        }
    }
}