using System;
using System.Collections.Generic;
using System.IO;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using PG.StarWarsGame.Files.MEG.Binary.V1;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Reader.V1;

public class MegFileBinaryReaderV1Test : MegFileBinaryReaderBaseTest
{
    private MegFileBinaryReaderV1 CreateV1Reader()
    {
        return new MegFileBinaryReaderV1(ServiceProvider);
    }

    private protected override IMegFileBinaryReader CreateService()
    {
        return CreateV1Reader();
    }

    [Fact]
    public void Test__CreateMegMetadata()
    {
        var header = (MegHeader)default;

        var nameTable = new BinaryTable<MegFileNameTableRecord>([]);
        var fileTable = new MegFileTable([]);

        var metadata = CreateV1Reader().CreateMegMetadata(header, nameTable, fileTable);

        Assert.Equal(header, metadata.Header);
        Assert.Equal(nameTable, metadata.FileNameTable);
        Assert.Equal(fileTable, metadata.FileTable);
    }

    [Fact]
    public void Test__BuildMegHeader_NotSupportedFileCount()
    {
        var data = new byte[]
        {
            0, 0, 0, 0x80,
            0, 0, 0, 0x80
        };

        var reader = new BinaryReader(new MemoryStream(data));
        Assert.Throws<NotSupportedException>(() => CreateV1Reader().BuildMegHeader(reader));
    }

    [Fact]
    public void Test__BuildMegHeader_NotEqualFile_FileName_Count()
    {
        var data = new byte[]
        {
            1, 0, 0, 0,
            2, 0, 0, 0
        };

        var reader = new BinaryReader(new MemoryStream(data));
        Assert.Throws<BinaryCorruptedException>(() => CreateV1Reader().BuildMegHeader(reader));
    }

    [Theory]
    [MemberData(nameof(HeaderTestData))]
    public void Test__BuildMegHeader_Correct(byte[] data, uint numFiles, uint numNames)
    {
        var reader = new BinaryReader(new MemoryStream(data));
        var header = CreateV1Reader().BuildMegHeader(reader);

        Assert.Equal(numFiles, header.NumFiles);
        Assert.Equal(numNames, header.NumFileNames);
    }

    public static IEnumerable<object[]> HeaderTestData()
    {
        return
        [
            [
                new byte[]
                {
                    0, 0, 0, 0,
                    0, 0, 0, 0
                },
                0u, 0u
            ],
            [
                new byte[]
                {
                    0, 0, 0, 0,
                    0, 0, 0, 0,
                    1, 2, 3, 4, // Random Junk 
                },
                0u, 0u
            ],
            [
                new byte[]
                {
                    1, 0, 0, 0,
                    1, 0, 0, 0
                },
                1u, 1u
            ],
            [
                new byte[]
                {
                    0xFF, 0xFF, 0xFF, 0x7F,
                    0xFF, 0xFF, 0xFF, 0x7F
                },
                (uint)int.MaxValue, (uint)int.MaxValue
            ]
        ];
    }

    [Theory]
    [MemberData(nameof(FileTableIntegrationTestData))]
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
        var fileTable = CreateV1Reader().BuildFileTable(reader, header);

        Assert.Equal((int)numberEntries, fileTable.Count);

        if (fileTable.Count == 0)
            return;

        var record = fileTable[dataToInspect];

        Assert.Equal(crc32, record.Crc32);
        Assert.Equal(fIndex, record.FileTableRecordIndex);
        Assert.Equal(fSize, record.FileSize);
        Assert.Equal(fOffset, record.FileOffset);
        Assert.Equal(nIndex, record.FileNameIndex);
    }

    public static IEnumerable<object[]> FileTableIntegrationTestData()
    {
        return
        [
            [0u, Array.Empty<byte>(), -1, new Crc32(), 0u,0u,0u,0u],
            [
                1u, new byte[]
            {
                1,0,0,0, // CRC
                0,0,0,0, // Index (FileTable)
                2,0,0,0, // Size
                0x40,0,0,0, // Offset
                0,0,0,0  // Index (NameTable)
            }, 0, new Crc32(1), 0u, 2u, 0x40u, 0u
            ],
            [
                1u, new byte[]
            {
                1,0,0,0, // CRC
                0,0,0,0, // Index (FileTable)
                2,0,0,0, // Size
                0x40,0,0,0, // Offset
                0,0,0,0,  // Index (NameTable)

                1,2,3,4 // Junk
            },  0, new Crc32(1), 0u, 2u, 0x40u, 0u
            ],
            [
                2u, new byte[]
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
            }, 1, new Crc32(2), 1u, 3u, 0x80u, 1u
            ]
        ];
    }


    [Theory]
    [MemberData(nameof(NotSupportedFileTableRecords))]
    public void Test__BuildFileTable_NotSupported(uint numberEntries, byte[] data)
    {
        var header = new MegHeader(numberEntries, numberEntries);
        var reader = new BinaryReader(new MemoryStream(data));
        Assert.Throws<NotSupportedException>(() => CreateV1Reader().BuildFileTable(reader, header));
    }

    public static IEnumerable<object[]> NotSupportedFileTableRecords()
    {
        return
        [
            [
                1u, new byte[]
                {
                    1, 0, 0, 0, // CRC
                    0, 0, 0, 0x80, // Index (FileTable)
                    0, 0, 0, 0, // Size
                    0, 0, 0, 0, // Offset
                    0, 0, 0, 0 // Index (NameTable)
                }
            ],
            [
                1u, new byte[]
                {
                    1, 0, 0, 0, // CRC
                    0, 0, 0, 0, // Index (FileTable)
                    2, 0, 0, 0, // Size
                    0, 0, 0, 0, // Offset
                    0, 0, 0, 0x80 // Index (NameTable)
                }
            ]
        ];
    }
}