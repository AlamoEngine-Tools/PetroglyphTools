using System;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.StarWarsGame.Files.MEG.Services;

namespace PG.StarWarsGame.Files.MEG.Test.Services;

[TestClass]
public class MegDataStreamFactoryTest
{
    private IServiceProvider _serviceProvider = null!;
    private readonly MockFileSystem _fileSystem = new();

    [TestInitialize]
    public void Init()
    {
        var spMock = new Mock<IServiceProvider>();
        spMock.Setup(s => s.GetService(typeof(IFileSystem))).Returns(_fileSystem);
        _serviceProvider = spMock.Object;
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_Ctor_Throws()
    {
        _ = new MegDataStreamFactory(null!);
    }

    [TestMethod]
    public void Test_CreateDataStream_EmptyDataFile()
    {
        _fileSystem.AddFile("test.meg", new MockFileData(new byte[] { 1, 2 }));

        var factory = new MegDataStreamFactory(_serviceProvider);
        var stream = factory.CreateDataStream("test.meg", 2, 0);
        Assert.AreEqual(0, stream.Length);
    }

    [TestMethod]
    public void Test_CreateDataStream_File()
    {
        _fileSystem.AddFile("test.meg", new MockFileData(new byte[] { 1, 2, 3, 4, 5 }));

        var factory = new MegDataStreamFactory(_serviceProvider);
        var stream = factory.CreateDataStream("test.meg", 1, 2);
        Assert.AreEqual(2, stream.Length);

        var resultStream = new MemoryStream(new byte[2]);
        stream.CopyTo(resultStream);
        CollectionAssert.AreEqual(new byte[] { 2, 3 }, resultStream.ToArray());
    }

    [TestMethod]
    [ExpectedException(typeof(FileNotFoundException))]
    public void Test_CreateDataStream_FileNotFound()
    { 
        var factory = new MegDataStreamFactory(_serviceProvider);
        factory.CreateDataStream("test.meg", 1, 2);
    }
}