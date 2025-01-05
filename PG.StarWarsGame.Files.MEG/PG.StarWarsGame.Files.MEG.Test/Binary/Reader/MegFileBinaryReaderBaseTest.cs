using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Reader;

public abstract class MegFileBinaryReaderBaseTest
{
    protected readonly MockFileSystem FileSystem = new();
    protected readonly ServiceProvider ServiceProvider;

    protected MegFileBinaryReaderBaseTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(FileSystem);
        sc.SupportMEG();
        ServiceProvider = sc.BuildServiceProvider();
    }

    private protected abstract IMegFileBinaryReader CreateService();

    [Fact]
    public void Test__Ctor_ThrowsNullArg()
    {
        Assert.Throws<ArgumentNullException>(() => new TestMegFileBinaryReader(null!));
    }

    [Fact]
    public void Test__ReadBinary_ThrowsArgs()
    {
        var service = CreateService();

        Assert.Throws<ArgumentNullException>(() => service.ReadBinary(null!));
        Assert.Throws<ArgumentException>(() => service.ReadBinary(new MemoryStream()));
        Assert.Throws<ArgumentException>(() => service.ReadBinary(new MemoryStream([])));
    }


    [Theory]
    [MemberData(nameof(FileTableTestData))]
    public void Test__ReadBinary_BuildFileNameTable(int fileNumber, byte[] data, string[] expectedValues)
    {
        var service = CreateService();

        var binaryReader = new BinaryReader(new MemoryStream(data));

        var nameTable = service.BuildFileNameTable(binaryReader, fileNumber);

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

    private class TestMegFileBinaryReader(IServiceProvider services)
        : MegFileBinaryReaderBase<IMegFileMetadata, IMegHeader, IMegFileTable>(services)
    {
        protected internal override IMegFileMetadata CreateMegMetadata(IMegHeader header, BinaryTable<MegFileNameTableRecord> fileNameTable, IMegFileTable fileTable)
        {
            throw new NotImplementedException();
        }

        protected internal override IMegHeader BuildMegHeader(BinaryReader binaryReader)
        {
            throw new NotImplementedException();
        }

        protected internal override IMegFileTable BuildFileTable(BinaryReader binaryReader, IMegHeader header)
        {
            throw new NotImplementedException();
        }
    }
}