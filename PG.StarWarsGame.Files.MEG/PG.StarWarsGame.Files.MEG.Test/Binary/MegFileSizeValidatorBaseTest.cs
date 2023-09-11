using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.V1.Metadata;

namespace PG.StarWarsGame.Files.MEG.Test.Binary;

[TestClass]
public class MegFileSizeValidatorBaseTest
{
    private Mock<IServiceProvider> _serviceProviderMock = null!;

    [TestInitialize]
    public void SetUp()
    {
        var fs = new MockFileSystem();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);
        _serviceProviderMock = sp;
    }

    [TestMethod]
    public void Test__Validate_MethodChain()
    {
        var serviceMock = new Mock<MegFileSizeValidatorBase<IMegFileMetadata>>(_serviceProviderMock.Object)
        {
            CallBase = true
        };
        var metadata = new Mock<IMegFileMetadata>();
        serviceMock.Object.Validate(1, 12, metadata.Object);
        serviceMock.Protected().Verify("ValidateCore", Times.Once(), 1L, 12L, metadata.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test__Validate_Throws_ArgumentNullException()
    {
        var serviceMock = new Mock<MegFileSizeValidatorBase<IMegFileMetadata>>(_serviceProviderMock.Object)
        {
            CallBase = true
        };
        serviceMock.Object.Validate(1, 12, null!);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidCastException))]
    public void Test__Validate_Throws_InvalidCastException()
    {
        var serviceMock = new Mock<MegFileSizeValidatorBase<MegMetadata>>(_serviceProviderMock.Object)
        {
            CallBase = true
        };
        var metadata = new Mock<IMegFileMetadata>();
        serviceMock.Object.Validate(1, 12, metadata.Object);
    }

    [TestMethod]
    [DataRow(-1L, 0L)]
    [DataRow(0L, -1L)]
    [DataRow(2L, 1L)]
    public void Test__Validate_InvalidCalls(long readBytes, long size)
    {
        var serviceMock = new Mock<MegFileSizeValidatorBase<IMegFileMetadata>>(_serviceProviderMock.Object)
        {
            CallBase = true
        };
        var metadata = new Mock<IMegFileMetadata>();
        Assert.IsFalse(serviceMock.Object.Validate(readBytes, size, metadata.Object));
    }
}