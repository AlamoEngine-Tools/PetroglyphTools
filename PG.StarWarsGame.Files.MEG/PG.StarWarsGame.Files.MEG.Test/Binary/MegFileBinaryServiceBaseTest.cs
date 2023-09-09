using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;

namespace PG.StarWarsGame.Files.MEG.Test.Binary;

[TestClass]
public class MegFileBinaryServiceBaseTest
{
    private Mock<IServiceProvider> _serviceProviderMock = null!;
    private IFileSystem _fileSystem = null!;

    [TestInitialize]
    public void SetUp()
    {
        _fileSystem = new MockFileSystem();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(_fileSystem);
        _serviceProviderMock = sp;
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test__Ctor_ThrowsNullArg()
    {
        _ = new TestMegFileBinaryService(null!);
    }

    [TestMethod]
    public void Test__ReadBinary_ThrowsArgs()
    {
        var serviceMock = new Mock<MegFileBinaryServiceBase<IMegFileMetadata, IMegHeader, IMegFileTable>>(_serviceProviderMock.Object)
        {
            CallBase = true
        };

        Assert.ThrowsException<ArgumentNullException>(() => serviceMock.Object.ReadBinary(null!));
        Assert.ThrowsException<ArgumentException>(() => serviceMock.Object.ReadBinary(new MemoryStream()));
        serviceMock.Object.Dispose();
        Assert.ThrowsException<ObjectDisposedException>(() => serviceMock.Object.ReadBinary(new MemoryStream(new byte[]{0})));
    }

    [TestMethod]
    public void Test__ReadBinary_MethodChain()
    {
        var serviceMock = new Mock<MegFileBinaryServiceBase<IMegFileMetadata, IMegHeader, IMegFileTable>>(_serviceProviderMock.Object)
        {
            CallBase = true
        };

        var fileNameTable = new MegFileNameTable(new List<MegFileNameTableRecord> { new("A") });
        var megHeader = new Mock<IMegHeader>();
        var fileTable = new Mock<IMegFileTable>();


        serviceMock.Protected().Setup<IMegHeader>("BuildMegHeader", ItExpr.IsAny<BinaryReader>())
            .Returns(megHeader.Object);
        serviceMock.Protected().Setup<MegFileNameTable>("BuildFileNameTable", ItExpr.IsAny<BinaryReader>(), megHeader.Object)
            .Returns(fileNameTable);
        serviceMock.Protected().Setup<IMegFileTable>("BuildFileTable", ItExpr.IsAny<BinaryReader>(), megHeader.Object)
            .Returns(fileTable.Object);

        serviceMock.Object.ReadBinary(new MemoryStream(new byte[]{0}));

        serviceMock.Protected().Verify("CreateMegMetadata", Times.Once(), megHeader.Object, fileNameTable, fileTable.Object);
        serviceMock.Protected().Verify("BuildMegHeader", Times.Once(), ItExpr.IsAny<BinaryReader>());
        serviceMock.Protected().Verify("BuildFileTable", Times.Once(), ItExpr.IsAny<BinaryReader>(), megHeader.Object);
        serviceMock.Protected().Verify("BuildFileNameTable", Times.Once(), ItExpr.IsAny<BinaryReader>(), megHeader.Object);
    }

    [DataTestMethod]
    [DynamicData(nameof(FileTableTestData), DynamicDataSourceType.Method)]
    public void Test__ReadBinary_BuildFileNameTable(int fileNumber, byte[] data, string[] expectedValues)
    {
        var serviceMock = new Mock<MegFileBinaryServiceBase<IMegFileMetadata, IMegHeader, IMegFileTable>>(_serviceProviderMock.Object)
        {
            CallBase = true
        };

        var megHeader = new Mock<IMegHeader>();
        megHeader.Setup(h => h.FileNumber).Returns(fileNumber);
       
        
        var binaryReader = new BinaryReader(new MemoryStream(data));

        var nameTable = serviceMock.Object.BuildFileNameTable(binaryReader, megHeader.Object);

        var names = nameTable.Select<MegFileNameTableRecord, string>(source => source.FileName).ToList();

        Assert.AreEqual(fileNumber, nameTable.Count);
        CollectionAssert.AreEqual(expectedValues, names);
    }

    private static IEnumerable<object[]> FileTableTestData()
    {
        return new[]
        {
            new object[] { 0, new byte[]
            {
                1, 0, (byte)'A'
            }, new string[] {  } },
            new object[] { 1, new byte[]
            {
                1, 0, (byte)'A'
            }, new[] { "A" } },
            new object[] { 2, new byte[] 
            { 
                1, 0, (byte)'A', 
                1, 0,  (byte) 'B'
            }, new[] { "A", "B" } },
            new object[] { 2, new byte[]
            {
                2, 0, (byte)'A', (byte)'A',
                1, 0,  (byte) 'B'
            }, new[] { "AA", "B" } },
            new object[] { 2, new byte[]
            {
                2, 0, (byte)'A', (byte)'A',
                2, 0,  (byte) 'B', (byte) 'B'
            }, new[] { "AA", "BB" } },

            new object[] { 1, new byte[]
            {
                2, 0, (byte)'A', (byte)'A',
                1, 2, 3, 4, 5, 6 // Random junk
            }, new[] { "AA" } },

            // This case occurs when reading .MEGs from Mike's tool, since it uses Latin1, instead of ASCII.
            new object[] { 1, new byte[]
            {
                1, 0, (byte)'ä'
            }, new[] { "?" } }
        };
    }



    private class TestMegFileBinaryService : MegFileBinaryServiceBase<IMegFileMetadata, IMegHeader, IMegFileTable>
    {
        public TestMegFileBinaryService(IServiceProvider services) : base(services)
        {
        }

        protected internal override IMegFileMetadata CreateMegMetadata(IMegHeader header, MegFileNameTable fileNameTable, IMegFileTable fileTable)
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