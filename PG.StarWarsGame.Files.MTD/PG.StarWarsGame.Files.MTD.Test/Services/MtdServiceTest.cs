using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AnakinRaW.CommonUtilities.Testing;
using AnakinRaW.CommonUtilities.Testing.Extensions;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MTD.Data;
using PG.StarWarsGame.Files.MTD.Files;
using PG.StarWarsGame.Files.MTD.Services;
using PG.Testing;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MTD.Test.Services;

public class MtdServiceTest : CommonMtdTestBase
{
    private readonly IMtdService _mtdService;

    public MtdServiceTest()
    {
        _mtdService = ServiceProvider.GetRequiredService<IMtdService>();
    }

    [Fact]
    public void LoadFile_ArgumentException_Throws()
    {
        Assert.Throws<ArgumentException>(() => _mtdService.LoadFile(""));
        Assert.Throws<ArgumentNullException>(() => _mtdService.LoadFile((string)null!));
        Assert.Throws<ArgumentNullException>(() => _mtdService.LoadFile((Stream)null!));
        Assert.Throws<ArgumentNullException>(() => _mtdService.LoadModel((Stream)null!));
        Assert.Throws<ArgumentNullException>(() => _mtdService.LoadModel((byte[])null!));
    }


    [Fact]
    public void LoadFile_FileNotFound_Throws()
    {
        Assert.Throws<FileNotFoundException>(() => _mtdService.LoadFile("test.mtd"));
    }

    [Theory]
    [MemberData(nameof(MtdTestData.InvalidMtdData), MemberType = typeof(MtdTestData))]
    public void LoadFile_CorruptedFile_Throws(byte[] data)
    {
        FileSystem.Initialize().WithFile("test.mtd").Which(m => m.HasBytesContent(data));

        Assert.Throws<BinaryCorruptedException>(() => _mtdService.LoadFile("test.mtd"));
        Assert.Throws<BinaryCorruptedException>(() => _mtdService.LoadFile(new TestMegDataStream("test.mtd", data)));
        Assert.Throws<BinaryCorruptedException>(() => _mtdService.LoadModel(new MemoryStream(data)));
        Assert.Throws<BinaryCorruptedException>(() => _mtdService.LoadModel(data));
        Assert.Throws<BinaryCorruptedException>(() => _mtdService.LoadModel(data.AsSpan()));
    }

    [Theory]
    [MemberData(nameof(MtdTestData.ValidMtdData), MemberType = typeof(MtdTestData))]
    public void LoadFile_ValidBinary(byte[] data, IList<MtdEntryInformationContainer> files)
    {
        FileSystem.Initialize().WithFile("test.mtd").Which(m => m.HasBytesContent(data));

        CompareFileWithExpected(files, _mtdService.LoadFile("test.mtd"));
        CompareFileWithExpected(files, _mtdService.LoadFile(new TestMegDataStream("test.mtd", data)));

        // The same data parses identically through the in-memory model overloads.
        CompareDirectoryWithExpected(files, _mtdService.LoadModel(new MemoryStream(data)));
        CompareDirectoryWithExpected(files, _mtdService.LoadModel(data));
        CompareDirectoryWithExpected(files, _mtdService.LoadModel(data.AsSpan()));
    }

    [Theory]
    [MemberData(nameof(MtdTestData.ValidMtdData), MemberType = typeof(MtdTestData))]
    public void LoadFile_StreamStaysOpen(byte[] data, IList<MtdEntryInformationContainer> files)
    {
        FileSystem.Initialize().WithFile("test.mtd").Which(m => m.HasBytesContent(data));

        using var fs = FileSystem.File.OpenRead("test.mtd");

        CompareFileWithExpected(files, _mtdService.LoadFile(fs));

        // Resetting the position should not throw
        fs.Position = 0;
    }

    [Fact]
    public void LoadFile_FocMtd()
    {
        var focFile = TestingHelpers.GetEmbeddedResource(GetType(), "Files.MT_COMMANDBAR.MTD");
        Assert.DoesNotThrow(() => _mtdService.LoadFile(new TestMegDataStream("MT_COMMANDBAR.MTD", focFile)));
    }

    private void CompareFileWithExpected(IList<MtdEntryInformationContainer> expectedFiles, IMtdFile mtdFile)
    {
        CompareDirectoryWithExpected(expectedFiles, mtdFile.Content);
    }

    private void CompareDirectoryWithExpected(IList<MtdEntryInformationContainer> expectedFiles, IMegaTextureDirectory directory)
    {
        var hashingService = ServiceProvider.GetRequiredService<ICrc32HashingService>();
        for (var i = 0; i < directory.Count; i++)
        {
            var expected = expectedFiles[i];
            var crc = hashingService.GetCrc32(expected.ExpectedName, Encoding.ASCII);

            Assert.True(directory.Contains(crc));
            Assert.True(directory.TryGetEntry(crc, out var actual));
            expected.AsserEquals(actual);
        }
    }

    [Fact]
    public void MTD_FileWithCollision()
    {
        var testStream = new TestMegDataStream("MT_COMMANDBAR.MTD", MtdTestData.MtdWithKnownCollision());
        var fileWithCollision = _mtdService.LoadFile(testStream);

        var expectedCrc = new Crc32(3596410486);

        Assert.Equal(2, fileWithCollision.Content.Count);
        Assert.True(fileWithCollision.Content.Contains(expectedCrc));
        Assert.Equal(2, fileWithCollision.Content.EntriesWithCrc(expectedCrc).Count);
    }
}