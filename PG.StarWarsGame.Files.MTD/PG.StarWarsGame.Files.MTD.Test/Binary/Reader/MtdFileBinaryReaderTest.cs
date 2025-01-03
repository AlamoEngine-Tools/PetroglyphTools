// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MTD.Binary;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MTD.Test.Binary.Reader;


public class MtdFileBinaryReaderTest
{
    private readonly MdtFileReader _binaryReader;

    public MtdFileBinaryReaderTest()
    {
        var fs = new MockFileSystem();
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(fs);
        PetroglyphCommons.ContributeServices(sc);
        sc.SupportMTD();
        _binaryReader = new MdtFileReader(sc.BuildServiceProvider());
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