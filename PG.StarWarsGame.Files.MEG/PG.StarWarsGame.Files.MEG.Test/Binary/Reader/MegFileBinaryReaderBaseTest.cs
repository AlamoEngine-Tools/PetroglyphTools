using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Reader;

public abstract class MegFileBinaryReaderBaseTest : CommonMegTestBase
{
    private protected abstract IMegFileBinaryReader CreateMegBinaryReader();

    protected abstract byte[] GetValidData();

    [Fact]
    public void ReadBinary_InvalidArgs_Throws()
    {
        var reader = CreateMegBinaryReader();

        Assert.Throws<ArgumentNullException>(() => reader.ReadBinary(null!));
        Assert.Throws<ArgumentException>(() => reader.ReadBinary(new MemoryStream()));
        Assert.Throws<ArgumentException>(() => reader.ReadBinary(new MemoryStream([])));
        Assert.Throws<NotSupportedException>(() => reader.ReadBinary(new MegTestConstants.NonSeekableStream()));
    }

    [Fact]
    public void ReadBinary_ShouldNotDisposeStream()
    {
        var data = GetValidData();
        var stream = new MemoryStream(data);
        var reader = CreateMegBinaryReader();
        reader.ReadBinary(stream);

        // Ensure the stream is not disposed
        stream.Position = 0;
    }
    
    [Theory]
    [MemberData(nameof(FileTableTestData))]
    public void ReadBinary_BuildFileNameTable(int fileNumber, byte[] data, string[] expectedValues)
    {
        var reader = CreateMegBinaryReader();

        var binaryReader = new PetroglyphBinaryReader(new MemoryStream(data), false);

        var nameTable = reader.BuildFileNameTable(binaryReader, fileNumber);

        var names = nameTable.Select(source => source.FileName).ToList();

        Assert.Equal(fileNumber, nameTable.Count);
        Assert.Equal(expectedValues, names);
    }

    public static IEnumerable<object[]> FileTableTestData()
    {
        return
        [
            [
                0, new byte[]
            {
                1, 0, (byte)'A'
            }, new string[] {  }
            ],
            [
                1, new byte[]
            {
                1, 0, (byte)'A'
            }, new[] { "A" }
            ],
            [
                2, new byte[]
            {
                1, 0, (byte)'A',
                1, 0,  (byte) 'B'
            }, new[] { "A", "B" }
            ],
            [
                2, new byte[]
            {
                2, 0, (byte)'A', (byte)'A',
                1, 0,  (byte) 'B'
            }, new[] { "AA", "B" }
            ],
            [
                2, new byte[]
            {
                2, 0, (byte)'A', (byte)'A',
                2, 0,  (byte) 'B', (byte) 'B'
            }, new[] { "AA", "BB" }
            ],

            [
                1, new byte[]
            {
                2, 0, (byte)'A', (byte)'A',
                1, 2, 3, 4, 5, 6 // Random junk
            }, new[] { "AA" }
            ],

            // This case occurs when reading .MEGs from Mike's tool, since it uses Latin1, instead of ASCII.
            [
                1, new byte[]
            {
                1, 0, unchecked((byte)'�')
            }, new[] { "?" }
            ]
        ];
    }

    [Fact]
    public void BuildFileTable_UnsortedCrc_ThrowsBinaryCorruptedException()
    {
        var data = new byte[]
        {
            5, 0, 0, 0, // 5 > 3
            0, 0, 0, 0,
            10, 0, 0, 0,
            100, 0, 0, 0,
            0, 0, 0, 0,

            3, 0, 0, 0, // Unsorted
            1, 0, 0, 0,
            20, 0, 0, 0,
            110, 0, 0, 0,
            1, 0, 0, 0
        };

        var header = new MegHeader(2, 2);
        var binaryReader = new PetroglyphBinaryReader(new MemoryStream(data), false);

        var exception = Assert.Throws<BinaryCorruptedException>(() =>
        {
            var megReader = (dynamic)CreateMegBinaryReader();
            megReader.BuildFileTable(binaryReader, header);
        });
        Assert.Contains("not sorted", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BuildFileTable_IndexMismatch_ThrowsBinaryCorruptedException()
    {
        var data = new byte[]
        {
            1, 0, 0, 0,
            0, 0, 0, 0,
            10, 0, 0, 0,
            100, 0, 0, 0,
            0, 0, 0, 0,

            2, 0, 0, 0,
            5, 0, 0, 0, // Should be 1
            20, 0, 0, 0,
            110, 0, 0, 0,
            1, 0, 0, 0
        };

        var header = new MegHeader(2, 2);
        var binaryReader = new PetroglyphBinaryReader(new MemoryStream(data), false);

        var exception = Assert.Throws<BinaryCorruptedException>(() =>
        {
            var megReader = (dynamic)CreateMegBinaryReader();
            megReader.BuildFileTable(binaryReader, header);
        });
        Assert.Contains("index", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("does not match", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}