// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Binary;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.V1;
using PG.StarWarsGame.Files.MEG.Binary.V1.Metadata;

namespace PG.StarWarsGame.Files.MEG.Test.Binary;

[TestClass]
public class MegFileBinaryServiceV1Test
{
    private MegFileBinaryServiceV1 _binaryService = null!;

    [TestInitialize]
    public void SetUp()
    {
        var fs = new MockFileSystem();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);
        _binaryService = new MegFileBinaryServiceV1(sp.Object);
    }

    [TestMethod]
    public void Test__CreateMegMetadata()
    {
        var header = (MegHeader)default;
        var nameTable = new Mock<MegFileNameTable>(Array.Empty<MegFileNameTableRecord>());
        var fileTable = new Mock<MegFileTable>(Array.Empty<MegFileContentTableRecord>());

        var metadata = _binaryService.CreateMegMetadata(header, nameTable.Object, fileTable.Object);

        Assert.AreEqual(header, metadata.Header);
        Assert.AreEqual(nameTable.Object, metadata.FileNameTable);
        Assert.AreEqual(fileTable.Object, metadata.FileTable);
    }

    [TestMethod]
    public void Test__BuildMegHeader_NotSupportedFileCount()
    {
        var data = new byte[]
        {
            0, 0, 0, 0x80,
            0, 0, 0, 0x80
        };

        var reader = new BinaryReader(new MemoryStream(data));
        Assert.ThrowsException<NotSupportedException>(() => _binaryService.BuildMegHeader(reader));
    }

    [TestMethod]
    public void Test__BuildMegHeader_NotEqualFile_FileName_Count()
    {
        var data = new byte[]
        {
            1, 0, 0, 0,
            2, 0, 0, 0
        };

        var reader = new BinaryReader(new MemoryStream(data));
        Assert.ThrowsException<BinaryCorruptedException>(() => _binaryService.BuildMegHeader(reader));
    }

    [DataTestMethod]
    [DynamicData(nameof(HeaderTestData), DynamicDataSourceType.Method)]
    public void Test__BuildMegHeader_Correct(byte[] data, uint numFiles, uint numNames)
    {
        var reader = new BinaryReader(new MemoryStream(data));
        var header = _binaryService.BuildMegHeader(reader);

        Assert.AreEqual(numFiles, header.NumFiles);
        Assert.AreEqual(numNames, header.NumFileNames);
    }

    private static IEnumerable<object[]> HeaderTestData()
    {
        return new[]
        {
            new object[] { new byte[]
            { 
                0,0,0,0,
                0,0,0,0
            }, 0u, 0u },
            new object[] { new byte[]
            {
                0,0,0,0,
                0,0,0,0,
                1,2,3,4, // Random Junk 
            }, 0u, 0u },
            new object[] { new byte[]
            {
                1,0,0,0,
                1,0,0,0
            }, 1u, 1u },
            new object[] { new byte[]
            {
                0xFF,0xFF,0xFF,0x7F,
                0xFF,0xFF,0xFF,0x7F
            }, (uint)int.MaxValue, (uint)int.MaxValue },
        };
    }

    [DataTestMethod]
    [DynamicData(nameof(FileTableIntegrationTestData), DynamicDataSourceType.Method)]
    public void Test__BuildFileTable_Integration(
        uint numberEntries, 
        byte[] data, 
        int dataToInspect, 
        Crc32 crc32,
        uint fIndex,
        uint fSize,
        uint fOffset,
        uint nIndex)
    {
        var header = new MegHeader(numberEntries, numberEntries);

        var reader = new BinaryReader(new MemoryStream(data));
        var fileTable = _binaryService.BuildFileTable(reader, header);

        Assert.AreEqual((int)numberEntries, fileTable.Count);

        if (fileTable.Count == 0)
            return;

        var record = fileTable[dataToInspect];

        Assert.AreEqual(crc32, record.Crc32);
        Assert.AreEqual(fIndex, record.FileTableRecordIndex);
        Assert.AreEqual(fSize, record.FileSize);
        Assert.AreEqual(fOffset, record.FileOffset);
        Assert.AreEqual(nIndex, record.FileNameIndex);
    }

    private static IEnumerable<object[]> FileTableIntegrationTestData()
    {
        return new[]
        {
            new object[] { 0u, Array.Empty<byte>(), -1, new Crc32(), 0u,0u,0u,0u},
            new object[] { 1u, new byte[]
            {
                1,0,0,0, // CRC
                0,0,0,0, // Index (FileTable)
                2,0,0,0, // Size
                0x40,0,0,0, // Offset
                0,0,0,0  // Index (NameTable)
            }, 0, new Crc32(1), 0u, 2u, 0x40u, 0u},
            new object[] { 1u, new byte[]
            {
                1,0,0,0, // CRC
                0,0,0,0, // Index (FileTable)
                2,0,0,0, // Size
                0x40,0,0,0, // Offset
                0,0,0,0,  // Index (NameTable)

                1,2,3,4 // Junk
            },  0, new Crc32(1), 0u, 2u, 0x40u, 0u},
            new object[] { 2u, new byte[]
            {
                1,0,0,0, // CRC
                0,0,0,0, // Index (FileTable)
                2,0,0,0, // Size
                0x40,0,0,0, // Offset
                0,0,0,0,  // Index (NameTable)

                2,0,0,0, // CRC
                1,0,0,0, // Index (FileTable)
                3,0,0,0, // Size
                0x80,0,0,0, // Offset
                1,0,0,0  // Index (NameTable)
            }, 1, new Crc32(2), 1u, 3u, 0x80u, 1u}
        };
    }


    [DataTestMethod]
    [DynamicData(nameof(NotSupportedFileTableRecords), DynamicDataSourceType.Method)]
    public void Test__BuildFileTable_NotSupported(uint numberEntries, byte[] data)
    {
        var header = new MegHeader(numberEntries, numberEntries);
        var reader = new BinaryReader(new MemoryStream(data));
        Assert.ThrowsException<NotSupportedException>(() => _binaryService.BuildFileTable(reader, header));
    }

    private static IEnumerable<object[]> NotSupportedFileTableRecords()
    {
        return new[]
        {
            new object[]
            {
                1u, new byte[]
                {
                    1, 0, 0, 0, // CRC
                    0, 0, 0, 0x80, // Index (FileTable)
                    0, 0, 0, 0, // Size
                    0, 0, 0, 0, // Offset
                    0, 0, 0, 0 // Index (NameTable)
                }
            },
            new object[]
            {
                1u, new byte[]
                {
                    1, 0, 0, 0, // CRC
                    0, 0, 0, 0, // Index (FileTable)
                    2, 0, 0, 0, // Size
                    0, 0, 0, 0, // Offset
                    0, 0, 0, 0x80 // Index (NameTable)
                }
            },
        };
    }
}