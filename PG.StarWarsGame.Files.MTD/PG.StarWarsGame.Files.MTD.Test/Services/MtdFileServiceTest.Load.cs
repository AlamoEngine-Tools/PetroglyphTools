﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PG.Commons.Binary;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MTD.Files;
using PG.Testing;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MTD.Test.Services;

public partial class MtdFileServiceTest
{
    [Fact]
    public void Test_Load_ArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _mtdFileService.Load(""));
        Assert.Throws<ArgumentNullException>(() => _mtdFileService.Load((string)null!));
        Assert.Throws<ArgumentNullException>(() => _mtdFileService.Load((Stream)null!));
    }


    [Fact]
    public void Test_Load_ThrowFileNotFound()
    {
        Assert.Throws<FileNotFoundException>(() => _mtdFileService.Load("test.mtd"));
    }

    [Theory]
    [MemberData(nameof(MtdTestData.InvalidMtdData), MemberType = typeof(MtdTestData))]
    public void Test_Load_CorruptedFile_Throws(byte[] data)
    {
        _fileSystem.Initialize().WithFile("test.mtd").Which(m => m.HasBytesContent(data));

        Assert.Throws<BinaryCorruptedException>(() => _mtdFileService.Load("test.mtd"));
        Assert.Throws<BinaryCorruptedException>(() => _mtdFileService.Load(new TestMegDataStream("test.mtd", data)));
    }

    [Theory]
    [MemberData(nameof(MtdTestData.ValidMtdData), MemberType = typeof(MtdTestData))]
    public void Test_Load_ValidBinary(byte[] data, IList<MtdEntryInformationContainer> files)
    {
        _fileSystem.Initialize().WithFile("test.mtd").Which(m => m.HasBytesContent(data));

        CompareFileWithExpected(files, _mtdFileService.Load("test.mtd"));
        CompareFileWithExpected(files, _mtdFileService.Load(new TestMegDataStream("test.mtd", data)));
    }

    private void CompareFileWithExpected(IList<MtdEntryInformationContainer> expectedFiles, IMtdFile mtdFile)
    {
        var hashingService = _serviceProvider.GetRequiredService<ICrc32HashingService>();
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