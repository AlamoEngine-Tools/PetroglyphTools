using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Moq;
using Moq.Protected;
using PG.Commons.Binary;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Test.Binary.Metadata;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Reader;

public class MegFileBinaryReaderBaseTest
{
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly IFileSystem _fileSystem = new MockFileSystem();

    public MegFileBinaryReaderBaseTest()
    {
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(_fileSystem);
        _serviceProviderMock = sp;
    }

    [Fact]
    public void Test__Ctor_ThrowsNullArg()
    {
        Assert.Throws<ArgumentNullException>(() => new TestMegFileBinaryReader(null!));
    }

    [Fact]
    public void Test__ReadBinary_ThrowsArgs()
    {
        var serviceMock = new Mock<MegFileBinaryReaderBase<IMegFileMetadata, IMegHeader, IMegFileTable>>(_serviceProviderMock.Object)
        {
            CallBase = true
        };

        Assert.Throws<ArgumentNullException>(() => serviceMock.Object.ReadBinary(null!));
        Assert.Throws<ArgumentException>(() => serviceMock.Object.ReadBinary(new MemoryStream()));
        Assert.Throws<ArgumentException>(() => serviceMock.Object.ReadBinary(new MemoryStream(Array.Empty<byte>())));
    }

    [Fact]
    public void Test__ReadBinary_MethodChain()
    {
        var serviceMock = new Mock<MegFileBinaryReaderBase<IMegFileMetadata, IMegHeader, IMegFileTable>>(_serviceProviderMock.Object)
        {
            CallBase = true
        };

        var fileNameTable = new BinaryTable<MegFileNameTableRecord>(new List<MegFileNameTableRecord> { MegFileNameTableRecordTest.CreateNameRecord("A") });
        var megHeader = new Mock<IMegHeader>();
        var fileTable = new Mock<IMegFileTable>();


        serviceMock.Protected().Setup<IMegHeader>("BuildMegHeader", ItExpr.IsAny<BinaryReader>())
            .Returns(megHeader.Object);
        serviceMock.Protected().Setup<BinaryTable<MegFileNameTableRecord>>("BuildFileNameTable", ItExpr.IsAny<BinaryReader>(), megHeader.Object)
            .Returns(fileNameTable);
        serviceMock.Protected().Setup<IMegFileTable>("BuildFileTable", ItExpr.IsAny<BinaryReader>(), megHeader.Object)
            .Returns(fileTable.Object);

        serviceMock.Object.ReadBinary(new MemoryStream([0]));

        serviceMock.Protected().Verify("CreateMegMetadata", Times.Once(), megHeader.Object, fileNameTable, fileTable.Object);
        serviceMock.Protected().Verify("BuildMegHeader", Times.Once(), ItExpr.IsAny<BinaryReader>());
        serviceMock.Protected().Verify("BuildFileTable", Times.Once(), ItExpr.IsAny<BinaryReader>(), megHeader.Object);
        serviceMock.Protected().Verify("BuildFileNameTable", Times.Once(), ItExpr.IsAny<BinaryReader>(), megHeader.Object);
    }


    [Fact]
    public void Test__ReadBinary_ThrowOnNullObjects()
    {
        var serviceMock = new Mock<MegFileBinaryReaderBase<IMegFileMetadata, IMegHeader, IMegFileTable>>(_serviceProviderMock.Object)
        {
            CallBase = true
        };

        var fileNameTable = new BinaryTable<MegFileNameTableRecord>(new List<MegFileNameTableRecord> { MegFileNameTableRecordTest.CreateNameRecord("A") });
        var megHeader = new Mock<IMegHeader>();
        var fileTable = new Mock<IMegFileTable>();

        serviceMock.Protected().Setup<IMegHeader>("BuildMegHeader", ItExpr.IsAny<BinaryReader>())
            .Returns((IMegHeader)null!);
        serviceMock.Protected().Setup<BinaryTable<MegFileNameTableRecord>>("BuildFileNameTable", ItExpr.IsAny<BinaryReader>(), megHeader.Object)
            .Returns(fileNameTable);
        serviceMock.Protected().Setup<IMegFileTable>("BuildFileTable", ItExpr.IsAny<BinaryReader>(), megHeader.Object)
            .Returns(fileTable.Object);

        Assert.Throws<InvalidOperationException>(() => serviceMock.Object.ReadBinary(new MemoryStream([0])));

        serviceMock.Protected().Setup<IMegHeader>("BuildMegHeader", ItExpr.IsAny<BinaryReader>())
            .Returns(megHeader.Object);
        serviceMock.Protected().Setup<BinaryTable<MegFileNameTableRecord>>("BuildFileNameTable", ItExpr.IsAny<BinaryReader>(), megHeader.Object)
            .Returns((BinaryTable<MegFileNameTableRecord>)null!);
        serviceMock.Protected().Setup<IMegFileTable>("BuildFileTable", ItExpr.IsAny<BinaryReader>(), megHeader.Object)
            .Returns(fileTable.Object);

        Assert.Throws<InvalidOperationException>(() => serviceMock.Object.ReadBinary(new MemoryStream([0])));

        serviceMock.Protected().Setup<IMegHeader>("BuildMegHeader", ItExpr.IsAny<BinaryReader>())
            .Returns(megHeader.Object);
        serviceMock.Protected().Setup<BinaryTable<MegFileNameTableRecord>>("BuildFileNameTable", ItExpr.IsAny<BinaryReader>(), megHeader.Object)
            .Returns(fileNameTable);
        serviceMock.Protected().Setup<IMegFileTable>("BuildFileTable", ItExpr.IsAny<BinaryReader>(), megHeader.Object)
            .Returns((IMegFileTable)null!);
    }


    [Theory]
    [MemberData(nameof(FileTableTestData))]
    public void Test__ReadBinary_BuildFileNameTable(int fileNumber, byte[] data, string[] expectedValues)
    {
        var serviceMock = new Mock<MegFileBinaryReaderBase<IMegFileMetadata, IMegHeader, IMegFileTable>>(_serviceProviderMock.Object)
        {
            CallBase = true
        };

        var megHeader = new Mock<IMegHeader>();
        megHeader.Setup(h => h.FileNumber).Returns(fileNumber);


        var binaryReader = new BinaryReader(new MemoryStream(data));

        var nameTable = serviceMock.Object.BuildFileNameTable(binaryReader, megHeader.Object);

        var names = nameTable.Select<MegFileNameTableRecord, string>(source => source.FileName).ToList();

        Assert.Equal(fileNumber, nameTable.Count);
        Assert.Equal(expectedValues, names);
    }

    public static IEnumerable<object[]> FileTableTestData()
    {
        return new[]
        {
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
            new object[] { 1, new byte[]
            {
                1, 0, unchecked((byte)'ä')
            }, new[] { "?" } }
        };
    }

    private class TestMegFileBinaryReader : MegFileBinaryReaderBase<IMegFileMetadata, IMegHeader, IMegFileTable>
    {
        public TestMegFileBinaryReader(IServiceProvider services) : base(services)
        {
        }

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