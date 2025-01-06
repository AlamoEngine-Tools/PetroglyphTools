using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MTD.Files;
using PG.StarWarsGame.Files.MTD.Services;
using PG.Testing;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MTD.Test.Services;

public class MtdFileServiceTest : CommonMtdTestBase
{
    private readonly MtdFileService _mtdFileService;

    public MtdFileServiceTest()
    {
        _mtdFileService = new MtdFileService(ServiceProvider);
    }

    [Fact]
    public void Load_ArgumentException_Throws()
    {
        Assert.Throws<ArgumentException>(() => _mtdFileService.Load(""));
        Assert.Throws<ArgumentNullException>(() => _mtdFileService.Load((string)null!));
        Assert.Throws<ArgumentNullException>(() => _mtdFileService.Load((Stream)null!));
    }


    [Fact]
    public void Load_FileNotFound_Throws()
    {
        Assert.Throws<FileNotFoundException>(() => _mtdFileService.Load("test.mtd"));
    }

    [Theory]
    [MemberData(nameof(MtdTestData.InvalidMtdData), MemberType = typeof(MtdTestData))]
    public void Load_CorruptedFile_Throws(byte[] data)
    {
        FileSystem.Initialize().WithFile("test.mtd").Which(m => m.HasBytesContent(data));

        Assert.Throws<BinaryCorruptedException>(() => _mtdFileService.Load("test.mtd"));
        Assert.Throws<BinaryCorruptedException>(() => _mtdFileService.Load(new TestMegDataStream("test.mtd", data)));
    }

    [Theory]
    [MemberData(nameof(MtdTestData.ValidMtdData), MemberType = typeof(MtdTestData))]
    public void Load_ValidBinary(byte[] data, IList<MtdEntryInformationContainer> files)
    {
        FileSystem.Initialize().WithFile("test.mtd").Which(m => m.HasBytesContent(data));

        CompareFileWithExpected(files, _mtdFileService.Load("test.mtd"));
        CompareFileWithExpected(files, _mtdFileService.Load(new TestMegDataStream("test.mtd", data)));
    }

    [Fact]
    public void Load_FocMtd()
    {
        var focFile = TestUtility.GetEmbeddedResource(GetType(), "Files.MT_COMMANDBAR.MTD");
        ExceptionUtilities.AssertDoesNotThrowException(() => _mtdFileService.Load(new TestMegDataStream("MT_COMMANDBAR.MTD", focFile)));
    }

    private void CompareFileWithExpected(IList<MtdEntryInformationContainer> expectedFiles, IMtdFile mtdFile)
    {
        var hashingService = ServiceProvider.GetRequiredService<ICrc32HashingService>();
        for (var i = 0; i < mtdFile.Content.Count; i++)
        {
            var expected = expectedFiles[i];
            var crc = hashingService.GetCrc32(expected.ExpectedName, Encoding.ASCII);

            Assert.True(mtdFile.Content.Contains(crc));
            Assert.True(mtdFile.Content.TryGetEntry(crc, out var actual));
            expected.AsserEquals(actual);
        }
    }
}