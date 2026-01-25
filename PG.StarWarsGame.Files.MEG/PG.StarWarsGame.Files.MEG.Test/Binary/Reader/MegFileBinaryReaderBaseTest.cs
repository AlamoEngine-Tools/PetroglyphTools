using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Binary;
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
                1, 0, unchecked((byte)'ä')
            }, new[] { "?" }
            ]
        ];
    }
}